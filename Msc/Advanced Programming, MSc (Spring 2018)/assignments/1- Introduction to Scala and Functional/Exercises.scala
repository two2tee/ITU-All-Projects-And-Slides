// Advanced Programming, Exercises by A. Wąsowski, IT University of Copenhagen
//
// AUTHOR1:tvao@itu.dk
// AUTHOR2:dttn@itu.dk
// AUTHOR3:daro@itu.dk
//
// Write ITU email addresses of both group members that contributed to
// the solution of the exercise (in lexicographic order).
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

// The extension of App allows writing statements at class top level (the so
// called default constructor). For App objects they will be executed as if they
// were placed in the main method in Java.

object Exercises extends App {

  // Exercise 3 : Referentially transparent and tail recursive Fibonacci implementation  
  def fib (n: Int) : Int = {
    @annotation.tailrec // Tail recursive function as recursive call is the last expression executed by function 
    def tailrec(n: Int, prev: Int, current: Int) : Int = n match {
      case 0 => current  
      case n => tailrec(n-1, current, prev + current) // fib(n-1) + fib(n-2) 
    }
    tailrec(n, 0, 1) 
  }

  // some tests (uncomment, add more):
  assert (fib (1) == 1)
  assert (fib (2) == 2)
  assert (fib (3) == 3)
  assert (fib (4) == 5)
  assert (fib (5) == 8)
  assert (fib (6) == 13)
  assert (fib (7) == 21)
  assert (fib (8) == 34)

  // Exercise 4

  // A simple object describing a cost line; implemented imperatively, Java
  // style (this way until we learn more Scala) 
  // class Expense {

  //   // A constructor definition
  //   def this (tag :String, price :Int) = {
  //     this()
  //     this.tag = tag
  //     this.price = price
  //   }

  //   var tag   :String = "" // a tag line in the accounting system
  //   var price :Int    = 0 // the price is in cents
  // }

  // Case class alternative (more conscise and expressive)
  case class Expense(tag: String, price: Int)

  // computes the total of expenses in cents
  def total (expenses: Array[Expense]) : Int = {
    @annotation.tailrec
    def tailrec(expenses: Array[Expense], acc:Int) : Int = expenses match {
      case Array() => acc 
      case _ => tailrec(expenses.tail, (acc+expenses.head.price))
    }
    tailrec(expenses, 0)
  }

  // HoF version using map and reduce 
  // Use anonymous aggregator function to sum prices starting from 0 
  def total2 (expenses: Array[Expense]) : Int = expenses.map(_.price).foldRight(0)(_+_) 

  val testcase1 = Array[Expense](
    new Expense("Coffee", 450),
    new Expense("Cake", 350) )
  assert (total (testcase1) == 800) // uncomment

  // Add one or two more tests
  val testcase2 = Array[Expense]()
  assert (total (testcase2) == 0)
  val testcase3 = Array[Expense](Expense("Shortbread", 450)) // No need to use verbose new keyword in case class   
  assert (total (testcase3) == 450)


  // Exercise 5
  @annotation.tailrec 
  def isSorted[A] (as: Array[A], ordered: (A,A) =>  Boolean) :Boolean = as match {
    case Array() => true 
    case Array(a, b, _*) => ordered (a, b) match {
      case true => isSorted (as.drop(2), ordered)
      case false => false 
    }
  }

  // some tests (uncomment)
  assert ( isSorted (Array(1,2,3,4,5,6), (a: Int, b: Int)=> a <= b))
  assert (!isSorted (Array(6,2,3,4,5,6), (a: Int, b: Int)=> a <= b))
  assert (!isSorted (Array(1,2,3,4,5,1), (a: Int, b: Int)=> a <= b))

  // add two tests with another type, for example an Array[String]
  assert ( isSorted (Array("hello", "hello"), (a: String, b: String) => a == b))
  assert (!isSorted (Array("hello", "helloWorld"), (a: String, b: String) => a == b))
  assert (isSorted (Array("hello", "helloWorld"), (a: String, b: String) => a <= b))

  // Exercise 6

  // Convert uncurried function to curried function that enables partial application, flexibility and function composition
  // Currying takes function with multiple arguments and converts to a series of functions that can take one argument only 
  // This can be used to enable partially-applied functions where you create new functions by supplying fewer parameters 
  def curry[A,B,C] (f: (A,B)=>C) : A => (B => C) = 
    //(a: A) => (b: B) => f(a,b)
    (a: A) => { (b: B) => f(a, b) }

  // test if it type checks by currying isSorted automatically
  def isSorted1[A]: Array[A] => ((A,A)=>Boolean) => Boolean = curry(isSorted) 
  assert ( isSorted1 (Array(1,2,3,4,5,6)) ((a: Int, b: Int)=> a <= b) )

  // Exercise 7

  def uncurry[A,B,C] (f: A => B => C) : (A,B) => C =
    (a: A, b: B) => f (a) (b) 

  def isSorted2[A] : (Array[A], (A,A) => Boolean) => Boolean = uncurry(isSorted1) 
  assert ( isSorted2 (Array(1,2,3,4,5,6), (a: Int, b: Int)=> a <= b))

  // Exercise 8

  // Compose applies inner function G on A and outputs B as input to outer function f that outputs C 
  def compose[A,B,C] (f: B => C, g: A => B) : A => C = (a: A) => f(g(a)) // F(G(A)) => C 



}
