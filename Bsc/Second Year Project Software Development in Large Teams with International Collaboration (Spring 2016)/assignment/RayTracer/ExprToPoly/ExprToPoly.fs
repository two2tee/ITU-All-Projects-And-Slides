module ExprToPoly

(*#load "ExprParse.fs"*)

type expr = ExprParse.expr
open ExprParse

let rec ppExpr = function
  | FNum c -> string(c)
  | FVar s -> s
  | FAdd(e1,e2) -> "(" + (ppExpr e1) + " + " + (ppExpr e2) + ")"
  | FMin(e1,e2) -> "(" + (ppExpr e1) + " - " + (ppExpr e2) + ")"
  | FMult(e1,e2) -> (ppExpr e1) + " * " + (ppExpr e2)
  | FDiv(e1,e2) -> (ppExpr e1) + " / " + (ppExpr e2)
  | FExponent(e,n) -> "(" + (ppExpr e) + ")^" + string(n)
  //| FRoot(e1,e2) -> "( Root " + (ppExpr e1) + "_" + (ppExpr e2) + ")"

let rec subst e (x,ex) =
  match e with
  | FInt i -> FInt i
  | FNum c -> FNum c
  | FVar s -> if(s=x) then ex else FVar s
  | FAdd(e1,e2) -> FAdd(subst e1 (x,ex), subst e2 (x,ex))
  | FMin(e1,e2) -> FMin(subst e1 (x,ex), subst e2 (x,ex))
  | FMult(e1,e2) -> FMult(subst e1 (x,ex), subst e2 (x,ex))
  | FDiv(e1,e2) -> FDiv(subst e1 (x,ex), subst e2 (x,ex))
  | FExponent(e1,n) -> FExponent(subst e1 (x,ex),n)
  | FRoot(e1,e2) -> FRoot(subst e1 (x,ex), e2)

type atom = ANum of float | AExponent of string * int
type atomGroup = atom list
type simpleExpr = SE of atomGroup list
let isSimpleExprEmpty (SE ags) = ags = [] || ags = [[]]
    
let simpleExprToExpr (SE ags) =
    let rec agsToExpr _ags =
        match _ags with
        |ANum f::[] -> FNum f
        |AExponent(s,i)::[] -> FExponent(FVar s, i)
        |ANum 0.0::_ -> FNum 0.0
        |ANum f::zs -> FMult(FNum f, agsToExpr zs)
        |AExponent(s,i)::zs -> FMult(FExponent(FVar s, i), agsToExpr zs)
        |[] -> FInt 0

    let rec atomGrpToExpr atGrp =
        match atGrp with
        |x::[] -> agsToExpr x
        |x::xs -> FAdd(agsToExpr x, atomGrpToExpr xs)
        |[] -> FNum 0.0
    atomGrpToExpr ags

//Checks whether expression contains a FDiv.
let rec containsDiv = function
    | FAdd(e1, e2) -> containsDiv e1 || containsDiv e2
    | FMin(e1, e2) -> containsDiv e1 || containsDiv e2
    | FMult(e1, e2) -> containsDiv e1 || containsDiv e2
    | FRoot(e1, _) -> containsDiv e1
    | FExponent(e1, _) -> containsDiv e1
    | FVar _ | FNum _ | FInt _ -> false
    | FDiv(_, _) -> true

//Checks whether expression contains a FDiv.
let rec containsRoot = function
    | FAdd(e1, e2) -> containsRoot e1 || containsRoot e2
    | FMin(e1, e2) -> containsRoot e1 || containsRoot e2
    | FMult(e1, e2) -> containsRoot e1 || containsRoot e2
    | FDiv(e1, e2) -> containsRoot e1 || containsRoot e2
    | FRoot(e1, _) -> true
    | FExponent(e1, _) -> containsRoot e1
    | FVar _ | FNum _ | FInt _ -> false

//Checks whether expression contains a FRoot.
//let rec containsRoot f = function
//    | FAdd(e1, e2) -> containsRoot e1 (fun e1 -> containsRoot e2 (fun e2 -> e2 = FDiv (x,y))
//    | FMin(e1, e2) -> containsRoot e1 (fun e1 -> containsRoot e2 (fun e2 -> e2 = FDiv (x,y))
//    | FDiv(e1, e2) -> containsRoot e1 (fun e1 -> containsRoot e2 (fun e2 -> e2 = FDiv (x,y))
//    | FMult(e1, e2) -> containsRoot e1 (fun e1 -> containsRoot e2 (fun e2 -> e2 = FDiv (x,y))
//    | FExponent(e1, _) -> containsRoot e1 (fun e1 -> containsRoot e1 (fun e -> )
//    | FRoot(e1, _) -> true
//    | FVar _ | FNum _ | FInt _ -> false

let ppAtom = function
  | ANum c -> string(c)
  | AExponent(s,1) -> s
  | AExponent(s,n) -> s+"^"+(string(n))
let ppAtomGroup ag = String.concat "*" (List.map ppAtom ag)
let ppSimpleExpr (SE ags) = String.concat "+" (List.map ppAtomGroup ags)

let rec combine xss = function
  | [] -> []
  | ys::yss -> List.map ((@) ys) xss @ combine xss yss

//let rec getMultipliers (e:expr) c =
//    let checkExponent (ex:expr) =
//        match ex with
//        | FExponent(_, 0) -> c (FInt 1)
//        | FExponent(ex1, 1) -> c ex1
//        | FExponent(ex1, n) -> getMultipliers (FMult(ex1, FExponent(ex1, n-1))) c
//        | _ -> c ex
//    
//    match e with
//    | FAdd(e1, e2) -> getMultipliers e1 (fun leftEx -> getMultipliers e2 (fun rightEx -> [leftEx;rightEx]))
//    | FMin(e1, e2) -> getMultipliers e1 (fun leftEx -> getMultipliers e2 (fun rightEx -> [leftEx;rightEx]))
//    | FDiv(e1, e2) -> getMultipliers e1 (fun leftEx -> getMultipliers e2 (fun rightEx -> [leftEx;rightEx]))
//    | FMult(e1, e2) -> checkExponent e1 @ checkExponent e2
//    | FExponent(e1, 0) -> c (FInt 1)
//    | FExponent(e1, 1) -> c e1
//    | FExponent(e1, n) -> c (FMult(e1, FExponent(e1, n-1)))
//    | FVar s -> c (FVar s)
//    | FNum n -> c (FNum n)
//    | FInt i -> c (FInt i)

let rec getMultipliers (e:expr) (acc: expr list)=
    let checkExponent (ex:expr) =
        match ex with
        | FExponent(_, 0) -> [FInt 1]
        | FExponent(ex1, 1) -> [ex1]
        | FExponent(ex1, n) -> getMultipliers (FMult(ex1, FExponent(ex1, n-1))) acc
        | _ -> [ex] 
    
    match e with
    | FAdd(e1, e2) -> getMultipliers e1 acc @ getMultipliers e2 acc
    | FMin(e1, e2) -> getMultipliers e1 acc @ getMultipliers e2 acc
    | FDiv(e1, e2) -> getMultipliers e1 acc @ getMultipliers e2 acc
    | FMult(e1, e2) -> checkExponent e1 @ checkExponent e2 @ acc
    | FExponent(e1, 0) -> acc
    | FExponent(e1, 1) -> e1 :: acc
    | FExponent(e1, n) -> getMultipliers (FMult(e1, FExponent(e1, n-1))) acc
    | _ -> acc

let rec removeMultiplier (ex:expr) (mul:expr) =
    match ex with
    | FAdd(e1, e2) -> FAdd(removeMultiplier ex e1, removeMultiplier ex e2)
    | FMin(e1, e2) -> FMin(removeMultiplier ex e1, removeMultiplier ex e2)
    | FMult(e1, e2) -> if mul = e1 then e2
                       elif mul = e2 then e1
                       else FMult(removeMultiplier ex e1, removeMultiplier ex e2)
    | FExponent(e1, n) -> if mul = e1 then FExponent(e1, n-1)
                          else FExponent(e1, n)
    //| FRoot(e1, n) -> FRoot(removeMultiplier ex e1, n)
    | FDiv(e1, e2) -> FDiv(removeMultiplier ex e1, removeMultiplier ex e2)
    | FVar s -> FVar s
    | FNum n -> FNum n
    | FInt i -> FInt i

//let rec removeMultiplier (ex:expr) (mul:expr) c =
//    match ex with
//    | FAdd(e1, e2) -> removeMultiplier e1 mul (fun leftEx -> removeMultiplier e2 mul (fun rightEx -> FAdd(leftEx, rightEx)))
//    | FMin(e1, e2) -> removeMultiplier e1 mul (fun leftEx -> removeMultiplier e2 mul (fun rightEx -> FMin(leftEx, rightEx)))
//    | FMult(e1, e2) -> if mul = e1 then c e2
//                       elif mul = e2 then c e1
//                       else removeMultiplier e1 mul (fun leftEx -> removeMultiplier e2 mul (fun rightEx -> FMult(leftEx, rightEx)))
//    | FExponent(e1, n) -> if mul = e1 then c (FExponent(e1, n-1))
//                          else c (FExponent(e1, n))
//    //| FRoot(e1, n) -> FRoot(removeMultiplier ex e1, n)
//    | FDiv(e1, e2) -> removeMultiplier e1 mul (fun leftEx -> removeMultiplier e2 mul (fun rightEx -> FDiv(leftEx, rightEx)))
//    | FVar s -> c (FVar s)
//    | FNum n -> c (FNum n)
//    | FInt i -> c (FInt i)

//True of param e is a member of param l1. else false.
let rec mem (l1: expr list) (e: expr) =
    match l1 with
    | x::xs -> if x = e
               then true
               else mem xs e
    | _ -> false

//Tailrecursive intersection of lists
let rec listIntersect (l1: expr list) (l2: expr list) (acc:expr list) =
    match l1 with
    | x::xs -> match mem l2 x with
               | true -> listIntersect xs l2 (x::acc)
               | false -> listIntersect xs l2 acc
    | [] -> acc

let rec getExprValue = function
    | FNum n -> n
    | FInt i -> float(i)
    | FAdd(e1, e2) -> getExprValue e1 + getExprValue e2
    | FMin(e1, e2) -> getExprValue e1 - getExprValue e2
    | FMult(e1, e2) -> getExprValue e1 * getExprValue e2
    | FDiv(e1,e2) -> getExprValue e1 / getExprValue e2
    | FExponent(e1, n) -> System.Math.Pow(getExprValue e1, float(n))
    | FVar s -> 1.0 

//Removes identical multiplication from division. (4*x*x / 4*x -> x) 
let rec cleanDivMults (div:expr) =
    match div with
    | FDiv(e1, e2) -> let upper = getMultipliers e1 []
                      let lower = getMultipliers e2 []
                      let intersect = listIntersect upper lower []
                      
                      let cleanLower = List.fold (fun acc ex -> (removeMultiplier acc ex)) e2 intersect
                      let cleanUpper = List.fold (fun acc ex -> (removeMultiplier acc ex)) e1 intersect
                      FDiv(cleanUpper, cleanLower)
    | FAdd(e1, e2) -> FAdd(cleanDivMults e1, cleanDivMults e2)
    | FMin(e1, e2) -> FMin(cleanDivMults e1, cleanDivMults e2)
    | FMult(e1, e2) -> FMult(cleanDivMults e1, cleanDivMults e2)
    | FExponent(e1, n) -> FExponent(cleanDivMults e1, n)
    | FRoot(e1, n) -> FRoot(cleanDivMults e1, n)
    | ex -> failwith "not a div"

let rec multDiv (e1:expr) (e2:expr) =
    match e1 with
    | FDiv(ex1, ex2) -> FDiv(FMult(ex1, e2), ex2)
    | FAdd(ex1, ex2) -> FAdd(multDiv ex1 e2, multDiv ex2 e2)
    | FMin(ex1, ex2) -> FMin(multDiv ex1 e2, multDiv ex2 e2)
    | FMult(ex1, ex2) -> match containsDiv ex1 with
                         | true -> multDiv (multDiv ex1 ex2) e2
                         | false -> multDiv (multDiv ex2 ex1) e2
    | FExponent(ex1, 0) -> FInt 1
    | FExponent(ex1, 1) -> ex1
    | FExponent(ex1, n) -> multDiv (FMult(FExponent(ex1, n-1), ex1)) e2
    | _ -> FMult(e1, e2)

let cleanRootExp (e:expr) (n:int) =
    match e with
    | FRoot(e1, n1) when n = n1 -> e1
    | FAdd(e1, e2) -> FAdd(FAdd(FExponent(e1, n), FExponent(e1, n)), FMult(FInt 2, FMult(e1, e2)))
    | FMin(e1, e2) -> FMin(FAdd(FExponent(e1, n), FExponent(e1, n)), FMult(FInt 2, FMult(e1, e2)))
    | FMult(e1, e2) -> FMult(FExponent(e1, n), FExponent(e2, n))
    | ex -> ex

//Takes expr = 0 and isolates division expressions in order to express them
//without using division.
let rec isolateDiv ((leftEx:expr), rightEx:expr) =
    match (leftEx, rightEx) with
    | (FAdd(e1, e2), resEx)     -> match (containsDiv e1, containsDiv e2) with
                                   | (true, false) -> isolateDiv (e1, FMin(resEx, e2))
                                   | (false, true) -> isolateDiv (e2, FMin(resEx, e1))
                                   | (true, true) -> isolateDiv (e1, FMin(resEx, e2))//isolateDiv (FAdd((isolateDiv(e1, resEx)), (isolateDiv(e2, resEx))), resEx)                  
                                   | (false, false) -> failwith "No division found."
    | (FMin(e1, e2), resEx)     -> match (containsDiv e1, containsDiv e2) with
                                   | (true, false) -> isolateDiv (e1, FAdd(resEx, e2))
                                   | (false, true) -> isolateDiv (FMult(FInt -1, e2), FMin(resEx, e1))
                                   | (true, true) -> isolateDiv (e1, FAdd(resEx, e2))//isolateDiv (FMin((isolateDiv(e1, resEx)), (isolateDiv(e2, resEx))), resEx)
                                   | (false, false) -> failwith "No division found."
    | (FMult(e1, e2), resEx)    -> match (containsDiv e1, containsDiv e2) with
                                   | (true, false) -> isolateDiv (e1, FDiv(resEx, e2))//((cleanDivMults (multDiv e1 e2)), resEx)
                                   | (false, true) -> isolateDiv (e2, FDiv(resEx, e1))//((cleanDivMults (multDiv e2 e1)), resEx)
                                   | (true, true) -> isolateDiv (e1, FDiv(resEx, e2))//((cleanDivMults (multDiv e1 e2)), resEx)//isolateDiv (FMult((isolateDiv(e1, resEx)), (isolateDiv(e2, resEx))), resEx)
                                   | (false, false) -> failwith "No division found."
    | (FExponent(e1, n), resEx) -> match (containsDiv e1) with
                                   | true -> isolateDiv (e1, FRoot(resEx, n))
                                   | false -> failwith "No division found."
    | (FRoot(e1, n), resEx)     -> match (containsDiv e1) with
                                   | true -> isolateDiv (e1, FExponent(resEx, n))
                                   | false -> failwith "No division found."
    | (FDiv(e1, e2), resEx)     -> match (containsDiv e1, containsDiv e2) with
                                   | (true, false) -> FMin(FMult(resEx, e2), e1)
                                   | (false, true) -> FMin(FMult(resEx, e2), e1)
                                   | (true, true) -> FMin(FMult(resEx, e2), e1)//isolateDiv (FDiv((isolateDiv(e1, resEx)), (isolateDiv(e2, resEx))), resEx)
                                   | (false, false) -> match cleanDivMults (FDiv(e1, e2)) with
                                                       | FDiv(ex1, ex2) -> match (ex1, ex2) with //supports simple division.
                                                                           |(FInt i, FInt i2) -> FInt (i/i2)
                                                                           |(FInt i, FNum n) -> FNum (float(i)/n)
                                                                           |(FNum n, FInt i) -> FNum (n/float(i))
                                                                           |(FNum n, FNum n2) -> FNum (n/n2)
                                                                           |_ -> FMin(FMult(resEx, ex2), ex1)
                                                       | _ -> failwith "cleanDivMults did not return a div."
    | (_, resEx) -> resEx

//Takes expr = 0 and isolates the root in order to express it without
//using roots.
let rec isolateRoot ((leftEx:expr), rightEx:expr) =
    match (leftEx, rightEx) with
    | (FAdd(e1, e2), resEx)     -> match (containsRoot e1, containsRoot e2) with
                                   | (true, false) -> isolateRoot (e1, FMin(resEx, e2))
                                   | (false, true) -> isolateRoot (e2, FMin(resEx, e1))
                                   | (true, true)  -> isolateRoot (FAdd((isolateRoot(e1, resEx)), (isolateRoot(e2, resEx))), resEx)
                                   | (false, false) -> FMin(FMin(resEx, e1), e2)
    | (FMin(e1, e2), resEx)     -> match (containsRoot e1, containsRoot e2) with
                                   | (true, false) -> isolateRoot (e1, FAdd(resEx, e2))
                                   | (false, true) -> isolateRoot (e2, FMin(resEx, e1))
                                   | (true, true)  -> isolateRoot (FMin((isolateRoot(e1, resEx)), (isolateRoot(e2, resEx))), resEx)
                                   | (false, false) -> FMin(FAdd(resEx, e2), e1)
    | (FMult(e1, e2), resEx)    -> match (containsRoot e1, containsRoot e2) with
                                   | (true, false) -> isolateRoot (e1, FDiv(resEx, e2))
                                   | (false, true) -> isolateRoot (e2, FDiv(resEx, e1))
                                   | (true, true) -> isolateRoot (FMult((isolateRoot(e1, resEx)), (isolateRoot(e2, resEx))), resEx)
                                   | (false, false) -> failwith "No root found"
    | (FExponent(e1, n), resEx) -> match (containsRoot e1) with
                                   | true -> isolateRoot (e1, resEx) //(cleanRootExp e1 n, resEx)
                                   | false -> failwith "No root found"
    | (FRoot(e1, n), resEx)     -> match (containsRoot e1) with
                                   | true -> isolateRoot (e1, FExponent(resEx, n))
                                   | false -> FMin(FExponent(resEx, n), e1)
    | (FDiv(e1, e2), resEx)     -> match (containsRoot e1, containsRoot e2) with
                                   | (true, false) -> isolateRoot (e1, FMult(resEx, e2))
                                   | (false, true) -> isolateRoot (e2, FMult(resEx, FDiv(FInt 1, e1)))
                                   | (true, true) -> isolateRoot (FDiv((isolateRoot(e1, resEx)), (isolateRoot(e2, resEx))), resEx)
                                   | (false, false) -> failwith "No root found"
    | (_, resEx) -> resEx

////Checks whether param e contains a division or root.
////If it does, the expr is rewritten expressing it without using the root or the division.
////Else the param e is returned.
let rec cleanExpr (e:expr) =
    match (containsDiv e, containsRoot e) with
    | (true, false) -> isolateDiv (e, (FInt 0)) |> cleanExpr
    | (false, true) -> isolateRoot (e, (FInt 0)) |> cleanExpr
    | (true, true) -> isolateRoot ((isolateDiv (e, (FInt 0))), FInt 0) |> cleanExpr
    | (false, false) -> e

let rec simplify (ex:expr) =
    match cleanExpr ex with
    | FNum c -> [[ANum c]]
    | FInt c -> [[ANum (float(c))]]
    | FVar s -> [[AExponent(s,1)]]
    | FAdd(e1,e2) -> simplify e1 @ simplify e2
    | FMin(e1,e2) -> simplify e1 @ simplify (FMult(FNum -1.0, e2))
    | FMult(e1,e2) -> (combine (simplify e1) (simplify e2))
    | FExponent(e1, 0) -> [[ANum 1.0]]
    | FExponent(e1, 1) -> simplify e1
    | FExponent(e1, n) -> simplify (FMult(e1,FExponent(e1,n-1)))
    | FRoot(_, _) -> failwith "checkExpr did not catch FRoot."
    | FDiv(_, _) -> failwith "checkExpr did not catch FDiv."

//Returns the degree of param al. Assuming there is only one unknown.
let rec simExDegree (al: atom list) (acc:int)=
    match al with
    | AExponent(s, i)::xs -> simExDegree xs (acc+i)
    | ANum c::xs -> simExDegree xs acc
    | _ -> acc

//Returns the product of the constants of param al.
let rec getSimExCons (al:atom list) (acc:float) =
    match al with
    | AExponent(s, i)::xs -> getSimExCons xs acc
    | ANum c::xs -> getSimExCons xs (acc*c)
    | _ -> acc

//Compares two atom lists, returns 0 if they are same degree and the sum of
//their constans is zero. else return param al2.
let simplifyAgs (al1: atom list) (al2: atom list) =
    match simExDegree al1 0 = simExDegree al2 0 with
    | true  -> match (getSimExCons al1 1.0) + (getSimExCons al2 1.0) with
               | 0.0 -> [ANum 0.0]
               | _   -> al2 //[AExponent("x", (simExDegree al1 0));(ANum ((getSimExCons al1 1.0) + (getSimExCons al2 1.0)))]
    | false -> al2

//Compares param al to the atom lists in ags.
let rec collapseSE (SE ags) (al:atom list) =
    match ags with
    |x::xs -> simplifyAgs al x :: collapseSE (SE(xs)) al
    |_     -> []

//Cleans min of exponents. Ie. 2x - 2x should be 0.
let rec simplifySE (SE ags) (SE ags1) =
    match ags1 with
    |x::xs -> simplifySE (SE(collapseSE (SE(ags)) x)) (SE(xs))
    |_ -> SE (ags)

let simplifyAtomGroup ag = let exponents = List.fold (fun map x -> match x with 
                                                                      |AExponent(s,i) -> let v = Map.tryFind s map
                                                                                         if v <> None then Map.add s (i+v.Value) map 
                                                                                         else Map.add s i map
                                                                      |_ -> map)
                                                                         Map.empty ag
                           let product = List.fold (fun prod x -> match x with 
                                                                  |ANum i -> prod*i
                                                                  | _ -> prod)
                                                                  1.0 ag
                           let list = Map.foldBack (fun key v list -> AExponent(key, v) :: list) exponents []
                           if product <> 1.0 then (ANum product) :: list else if list = [] then [ANum product] else list                    

let simplifySimpleExpr (SE ags) =
  let ags' = List.map simplifyAtomGroup ags
  // Add atom groups with only constants together
  let addedGroups = List.fold (fun sum x -> match x with
                                             |[ANum i] -> sum+i
                                             |_ -> sum)
                                             0.0 ags'
  //Last task is to group similar atomGroups into one group.
  let exponents = List.fold (fun map x -> match x with 
                                          | [ANum i] -> map
                                          | c -> let v = Map.tryFind c map
                                                 if v <> None then Map.add c (v.Value+1.0) map 
                                                 else Map.add c 1.0 map) Map.empty ags'
  let list = Map.foldBack (fun key v l -> if v <> 1.0 then ((ANum v :: key) :: l) else  key :: l) exponents []
  let ags'' = if addedGroups <> 0.0 then [ANum addedGroups] :: list else list
  simplifySE (SE(ags'')) (SE(ags''))
    
let exprToSimpleExpr e = simplifySimpleExpr (SE (simplify e))

type poly = P of Map<int,simpleExpr>

let getPolyMap (p:poly) =
    match p with
    P map -> map

let ppPoly v (P p) =
  let pp (d,ags) =
    let prefix = if d=0 then "" else ppAtom (AExponent(v,d))
    let postfix = if isSimpleExprEmpty ags then "" else "(" + (ppSimpleExpr ags) + ")"
    prefix + postfix
  String.concat "+" (List.map pp (Map.toList p))

(* Collect atom groups into groups with respect to one variable v *)
let splitAG v m = function
  | [] -> m
  | ag ->
    let eqV = function AExponent(v',_) -> v = v' | _ -> false
    let addMap d ag m = match Map.tryFind(d) m with
                        | None -> m.Add(d,SE([ag]))
                        | Some (SE(s)) -> m.Add(d,SE(ag::s))
                         
    match List.tryFind eqV ag with
      | Some (AExponent(_,d)) ->
        let ag' = List.filter (not << eqV) ag
        addMap d ag' m
      | Some _ -> failwith "splitAG: Must never come here! - ANum will not match eqV"
      | None -> addMap 0 ag m

let simpleExprToPoly (SE ags) v =
  P (List.fold (splitAG v) Map.empty ags)

let exprToPoly e v = (exprToSimpleExpr >> simplifySimpleExpr >> simpleExprToPoly) e v

let highestVal (i1:int) (i2:int) =
    match i1 > i2 with
    | true  -> i1
    | false -> i2

let rec getExprDegree (e:expr) (acc:int) =
    match e with
    | FAdd(e1, e2) -> highestVal (getExprDegree e1 acc) (getExprDegree e2 acc)
    | FMin(e1, e2) -> highestVal (getExprDegree e1 acc) (getExprDegree e2 acc)
    | FMult(e1, e2) -> highestVal (getExprDegree e1 acc) (getExprDegree e2 acc)
    | FExponent(e1, n) -> highestVal (getExprDegree e1 acc) n
    | _ -> acc

let simplifyToExpr (e:expr) = (exprToSimpleExpr >> simplifySimpleExpr >> simpleExprToExpr) e

let polyToExpr (polynomial:poly) (s:string) = (ppPoly s polynomial) |> scan |> insertMult |> parse