import Model._
import Utils._
/**
  * Main entry of auction prediction program used to load data and run data mining algorithms on the data.
  * Price movements are predicted by following a train, test, validation model in the classification algorithms.
  */
object AuctionApp extends SparkConfiguration { // For implicit conversions like converting RDDs to DataFrames

  import sparkSession.implicits._

  def main(args: Array[String]) {

    // Read and load preprocessed auctions
    val (preprocessedAuctions, mostVolatileItems) = AuctionReader.readAuctions("data/data.json")

    // Frequent pattern mining
    runAprioriTestsForDifferentTimes(preprocessedAuctions)

    // Pick supervised classifier
    val ANNclassifier = ClassifyFactory.getANNClassifier

    // Split data to training and test
    val (classificationData, latestAuction) = AuctionPreprocessor.getAveragedAuctionsByDay(preprocessedAuctions, 2589)
    val Array(trainingData, testData) = classificationData.randomSplit(Array(0.7, 0.3),1234L)
    classificationData.filter(_.item == 2589).groupBy("priceMovementLabel").count.show() // Distribution of price movements
    trainingData.show()
    testData.show()

    // Train classifier for single prediction
    val PredictionModel(labels, model) = ANNclassifier.train(trainingData, testData)
    val predictedPriceMovement = ANNclassifier.predict(latestAuction, model)
    println(predictedPriceMovement)


    // Suggest actions based on most volatile auctions
    val volatileItems = mostVolatileItems.map(auc => auc.item).take(10).toList
    val ANNsuggestions = ANNclassifier.suggestActionsFromItems(preprocessedAuctions, volatileItems)
    ANNsuggestions.show(truncate = false)


    val DecisionClassifier = ClassifyFactory.getDecisionTreeClassifier
    val DecisionSuggestions = DecisionClassifier.suggestActionsFromItems(preprocessedAuctions, volatileItems)
    DecisionSuggestions.show(truncate = false)

    val RandomClassifier = ClassifyFactory.getRandomForestClassifier
    val RandomSuggestions = RandomClassifier.suggestActionsFromItems(preprocessedAuctions, volatileItems)
    RandomSuggestions.show(truncate = false)

    sparkSession.close()

  }

}


