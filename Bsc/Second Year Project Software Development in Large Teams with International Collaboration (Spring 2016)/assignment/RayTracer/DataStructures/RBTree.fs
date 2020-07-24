module RBTree
//Source: https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures
//This module will be used to hold vertices / triangles in order to optimize the parsing process.
//The source is used as a platform for our own implementation (Vertices and triangles).
type color = Red | Black
type 'a node when 'a : comparison = int*'a
type 'a tree when 'a : comparison =
    | Empty
    | Tree of color * 'a tree * 'a node * 'a tree
    
module Tree =
    //Gets the head of the given tree
    let hd = function
        | Empty -> failwith "empty"
        | Tree(color, left, node, right) -> node
    
    //Gets the left tree of the given tree
    let left = function
        | Empty -> failwith "empty"
        | Tree(color, left, node, right) -> left
    
    //Gets the right tree of the given tree
    let right = function
        | Empty -> failwith "empty"
        | Tree(color, left, node, right) -> right
    
    //Checks if a given item exists in the given tree
    let rec exists item = function
        | Empty -> false
        | Tree(color, left, node, right) ->
            if item = snd node then true //Found the indexed item
            elif item < snd node then exists item left //item is smaller than current node Tx, traversing left
            else exists item right //item is larger than current node Tx, traversing right
    
    //Balances the tree after insert
    let balance = function                                              (* Red nodes in relation to black root *) //4 cases where tree should be balanced.
        | Black, Tree(Red, Tree(Red, tree, node2, tree1), node, tree2), node3, tree3            (* Left, left *)
        | Black, Tree(Red, tree, node2, Tree(Red, tree1, node, tree2)), node3, tree3            (* Left, right *)
        | Black, tree, node2, Tree(Red, Tree(Red, tree1, node, tree2), node3, tree3)            (* Right, left *)
        | Black, tree, node2, Tree(Red, tree1, node, Tree(Red, tree2, node3, tree3))            (* Right, right *)
            -> Tree(Red, Tree(Black, tree, node2, tree1), node, Tree(Black, tree2, node3, tree3))
        | color, left, node, right -> Tree(color, left, node, right)
    
    //Inserts item into given tree
    //If tree is empty, the item becomes root
    //If item already exists, the given item is returned.
    let insert item tree =
        let rec ins = function
            | Empty -> Tree(Red, Empty, item, Empty)
            | Tree(color, left, head, right) as node ->
                if fst item = fst head then node
                elif fst item < fst head then balance(color, ins left, head, right)
                else balance(color, left, head, ins right)

        (* Forcing root node to be black *)                
        match ins tree with
            | Empty -> failwith "Should never return empty from an insert"
            | Tree(_, left, node, right) -> Tree(Black, left, node, right)
            
    let rec print (spaces : int) = function
        | Empty -> ()
        | Tree(c, l, x, r) ->
            print (spaces + 4) r
            printfn "%s %A%A" (new System.String(' ', spaces)) c x
            print (spaces + 4) l
    
    //Searches tree for node given index.
    //Insert can function as a search method, but on item, not index.
    let rec getNode tree index =
        match tree with
        |Empty -> failwith "The tree is empty, no result found"
        |Tree(color,left,node,right) -> if fst node = index then Tree(color, left, node, right) //Node with given index found
                                        elif fst node < index then getNode right index //Current node has index lower than given index, traverse right subtree
                                        else getNode left index //Current node has index higher than given index, traverse left subtree

type 'a BinaryTree when 'a : comparison (inner : 'a tree) =
    member this.hd = Tree.hd inner
    member this.left = BinaryTree(Tree.left inner)
    member this.right = BinaryTree(Tree.right inner)
    member this.exists item = Tree.exists item inner
    member this.insert item = BinaryTree(Tree.insert item inner)
    member this.print() = Tree.print 0 inner
    static member empty = BinaryTree<'a>(Empty)