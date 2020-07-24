
(* 
Exercise 1 in Functional Programming (BFNP)  
Date: February 11th
Authors: Dennis (dttn) and Thor (tvao)
*) 

(* 
Exercise 1.1 Write a function sqr:int->int so that sqr x returns x^2
*)

let sqr x = x*x;;

(* 
Write a function pow : float -> float -> float so that pow x n returns x^n.
You can use the library function: System.Math.Pow.
*)

// Using library from System.Math.Pow;;
let pow x n = System.Math.Pow(x,n);;

(* 
Exercise 1.3 Solve HR, exercise 1.1

1.1 Declare a function g: int −> int, where g(n) = n + 4.

g: int −> int, where g(n) = n + 4.

*)

let g n = n+4;;


(* 
Exercise 1.4 Solve HR, exercise 1.2

Declare a function h: float * float -> float, where  

h: float * float -> float where h(x,y) = sqrt(x^2+y^2) 
*)

let h (x,y) = System.Math.Sqrt(x*x + y*y);;

(* 
Exercise 1.5 Solve HR, exercise 1.4

Declare a recursive function f: int -> int, where
f(n) = 1+2+...+(n-1)+n
*)

let rec fact = function
| 0 -> 1 
| n -> n + f(n-1);;

    (*
    Recursion formula given by:
    0 = 1 (clause 1)
    n = n + (n-1) for n > 0 (clause 2)
    *)

// Give evaluation of f(4)
let evaluation = f(4);;

(* 
Exercise 1.6 Solve HR, exercise 1.5

The sequence F0, F1, F2, … of Fibonacci numbers is defined by:
F0 = 0
F1 = 1
Fn = Fn-1+Fn-2
Thus, the first members of the sequence are 0,1,1,2,3,5,8,13
Declare an F# function to compute Fn. 
Use a declaration with three clauses, where the patterns correspond to the three cases of the above definition.
Give an evaluations for F4.
*)

let rec fibo = function
| 0 -> 0
| 1 -> 1 
| n -> fact(n-1) + f(n-2) ;;

let evaluation2 = fact(4);;

(* 
Exercise 1.7 Solve HR, exercise 1.6

Declare a recursive function sum: int * int -> int, where
sum(m, n) = m + (m + 1) + (m + 2) + · · · + (m + (n − 1)) + (m + n)
for m ≥ 0 and n ≥ 0. (Hint: use two clauses with (m,0) and (m,n) as patterns.)
Give the recursion formula corresponding to the declaration.
*)

let rec sum = function
| (m,0) -> 0 
| (m,n) -> (m + sum(m,n-1) + (m + n)) ;;

    (*
    Recursion formula given by:
    (m,0) = 0 for m=> 0, n = 0 (clause 1)
    (m,n) = m + sum(m,n-1) + (m+n) for m => 0, n > 0 (clause 2) 
    *)

(* 
Exercise 1.8 Solve HR, exercise 1.7

Determine a type for each of the expressions:
      (System.Math.PI, fact -1)
      fact(fact 4)
      power(System.Math.PI, fact 2)
      (power, fact)
*)

// (System.Math.PI, fact -1) is of type (float*int) 
// fact(fact 4) is of type int 
// power(System.Math.PI, fact 2) is of type float 
// (power, fact) is of type (int*int)


(* 
Exercise 1.9 Solve HR, exercise 1.8

Consider the declarations:
      let a = 5;;
      let f a = a + 1;;
      let g b = (f b) + a;;
Find the environment obtained from these declarations 
and write the evaluations of the expressions f 3 and g 3.
*)

    (*       __                   __
           | a    |--> 5         |
     env = | f(a) |--> a + 1     |
           | g(b) |--> f(b) + a  |
           __                   __
     *)

// Declarations 
let a = 5;;
let f a = a + 1;; 
let g b = f(b) + a;;

// Evaluations 
let fEvaluation = f(3);;
let gEvaluation = g(3);;

(*
Exercise 1.10 Write a function dup:string->string that concatenates a string with itself
*)

let dup s : string = s + s;;

(* 
Exercise 1.11 Write a function dupn:string->int->string so that dupn s n creates the concatenation
of n copies of s.
*)

let rec dupn s n = 
match (s,n) with 
| (s,0) -> ""
| (s,n) -> s + dupn s (n-1);; 

(* 
Exercise 1.12 Assume the time of day is represented as a pair (hh, mm):int*int.
Write a function timediff:int*int->int*int->int so that timediff t1 t2 computes the difference
in minutes between t1 and t2, i.e., t2-t1
*)

let timeOfDayInMin = function
| (0,mm) -> mm
| (hh,mm) -> (hh*60) + mm ;;

let timeDiff (hh1,mm1) (hh2,mm2) = timeOfDayInMin (hh2,mm2) - timeOfDayInMin (hh1,mm1) ;; 

(*
Exercise 1.13 Write a function minutes:int*int->int to compute the number of minutes since midnight.
Easily done using the function timediff.
*)

let minutes (hh,mm) = timeDiff(00,00)(hh,mm);;

(* 
Exercise 1.14 Solve HR, exercise 2.2

Declare an F# function pow: string * int -> string, where:

pow(s,n) = s·s···· ·s where we use · to denote string concatenation.
*)

let rec pow (s,n) = 
 match (s,n) with 
 | (s,0) -> "" 
 | (s,n) -> s+ pow(s, n-1) ;; 

(* 
Exercise 1.15 Solve HR, exercise 2.8

Declare an F# function bin: int * int -> int to compute binomial coefficients.
*)

let rec bin (n,k) = 
    match (n,k) with
    | (n,0) -> 1 
    | (n,k) when n=k -> 1
    | (n,k) when n>k -> bin(n-1,k-1) + bin(n-1,k)
    | _ -> 0;; // To cover all other cases when k>n 

(* 
Exercise 1.16 Solve HR, exercise 2.9

Consider the declaration

let rec f = function 
| (0,y) -> y 
| (x,y) -> f(x-1, x*y);;

1. Determine the type of f.
2. For which arguments does the evaluation of f terminate? 
3. Write the evaluation steps for f(2,3).
4. What is the mathematical meaning of f(x,y)?
*)

// 1) (int*(int*int))
// 2) For x >= 0 and any value of y, the function will terminate correctly. If x is negative, overflow will occur.
// 3)
    (*
     f(2,3)
     f(2,3) -> f(2-1, 2*3)
     f(1,6) -> f(1-1, 1*6)
     f(0,6) -> 6   
    *)
// 4) It is a mathematical function that corresponds to: x! * y. 

let rec f = function 
| (0,y) -> y 
| (x,y) -> f(x-1, x*y);;

(* 
Exercise 1.17 Solve HR, exercise 2.10

Consider the following declaration:
  let test(c,e) = if c then e else 0;;

1. What is the type of test?
2. What is the result of evaluating test(false,fact(-1))? 
3. Compare this with the result of evaluating
    if false then fact -1 else 0
*)

let test(c,e) = if c then e else 0;;

// 1) Type is bool * int 
// 2) 0 because the evaluation of c in the expression is false. 
// 3) This is the opposite and will thus return e because c is false and the user input it false, thus true. 

(* 
Exercise 1.18 Solve HR, exercise 2.13

The functions curry and uncurry of types
      curry   : (’a * ’b -> ’c) -> ’a -> ’b -> ’c (3 arguments)
      uncurry : (’a -> ’b -> ’c) -> ’a * ’b -> ’c (3 arguments)
are defined in the following way:

curry f is the function g where g x is the function h where h y = f(x,y). 
uncurry g is the function f where f(x,y) is the value h y for the function h = g x. 

Write declarations of curry and uncurry.
*)

// Uncurry: f(x,y) is uncurried because it takes arguments as tuple, a*b -> c 
// Curry: f x y id curried because it takes arguments independdently a -> b -> c 

let curry f x y = f(x,y);; 
let uncurry f (x,y) = f x y;; 

let curry f = let g x = let h y = f(x,y) in h in g;; 

let f x = let a = 2 in x+a;; // Temporary calculation 