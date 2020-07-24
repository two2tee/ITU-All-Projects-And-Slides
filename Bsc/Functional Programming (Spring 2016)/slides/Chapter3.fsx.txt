// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 3: Tuples, records and tagged values. 

let rec unzip = function
    []          -> ([],[])
  | (x,y)::rest -> let (xs,ys) = unzip rest
                   (x::xs,y::ys);;

let _ = unzip [(1,"a");(2,"b")];;

// From Section 3.2 Polymorphism
let swap (x,y) = (y,x);;


// From Section 3.3 Example: Geometric vectors
let (~-.) (x:float,y:float) = (-x,-y);;

let (+.) (x1, y1) (x2,y2) = (x1+x2,y1+y2): float*float;;

let (-.) v1 v2 = v1 +. -. v2;;

let ( *.) x (x1,y1) = (x*x1, x*y1): float*float;;

let (&.) (x1,y1) (x2,y2) = x1*x2 + y1*y2: float;;

let norm(x1:float,y1:float) = sqrt(x1*x1+y1*y1);;

let a = (1.0,-2.0);;
let b = (3.0,4.0);;
let c = 2.0 *. a -. b;;
let d = c &. a;;
let e = norm b;;


// From Section 3.4 Records
type Person = {age : int; birthday : int * int;
               name : string; sex : string};;

let john = {name =  "John"; age = 29;
            sex = "M"; birthday = (2,11)};;

let age {age = a; name = _; sex=_; birthday=_} = a;;

let isYoungLady {age=a; sex=s; name=_; birthday=_}
                                        = a < 25 && s = "F";;



// From Section 3.5 Example: Quadratic equations
// and  Section 3.6 Locally declared identifiers
type Equation = float * float * float;;
type Solution = float * float;;
exception Solve;;

let solve(a,b,c) =
  if b*b-4.0*a*c < 0.0 || a = 0.0 then raise Solve
  else ((-b + sqrt(b*b-4.0*a*c))/(2.0*a),
        (-b - sqrt(b*b-4.0*a*c))/(2.0*a));;

let solve1(a,b,c) =
  if b*b-4.0*a*c < 0.0 || a = 0.0
  then failwith "discriminant is negative or a=0.0"
  else ((-b + sqrt(b*b-4.0*a*c))/(2.0*a),
        (-b - sqrt(b*b-4.0*a*c))/(2.0*a));;

let solve2(a,b,c) =
  let d = b*b-4.0*a*c
  if d < 0.0 || a = 0.0
  then failwith "discriminant is negative or a=0.0"
  else ((-b + sqrt d)/(2.0*a),(-b - sqrt d)/(2.0*a));;

let solve3(a,b,c) =
  let sqrtD =
    let d = b*b-4.0*a*c
    if d < 0.0 || a = 0.0
      then failwith "discriminant is negative or a=0.0"
      else sqrt d
  ((-b + sqrtD)/(2.0*a),(-b - sqrtD)/(2.0*a));;

let solve4(a,b,c) =
  let d = b*b-4.0*a*c
  if d < 0.0 || a = 0.0
   then failwith "discriminant is negative or a=0.0"
   else let sqrtD = sqrt d
        ((-b + sqrtD)/(2.0*a),(-b - sqrtD)/(2.0*a));;

// From Section 3.7 Example: Rational numbers. Invariants
type Qnum = int*int;;    // (a,b) where b > 0 and gcd(a,b) = 1

let rec gcd = function
    | (0,n) -> n
    | (m,n) -> gcd(n % m,m);;

let canc(p,q) =
   let sign = if p*q < 0 then -1 else 1
   let ap = abs p
   let aq = abs q
   let d  = gcd(ap,aq)
   (sign * (ap / d), aq / d);;

let mkQ = function
   | (_,0)  -> failwith "Division by zero"
   | pr     -> canc pr;;

let (.+.) (a,b) (c,d) = canc(a*d + b*c, b*d);;     // Addition

let (.-.) (a,b) (c,d) = canc(a*d - b*c, b*d);;  // Subtraction

let (.*.) (a,b) (c,d) = canc(a*c, b*d);;     // Multiplication

let (./.) (a,b) (c,d) = (a,b) .*. mkQ(d,c);;       // Division

let (.=.) (a,b) (c,d) = (a,b) = (c,d);;            // Equality

let (.**.) = (.=.)

let toString(p:int,q:int) = (string p) + "/" + (string q);;

let q1 = mkQ(2,-3);;
let q2 = mkQ(5,10);;
let q3 = q1 .+. q2;;
toString(q1 .-. q3 ./. q2);;

let q4 = mkQ(1,2);;
let q5 = q4 .+. q4 .*. q4

// From Section 3.8 Tagged values. Constructors
type Shape = | Circle of float
             | Square of float
             | Triangle of float*float*float;;

let area = function
    | Circle r        -> System.Math.PI * r * r
    | Square a        -> a * a
    | Triangle(a,b,c) ->
          let s = (a + b + c)/2.0
          sqrt(s*(s-a)*(s-b)*(s-c));;

let isShape = function
    | Circle r        -> r > 0.0
    | Square a        -> a > 0.0
    | Triangle(a,b,c) ->
        a > 0.0 && b > 0.0 && c > 0.0
        && a < b + c && b < c + a && c < a + b;;

let area1 x =
    if not (isShape x)
    then failwith "not a legal shape" raise
    else match x with
         | Circle r        -> System.Math.PI * r * r
         | Square a        -> a * a
         | Triangle(a,b,c) ->
             let s = (a + b + c)/2.0
             sqrt(s*(s-a)*(s-b)*(s-c));;


// From Section 3.9 Enumeration types
type Colour = Red | Blue | Green | Yellow | Purple;;

let niceColour = function
    | Red  -> true
    | Blue -> true
    | _    -> false;;

type Month = January | February | March | April
             | May | June | July | August | September
             | October | November | December;;

let daysOfMonth = function
    | February                            -> 28
    | April | June | September | November -> 30
    | _                                   -> 31;;

// From Section 3.10 Exceptions
let solveText eq =
    try
        string(solve eq)
    with
    | Solve -> "No solutions";;


// From Section 3.11 Partial functions. The option type
let rec fact = function
    | 0 -> 1
    | n -> n * fact(n-1);;  

let optFact n = if n < 0 then None else Some(fact n);;

let rec optFact1 = function
  | 0            -> Some 1
  | n when n > 0 -> Some(n * Option.get(optFact1(n-1)))
  | _            -> None;;










