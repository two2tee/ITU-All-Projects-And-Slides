import java.io.PrintWriter

import Model._
import org.apache.spark.ml.evaluation.MulticlassClassificationEvaluator
import org.apache.spark.mllib.evaluation.MulticlassMetrics
import org.apache.spark.rdd.RDD
import org.apache.spark.sql.Dataset

/**
  * Trait (interface) responsible for evaluating the prediction model.
  *
  * It uses the cross validation technique by partitioning the original sample into a training set to train the auction prediction model, and a test set to evaluate it.
  * Namely, k-fold cross validation is used in which the original sample is partitioned into k equal size subsamples.
  */
sealed trait Evaluator {


  /**
    * Partitions original [[LabeledAuction]] data sample into a training set and test set using k partitions.
    *
    * @param labeledAuctions Auctions with price movement labels.
    * @param itemName Item for which predictions are made.
    * @param kPartitions Amount of partitions.
    * @return Best ML model based on highest accuracy.
    */
  def crossValidation[Model](labeledAuctions: Dataset[LabeledAuction], itemName: Option[String],
                             classifier: Classifier[Model],
                             kPartitions: Int = 5): Model

}

object Evaluator extends Evaluator with SparkConfiguration {

  import sparkSession.implicits._

  // 10 fold cross validation by default: https://www.openml.org/a/estimation-procedures/1
  def crossValidation[Model](labeledAuctions: Dataset[LabeledAuction], itemName: Option[String],
                             classifier: Classifier[Model],
                             kPartitions: Int = 10): Model = {

    val evaluator = new MulticlassClassificationEvaluator()
    val totalSize = 100
    val partitionSize = totalSize / kPartitions
    val partitions = List.range(partitionSize, totalSize + 1, partitionSize) // Not inclusive so increment

    val modelInfo = partitions.foldRight(ModelInfo(1.0, -1.0, None: Option[Model], ""))((position, modelAcc) => {
      val stepSize = 1.0 / kPartitions
      val round = s"CROSS VALIDATION PERCENTAGE: $position \n"
      val start = position.toDouble / totalSize
      val Array(leftTrainSubset, test, rightTrainSubset) =
        labeledAuctions.randomSplit(Array(Math.max(start - stepSize, 0.0), stepSize, totalSize / totalSize - start), 1234L)
      val trainingPartition = leftTrainSubset.union(rightTrainSubset)
      trainingPartition.cache()

      val trainingLabelDistribution = trainingPartition.groupBy("priceMovementLabel").count
      val trainDistributionInfo = "TRAINING SET LABEL DISTRIBUTION \n" + Utils.prettyPrintData(trainingLabelDistribution) + "\n"

      val testLabelDistribution = test.groupBy("priceMovementLabel").count
      val testDistributionInfo = "TEST SET LABEL DISTRIBUTION \n" + Utils.prettyPrintData(testLabelDistribution) + "\n"

      val PredictionModel(labels, newModel) = classifier.train(trainingPartition, test)

      val predictionLabels = labels.as[NumericLabelPrediction]
        .map(labelPrediction => LabelPrediction(PriceMovementLabel.toString(labelPrediction.label.toInt),
          PriceMovementLabel.toString(labelPrediction.prediction.toInt)))
      val predictionInfo = Utils.prettyPrintData(predictionLabels.toDF())

      val accuracy = evaluator.setMetricName("accuracy").evaluate(labels)
      val accuracyInfo = s"ACCURACY: $accuracy \n \n"

      val minAccuracy = Math.min(modelAcc.minAccuracy, accuracy)
      val bestAccuracy = Math.max(modelAcc.maxAccuracy, accuracy)
      val newBestModel = if (modelAcc.maxAccuracy > accuracy) modelAcc.bestModel else Some(newModel)
      val summary = Seq(round, trainDistributionInfo, testDistributionInfo, predictionInfo, accuracyInfo).mkString

      ModelInfo[Model](minAccuracy, bestAccuracy, newBestModel, summary + modelAcc.summary)
    })

    val maxAccuracy = modelInfo.maxAccuracy
    val minAccuracy = modelInfo.minAccuracy
    val variance = maxAccuracy - minAccuracy

    // Save prediction model info to txt file
    new PrintWriter(s"./data/crossvalidation_${classifier.getClass.getName}_${itemName.getOrElse("Unknown")}.txt") {
      write(modelInfo.summary)
      write(s"Variance: $variance, minAccuracy: $minAccuracy, maxAccuracy: $maxAccuracy \n")
      close()
    }

    // Return best model
    modelInfo.bestModel.get
  }

  /**
    * Run different cross validation partition sizes to validate accuracy depending on size of training and test set.
    *
    * @param preprocessedAuctions Auction data sample.
    * @param classifier           Classifier used on sample.
    * @tparam Model Model used for prediction.
    * @return Models derived from running different test and training k-partitions on sample.
    */
  def partitionTestCrossValidation[Model](preprocessedAuctions: Dataset[PreprocessedAuction], classifier: Classifier[Model]): List[Model] = {
    val (classificationData, latestAuction) = AuctionPreprocessor.getAveragedAuctionsByDay(preprocessedAuctions, 2589)

    classificationData.groupBy("priceMovementLabel").count.show() // Distribution of price movements
    List.range(2, 11, 1).map(k =>
      Evaluator.crossValidation(classificationData, Some("k" + k), classifier, kPartitions = k)
    )
  }

  /**
    * Evaluates different metrics of tree classifier including accuracy, precision, recall, and FP rate.
    *
    * @param predictionAndLabels Predicted labels.
    * @param treeType            Type of tree classifier.
    */
  def evaluate(predictionAndLabels: RDD[(Double, Double)], treeType: String): Unit = {
    val matrices = new MulticlassMetrics(predictionAndLabels)
    // Confusion Matrices
    println(s"Confusion Matrix : $treeType")
    println(matrices.confusionMatrix)

    // Statistics
    println("Overall Statistics")
    println(s"Accuracy = ${matrices.accuracy}")

    // Precision by label
    matrices.labels.foreach(l => {
      val labelName = PriceMovementLabel.toString(PriceMovementLabel.getPriceMovement(l.toInt))
      println(s"Precision for $labelName: ${matrices.precision(l)}")
    })

    // Recall by label
    matrices.labels.foreach(l => {
      val labelName = PriceMovementLabel.toString(PriceMovementLabel.getPriceMovement(l.toInt))
      println(s"Recall for $labelName: ${matrices.recall(l)}")
    })

    // FP Rate by label
    matrices.labels.foreach(l => {
      val labelName = PriceMovementLabel.toString(PriceMovementLabel.getPriceMovement(l.toInt))
      println(s"FP-rate for $labelName: ${matrices.falsePositiveRate(l)}")
    })

    // F-Measure  by label
    matrices.labels.foreach(l => {
      val labelName = PriceMovementLabel.toString(PriceMovementLabel.getPriceMovement(l.toInt))
      println(s"F1-Score for $labelName: ${matrices.fMeasure(l)}")
    })

  }

}

