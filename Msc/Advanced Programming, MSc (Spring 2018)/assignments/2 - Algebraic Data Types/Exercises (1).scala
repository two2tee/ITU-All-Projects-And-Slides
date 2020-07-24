// Advanced Programming 2018,
// A. Wąsowski, Z.Fu. IT University of Copenhagen
//
// AUTHOR1: tvao@itu.dk (Thor)
// AUTHOR2: dttn@itu.dk (Dennis)
// AUTHOR3: daro@itu.dk (Daniel) 
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
// To run the compiled file do "scala Exercises"
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
//
// Please only hand in files that compile (comment out or use '???' for the
// parts you don't know how to solve or cannot complete).

/* Exercise 1 : Results of match expressions 
    List(1,2,3,4,5) will match the pattern on case 4
      Cons(x, Cons(y, Cons(3, Cons(4, _)))) -> Cons(1, Cons(2, Cons(3, Cons(4, _)))
    and returns x + y <=> 1 + 2 = 3 
    It will not match on case 5 as the pattern match contruct greedily matches on the patterns going from the top.
    Hence, since the list is decomposed in case 4, it never reaches case 5, despite the pattern being perfectly valid.
*/

// An ADT of Lists
sealed trait List[+A]
case object Nil extends List[Nothing]
case class Cons[+A](head: A, tail: List[A]) extends List[A]

object List {

  // override function application to provide a factory of lists (convenience)

  def apply[A](as: A*): List[A] = // Variadic function
    if (as.isEmpty) Nil
    else Cons(as.head, apply(as.tail: _*))

  // Exercise 2

  // On case Nil you can choose to return Nil or Cons(Nil, Nil) signifying the same empty list result
  // Student Question: why is Nil an object and Cons a class signature?
  def tail[A] (as: List[A]) : List[A] = as match {
    case Nil => Nil 
    case Cons(h, t) => t 
  }

  // Solution 2 - which one do you prefer?
  def tail2[A] (as: List[A]) :List[A] = {
    as match {
      case Nil => Nil
      case Cons(_,Nil) => Nil
      case Cons(_,t) => t
    }
  }

  // Exercise 3

  def drop[A] (l: List[A], n: Int) : List[A] = n match {
    case n if n <= 0 => l // Negative or zero 
    case n => l match {
      case Nil => Nil 
      case Cons(h,t) => drop (t, n-1)
    }
  }

  // Exercise 4

  def dropWhile[A](l: List[A], f: A => Boolean): List[A] = l match {
    case Nil => Nil 
    case Cons(h,t) => f(h) match {
      case true => dropWhile(t, f)
      case false => Cons(h, dropWhile(t, f))
    }
  }

  // Exercise 5

  def init[A](l: List[A]): List[A] = l match {
    case Nil => Nil
    case Cons (h, Nil) => Nil  
    case Cons(h,t) => Cons(h, init(t))
  }

  /*
    Question: Is this function constant time, like tail? Is it constant space?

    No, the function recursively builds a list starting from the head until it reaches the end of the list and skips the last element.
    Thus, the function is linear time. Also, the function builds a list that is linear in size to the number of elements.
  */


  // Exercise 6
  // Example: foldRight (List(1,2,3,4), 0) (_+_) 
  def foldRight[A,B] (as :List[A], z: B) (f : (A,B)=> B) :B = as match {
    case Nil => z
    case Cons (x,xs) => f (x, foldRight (xs,z) (f)) // Not tail recursive as foldRight depends on evaluation of f 
  }

  def length[A] (as: List[A]): Int = foldRight (as, 0) ((_, n:Int) => n + 1)

  // Exercise 7
  // Tail recursive fold iterates from the left side 
  def foldLeft[A,B] (as: List[A], z: B) (f: (B, A) => B) : B = as match {
    case Nil => z 
    case Cons(x,xs) => foldLeft(xs, f(z, x)) (f) 
  }

  // Exercise 8
  def product (as :List[Int]) : Int = as match {
    case Nil => 1
    case Cons(x,xs) => x * product(xs) 
  }
  def product2 (as :List[Int]) : Int = foldLeft (as,0) (_*_) // Conscise version 

  def length1 (as :List[Int]) : Int = 
    foldLeft (as, 0) ((acc, _) => acc+1)

  // Exercise 9

  def reverse[A] (as :List[A]) :List[A] = 
    foldLeft (as, Nil:List[A]) ((acc, elem) => Cons(elem, acc)) 
    // Question : Use Nil or List() as start value?
    //foldLeft[A,List[A]](as,List()){(acc,x)=> Cons(x,acc)}

  // Exercise 10

  def foldRight1[A,B] (as: List[A], z: B) (f: (A, B) => B) : B = 
    foldLeft (List.reverse(as), z) ((b: B, a: A) => f(a,b)) //(f) 

  // Alternative definition ?
  def foldRight2[A,B] (as: List[A], z: B) (f: (A, B) => B) : B = {
      val reversed = List.reverse(as)
      def g(f: (A, B) => B): (B,A) => B = { (B,A) => f(A,B) }
      foldLeft(as,z)(g(f))
  }


  // Question : How do we do this function as a partial value? 
  def foldLeft1[A,B] (as: List[A], z: B) (f: (B,A) => B) : B = {
    val partialFold = foldRight (as, z) _ // ((a: A, b: B) => f(b,a)) 
    partialFold ((a: A, b: B) => f(b,a))  
  }

  // Exercise 11

  def append[A](a1: List[A], a2: List[A]): List[A] = a1 match {
    case Nil => a2
    case Cons(h,t) => Cons(h, append(t, a2))
  }


  def concat[A] (as: List[List[A]]) :List[A] = {
    reverse( foldLeft (as,List[A]()) {(acc,x) => append (reverse(x),acc) })
  }

  // Exercise 12

  def filter[A] (as: List[A]) (f: A => Boolean) : List[A] = as match {
    case Nil => Nil 
    case Cons(h,t) => f(h) match {
      case true => Cons(h, filter (t) (f)) // Satisfy -> Do not remove 
      case false => filter (t) (f) // Remove  
    }
  }

  // Alternative filter using accumulator 
  def filter2[A] (as: List[A]) (f: A => Boolean) : List[A] = {
    val res =foldLeft(as,List[A]()){(acc,x)=> {
      if(f(x))Cons(x,acc)
      else acc
    }}
    reverse(res)
  }

  // Exercise 13
  def flatMap[A,B](as: List[A])(f: A => List[B]) : List[B] = as match {
    case Nil => Nil 
    case Cons(h,t) => append (f(h), flatMap (t) (f)) // Perhaps append bottleneck?
  }

  // Tail recursive flat map
  def flatMap2[A,B](as: List[A])(f: A => List[B]) : List[B] = {
    foldLeft(as,List[B]())( (acc,x)=> append(acc,f(x)) )
  }


  // Exercise 14
  def filter1[A] (l: List[A]) (p: A => Boolean) :List[A] = 
    flatMap (l) ((a: A) => {
      p(a) match {
        case true => List(a) // Satisfy -> Do not remove 
        case false => Nil // Remove 
      }
    })

  // Exercise 15

  def add (l: List[Int]) (r: List[Int]): List[Int] = l match {
    case Nil => Nil  
    case Cons(h,t) => r match {
      case Nil => Nil 
      case Cons(h2,t2) => Cons(h+h2, add (t)(t2) )
    } 
  }

  // Question: is this tail-recursive version too verbose?
  def add2 (l: List[Int]) (r: List[Int]): List[Int] =
  {
    //@tailrec
    def addAcc(l: List[Int]) (r: List[Int])(acc: List[Int]): List[Int] ={
      l match{
        case Nil => acc
        case Cons(x,xs) => {
          r match{
            case Nil => acc
            case Cons(y,ys) => addAcc(xs)(ys)(Cons(x+y,acc))
          }
        }
      }
    }
    reverse(addAcc(l)(r)(List[Int]()))
  }

  // Exercise 16
  // Student Question: is this not reduce?
  def zipWith[A,B,C] (f : (A,B)=>C) (l: List[A], r: List[B]) : List[C] = l match {
    case Nil => Nil 
    case Cons(h,t) => r match {
      case Nil => Nil 
      case Cons(h2,t2) => Cons(f(h,h2), zipWith (f) (t, t2))
    }
  }

  // Question: is this tail recursive version too verbose?
  def zipWith2[A,B,C] (f : (A,B)=>C) (l: List[A], r: List[B]) : List[C] = {
    //@tailrec
    def zipWithAcc(l: List[A]) (r: List[B])(acc: List[C]): List[C] ={
      l match{
        case Nil => acc
        case Cons(x,xs) => {
          r match{
            case Nil => acc
            case Cons(y,ys) => zipWithAcc(xs)(ys)(Cons(f(x,y),acc))
          }
        }
      }
    }
    reverse(zipWithAcc(l)(r)(List[C]()))
  }

  // Exercise 17
  

  def hasSubsequence[A] (sup: List[A], sub: List[A]) :Boolean = sub match {
    case Nil => true // Empty sequence is subsequence of any other sequence 
    case Cons(h,t) => sup match {
      case Nil => false 
      case Cons(h2,t2) => if (h == h2) (hasSubsequence (t,t2)) else (hasSubsequence (t2,sub)) 
    }
  }


  // Exercise 18

  def pascal (n: Int) :List[Int] = {
    def pascalAcc (n: Int) (acc : List[Int]) :List[Int] = n match {
    case n if n <= 1 => acc 
    case n => {
      val shiftedRow = Cons(0,acc)
      val originalRow = append (acc, List(0)) // Bottleneck: how can we add a 0 for padding to end of original row w/o linear time append? 
      val newRow = zipWith ((x:Int, y:Int) => x+y) (shiftedRow, originalRow) 
      pascalAcc (n-1) (newRow) 
      }
    }
    pascalAcc (n) (List(1))
  }

}

// Test program 
object Program extends App {

  // Test tail (Exercise 2) 
  assert (List.tail(Nil) == Nil)
  assert(List.tail(List(1,Nil)) == Cons(Nil, Nil))
  assert (List.tail(List(1,2)) == Cons(2,Nil))

  // Test drop (Exercise 3)
  assert ( List.drop(List(1,2), 2) == Nil)

  // Test dropWhile (Exercise 4) 
  assert ( List.dropWhile(List(1,2,3,4), (l: Int) => l % 2 == 1) == List(2,4)) // Drop odd numbers 
  assert ( List.dropWhile(List(1,2,3,4), (l: Int) => l % 2 == 0) == List(1,3)) // Drop even numbers 
  
  // Test init (drop last Exercise 5)
  assert ( List.init(List(1,2,3)) == List(1,2))

  // Test length based on fold (Exercise 6)
  assert (List.length(List(1,2)) == 2)

  // Test foldLeft (Exercise 7) 
  assert (List.foldLeft(List(1,2,3,4), 0) (_ + _) == 10)

  // Test product and length1 (Exercise 8) 
  assert (List.product(List(1,2,3)) == 6)
  assert (List.length1(List(1,2,3)) == 3)
  
  // Test reverse (Exercise 9)
  assert (List.reverse(List(1,2,3)) == List(3,2,1))

  // Test foldLeft1, foldRight1 (Exercise 10) 
  //   println("Fold Left (Tail Recursive: ")
  // List.foldLeft1(List(1,2,3,4), 0) ((acc: Int, x:Int) => { 
  //   println(x) 
  //   acc+x 
  // })
  // println("Fold Right (Backwards): ")
  // List.foldRight1(List(1,2,3,4), 0) ((acc: Int, x:Int) => { 
  //   println(x) 
  //   acc+x 
  // })

  // Test concat (Exercise 11)
  assert ( List.concat( List(List(1,2)) ) == List(1,2))

  // Test filter (Exercise 12)
  assert ( List.filter (List(1,2,3,4)) ((a:Int) => a % 2 == 0) == List(2,4))

  // Test flatMap (Exercise 13)
  assert ( List.flatMap (List(1,2,3)) (i => List(i,i)) == List(1,1,2,2,3,3) )

  // Test filter1 (Exercise 14)
  assert ( List.filter1 (List(1,2,3,4)) ((a:Int) => a % 2 == 0) == List(2,4))

  // Test add (Exercise 15)
  assert ( List.add (List(1,2,3)) (List(4,5,6,7)) == List(5,7,9) )

  // Test zipWith (Exercise 16, analogous to reduce?)
  assert ( List.zipWith ((a:Int, b:Int) => a*b) (List(1,2,3,4), List(1,2,3)) == List(1,4,9) )

  // Test hasSubsequence (Exercise 17)
  assert ( List.hasSubsequence (List(1,2,3,4), List(2,3)) == true )
  assert ( List.hasSubsequence (List(1,2,3,4), List(4)) == true )
  //assert ( List.hasSubsequence (List(1,2,3,4), Nil) == true )
  assert ( List.hasSubsequence (List(1,2,3,4), List(1,3)) == false )

  // Test Pascal (Exercise 18)
  assert ( List.pascal (1) == List(1) )
  assert ( List.pascal (2) == List(1,1) )
  assert ( List.pascal (3) == List(1,2,1) )
  assert ( List.pascal (4) == List(1,3,3,1) )
  assert ( List.pascal (5) == List(1,4,6,4,1))

  println("All Tests Passed...")
}
