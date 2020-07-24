namespace TracerTestSuite

open System.IO
open API.API
open System

module Util =
  let degrees_to_radians (d : float) = d * Math.PI / 180.0

  let private source_path = "../../.."
  let private result_path = source_path + "/result"
  let private timings = result_path + "/runtime.txt"

  let mutable private timings_wr = null

  let init () = 
    Directory.CreateDirectory result_path |> ignore
    timings_wr <- new StreamWriter(timings, false)

  let finalize () = timings_wr.Close()

  let render s (toFile : (string*string) option) =
    match toFile with
    | Some (f, fn) -> 
      let path = if f = "" then result_path else result_path + "/" + f
      Directory.CreateDirectory path |> ignore
      let stopWatch = System.Diagnostics.Stopwatch.StartNew()
      renderToFile s (path + "/" + fn)
      stopWatch.Stop();
      timings_wr.WriteLine("{0}/{1} rendered in {2} seconds", f, fn, stopWatch.Elapsed.TotalMilliseconds / 1000.0)
    | None -> renderToScreen s
   


  let render' (s : scene) (toFile : (string*string)) (toScreen : bool) = 
    if toScreen
    then render s None
    else render s (Some toFile)
  