// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 5: Collections: Lists, maps and sets. 
// Just from Section 5.2 Finite sets

let setOfCounts s = Set.map Set.count s;;

let sumSet s = Set.foldBack (+) s 0;;

let setOfCounts1 s = Set.foldBack
                       (fun se sn -> Set.add (Set.count se) sn)
                       s
                       Set.empty;;

let sumSet1 s = Set.fold (+) 0 s;;

let setOfCounts2 s = Set.fold
                       (fun sn se -> Set.add (Set.count se) sn)
                       Set.empty
                       s;;

let rec tryFind p s =
   if Set.isEmpty s then None
   else let minE = Set.minElement s
        if p minE then Some minE
        else tryFind p (Set.remove minE s);;

// Map colouring

type Country   = string;;
type Map       = Set<Country*Country>;;
type Colour    = Set<Country>;;
type Colouring = Set<Colour>;;

let areNb c1 c2 m =
    Set.contains (c1,c2) m || Set.contains (c2,c1) m;;

let canBeExtBy m col c =
    Set.forall (fun c' -> not (areNb c' c m)) col;;

let rec extColouring m cols c =
    if Set.isEmpty cols
    then Set.singleton (Set.singleton c)
    else let col = Set.minElement cols
         let cols' = Set.remove col cols
         if canBeExtBy m col c
         then Set.add (Set.add c col) cols'
         else Set.add col (extColouring m cols' c);;

let countries m =
    Set.fold
      (fun set (c1,c2) -> Set.add c1 (Set.add c2 set))
      Set.empty
      m;;

let colCntrs m cs = Set.fold (extColouring m) Set.empty cs;;

let colMap m = colCntrs m (countries m);;

let exMap = Set.ofList [("a","b"); ("c","d"); ("d","a")];;


// Improved version is based on:

type Country1   = string;;
type Map1       = Set<Country1*Country1>;;
type Colour1    = Set<Country1>;;
type Colouring1 = Colour1 list;;


let rec extColouring1 m cols c =
    match cols with
    | []         -> [Set.singleton c]
    | col::cols' -> if canBeExtBy m col c
                    then (Set.add c col)::cols'
                    else col::(extColouring1 m cols' c);;

let colCntrs1 m cs = Set.fold (extColouring1 m) [] cs;;

let colMap1 m = colCntrs1 m (countries m);;








