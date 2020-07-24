import Model.{LabeledAuction, NumericLabelPrediction, PredictionModel, PriceMovementLabel}
import TreeModel._
import org.apache.spark.mllib.tree.DecisionTree
import org.apache.spark.mllib.tree.model.DecisionTreeModel
import org.apache.spark.sql.Dataset


/**
  * Implementation of [[DecisionTreeClassifier]] interface.
  *
  *  Decisions tree (DT) is a powerful method for classification and prediction.
  *  A DT is made up of two components:
  *  - Decision
  *  - Outcome (Label)
  *
  *  A DT includes three types of nodes:
  *  - Root Node : Top node with all the data
  *  - Splitting node : Node that assigns data to a subgroup of data
  *  - Terminal node : the final decision (label)
  *
  *  For each node a decision is made which travels to the next child node until a terminal node is reached
  *
  * This implementation utilizes sparks decision tree which is
  * We use a single tree for the classification but technically a random forest algorithm is used under the
  * the hood, but we only set a single tree in the forest.
  *
  * Spark Decision tree is a greedy algorithm that uses recursive binary partitioning of a given feature space.
  * Each partition is chosen greedily by selecting the best split from a set of possible splits to
  * maximise information gain at give node.
  *
  * For Node impurity measuring, spark is compatible with Gini and entropy
  */
object DecisionTreeClassifier extends Classifier[DecisionTreeModel] with SparkConfiguration {
  val baseFilename = "decisionTreeModel"

  import sparkSession.implicits._ // For implicit conversions like converting RDDs to DataFrames

  override def train(train: Dataset[LabeledAuction], test: Dataset[LabeledAuction]): PredictionModel[DecisionTreeModel] = {

    val itemName = DataFetcher.getItemName(train.first().item)
    val trainData = getModel(train)
    val testData = getModel(test)

    //Setup DecisionTree variables
    val numClasses = 3 //Number of classification classes (eg: up, down, station = 3 classes)

    //Denotes which features are categorical and how many categorical values each feature take
    val categoricalFeatures = trainData.categoricalInfo

    val impurity = "gini"

    //Max depth of tree
    val maxDepth = 1

    //Number of bins used when discretizing continuous features
    //Increase allow algorithm to consider more split candidates ==> fine-grained split decisions in cost of speed
    //NOTICE: Minimum must be at least maximum number of categorical features
    val maxBins = 64

    //Setup spark decision tree and train it
    println(s"\nTraining decision tree for $itemName")

    val model = DecisionTree.trainClassifier(trainData.labeledData, numClasses, categoricalFeatures,
      impurity, maxDepth, maxBins)

    //test model with test data
    val testResult = testData.labeledData.map { point =>
      val prediction = model.predict(point.features)
      (point.label, prediction)
    }

    val labelAndPrediction = testResult.map { point =>
      NumericLabelPrediction(point._1, point._2)
    }

    //Evaluate results
    Evaluator.evaluate(testResult, "Decision Tree")

    PredictionModel(labelAndPrediction.toDF(), model)
  }

  /**
    * Predict auction based on decision tree model.
    * @param latestAuction Latest auction predicted.
    * @param model Model for prediction.
    * @return Price movement label.
    */
  override def predict(latestAuction: LabeledAuction, model: DecisionTreeModel): PriceMovementLabel.Value = {
    val prediction = model.predict(getVector(latestAuction))
    PriceMovementLabel.getPriceMovement(prediction.toInt)
  }
}


