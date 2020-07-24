// Advanced Programming 2015
// Andrzej Wasowski, IT Univesity of Copenhagen


// 1. Introduction
//
// This file is best worked on by reading exercise descriptions in the
// corresponding PDF file.  Then find the place in the file where the stub for
// the exercise is created and complete it.
//
// Before starting to work on the exercises, familiarize yourself with the the
// content already in the file (it has been explained in chapter 8, but it is
// useful to see it all together in one file).
//
// I compile this file using sbt, but in case you need to debug sbt, the
// following should also work (if you put both scala files in the same
// directory):
//
// $ fsc State.scala Gen.scala
//
// Then you can load Exercise2.scala into the REPL to interactively test your
// work. (Exercise2.scala is not so key, it just has all the useful imports in
// the header, that make working with REPL more effcient for these exercises).
// This can be done by issuing "sbt console" and importing the write object from
// Exercise2 (the class will be loaded automatically by sbt).
//
// The file is similar to the one created by Chiusano and Bjarnasson, but it
// has small differences, since I created it myself working through the
// chapter.

package fpinscala.testing
import fpinscala.state._
import fpinscala.state.RNG._


// A generator will use a random number generator RNG in its state, to create
// random instances (but perhaps also some other staff)
case class Gen[A] (sample :State[RNG,A]) {

  // Let's convert generator to streams of generators
  def toStream (seed :Long):Stream[A] =
    Gen.state2stream(this.sample) (RNG.Simple(seed))
  def toStream (rng :RNG):Stream[A] =
    Gen.state2stream(this.sample) (rng)

  // the book uses Stream.unfold, but apparently the standard library lacks this method

  // Exercise 3 (Ex 8.5 second part)
  //
  // Hint: The standard library has the following useful function (List
  // companion object):
  //
  // def fill[A](n: Int)(elem: ⇒ A): List[A]
  //
  // It is of course possible to implement a solution without it, but the
  // result is ugly (you need to replicate the behavior of fill inside
  // listOfN). Then note that State has a method sequence which allows to take
  // a list of automata and execute their transitions as a sequence, feeding
  // the output state of one as an input to the next.  This can be used to
  // execute a series of consecutive generations, passing the RNG state around.

  // Return list of length n containing A elements generated by this generator
  def listOfN (n :Int) :Gen[List[A]] = {
    // Transform state elements from A values to List[A] values and wrap in generator
    //Gen(State.sequence(List.fill(n)(this.sample)))
    Gen(this.sample.map(elem => List.fill (n) (elem)))
  }



  // Exercise 4 (Ex. 8.6 [Chiusano, Bjarnasson 2015])

  def flatMap[B] (f: A => Gen[B]) :Gen[B] = {
    // flatMap underlying state via sample and map from Gen[B] to State[S,B] to return Gen[B]
    Gen(this.sample.flatMap (elemA => f(elemA).sample))
  }


  // It would be convenient to also have map (uncomment once you have unit and flatMap)

  def map[B] (f : A => B) :Gen[B] = this.flatMap (a => Gen.unit[B] (f(a)))

  // Exercise 5 (Second part of Ex. 8.6)
  // Generate lists with a random size determined by a generator of integers
  def listOfN (size: Gen[Int]): Gen[List[A]] = {
    size.flatMap(elem => listOfN(elem)) // flatMap generator, pass random value and wrap final list result in Gen
  }

  // Exercise 6 (Ex. 8.7; I implemented it as a method, the book asks for a
  // function, the difference is minor; you may want to have both for
  // convenience)
  //
  // Hint: we already have a generator that emulates tossing a coin. Which one
  // is it? Use flatMap with it.

  // Combine two generators of same type into one, by pulling values from each generator with equal chance
  def union (that :Gen[A]) :Gen[A] = {
    // Use boolean generator to emulate tossing a coin => decide whether to pick value from this or that
    // Create generator based on coin toss that randomly take values from this and that and flatten to Gen[A]
    Gen.boolean.flatMap(coin => if (coin) this else that) // TODO: does this actually extract values?
  }

  // Exercise 7 continues in the bottom of the file (in the companion object)
}

object Gen {

  // A convenience function to convert states (automata) to streams (traces)
  // It would be better to have it in State, but I am not controlling
  // State.scala.

  private  def state2stream[A] (s :State[RNG,A]) (seed :RNG) :Stream[A] =
    s.run(seed) match { case (n,s1) => n #:: state2stream (s) (s1) } // #:: cons operator for Streams

  // A generator for Integer instances

  def anyInteger :Gen[Int] = Gen(State(_.nextInt))

  // Exercise 1 (Ex. 8.4)
  //
  // Hint: Before solving the exercise study the type \lstinline{Gen} in
  // \texttt{Gen.scala}. Then, think how to convert a random integer to a
  // random integer in a range.  Then recall that we are already using
  // generators that are wrapped in \texttt{State} and the state has a
  // \lstinline{map} function.
  // Generate integers in range start to stopExclusive
  def choose (start :Int, stopExclusive :Int) :Gen[Int] = {
    // Generator is wrapped in state with map function used to generate within range
    Gen(anyInteger.sample.map(n => n % (stopExclusive - start) + start)) // Logic
  }


  // Exercise 2 (Exercise 8.5, part one)
  //
  // Hint: The \lstinline{State} trait already had \lstinline{unit}
  // implemented:
  // def unit[S, A](a: A): State[S, A] = State(s => (a, s))
  // Generate constant value a in parameter
  def unit[A] (a : =>A) :Gen[A] = Gen(State.unit (a))

  // Hint: How do you convert a random integer number to a random Boolean?
  // Alternatively: do we already have a random generator for booleans? Could
  // we wrap it in.
  // Convert random integer to random Boolean
  def boolean :Gen[Boolean] = {
    //Gen(anyInteger.sample.map(elem => elem % 2 == 0)) // Map internal state and return true or false from random value
    // State accepts run method (S => (A,S) and RNG.boolean returns (RNG => (Boolean, RNG)) where S=RNG
    Gen(State(RNG.boolean))
  }



  // Hint: Recall from Exercise1.scala that we already implemented a random
  // number generator for doubles.

  def double :Gen[Double] = {
    // RNG.double returns (RNG => Double, RNG) which satisfies State run method: (S => (A,S) where S=RNG
    Gen(State(RNG.double))
  }

  // (Exercise 3 is found in the Gen class above)

  // (Exercise 7 is found below in class Prop)

}

// This is the Prop type implemented in [Chiusano, Bjarnasson 2015]

object Prop {

  type TestCases = Int   // How many test cases to examine before passed
  type SuccessCount = Int // Count of successful test cases
  type FailedCase = String // How many tests failed

  // the type of results returned by property testing

  sealed trait Result { def isFalsified: Boolean } // Feature of Result type
  case object Passed extends Result { def isFalsified = false } // All test cases passed
  case class Falsified(failure: FailedCase,
    successes: SuccessCount) extends Result { //
      def isFalsified = true
  }
  case object Proved extends Result { def isFalsified = false } // Property proved after one test case

  // Create property using random generator Gen[A] to generate relevant A test cases
  def forAll[A] (as: Gen[A]) (f: A => Boolean): Prop = Prop {
    (n,rng) => as.toStream(rng).zip(Stream.from(0)).take(n).map {
      case (a,i) => try {
        if (f(a)) Passed else Falsified(a.toString, i) // Record failed test and index (tests passed before fail)
      } catch { case e: Exception => Falsified(buildMsg(a, e), i) } // Capture exceptions thrown by test case
    }.find(_.isFalsified).getOrElse(Passed) // Return Falsified or Passed if no falsified tests
  }

  // Capture information on exception thrown by test case
  def buildMsg[A](s: A, e: Exception): String =
    s"test case: $s\n" +
    s"generated an exception: ${e.getMessage}\n" +
    s"stack trace:\n ${e.getStackTrace.mkString("\n")}"
}

import Prop._

// Test Case generator
case class Prop (run :(TestCases,RNG) => Result) {

  // (Exercise 7)

  // Gen[A] can be used to generate values of type A
  // Gen wraps State transition over random number generator: Gen[A] (sample: State[RNG,A])

  // Compose properties: Prop[A].&& should succeed only if both properties (this and that) succeed
  def && (that :Prop) :Prop = Prop {  // Pass anonymous random test case generator
    (testCase, rng) => (this.run(testCase, rng), that.run(testCase, rng)) match {
      case (Passed, Passed) => Passed
      case (left @ Falsified(_,_), _) => left
      case (_, right @ Falsified(_,_)) => right
      case (Proved, Proved) => Proved
      case (Passed, Proved) | (Proved, Passed) => Proved
    }

  }
  def &&& (that :Prop) :Prop = Prop { // 2nd version
    (t,rng) => this.run(t,rng) match{
      case Passed => that.run(t,rng)
      case Proved => that.run(t,rng)
      case otherProp => otherProp
    }

  }



  // Prop[A].|| should fail only if both composed properties (this and that) fail
  // fc = failed case and sc = success count
  def || (that :Prop) :Prop = Prop {
    (testCase, rng) => (this.run(testCase, rng), that.run(testCase, rng)) match {
      //case (Falsified(fc, sc), _) => Falsified(fc, sc)
      case (Passed, _) | (_, Passed) => Passed // Either passed
      case (leftCaseFail @ Falsified(_, _), _) => leftCaseFail
      case (_, rightCaseFail @ Falsified(_, _)) => rightCaseFail
      case (Proved, _) | (_, Proved) => Proved // TODO: had to cover Proved case to exhaust patterns
    }
  }

  def ||| (that :Prop) :Prop = Prop { //2nd version
    (t,rng) => (this.run(t,rng),that.run(t,rng)) match {
      case (f:Falsified, _:Falsified) => f
      case (_,_) => Passed
    }
  }

}

// vim:cc=80:foldmethod=indent:foldenable