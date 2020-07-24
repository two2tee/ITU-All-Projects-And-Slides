module ExprToPoly

type expr = ExprParse.expr
val subst: expr -> (string * expr) -> expr

type atom = ANum of float | AExponent of string * int
type atomGroup = atom list  
type simpleExpr = SE of atomGroup list
val ppSimpleExpr: simpleExpr -> string
val exprToSimpleExpr: expr -> simpleExpr
val simplifyAtomGroup: atomGroup -> atomGroup
val simplifySimpleExpr: simpleExpr -> simpleExpr
val simplify: expr -> atom list list
val simpleExprToExpr: simpleExpr -> expr
val simplifyToExpr: expr -> expr
val getExprDegree: expr -> int -> int

type poly = P of Map<int,simpleExpr>
val getPolyMap: poly -> Map<int, simpleExpr>
val ppPoly: string -> poly -> string
val simpleExprToPoly: simpleExpr -> string -> poly
val exprToPoly: expr -> string -> poly
val polyToExpr: poly -> string -> expr
val isolateDiv: expr*expr -> expr
val cleanExpr: expr -> expr
val simplifySE: simpleExpr -> simpleExpr -> simpleExpr
val cleanDivMults: expr -> expr
val getExprValue: expr -> float