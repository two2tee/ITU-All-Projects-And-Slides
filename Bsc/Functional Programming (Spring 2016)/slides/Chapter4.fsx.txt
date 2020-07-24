// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 4: Lists. 

// From Section 4.3 Typical recursions over lists

let rec suml = function
    | []    -> 0
    | x::xs -> x + suml xs;;
    
let rec altsum = function
    | []         -> 0
    | [x]        -> x
    | x0::x1::xs -> x0 - x1 + altsum xs;;


let rec succPairs = function
    | x0 :: x1 :: xs -> (x0,x1) :: succPairs(x1::xs)
    | _              -> [];;


let rec succPairs1 = function
    | x0::(x1::_ as xs) -> (x0,x1) :: succPairs1 xs
    | _                  -> [];;


let rec sumProd = function
    | []   -> (0,1)
    | x::rest ->
          let (rSum,rProd) = sumProd rest
          (x+rSum,x*rProd);;


let rec unzip = function
    | []          -> ([],[])
    | (x,y)::rest ->
          let (xs,ys) = unzip rest
          (x::xs,y::ys);;

let rec mix = function
    | (x::xs,y::ys) -> x::y::(mix (xs,ys))
    | ([],[])       -> []
    | _             -> failwith "mix: parameter error";;


let rec mix1 xlst ylst =
    match (xlst,ylst) with
    | (x::xs,y::ys) -> x::y::(mix1 xs ys)
    | ([],[])       -> []
    | _             -> failwith "mix: parameter error";;



// From Section 4.4 Polymorphism

let rec isMember x = function
    | y::ys -> x=y || (isMember x ys)
    | []    -> false;;


let rec naive_rev = function
 | []    -> []
 | x::xs -> naive_rev xs @ [x]

compare (set ["Bob";"Bill"]) (set ["Bill";"Bob";"Bill"])

// From Section 4.6 Examples. A model-based approach

// Example: Cash register

type ArticleCode = string;;
type ArticleName = string;;
type Price       = int;;         // pr  where  pr >= 0
type Register    = (ArticleCode * (ArticleName*Price)) list;;

let reg = [("a1",("cheese",25));
           ("a2",("herring",4));
           ("a3",("soft drink",5)) ];;


type NoPieces    = int;;         // np  where np >= 0
type Item        = NoPieces * ArticleCode;;
type Purchase    = Item list;;

let pur = [(3,"a2"); (1,"a1")];;

type Info        = NoPieces * ArticleName * Price;;
type Infoseq     = Info list;;
type Bill        = Infoseq * Price;;

let rec findArticle ac = function
    | (ac',adesc)::_ when ac=ac' -> adesc
    | _::reg                     -> findArticle ac reg
    | _                          ->
           failwith(ac + " is an unknown article code");;

let rec makeBill reg = function
  | []           -> ([],0)
  | (np,ac)::pur -> let (aname,aprice) = findArticle ac reg
                    let tprice         = np*aprice
                    let (billtl,sumtl) = makeBill reg pur
                    ((np,aname,tprice)::billtl,tprice+sumtl);;


// Example: Map colouring

type Country = string;;
type Map     = (Country * Country) list;;

type Colour     = Country list;;
type Colouring  = Colour list;;

let areNb m c1 c2 =
    isMember (c1,c2) m || isMember (c2,c1) m;;

let rec canBeExtBy m col c =
    match  col with
    | []       -> true
    | c'::col' -> not(areNb m c' c) && canBeExtBy m col' c;;

let rec extColouring m cols c =
    match cols with
    | []         -> [[c]]
    | col::cols' -> if canBeExtBy m col c
                    then (c::col)::cols'
                    else col::extColouring m cols' c;;

let addElem x ys = if isMember x ys then ys else x::ys;;

let rec countries = function
    | []           -> []
    | (c1,c2)::m -> addElem c1 (addElem c2 (countries m));;

let rec colCntrs m = function
    | []    -> []
    | c::cs -> extColouring m (colCntrs m cs) c;;


let colMap m = colCntrs m (countries m);;

let exMap = [("a","b"); ("c","d"); ("d","a")];;









        
                                              
