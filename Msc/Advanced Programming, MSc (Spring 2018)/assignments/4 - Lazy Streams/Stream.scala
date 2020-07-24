// Advanced Programming
// Andrzej Wąsowski, IT University of Copenhagen
// meant to be compiled, for example: fsc Stream.scala
// AUTHOR1: Thor Olesen (tvao@itu.dk)
// AUTHOR2: Dennis Nguyen (dttn@itu.dk)
// AUTHOR3: Daniel Hansen (dvao@itu.dk)
// Group number: 7 
import Stream._

// A sealed trait can only be extended within the file it is defined
// All Stream subclasses are declared within same file (sealed)
// A trait is used to share interfaces and fields between classes (similar to interface that cannot be instantiated)
sealed trait Stream[+A] {

  def headOption () :Option[A] =
    this match {
      case Empty => None
      case Cons(h,_) => Some(h())
    }

  def tail :Stream[A] = this match {
    case Empty => Empty
    case Cons(_,t) => t()
  }

  def foldRight[B] (z : =>B) (f :(A, =>B) => B) :B = this match {
    case Empty => z
    case Cons (h,t) => f (h(), t().foldRight (z) (f))
    // Note 1. f can return without forcing the tail
    // Note 2. this is not tail recursive (stack-safe) It uses a lot of stack
    // if f requires to go deeply into the stream. So folds sometimes may be
    // less useful than in the strict case
  }

  // Note 1. eager; cannot be used to work with infinite streams. So foldRight
  // is more useful with streams (somewhat opposite to strict lists)
  def foldLeft[B] (z : =>B) (f :(A, =>B) =>B) :B = this match {
    case Empty => z
    case Cons (h,t) => t().foldLeft (f (h(),z)) (f)
    // Note 2. even if f does not force z, foldLeft will continue to recurse
  }

  def exists (p : A => Boolean) :Boolean = this match {
    case Empty => false
    case Cons (h,t) => p(h()) || t().exists (p)
    // Note 1. lazy; tail is never forced if satisfying element found this is
    // because || is non-strict
    // Note 2. this is also tail recursive (because of the special semantics
    // of ||)
  }

  // Exercise 2
  def toList: List[A] = this match {
    case Empty => Nil
    case Cons(h,t) => h() :: t().toList
  }

  // Exercise 3
  /*
    Try the following test case (should terminate with no memory exceptions and very fast). Why?
    naturals.take(1000000000).drop(41).take(10).toList

    Because take is lazily implemented and the Stream is only iterated when the toList is called on
    the first 10 elements
  */
  def takeacc (n: Int): Stream[A] = {
    def acc (current: Int, n: Int): Stream[A] = this match {
      case Empty => empty
      case Cons (h,t) => if (n == current) Empty else cons(h(), t().take(n-1))
    }
    acc(0, n)
  }

  def take (n: Int): Stream[A] = this match {
      case Empty => Empty
      case Cons (h,t) =>
          if(n>0) Cons (h,()=>t().take(n-1)) else Empty
  }

  def dropacc (n: Int): Stream[A] = {
    def acc(current: Int, n: Int): Stream[A] = this match {
      case Empty => empty
      case Cons (h,t) => if (current >= n) cons(h(), t()) else t().drop(n-1)
    }
    acc(0, n)
  }

    def drop (n: Int): Stream[A] = this match {
      case Empty => Empty
      case Cons (_,t) => if(n>0) t().drop(n-1) else this
  }

  // Exercise 4
  def takeWhile(p: A => Boolean): Stream[A] = this match {
    case Empty => empty
    case Cons(h,t) => if (p(h())) cons(h(), t().takeWhile(p)) else empty
  }
  // naturals.takeWhile.(_<1000000000).drop(100).take(50).toList
  // The above test case terminates fast because it is lazy evaluated and only computes on the first 50 values

  // Exercise 5
  def forAll(p: A => Boolean) : Boolean = this match {
    case Empty => true
    case Cons(h,t) => if (p(h())) (t().forAll(p)) else false
  }
  // naturals.forAll (_ < 0)
  // Passes because the predicate returns false immediately, since naturals first number is 0 so 0 < 0 returns false
  // naturals.forAll (_ >=0)
  // Fails due to memory limit being exceeded by trying to evaluate predicate continuously on an infinitely growing sequence
  // NB: it will try to evaluate all numbers from 0 to infinite before getting out of memory
  
  // In other words
  // naturals.forAll (_ < 0) succeeds because it just have to find 1 case where p(h)!=true for the eveluation to stop, so this will work on infinite streams
  // naturals.forAll (_ >=0) crashes because the stream is infinite and it cannot find a false case to stop the evaluation
  // If the stream is finite the function will terminate at some point.

  // Exists and ForAll:
  // You should be cautious using forAll and exists, since you potentially risk having to evaluate all elements
  // On the other hand, a finite stream is fine so long the size does not exceed the memory available on the stack

  // Exercise 6
  def takeWhileFold(p: A => Boolean): Stream[A] =
    foldRight (empty:Stream[A]) ((elem, acc) => if (p(elem)) cons(elem, acc) else acc)

  // Exercise 7
  def headOptionFold () : Option[A] =
    foldRight (None:Option[A]) ((elem, acc) => 
      if (elem != empty)  Some(elem)) else acc

  def headOptionFold2() :Option[A] = //How does this cover the empty case?
      foldRight[Option[A]] (None) ((h,_) => {Some(h)})

  // Exercise 8 : map, filter, append, and flatMap using foldRight
  def map[B] (f: A => B): Stream[B] =
    foldRight (empty:Stream[B]) ((elem, acc) => cons(f(elem), acc))

  def filter (p: A => Boolean): Stream[A] =
    foldRight (empty:Stream[A]) ((elem, acc) => if(p(elem)) cons(elem, acc) else acc)

  // [B >: A] means B is constrained to be a supertype of A
  // [B <: A] means B is constrained to be a subtype of A
  // B <: A means type parameter B must be a subtype of type Y and B >: A means the opposite, B must be a super type of A
  // Analogous example: Animal >: Dog since Animal is a supertype of Dog and Dog <: Animal since Dog is a subtype of Animal
  def append[B >: A] (that: => Stream[B]): Stream[B] =
    foldRight (that) ((elem, acc) => cons(elem, acc)) // Append items from this stream onto that
  
  def append2[B>:A] (that: => Stream[B]): Stream[B] = // that is lazy loaded
    foldRight[Stream[B]] (that) (cons(_,_)) //syntactic sugar

  def flatMap[B] (f: A => Stream[B]): Stream[B] =
    foldRight (empty[B]) ((head, acc) => f(head).append(acc))

  // Exercise 9: def find (p :A => Boolean) :Option[A] = this.filter (p).headOption
  // It is efficient because it is a stream that is lazy evaluated so it only evaluates the elements that satisfy the predicate as-needed.
  // On the other hand, a list would apply the filter operation on all of its operands before returning a head element.

  // Exercise 13
  def mapUnfold[B] (f: (A => B)): Stream[B] = unfold(this) {
    case Empty => None
    case Cons(h, t) => Some(f(h()), t())
  }

  def takeWhileUnfold(p: A => Boolean): Stream[A] = unfold(this) {
    case Empty => None
    case Cons(h, t) => if (p(h())) Some(h(), t()) else None
  }

  def takeUnfoldacc (n: Int): Stream[A] = {
    def acc (current: Int, n: Int): Stream[A] = unfold(this) {
      case Empty => None
      case Cons (h,t) => if (n == current) None else Some(h(), t().take1(n-1)) // TODO: is this correct?
    }
    acc(0, n)
  }

  def takeUnfold (n: => Int): Stream[A] = unfold (this,n) { //How can unfold take to params? 
    case (Empty,_) => None //are these two empty cases necessary?
    case (_,0) => None
    case (Cons(h,t),i) => Some(h(), (t(),i-1))
  }

  // Example: Stream(1,2).zip (Stream(3,4)) (+) should yield Stream(4,6)
  //Non lazy loaded
  def zipWith[B, C] (f: (A, B) => C) (s2: Stream[B]) : Stream[C] = unfold(this, s2) {
    case (s1, s2) => s1 match {
      case Empty => None
      case Cons(h, t) => s2 match {
        case Empty => None
        case Cons(h2,t2) => Some(f(h(), h2()), (t(), t2()))
      }
    }
  }
  //Lazy loaded
  def zipWith2[B,C] (p: (A,B) => C )(that: => Stream[B]): Stream[C] = // that is lazy loaded
    unfold (this,that) 
      case (Empty,_) => None
      case (_,Empty) => None
      case (Cons(h1,t1),Cons(h2,t2)) => Some(p(h1(),h2()), (t1(),t2()))
    }

  // naturals.map (_%2==0).zipWith[Boolean,Boolean] (_||_) (naturals.map (_%2==1)).take(10).toList
  // Code zips two streams of the 10 first natural even and odd numbers and whether either number is even or odd respectively.
  // Obviously, this is always the case, as e.g. (0%2 == 0) || (1%2 == 0) returns true etc, so final Stream has 10 true values.
}

// Use extends to extend a trait
case object Empty extends Stream[Nothing]
case class Cons[+A](h: ()=>A, t: ()=>Stream[A]) extends Stream[A]

// Companion object to Stream trait with utility functions (analogous to Java factory class with static methods)
object Stream {

  def empty[A]: Stream[A] = Empty

  def cons[A] (hd: => A, tl: => Stream[A]) :Stream[A] = {
    lazy val head = hd
    lazy val tail = tl
    Cons(() => head, () => tail)
  }

  def apply[A] (as: A*) :Stream[A] =
    if (as.isEmpty) empty
    else cons(as.head, apply(as.tail: _*))
  // Note 1: ":_*" tells Scala to treat a list as multiple params
  // Note 2: pattern matching with :: does not seem to work with Seq, so we
  //         use a generic function API of Seq

  
  // Exercise 11 with A = initial state and S : value, eg unfold (0) (s => Some(s, s + 1)) to generate naturals
  // TODO: Why do I need to use the { case ... } construct inside fold instead of just (a,s)....
  def unfold[A, S](z: S) (f: S => Option[(A, S)]): Stream[A] = {
    f(z).fold (empty:Stream[A]) ({ case (a, s) => cons(a, unfold (s) (f)) })
  }

  def unfoldMap[A, S](z: S)(f: S => Option[(A, S)]): Stream[A] =
  f(z).map { case (a,b) => cons(a, unfold (b) (f) )}.getOrElse(Empty)
}

object IntStream {
  //Exercise 1
  def from (n: Int): Stream[Int] = cons(n, from(n+1))
  
  def to(n: Int) : Stream[Int] = {
        if(n<0) Empty else cons(n,to(n-1))
  }

  // Exercise 10 
  //lazy version --> Why is this much slower than non lazy?
  lazy val naturals = from (1)
  lazy val fibs = {
    def fibFunc (i: => Int) (o: => Int) : Stream[Int] = {
      cons(i,fibFunc (o) (i+o))
    }
    fibFunc (0) (1)
  }
  //Non lazy version
  def naturals2 = from(1)
  def fibs2 : Stream[Int] = {
    def f(a: Int, b: Int) : Stream[Int] = cons(a, f(b, a+b))
      f(0,1)
  }
  //Zip version
  //def fibo2 : Stream[Int] = cons(0, cons(1, fibo2.zip(fibo2.tail).map(n => n._1 + n._2)))

  //Exercise 12
  //Non lazy
  // Use ._1 and ._2 on state s to refer to tuple field one and two
  def fibs1 : Stream[Int] = unfold (0, 1) (s => Some(s._1, (s._2, s._1+s._2)))
  def from1 (n: Int): Stream[Int] = unfold (n) (s => Some(s, s + 1))

  //Lazy version
  lazy val naturals1 = unfold (1) (x => Some(x,x+1));
  lazy val fib1 = unfold (0,1) {case (a,b) => Some(a,(b,a+b))} //new state is (f_n,f_n+f_n-1)) and yield f_n
}
