// Mutable records
;;
type counter = {mutable count : int;
                name : string}

let mkCounter n = {count = 0; name = n}
let incr c = c.count <- c.count+1
let c1 = mkCounter "c1"
let c2 = mkCounter "c2"
incr c1
incr c1
incr c2
incr c2
incr c1
incr c2
incr c1

;;
type counter2 = {count2 : int;
                 name2 : string}

let mkCounter2 n = {count2 = 0; name2 = n}
let incr2 c = {count2 = c.count2+1; name2=c.name2}

let c1 = mkCounter2 "c1"
let c2 = mkCounter2 "c2"
incr2 c1
incr2 c1
incr2 c2
incr2 c2
                                           
let mutable c1 = mkCounter2 "c1"
let mutable c2 = mkCounter2 "c2"
c1 <- incr2 c1;c1
c1 <- incr2 c1;c1
c2 <- incr2 c2;c2
c2 <- incr2 c2;c2
c1 <- incr2 c1;c1
c2 <- incr2 c2;c2
c1 <- incr2 c1;c1

fun () -> 3+4

let idWithPrint i = let _ = printfn "%d"i
                    i
;;
idWithPrint 3
fun () -> (idWithPrint 3) + (idWithPrint 4)
it();;
seq [10; 7; -25]
Seq.item 0 nat

let nat = Seq.initInfinite (fun i -> i)
let nat = Seq.initInfinite idWithPrint
Seq.item 4 nat
 ;;
let even = Seq.filter (fun n -> n%2=0) nat
Seq.item 9 even
Seq.toList (Seq.take 4 even)

let sift a sq = Seq.filter (fun n -> n % a <> 0) sq

let rec sieve sq =
  Seq.delay (fun () -> 
               let p = Seq.item 0 sq
               Seq.append
                (Seq.singleton p)
                (sieve (sift p (Seq.skip 1 sq))))

let primes = sieve (Seq.initInfinite (fun n -> n+2))
let nthPrime n = Seq.item n primes
nthPrime 500

let primesCached = Seq.cache primes
let nthPrime' n = Seq.item n primesCached

#time
nthPrime' 500

nthPrime' 500

let rec sieve sq =
  seq { let p = Seq.item 0 sq
        yield p
        yield! sieve (sift p (Seq.skip 1 sq)) }

open System.IO
let rec allFiles dir =
  seq {yield! Directory.GetFiles dir
       yield! Seq.collect allFiles (Directory.GetDirectories dir)}
Directory.SetCurrentDirectory @"/Users/nh/Dropbox/Documents/Work/ITU/Course/BFNP-F2016/Lectures/Lec08/FSharpHR08"
let files = allFiles "."
Seq.take 10 files

open System.Text.RegularExpressions

let captureSingle (ma:Match) (n:int) =
   ma.Groups.[n].Captures.[0].Value

let rec searchFiles files exts =
  let reExts = List.foldBack (fun ext re -> ext+"|"+re) exts ""
  let re = Regex (@"\G(\S*/)([^/]+)\.(" + reExts + ")$")
  seq {for fn in files do
         let m = re.Match fn
         if m.Success 
         then let path = captureSingle m 1
              let name = captureSingle m 2
              let ext  = captureSingle m 3
              yield (path,name,ext)}
                    
let funFiles = Seq.cache (searchFiles (allFiles ".") ["tex";"pdf";"sty"])
Seq.item 0 funFiles
Seq.take 10 funFiles


                   
