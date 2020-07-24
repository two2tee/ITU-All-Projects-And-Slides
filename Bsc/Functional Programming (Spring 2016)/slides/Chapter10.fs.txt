// Code from Hansen and Rischel: Functional Programming using F#     16/12 2012
// Chapter 10: Text processing programs
// All programs except those from the keyword-index program 


// From Section 10.2 Capturing data using regular expressions

open System.Text.RegularExpressions;;

// Copied from TextProcessing
let captureSingle (ma:Match) (n:int) =
   ma.Groups.[n].Captures.[0].Value ;;

let captureList (ma:Match) (n:int) = 
  let capt = ma.Groups.[n].Captures
  let m = capt.Count - 1 
  [for i in 0..m -> capt.[i].Value] ;;

let captureCount (ma:Match) (n:int) = 
  ma.Groups.[n].Captures.Count ;;

let captureCountList (ma:Match) = 
  let m = ma.Groups.Count - 1
  [for n in 0..m -> ma.Groups.[n].Captures.Count] ;;



let reg =
    Regex @"\G\s*\042([^\042]+)\042(?:\s+([^\s]+))*\s*$";;

// Needs referencing the TextProcessing Module 
//#r @"./KeywordIndex/TextProcessing.dll";;
open TextProcessing;;
let m = reg.Match "\"Control.Observable Module (F#)\" observer event~observer";;

m.Success;;

captureSingle m 1;;

captureList m 2;;

let tildeReg = Regex @"~";;
let tildeReplace str = tildeReg.Replace(str," ");;

tildeReplace "event~observer";;

let regNest =
    Regex @"\G(\s*([a-zA-Z]+)(?:\s+(\d+))*)*\s*$";;

let regOuter = Regex @"\G(\s*[a-zA-Z]+(?:\s+\d+)*)*\s*$";;

let m1 = regOuter.Match " John 35 2 Sophie 27 Richard 17 89 3 "

captureList m1 1;;

let regPerson1 =
    Regex @"\G\s*([a-zA-Z]+)(?:\s+(\d+))*\s*$"

let extractPersonData subStr =
  let m = regPerson1.Match subStr
  (captureSingle m 1, List.map int (captureList m 2));;


let getData1 str =
  let m = regOuter.Match str
  match (m.Success) with
  | false -> None
  | _     ->
      Some (List.map extractPersonData (captureList m 1));;

getData1 " John 35 2 Sophie 27 Richard 17 89 3 ";;

let regPerson2 =
   Regex @"\G\s*([a-zA-Z]+)(?:\s+(\d+))*\s*";;

let m2 =
  regPerson2.Match
    (" John 35 2 Sophie 27 Richard 17 89 3 ", 11);;

captureSingle m2 1;;

captureList m2 2;;

m2.Length ;;

let rec personDataList str pos top =
  if pos >= top then Some [] 
  else let m = regPerson2.Match(str,pos)
       match m.Success with
       | false -> None
       | true  -> let data = (captureSingle m 1,
                              List.map int (captureList m 2))
                  let newPos = pos + m.Length
                  match (personDataList str newPos top) with
                  | None     -> None
                  | Some lst -> Some (data :: lst);;

let getData2 (s: string) = personDataList s 0 s.Length;;
getData2 " John 35 2 Sophie 27 Richard 17 89 3 ";;


// From Section 10.4 File handling. Save and restore values in files

open TextProcessing;;
let v1 = Map.ofList [("a", [1..3]); ("b", [4..10])];;

saveValue v1 "v1.bin";;

let v2 = [(fun x-> x+3); (fun x -> 2*x*x)];;

saveValue v2 "v2.bin";;
 
let value1:Map<string,int list> = restoreValue "v1.bin";;

let [f;g]: (int->int) list = restoreValue "v2.bin";;

f 7;;

g 2;;


// From Section 10.6 Culture-dependent information. String orderings

open System.Globalization;;
let SpanishArgentina = CultureInfo "es-AR";;

let printCultures () =
  Seq.iter
    (fun (a:CultureInfo) ->
          printf "%-12s %s{\ttbsl}n" a.Name a.DisplayName)
    (CultureInfo.GetCultures(CultureTypes.AllCultures));;


open System.Globalization;;
open TextProcessing;;

let svString = orderString "sv-SE";;

let dkString = orderString "da-DK";;

let enString = orderString "en-US";;

svString "ø" < svString "å";;

dkString "ø" < dkString "å";;

dkString "a" < svString "b";; 

let str = svString "abc";;
string str;;

orderCulture str;;

let enListSort lst =
    List.map string (List.sort (List.map enString lst));;

enListSort ["Ab" ; "ab" ; "AC" ; "ad" ];;

enListSort ["a"; "B"; "3"; "7"; "+"; ";"];;

enListSort ["multicore";"multi-core";"multic";"multi-"];;
