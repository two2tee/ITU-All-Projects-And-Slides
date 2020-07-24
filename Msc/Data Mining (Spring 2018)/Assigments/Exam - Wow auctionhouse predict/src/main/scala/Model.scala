import java.sql.Timestamp

import AuctionReader.sparkSession
import Model.{LabeledAuction, PriceMovementLabel}
import org.apache.spark.mllib.linalg.{Vector, Vectors}
import org.apache.spark.mllib.regression.LabeledPoint
import org.apache.spark.rdd.RDD
import org.apache.spark.sql.{DataFrame, Dataset, Encoders}


/**
  * Base Model used to represent the auction house data.
  * Case class is used to define the structure of data schema in the auction data set.
  */
object Model {

  object PriceMovementLabel extends Enumeration {
    type PriceMovementLabel = Value
    val priceMovementThreshold = 0.98 // Less than 2% price movement -> Stationary
    val Up, Down, Stationary = Value
    def toString(priceMovementLabel: PriceMovementLabel): String = priceMovementLabel match {
      case Up => "Up"
      case Down => "Down"
      case Stationary => "Stationary"
    }

    def getPriceMovement(value: String): PriceMovementLabel = value match {
      case "Up" => Up
      case "Down" => Down
      case _ => Stationary
    }

    def getPriceMovement(numericValue: Int): PriceMovementLabel = numericValue match {
      case 0 => Up
      case 1 => Down
      case _ => Stationary
    }

    def getNumericValue(label: PriceMovementLabel): Int = label match {
      case Up => 0
      case Down => 1
      case _ => 2
    }

    def toString(label: Int): String = toString(getPriceMovement(label))

    def getNumericValueFromString(label: String): Int = getNumericValue(getPriceMovement(label))

    def getMovement(currentMeanBuyout: Double,futureMeanBuyout: Double): PriceMovementLabel = {
      if(futureMeanBuyout > currentMeanBuyout) {
        if(currentMeanBuyout / futureMeanBuyout > priceMovementThreshold) PriceMovementLabel.Stationary
        else PriceMovementLabel.Up
      } else {
        if(futureMeanBuyout / currentMeanBuyout > priceMovementThreshold) PriceMovementLabel.Stationary
        else PriceMovementLabel.Down
      }
    }
  }

  // Different label representations (0=Up, 1=Down, 2=Stationary)
  case class NumericLabelPrediction(label: Double, prediction: Double)
  case class LabelPrediction(label: String, prediction: String)

  case class Auction(date: Option[String],
                     auc: Integer,
                     item: Integer,
                     owner: String,
                     ownerRealm: String,
                     bid: Integer,
                     buyout: Integer,
                     quantity: Integer,
                     timeLeft: String)

  case class PreprocessedAuction(timestamp:Option[Timestamp],
                                 date: Option[String],
                                 dayOfWeek: Option[Int],
                                 morning: Option[Boolean],
                                 evening: Option[Boolean],
                                 auc: Int,
                                 item: Int,
                                 owner: String,
                                 ownerRealm: String,
                                 bid: Double,
                                 buyout: Double,
                                 quantity: Double,
                                 timeLeft: String)

  // Model representing most volatile item
  case class VolatileAuction(item: Int, minMeanBuyout: Double, maxMeanBuyout: Double, count: Int, minMaxRatio: Double)

  // Auction with basic statistics
  case class SummaryAuction(date: Option[String],
                            auc: Int,
                            item: Int,
                            owner: String,
                            ownerRealm: String,
                            bid: Double,
                            buyout: Double,
                            quantity: Double,
                            timeLeft: String,
                            minBuyout: Double,
                            minBid: Double,
                            maxBuyout: Double,
                            maxBid: Double,
                            totalItemAuctions: Double)

  // Auction model with price movement label
  case class LabeledAuction(item: Int,
                            date: String,
                            meanBuyout: Double,
                            dayOfWeek: Int,
                            meanBid: Double,
                            totalQuantity: Double,
                            minBuyout: Double,
                            minBid: Double,
                            morning: Boolean,
                            evening: Boolean,
                            weekend: Boolean,
                            tomorrowWeekend: Boolean,
                            previousPriceMovementLabel: Int,
                            priceMovementLabel: String)

  // Labels and ML model
  case class PredictionModel[A](labels: DataFrame, model: A)

  // Model used to hold information gained from cross validation (model, accuracy and summary)
  case class ModelInfo[A](minAccuracy: Double, maxAccuracy: Double, bestModel: Option[A], summary: String)

  case class PricePrediction(item: Int, previousPriceMovementLabel: Int, prediction: Double)

  case class Suggestion(item: String, yesterday: String, tomorrow: String, suggestion: String)

  // Item transactions used for frequency pattern mining
  case class ItemTransaction(items: List[Int])

}

/**
  * Models for kmeans
  */
object KMeansModel{
  // Point represents feature vector with three variables (k=3) used for classification
  case class Point(itemBid: Double, itemBuyout: Double) {
    // Euclidean distance: dist(p1,p2) = | p1 - p2 | =  sqrt ( (p1.x - p2.x)^2 + (p1.y - p2.y)^2 )
    def distanceTo(that: Point) : Double = {
      Math.sqrt(Math.pow(this.itemBid - that.itemBid, 2) +
        Math.pow(this.itemBuyout - that.itemBuyout, 2))
    }
  }
  // A cluster is a group of similar data points defined by a k centroid (center)
  case class Cluster(centroid: Point, members: List[Point])
}

/**
  * Models used by random forests and decision trees
  */
object TreeModel{
  case class TreeForestModel(itemId: Int,
                             labeledData: RDD[LabeledPoint], //Data used to train model
                             categoricalInfo: Map[Int,Int]) //denotes which class in vector is categorical and how many values they have

  case class DecisionVector(meanBid: Double,meanBuyout: Double,tomorrowWeekend: Boolean,weekend: Boolean,morning: Boolean,evening: Boolean,
                            previousPriceMovementLabel: Int)

  def DecisionVectorIndexOf(fieldName:String): Int = Encoders.product[DecisionVector].schema.fieldIndex(fieldName)

  /**
    * Convert [[LabeledAuction]] dataset to [[LabeledPoint]] dataset used to train decision tree.
    *
    * @param dataset [[LabeledPoint]] dataset of preprocessed data.
    * @return DecisionTreeDataModel with labeled data
    */
  def getModel(dataset: Dataset[LabeledAuction]): TreeForestModel = {
    val data = dataset.collect()
    val binaryDecision = 2
    val numberOfPriceLabels = 3

    //Denotes which class in vector is categorical and how many values it has eg: class 3 has 2 values : 3 -> 2
    val categoricalInf = Map[Int,Int](
      DecisionVectorIndexOf("tomorrowWeekend") -> binaryDecision,
      DecisionVectorIndexOf("weekend") -> binaryDecision,
      DecisionVectorIndexOf("morning") -> binaryDecision,
      DecisionVectorIndexOf("evening") -> binaryDecision,
      DecisionVectorIndexOf("previousPriceMovementLabel") -> numberOfPriceLabels
    )

    val points = data.map(
      row => {
        val label = PriceMovementLabel.getNumericValueFromString(row.priceMovementLabel)
        val features = getVector(row)
        LabeledPoint(label,features)
      }
    )
    val itemId = dataset.first().item

    TreeForestModel(itemId,sparkSession.sparkContext.parallelize(points),categoricalInf)
  }

  //Help function to create vector for decision tree
  def getVector(data: LabeledAuction): Vector ={
    Vectors.dense(Array(
      data.meanBid,
      data.meanBuyout,
      toBinary(data.tomorrowWeekend),
      toBinary(data.weekend),
      toBinary(data.morning),
      toBinary(data.evening),
      data.previousPriceMovementLabel
    ))
  }

  //Convert boolean values to Binary
  def toBinary(input: Boolean): Double = input match {
    case true => 1.0
    case false => 0.0
  }
}