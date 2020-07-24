
import java.nio.file.{Files, Paths}

import Model.{Auction, PreprocessedAuction, VolatileAuction}
import org.apache.spark.sql.{Dataset, Encoders, SaveMode}

/**
  * Trait (interface) responsible for reading/loading [[Model.Auction]].
  */
sealed trait AuctionReader {

  /**
    * Loads raw auction data located at provided path.
    * Convert raw data into type safe, strongly typed, immutable data set.
    * @param path Path of csv file
    * @param isCached True if data should be loaded from local cache.
    * @return an encoded type safe data set [[Model.Auction]] containing all the auction house records.
    */
  def readAuctions(path: String, isCached: Boolean) : (Dataset[PreprocessedAuction], Dataset[VolatileAuction])

}

/**
  * Implementation of [[AuctionReader]] responsible for reading auction house data from a JSON file.
  */
object AuctionReader extends AuctionReader with SparkConfiguration {

  import sparkSession.implicits._ // For implicit conversions like converting RDDs to DataFrames

  /**
    * Loads raw JSON auction data located at provided path.
    * Convert raw data into type safe, strongly typed, immutable data set.
    * NB: Spark cannot convert JSON-arrays to records directly: http://jsonlines.org/
    * @param fileName The name of the JSON file to be read.
    * @param useCache True if data should be loaded from local cache.
    * @return An encoded (type safe) [[Model.Auction]] data set containing all the auction house records.
    */
  override def readAuctions(fileName: String, useCache: Boolean = true) : (Dataset[PreprocessedAuction], Dataset[VolatileAuction]) = {

    val basePath = System.getProperty("user.dir")

    // Fetch auction data online if local file does not exist
    if(!Files.exists(Paths.get(s"$basePath/$fileName"))) {
      println("Data file not found - Fetching from server...")
      val url = "http://www.appnow.dk/api/datamining/fetch.php"
      if(DataFetcher.isConnected(url)) {
        DataFetcher.downloadAuctionJSON(url, "data.json", shouldDownload = true)
        println("Data downloaded")
      }
      else {
        println("NO INTERNET CONNECTION: please connect to download latest auctions...")
        System.exit(1)
      }
    }

    // Fetch cached preprocessed auctions if stored locally
    val preprocessedAuctions =
      // Read preprocessed auctions from local cache
      if(useCache && Files.exists(Paths.get(s"$basePath/data/preprocessed.json")) && Files.exists(Paths.get(s"$basePath/data/volatile.json"))) {
        println("LOADING AUCTIONS FROM CACHE...")
        val preprocessedSchema = Encoders.product[PreprocessedAuction].schema
        val volatileItemSchema = Encoders.product[VolatileAuction].schema
        val preprocessedAuctions = sparkSession.read.schema(preprocessedSchema).json(s"$basePath/data/preprocessed.json").as[PreprocessedAuction]
        val mostVolatileAuctions = sparkSession.read.schema(volatileItemSchema).json(s"$basePath/data/volatile.json").as[VolatileAuction]
        println(s"${preprocessedAuctions.count} AUCTIONS LOADED FROM CACHE...")
        (preprocessedAuctions, mostVolatileAuctions)
      }

      // Otherwise load and preprocess raw auctions
      else {
        println("LOADING AND CLEANING RAW DATA...")
        val schema = Encoders.product[Auction].schema
        val auctionsDataFrame = sparkSession.read.schema(schema).option("header", "true").json(s"$basePath/$fileName").as[Auction]
        val (preprocessedAuctions, mostVolatileAuctions) = AuctionPreprocessor.preprocess(auctionsDataFrame)

        preprocessedAuctions.write.mode(SaveMode.Overwrite).format("json").save(s"$basePath/data/preprocessed.json") // Cache preprocessed auctions
        mostVolatileAuctions.write.mode(SaveMode.Overwrite).format("json").save(s"$basePath/data/volatile.json") // Cache most volatile auctions
        println(s"${preprocessedAuctions.count} AUCTIONS LOADED...")
        (preprocessedAuctions, mostVolatileAuctions)
      }

    preprocessedAuctions
  }
}

