package adpro
import adpro.data.FingerTree._

import scala.language.higherKinds

// The implementation is based on Section 3 of the paper.
//
// This implementation is designed to be eager, following the regular strictness
// of Scala.  However it would be an interesting exercise to extend it so that
// it is possibly lazy, like in the paper of Hinze and Paterson.  The obvious
// choice is to make values of elements stored in the queue lazy.  Then there is
// also a discussion of possible suspension of the middle element of the tree on
// page 7.

// Complete the implementation of Finger Trees below.  Incomplete
// places are marked ...
//
// I am Simulating a package with an object, because type declarations
// can only be placed in objects (so this allows me to place Digit on top).

object data {

  // The interface spec for reducible structures, plus two useful derived
  // reducers that the paper introduces (toList and toTree)

  // I changed the type of reducers to not use curried operators, but regular
  // binary operators.  This is more natural in Scala, and gives easier to read
  // syntax of expressions.  Curried style is preferred in Haskell.

  trait Reduce[F[_]] {
    def reduceR[A,B] (opr: (A,B) => B) (fa: F[A], b: B) :B
    def reduceL[A,B] (opl: (B,A) => B) (b: B, fa: F[A]) :B

    // page 3

    // A is A and B is List[A] result
   def toList[A] (fa: F[A]) :List[A] = {
     reduceR[A, List[A]] ((elem, acc) => elem :: acc) (fa, List())
   }

    // page 6
    // F[A] is e.g. List[A] so should return FingerTree[Node[A]]
     def toTree[A] (fa :F[A]) :FingerTree[A] = {
      reduceR[A, FingerTree[A]] ((elem, acc) => addL(elem, acc)) (fa, Empty())
    }

  }

  // Types for Finger trees after Hinze and Pattersoni (page 4)

  type Digit[A] = List[A]

  sealed trait Node[+A] {

    // uncomment the delagation once Node.toList is implemented
     def toList :List[A] = Node.toList (this)
  }

  case class Node2[A] (l :A, r :A) extends Node[A]
  case class Node3[A] (l: A, m: A, r: A) extends Node[A]

  sealed trait FingerTree[+A] {

    // The following methods are convenience delagation so we can use
    // the operations both as methods and functions.
    // Uncomment them once you have implemented the corresponding functions.

     def addL[B >:A] (b: B) :FingerTree[B] = FingerTree.addL (b,this)
     def addR[B >:A] (b: B) :FingerTree[B] = FingerTree.addR (this,b)
     def toList :List[A] = FingerTree.toList (this)

     def headL :A = FingerTree.headL (this)
     def tailL :FingerTree[A] = FingerTree.tailL (this)
//     def headR :A = FingerTree.headR (this)
//     def tailR :FingerTree[A] = FingerTree.tailR (this)

    // page 7 (but this version uses polymorphis for efficiency, so we can
    // implement it differently; If you want to follow the paper closely move them to
    // FingerTree object and delegate the methods, so my tests still work.
     def empty :Boolean = FingerTree.viewL(this) match {
      case NilTree() => true
      case _ => false
    }
     def nonEmpty :Boolean = !empty
  }
  case class Empty () extends FingerTree[Nothing] {

    // page 7
    //
     override def empty =  true
     override def nonEmpty = false
  }
  case class Single[A] (data: A) extends FingerTree[A]
  // paramter names: pr - prefix, m - middle, sf - suffix
  case class Deep[A] (pr: Digit[A], m: FingerTree[Node[A]], sf: Digit[A]) extends FingerTree[A]

  val treeExample = Deep[Int] (List(1,2), Single(Node3(1,2,3)), List(0))

  // page 6
  //
  // Types of views on trees
  // The types are provided for educational purposes.  I do not use the view
  // types in my implementation. I implement views as Scala extractors.
  // But you may want to implement views first like in the paper, and then
  // conver them to Scala extractors.

  // In the paper views are generic in the type of tree used. Here I make them
  // fixed for FingerTrees.

    sealed trait ViewL[+A]
    case class NilTree () extends ViewL[Nothing]
    case class ConsL[A] (hd: A, tl: FingerTree[A]) extends ViewL[A]

  // Left extractors for Finger Trees (we use the same algorithm as viewL in the
  // paper). You can do this, once you implemented the views the book way.
  // Once the extractors are implemented you can pattern match on NilTree, ConsL
  // and ConsR
  //
  // See an example extractor implemented for Digit below (Digit.unapply)

  // Apply is like a constructor which takes arguments and creates object
  // Unapply method takes an object and tries to give back arguments
  object NilTree { // we use the same extractor for both left and right views
    def unapply (t: Any): Boolean = true // TODO: How do I unapply NilTree?
//     def unapply[A] (t: FingerTree[A]) :Boolean = true
  }

  object ConsL {
//     def unapply[A] (t: FingerTree[A]) :Option[(A,FingerTree[A])] = Some((t.headL, t.tailL)) // TODO: correct?
  }

  object ConsR {
//     def unapply[A] (t: FingerTree[A]) :Option[(FingerTree[A],A)] = Some((t.tailR, t.headR)) // TODO: correct?
  }

  // several convenience operations for Digits.
  //
  object Digit extends Reduce[Digit] { // uncomment once the interfaces are provided

    // page 3, top
    //
    def reduceR[A,Z] (opr: (A,Z) => Z) (d: Digit[A], z: Z) :Z = d.foldRight (z) (opr)
     def reduceL[A,Z] (opl: (Z,A) => Z) (z: Z, d: Digit[A]) :Z = d.foldLeft (z) (opl)

    // Digit inherits toTree from Reduce[Digit] that we will also apply to other
    // lists, but this object is a convenient place to put it (even if not all
    // lists are digits)

    // This is a factory method that allows us to use Digit (...) like a
    // constructor
    def apply[A] (as: A*) : Digit[A] = List(as:_*)

    // This is an example of extractor, so that we can use Digit(...) in pattern
    // matching.  Case classes have extractors automatically, but Digit defined
    // as above is not a case class, but just a type name.
    def unapplySeq[A] (d: Digit[A]): Option[Seq[A]] = Some (d)
  }


  object Node extends Reduce[Node] {

    // page 5, top
     def reduceR[A,Z] (opr: (A,Z) => Z) (n :Node[A], z: Z) :Z = n match {
       case Node2(l, r) => opr(l, opr(r, z))
       case Node3(l, m, r) => opr(l, opr(m, opr(r, z)))
     }

     def reduceL[A,Z] (opl: (Z,A) => Z) (z: Z, n :Node[A]) :Z = n match {
       case Node2(l, r) => opl(opl(z, l), r)
       case Node3(l, m, r) => opl(opl(opl(z, l), m), r)
     }
   }



  // Most of the paper's key functions are in the module below.

  object FingerTree extends Reduce[FingerTree] {

    // page 5
     def reduceR[A,Z] (opr: (A,Z) => Z) (t: FingerTree[A], z: Z) :Z = t match {
       case Empty() => z
       case Single(x) => opr(x, z)
       case Deep(pr, m, sf) => // TODO HARD
         Digit.reduceR (opr) (pr, reduceR(Node.reduceR(opr)) (m, Digit.reduceR(opr)(sf, z)))
     }

     def reduceL[A,Z] (opl: (Z,A) => Z) (z: Z, t: FingerTree[A]) :Z = t match {
       case Empty() => z
       case Single(x) => opl(z, x)
       case Deep(pr, m, sf) => Digit.reduceL (opl) (reduceL (Node.reduceL(opl)) (Digit.reduceL(opl)(z, pr), m), sf)
     }

    // page 5 bottom (the left triangle); Actually we could use the left
    // triangle in Scala but I am somewhat old fashioned ...

     def addL[A] (a: A, t: FingerTree[A]): FingerTree[A] = t match {
       case Empty() => Single(a)
       case Single(x) => Deep(Digit(a), Empty(), Digit(x))
       case Deep(Digit(x,b,c,d), m, sf) => Deep(Digit(a, x), addL(Node3(b,c,d), m), sf)
       case Deep(pr, m, sf) => Deep(a :: pr, m, sf)
     }

     def addR[A] (t: FingerTree[A], a: A): FingerTree[A] = t match {
       case Empty() => Single(a)
       case Single(x) => Deep(Digit(a), Empty(), Digit(x)) //Deep(Digit(x), Empty(), Digit(a))
       case Deep(pf, m, Digit(x, b, c, d)) => Deep(pf, addR(m, Node3(b,c,d)), Digit(x, a))
       // Deep(Digit(a, d), addR(m, Node3(c, b, x)), sf)
       case Deep(pr, m, sf) => Deep(pr, m, a :: sf)
     }

    // page 6
    //
    // This is a direct translation of view to Scala. You can replace it later
    // with extractors in Scala, see above objects NilTree and ConsL (this is an
    // alternative formulation which is more idiomatic Scala, and slightly
    // better integrated into the language than the Haskell version).
    // In Haskell we need to call viewL(t) to pattern match on views.  In Scala,
    // with extractors in place, we can directly pattern match on t.
    //
    // viewL defines a veiw of a fingertree as a list
    // this allows you to call a function lazily to convert tree into traditional const list
    def viewL[A] (t: FingerTree[A]) :ViewL[A] = t match {
      case Empty() => NilTree()
      case Single(x) => ConsL(x, Empty())
      case Deep(pr, m, sf) => ConsL(pr.head, deepL(pr.tail, m, sf)) // HARD
    }

    // page 6
    //
    // A smart constructor that allows pr to be empty
    // Sometimes there will only be one element in left digit, which Deep constructor of finger tree does not accept
    def deepL[A] (pr: Digit[A], m: FingerTree[Node[A]], sf: Digit[A]) :FingerTree[A] = pr match {
      case Nil => viewL(m) match {
        case NilTree() => Digit.toTree (sf)
        case ConsL(hd, m2) => Deep(Node.toList(hd), m2, sf)
      }
      case _ => Deep(pr, m, sf)
    }

    // OMITTED mirror image of left view using deepR function
//    def deepR[A] (pr: Digit[A], m: FingerTree[A], sf: Digit[A]): FingerTree[A] =

    // page 7
    // TODO: how do I cover empty case?
   def headL[A] (t: FingerTree[A]): A = viewL(t) match {
     case ConsL(h, _) => h
     case NilTree() => throw new NoSuchElementException("headLeft on empty finger tree")
   }

    // TODO: how do I cover empty case?
   def tailL[A] (t: FingerTree[A]): FingerTree[A] = viewL(t) match {
     case ConsL(_, tl) => tl
     case NilTree() => throw new NoSuchElementException("tailLeft on empty finger tree")
   }

//   def headR[A] (t: FingerTree[A]): A = viewR(t) match {
//     case ConsR(h, _) => h
//   }

//   def tailR[A] (t: FingerTree[A]): FingerTree[A] = viewR(t) match {
//     case ConsR(_, tl) => tl
//   }

  }

}


