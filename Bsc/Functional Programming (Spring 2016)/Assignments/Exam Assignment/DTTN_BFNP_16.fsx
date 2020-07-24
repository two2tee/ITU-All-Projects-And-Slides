open System
(*  //DISCLAIMER//
    I hereby declare that I myself have created this exam hand-in in its entirety without help from
    anybody else.


    Dennis Thinh Tan Nguyen | 01/04/1993 | CODE-BFNP | 03/06/16 | 
*)


//Note You MUST include explanations and comments to support your solutions


//Question 1
type Multiset<'a when 'a: comparison> = MSet of Map<'a, int>

let ex = MSet (Map.ofList [("a",1);("b",2);("c",1)])

let wrong = MSet (Map.ofList [("a",0);("b",2);("c",1)])

//Question 1.1

let diceSet = MSet(Map.ofList[(1,2);(2,1);(3,5);(5,2);(6,2)])

//The type of the value is the type of integers.

//No as the functions are used in a context that is not expected. One can for instance not compare function in F# which is a requirement for the multiset that the values 'a can be compared

//Question 1.2
let newMultiset =  MSet(Map.empty)

let isEmpty ms = 
    match ms with
    | MSet m -> m.IsEmpty //Using Map.isEmpty function


//Question 1.3

//This function adds elements to a given multiset
let add k ms = 
    match ms with
    | MSet m -> if m.ContainsKey k
                then let value = m.TryFind k
                     match value with
                     |Some v -> MSet(Map.add k (v+1) m) //If key already exist, increment value with one
                     |None -> failwith "malformed value" 
                else let nList = (k,1)::(Map.toList m)
                     MSet(Map.ofList nList)  //If key does not exist add a new value of 1 binded to the given key

//This help function will remove a given element from a list
//It is used in conjuction with del function
let deleteFromList toDel l =
    List.fold(fun acc (k,v) -> if toDel = k 
                               then acc
                               else (k,v)::acc )[] l 

//Removes a given element from a multiset       
let del k ms =
    match ms with
    |MSet m -> if m.ContainsKey k
               then let nList = m |> Map.toList |> (deleteFromList k)
                    MSet(Map.ofList nList)
               else ms
 

//Question 1.4
//I have defined two toList functions one that creates a list of values and another of keys
//I was not sure which one was required so both was created. The given question said the returned value 
// must be a list value, but the example was a list with keys.

//This function makes a list containing the keys from the multiset (Or the values from the set)
let toListK ms =
    match ms with
    | MSet m -> let list = Map.toList m
                List.fold(fun acc (k,v)-> k::acc)[] list

//this functions makes a list containing the values from the multiset (That is the number of value occurrences) 
let toListV ms =
    match ms with
    | MSet m -> let list = Map.toList m
                List.fold(fun acc (k,v)-> v::acc)[] list



//Question 1.5
//This function takes a list and creates a new multiset
let fromList l =
    let map = List.fold(fun acc k -> add k acc ) newMultiset l
    map


//Maps for each element with a given function in a multiset 
let map f ms =
    let mlist = toListK ms |> List.map(fun k -> f k)
    fromList mlist

//Folds a multiset
let fold f a ms =
    let mList = toListK ms
    MSet(List.fold f a mList)

//Question 1.6

//Union to given multisets
let union msX msY =
    let lX = toListK msX
    let rec union' lx acc =
        match lx with
        | [] -> acc
        | x::xs -> union' xs (add x acc) //Add item from multiset y to multiset x
    union' lX msY
    
//Almost same as union but the difference is the fact that you from the acc
let minus msX msY =
    let lX = toListK msX
    let rec minus' lx acc =
        match lx with
        | [] -> acc
        | x::xs -> minus' xs (del x acc)
    minus' lX msY


//Question 2
let rec f n =
    if n < 10 
    then "f" + g (n+1)
    else "f"
and g n =
    if n < 10 then "g" + f(n+1)
    else "g"

(*
    For each infinite even negative integers and positive number up till 10, the result will always be a string that starts
    with f and ends with f 
*)

(*
    yes one can just call the function g
*)
g 4 = "gfgfgfg"


(*
    Assume that (int)Single.NegativeInfinity does return a negative infinity number and is applied to
    the function f or g. The function f and g will start an infinite computation 
*)


//Question 2.2
let rec fA n acc=
    if n < 10 
    then gA(n+1) ("f"+acc)
    else "f"
and gA n acc=
    if n < 10 then fA(n+1) ("g"+acc)
    else "g"

fA 0 ""
(*
    The result i get from my tail recursive functions is wrong
    It will only return a given string with one single char
    As far as i can see, i am concatenating the string with acc before 
    handing it over to the next function and thus it should work, but it does not.
    But the idea is to concatenate f or g with acc and then hand acc to the next function
*)

//Question 3

//Question 3.1
let myFinSeq(n,m) = seq {for i in [0..n] do
                            yield! seq{for j in [0..m] do yield j}}

(*
    myFinSeq returns a sequence that contains n+1 subsequneces of 0 to m
    Note that the subsequences are merged into the final sequence
*)
(*
    No it is not possible to generate a sequence  such that there exist some subsequences Sa in S that differs from
    other subsequences sN in S.
     yield! seq{for j in [0..m] do yield j will always generate a sub sequence from 0 to m
     thus one can not generate a sequence mentioned in the question

*)

//Question 3.2
let myFinSeq2(n,m) =  let s = seq{for j in [0..m] do yield j}
                      let c = 0
                      let rec makeL c acc =
                        match c with
                        | c when c = n -> if c=0 && n=0 
                                          then  (c,s)::acc
                                          else (c,s)::acc
                        | c when c<n -> makeL (c+1) ((c,s)::acc)
                        | _ -> failwith "n is less than zero"
                      Seq.ofList (List.rev (makeL c []))

myFinSeq2(0,0)
myFinSeq2(1,1)
myFinSeq2(1,2)
myFinSeq2(2,1)

//Question 4.0
type Row = int
type Col = char
type CellAddr = Row * Col
type ArithOp = Add | Sub | Mul | Div
type RangeOp = Sum | Count
type CellDef =
  | FCst of float
  | SCst of string
  | Ref of CellAddr
  | RangeOp of CellAddr * CellAddr * RangeOp
  | ArithOp of CellDef * ArithOp * CellDef
type CellValue = 
    S of string
  | F of float
type Sheet = Map<CellAddr,CellDef>
(*
      |        A |        B |        C |        D |        E |        F |        G |        H
  ----+----------+----------+----------+----------+----------+----------+----------+---------
    1 |    #EYES |        1 |        2 |        3 |        4 |        5 |        6 |    Total
    2 |   RESULT |     2.00 |     1.00 |     5.00 |     0.00 |     2.00 |     2.00 |    12.00
    3 |      PCT |    16.67 |     8.33 |    41.67 |     0.00 |    16.67 |    16.67 |   100.00
*)

let header = [((1,'A'),SCst "#EYES");((1,'B'),SCst "1");((1,'C'),SCst "2");
                ((1,'D'),SCst "3");((1,'E'),SCst "4");((1,'F'),SCst "5");
                ((1,'G'),SCst "6");((1,'H'),SCst "Total")]
let result = [((2,'A'),SCst "RESULT");((2,'B'),FCst 2.0);((2,'C'),FCst 1.0);
                ((2,'D'),FCst 5.0);((2,'E'),FCst 0.0);((2,'F'),FCst 2.0);
                ((2,'G'),FCst 2.0);((2,'H'),RangeOp((2,'B'),(2,'G'),Sum))]
let calcPct col = ArithOp(FCst 100.0, Mul, ArithOp(Ref(2,col),Div,Ref(2,'H')))
let pct = [((3,'A'),SCst "PCT");((3,'B'),calcPct 'B');((3,'C'),calcPct 'C');
             ((3,'D'),calcPct 'D');((3,'E'),calcPct 'E');((3,'F'),calcPct 'F');
             ((3,'G'),calcPct 'G');((3,'H'),calcPct 'H')]
let dice = Map.ofList (header @ result @ pct)

//Question 4.1

(*
      |        B |        C
    ----+----------+---------
      4 |     NAME |   HEIGHT
      5 |     Hans |   167.40
      6 |    Trine |   162.30
      7 |    Peter |   179.70
      8 |          |         
      9 |     3.00 |   169.80

*)

//This list represents the header of the sheet
let headerH = [((4,'B'),SCst "NAME");((4,'C'),SCst "HEIGHT")]

//This list represents the column B of the sheet
let personsB = [((5,'B'),SCst "Hans");((6,'B'),SCst "Trine"); ((7,'B'),SCst "Peter");((9,'B'),RangeOp((5,'B'),(7,'B'),Count))]

//This list represents the column C of the sheet
let heightC =  [((5,'C'),FCst 167.40);((6,'C'),FCst 162.30); ((7,'C'),FCst 179.70);((9,'C'),ArithOp(RangeOp((5,'C'),(7,'C'),Sum),Div,Ref(9,'B')))]

let heights = Map.ofList (headerH @ personsB @ heightC)


//Question 4.2

//Function to retrieve float value
let getF = function
    | F f -> f
    | S s -> failwith "getF: expecting a float but got a string"

let getS = function
    | F f -> failwith "getS: expecting a string but got a float"
    | S s -> s

let evalRangeOp xs op =
    match op with
    | Sum ->   List.fold(fun acc x-> acc+(getF x)) 0.0 xs
    | Count -> List.fold(fun acc x-> acc+1.0) 0.0 xs

evalRangeOp [F 33.0; F 32.0] Sum = 65.0
evalRangeOp [] Sum = 0.0
evalRangeOp [F 23.0; S "Hans"] Sum //Throws exception
evalRangeOp [F 33.0; S "Hans"] Count = 2.0


let evalArithOp v1 v2 op =
    match op with
    | Add -> let x = getF v1
             let y = getF v2
             x+y
    | Sub ->
             let x = getF v1
             let y = getF v2
             x-y
    | Mul ->
            let x = getF v1
            let y = getF v2
            x*y
    | Div -> 
             let x = getF v1
             let y = getF v2
             match y with //Check for zero
             | 0.0 -> failwith "please don't divide by zero"
             | y -> x/y

evalArithOp (F 33.0) (F 32.0) Sub = 1.0
evalArithOp (S "Hans") (F 32.0) Add // Throws exception
evalArithOp (F 10.0) (F 0.0) Div // Throws exception
evalArithOp (F 10.0) (F 2.0) Div = 5.0

//Question 4.3
//This function retrieves a value from the sheet


let rec evalValue v (sheet: Sheet) = 
    match v with
    | FCst f-> F f 
    | SCst s -> S s
    | Ref ca -> evalCell ca sheet
    | RangeOp ((r1,c1),(r2,c2),op) -> let rec mkList acc =
                                        match r1 with
                                        | r1 when r1 = r2 -> acc //Returns retrieved cells in a list
                                        | r1 when r1 < r2 -> (evalCell(r1+1,c1) sheet)::acc //Getting next cell
                                      let list = mkList ((evalCell(r1,c1)sheet)::[]) //list of cell values
                                      F(evalRangeOp list op) //Evaluate cell values with evalrangeOP

    | ArithOp (v1,op,v2) -> let x = (evalValue v1 sheet)
                            let y = (evalValue v2 sheet)
                            F (evalArithOp x y op)
and evalCell ca sheet  = 
    match Map.tryFind ca sheet with
    |None -> S ""
    |Some v -> evalValue v sheet

evalCell (3,'G') dice //I get the wrong value  F100 but I believe the error is due to the implementation of RangeOp

//Question 4.3
(*
    To solve this question, I would create a function that takes a sheet and folds it. The function will utilize the evalCell and for each cell address in the sheet. Based on the cell value returned, pattern matching can be used to print
    out appropriate strings.




*)

