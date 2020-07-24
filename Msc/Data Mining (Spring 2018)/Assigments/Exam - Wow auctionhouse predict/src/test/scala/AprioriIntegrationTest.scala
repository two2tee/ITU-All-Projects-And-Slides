import org.apache.spark.ml.fpm.FPGrowth
import org.scalatest.FlatSpec

/**
  * Apriori Integration Test Suite used to test the Apriori algorithm on real auction house data.
  * The association rules are compared with the results gained from using the official Spark ML FP-growth algorithm.
  */
class AprioriIntegrationTest extends FlatSpec with SparkConfiguration {

  // Spark Library
  import sparkSession.implicits._

  behavior of "Apriori"

  it should "find the correct amount of association rules in a local database of transactions" in {
    // Arrange
    val fpg = new FPGrowth().setMinSupport(0.5).setMinConfidence(0.7)
    val transactions = sparkSession.createDataset(Seq(
      "bread milk",
      "bread diaper beer eggs",
      "milk diaper beer coke",
      "bread milk diaper beer",
      "bread milk diaper coke"
    )).map(_.split(" ")).toDF("items")
    val transactionsArray = transactions.collect().map(row => row.getSeq[String](0).toArray.map(_.toString))

    // Act
    val actualRules = Apriori.run[String](transactionsArray, minSupport = 0.5, minConfidence = 0.7)
    val model = fpg.fit(transactions)
    val expectedLibraryRulesCount = model.associationRules.count.toInt

    sparkSession.close()

    // Assert
    assert(actualRules.size == expectedLibraryRulesCount)

  }

  it should "find the correct amount of association rules in a database of auction transactions" in {
    // Arrange
    val filePath = System.getProperty("user.dir") + "/data/data.json"
    val dataSet = AuctionReader.readAuctions(filePath, false)
    val (preprocessedData, _) = AuctionPreprocessor.preprocess(dataSet)
    val transactions = AuctionPreprocessor.getItemTransactions(preprocessedData)
    val transactionsDataSet = transactions.map(_.items.toArray.map(_.toString)).toDF("items")
    val transactionsArray = transactions.collect().map(_.items.toArray)
    val fpg = new FPGrowth().setMinSupport(0.01).setMinConfidence(0.5)

    // Act
    val actualRules = Apriori.run[Int](transactionsArray, minSupport = 0.01, minConfidence = 0.5)
    val model = fpg.fit(transactionsDataSet)
    val libraryRulesCount = model.associationRules.count.toInt

    sparkSession.close()

    // Assert
    assert(actualRules.size == libraryRulesCount)

  }

}
