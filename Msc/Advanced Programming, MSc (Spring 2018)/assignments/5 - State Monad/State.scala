// Advanced Programming, A. Wąsowski, IT University of Copenhagen
//
// AUTHOR1: Thor Olesen (tvao@itu.dk)
// AUTHOR2: Dennis Nguyen (dttn@itu.dk)
// AUTHOR3: Daniel Hansen (daro@itu.dk)
// Group number: 7
//


trait RNG {
  def nextInt: (Int, RNG)
}

object RNG {
  // NB - this was called SimpleRNG in the book text

  case class Simple(seed: Long) extends RNG {
    def nextInt: (Int, RNG) = {
      val newSeed = (seed * 0x5DEECE66DL + 0xBL) & 0xFFFFFFFFFFFFL // `&` is bitwise AND. We use the current seed to generate a new seed.
      val nextRNG = Simple(newSeed) // The next state, which is an `RNG` instance created from the new seed.
      val n = (newSeed >>> 16).toInt // `>>>` is right binary shift with zero fill. The value `n` is our new pseudo-random integer.
      (n, nextRNG) // The return value is a tuple containing both a pseudo-random integer and the next `RNG` state.
    }
  }

  // Exercise 1 (CB 6.1)

  def nonNegativeInt(rng: RNG): (Int, RNG) = {
      (rng.nextInt) match  {
          case (i, next) if (i < 0) => (Int.MaxValue + 1 + i, next) //We add +1 to reset to zero
          case (i, next) => (i, next)
        }
  }
  //VERSION 2
  def nonNegativeInt2(rng: RNG): (Int, RNG) = {
    val (n, nextRNG) = rng.nextInt
    if (n == Int.MinValue) (0, nextRNG)
    else (math.abs(n), nextRNG)
  }

  // Exercise 2 (CB 6.2)

  def double(rng: RNG): (Double, RNG) = {
    (nonNegativeInt(rng)) match  {
      case (i,rng) => (i.toDouble / Int.MaxValue.toDouble,rng)
    }
  }
  //there are more double values compared to int values, so one will not be able to obtain all possible values

  // Exercise 3 (CB 6.3)

  def intDouble(rng: RNG): ((Int, Double), RNG) = {
    val (d,rng2) = double(rng)
    val (i,rng3) = rng2.nextInt
    ((i,d),rng3)
  }
  //version 2
  def intDouble2(rng: RNG): ((Int, Double), RNG) = {
    val (i, rng1) = nonNegativeInt(rng)
    val (d, rng2) = double(rng1)
    ((i, d), rng2)
  }

  def doubleInt(rng: RNG): ((Double, Int), RNG) = {
    val (d,rng2) = double(rng)
    val (i,rng3) = rng2.nextInt
    ((d,i),rng3)
  }
  //version 2
  def doubleInt2(rng: RNG): ((Double, Int), RNG) = {
    // Composition
    val ((i, d), rng1) = intDouble(rng)
    ((d, i), rng1)
  }

  //def boolean(rng: RNG): (Boolean, RNG) = rng.nextInt match { case (i,rng2) => (i%2==0,rng2) }

  // Exercise 4 (CB 6.4)

  def ints(count: Int)(rng: RNG): (List[Int], RNG) = {
    val (i,ran) = rng.nextInt
    if(count>0){
      val (l,ran2) = ints (count-1) (ran)
      (i::l,ran2)
    }else
      (List[Int](),ran)
  }


  // Version 2
  def ints2(count: Int)(rng: RNG): (List[Int], RNG) ={

    def intsAcc(n:Int, state:RNG, acc:List[Int]): (List[Int],RNG)={
      n match {
        case n if n <= 0 => (acc,state)
        case _ => {
          val (i,next) = state.nextInt
          intsAcc(n-1,next, i :: acc)
        }
      }
    }
    intsAcc(count,rng,List[Int]())
  }

  //version 3
  def ints3(count: Int)(rng: RNG): (List[Int], RNG) = count match {
    case 0 => (List(), rng)
    case _ => {

      // Succinct
      val (n, nextRNG) = rng.nextInt
      (n :: (ints3 (count - 1) (nextRNG))._1, nextRNG)

    }
  }


  // There is something terribly repetitive about passing the RNG along
  // every time. What could we do to eliminate some of this duplication
  // of effort?

  type Rand[+A] = RNG => (A, RNG)

  val int: Rand[Int] = _.nextInt

  def unit[A](a: A): Rand[A] =
    rng => (a, rng)

  def map[A,B](s: Rand[A])(f: A => B): Rand[B] =
    rng => {
      val (a, rng2) = s(rng)
      (f(a), rng2)
    }

  def nonNegativeEven: Rand[Int] = map(nonNegativeInt)(i => i - i % 2)

  // Exercise 5 (CB 6.5)

  val _double: Rand[Double] = { map(nonNegativeInt)(i => i.toDouble / Int.MaxValue.toDouble) }

  //version 2

  val _double2: Rand[Double] = map(double)(d => d)

  // Exercise 6 (CB 6.6)

  def map2[A,B,C](ra: Rand[A], rb: Rand[B])(f: (A, B) => C): Rand[C] =
    rng => {
      val (a, rng2) = ra(rng)
      val (b, rng3) = rb(rng2)
      (f(a,b), rng3)
    }

  // this is given in the book

  //def both[A,B](ra: Rand[A], rb: Rand[B]): Rand[(A,B)] = map2(ra, rb)((_, _))

  //val randIntDouble: Rand[(Int, Double)] = both(int, double)

  //val randDoubleInt: Rand[(Double, Int)] = both(double, int)

  // Exercise 7 (6.7)
  def sequence[A](fs: List[Rand[A]]): Rand[List[A]] = {
    fs.foldRight (unit(List[A]())) ((e,acc) => {
      map2(e,acc)((a,l) => a :: l)
    })
  }

  //def _ints(count: Int): Rand[List[Int]] = rng => {sequence (ints (count) (rng))}

  // Exercise 8 (6.8)

  def flatMap[A,B](f: Rand[A])(g: A => Rand[B]): Rand[B] =
    rng => {
      val (a,rng2) = f (rng)
      g (a) (rng2)
    }

  //def nonNegativeLessThan(n: Int): Rand[Int] = { ...

}

import State._

case class State[S, +A](run: S => (A, S)) {

  // Exercise 9 (6.10)

  def map[B](f: A => B): State[S, B] = {
    State(st => { // Equivalent to new State but case class
      val (a, st2) = run(st) // Retrieve old state (i.e. this.run)
      (f(a), st2) // New State with new run method
    })
  }

  def map2[B,C](sb: State[S, B])(f: (A, B) => C): State[S, C] = State(s => {
    val (a, s2) = run(s)
    val (b, s3) = sb.run(s2)
      (f(a,b), s3)
    })

  def flatMap[B](f: A => State[S, B]): State[S, B] = {
      State(s => {
        val (a, st1) = run(s)
        f(a).run(st1)
      })
    }

}

object State {
  type Rand[A] = State[RNG, A]

  def unit[S, A](a: A): State[S, A] =
    State(s => (a, s))


  // Exercise 9 (6.10) continued

  def sequence[S,A](sas: List[State[S, A]]): State[S, List[A]] = {
    sas.foldRight (unit[S, List[A]](List[A]())) ((elem, acc) => elem.map2(acc) ((h,t) => h :: t))
  }


  // def modify[S](f: S => S): State[S, Unit] = for {
  //   s <- get // Gets the current state and assigns it to `s`.
  //   _ <- set(f(s)) // Sets the new state to `f` applied to `s`.
  // } yield ()

  def get[S]: State[S, S] = State(s => (s, s))

  def set[S](s: S): State[S, Unit] = State(_ => ((), s))


  def random_int :Rand[Int] =  State (_.nextInt)

  // Exercise 10

  def state2stream[S,A] (s :State[S,A]) (seed :S) :Stream[A] = {
    val (a, st1) = s.run(seed)
    Stream.cons(a, state2stream(s)(st1)) // Continue recursively using new state st1
  }

  // Exercise 11

  val random_integers = state2stream (random_int) (RNG.Simple(0))
  val rand_10 = random_integers.take(10).toList
}


// vim:cc=80:foldmethod=indent:foldenable
