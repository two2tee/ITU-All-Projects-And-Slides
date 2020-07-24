// Advanced Programming 2015
// Andrzej Wasowski, IT University of Copenhagen
// Example solution for scala exercises using scalacheck
// Scalacheck's user guide:
// https://github.com/rickynils/scalacheck/wiki/User-Guide

package fpinscala.monoids
import org.scalacheck._
import org.scalacheck.Prop._
import Arbitrary.arbitrary


object MonoidSpec extends Properties("Monoids..") {

  import Monoid._

  // Exercise 4 (intro to the exercise)

  // Monoid properties: algebraic structure with a single associative binary operation and an identity element

  def associative[A :Arbitrary] (m: Monoid[A]) :Prop =
    forAll { (a1: A, a2: A, a3: A) =>
      m.op(m.op(a1,a2), a3) == m.op(a1,m.op(a2,a3)) } :| "associativity"

  def unit[A :Arbitrary] (m :Monoid[A]) =
    forAll { (a :A) => m.op(a, m.zero) == a } :| "right unit" &&
    forAll { (a :A) => m.op(m.zero, a) == a } :| "left unit"

  def monoid[A :Arbitrary] (m :Monoid[A]) :Prop = associative (m) && unit (m)

  property ("stringMonoid is a monoid") = monoid (stringMonoid)

  // Exercise 4: test intAddition, intMultiplication, booleanOr,
  // booleanAnd and optionMonoid.

  property ("intAddition is a monoid") = monoid(intAddition)
  property ("intMultiplication is a monoid") = monoid(intMultiplication)
  property ("booleanOr is a monoid") = monoid(booleanOr)
  property ("booleanAnd is a monoid") = monoid(booleanAnd)

  // TODO: How do I generalize arbitrary Monoid option generator over type A using implicit parameters and higher kinded types ???
  import scala.language.implicitConversions
  import scala.language.higherKinds

  // Arbitrary generator instance of the Option type 
  // When n is larger, make it less likely that we generate None,
  // but still do it some of the time. When n is zero, we always
  // generate None, since it's the smallest value.
  implicit def arbOption[A] (implicit a: Arbitrary[A]) : Arbitrary[Option[A]] = 
    Arbitrary(Gen.sized(n =>
      Gen.frequency(
        (n, Gen.resize(n / 2, arbitrary[A]).map(Some(_))),
        (1, Gen.const(None)))))  
  trait Arbitrary1[F[_]] { // Tried with higher kinded type constructor (super generic)
    implicit def liftArb[A] (arb: Arbitrary[A]): Arbitrary[F[A]]
  }
  implicit def arbOption[A: Arbitrary] (implicit a: A) : Arbitrary[Option[A]] = Arbitrary {
    val genA: Gen[A] = for { a <- arbitrary[A] } yield a 
    Gen.option(genA) // Gen[Option[A]]  
  }

  property ("optionMonoid is a monoid") = monoid (optionMonoid[Int])

  // Exercise 5

  // Property test used to check if a function is a homomorphism between two sets (structure-preserving) 
  // Homomorphism is a simple function f: A => B which preserves the monoid structure
  // A monoid homomorphism f between monoids M[A] and N[B] obeys the general law for all x and y: 
  // M.op(f(x), f(y)) == f(N.op(x,y))
  // Example: "foo".length + "bar".length == ("foo" + "bar").length
  def homomorphism[A :Arbitrary,B :Arbitrary] (ma: Monoid[A]) (f: A => B) (mb: Monoid[B]) : Prop = 
    forAll { (a1: A, a2: A) => f(ma.op(a1, a2)) == mb.op(f(a1), f(a2)) } :| "homomorphism"

  property ("length is a monoid homomorphism") = 
    homomorphism[String, Int] (stringMonoid) (_.length) (intAddition)

  // Isomorphism implies a homomorphism in both directions between two monoids. 
  // A monoid isomorphism between M and N has two homomorphisms f and g, 
  // where both f andThen g and g andThen f are an identity function.
  // Example: String and List[Char] monoids with concatenation are isomorphic 
  def isomorphism[A :Arbitrary, B :Arbitrary] (ma: Monoid[A]) (f: A => B) (mb: Monoid[B]) (g: B => A) : Prop = 
    homomorphism (ma) (f) (mb) == homomorphism (mb) (g) (ma) 

  property ("stringMonoid and listMonoid[Char] are isomorphic") = 
    isomorphism[String, List[Char]] (stringMonoid) (_.toList) (listMonoid[Char]) (_.mkString)

  // Exercise 6

  property ("booleanOr and booleanAnd are isomorphic") = 
    isomorphism[Boolean, Boolean] (booleanOr) (p => !p) (booleanAnd) (p => !p)

  // Exercise 7 (the testing part)

  property ("productMonoid is a monoid") = 
    monoid (productMonoid (optionMonoid[Int]) (listMonoid[String]))
}
