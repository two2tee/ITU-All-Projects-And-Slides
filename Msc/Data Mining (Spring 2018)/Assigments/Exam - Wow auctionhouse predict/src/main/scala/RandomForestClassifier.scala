import Model.PriceMovementLabel.PriceMovementLabel
import Model.{LabeledAuction, NumericLabelPrediction, PredictionModel, PriceMovementLabel}
import TreeModel._
import org.apache.spark.mllib.tree.RandomForest
import org.apache.spark.mllib.tree.model.RandomForestModel
import org.apache.spark.sql.Dataset

/**
  * Implementation of [[RandomForestClassifier]] interface.
  * Random forests are collection of decision trees, where it combines many
  * decision tree to reduce risk of overfitting.
  *
  * The algorithm trains a set of decision trees separately such that
  * each tree is a bit different.
  *
  * The randomness process that spark uses is as:
  * - it considers a different random subset of features to split on at each tree node
  * - It subsamples the original dataset on each iteration to get
  *   a different training set.
  *
  * Classification of data
  * For the classification or prediction, it takes the majority vote.
  * Each tree's prediction is counted as a vote for one class. The label is predicted
  * is the label or class with most votes
  *
  * For Node impurity measuring, spark is compatible with Gini and entropy
  */
object RandomForestClassifier extends Classifier[RandomForestModel] with SparkConfiguration {
  val baseFilename = "randomForest"
  import sparkSession.implicits._ // For implicit conversions like converting RDDs to DataFrames


  override def train(train: Dataset[LabeledAuction], test: Dataset[LabeledAuction]): PredictionModel[RandomForestModel] = {
    val itemName = DataFetcher.getItemName(train.first().item)
    val trainData = getModel(train)
    val testData = getModel(test)

    //Setup DecisionTree variables
    val numClasses = trainData.categoricalInfo.count(p => true)
    val categoricialFeatures = trainData.categoricalInfo
    val impurity = "gini"
    val maxDepth = 4
    val maxBins = 64
    val numTrees = 128

    //Setup spark decision tree and train it
    println(s"\nTraining Random forest tree for $itemName")
    val model = RandomForest.trainClassifier(trainData.labeledData, numClasses, categoricialFeatures, numTrees,
      "auto", impurity, maxDepth, maxBins)


    val testResult = testData.labeledData.map { point =>
      val prediction = model.predict(point.features)
      (point.label, prediction)
    }

    val labelAndPrediction = testResult.map { point =>
      NumericLabelPrediction(point._1, point._2)
    }
    //Evaluate results
    Evaluator.evaluate(testResult, "Random Forest")

    PredictionModel(labelAndPrediction.toDF(), model)
  }

  /**
    * Predict single auction from prediction model.
    * @param latestAuction Latest auction.
    * @param model Prediction model.
    * @return Price movement label.
    */
  override def predict(latestAuction: LabeledAuction, model: RandomForestModel): PriceMovementLabel = {
    val prediction = model.predict(getVector(latestAuction))
    PriceMovementLabel.getPriceMovement(prediction.toInt)
  }
}