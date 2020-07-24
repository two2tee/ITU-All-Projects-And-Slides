de
(*
Exercise 1. Run the slow Fibonacci computations from the lecture's
examples on your own computer.  Use the #time directive to turn on
timing, and see what is the best speed-up you can obtain for
computing, say, the first 43 Fibonacci numbers using Async.Parallel.
This may be quite different on MS .NET than on Mono.
*)

// Computing slowfib(42) is CPU-intensive, ca 2 sec. Mono Mac 

let rec slowfib n = if n<2 then 1.0 else slowfib(n-1) + slowfib(n-2);;

// Times are Mono 4.2.1 on MacOS 10.11.3 Intel Quad Core i7 2,3 GHz

#time 

let fibSingleSync = slowfib(42);;
// Real: 00:00:02.254, CPU: 00:00:02.254, GC gen0: 0, gen1: 0

let fibTwoSync = [ slowfib(41); slowfib(42) ];;
// Real: 00:00:03.594, CPU: 00:00:03.596, GC gen0: 0, gen1: 0

let fibTwoAsync =
    let tasks = [ async { return slowfib(41) };
                  async { return slowfib(42) } ]
    Async.RunSynchronously (Async.Parallel tasks);;
// Real: 00:00:02.240, CPU: 00:00:03.636, GC gen0: 0, gen1: 0

let fibSyncYield = [ for i in 0..42 do yield slowfib(i) ];;
// Real: 00:00:06.181, CPU: 00:00:06.179, GC gen0: 0, gen1: 0

let fibAsyncParallelYield =
    let tasks = [ for i in 0..42 do yield async { return slowfib(i) } ]
    Async.RunSynchronously (Async.Parallel tasks);;
// Real: 00:00:02.628, CPU: 00:00:07.073, GC gen0: 0, gen1: 0 , 4,2 on dual core i7 

#time

// Dissection of this:

// async { return slowfib(i) }
// has type: Async<float>
// an asynchronous tasks that, when run, will produce a float

// let tasks = [ for i in 0..42 do yield async { return slowfib(i) } ]
// has type: Async<float> list
// a list of asynchronous tasks, each of which, when run, will produce a float

// Async.Parallel tasks
// has type: Async<float []>
// an asynchronous tasks that, when run, will produce a list of floats

// Async.RunSynchronously (Async.Parallel tasks)
// has type: float []
// a list of floating-point numbers


(*
Exercise 2. Similarly, run the prime factorization example on your own
computer, and see what speedup you get.
*) 

let isPrime n =
    let rec testDiv a = a*a > n || n % a <> 0 && testDiv (a+1)
    n>=2 && testDiv 2;;

let factors n =
    let rec factorsIn d m =
        if m <= 1 then []
        else if m % d = 0 then d :: factorsIn d (m/d) else factorsIn (d+1) m
    factorsIn 2 n;;

let random n =
    let generator = new System.Random ()
    fun () -> generator.Next n;;

let r10000 = random 10000;; // 150x faster than creating a new System.Random

let rec ntimes (f : unit -> 'a) n =
    if n=0 then () else (ignore (f ()); ntimes f (n-1));;
    
let bigArray = Array.init 500000 (fun _ -> r10000 ());;

#time;;

// Array.map isPrime bigArray;;
// Not much of a win on two-core Mono MacOS.  Presumably because most
// integers have small prime factors ... eg. 2/3 of them have either 2
// or 3 as prime factor.
// Array.Parallel.map isPrime bigArray;;
// Strangely, today (2013-04-24) the parallelized version is nearly twice
// as fast...

// Better example: Prime factors of random numbers (more work)

Array.map factors bigArray;; // Real: 00:00:01.910, CPU: 00:00:01.923, GC gen0: 6, gen1: 0
Array.Parallel.map factors bigArray;; // Real: 00:00:00.513, CPU: 00:00:03.126, GC gen0: 6, gen1: 0

// Even better example: Prime factors of [1..200000]

Array.init 200000 factors;; // Real: 00:00:10.610, CPU: 00:00:10.634, GC gen0: 1, gen1: 1 
let factors200000 = Array.Parallel.init 200000 factors;; // Real: 00:00:02.283, CPU: 00:00:17.907, GC gen0: 2, gen1: 0

// > Array.init 200000 factors;;
// Real: 00:00:03.043, CPU: 00:00:03.042, GC gen0: 1, gen1: 0

// > Array.Parallel.init 200000 factors;;
// Real: 00:00:01.590, CPU: 00:00:03.056, GC gen0: 1, gen1: 0

#time 

(*
Exercise 3. The lecture's construction of a histogram (counting the
numbers of times that each prime factor 2, 3, 5, 7, 11 ... appears)
uses a side effect in the assignment

     histogram.[i] <- histogram.[i] + 1

But side effects should be avoided.  Program the histogram
construction so that it does not use side effects but purely
functional programming.  There are several useful functions in the Seq
module.  The final result does not have to be an int[] array, but
could be a seq<int * int> of pairs (p, n) where p is a prime factor
and n is the number of times p appears in the array of lists of prime
factors.
*)

let histogram = Array.init 200000 (fun i -> 0)
let incr i = histogram.[i] <- histogram.[i] + 1 // Side effect 
Array.iter (fun fs -> List.iter incr fs) factors200000;;
// Real: 00:00:00.088, CPU: 00:00:00.093, GC gen0: 1, gen1: 0 
 
let histogramAlt =
    let fold1 map number =  
        let fold2 (acc:Map<int, int>) e =   
            let present = acc.TryFind(e)
            match present with
            | Some x -> acc.Add(e, x+1)
            | None -> acc.Add(e,1)                                  
        List.fold fold2 map (factors (number-1))
    List.fold fold1 Map.empty [1 .. 200000]

// New way using sq and countBy ??? 

let seqHistogram = Seq.init 20000 (fun i -> 0)  
Seq.countBy(fun elem -> if (isPrime elem) then 1 else 0) seqHistogram ;; 

//let rec histogramCounter list = 
//    match list with
//    | [] -> 0 
//    | _::tail -> 1 ;;
//
//histogramCounter factors200000 ;;

(*
Exercise 4. Find the fastest way on your hardware to count the number
of prime numbers between 1 and 10 million (the correct count is
664579).  

Use this F# function to determine whether a given number n
is prime:

let isPrime n =
    let rec testDiv a = a*a > n || n % a <> 0 && testDiv (a+1)
    n>=2 && testDiv 2;;

or if you believe it would be faster in C, C# or Java, you may use
this version:

private static boolean isPrime(int n) {
  int k = 2;
  while (k * k <= n && n % k != 0)
    k++;
  return n >= 2 && k * k > n;
}

Remember to use parallel programming to the extent possible.  On my
Intel i7 Mac the result can be computed in 2 seconds wall-clock time
using 4 cores, and 99 characters of F# code (in addition to the above
function).

*)

#time 

// Old way
// Real: 00:00:42.216, CPU: 00:05:28.882, GC gen0: 13, gen1: 1 
//let factors1000000 = Array.Parallel.init 1000000 factors;;

// Test if single number is prime 
let isPrime4 n =
    let rec testDiv a = a*a > n || n % a <> 0 && testDiv (a+1)
    n>=2 && testDiv 2;;

// Create array of numbers between 1 and 10000000
let numbers = [|1 .. 10000000|]

// Filter list by function
// Real: 00:00:01.600, CPU: 00:00:10.637, GC gen0: 0, gen1: 1
// Pick first list of two with elements for which the predicate isPrime return true and false 
let factors10000000 = (fst (Array.Parallel.partition isPrime4 numbers)).Length ;; // returns 664579 
