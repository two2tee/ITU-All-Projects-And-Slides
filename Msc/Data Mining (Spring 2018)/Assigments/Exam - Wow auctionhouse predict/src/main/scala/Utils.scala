import java.sql.Timestamp
import java.time.DayOfWeek

import Model.PreprocessedAuction
import org.apache.commons.lang.StringUtils
import org.apache.spark.sql.{DataFrame, Dataset, SaveMode}

/**
  * This contains helper functions used to display results from mining of auction data.
  */
object Utils {

  /**
    * Displays a Spark DataFrame as a pretty printed string.
    * Credits: https://stackoverflow.com/questions/45741035/is-there-any-way-to-get-the-output-of-sparks-dataset-show-method-as-a-string
    *
    *
    * @param df       Input dataframe.
    * @param _numRows Amount of rows in data.
    * @param truncate Amount of characters used to display column values.
    * @return Pretty printed data.
    */
  def prettyPrintData(df: DataFrame, _numRows: Int = 20, truncate: Int = 20): String = {
    val numRows = _numRows.max(0)
    val takeResult = df.take(numRows + 1)
    val hasMoreData = takeResult.length > numRows
    val data = takeResult.take(numRows)

    // For array values, replace Seq and Array with square brackets
    // For cells that are beyond `truncate` characters, replace it with the
    // first `truncate-3` and "..."
    val rows: Seq[Seq[String]] = df.schema.fieldNames.toSeq +: data.map { row =>
      row.toSeq.map { cell =>
        val str = cell match {
          case null => "null"
          case binary: Array[Byte] => binary.map("%02X".format(_)).mkString("[", " ", "]")
          case array: Array[_] => array.mkString("[", ", ", "]")
          case seq: Seq[_] => seq.mkString("[", ", ", "]")
          case _ => cell.toString
        }
        if (truncate > 0 && str.length > truncate) {
          // do not show ellipses for strings shorter than 4 characters.
          if (truncate < 4) str.substring(0, truncate)
          else str.substring(0, truncate - 3) + "..."
        } else {
          str
        }
      }: Seq[String]
    }

    val sb = new StringBuilder
    val numCols = df.schema.fieldNames.length

    // Initialise the width of each column to a minimum value of '3'
    val colWidths = Array.fill(numCols)(3)

    // Compute the width of each column
    for (row <- rows) {
      for ((cell, i) <- row.zipWithIndex) {
        colWidths(i) = math.max(colWidths(i), cell.length)
      }
    }

    // Create SeparateLine
    val sep: String = colWidths.map("-" * _).addString(sb, "+", "+", "+\n").toString()

    // column names
    rows.head.zipWithIndex.map { case (cell, i) =>
      if (truncate > 0) {
        StringUtils.leftPad(cell, colWidths(i))
      } else {
        StringUtils.rightPad(cell, colWidths(i))
      }
    }.addString(sb, "|", "|", "|\n")

    sb.append(sep)

    // data
    rows.tail.map {
      _.zipWithIndex.map { case (cell, i) =>
        if (truncate > 0) {
          StringUtils.leftPad(cell.toString, colWidths(i))
        } else {
          StringUtils.rightPad(cell.toString, colWidths(i))
        }
      }.addString(sb, "|", "|", "|\n")
    }

    sb.append(sep)

    // For Data that has more than "numRows" records
    if (hasMoreData) {
      val rowsString = if (numRows == 1) "row" else "rows"
      sb.append(s"only showing top $numRows $rowsString\n")
    }

    sb.toString()
  }

  // Extract item transactions and run apriori to find any association rules
  def runApriori(inputAuctions: Dataset[PreprocessedAuction]): Unit = {
    println("Running Apriori...")
    val itemTransactions = AuctionPreprocessor.getItemTransactions(inputAuctions).collect().map(_.items.toArray)
    val associationRules = Apriori.run(itemTransactions, minConfidence = 0.6)
    val rulesByName = associationRules.map(rule => rule.copy(LHS = rule.LHS.map(i => DataFetcher.getItemName(i).getOrElse("Unknown")),
      RHS = rule.RHS.map(i => DataFetcher.getItemName(i).getOrElse("Unknown"))))
    Apriori.printAssociationRules(rulesByName)
  }

  // Run Apriori on different months
  def runAprioriTestsForDifferentTimes(preprocessedAuctions: Dataset[PreprocessedAuction]) = {
    println("Testing weekends")
    runApriori(preprocessedAuctions.filter("dayOfWeek > "+DayOfWeek.FRIDAY.getValue))
    println("Testing weekdays")
    runApriori(preprocessedAuctions.filter("dayOfWeek <= "+DayOfWeek.FRIDAY.getValue))

    // Define months
    val april = Timestamp.valueOf("2018-04-01 00:00:00")
    val may = Timestamp.valueOf("2018-05-01 00:00:00")
    val june = Timestamp.valueOf("2018-06-01 00:00:00")

    def filterAuctionDatesBetween(from:Timestamp,to:Timestamp) =
      preprocessedAuctions.filter(auc => auc.timestamp.get.after(from)).filter(auc => auc.timestamp.get.before(to))

    println("Testing April")
    runApriori(filterAuctionDatesBetween(april,may))
    println("Testing May")
    runApriori(filterAuctionDatesBetween(may,june))
    println("Testing All")
    runApriori(preprocessedAuctions)
  }


  // Save data to CSV for visualization
  def saveData(data: DataFrame, format: String, name: String): Unit = {
    val basePath = System.getProperty("user.dir")
    data.coalesce(1).write.mode(SaveMode.Overwrite)
      .format(format)
      .option("header", "true")
      .save(s"$basePath/data/$name.csv")

    println(s"Saved data $name to $format")
  }

}
