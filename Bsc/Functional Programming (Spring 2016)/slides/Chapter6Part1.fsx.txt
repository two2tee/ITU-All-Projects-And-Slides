// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 6: Finite trees. Just from the sections 6.1, 6.2 and 6.3

// From Section 6.1 Chinese boxes

type Colour = Red | Blue | Green | Yellow | Purple;;

type Cbox = | Nothing                         // 1.
            | Cube of float * Colour * Cbox;; // 2.

let cb1 = Cube(0.5, Red, Nothing);;
let cb2 = Cube(1.0, Green, cb1);;
let cb3 = Cube(2.0, Yellow, cb2);;

let rec count = function
    | Nothing       -> 0
    | Cube(r,c,cb)  -> 1 + count cb;;

let rec count2 c =
    match c with
      Nothing -> 0
    | Cube(r,c,cb) -> 1 + count2 cb;;


let foo1 = Cube(10.0,Red,Cube(20.0,Green,Nothing))

let rec insert(r,c,cb) =
  if r <= 0.0 then failwith "ChineseBox"
  else match cb with
       | Nothing         -> Cube(r,c,Nothing)
       | Cube(r1,c1,cb1) ->
             match compare r r1 with
             | t when t > 0 -> Cube(r,c,cb)
             | 0            -> failwith "ChineseBox"
             | _            -> Cube(r1,c1,insert(r,c,cb1));;


type Cbox1 = | Single of float * Colour
             | Multiple of float * Colour * Cbox1;;

let rec count1 = function
  | Single _         -> 1
  | Multiple(_,_,cb) -> 1 + count1 cb;;

let rec insert1 (r1,c1,cb2) =
  if r1 <= 0.0 then failwith "insert1: Chinese box"
  else match cb2 with
       | Single (r2,c2)       ->
         match compare r1 r2 with
         | t when t < 0 -> Multiple(r2,c2,Single(r1,c1))
         | 0            -> failwith "ChineseBox"
         |  _           -> Multiple(r1,c1,cb2)
       | Multiple (r2,c2,cb3) ->
         match compare r1 r2 with
         | t when t < 0 -> Multiple(r2,c2,insert1(r1,c1,cb3))
         | 0            -> failwith "ChineseBox"
         | _            -> Multiple(r1,c1,cb2);;


// From Section 6.2 6.2 Symbolic differentiation

type Fexpr = | Const of float
             | X
             | Add of Fexpr * Fexpr
             | Sub of Fexpr * Fexpr
             | Mul of Fexpr * Fexpr
             | Div of Fexpr * Fexpr
             | Sin of Fexpr
             | Cos of Fexpr
             | Log of Fexpr
             | Exp of Fexpr;;


let ex1 = Add(Mul(Const 5.0, X),Mul(X,X))

let rec D = function
  | Const _    -> Const 0.0
  | X          -> Const 1.0
  | Add(fe,ge) -> Add(D fe, D ge)
  | Sub(fe,ge) -> Sub(D fe, D ge)
  | Mul(fe,ge) -> Add(Mul(D fe, ge), Mul(fe, D ge))
  | Div(fe,ge) -> Div(Sub(Mul(D fe,ge), Mul(fe,D ge)),
                      Mul(ge,ge))
  | Sin fe     -> Mul(Cos fe, D fe)
  | Cos fe     -> Mul(Const -1.0, Mul(Sin fe, D fe))
  | Log fe     -> Div(D fe, fe)
  | Exp fe     -> Mul(Exp fe, D fe);;

let rec toString = function
    | Const x       -> string x
    | X             -> "x"
    | Add(fe1,fe2)  -> "(" + (toString fe1) + ")"
                       + " + " + "(" + (toString fe2) + ")"
    | Sub(fe1,fe2)  -> "(" + (toString fe1) + ")"
                       + " - " + "(" + (toString fe2) + ")"
    | Mul(fe1,fe2)  -> "(" + (toString fe1) + ")"
                       + " * " + "(" + (toString fe2) + ")"
    | Div(fe1,fe2)  -> "(" + (toString fe1) + ")"
                       + " / " + "(" + (toString fe2) + ")"
    | Sin fe        -> "(sin " + (toString fe) + ")"
    | Cos fe        -> "(cos " + (toString fe) + ")"
    | Log fe        -> "(log " + (toString fe) + ")"
    | Exp fe        -> "(exp " + (toString fe) + ")";;


 let rec compute x = function
       Const r ->r
     | X ->x
     | Add(fe1,fe2) -> compute x fe1 + compute x fe2
     | Sub(fe1,fe2) -> compute x fe1 - compute x fe2
     | Mul(fe1,fe2) -> compute x fe1 * compute x fe2
     | Div(fe1,fe2) -> compute x fe1 / compute x fe2
     | Sin fe -> System.Math.Sin (compute x fe)
     | Cos fe -> System.Math.Cos (compute x fe)
     | Log fe -> System.Math.Log (compute x fe)
     | Exp fe -> System.Math.Exp (compute x fe)

// From Section 6.3 Binary trees. Parameterized types

type BinTree<'a,'b> =
     | Leaf of 'a
     | Node of BinTree<'a,'b> * 'b * BinTree<'a,'b>;;

let t1 = Node(Node(Leaf 1,"cd",Leaf 2),"ab",Leaf 3);;

let rec depth = function
    | Leaf _        -> 0
    | Node(t1,_,t2) -> 1 + max (depth t1) (depth t2);;


