// Name: Dennis Thinh Tan Nguyen
// ITU email: dttn@itu.dk
package adpro.exam2018

import fpinscala.monoids.Monoid
import fpinscala.monads.Monad
import fpinscala.monads.Functor
import fpinscala.laziness.{Cons, Empty, Stream}
import fpinscala.laziness.Stream._
import fpinscala.parallelism.Par._

import scala.language.higherKinds
import adpro.data._
import adpro.data.FingerTree._
import fpinscala.parallelism.Par
import fpinscala.state.State
import monocle.Lens

object Q1 { 

  def groupByKey[K,V] (l :List[(K,V)]) :List[(K,List[V])] = {
    def go(l: List[(K,V)], acc:List[(K,List[V])]):List[(K,List[V])] = l match {
      case Nil => acc
      case x :: xs =>
        if(!acc.exists(pair => pair._1 == x._1))  { //Insert new key value
          val updated = (x._1,x._2::Nil)::acc
          go(xs, updated)
        }
        else {
          val updated: List[(K,List[V])] =
            acc.map(pair => if (pair._1 == x._1) (x._1,x._2 :: pair._2) else pair) // update existing
          go(xs, updated)
        }
    }
    go(l,Nil)
  }

}


object Q2 { 

  def f[A,B] (results: List[Either[A,B]]) :Either[List[A],List[B]] = {
    val(errors,success) = results.foldRight((List.empty[A],List.empty[B]))((either,acc) => either match {
      case Left(a:A)  => (a::acc._1,acc._2)
      case Right(b:B) => (acc._1,b :: acc._2)
    })
    if (errors.isEmpty) Right(success) //If all computations is okay return list of computed results
    else Left(errors)                  //If any errors return list of errors
  }
}

object Q3 {

  val optionMonad = new Monad[Option] {
    override def unit[A](a: => A): Option[A] = Some(a)
    override def flatMap[A, B](ma: Option[A])(f: A => Option[B]): Option[B] = ma.flatMap(f)
  }

  type T[B] = Either[String,B]
  implicit val eitherStringIsMonad :Monad[T] = new Monad[T] {
    def unit[B](b: => B): T[B] = Right(b)
    def flatMap[B](ma: T[String])(f: String => T[B]): T[B] = ma.flatMap(f)
  }


  implicit def eitherIsMonad[A] = {
    type T[B] = Either[A,B]
    val eitherAIsMonad: Monad[T] = new Monad[T] {
      def unit[B](b: => B): T[B] = Right(b)
      def flatMap[B](ma: T[A])(f: A => T[B]): T[B] = ma.flatMap(f)
    }
  }

} // Q3


object Q4 {

//  val s = State[(Int,Int), Int] {x => (x._1,(x._2,x._1+x._2))}
//  val (y,s1) = s.run((1,1))
//  val (z,s2) = s.run(s2)

  // Write the answers in English below.
   
  // A. Given (1,1) running the automaton, the first 5 elements produced is:
  //    (1,1) produces (1,(1,2)) -> (1,(2,3)) -> (2,(3,5)) -> (3,(5,7)) -> (5,(7,12))
  // B.Given (1,1) all possible state values is an infinite stream of integers greater than 0
   
}


object Q5 { 

  def parForall[A] (as: List[A]) (p: A => Boolean): Par[Boolean] = {
    val booleans = Par.parMap(as)(a => p(a)) // Parallel mapping of all elements in list to get booleans
    Par.map(booleans)(bs => bs.forall(identity)) //map all booleans and check that all results are true
  }
}


object Q6 {

  def apply[F[_],A,B](fab: F[A => B])(fa: F[A]): F[B] = ???
  def unit[F[_],A](a: => A): F[A] = ???
  val f: (Int,Int) => Int = _ + _
  def a :List[Int] = ???

  val x = apply(apply(unit(f.curried))(a))(a)

  // Answer below in a comment:
  /*
   * Calling apply(apply(unit(f))(a))(a) would produce a value of type List[Int]
   * It is well typed regardless of the compiler error because given the input a of List[Int], a
   * is a monad and would thus satisfy apply and unit since they take a monad F[_] as a type parameter.
   * Yet apply and unit uses higher ordered types but given no explicit type parameters for the two functions, the
   * compiler is unable to find explicits params such that that they can be applied as arguments and therefore an error
   * is produced.
   */

} // Q6


object Q7 {

  def map2[A,B,C] (a :List[A], b: List[B]) (f: (A,B) => C): List[C] = {
    a.flatMap(aa => b.map(bb => f(aa,bb)))
  }


  def map3[A,B,C,D] (a :List[A], b: List[B], c: List[C]) (f: (A,B,C) => D) :List[D] = {
    val aux = (t:(A, B),c:C) => f(t._1,t._2,c) // creating and aux function to wrap the params A and B into a tuple
    val AB = map2(a,b)((a,b) =>(a,b))          // apply map2 on the first two lists producing a tupled list of (A,B)
    map2(AB,c)((ab,c)=> aux(ab,c))             // then apply map2 on the tuples AP with C on our aux function
  }


  /*
   * I did not manage to get the enforcement working but the concept is
   * to check whether M is and instance of monad and then convert
   * M to the correct type such that the the monad's map2 function is applicable.
   * Once the monad is enforced the rest of the implementation would be similar to
   * the above map3, but the difference is it would be using the monad's implementation of map2
   */
  def map3monad[M[_],A,B,C,D](a: M[A], b: M[B], c: M[C])(f: (A,B,C) => D): M[C] = {
      if (Monad.isInstanceOf[M]) //Check if M is an instance of a monad
      type m = Monad[M] //Set type - conceptual
      val aux = (t:(A, B),c:C) => f(t._1,t._2,c)   // creating and aux function to wrap the params A and B into a tuple
      val AB = m.map2(a,b)((a,b) =>(a,b))          // apply map2 of monad on the first two lists producing a tupled list of (A,B)
      m.map2(AB,c)((ab,c)=> aux(ab,c))             // then apply map2 of monad on the tuples AP with C on our aux function
  }

} // Q7


/**
  * Doesn't compile 100%, but the concept is to traverse the finger tree and check all fingers satisfy
  * the given predicate and prune those that does not
  * It does not compile because when filter is called on the spine 'm', the compiler is unable to match predicate
  * of A with Node[A]
  */
object Q8 {
  def filter[A] (t: FingerTree[A]) (p: A => Boolean): FingerTree[A] = t match {
    case Empty()  => Empty()
    case Single(x) => if(p(x)) Single(x) else Empty()
    case Deep(pr,m,sf) => Deep[A](filterDigit(pr)(p),filter(m)(p),filterDigit(sf)(p))
  }

  def filterDigit[A](d: Digit[A])(p: A => Boolean) : Digit[A] = {
   d.foldRight(Digit()[A])((elem,acc) => if(p(elem)) elem::acc else acc )
  }
}


object Q9 {

  def eitherOption[A,B] (default: => A): Lens[Either[A,B],Option[B]] = {
    Lens[Either[A, B], Option[B]] {
      case Right(x) => Some(x)
      case Left(d) => None
    }(a => _ => a match {
      case Some(x) => Right(x)
      case None => Left(default)
    })
  }

  val l = eitherOption[String,Int]("error")
  val a = None
  val a1 = Some(2)
  val c = Left("error2")

  l.get ( l.set(a)(c) ) == a
  l.set (l.get(c))(c) == c
  l.set (a1)(l.set(a)(c)) == l.set(a1)(c)


  // Answer the questions below:

  // A. Yes : based on the above example l.get ( l.set(a)(c) ) == a and would thus satisfy the put get law
  // Inserting an abstract view None would yield the concrete view Left and getting Left would yield None again

  // B. No : based on the above example l.set (l.get(c))(c) != c  and would thus not satisfy the get put law
  // Assumed that c has the value "error2", reinserting a none value would yield a concrete value with its default value "error"
  // hence Left("error") != Left("error2")

  // C. Yes : based on the above example l.set (a1)(l.set(a)(c)) == l.set(a1)(c) put put
  // Thus inserting None and then Some(2) into C would be equally as inserting Some(2) into C

} // Q9

