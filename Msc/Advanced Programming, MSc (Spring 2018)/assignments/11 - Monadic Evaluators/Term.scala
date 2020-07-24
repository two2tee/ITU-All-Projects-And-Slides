/* ADVANCED PROGRAMMING. Monadic Evaluators. Andrzej Wąsowski */
package adpro.monads
import scala.language.higherKinds

// Work through this file top down

// Section 2.1 [Wadler]

// Wadler uses a langauge similar to Haskell to implement his evaluator. We will
// use scala.  This is Wadler's Term language implemented in Scala:

// A term is either a constant or a quotient 
// data Term = Con Int | Div Term Term 
trait Term
case class Cons (value :Int) extends Term
case class Div (left :Term, right :Term) extends Term

// From now on we create one module (object) per section to avoid name clashes
// between different variants.

// This is the basic evaluator (compare to the paper, to see whether you understand)
object BasicEvaluator {

  def eval (term :Term) :Int = term match {
    case Cons(a) => a
    case Div(t,u) => eval (t) / eval (u)
  }

  // Examples
  val answer = Div(Div(Cons (1972), Cons(2)), Cons(23))
  val error = Div(Cons(1), Cons(0)) // No error checking
  assert (eval(answer) == 42)
  
}

// Uncomment sections below as you proceed

// Section 2.2 [Wadler] Variation one: Exceptions

// New type to represent computations that may raise an exception
// data M a = Raise Exception | Return a 
// type Exception = String 
object ExceptionEvaluator {

  // an implementation of Wadler's types in Scala
  type Exception = String
  trait M[+A]
  case class Raise (e: String) extends M[Nothing]
  case class Return[A] (a: A) extends M[A]

  // an implementation of direct exception evaluator in Scala:
  // Cumbersome because form of result must be checked at each call to see if exception was re-raised
  def eval (term :Term) :M[Int] = term match {
    case Cons(a) => Return (a)
    case Div(t,u) => eval (t) match {
      case Raise (e) => Raise (e) 
      case Return (a) => eval (u) match {
        case Raise (e) => Raise (e) 
        case Return (b) => {
          if (b == 0) (Raise ("divide by zero")) 
          else (Return (a / b))
        }
      }
    }
  }

  // Example
  val error = Div(Cons(1), Cons(0)) // Now Raise "divide by zero"
  assert (eval(error) == Raise("divide by zero"))

  // Once you are done reflect how massive was the change from the
  // BasicEvaluator to the exception evaluator (no need to write anything).
}

// Section 2.3 [Wadler] Variation two: State
// Desire is to count number of divisions performed during evaluation (using state in impure language)
object StateEvaluator {

  // Pure language uses state type to represent computations that act on state 
  type State = Int
  case class M[+A] (step: State => (A,State)) // Function accepts initial state and returns (value, newState)

  // Tedious: At each call, old state must be passed in, new state extracted from result, and passed on. 
  def eval (term :Term) :M[Int] = term match {
    case Cons (a) => M[Int] (x => (a,x))
    case Div (t,u) => M[Int] (x => {
      val (a,y) = eval(t).step(x) // in 
        val (b,z) = eval(u).step(y) // in 
          (a / b, z+1)
    })
  }

  val answer = Div(Div(Cons (1972), Cons(2)), Cons(23))
  assert(eval(answer).step(0) == (42,2))
}

// Section 2.4 [Wadler] Variation three: Output
// Desired to display a trace of execution (impure simply outputs side effect as command)
// Pure language mimick output using type to represent computations that generate output 
object OutputEvaluator {

  type Output = String
  case class M[+A] (o: Output, a: A) // Type contains generated output paired with computed value

  // Helper function generates one line of the output 
  def line (a :Term) (v :Int) :Output =
    "eval(" + a.toString + ") <= " + v.toString + "\n"

  // Tedious to adapt evaluator as output must be collected and assembled at each call 
  def eval (term :Term) :M[Int] = term match {
    case Cons (a) => M(line (term) (a), a)
    case Div (t,u) => {
      val M(x,a) = eval (t) // in 
        val M(y,b) = eval (u) // in 
          M(x + y + line (term) (a/b), a/b)
    }
  }
  
}

// Section 2.5 [Wadler] A monadic evaluator

// The following are two generic monadic interfaces (one for classes, one for
// meta-classes/objects) that we will use to type check our monadic solutions.
//
// We shall provide flatMap and map for our monads to be able to use for
// comprehensions in Scala.
//
// IMPORTANT: flatMap is called "(*)" in the paper.

// A monad is a type representing a computation with an effect (e.g. raise exception, act on state, generate output,)
// A monad is a triple (M, unit, flatMap) consisting of a type constructor M and two polymorphic operations
// Operations must satisfy the identity law and associativity law. 
trait Monad[+A,M[_]] {
  def flatMap[B] (k: A => M[B]) :M[B] // Bind a to result, perform computation n, return value 
  def map[B] (k: A => B) :M[B]
}

// we will provide unit, as the paper does. This will be placed in a companion
// object.

trait MonadOps[M[_]] { def unit [A] (a :A) :M[A] }

// The above abstract traits will be used to constraint types of all our monadic
// implementations, just to ensure better type safety and uniform interfaces.

// Now we are startin to implement the monadic evaluator from the paper.
// Compare this implementation to the paper, and make sure that you understand
// the Scala rendering.

// Section 2.6 [Wadler] Variation zero, revisited: The basic evaluator

object BasicEvaluatorWithMonads {

  // We enrich our M type with flatMap and map;
  // A flatMap is already in the paper (called *)
  // I add map, so that we can use for comprehensions with this type
  case class M[+A] (a: A) extends Monad[A,M] {
    def flatMap[B] (k: A => M[B]) :M[B] = k (this.a)
    def map[B] (k: A => B) :M[B] = M.unit (k (this.a))
  }

  // The paper also uses unit, so we put it in the companion object
  object M extends MonadOps[M] { def unit[A] (a : A) :M[A] = M[A] (a) }

  def eval (term: Term) :M[Int] = term match {
    case Cons (a) => M.unit (a)
    case Div (t,u) => for {
      a <- eval (t) //     (eval t).flatMap ( a => 
      b <- eval (u) //     (eval u).flatMap ( b => 
      r <- M.unit (a/b) // M.unit(a/b)) ))
    } yield r
  }

  // QUESTION: there is no need to use map???
  def eval2 (term: Term) :M[Int] = term match {
    case Cons (a) => M.unit (a) 
    case Div (t,u) => {
      (eval (t))
        .flatMap(a => (eval (u))
        .flatMap(b => M.unit(a/b)))
    }
  }

  // Make sure that you understand the above implementation (an dhow it
  // relates to the one in the paper). If you find the for comprehension to be
  // obscuring things, you may want to rewrite the above using just map and
  // flatMap.
}

// Section 2.7 [Wadler] The monadic evaluator with exceptions

object ExceptionEvaluatorWithMonads {

  type Exception = String

  trait M[+A] extends Monad[A,M] {

    def flatMap[B] (k: A => M[B]) :M[B] = this match {
       case Raise (e) => Raise (e)
       case Return (a) => k(a)
    }

    def map[B] (k: A => B) :M[B] = this match {
      case Raise (e) => Raise (e)
      case Return (a) => Return (k(a))
    }
  }

  object M extends MonadOps[M] { def unit[A] (a : A) :M[A] = Return (a) }

  case class Raise (e: String) extends M[Nothing]
  case class Return[A] (a: A) extends M[A]

  // Monad exception evaluator 
  def eval (term :Term) :M[Int] = term match {
    case Cons (a) => M.unit (a)
    case Div (t,u) => for {
      a <- eval (t) //     (eval t).flatMap ( a => 
      b <- eval (u) //     (eval u).flatMap ( b => 
      r <- if (b == 0) Raise ("divide by zero") else (M.unit (a/b)) // Raise or M.unit(a/b)) ))
    } yield r
  }

  // Discuss in the group how the monadic evaluator with exceptions
  // differs from the monadic basic one
  
  // The monadic evaluator with exceptions examines the result of the computation m. 
  // flatMap checks if exception is re-raised, otherwise returns the value instead of just doing unit(a/b).
}

// Section 2.8 [Wadler] Variation two, revisited: State

object StateEvaluatorWithMonads {

  type State = Int

  case class M[+A] (step: State => (A,State)) extends Monad[A,M] {

    // flatMap is bind or (*) in the paper
    def flatMap[B] (k :A => M[B]) = M[B] {
      x => { val (a,y) = this.step (x); k(a).step(y) } }

    def map[B] (k :A => B) :M[B] =
      M[B] { x => { val (a,y) = this.step(x); (k(a),y) } }
  }

  // Complete the implementation of unit, based on the paper
  object M extends MonadOps[M] { def unit[A] (a : A) :M[A] = M (s => (a,s)) }

  // Tick increments state x and returns empty value ()
  def tick: M[Unit] = M (x => ((), x + 1))

  // Complete the implementation of the evalutor:
  def eval2 (term :Term) :M[State] = term match {
    case Cons (a) => M.unit (a) // No need to wrap in unit below due to flatMap
    case Div (t,u) => (eval (t)).flatMap(a => eval(u).flatMap(b => tick.map(x => a/b))) 
  }

  def eval (term: Term) :M[State] = term match {
    case Cons (a) => M.unit (a) 
    case Div (t,u) => for {
      a <- eval (t) // (eval t).flatMap(a => 
      b <- eval (u) // (eval u).flatMap(b => 
      r <- tick // tick.flatMap(r => )
    } yield a/b // r.map(_ => a/b)
  }

  // Discuss in the group how the monadic evaluator with counter differs
  // from the monadic basic one (or the one with exceptions)

  // The monadic state evaluator with counter differs by using a lambda expression to increment state

}

// Section 2.9 [Wadler] Output evaluator

object OutputEvaluatorWithMonads {

  type Output = String

  case class M[+A] (o: Output, a: A) {

    // flatMap is (*) in [Wadler]
    def flatMap[B] (k :A => M[B]) = //(this.o, k(this.a)) 
    {
      val (x, a) = (this.o, this.a) // extract output x and value a from computation m 
      val (y, b) = (k(a).o, k(a).a) // extract output y and value b from computation k a 
      M (x + y, b)// returns output formed by concatenating x and y paired with value b 
      // call out x returns computation with output x and empty value 
    }

    def map[B] (k :A => B) :M[B] = M[B] (this.o, k(this.a))

  }

  // implement unit
  object M { def unit[A] (a : A) :M[A] = M("", a) } // Returns no output 

  def line (a :Term) (v :Int) :Output =
    "eval(" + a.toString + ") <= " + v.toString + "\n"

  // out x returns computation with ouput x and empty value () 
  def out (o: Output): M[Unit] = M (o, ())

  // implement eval
  def eval2 (term :Term) :M[Int] = term match {
    case Cons (a) => out(line (term) (a)).map(_ => a)
    case Div (t,u) => (eval (t)).flatMap(a => eval(u).flatMap(b => out(line (term) (a/b)).map(_ => a/b)))
  }

  def eval (term: Term): M[Int] = term match {
    case Cons (a) => out(line (term) (a)).map(_ => a)
    case Div (t, u) => for {
      a <- eval (t) // flatMap
      b <- eval (u) // flatMap 
      r <- out(line (term) (a/b)) // Input for map 
    } yield a/b  // Map 
  }

  // Discuss in the group how the monadic evaluator with output differs from
  // the monadic basic one (or the one with state/counter).

  // The output monadic evaluator differs in its computation representation in that it has an output paired with a value.
  // Question: is this the only difference????
}

