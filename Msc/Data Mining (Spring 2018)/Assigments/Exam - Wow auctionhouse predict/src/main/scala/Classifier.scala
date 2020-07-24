import Model.PriceMovementLabel.PriceMovementLabel
import Model._
import org.apache.spark.sql.Dataset

/**
  * Trait (interface) responsible for defining the functionality used across all classifiers (e.g. ANN and decision tree).
  * @tparam Model Classification model used for prediction.
  */
trait Classifier[Model] extends SparkConfiguration {

  import sparkSession.implicits._

  /**
    * Train classifier on auction data used to predict price movements.
    * Auction features are used to train the classifier algorithm.
    * @param train Training data set used to make prediction model.
    * @param test Test data set used to predict/label.
    * @return Prediction Model used to predict price movements on new auction data.
    */
  def train(train: Dataset[LabeledAuction], test: Dataset[LabeledAuction]): PredictionModel[Model]

  def predict(latestAuction: LabeledAuction, model: Model): PriceMovementLabel

  override def toString: String = {
    this.getClass.getName
  }

  /**
    * Provides a Suggestion whether to buy, sell or keep an auctioned item based on its predicted price movement.
    * @param auctionToPredict Auction that is predicted.
    * @param model Model used for prediction.
    * @return Suggestion of whether to buy, sell, or keep a given auction item.
    */
  def suggestActionFromLatestAuction(auctionToPredict: LabeledAuction, model: Model): Suggestion = {
    val prediction = predict(auctionToPredict, model)
    val itemName = DataFetcher.getItemName(auctionToPredict.item).getOrElse("Unknown")
    val yesterday = PriceMovementLabel.toString(auctionToPredict.previousPriceMovementLabel)
    val tomorrow = PriceMovementLabel.toString(prediction)
    val suggestion = prediction match {
      case PriceMovementLabel.Up => "BUY"
      case PriceMovementLabel.Down => "SELL"
      case PriceMovementLabel.Stationary => "KEEP"
    }

    Suggestion(itemName, yesterday, tomorrow , suggestion)
  }

  /**
    * Takes a range of auctioned items and suggests whether to buy, sell, or keep them.
    * @param preprocessedAuctions Set of auctions.
    * @param items Items to suggest actions for.
    * @return Suggestions denoting whether to buy, sell, or keep items.
    */
  def suggestActionsFromItems(preprocessedAuctions: Dataset[PreprocessedAuction], items: List[Int]) : Dataset[Suggestion] = {

    items.map(item => {

      // Retrieve item information
      val itemName = DataFetcher.getItemName(item)
      val (classificationData, latestAuction) = AuctionPreprocessor.getAveragedAuctionsByDay(preprocessedAuctions, item)
      classificationData.cache()
      println(s"Total classification data points: ${classificationData.count()}")
      //      classificationData.groupBy("priceMovementLabel").count.show() // Distribution of price movements

      // Get best prediction model
      val bestPredictionModel = Evaluator.crossValidation(classificationData, itemName, this)

      //Validate by predicting expected price movement of item tomorrow
      val suggestion = suggestActionFromLatestAuction(latestAuction, bestPredictionModel)

      suggestion
    }).toDS()
  }

}

