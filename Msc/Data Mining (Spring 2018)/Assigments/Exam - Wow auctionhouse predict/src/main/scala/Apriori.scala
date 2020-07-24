import Apriori.AssociationRule
import scala.annotation.tailrec
import scala.language.higherKinds

/**
  * Trait (interface) responsible for defining the Apriori algorithm used to find frequent auction items.
  * This may be used to find associations (i.e. trends) in [[Model.PreprocessedAuction]] data.
  *
  * The Apriori algorithm learns association rules and is applied to a database containing a large number of transactions.
  * By applying the Apriori algorithm, we can learn which items frequently occur together a.k.a association rules.
  * A frequent item set is an item set appearing at least as many times as minimum support threshold.
  * Thus, Apriori attempts to search for relationships and patterns among a set of transactions to reveal trends.
  *
  * Association rule learning: https://en.wikipedia.org/wiki/Association_rule_learning
  * Higher kinded type inspiration: https://stackoverflow.com/questions/33519982/higher-kinded-types-scala-for-list-options-and-other-complex-types
  */
sealed trait Apriori[A[_], L[_]] { // Higher kinded types used to create Array[List[T]] data structure representing array of item transactions as lists
  /**
    * Initializes the first 1-item set and transactions transaction database.
    * This is based on the survey data revolved around the programming languages students are familiar with according to the survey.
    * Runs the apriori algorithm to find association rules that may reveal general trends in the transaction database.
    *
    * @param transactions Database of transactions.
    * @param minSupport Metric of how frequently item set should appear in the transaction database.
    * @param minConfidence Measure of how often an association rule should be true.
    */
  def run[T] (transactions: A[L[T]], minSupport: Double, minConfidence: Double) : List[AssociationRule[T]]
}

/**
  * Generic implementation of Apriori algorithm.
  *
  */
object Apriori extends Apriori[Array, Array] {

  // Model representing an association rule between two item sets and its support
  case class AssociationRule[T] (LHS: Set[T], RHS: Set[T], confidence: Double)

  /**
    * Initializes the first 1-item set and transactions database.
    * Runs the apriori algorithm to find association rules that may reveal general trends in the transaction database.
    * @param transactions Database of transactions.
    * @param minSupport Metric of how frequently item set should appear in the transaction database.
    * @param minConfidence Measure of how often an association rule should be true.
    * @return Association rules revealed by the Apriori algorithm in the transaction database.
    */
  def run[T] (transactions : Array[Array[T]], minSupport: Double = 0.01, minConfidence: Double = 0.5) : List[AssociationRule[T]] = {
    val transactionSets: List[Set[T]] = transactions.map(transaction => transaction.toSet).toList
    val itemSet : Set[T] = transactionSets.foldLeft (Set[T]()) ((acc, set) => acc ++ set) // "++": Set containing all items
    val frequentItemSets = apriori(itemSet, transactionSets, minSupport, minConfidence)
    val associationRules = calculateAssociationRule(frequentItemSets, minConfidence)
    associationRules
  }

  /**
    * Calculates support which is an indication of how frequently the item set appears in the transaction database.
    * It counts the support (frequency) of an item set by counting all its subsets across all transactions.
    * Support = #transactions containing 'a' and 'b' / total no of transaction => supp(a,b) => p(a U b)
    * @param itemSet group of items
    * @param transactions set of transactions
    * @return support metric between 0 and 1 indicating how many times item set occurs in data set.
    */
  def countSupport[T] (itemSet: Set[T], transactions: List[Set[T]]): Double = {
    val count = transactions.count(transaction => itemSet.subsetOf(transaction))
    count.toDouble / transactions.size.toDouble
  }

  /**
    * Apriori algorithm finds frequent item sets and calculates association rules over transactional database.
    * It proceeds by identifying frequent items in the transaction database and extends them to larger frequent item sets.
    * The frequent item sets are used to determine association rules which highlight trends in the database.
    * @param itemSet Set containing all items in database.
    * @param transactions Database of transactions.
    * @param minSupport Metric of how frequently item set should appear in the transaction database.
    * @param minConfidence Measure of how often an association rule should be true.
    * @return frequent item sets revealed by Apriori in the transaction database.
    */
  def apriori[T](itemSet: Set[T], transactions: List[Set[T]], minSupport: Double, minConfidence: Double): Map[Set[T], Double] = {

    // Use accumulator pattern to accumulate frequent items recursively
    def getFrequentItemSets(candidateSet: Set[Set[T]]): Set[(Set[T], Double)] = {
      @tailrec
      def accumulator(candidateSet: Set[Set[T]], kItemSets: Set[(Set[T], Double)], k: Int): Set[(Set[T], Double)] = {
        // Find candidate set, Ck, for level k: preceding candidate k-item sets are used to explore (k+1)-item sets
        val nextKItemSet : Set[(Set[T], Double)] = candidateSet
          // Find set of frequent k-item sets by scanning transaction database to find support (count) of each item
          .map( itemSet => (itemSet, countSupport(itemSet, transactions)))
          // Prune step (Apriori property) : only collect item sets that satisfy minimum support
          .filter( itemSetSupportPair => itemSetSupportPair._2 >= minSupport)

        // L1 resulting k-set of frequent item sets that satisfy minimum support
        val currentLSet = nextKItemSet.map( itemSetSupportPair => itemSetSupportPair._1)

        currentLSet.isEmpty match {
          case true => kItemSets // Stop condition: Break if no more frequent k-item sets
          case _ => // General case: Find frequent k-item sets

            // Join step: L1 k-item set is self joined to generate (k+1)-item candidate sets (L2, L3, etc)
            val nextCandidateSet = currentLSet
              .map( itemSet1 => currentLSet.map(itemSet2 => itemSet1 | itemSet2)) // Union item sets
              .reduceRight( (set1, set2) => set1 | set2)
              .filter( itemSet => itemSet.size == k) // K-sized item sets only

            val newKItemSet = kItemSets | nextKItemSet // Union

            // Repeat generating candidate sets for next level
            accumulator(candidateSet = nextCandidateSet, kItemSets = newKItemSet, k = k+1)
        }
      }
      accumulator(candidateSet, kItemSets = Set(), k = 1)
    }

    val candidateSet : Set[Set[T]] = itemSet.map( number => Set(number) ) // 1-item candidate set
    val frequentItemSets : Set[(Set[T], Double)] = getFrequentItemSets(candidateSet) // k-item candidate set
    val frequencyTable = // Add frequent item sets and their support count to map with constant lookup time
      frequentItemSets.foldLeft (Map[Set[T], Double]()) ((frequencyTable, itemSet) => frequencyTable + (itemSet._1 -> itemSet._2))

    frequencyTable
  }

  /**
    * Finds the association rules (i.e. items) that are associated (e.g. purchased) together more frequently.
    * A minimum confidence constraint is applied to frequent item sets in order to form rules.
    * @param frequencyTable Map containing item sets and their frequency.
    * @param minConfidence Threshold indicating how often rule should be true.
    * @return association rules discovered in transaction database
    */
  def calculateAssociationRule[T] (frequencyTable: Map[Set[T], Double], minConfidence: Double): List[AssociationRule[T]] = {

    // For comprehension used to gather association rules from frequency table
    val associationRules =
      for  {
        frequentItem <- frequencyTable.keys
        subset <- frequentItem.subsets
        if subset.size < frequentItem.size & subset.nonEmpty
      } yield AssociationRule(subset, frequentItem diff subset, frequencyTable(frequentItem) / frequencyTable(subset))

    // Remove item sets that do not appear frequently enough based on confidence threshold
    associationRules
      .filter( rule => rule.confidence >= minConfidence)
      .map(rule => rule.copy(confidence = BigDecimal(rule.confidence).setScale(2, BigDecimal.RoundingMode.HALF_UP).toDouble)).toList
  }

  /**
    * Confidence is an indication of how often an association rule is found to be true.
    * Confident = No.of transactions containing 'a' and 'b' / no of transaction containing 'a'.
    * Confident => con (a, b) == > P (b|a) nothing but conditional probability.
    * @param associationRule Rule to which confidence should be found.
    * @param transactions Database of transactions.
    * @return Confidence metric between 0 and 1 indicating how often association rule is true in across transactions in database.
    */
  def countConfidence[T] (associationRule: AssociationRule[T], transactions: List[Set[T]]): Double = {
    val count = transactions.count(transaction => associationRule.LHS.subsetOf(transaction) &&
      associationRule.RHS.subsetOf(transaction))
    count.toDouble / transactions.count(transaction => associationRule.LHS.subsetOf(transaction)).toDouble
  }

  // Helper function to pretty print association rules
  def printAssociationRules[T] (associationRules: List[AssociationRule[T]]) : Unit = {
    associationRules.foreach(rule => println(s"LHS: ${rule.LHS} => RHS: ${rule.RHS} | confidence: ${rule.confidence}" ))
  }

}
