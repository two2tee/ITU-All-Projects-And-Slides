module ExprParse

type terminal = Add | Min | Mul | Div | Pwr | Root | Lpar | Rpar | Int of int | Float of float | Var of string
exception Scanerror
val scan: char seq -> terminal list
val insertMult: terminal list -> terminal list
//val arithmetic: terminal list -> float

type expr = FNum of float | FInt of int | FVar of string | FAdd of expr*expr | FMin of expr*expr | FMult of expr*expr | FDiv of expr*expr | FExponent of expr*int | FRoot of expr*int
exception Parseerror
val parse: terminal list -> expr
val parseStr: seq<char> -> expr
val dotAST: expr -> string
