namespace Camera

module Camera =

type Camera = {position : point; lookat : point; up : vector; zoom : float;
   unitWidth: float; pixelWidth : int; pixelHeight : int}

val mkCamera : position : point -> lookat : point -> up : vector -> zoom : float -> 
    unitWidth : float -> unitHeight : float -> pixelWidth : int -> pixelHeight : int -> Camera

val generateRays : Camera -> ray list


