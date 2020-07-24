
import java.sql.Timestamp
import java.time.DayOfWeek
import java.time.format.DateTimeFormatter

import Model.{PriceMovementLabel, _}
import org.apache.spark.sql.{Dataset, SaveMode}
import org.apache.spark.sql.functions.{collect_list, _}

/**
  * Trait (interface) responsible for pre processing [[Auction]] to obtain quality data.
  *
  * Preprocessor is used for data pre processing in the auction house data.
  * The data is incomplete (lack attribute values), noisy (containing outliers or errors), and inconsistent (discrepancies in values)
  * Tasks in data pre processing
  *   - Data cleaning: fill in missing values, smooth noisy data, identify or remove outliers, and resolve inconsistencies.
  *   - Data integration: using multiple databases, data cubes, or files.
  *   - Data transformation: normalization and aggregation.
  *   - Data reduction: reducing the volume but producing the same or similar analytical results.
  *   - Data discretization: part of data reduction, replacing numerical attributes with nominal ones.
  *
  */
sealed trait AuctionPreprocessor {

  /**
    * Reduce the volume of data by removing columns that are not used for further analysis
    * @param dataSet Dirty [[Auction]] data set to reduce.
    * @return Reduced [[Auction]] data set.
    */
  def reduce (dataSet: Dataset[Auction]) : Dataset[Auction]

  /**
    * Clean [[Auction]] data by filling in missing values, smoothing noisy data, removing outliers, and resolving inconsistencies.
    * @param dataSet Dirty [[Auction]] data set to clean.
    * @return Cleaned [[Auction]] data set.
    */
  def clean (dataSet: Dataset[Auction]) : Dataset[Auction]

  /**
    * Normalize [[Auction]] data by adjusting the data to be consistent
    * @param dataSet Dirty [[Auction]] data set.
    * @return Normalized [[Auction]] data set.
    */
  def normalize (dataSet: Dataset[Auction]) : Dataset[PreprocessedAuction]

  /**
    * Complete preprocessing of [[Auction]] data used to obtain quality data for analysis.
    * @param dataSet dirty [[Auction]] data set.
    * @return preprocessed [[Auction]] data set.
    */
  def preprocess(dataSet: Dataset[Auction]) : (Dataset[PreprocessedAuction], Dataset[VolatileAuction])

}

/**
  * Implementation of [[AuctionPreprocessor]] responsible for preprocessing [[Auction]] data.
  *
  * @author Thor Olesen
  */
object AuctionPreprocessor extends AuctionPreprocessor with SparkConfiguration {

  import sparkSession.implicits._ // For implicit conversions like converting RDDs to DataFrames

  /**
    * Reduce the volume of data by removing columns that are not used for further analysis
    *
    * @param dataSet Dirty [[Auction]] data set to reduce.
    * @return Reduced [[Auction]] data set.
    */
  override def reduce(dataSet: Dataset[Auction]): Dataset[Auction] = {
    val reducedAuctions = dataSet
      .drop("_corrupt_record") // Nested outer JSON not supported in Spark
      .drop("name") // Server name
      .drop("slug") // Server ID
      .drop("bonusLists") // Item upgrades
      .drop("context") // Dungeon id
      .drop("modifiers")
      .drop("petBreedId") // Ignore pet info
      .drop("petLevel")
      .drop("petQualityId")
      .drop("petSpeciesId")
      .drop("rand") // Item stat boost
      .drop("seed") // Seed used to calculate stat boost of item
    reducedAuctions.as[Auction]
  }

  def calculateMostVolatile(dataset: Dataset[Auction]): Dataset[VolatileAuction] = {
    println("Finding most volatile item...")
    // Find most frequent auction item for prediction
    val dates = dataset.select("date").distinct.count()
    val tolerance = 200*dates; //minimum amount of auctions per day
    val mostVolatile = dataset
      .groupBy("item")
      .agg(
        min("buyout").name("minMeanBuyout"),
        max("buyout").name("maxMeanBuyout"),
        count("item").cast("int").name("count")
      )
      .filter("count>"+tolerance)
      .withColumn("minMaxRatio",expr("minMeanBuyout/maxMeanBuyout"))
      .sort("minMaxRatio")
    println(s"Found most volatile item: $mostVolatile")
    mostVolatile.as[VolatileAuction]
  }
  /**
    * Clean [[Auction]] data by filling in missing values, smoothing noisy data, removing outliers, and resolving inconsistencies.
    *
    * @param dataSet Dirty [[Auction]] data set to clean.
    * @return Cleaned [[Auction]] data set.
    */
    override def clean(dataSet: Dataset[Auction]): Dataset[Auction] = {

      // Remove empty auctions
      val filteredAuctions = dataSet.filter(auction => auction.auc != null)

      // Remove missing values
      val nonEmptyAuctions = filteredAuctions.filter(auction => auction.productIterator.forall( {
        case null => false
        case _ => true
      }))

      //remove auctions with only bid
      val onlyBuyoutAuctions = nonEmptyAuctions.filter(_.buyout > 0)

      //signalize the auctions so that we can always expect quantity to be 1
      val singleQuantityAuctions = onlyBuyoutAuctions.map(auc => auc.copy(bid = auc.bid/auc.quantity,buyout = auc.buyout/auc.quantity,quantity = 1))

      // Identify and remove outliers
      val filteredBuyOutliers = quartileOutlierDetection(singleQuantityAuctions, "buyout")

      filteredBuyOutliers
    }

  // Helper function uses quantiles to measure central tendency and spread of data
  // Quantiles are used to identify outliers as a more robust normalization technique compared to mean, stdev or minmax
  private def quartileOutlierDetection(dataSet: Dataset[Auction], column: String): Dataset[Auction] = {
    dataSet.createOrReplaceTempView("df") //create sql table

    //run a query finding the quantiles for each item
    val withQuartiles = sparkSession.sql(s"select item, percentile_approx($column,0.75) as Q3," +
                    s"percentile_approx($column,0.25) as Q1 from df group by item")
    val withIQR = withQuartiles.withColumn("IQR",expr("Q3-Q1"))
    //join with the dataSet
    val combined = dataSet.join(withIQR, "item")
      .withColumn("lowerBound",expr("Q1 - 1.5 * IQR"))
      .withColumn("upperBound",expr("Q3 + 1.5 * IQR"))

    //filter the auctions not within the range.
    combined.filter(s"$column > lowerBound and $column < upperBound").as[Auction]
  }

  /**
    * Normalize [[Auction]] data by adjusting the data to be consistent.
    *
    * @param dataSet Dirty [[Auction]] data set.
    * @return Normalized [[Auction]] data set.
    */
  override def normalize(dataSet: Dataset[Auction]): Dataset[PreprocessedAuction] = {
    val summary = dataSet
      .groupBy("item")
      .agg(
        min("Bid").name("minBid"),
        min("Buyout").name("minBuyout"),
        max("Bid").name("maxBid"),
        max("Buyout").name("maxBuyout"),
        count("item").as("totalItemAuctions")
      )

    val combined = dataSet.join(summary, "item").as[SummaryAuction]

    val normalized = combined.map(auc => auc.copy(
      bid = normalize(auc.bid, auc.minBid, auc.maxBid, auc.totalItemAuctions),
      buyout = normalize(auc.buyout, auc.minBuyout, auc.maxBuyout, auc.totalItemAuctions)
    ))
    val mappedToPreprocessedAuctions =  normalized.map(auction => PreprocessedAuction(timestamp = auction.date.map(Timestamp.valueOf),
        date = auction.date.map(Timestamp.valueOf(_).toLocalDateTime.format(DateTimeFormatter.ofPattern("MM-dd"))),
        dayOfWeek = auction.date.map(Timestamp.valueOf(_).toLocalDateTime.getDayOfWeek.getValue),
        morning = auction.date.map(Timestamp.valueOf(_).toLocalDateTime.getHour < 12),
        evening = auction.date.map(Timestamp.valueOf(_).toLocalDateTime.getHour > 12),
        auc = auction.auc, item = auction.item, owner = auction.owner, ownerRealm = auction.ownerRealm,
        bid = auction.bid/auction.quantity, buyout = auction.buyout/auction.quantity, quantity = 1, timeLeft = auction.timeLeft))

    mappedToPreprocessedAuctions.as[PreprocessedAuction]
  }

  // Helper function used to normalize input values to have same range of values between 0 and 1 (minmax)
  // This is useful in ANN to promote stable/faster convergence of weights and biases
  private def normalize(value: Double, min: Double, max: Double, totalAuctionsWithItem: Double) : Double = {
    // Check if all auctions of item have the same value so normalize to 1 / length (#total item auctions)
    if(max - min == 0) 1 / totalAuctionsWithItem
    else (value - min) / (max - min)
  }

  // Helper function used to transform normalized output back to original readable form
  private def denormalize(normalized: Double, min: Double, max: Double) : Double = normalized * (max - min) + min

  /**
    * Complete preprocessing of [[Auction]] data used to obtain quality data for analysis.
    *
    * @param dataSet dirty [[Auction]] data set.
    * @return preprocessed [[PreprocessedAuction]] data set.
    */
  override def preprocess(dataSet: Dataset[Auction]): (Dataset[PreprocessedAuction], Dataset[VolatileAuction]) = {
    val reducedData = reduce(dataSet)
    val cleanedData = clean(reducedData)
    val mostVolatileItems = calculateMostVolatile(cleanedData)
    val normalizedData = normalize(cleanedData)
    Utils.saveData(normalizedData.select("item", "bid", "buyout","owner"),"csv","NormalizedQuartile25Percent")

    println{s"PREPROCESSED DOWN TO ${cleanedData.count} AUCTIONS..."}
    (normalizedData, mostVolatileItems)
  }

  /**
    * Returns an array item transactions to be used by the [[Apriori]] algorithm.
    * The transactions are stored in an Array[Array[Int] and represents sequences of auctions grouped by owner.
    * @param dataset [[PreprocessedAuction]] data set.
    * @return sequence of auction items.
    */
  def getItemTransactions(dataset: Dataset[PreprocessedAuction]): Dataset[ItemTransaction] = {

    val itemsByOwner = dataset
      .groupBy("owner")
      .agg(collect_list("item")
      .as("items"))

    val itemTransactions = itemsByOwner
      .drop("owner")
      .as[ItemTransaction]
      .map(transaction => transaction.copy(items = transaction.items.distinct)) // Items in transaction are unique

    itemTransactions
  }

  /**
    * Fit [[PreprocessedAuction]] data to [[ArtificialNeuralNetwork]] prediction model.
    * This is done by adding averaged numeric values (bid, buyout, quantity) and day of auctions.
    *
    * @param dataset Data set of auctions.
    * @return [[LabeledAuction]] model used for prediction in [[ArtificialNeuralNetwork]].
    */
  def getAveragedAuctionsByDay(dataset: Dataset[PreprocessedAuction], itemID: Int): (Dataset[LabeledAuction],LabeledAuction) = {

    println(s"Filtering for: ${DataFetcher.getItemName(itemID)}")
    val auctionsById = dataset.filter(auc => auc.item == itemID)

    val averagedAuctions = auctionsById
      .groupBy("date", "item", "evening", "morning", "dayOfWeek")
      .agg(
        mean("buyout").name("meanBuyout"),
        mean("bid").name("meanBid"),
        min("buyout").name("minBuyout"),
        min("bid").name("minBid"),
        sum("quantity").name("totalQuantity"), //sum instead of mean, mean("quantity") does not provide us any relevant information
        stddev_pop("buyout").name("stdBuyout")
      )
    println(s"MOST VOLATILE ITEM WITH ID $itemID: ${DataFetcher.getItemName(itemID)} and ${auctionsById.count} AUCTIONS...")

    val auctionsByDay = averagedAuctions
      .withColumn("weekend", expr("dayOfWeek > " + DayOfWeek.FRIDAY.getValue))
      .withColumn("tomorrowWeekend", expr("dayOfWeek == " + DayOfWeek.FRIDAY.getValue))
      .withColumn("priceMovementLabel",lit(PriceMovementLabel.toString(PriceMovementLabel.Stationary)))
      .withColumn("previousPriceMovementLabel",lit(PriceMovementLabel.getNumericValue(PriceMovementLabel.Stationary)))
    val predictionModel = auctionsByDay.as[LabeledAuction].sort( "date","morning")

    // Label auctions by their price movement
    val labelFoldResult = {
      predictionModel
        .collect() // (list, currentAuction)
        .foldLeft ((List[LabeledAuction](), predictionModel.first())) ((auctionsCurrentPairAcc, nextAuction) => {
          val (auctionsAcc, currentAuction) = auctionsCurrentPairAcc
          val priceMovement = PriceMovementLabel.getMovement(currentAuction.meanBuyout, nextAuction.meanBuyout)
          val labelledAuction = currentAuction.copy(
            priceMovementLabel = PriceMovementLabel.toString(priceMovement)
          )
          val updatedNextAuction = nextAuction.copy(
            previousPriceMovementLabel = PriceMovementLabel.getNumericValueFromString(labelledAuction.priceMovementLabel)
          )
          (labelledAuction :: auctionsAcc, updatedNextAuction)
      })
    }

    val newestAuction = labelFoldResult._2 //the auctions for today
    val labelledPredictions = labelFoldResult._1.reverse.drop(1).toDS() // Reverse result and drop first element

    println(s"${labelledPredictions.count} AUCTIONS LABELLED FOR PREDICTION...")

    (labelledPredictions,newestAuction)
  }

  // Round number with a given decimal precision set to 4 by default
  private def roundNumber (number: Double, precision: Int = 4): Double = {
    val s = math pow (10, precision)
    (math round number * s) / s
  }

}



