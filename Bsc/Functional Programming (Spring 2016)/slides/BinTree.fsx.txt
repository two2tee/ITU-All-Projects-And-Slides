type 'a BinTree =
    Leaf
  | Node of 'a * 'a BinTree * 'a BinTree

let intBinTree = 
  Node(43, Node(25, Node(56,Leaf, Leaf), Leaf),
                    Node(562, Leaf, Node(78, Leaf, Leaf)))

let rec countNodes tree =
  match tree with
    Leaf -> (1,0)
  | Node(_,treeL, treeR) ->
      let (ll,lr) = countNodes treeL in
      let (rl,rr) = countNodes treeR in
      (ll+rl,1+lr+rr);

let floatBinTree = 
  Node(43.0,Node(25.0, Node(56.0,Leaf, Leaf), Leaf),
            Node(562.0, Leaf, Node(78.0, Leaf,Leaf)))

let rec preOrder tree =
  match tree with
    Leaf -> []
  | Node(n,treeL,treeR) ->
      n :: preOrder treeL @ preOrder treeR;

preOrder intBinTree;
