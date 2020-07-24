// Advanced Programming, A. Wąsowski, IT University of Copenhagen
//
// AUTHOR1: Thor Olesen (tvao@itu.dk)
// AUTHOR2: Dennis Nguyen (dttn@itu.dk)
// AUTHOR3: Daniel Hansen (dvao@itu.dk)
// Group number: 7 
//
// Write names and ITU email addresses of both group members that contributed to
// the solution of the exercise (in alphabetical order by family name).
//
// You should work with the file by following the associated exercise sheet
// (available in PDF from the course website).
//
// The file is meant to be compiled as follows:
//
// scalac Exercises.scala
//
// or
//
// fsc Exercises.scala
//
// To run the compiled file execute "scala Tests" from a command line (or use
// another wayt to execute the Tests class from your IDE).
//
// To load the file int the REPL start the 'scala' interpreter and issue the
// command ':load Exercises.scala'. Now you can interactively experiment with
// your code.
//
// Continue solving exercises in the order presented in the PDF file. Large
// parts of the file are commented in order to make sure that the exercise
// compiles.  Uncomment and complete fragments as you proceed.  The file shall
// always compile and run after you are done with each exercise (if you do them
// in order).  Please compile and test frequently.

// Exercise  1

/* We create OrderedPoint as a trait instead of a class, so we can mix it into
 * Points (this allows to use java.awt.Point constructors without
 * reimplementing them). As constructors are not inherited, We would have to
 * reimplement them in the subclass, if classes not traits are used.  This is
 * not a problem if I mix in a trait construction time. */

trait OrderedPoint extends scala.math.Ordered[java.awt.Point] {
  this :java.awt.Point => // Restrict trait to only be mixed into subclasses of Point to access (x,y) components 
  override def compare (that :java.awt.Point) :Int = that.x match {
    case x if x < this.x || x == this.x || that.y < this.y  => -1 
    case x if x > this.x => 1
    case _ => 0 
  }
} 

// Chapter 3


sealed trait Tree[+A]
case class Leaf[A] (value: A) extends Tree[A]
case class Branch[A] (left: Tree[A], right: Tree[A]) extends Tree[A]

object Tree {

  // Exercise 2 (3.25)

  def size[A] (t :Tree[A]) :Int = t match {
    case Leaf(_) => 1  
    case Branch(left, right) => 1 + size(left) + size(right) // How to use continuation to tail recursively count size in sub trees 
  }

  // Exercise 3 (3.26)

  def maximum (t: Tree[Int]) :Int = t match {
    case Leaf(value) => value 
    case Branch(left, right) => (maximum (left)) max (maximum (right)) 
  }

  // Exercise 4 (3.28)

  def map[A,B] (t: Tree[A]) (f: A => B) : Tree[B] = t match {
    case Leaf(value) => Leaf(f (value))
    case Branch(left, right) => Branch(map (left) (f), map (right) (f))
  }

  // Exercise 5 (3.29)

  def fold[A,B] (t: Tree[A]) (f: (B,B) => B) (g: A => B) :B = t match {
    case Leaf(a) => g(a) 
    case Branch(left, right) => f(fold (left) (f) (g), fold (right) (f) (g)) 
  }

  def size1[A] (t: Tree[A]) :Int = fold (t) ((l:Int, r:Int) => 1 + l + r) (x => 1)
  def maximum1[A] (t: Tree[Int]) :Int = fold (t) ((l:Int, r:Int) => l max r) (x => x)
  def map1[A,B] (t: Tree[A]) (f: A => B) : Tree[B] = fold[A,Tree[B]] (t) ((l, r) => Branch(l,r)) (x => Leaf(f(x)))

}

sealed trait Option[+A] {

  // Exercise 6 (4.1)
  def map[B] (f: A=>B) : Option[B] = this match {
    case None => None 
    case Some(a) => Some(f(a)) // f can make map fail 
  }

  // Ignore the arrow in default's type below for the time being.
  // (it should work (almost) as if it was not there)

  def getOrElse[B >: A] (default: => B) :B = this match {
    case None => default 
    case Some(b) => b 
  }

  def flatMap[B] (f: A=>Option[B]) : Option[B] = this match {
    case None => None 
    case Some(a) => f(a) match { // flatMap will indicate if computation fails 
      case None => None 
      case Some(b) => Some(b) 
    }
  }

  
  //different version flatmap only applies function and return underlying option but is this correct?
  def flatMap2[B] (f: A=>Option[B]) : Option[B] = {
    this match {
      case None => None
      case Some(x) => f(x)
    }
  }

  def filter (f: A => Boolean) : Option[A] = this match {
    case None => None 
    case Some(a) => if (f(a)) Some(a) else None 
  }

  //Different version but is this correct
  def filter2 (f: A => Boolean) : Option[A] = {
    flatMap(a => if(f(a)) Some(a) else None)
  }


}

// Question : why is this outside the sealed trait Option type?
case class Some[+A] (get: A) extends Option[A]
case object None extends Option[Nothing]

object ExercisesOption {

  // Remember that mean is implemented in Chapter 4 of the text book

  def mean(xs: Seq[Double]): Option[Double] = {
    if (xs.isEmpty) None
    else Some(xs.sum / xs.length)
  }
  // Exercise 7 (4.2) 
  // Mean = average = #sum of numbers / # number count 
  // Variance = math.pow(x - m, 2) for each element x in sequence 
  // for { bindings...} yield consists of flatMap bindings and final yield of map result 
  def variance (xs: Seq[Double]) : Option[Double] = {
    if (xs.isEmpty) None 
    else {
      val seqMean = mean(xs)
      mean(xs).flatMap(elem => math.pow(elem - seqMean, 2))
    }
  }

    //Different version of variance 
  def variance2 (xs: Seq[Double]) : Option[Double] = {
    if(xs.isEmpty) None
    else {
      mean(xs) flatMap(m => mean(xs.map(x => math.pow(x - m, 2))))
      }
  }

  // def variance2 (xs: Seq[Double]) : Option[Double] = {
  //   for {
  //     // Do work 
  //   } yield result 
  //  }
  
  // Exercise 8 (4.3)

  // Map function is lifted to operate within context of two Option values 
  // Question: is this correct and can it be shortened (way we fetch option values)?
  def map2[A,B,C] (ao: Option[A], bo: Option[B]) (f: (A,B) => C) :Option[C] = 
    if (ao == None || bo == None) None 
    else { 
      val Some(a) = ao 
      val Some(b) = bo
      Some(f(a,b)) 
    }


    
  //Correct standard
  def map2clean[A,B,C] (ao: Option[A], bo: Option[B]) (f: (A,B) => C) :Option[C] ={
    ao flatMap(a => bo map(b=> f(a,b)))
  }

  //for version
  def map2for[A,B,C] (ao: Option[A], bo: Option[B]) (f: (A,B) => C) :Option[C] ={
    for{
      a <- ao //flatmap
      b <- bo //map
    } yield f(a,b)
  }

  
    
  // Exercise 9 (4.4)
  // Question: is this correct and can it be shortened somewhat?
  def sequence[A] (aos: List[Option[A]]) : Option[List[A]] = {
    aos.foldRight (Some(Nil):Option[List[A]]) ((elem, acc) => {
      if (elem == None) None 
      else {
        val Some(a) = elem 
        Some(a :: acc.getOrElse(Nil)) 
      }
    })
  }

  //Als another version we tried out but we still need to know which way is correct
  def sequence2[A] (aos: List[Option[A]]) : Option[List[A]] = {
    val result =aos.foldLeft(List[A]())((acc,a) =>
                    a match {
                      case None => acc
                      case Some(v) => v::acc
                    })

    if(result.isEmpty) None else Some(result)
  }

  // Exercise 10 (4.5)
  def traverse[A,B] (as: List[A]) (f :A => Option[B]) :Option[List[B]] = as match {
    case Nil => Some(Nil) 
    case x::xs => f(x) match {
      case None => None 
      case Some(b) => Some(b :: (traverse (xs) (f)).getOrElse(Nil))
    }
  }

  //Another proposed solution of traverse but some of us are a bit in doubt in how this works with (_::_)
  def traverse2[A,B] (as: List[A]) (f :A => Option[B]) :Option[List[B]] = {
    as match {
      case Nil => Some(Nil)
      case x::xs => map2(f(x),traverse(xs)(f))(_::_) 
    }
  }

}

// Test cases for running in the compiled vesion (uncomment as you go, or paste
// them into REPL in the interactive version)

object Tests extends App {

  // Exercise 1
  val p = new java.awt.Point(0,1) with OrderedPoint
  val q = new java.awt.Point(0,2) with OrderedPoint
  assert(p < q)

  // Notice how we are using nice infix comparison on java.awt
  // objects that were implemented way before Scala existed :) (And without the
  // library implementing a suitable comparator). We did not have to recompile
  // java.awt.Point


  // Exercise 2
  assert (Tree.size (Branch(Leaf(1), Leaf(2))) == 3)
  //Exercise 3
  assert (Tree.maximum (Branch(Leaf(1), Leaf(2))) == 2)
  //Exercise 4
  val t4 = Branch(Leaf(1), Branch(Branch(Leaf(2),Leaf(3)),Leaf(4)))
  val t5 = Branch(Leaf("1"), Branch(Branch(Leaf("2"),Leaf("3")),Leaf("4")))
  assert (Tree.map (t4) (_.toString) == t5)

  // Exercise 5
  assert (Tree.size1 (Branch(Leaf(1), Leaf(2))) == 3)
  assert (Tree.maximum1 (Branch(Leaf(1), Leaf(2))) == 2)
  assert (Tree.map1 (t4) (_.toString) == t5)

  // Exercise 6
  assert (Some(1).map (x => x +1) == Some(2))
  assert (Some(41).getOrElse(42) == 41)
  assert (None.getOrElse(42) == 42)
  assert (Some(1).flatMap (x => Some(x+1)) == Some(2))
  assert ((None: Option[Int]).flatMap[Int] (x => Some(x+1)) == None)
  assert (Some(42).filter(_ == 42) == Some(42))
  assert (Some(41).filter(_ == 42) == None)
  assert ((None: Option[Int]).filter(_ == 42) == None)

  // Exercise 7
  //ExercisesOption.variance (List(42,42,42))
  // assert (ExercisesOption.variance (List(42,42,42)) == Some(0.0))
  // assert (ExercisesOption.variance (List()) == None)


  // Exercise 8
  assert (ExercisesOption.map2 (Some(42),Some(7)) (_ + _) == Some(49))
  assert (ExercisesOption.map2 (Some(42),None) (_ + _) == None)
  assert (ExercisesOption.map2 (None: Option[Int],Some(7)) (_ + _) == None)
  assert (ExercisesOption.map2 (None: Option[Int],None) (_ + _) == None)

  // Exercise 9
  assert (ExercisesOption.sequence (List(Some(1), Some(2), Some(42))) == Some(List(1,2,42)))
  assert (ExercisesOption.sequence (List(None,    Some(2), Some(42))) == None)
  assert (ExercisesOption.sequence (List(Some(1), None,    Some(42))) == None)
  assert (ExercisesOption.sequence (List(Some(1), Some(2), None    )) == None)

  // Exercise 10
  def f (n: Int) :Option[Int] = if (n%2 == 0) Some(n) else None
  assert (ExercisesOption.traverse (List(1,2,42)) (Some(_)) == Some(List(1,2,42)))
  assert (ExercisesOption.traverse (List(1,2,42)) (f) == None)

}
