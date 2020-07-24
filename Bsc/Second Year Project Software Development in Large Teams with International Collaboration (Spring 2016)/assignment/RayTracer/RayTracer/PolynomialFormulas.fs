(*
This module concerns itself with solving polynomials of different degrees.

Helpful links:

The cubic formula (3rd)   -> https://www.youtube.com/watch?v=wRkXP3eRiy8
The quartic formula (4th) -> https://www.youtube.com/watch?v=3lYTBIEgyaM
*)
module PolynomialFormulas
    
    open ExprToPoly
    open ExprParse
    open RayTracer.FloatHelper
    type Poly = ExprToPoly.poly
    type Expr = ExprParse.expr
    type Atom = ExprToPoly.atom
    type AtomGroup = ExprToPoly.atomGroup
    type SimpleExpr = ExprToPoly.simpleExpr

    //converts atom list list to an atom group list
    let rec atomListListToAtomGrp (list : atom list list) =
        match list with
        |x::xs -> (x:AtomGroup) :: atomListListToAtomGrp xs
        |[] -> []

    //Converts atom group list to type poly
    let atomGrpListToPoly (atomGrp:AtomGroup list) (s:string) = simpleExprToPoly (SE atomGrp) s

    //Converts an atom list list to type poly
    let atLLToPoly (list : atom list list) (s:string) = (atomListListToAtomGrp >> atomGrpListToPoly) list s

    //Converts type poly to expression in given string
    let polyToExpr (polynomial:Poly) (s:string) = (ppPoly s polynomial) |> scan |> insertMult |> parse
    
    let getPolyMap (p:poly) =
        match p with
        P map -> map

    //Converts poly to a map<int, expr> with variables in it.
    let polyToExprMap (p:poly) (var:string) =
        let map = getPolyMap p
        Map.fold (fun state key value -> Map.add key (FMult(simpleExprToExpr value, FExponent(FVar var, key))) state) Map.empty map
    
    let ExprMapToExpr (map:Map<int, Expr>) =
        Map.fold (fun state key value -> FAdd(state, value)) (FInt 0) map

    let simplifyToExpr (e:Expr) = (exprToSimpleExpr >> simplifySimpleExpr >> simpleExprToExpr) e

    
    let polyLongDiv (p1:Poly) (p2:Poly) (variable:string) =
        let expr2 = polyToExpr p2 variable
        let expr2Dgr = (getExprDegree (simplifyToExpr expr2) 0)
        let sndLeadMap = polyToExprMap p2 variable

        let rec divide q expr1 =
            let expr1Dgr = (getExprDegree expr1) 0
            
            match expr1Dgr >= expr2Dgr with
            | true -> let leadMap = polyToExprMap (exprToPoly expr1 variable) variable //Creates leadmap (r)
                      //t = lead(r)/lead(d)
                      let leadR = (Map.find expr1Dgr leadMap)
                      let leadD = (Map.find expr2Dgr sndLeadMap)
                      let t = FDiv(leadR, leadD)
                      
                      //r = r - (t*d)
                      let newExpr1 = simplifyToExpr (FMin(expr1, FMult(t, expr2)))

                      //q = q+t
                      let newq = simplifyToExpr (FAdd(q, t))
                      divide newq newExpr1
            | false -> (q, expr1)
        divide (FNum 0.0) ((polyToExprMap p1 variable) |> ExprMapToExpr)

    let rec containsVar = function
        | FAdd(e1, e2) -> containsVar e1 || containsVar e2
        | FMin(e1, e2) -> containsVar e1 || containsVar e2
        | FMult(e1, e2) -> containsVar e1 || containsVar e2
        | FDiv(e1, e2) -> containsVar e1 || containsVar e2
        | FRoot(e1, _) -> containsVar e1
        | FExponent(e1, _) -> containsVar e1
        | FVar s -> true
        | _ -> false

    let rec deriveExpr (e:expr) =
        match e with
        |FExponent(e1, 0) -> FInt 1
        |FExponent(e1, 1) -> e1
        |FExponent(FVar s, i) -> FMult(FInt i, FExponent(FVar s, i-1))
        |FAdd(e1, e2) -> match (containsVar e1, containsVar e2) with
                         | (true, true) -> FAdd(deriveExpr e1, deriveExpr e2)
                         | (true, false) -> FAdd(deriveExpr e1, FInt 0)
                         | (false, true) -> FAdd(FInt 0, deriveExpr e2)
                         | (false, false) -> FInt 0
        |FMin(e1, e2) -> match (containsVar e1, containsVar e2) with
                         | (true, true) -> FMin(deriveExpr e1, deriveExpr e2)
                         | (true, false) -> FMin(deriveExpr e1, FInt 0)
                         | (false, true) -> FMin(FInt 0, deriveExpr e2)
                         | (false, false) -> FInt 0
        |FMult(e1, e2) -> match (containsVar e1, containsVar e2) with
                          | (true, true) -> FMult(deriveExpr e1, deriveExpr e2)
                          | (true, false) -> FMult(deriveExpr e1, e2)
                          | (false, true) -> FMult(e1, deriveExpr e2)
                          | (false, false) -> FMult(e1, e2)
        |FDiv(e1, e2) -> FDiv(FMin(FMult(e2, deriveExpr e1), FMult(e1, deriveExpr e2)), FExponent(e2, 2)) //http://archives.math.utk.edu/visual.calculus/2/quotient_rule.4/
        |FVar s -> FInt 1
        |FNum f -> FNum f
        |FInt i -> FInt i

    //Solves a second degree polynomial given a, b and c.
    //Only returns smallest root
    // Todo replace with separate fs/fsi for solving second degree polynomials 
    let solveSecondDegree a b c =
        let disc = b*b - (4.0*a*c)
        if disc < 0.0
        then None
        else if disc = 0.0
        then Some ((-1.0*b) / 2.0*a)
        else
            let dist1 = (-1.0*b + System.Math.Sqrt(disc))/(2.0*a)
            let dist2 = (-1.0*b - System.Math.Sqrt(disc))/(2.0*a)
            if(dist1>ZeroThreshold && dist2>ZeroThreshold) then
                if dist1 < dist2 then Some dist1
                else
                    Some dist2
            else if(dist1>ZeroThreshold) then
                Some dist1
            else if (dist2>ZeroThreshold) then
                Some dist2
            else
                None
    
    
    let solveSecondDegreeP (p:Poly) =
        let map = getPolyMap p
        match map.Count - 1 with
        | 2 -> let exValues = Map.foldBack (fun key value state -> Array.append [|(value |> simpleExprToExpr |> getExprValue)|] state) map [| |]
               solveSecondDegree exValues.[2] exValues.[1] exValues.[0]
        | _ -> None 

    let solveFirstDgr (ex: expr) =
         let rec isolateVar (left : expr) (right:expr) =
             match left with
             | FAdd(e1, e2) -> match (containsVar e1, containsVar e2) with
                               | (true, false) -> isolateVar e1 (FAdd(right, (FMult(FInt -1, e2))))
                               | (false, true) -> isolateVar e2 (FAdd(right, (FMult(FInt -1, e1))))
                               | (false, false) -> failwith "No var found."
             | FMin(e1, e2) -> match (containsVar e1, containsVar e2) with
                               | (true, false) -> isolateVar e1 (FAdd(right, e2))
                               | (false, true) -> isolateVar e2 (FMin (right, e1))
                               | (false, false) -> failwith "No var found."
             | FMult(e1, e2) ->match (containsVar e1, containsVar e2) with
                               | (true, false) -> isolateVar e1 (FDiv(right, e2))
                               | (false, true) -> isolateVar e2 (FDiv(right, e1))
                               | (false, false) -> failwith "No var found."
             | FDiv(e1, e2) -> match (containsVar e1, containsVar e2) with
                               | (true, false) -> isolateVar e1 (FMult(right, e2))
                               | (false, true) -> isolateVar e2 (FMult(right, e1))
                               | (false, false) -> failwith "No var found."
             | FExponent(e1, 0) -> failwith "Var to the pwr of 0."
             | FExponent(e1, 1) -> isolateVar e1 right
             | FVar s -> right
             | _ -> failwith "no var found."
         let dist = getExprValue (isolateVar ex (FInt 0))
         if dist >= 0.0 then Some dist
         else None

    //given a poly and its degree
    let rec sturmPolynomials (p0:expr) (dgr:int) =
        
        let p1 = deriveExpr p0
        let rec findSturmSeq (px0:expr) (px1:expr) (dgr1:int) (acc:expr list) =
           match dgr >= 0 with
           | true -> let px = FMult(FInt -1, (snd (polyLongDiv (exprToPoly px0 "t") (exprToPoly px1 "t") "t")))
                     findSturmSeq px1 px (dgr1-1) (px::acc)
           | false -> acc
        findSturmSeq p0 p1 (dgr-2) [p1;p0]
    
    let signSwitch (v1:expr) (v2:expr) =
        match (getExprValue v1, getExprValue v2) with
        | (x, y) when x > 0.0 && y < 0.0 -> true
        | (x, y) when y > 0.0 && x < 0.0 -> true
        | (0.0, y) when y < 0.0 -> true
        | (x, 0.0) when x < 0.0 -> true
        | _ -> false

    let rec signSwitchSum (el : expr list) (acc:int) =
        match el with
        |x::x1::xs -> if signSwitch x x1 
                      then signSwitchSum (x1::xs) (acc+1)
                      else signSwitchSum (x1::xs) acc
        |x::xs -> if signSwitch x (el |> Seq.skip (el.Length - 1) |> Seq.head)
                  then acc + 1
                  else acc
        |[] -> acc
    
    let rec sturmSeq (p:expr) =
        let sturm = sturmPolynomials p (getExprDegree p 0)
        
        let rec partition (l:expr list) (floor:float) (range:float) (i:int) =
            if i >= 7 then Some (floor, range)
            else
                let a = List.fold (fun s x -> ((subst x ("t", FNum floor))) :: s) [] l
                let b = List.fold (fun s x -> ((subst x ("t", FNum range))) :: s) [] l
                let roots = (signSwitchSum a 0) - (signSwitchSum b 0)
                if roots <= 0 then None
                else
                match partition l floor (range - ((range-floor)/2.0)) (i+1) with //Look in left interval
                | Some l1 -> Some l1//Look onwards in left 
                | None -> partition l (floor + (range - floor)/2.0) range (i+1)//Look in right interval
        partition sturm 0.0 100.0 0
            
    
    let rec newtonsMethod (p:Expr) (pd:Expr) (x0:float) (i:int) =
        //Max 20 recursive calls.
        if i >= 5 then Some x0
        else
            //Insert x0 into y and y'
            let y = subst p ("t", FNum x0) |> simplifyToExpr |> getExprValue
            let y' = subst pd ("t", FNum x0) |> simplifyToExpr |> getExprValue
            newtonsMethod p  pd (x0 - (y/y')) (i+1)