module Point
open Vector;
open System;
type Vector = Vector.Vector
type Point =
  | P of float * float * float
  override p.ToString() =
    match p with
      P(x,y,z) -> "("+x.ToString()+","+y.ToString()+","+z.ToString()+")"


let zero = P(0.0,0.0,0.0)
let mkPoint x y z = P(x,y,z)
let getX (P(x,_,_)) = x
let getY (P(_,y,_)) = y
let getZ (P(_,_,z)) = z
let getCoord (P(x,y,z)) = (x,y,z)
let move (P(x,y,z)) (v:Vector) = P((x+Vector.getX(v)),(y+Vector.getY(v)),(z+Vector.getZ(v))) 


let distance (P(px,py,pz)) (P(qx,qy,qz)) = (Vector.mkVector (qx-px) (qy-py) (qz-pz))
let direction p q = Vector.normalise (distance p q)