// Advanced Programming 2015
// Andrzej Wasowski, IT University of Copenhagen

// Example solutions for Monad exercises, using scalacheck
// Scalacheck's user guide:
// https://github.com/rickynils/scalacheck/wiki/User-Guide

package fpinscala.monads
import org.scalacheck._
import org.scalacheck.Prop._
import Arbitrary.arbitrary
import scala.language.higherKinds
import Functor._

// Test suite of Functor generalization of map transformation function 
// 1) Use type constructor parameters (higher kinded types) 
// 2) Implicit conversion impose an interface on a generic type that force function to provide conversion 
object FunctorSpec extends Properties("Functor[F[_]] properties..") {

  // Implicit param states there must exist an implicit conversion rule from F[A] to Arbitrary[F[A]]
  def mapLaw[A,F[_]] (fn :Functor[F]) (implicit arb: Arbitrary[F[A]]) :Prop =
    forAll { (fa :F[A]) => fn.map[A,A] (fa) (x => x) == fa }

  property ("Functor[List[Int]] satisfies the functor law") =
    mapLaw[Int,List](ListFunctor)

  // Exercise 11 (for OptionFunctor)

  property ("Functor[Option[Char]] satisfies the functor law") = 
    mapLaw[Char,Option](OptionFunctor)
}


// vim:cc=80
