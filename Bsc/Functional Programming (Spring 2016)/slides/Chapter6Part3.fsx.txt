// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 6: Finite trees. Just from Section 6.5, 6.6 and 6.7

// From Section 6.5 Expression trees

type ExprTree = | Const of int
                | Ident of string
                | Minus of ExprTree
                | Sum   of ExprTree * ExprTree
                | Diff  of ExprTree * ExprTree
                | Prod  of ExprTree * ExprTree
                | Let of string * ExprTree * ExprTree;;

let t1 =
    Prod(Ident "a",
         Sum(Minus (Const 3),
             Let("x", Const 5, Sum(Ident "x", Ident "a"))));;

let t2 =
    Prod(Ident "a",
         Sum(Const -3,
             Let("x", Const 5, Sum(Ident "x", Ident "a"))));;


let rec eval t env =
    match t with
    | Const n      -> n
    | Ident s      -> Map.find s env
    | Minus t      -> - (eval t env)
    | Sum(t1,t2)   -> eval t1 env + eval t2 env
    | Diff(t1,t2)  -> eval t1 env - eval t2 env
    | Prod(t1,t2)  -> eval t1 env * eval t2 env
    | Let(s,t1,t2) -> let v1   = eval t1 env
                      let env1 = Map.add s v1 env
                      eval t2 env1;;

let env = Map.add "a" -7 Map.empty;;

eval t2 env


// From Section 6.6 Trees with a variable number of sub-trees. Mutual recursion

type ListTree<'a> = Node of 'a * (ListTree<'a> list);;

let rec depthFirst (Node(x,ts)) =
    x :: (List.collect depthFirst ts);;

let rec depthFirstFold f e (Node(x,ts)) =
    List.fold (depthFirstFold f) (f e x) ts;;


let rec breadthFirstList = function
    | []                   -> []
    | (Node(x,ts)) :: rest ->
          x :: breadthFirstList(rest@ts);;

let breadthFirst t = breadthFirstList [t];;

let rec breadthFirstFoldBackList f ts e =
    match ts with
    | []    -> e
    | (Node(x,ts))::rest ->
          f x (breadthFirstFoldBackList f (rest@ts) e);;

let breadthFirstFoldBack f t e =
                 breadthFirstFoldBackList f [t] e;;


// Example: File system

type FileSys = Element list
and Element  = | File of string
               | Dir of string * FileSys;;

let d1 =
  Dir("d1",[File "a1";
            Dir("d2", [File "a2"; Dir("d3", [File "a3"])]);
            File "a4";
            Dir("d3", [File "a5"])
           ]);;

let rec namesFileSys = function
    | []    -> []
    | e::es -> (namesElement e) @ (namesFileSys es)
and namesElement = function
    | File s    -> [s]
    | Dir(s,fs) -> s :: (namesFileSys fs);;

namesElement d1;;

// From Section 6.7 Electrical circuits

type Circuit<'a> = | Comp of 'a
                   | Ser  of Circuit<'a> * Circuit<'a>
                   | Par  of Circuit<'a> * Circuit<'a>;;

let cmp = Ser(Par(Comp 0.25, Comp 1.0), Comp 1.5);;

let rec count = function
    | Comp _     -> 1
    | Ser(c1,c2) -> count c1 + count c2
    | Par(c1,c2) -> count c1 + count c2;;

let rec resistance = function
    | Comp r     -> r
    | Ser(c1,c2) -> resistance c1 + resistance c2
    | Par(c1,c2) ->
          1.0 / (1.0/resistance c1 + 1.0/resistance c2);;

let rec circRec (c,s,p) = function
    | Comp x     -> c x
    | Ser(c1,c2) ->
          s (circRec (c,s,p) c1) (circRec (c,s,p) c2)
    | Par(c1,c2) ->
          p (circRec (c,s,p) c1) (circRec (c,s,p) c2);;

let count1 circ = circRec((fun _ -> 1), (+), (+)) circ : int;;

let resistance1 =
    circRec(
        (fun r -> r),
        (+),
        (fun r1 r2 -> 1.0/(1.0/r1 + 1.0/r2)));;



