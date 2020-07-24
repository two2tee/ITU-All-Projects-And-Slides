// Advanced Programming, A. Wąsowski, IT University of Copenhagen
//
// AUTHOR1: Thor Olesen (tvao@itu.dk)
// AUTHOR2: Dennis Nguyen (dttn@itu.dk)
// AUTHOR3: Daniel Hansen (daro@itu.dk)
// Group number: 7

import java.util.concurrent._
import scala.language.implicitConversions

// Work through the file top-down, following the exercises from the week's
// sheet.  Uncomment and complete code fragments.

// General idea so far:
// Monad: abstraction -> type, id and flatMap (bind)
// Example: List has a type Int, id 0 and flatMap taking List[List[Int] turning it into List[Int]
// API built with algebra: data structure, functions on data structures and laws/properties to express relationships between functions (and generate prop tests)

object Par {

  type Par[A] = ExecutorService => Future[A]
  def run[A] (s: ExecutorService) (a: Par[A]) : Future[A] = a(s)


  case class UnitFuture[A] (get: A) extends Future[A] {
    def isDone = true
    def get (timeout: Long, units: TimeUnit) = get
    def isCancelled = false
    def cancel (evenIfRunning: Boolean) : Boolean = false
  }

  def unit[A] (a: A) :Par[A] = (es: ExecutorService) => UnitFuture(a)

  def map2[A,B,C] (a: Par[A], b: Par[B]) (f: (A,B) => C) : Par[C] =
    (es: ExecutorService) => {
      val af = a (es)
      val bf = b (es)
      UnitFuture (f(af.get, bf.get))
    }

  def fork[A] (a: => Par[A]) : Par[A] = es => es.submit(
    new Callable[A] { def call = a(es).get }
  )

  def lazyUnit[A] (a: =>A) : Par[A] = fork(unit(a))

  // Exercise 1 (CB7.4) : converts any function A => B to one that evaluates its result asynchronously (separate thread)
  def asyncF[A,B] (f: A => B) : A => Par[B] = (a: A) => lazyUnit(f(a)) // Returns Par[A => B] or Par[B]

  // map is shown in the book

  def map[A,B] (pa: Par[A]) (f: A => B) : Par[B] =
    map2 (pa,unit (())) ((a,_) => f(a))

  // Exercise 2 (CB7.5)
  // Takes a list of parallel computations and returns a parallel computation producing a list
  def sequence[A] (ps: List[Par[A]]): Par[List[A]] = {
    // Fold starting with empty Par[List[A]] result using unit
    // Map over each Par[A] and accumulator (List[A]), which is B in Par[B]
    ps.foldRight (unit(List[A]())) ((elem, acc) => map2 (elem, acc) ((a,b) => a :: b))
  }

  // Exercise 3 (CB7.6)

  // this is shown in the book:
  // Map list in parallel
  def parMap[A,B](ps: List[A])(f: A => B): Par[List[B]] = fork {
     val fbs: List[Par[B]] = ps.map(asyncF(f))
     sequence(fbs)
  }

  // Filter elements of a list in parallel
  // Uses lazyUnit to make each element lazy and wrap it with fork in a Par[A] to be executed async on N parallel computations
  def parFilter[A] (as: List[A]) (f: A => Boolean): Par[List[A]] = {
    //lazyUnit (as.filter(f)) // Make evaluation of predicate f on list as lazy and execute in parallel
    // Run predicate in parallel and at end compress
    val filterPredicates: Par[List[List[A]]] = parMap (as) (elem => if (f(elem)) List(elem) else Nil) // Parallel map
    map (filterPredicates) (elem => elem.flatten) // TODO: does map actually handle elements in parallel?

    //val bs :Par[List[Boolean]] = parMap (as) (f)
    //map[List[Boolean], Boolean] (bs) (bs => bs.foldRight (true) (_&&_))
  }


  //Alternate solution where we use tuples to wrap the elements
  def parFilterWithTuple[A](as: List[A])(f: A => Boolean): Par[List[A]] = {
    val l = parMap(as)(a=>(f(a),a))
    map(l)(a => a.foldRight (List[A]()) ((e,l) => {if(e._1) e._2::l else l}))
  }


  //Alternate solution where we wrap all elements in a list and flatten it in the end
  def parFilterListWrap[A](as: List[A])(f: A => Boolean): Par[List[A]] = {
    val parList = parMap(as)(a=> //Converts list into a list of lists containing elements or not which we would flatten
      if(f(a)) List(a)
      else     List()
    )
    map(parList)(ll => ll.flatten)

  }


  
  // Check whether elements in list satisfy predicate in parallel
  def parForAll[A] (as: List[A]) (p: A => Boolean): Par[Boolean] = {
    //lazyUnit (as.forall (p)) // TODO: 1 point as not entirely parallel
    val bs :Par[List[Boolean]] = parMap (as) (p)
    map[List[Boolean], Boolean] (bs) (bs => bs.foldRight (true) (_&&_))

  }

  // Exercise 4: implement map3 using map2

  def map3[A,B,C,D] (pa :Par[A], pb: Par[B], pc: Par[C]) (f: (A,B,C) => D) :Par[D]  = {
    map2 (pa, map2(pb, pc) ((b,c) => (b,c)) ) ({ case (a, (b,c)) => f(a,b,c) }) // Map(A, Map(B,C)) to get all 3 values and produce D
    // map2 (pa, map2(pb, pc) ((b,c) => (b,c)) ) ((a, (b,c) => f(a,b,c) }) // TODO: why can I not do this without explicitly pattern matching result?
    // Tuples => match components
    map2 (pa, map2(pb,pc) ((b, c) => (b,c))) ((a, tuple) => f(a, tuple._1, tuple._2)) // Compiler cannot infer (A, tuple)
  }

  //Alternate solution where we use a function that wraps two of the parameters in a tuble
  def map3WithAuxFunction[A,B,C,D] (pa :Par[A], pb: Par[B], pc: Par[C]) (f: (A,B,C) => D) :Par[D]  = {

    val aux = (t:(A, B),c:C) => f(t._1,t._2,c)
    val AB = map2(pa,pb)((a,b) =>(a,b))

    map2(AB,pc)((ab,c)=> aux(ab,c))
  }

  //Alternate solution where we create a partial computed function with map2 
  def map3PartialComputed[A,B,C,D] (pa :Par[A], pb: Par[B], pc: Par[C]) (f: (A,B,C) => D) :Par[D]  = {
    val parf = map2 (pa,pb) ((a,b)=> { c:C => f(a,b,c) }); // Par[C => D]
    map2 (pc,parf) ((c,f) => {f(c)}) // Par[D]
  }


  // shown in the book

  // def equal[A](e: ExecutorService)(p: Par[A], p2: Par[A]): Boolean = p(e).get == p2(e).get

  // Exercise 5 (CB7.11)
  // Choose between N forking computations from choices based on result of initial computation n
  def choiceN[A] (n: Par[Int]) (choices: List[Par[A]]) :Par[A] = {
          // TODO: what is ment by picking n computations?
    es => choices (run (es) (n).get()) (es) // Run n and use result to pick computation n from N ala list (n) yields element n from list of N
  }

  // choose between two forking computations based on the result of an initial computation
  def choice[A] (cond: Par[Boolean]) (t: Par[A], f: Par[A]) : Par[A] = {
    //es => if (run(es) (cond).get) t(es) else f(es)
    val choices = List(t,f)
    choiceN { // unit(0) is the index in choices that needs to be wrapped in a Par so Par[Int]
      es => if(run(es)(cond).get) unit(0)(es) else unit(1)(es) // Computation n picks element at index 0 if condition is true, otherwise 1
    } (choices) // Choices to pick from
  }

  // Exercise 6 (CB7.13) // TODO REDO

  // Use chooser to implement choices
  // Now replace Par[Int] and Par[Boolean] with generic Par[A]
  // Pick parallel computation A from choices based on computation of pa : Par[A]
   def chooser[A,B] (pa: Par[A]) (choices: A => Par[B]): Par[B] = {
     service => {
       val value = run(service)(pa).get //Run pa to get value
       run(service)(choices(value))     //find and run a computation with value
     }
   }

  def choiceNviaChooser[A] (n: Par[Int]) (choices: List[Par[A]]) :Par[A] = {
    es => chooser (n) (i => choices(i)) (es)
    // choices (run(es) (n).get()) (es)
  }

  def choiceViaChooser[A] (cond: Par[Boolean]) (t: Par[A], f: Par[A]) : Par[A] = {
    es => chooser (cond) (b => if(b) t else f) (es)
  }

  // Exercise 7 (CB7.14) // TODO: redo
  // TODO: how to implement flatMap using join, vice versa?
  // TODO: compare join with type of List.flatten (and relation of join to chooser against relation of List.flatten to List.flatMap)
  // FlatMap : Par[Par[A]] becomes Par[A] (flattened)
  // Join combines the results of all nested Par[A]'s into one Par[A]
  def join[A] (a : Par[Par[A]]) :Par[A] ={
    service => {
      val joined = a(service).get //Joins the two pars by running it
      joined(service) //return the joined with the executor service
    }
  }

  def flatMap[A,B] (par : Par[A]) (f: A => Par[B]): Par[B] = {
    val unflattenedComputations: Par[Par[B]] = map (par) (f)
    join (unflattenedComputations)
  }

  class ParOps[A](p: Par[A]) {

  }

  implicit def toParOps[A](p: Par[A]): ParOps[A] = new ParOps(p)
}
