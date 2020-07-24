// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 6: Finite trees.
// Just from Section 6.4 Traversal of binary trees. Search trees


type BinTree<'a when 'a : comparison> =
     | Leaf
     | Node of BinTree<'a> * 'a * BinTree<'a>;;

let rec preOrder = function
    | Leaf          -> []
    | Node(tl,x,tr) -> x :: (preOrder tl) @ (preOrder tr);;

let rec inOrder = function
    | Leaf          -> []
    | Node(tl,x,tr) -> (inOrder tl) @ [x] @ (inOrder tr);;

let rec postOrder = function
    | Leaf          -> []
    | Node(tl,x,tr) -> (postOrder tl) @ (postOrder tr) @ [x];;

let rec postFoldBack f t e =
    match t with
    | Leaf          -> e
    | Node(tl,x,tr) ->
          let ex = f x e
          let er = postFoldBack f tr ex
          postFoldBack f tl er;;

let rec add x t =
    match t with
    | Leaf                   -> Node(Leaf,x,Leaf)
    | Node(tl,a,tr) when x<a -> Node(add x tl,a,tr)
    | Node(tl,a,tr) when x>a -> Node(tl,a,add x tr)
    | _                      -> t;;

let rec contains x = function
    | Leaf                  -> false
    | Node(tl,a,_) when x<a -> contains x tl
    | Node(_,a,tr) when x>a -> contains x tr
    | _                     -> true;;


