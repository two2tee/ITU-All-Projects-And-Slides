// Advanced Programming
// Thor Olesen, Daniel Hansen & Dennis Nguyen, IT University of Copenhagen

package fpinscala.laziness

import org.scalacheck.Arbitrary.arbitrary
import org.scalacheck.Prop._
import org.scalacheck._
import org.scalatest.FlatSpec
import org.scalatest.prop.Checkers

import scala.language.higherKinds

// If you comment out all the import lines below, then you test the Scala
// Standard Library implementation of Streams. Interestingly, the standard
// library streams are stricter than those from the book, so some laziness tests
// fail on them :)

 import fpinscala.laziness.stream00._    // uncomment to test the book solution
//import stream01._ // uncomment to test the broken headOption implementation
// import stream02._ // uncomment to test another version that breaks headOption

/**
  * Test suite (i.e. collection of tests) used to test the behavior of the 'lazy list' Stream implementation.
  * Scalatest FlatSpec is used to enable BDD ('behavior-driven development'), in which tests are combined with text that specifies the behavior the tests verify.
  * ScalaCheck is used to enable property-based testing. Given a property describing the expected behavior, it will generate random input data, covering many edge cases, and check the result.
  */
class StreamSpecTvaoDaroDttn extends FlatSpec with Checkers {

  import Stream._

  // An example generator of random finite non-empty streams
  def list2stream[A] (la :List[A]): Stream[A] = la.foldRight (empty[A]) (cons[A](_,_))

  // In ScalaTest we use the check method to switch to ScalaCheck's internal DSL
  // Implicit is used by scala to find Arbitrary[A] generator that works for parameter arbA
  def genNonEmptyStream[A] (implicit arbA :Arbitrary[A]) :Gen[Stream[A]] =
    for { la <- arbitrary[List[A]] suchThat (_.nonEmpty)} // arbitrary generator of non empty lists
      yield list2stream (la)

  // Random Int Stream generator
  def genInt[Int]() =
    for {
      n <- Gen.choose(1, 100)
    } yield n

  behavior of "headOption"

  // headOption scenario test
  it should "return None on an empty Stream (01)" in {
    assert(empty.headOption == None)
  }

  // headOption property test
  it should "return the head of the stream packaged in Some (02)" in check {
    // the implicit makes the generator available in the context
    implicit def arbIntStream = Arbitrary[Stream[Int]] (genNonEmptyStream[Int])
    ("singleton" |: // "|:" attach a string label to a part of the boolean expression
      Prop.forAll { (n :Int) => Stream.cons (n,empty).headOption == Some (n) } ) &&
      ("random" |:
        Prop.forAll { (s :Stream[Int]) => s.headOption != None } )
  }

  // Returns head of an infinite stream without crashing, which means the tail is not force evaluated but lazy
  it should "not force evaluate the tail of the stream in Some(03)" in {
    // Arrange
    def infiniteStream:Stream[Int] = Stream.cons(0, infiniteStream)
    val expected = Some(0)

    // Act
    val actual = infiniteStream.headOption

    // Assert
    assert(actual == expected)
  }

  behavior of "take"

  // Scenario test of "take"
  // TODO: noException should be thrownBy syntax does not work ?
  it should "not force any heads nor any tails of the Stream it manipulates (04)" in {
    // Arrange
    val testStream = Stream.cons(0, cons(throw new AssertionError("Forced"), empty))

    // Act
    try { testStream.take(1).headOption.get } // Call take on lazy Exception element

    // Assert
    catch { case _ : Throwable => fail("Stream is forced and not lazy") }
  }

  // Property test of "take"
  it should "not force any heads nor any tails of the Stream it  manipulates (05)" in check {
    Prop.forAll { (n: Int) => {
      // Arrange
      val testStream = Stream.cons(throw new AssertionError("Forced"), throw new AssertionError("Forced"))

      // Act
      testStream.take(n)

      // Assert
      passed
    }}
  }

  it should "hold that s.take(n).take(n) == s.take(n) for any Stream s and any n (idempotent values) IMPLICIT (06)" in check {
    // the implicit makes the generator available in the context
    implicit def arbIntStream = Arbitrary[Stream[Int]] (genNonEmptyStream[Int])
    ("empty" |: Prop.forAll { (n :Int) => empty.take(n).take(n) == empty.take(n) } ) &&
      ("singleton" |: Prop.forAll { (n :Int) => cons (n,empty).take(n).take(n).headOption == cons(n,empty).take(n).headOption } ) &&
      ("random" |: Prop.forAll {(s :Stream[Int]) => s.take(20).take(20).toList.zip(s.take(20).toList).forall{ case(x,y) => x == y }})
  }

  // Explicit property test version
  it should "hold that s.take(n).take(n) == s.take(n) for any Stream s and any n (idempotent values) EXPLICIT (06)" in check {
    "empty" |: Prop.forAll { (n: Int) => empty.take(n).take(n) == empty.take(n) }
    "singleton" |: Prop.forAll { (n: Int) => cons(n, empty).take(n).take(n).headOption == cons(n, empty).take(n).headOption }
    "random" |: Prop.forAll (genNonEmptyStream[Int]) { (s: Stream[Int]) => s.take(100).take(100).toList.zip(s.take(100).toList).forall { case (x,y) => x == y} }
  }

  // Scenario test
  it must "not force (n+1)st head ever in take(n) (even if we force all elements of take(n) (07)" in {
    // Arrange
    val testStream = cons(1, cons (1, cons (throw new AssertionError, empty)))

    // Act
    try { testStream.take(2).toList } // Try forcing all elements without exception

    // Assert
    catch { case _ : Throwable => fail("Stream forced") }
  }

  // Property test
  it must "not force (n+1)st head ever in take(n) (even if we force all elements of take(n) (08)" in check {
    Prop.forAll (genNonEmptyStream){  (s: Stream[Int]) => {
      // Arrange
      val testStream = s.append(cons(throw new AssertionError,empty)) // Dynamic but depend on append

      // Act
      testStream.take(s.toList.size).toList

      // Assert
      passed
    }}
  }

  it should "return the last take element in any Stream (09)" in  check {
    "Random" |: Prop.forAll (genNonEmptyStream) { (s: Stream[Int]) =>
      s.take(100).take(10).take(5).toList.zip(s.take(5).toList).forall { case(x,y) => x == y }
    }
  }

  behavior of "drop"

  //  Property test: s.drop(n).drop(m) == s.drop(n+m) for any n, m (additivity)
  it should "drop (skip) all elements in a chain of drop calls on a Stream (10)" in check {
    "empty" |: Prop.forAll { (n : Int, m : Int) => empty.drop(n).drop(m) == empty.drop(n+m) }
    "singleton" |: Prop.forAll { (n : Int, m : Int) => cons(n, cons(m, empty)).drop(n).drop(m) == cons(n, cons(m, empty)).drop(n + m)  }
    "random" |: Prop.forAll (genNonEmptyStream, genInt, genInt) { (stream : Stream[Int], n : Int, m : Int) => stream.drop(n).drop(m) == stream.drop(n+m) }
  }

  // s.drop(n) does not force any of the dropped elements heads
  it should "not force any of the dropped element heads (11)" in {
    // Arrange
    val testStream = cons(throw new AssertionError,cons(throw new AssertionError,empty))

    // Act
    try {testStream.drop(2).headOption }
    catch {case _: Throwable => fail("drop head forced")}

    // Assert
    passed
  }

  it should "throw exception when forcing empty head value on dropped stream (12)" in {
    // Arrange
    val testStream = cons(throw new AssertionError,cons(throw new AssertionError,empty))

    // Act and assert
    intercept[NoSuchElementException] { testStream.drop(2).headOption.get }
  }

  // the above should hold even if we force some stuff in the tail
  it should "not force any dropped element heads even if we force elements in the tail (13)" in {
    // Arrange
    val testStream = cons(throw new AssertionError,cons(throw new AssertionError,cons(1,cons(2,empty))))

    // Act
    testStream.drop(2).toList

    // Assert
    passed
  }

  behavior of "map"

  // x.map(id) == x (where id is the identity function)
  it should "always return same value that was used as its argument to the identity function (14)" in check {
    "empty" |: Prop.forAll { (_:Int) => empty.map(identity).toList.zip(empty.toList).forall{ case(x,y) => x==y } }
    "Random"  |: Prop.forAll(genNonEmptyStream) { (st: Stream[Int]) => st.map(identity).toList.zip(st.toList).forall{case(x,y) => x==y}}
  }

  // map terminates on infinite streams
  it should "terminate map on infinite streams (15)" in {
    // Arrange
    val infiniteStream = from(1)

    // Act
    infiniteStream.map(x=>x+1)

    // Assert
    passed
  }

  behavior of "append"

  // TODO: Why does fold force/unpack Stream elements
  it should "force elements in stream that is appended from (16)" in {
    // Arrange
    val streamA = cons(throw new RuntimeException("Forced"), empty)
    val streamB = cons(1,empty)

    // Act and Assert
    intercept[RuntimeException]{streamA.append(streamB)}
  }

  it should "not force head or tail on stream that is appended to (17)" in {
    val streamA = cons(1,cons(2,empty))
    // Arrange
    val streamB = cons(throw new AssertionError,cons(throw new AssertionError, empty))

    // Act
    try {streamA.append(streamB)}
    catch {case _: Throwable => fail("heard or tail was forced")}

    // Assert
    passed
  }

  it should "terminate on any finite streams and property size(s1 + s2) == size(s1.append(s2) must hold (18)" in check {
    implicit def arbIntStream = Arbitrary[Stream[Int]] (genNonEmptyStream[Int])
    "random streams" |: Prop.forAll { (streamA: Stream[Int],streamB: Stream[Int]) =>
      streamA.append(streamB).toList.size == streamA.toList.size+streamB.toList.size
    }
  }

  // Scenario test
  it should "maintain the ordering such that the appended stream is at the end of the resulting stream (19)" in {
    // Arrange
    val testStream = from(0).take(100)
    val testStream2 = from(0).drop(100).take(100)
    val expected = from(0).take(100).toList

    // Act
    val actual = testStream.append(testStream2).toList

    // Assert
    actual.zip(expected).forall({ case (x,y) => x == y})
  }

  it should "hold that an appended stream is appended at the end (20)" in check {
    implicit def arbIntStream = Arbitrary[Stream[Int]] (genNonEmptyStream[Int])
    "Append Ordering" |: Prop.forAll { (s1: Stream[Int], s2: Stream[Int]) =>
      // Arrange
      val fromStream = s1.take(100)
      val toStream = s2.take(100)

      // Act
      val orderedOutput = fromStream.append(toStream).drop(fromStream.toList.size).toList.zip(toStream.toList)

      // Assert
      orderedOutput.forall({ case (x,y) => x == y})
    }
  }


}
