(*
   Source: http://fsharpforfunandprofit.com/posts/fsharp-syntax/
   by Scott Wlaschin.

   Examples below are copied from the article
   License: http://creativecommons.org/licenses/by/3.0/
*)

let f =
  let x = 1  // offside line at start of let, column 3
  let y = 1  // must be at column 3
  x+y        // must be at column 3

let f =
  let x = 1  // offside line at column 3
   x+1       // error because x starts at column 4 and not 3

let f =
  let x = 1  // offside line at column 3
 x+1         // error because x start at column 2

let f = let x=1 // new offside line where second let starts
        x+1     // must start at the new offside line

let f = let x=1 // 
       x+1      // error because x+1 is before offside line.

let f = let x=1 // 
         x+1    // error because x+1 is after offside line.

let f =  
  let x=1  // new offside line at start of let, column 3
  x+1      // must start at column 3.

let f = 
  let g = (         
    1+2)    // new offside line where 1 begin
  g         // g is body to let and prev. offside line popped

let f = 
  let g = (         
    1+2)    // new offside line where 1 begin
 g         // error because g before offside line, let

let f = 
  if true then
    1+2     // new offside line at 1
  else
    42      // new offside line at 42

let f = 
  let g = let x = 1 // first word after "let g =" 
                    // defines a new offside line
          x + 1     // "x" must align with recent offside line
                    // pop the offside line stack now
  g + 1             // back to previous offside line

let f = 
  let g=( // let defines a new offside line
 1+2)     // 1+2 can't define new line less than previous let
  g 

let x =  1   // defines a new line at 1
        + 2   // infix operators that start a line don't count
             * 3  // starts with "*" so doesn't need to align
         - 4  // starts with "-" so doesn't need to align

let f = fun x -> // "fun" should define a new line
  let y = 1      // but doesn't. The real line starts 
  x + y          // with let.


#indent "off"

     let f = 
    let x = 1 in
  if x=2 then 
begin "a" end else begin
"b" 
end

let x = let y = 1 in let z = 2 in y + z

#indent "on"
