// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 8: Imperative features.

// from Section 8.5 Mutable record fields

type intRec  = { mutable count : int };;

let incr (x: intRec) =
    x.count <- x.count + 1
    x.count;;

let makeCounter() =
    let counter = { count = 0 }
    fun () -> incr counter;;

let clock = makeCounter();;

let x = clock();;

let clock2 = makeCounter()
let x = clock2()


// From Section 8.6 References

let incr1 r = (r := !r + 1 ; !r);;
let makeCounter1() =
    let counter = ref 0
    fun () -> incr1 counter;;

let clock1 = makeCounter1();;

// From Section 8.9 Imperative tree traversal

type BinTree<'a> =
     | Leaf
     | Node of BinTree<'a> * 'a * BinTree<'a>;;

let rec preIter f = function
    | Leaf          -> ()
    | Node(tl,x,tr) -> f x ; preIter f tl ; preIter f tr;;

let rec inIter f = function
    | Leaf          -> ()
    | Node(tl,x,tr) -> inIter f tl ; f x ; inIter f tr;;


type ListTree<'a> = Node of 'a * (ListTree<'a> list);;
let rec depthFirstIter f (Node(x,ts)) =
    f x ; List.iter (depthFirstIter f) ts;;


// From Section 8.10 Arrays

open System;;
open System.IO;;

let Acode = int 'A'

let histogram path =
    let charCount   = [| for n in 'A'..'Z' -> 0 |]
    let file = File.OpenText path
    while (not file.EndOfStream) do
        let ch = char( file.Read() )
        if (Char.IsLetter ch) then
            let n = int (Char.ToUpper ch)  - Acode
            charCount.[n] <- charCount.[n] + 1
        else ()
    file.Close()
    let printOne n c = printf "%c:  %4d{\ttbsl}n" (char(n + Acode)) c
    Array.iteri printOne charCount;;


// From Section 8.12 Functions on collections. Enumerator functions

open System.Collections.Generic;;

let enumerator (m: IEnumerable<'c>) =
    let e = ref (m.GetEnumerator())
    let f () =
        match (!e).MoveNext() with
        | false -> None
        | _     -> Some ((!e).Current)
    f;;

let f = enumerator (Set.ofList [3 ; 1; 5]);;

let d = SortedDictionary<string,int>();;
d.Add("cd",3) ; d.Add("ab",5);;
let g = enumerator d;;

let tryFind p (s: Set<'a>) =
     let f = enumerator s
     let rec tFnd () =
         match f() with
         | None   -> None
         | Some x ->
             if (p x) then Some x else tFnd()
     tFnd();;

let s = Set.ofList [1;3;4;5];;


// From Section 8.13 Imperative queue

let breadthFirstIter f ltr =
    let remains = Queue<ListTree<'a>>()
    remains.Enqueue ltr
    while (remains.Count <> 0) do
        let (Node (x,tl)) = remains.Dequeue()
        List.iter (remains.Enqueue) tl
        f x;;

