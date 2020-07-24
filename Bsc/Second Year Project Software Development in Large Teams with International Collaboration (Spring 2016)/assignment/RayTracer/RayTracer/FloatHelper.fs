namespace RayTracer
(*
 The constant in this module is used for computations using float-values that might imply precicion-troubles. 
 We consider values the same if they differ by a small amount instead of exactly zero. 
 The constant in this module will be used to handle floats-near zero when solving quadratic equations.
*)

module FloatHelper =
 
    // Constant
    let ZeroThreshold = 0.0000000001
