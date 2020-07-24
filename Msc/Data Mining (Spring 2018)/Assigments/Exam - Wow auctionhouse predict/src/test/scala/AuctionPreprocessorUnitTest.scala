import breeze.linalg.split
import org.apache.spark.ml.feature.{MinMaxScaler, VectorAssembler}
import org.apache.spark.ml.stat._
import org.apache.spark.sql.Row
import org.apache.spark.ml.feature.StandardScaler
import org.apache.spark.ml.linalg.Vectors
import org.scalatest.FlatSpec
import org.apache.spark.sql.functions._

/**
  * [[AuctionPreprocessor]] Unit Test Suite used to test logic functionality of the preprocessor.
  * This includes testing the logic used to detect outliers and normalize the auction data set.
  *
  * Implements FlatSpec trait that facilitates a "behavior-driven" style of development (BDD).
  * Tests are combined with text that specifies the behavior the tests verify.
  * Link: http://doc.scalatest.org/1.8/org/scalatest/FlatSpec.html
  */
class AuctionPreprocessorUnitTest extends FlatSpec with SparkConfiguration {

  import sparkSession.implicits._

  behavior of "AuctionPreprocessor"

  it should "use quantiles to measure central tendency and identify outliers" in {

    // Arrange
    val sourceDF = Seq(
      (10, 20, 1, 2592),
      (10, 20, 1, 2592),
      (10, 20, 1, 2592),
      (20, 40, 1, 2592),
      (1000, 1000, 1, 2592), // Outlier
      (40, 80, 1, 1008),
      (50, 100, 1, 1008),
      (1000, 1000, 1, 1008) // Outlier
    )toDF("bid", "buyout", "quantity", "item")

    val expectedOutliers = sourceDF.filter(s"buyout >= 1000")

    // Act
    val quantiles = sourceDF.stat.approxQuantile("buyout", Array(0.25,0.75), 0.0)
    val Q1 = quantiles(0)
    val Q3 = quantiles(1)
    val IQR = Q3 - Q1

    val lowerRange = Q1 - 1.5 * IQR
    val upperRange = Q3 + 1.5 * IQR
    val actualOutliers = sourceDF.filter(s"buyout < $lowerRange or buyout > $upperRange")

    // Assert
    assert(expectedOutliers.intersect(actualOutliers).count == 2) // Identify 2 outliers
  }

  it should "normalize feature values to range between 0 and 1 using minmax normalizatipn" in {

    // Arrange
    val sourceDF = Seq(
      (10.0, 20.0, 20.0, 2592),
      (20.0, 40.0, 1.0, 2592),
//      (1000.0, 1000.0, 1.0, 2592),
      (40.0, 80.0, 1.0, 1008),
      (50.0, 100.0, 1.0, 1008)
      //(1000.0, 1000.0, 1.0, 1008)
    ).toDF("bid", "buyout", "quantity", "item")

    val assembler = new VectorAssembler()
      .setInputCols(Array("bid", "buyout", "quantity"))
      .setOutputCol("features")

    val featureDF = assembler.transform(sourceDF)
    featureDF.show(20, false)

    // Standardize features by scaling to unit variance and/or removing mean
    // Improve convergence rate and prevent large feature variance bias
//    val scaler = new StandardScaler()
//      .setWithMean(true)
//      .setWithStd(true)
//      .setInputCol("features")
//      .setOutputCol("scaledFeatures")

    val scaler = new MinMaxScaler()
      .setInputCol("features")
      .setOutputCol("scaledFeatures")
      .setMax(1.0)
      .setMin(0.0)

    // Compute summary statistics by fitting StandardScaler
    val scalerModel = scaler.fit(featureDF)

    // Normalize each feature to have unit standard deviation
    val scaledData = scalerModel.transform(featureDF)
    scaledData.show(20, false)

    val groupByItems = sourceDF
      .groupBy($"item")
      .agg(stddev_pop($"buyout").name("stdBuyout"))

    groupByItems.show(20, false)

    // Pick most promising item (most fluctuating/volatile)
    val item = groupByItems
      .sort($"stdBuyout".desc)
      .first().getInt(0)
    println(item)

    // TODO: max - min may be zero in which normalization should be 1 / length
    // https://stats.stackexchange.com/questions/70801/how-to-normalize-data-to-0-1-range
//    val groupByItems = sourceDF
//      .groupBy($"item")
//      .agg(min($"bid").name("minBid"),
//           max($"bid").name("maxBid"))
//    val joined = sourceDF.join(groupByItems, "item")
//    def normalizedFunc (value: Double, min: Double, max: Double) = (value - min ) / (max - min)
//    val normalized = joined.withColumn("normBid", ( $"bid" - $"minBid" ) / ( $"maxBid" - $"minBid" ) )


    assert(true)
  }

}
