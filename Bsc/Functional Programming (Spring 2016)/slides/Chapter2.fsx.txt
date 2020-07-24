[1;2] @ [3;4];;

let f x = 
  let y = x + 3
  if y = 2
  then x
  else y;;

let f x y : int = x + y;;

(float) 2;;

let a = 2;;

let f x y = x + y + a;;
let g = f 2;;
g 3;;

let a = 42;;
let g = f 2;;
g 3;;

let g x =
  let a = 6
  let f y = y + a
  x + f x;;
g 1;;
  
fun x y -> x+x*y;;

let myFun f g x = f(g x)
let f (x:int) = x+1
let myFun2 g x = myFun f

let x = (2, "Hej med dig", 4.3, (2,2));;

let addMult x y = x + x*y;;
let f = addMult 2;;
f 3;;

// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 2: Values, operators, expressions and functions. 

let even n = n % 2 = 0;; 

let isLowerCaseVowel ch =
    ch='a' || ch='e' || ch='i' || ch='o' || ch='u';;

let isLowerCaseConsonant ch =
    System.Char.IsLower ch && not (isLowerCaseVowel ch);;

isLowerCaseVowel 'i' && not (isLowerCaseConsonant 'i');;

isLowerCaseVowel 'I' || isLowerCaseConsonant 'I';;

not (isLowerCaseVowel 'z') && isLowerCaseConsonant 'z';;     

let nameAge(name,age) =
    name + " is " + (string age) + " years old";;
    
let adjString s = if even(String.length s)
                  then s else " " + s;;
                  
let square x = x * x;;

let rec power = function
    | (_, 0) -> 1.0                (* 1 *)
    | (x, n) -> x * power(x,n-1)   (* 2 *);;

let rec power a =
  match a with
  | (_,0) -> 1.0
  | (x,n) -> x * power(x,n-1);;

let rec power(x,n) =
  match n with
  | 0 -> 1.0
  | n' -> x * power(x,n'-1);;
                  
let plusThree = (+) 3;;

let f = fun y -> y+3;;        // f(y) = y+3

let g = fun x -> x*x;;        // g(x) = x*x

let h = f << g;;              // h = (f o g)

((fun y -> y+3) << (fun x -> x*x)) 4;;

let weight ro = fun s -> ro * s ** 3.0;;

let waterWeight = weight 1000.0;;

let methanolWeight = weight 786.5;;

let weight1 ro s = ro * s ** 3.0;;

let (.||.) p q = (p || q) && not(p && q);;

let (~%%) x = 1.0 / x;;

let eqText x y =
    if x = y then "equal" else "not equal";;

let ordText x y = if x > y then "greater"
                  else if x = y then "equal"
                  else "less";;

let ordText1 x y = match compare x y with
                   | t when t > 0 -> "greater"
                   | 0            -> "equal"
                   | _            -> "less";;




let (.<|.) f a = f a
let (.|>.) a f = f a
let (it = System.Math.Sin <| 2.0
let it = 2.0 |> System.Math.Sin
(<|)
(<<)
