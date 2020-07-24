import org.apache.log4j.{Level, Logger}
import org.apache.spark.sql.SparkSession

/**
  * Trait (interface) used to dependency inject spark session used across program for data processing purposes.
  */
trait SparkConfiguration {

  // Disable logging by default
  Logger.getLogger("org").setLevel(Level.ERROR)

  // def sparkSession: SparkSession
  lazy val sparkSession: SparkSession = SparkSession
    .builder()
    .master("local[*]")
    .appName("Data Mining Project - Auction Price Prediction")
    .getOrCreate()

}
