

//Exercise 2.1

let downTo n = 
    if n > 0 
    then [n.. -1 ..1]
    else failwith "invalid argument";;


let downTo2 n =
    match n with 
    | n when n <= 0 -> []
    | n when n > 0 -> [n.. -1 ..1]
    | _ -> failwith "invalid argument";;


//Exercise 2.2 

let rec removeOddIdx (xs:int list) =
    match xs with
    | [] -> []
    | [x] -> [x]
    | [x0;x1] -> [x0]
    | x :: _ :: tail ->  x :: removeOddIdx tail ;;

removeOddIdx [1;2;3;4;5;6];;

//Exercise 2.3
let rec combinePair (xs:int list) =
    match xs with
    | [] -> []
    | [x] -> []
    | x0 :: x1 :: tail  -> (x0,x1) :: combinePair tail;;


//Exercise 2.4
(*
The former British currency had 12 pence to a shilling and 20 shillings to a pound. Declare
functions to add and subtract two amounts, represented by triples (pounds, shillings, pence) of
integers, and declare the functions when a representation by records is used. Declare the functions
in infix notation with proper precedences, and use patterns to obtain readable declarations

 12 pence = 1 shilling
 20 shillings = 1 pound
*)

type amount = {pounds:int ; shillings:int ; pence:int};;

//Converts amount represented as triple (pounds,shillings,pence) to pence.
let denormalizeAmount x = {pounds = 0; shillings = 0; pence=(x.pounds*240)+(x.shillings*12)+x.pence};;
   

   
//Converts pence to shillings and pounds as a triple (pounds,shillings,pence)
let rec normalizeAmount xs =
    match xs with
    | {pounds = 0; shillings = 0; pence = y} ->    if y < 12                               
                                                   then xs
                                                   else normalizeAmount {pounds = 0; shillings = xs.pence/12; pence = xs.pence%12} // Converts pence to shillings
    | {pounds = 0; shillings = x; pence = y} ->    if x < 20                                  
                                                   then xs
                                                   else {pounds = x/20; shillings = x%20; pence = y};;                // Converts shillings to pounds


let amountToTriple x = (x.pounds, x.shillings, x.pence);;
 
 let addition (a1:amount) (a2:amount) =
    let amountToPence = denormalizeAmount {pounds = a1.pounds+a2.pounds ; shillings = a1.shillings+a2.shillings ; pence = a1.pence+a2.pence}
    let result = normalizeAmount amountToPence
    amountToTriple result;;

let total1 = addition {pounds = 0;shillings=0;pence=1} {pounds = 0;shillings=0;pence=1};;
let total2 = addition {pounds = 10;shillings=10;pence=10} {pounds = 10;shillings=10;pence=1};;


let substract (a1:amount) (a2:amount) =
    let amountToPence = denormalizeAmount {pounds = a1.pounds-a2.pounds ; shillings = a1.shillings-a2.shillings ; pence = a1.pence-a2.pence}
    normalizeAmount amountToPence;;
 

 (*
    Exercise 2.5
 *)

 //1) infix Addition and subtract functions


 let  (.+) (x1,y1) (x2,y2) = (x1+x2,y1+y2);;
 let  (.*) (x1,y1) (x2,y2) = (x1*x2)-(y1*y2) , (y1*x1)+(x1*y2);;

 //2)
 let (./) (x1,y1) (x2,y2) = ( (x1/x1*x1 + y1*y1) , (-y1/x1*x1 + y1*y1) ) .* (x2,y2);;
 let (.-) (x1,y1) (x2,y2) = ((x1,y1) .+ (-x2,-y2)) ;;  
 
 //3 
  let (../) (x1,y1) (x2,y2) = 
    let a = (x1/x1*x1 + y1*y1)
    let b = (-y1/x1*x1 + y1*y1)
    (a,b) .* (x2,y2);;

 (*
    Exercise 2.6
    Give a declaration for altsum (see Page 76) containing just two clauses.
 *)

 let rec altSum = function 
    | [] -> 0
    | x0::xs -> x0 - altSum xs;;
 

 altSum [1;2;3;4;5];;

 (*
    Exercise 2.7
 *)


let explode (s:string) = List.ofArray(s.ToCharArray()) // gives exploded list based on char array 

let rec explode2 (s:string) = if s.Length = 0 
                                then [] 
                                else s.Chars(0) :: explode2 (s.Remove(0,1)) ;; // Remove is used to remove first index and then explodes tail 

(* 
Exercise 2.8

Write a function implode:char list->string
so that implode s returns the characters concatenated into a string: implode [’a’;’b’;’c’] = "abc"
Hint: Use List.foldBack. Now write a function
implodeRev:char list->string
so that implodeRev s returns the characters concatenated in reverse order into a string:
implodeRev [’a’;’b’;’c’] = "cba"
Hint: Use List.fold.

*)

let implode (list : char list) =
    List.foldBack(fun c s -> c.ToString() + s) list "";;


let implodeRev (list : char list) =
    List.fold(fun c s -> s.ToString() + c) "" list;;

(*
Exercise 2.9
*)

let toUpper (s:string) =  explode s |> List.map System.Char.ToUpper |> implode;;

let toUpper1 (s:string) = s |> (explode >> List.map System.Char.ToUpper >> implode);;  //(a >> b >> c)

let toUpper2 (s:string) = s |> (implode << List.map System.Char.ToUpper << explode);;  //(c << b << a)



(* 
Exercise 2.10 

Write a function palindrome:string->bool,
so that palindrome s returns true if the string s is a palindrome; otherwise false.
A string is called a palindrome if it is identical to the reversed string, 
eg, “Anna” is a palindrome but “Ann” is not. The function is not case sensitive.

*)

// Shows that palindrome is case sensitive 
let palindrome s y = s=y;;
palindrome "anna" "ANNA" 
// val it : bool = false 

// Compares first half of string with the second half of the string 
// by exploding the second half and reverting the order and then imploding it.

let palindrone (s:string) = 
    let upper = toUpper(s) // Now also works for input like "ANna" 
    let firstHalf = upper.Substring(0, s.Length/2)
    let secondHalf = s.Substring(s.Length/2, s.Length/2) 
    let explodedSecondHalf = explode secondHalf
    let revertedSecondHalf = implodeRev explodedSecondHalf

    if firstHalf = revertedSecondHalf then true
    else false ;;

//let palindroneOld (s:string) =
//    if toUpper(s.Substring(0, s.Length/2)) 
//    = toUpper(implodeRev(explode (s.Substring(s.Length/2,s.Length/2)))) 
//    then true 
//    else false ;; 

palindrone "Anna";;

(* 
Exercise 2.11 

TheAckermannfunctionisarecursivefunctionwherebothvalueandnumberofmutuallyrecursive calls grow rapidly.
Write the function
ack:int*int->int
that implements the Ackermann function using pattern matching on the cases of (m,n) as given below.
 n + 1 if m = 0
A(m, n) = A(m − 1, 1) if m > 0 and n = 0  A(m−1,A(m,n−1)) ifm>0andn>0
What is the result of ack(3,11).
Notice: The Ackermann function is defined for non negative numbers only.

*)

let rec ack (m,n) = 
    match (m,n) with 
    | (0,n) -> n + 1
    | (m,0) when m > 0 -> ack(m-1, 1)
    | (m,n) when m > 0 && n > 0 -> ack(m-1, ack(m,n-1)) 
    | _ -> failwith "Only non negative integers" ;; 

// Result is 16381
let result = ack (3,11) ;; 

(* 
Exercise 2.12 

The function
time f:(unit->’a)->’a*TimeSpan
below times the computation of f x and returns the result and the real time used for the computation.

let time f =
  let start = System.DateTime.Now in
  let res = f () in
  let finish = System.DateTime.Now in
  (res, finish - start);

Try computetime (fun () -> ack (3,11)). 

Write a new function timeArg1 f a : (’a -> ’b) -> ’a -> ’b * TimeSpan 
that times the computation of evaluating the function f with argument a.

Try timeArg1 ack (3,11). 
Hint: You can use the function time above if you hide f a in a lambda (function).

*)

let time f =
    let start = System.DateTime.Now in
        let res = f () in
            let finish = System.DateTime.Now in
                (res, finish - start)

// timeArg1 takes a function (time) as argument and is used as a anonymous function
//  where f a is hidden in a lambda function 
let timeArg1 f a = 
    time (fun() -> f a) // a is a pair (int*int) 

timeArg1 ack (3,11)

(* 
Exercise 2.13 : HR exercise 5.4

Declare a function downto1 such that:
downto1 f n e = f(1, f(2,..., f(n−1, f(n,e))...)) for n>0 down to 1 
f n e = e for n≤0

Declare the factorial function by use of downto1.
Use down to 1 to declare a function that builds the list [g(1), g(2), . . . , g(n)] 
for a function g and an integer n.

*)

// Builds list of functions from parameters 
let rec downto1 f n e =
    match n with
    | n when n <= 0 -> e 
    | n -> 
        let list  = [1 .. n ]
        List.foldBack f list e  //f(n-1, f(n,e))

let factorial n:int =
    if n < 0
        then failwith "Invalid arg"
        else downto1 (fun x y -> x*y) n 1;; 


//Builds [g(1), g(2), . . . , g(n)] 
let rec listBuilder g n =
    downto1 (fun x y -> (g x)::y) n [];;

listBuilder (fun x -> x+1 ) 5;; // ~> [2,3,4,5,6]