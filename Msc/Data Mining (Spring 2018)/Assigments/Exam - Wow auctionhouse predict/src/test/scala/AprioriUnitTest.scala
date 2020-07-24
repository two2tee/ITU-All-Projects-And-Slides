import Apriori.AssociationRule
import org.scalatest.FlatSpec

/**
  * Apriori Unit Test Suite used to test logic functionality of the Apriori algorithm.
  * This includes testing the logic used to find the support and confidence of frequent item sets.
  *
  * Implements FlatSpec trait that facilitates a "behavior-driven" style of development (BDD).
  * Tests are combined with text that specifies the behavior the tests verify.
  * Link: http://doc.scalatest.org/1.8/org/scalatest/FlatSpec.html
  */
class AprioriUnitTest extends FlatSpec with SparkConfiguration {

  behavior of "Apriori"

  it should "count the support of frequent item sets to be their frequency" in {
    // Arrange
    val itemSet: Set[String] = Set("bread")
    val transactions: List[Set[String]] = List(Set("bread", "milk"), Set("bread", "bread"), Set("milk"), Set("milk", "tomato"))
    val expectedSupport = 0.5

    // Act
    val actualSupport = Apriori.countSupport(itemSet, transactions)

    // Assert
    assert(actualSupport == expectedSupport)

  }

  it should "count the confidence of association rules to be how often the rule is found to be true" in {
    // Arrange
    val LHS: Set[String] = Set("bread")
    val RHS: Set[String] = Set("milk")
    val associationRule = AssociationRule(LHS, RHS, 0.25)
    val transactions: List[Set[String]] = List(Set("bread", "milk"), Set("bread", "bread"), Set("milk"), Set("milk", "tomato"))
    val expectedConfidence = 0.5

    // Act
    val actualConfidence = Apriori.countConfidence(associationRule, transactions)

    // Assert
    assert(actualConfidence == expectedConfidence)

  }

}
