// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 5: Collections: Lists, maps and sets. 
// Just from Section 5.1 Lists


// Start --- programs needed from Chapter 3 
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

let toString(p:int,q:int) = (string p) + "/" + (string q);;

type Shape = | Circle of float
             | Square of float
             | Triangle of float*float*float;;

let area = function
    | Circle r        -> System.Math.PI * r * r
    | Square a        -> a * a
    | Triangle(a,b,c) ->
          let s = (a + b + c)/2.0
          sqrt(s*(s-a)*(s-b)*(s-c));;

// End -- programs needed from Chapter 3


let addFsExt = List.map (fun s -> s + ".fs");;

let intPairToRational = List.map (toString << mkQ);;

let areaList = List.map area;;

let intPairToRational1 = List.map (fun p -> toString(mkQ p));;

let intPairToRational2 ps =
    List.map (fun p -> toString(mkQ p)) ps;;

let isMember x xs = List.exists (fun y -> y=x) xs;;

let norm(x:float,y:float) = sqrt(x*x+y*y);;

let sumOfNorms vs =
  List.fold (fun s (x,y) -> s + norm(x,y)) 0.0 vs;;

let vs = [(1.0,2.0); (2.0,1.0); (2.0, 5.5)];;

let length lst = List.fold (fun e _ -> e+1) 0 lst;;

let rev xs = List.fold (fun rs x -> x::rs) [] xs;;

let backSumOfNorms vs =
  List.foldBack (fun (x,y) s -> s + norm(x,y)) vs 0.0;;

let app ys zs = List.foldBack (fun x xs -> x::xs) ys zs;;

let unzip zs = List.foldBack
                 (fun (x,y) (xs,ys) -> (x::xs,y::ys))
                 zs
                 ([],[]);;

let revUnzip zs =
  List.fold (fun (xs,ys) (x,y) -> (x::xs,y::ys)) ([],[]) zs;;

let map f xs = List.foldBack (fun x rs -> f x :: rs) xs [];;









