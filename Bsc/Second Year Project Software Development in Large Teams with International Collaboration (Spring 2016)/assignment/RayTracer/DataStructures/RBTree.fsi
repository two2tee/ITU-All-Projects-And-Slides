module RBTree
//Source: https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures
//This module will be used to hold vertices / triangles in order to optimize the parsing process.
//The source is used as a platform for our own implementation (Vertices and triangles)
type 'a node when 'a : comparison = int*'a
[<Class>]
type 'a BinaryTree when 'a : comparison =
    member hd : 'a node
    member left : 'a BinaryTree
    member right : 'a BinaryTree
    member exists : 'a -> bool
    member insert : 'a node -> 'a BinaryTree
    member print : unit -> unit
    static member empty : 'a BinaryTree