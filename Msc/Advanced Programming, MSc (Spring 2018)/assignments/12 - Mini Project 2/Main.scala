// Advanced Programming. Andrzej Wasowski. IT University
// To execute this example, run "sbt run" or "sbt test" in the root dir of the project
// Spark needs not to be installed (sbt takes care of it)

import org.apache.spark.ml.Pipeline
import org.apache.spark.ml.classification._
import org.apache.spark.ml.evaluation.{BinaryClassificationEvaluator, MulticlassClassificationEvaluator}
import org.apache.spark.ml.feature._
import org.apache.spark.ml.linalg.Vectors
import org.apache.spark.ml.tuning.CrossValidator
import org.apache.spark.mllib.evaluation.MulticlassMetrics
import org.apache.spark.sql.types._
import org.apache.spark.sql.{Dataset, Row, SparkSession}

object Main {

	case class Embedding(word: String, vector: List[Double])
	case class ParsedReview(id: Int, text: String, label: Double)
	case class LabelledReview(id: Int, text: String, label: Int)
	case class TokenizedReview(id: Int, text: String, label: Int, words: List[String])
	case class FlattenedReview(id: Int, label: Int, word: String)
	case class ReviewVectorPair(id: Int, vector: List[Double])
	case class SentimentVector(label: Int, features: List[Double])

	org.apache.log4j.Logger getLogger  "org"  setLevel (org.apache.log4j.Level.WARN)
	org.apache.log4j.Logger getLogger "akka" setLevel (org.apache.log4j.Level.WARN)

	val spark =  SparkSession.builder
		.appName ("Sentiment")
		.master  ("local[*]")
		.getOrCreate

  import spark.implicits._

	val reviewSchema = StructType(Array(
			StructField ("reviewText", StringType, nullable=false),
			StructField ("overall",    DoubleType, nullable=false),
			StructField ("summary",    StringType, nullable=false)))

	// Read file and merge the text abd summary into a single text column

	def loadReviews (path: String): Dataset[ParsedReview] =
		spark
			.read
			.schema (reviewSchema)
			.json (path)
			.rdd
			.zipWithUniqueId
			.map[(Int,String,Double)] { case (row,id) => (id.toInt, s"${row getString 2} ${row getString 0}", row getDouble 1) }
			.toDS
			.withColumnRenamed ("_1", "id" )
			.withColumnRenamed ("_2", "text")
			.withColumnRenamed ("_3", "label")
			.as[ParsedReview]

  // Load the GLoVe embeddings file

  def loadGlove (path: String): Dataset[Embedding] =
		spark
			.read
			.text (path)
      .map  { _ getString 0 split " " }
      .map  (r => (r.head, r.tail.toList.map (_.toDouble))) // yuck!
			.withColumnRenamed ("_1", "word" )
			.withColumnRenamed ("_2", "vector")
			.as[Embedding]

	// Helper function used to add and average two vectors
	def addVectors (vec1: List[Double], vec2: List[Double]) =
		vec1.zip (vec2).map(t => (t._1+t._2) / vec1.size)

	// Deep learning workflow: load data => preprocess => train => evaluate
  def main(args: Array[String]) = {

		// Step 1: Load review data
    val glove  = loadGlove ("data/glove.6B/glove.6B.50d.txt") //("data/glove.6B/glove.6B.50d.txt")
    val reviews = loadReviews ("data/reviews_Musical_Instruments_5.json") //("data/reviews_Musical_Instruments_5.json") //("data/reviews_Video_Games_5.json") //

		// See distribution of reviews (e.g. games is skewed towards 5.0 positive star reviews)
		reviews.groupBy("label").count.orderBy("label").show()

		// Step 2: Preprocess reviews
		val labelledReviews = reviews.map(rev => rev.label match { // Five class labels mapped to three
			case 1.0 | 2.0 => LabelledReview(rev.id, rev.text, 0) // Negative
			case 3.0  => LabelledReview(rev.id, rev.text, 1) // Neutral
			case 4.0 | 5.0 => LabelledReview(rev.id, rev.text, 2) // Positive
			case _ => LabelledReview(rev.id, rev.text, -1) // NA
		}).as[LabelledReview]

		// Tokenize (i.e. split) text into words and remove full stops and commas
		val regexTokenizer = new RegexTokenizer()
  			.setPattern("[a-z-A-Z]+")
  			.setGaps(false)
  			.setInputCol("text")
  			.setOutputCol("words")

		val tokenizedReviews = regexTokenizer.transform(labelledReviews).as[TokenizedReview]

		// Remove stop words involving low information (e.g. 'a', 'and', 'the')
		val remover = new StopWordsRemover()
			.setInputCol(regexTokenizer.getOutputCol)
			.setCaseSensitive(false)

		val removedStopWords = remover.transform(tokenizedReviews).as[TokenizedReview]

		// Flatten words into separate word entries
		val flattenedWords = removedStopWords
  		.flatMap {
				case TokenizedReview(id, text, label, words) => words.map(word => (id, label, word)) // Break text into words
			}.toDF("id", "label", "word").as[FlattenedReview]

		// NLP Word Embedding: Translate review text into vector of numbers (word embedding captures meaning of word as vector of numbers)
		val vectorizedWords = flattenedWords
			.join(glove, "word") // Ignores words not found in GLoVe corpus
  		.drop($"word")

		// Compute average vector of review by summing all vectors for given review and dividing by number of vectors summed
		val averagedReviewVectors = vectorizedWords
			.select($"id", $"vector")
			.as[ReviewVectorPair]
  		.rdd.map (pair => (pair.id, pair.vector))
			.reduceByKey(addVectors).toDF("id", "features")

		val sentimentVectors = averagedReviewVectors
  			.join(labelledReviews, "id")
  			.drop("text")
  			.drop("id")
  			.toDF("features", "label")
  			.as[SentimentVector]
  			.map(labelFeaturesPair => LabeledPoint(labelFeaturesPair.label, Vectors.dense(labelFeaturesPair.features.toArray)))
  			.as[LabeledPoint]
		crossValidation(sentimentVectors)
		/*
		// Step 3: split reviews into training and test to enable 10-fold cross validation
		val Array(train, test) = sentimentVectors.randomSplit(Array(0.9, 0.1), seed = 1234L)
		train.cache() // Cache training data set across iterations


		val predictionAndLabels = runANN(train,test)

		// Step 5: Validate accuracy of model on test data
		// Measure accuracy, recall, precision, false positives and false negatives (variance)

		val falseNeg = predictionAndLabels.filter("prediction!=0 and label==0").count
		val falseNeu = predictionAndLabels.filter("prediction!=1 and label==1").count
		val falsePos = predictionAndLabels.filter("prediction!=2 and label==2").count
		println(s"False Negative: $falseNeg, False Neutral: $falseNeu, False Positive: $falsePos")



*/
		spark.stop
  }

	def crossValidation(sentimentVectors: Dataset[LabeledPoint]): Unit ={
		val start = List.range(1, 10)
		start.foreach( s =>{
			val start = s/10.0
			val Array(train, test,train2) = sentimentVectors.randomSplit(Array(start-0.1,0.1, 1.0-start))
			runANN(train.union(train2),test)
		})
	}
	def runANN(train: Dataset[LabeledPoint], test: Dataset[LabeledPoint]) = {
		// Configure neural network
		// Input layer of size 50 (word embedding vector feature dimensions)
		// Two hidden layers of size 5 and 4 and
		// Output layer of size 3 (classes)
		val layers = Array(50, 5, 4, 3)

		// Create trainer and set parameters
		val trainer = new MultilayerPerceptronClassifier()
			.setLayers(layers)
			.setTol(1E-6) //Set the convergence tolerance of iterations. Smaller value will lead to higher accuracy with the cost of more iterations. Default is 1E-4.
			.setBlockSize(128)
			.setSeed(1234L)
			.setMaxIter(100)

		// Construct pipeline
		//	val pipeline = new Pipeline().setStages(Array(regexTokenizer, remover, trainer))

		// Step 4: Train model
		val model = trainer.fit(train)
		val predictions = model.transform(test)

		val predictionAndLabels = predictions.select("prediction", "label")
		val evaluator = new MulticlassClassificationEvaluator()
		println(s"Accuracy: ${evaluator.setMetricName("accuracy").evaluate(predictionAndLabels)}")
		println(s"Recall: ${evaluator.setMetricName("weightedRecall").evaluate(predictionAndLabels)}")
		println(s"Precision: ${evaluator.setMetricName("weightedPrecision").evaluate(predictionAndLabels)}")
		predictionAndLabels
	}

	def nativeSparkMl() = {

		// Step 1: Load review data
		val reviews = loadReviews ("data/reviews_Video_Games_5.json")  //"data/reviews_Musical_Instruments_5.json")

		// Step 2: Preprocess reviews
		val labelledReviews = reviews.map(rev => rev.label match { // Five class labels mapped to three
			case 1.0 | 2.0 => LabelledReview(rev.id, rev.text, 0) // Negative
			case 3.0  => LabelledReview(rev.id, rev.text, 1) // Neutral
			case 4.0 | 5.0 => LabelledReview(rev.id, rev.text, 2) // Positive
			case _ => LabelledReview(rev.id, rev.text, -1) // NA
		}).as[LabelledReview]

		// Step 3: split reviews into training and test to enable 10-fold cross validation
		val Array(train, test) = labelledReviews.randomSplit(Array(0.9, 0.1), seed = 1234L)
		train.cache() // Cache training data set across iterations

		// Step 4: Configure an ML pipeline with stages/steps used to transform the data before reviews are fed to ML algorithm
		// Pipeline stages: tokenize => remove => hashingTF => idf (NLP optimized data) => feed to ML algorithm

		// Tokenize (i.e. split) text into words and remove full stops and commas
		val regexTokenizer = new RegexTokenizer()
			.setPattern("[a-z-A-Z]+")
			.setGaps(false)
			.setInputCol("text")
			.setOutputCol("words")

		// Remove stop words involving low information (e.g. 'a', 'and', 'the')
		val remover = new StopWordsRemover()
			.setInputCol(regexTokenizer.getOutputCol)
  		.setOutputCol("stopwords")
			.setCaseSensitive(false)

		// Create 2-gram words
		// Develop word features using bigram tokens in feature space instead of unigrams only (an n-gram is a sequence of n tokens (words))
		val bigram = new NGram().setN(2)
			.setInputCol(remover.getOutputCol)
			.setOutputCol("bigram")

		// Convert single word terms into feature vectors
		val hashingTFUnigram = new HashingTF()
			.setNumFeatures(50000)
			.setInputCol(remover.getOutputCol)
			.setOutputCol("rawFeatures")

		// Convert 2-gram words into feature vectors
		val hashingTFBigram = new HashingTF()
			.setNumFeatures(50000)
			.setInputCol(bigram.getOutputCol)
			.setOutputCol("rawBigramFeatures")

		// Combine both feature vectors
		val assembler = new VectorAssembler()
			.setInputCols(Array(hashingTFUnigram.getOutputCol, hashingTFBigram.getOutputCol))

		// Scale feature vectors
		val idf = new IDF()
			.setInputCol(hashingTFUnigram.getOutputCol)
			.setOutputCol("features")
			.setMinDocFreq(0)

		// TODO: Word stemming
		// Reduce words to their root word or stem to enable grouping of words with same meaning
		// This NLP technique helps increase frequency of word and its information value
		// val stemmer = new PorterStemmer()

		// ML algorithms
		val logisticRegression = new LogisticRegression().setRegParam(0.01).setThreshold(0.5) // 89% accuracy
		val naiveBayes = new NaiveBayes() // Predict based on probability of word occurrences (85% accuracy)
		//val decisionTree = new DecisionTreeClassifier().setLabelCol(labelIndexer.getOutputCol).setFeaturesCol(featureIndexer.getOutputCol)

		// Pipelines
		val logisticPipeline = new Pipeline().setStages(Array(regexTokenizer, remover, hashingTFUnigram, idf, logisticRegression))
		val naivePipeline = new Pipeline().setStages(Array(regexTokenizer, remover, hashingTFUnigram, idf, naiveBayes))
		//val decisionTreePipeline = new Pipeline().setStages(Array(regexTokenizer, remover, hashingTFUnigram, idf, labelIndexer, featureIndexer, decisionTree, labelConverter))

		// TODO: parameter tuning: https://spark.apache.org/docs/latest/ml-tuning.html
		// Parameter tuning using grid to improve accuracy

		// Train model
		val model = logisticPipeline.fit(train)
		val predictions = model.transform(test)

		// Summary statistics
		val evaluator = new MulticlassClassificationEvaluator().setMetricName("accuracy")
		val predictionAndLabels = predictions.select("prediction", "label")
		println(s"Accuracy: ${evaluator.evaluate(predictionAndLabels)}")

		spark.stop()
	}

}





