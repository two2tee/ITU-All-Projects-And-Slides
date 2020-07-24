let g xs = List.map (fun x -> x*x+1) xs

let isMember x xs = List.exists (fun y -> y=x) xs
let isMember x = List.exists ((=) x)

let disjoint xs ys =
  List.forall (fun x -> not (List.exists (fun y -> y=x) ys)) xs

let subset xs ys =
  List.forall (fun x -> List.exists (fun y -> x=y) xs) ys

let inter xs ys =
  List.filter (fun x -> isMember x ys) xs

let insert x ys = if isMember x ys then ys else x::ys

let union xs ys =
  List.foldBack (fun x rs -> insert x rs) xs ys    

    
