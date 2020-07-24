import Model.PriceMovementLabel.PriceMovementLabel
import Model.{LabeledAuction, PredictionModel, PriceMovementLabel}
import org.apache.spark.ml.classification.MultilayerPerceptronClassifier
import org.apache.spark.ml.evaluation.MulticlassClassificationEvaluator
import org.apache.spark.ml.feature.{StringIndexer, VectorAssembler}
import org.apache.spark.ml.{Pipeline, PipelineModel}
import org.apache.spark.sql.Dataset

/**
  * Trait (interface) represents an artificial neural network.
  */
trait ArtificialNeuralNetwork extends Classifier[PipelineModel] {

}

/**
  * Implementation of [[ArtificialNeuralNetwork]].
  *
  * Configuration:
  *   8 input nodes: "meanBuyout","meanBid","quantity", "minBuyout", "minBid", "morning", "evening", "weekend"
  *   5 hidden nodes
  *   3 output nodes: PriceMovementLabel either Stationary, Up or Down
  *   NB: could be expected buyout price, expected bid price, and expected quantity
  */
object ArtificialNeuralNetwork extends ArtificialNeuralNetwork with SparkConfiguration {

  import sparkSession.implicits._

  /**
    * Train classifier on auction data used to predict price movements.
    * Auction features are used to train the classifier algorithm.
    *
    * Constructs multi layer network used to classify auction price movements from trained data.
    * Steps:
    * 1) Configure layers
    * 2) Convert labels to numbers
    * 3) Build features array and represent it as feature vector for fast matrix computations
    * 4) Build trainer and pipeline
    * 5) Fit test data using trained model
    * 6) Measure accuracy
    *
    * @param train Training data set used to make prediction model.
    * @param test  Test data set used to predict/label.
    * @return Prediction Model used to predict price movements on new auction data.
    */
  def train(train: Dataset[LabeledAuction], test: Dataset[LabeledAuction]): PredictionModel[PipelineModel] = {
    // Create an int array that will contain the count for the various attributes needed by our model:
    // Attribute value at index
    // 0: Number of layers at input layer of network => count of features passed to model
    // 1: hidden layer containing perceptrons (sigmoid neurons)
    // 2: another hidden layer containing sigmoid neurons.
    // 3: number of neurons representing the output label classes => three types of PriceMovement, hence three classes.
    val layers = Array(6, 12, 3)

    // Convert string labels to numbers for classification
    val labelIndexer = new StringIndexer().setInputCol("priceMovementLabel").setOutputCol("label") // predicted is label

    // Build the features array. These would be the features that we use when training our model:
    val featuresArr = Array("meanBuyout", "meanBid", "morning", "evening", "weekend", "previousPriceMovementLabel")

    // Build a features vector as this needs to be fed to our model.
    // To put the feature in vector form, we use the VectorAssembler class from the Spark ML library.
    // We also provide a features array as input and provide the output column where the vector array will be printed
    val va = new VectorAssembler().setInputCols(featuresArr).setOutputCol("features")

    // Build the multi-layer perceptron model that is bundled within the Spark ML library.
    // To this model we supply the array of layers we created earlier.
    // This layer array has the number of neurons (sigmoid neurons) that are needed in each layer of the multi-perceptron network:
    val trainer = new MultilayerPerceptronClassifier()
      .setLayers(layers)
      .setBlockSize(128) // Block size for putting input data in matrices for faster computation. Default 128.
      .setSeed(1234L) // Seed for weight initialization if weights are not set.
      //.setMaxIter(200) // Maximum number of iterations to be performed on the dataset while learning. The default value is 100.
      //.setStepSize(1E-5) // Set quantity by which weights are modified (learning rate)
      //.setTol(1E-10)

    // Hook all the workflow pieces together using the pipeline API.
    // To this pipeline API, we pass the different pieces of the workflow, that is, the labelindexer and vector assembler, and finally provide the model
    val pipeline = new Pipeline().setStages(Array(labelIndexer, va, trainer)) //(PipelineStage(labelIndexer, va, trainer))

    // Once our pipeline object is ready, we fit the model on the training dataset to train our model on the underlying training data:
    val model = pipeline.fit(train)

    // Once the model is trained, it is not yet ready to be run on the test data to figure out its predictions.
    // For this, we invoke the transform method on our model and store the result in a Dataset object

    val result = model.transform(test)
    println("NEURAL NETWORK RESULTS: ")

    // The last column depicts the predictions made by our model.
    // After making the predictions, let’s now check the accuracy of our model.
    // For this, we will first select two columns in our model which represent the predicted label,
    // as well as the actual label (recall that the actual label is the output of our StringIndexer):

    val predictionAndLabels = result.select("prediction", "label")

    // Use MulticlassClassificationEvaluator in Spark for checking the accuracy of the models.
    // Set the metric name of the metric, that is, accuracy, for which we want to get the value from our predicted results
    val evaluator = new MulticlassClassificationEvaluator()
      .setMetricName("accuracy")

    // Next, using the instance of this evaluator, invoke the evaluate method and pass the parameter of the dataset that contains the column
    // for the actual result and predicted result (in our case, it is the predictionAndLabels column):
    // 1 means that our model is 100% accurate, proving that neural networks yields a very high accuracy
    val accuracy = evaluator.evaluate(predictionAndLabels)
    println(s"Test set accuracy = $accuracy")

    // Store fitted models
    val filePath = System.getProperty("user.dir") + "/data/predictionmodel.json"
    if(accuracy >= 0.95) model.write.overwrite().save(filePath)

    PredictionModel(predictionAndLabels, model)
  }

  /**
    * Predict single auction from prediction model.
    * @param latestAuction Latest auction.
    * @param model Prediction model.
    * @return Price movement label.
    */
  override def predict(latestAuction: LabeledAuction, model: PipelineModel): PriceMovementLabel = {
    val prediction = model.transform(Seq(latestAuction).toDS)
    val numericPrediction = prediction.select("prediction").first().getDouble(0).toInt
    PriceMovementLabel.getPriceMovement(numericPrediction)
  }

}
