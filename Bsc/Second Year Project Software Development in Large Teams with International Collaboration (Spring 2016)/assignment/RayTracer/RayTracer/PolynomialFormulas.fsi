module PolynomialFormulas

type Poly = ExprToPoly.poly
type Expr = ExprParse.expr

val solveSecondDegree: float -> float -> float -> float option
val solveSecondDegreeP: Poly -> float option
val solveFirstDgr: Expr -> float option
//val solveCubic: Poly -> string -> float option
//val solveQuartic: Poly -> string -> float option
val polyLongDiv: Poly -> Poly -> string -> Expr*Expr
val deriveExpr: Expr -> Expr
val simplifyToExpr: Expr -> Expr
val newtonsMethod: Expr -> Expr -> float -> int -> float option
val sturmSeq: Expr -> (float*float) option
val sturmPolynomials: Expr -> int -> Expr list
//val newtonConverge: float -> float -> Poly -> float option