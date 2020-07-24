module Vector

type Vector = 
    V of float * float * float
        override v.ToString() =
            match v with
                V(x,y,z) -> "["+x.ToString()+","+y.ToString()+","+z.ToString()+"]"

let mkVector (x:float) (y:float) (z:float) = V(x,y,z)

let zero = V(0.0,0.0,0.0)

let getX (V(x,_,_)) = x

let getY (V(_,y,_)) = y

let getZ (V(_,_,z)) = z

let getCoord (V(x,y,z)) = (x,y,z)

let multScalar (V(x,y,z)) s = V(x*s,y*s,z*s)

let magnitude (V(x,y,z)) = System.Math.Sqrt(x*x + y*y + z*z)

let dotProduct (V(x1,y1,z1)) (V(x2,y2,z2)) = (x1*x2)+(y1*y2)+(z1*z2)

let crossProduct (V(x1,y1,z1)) (V(x2,y2,z2)) = V(y1*z2-z1*y2,z1*x2-x1*z2,x1*y2-y1*x2)

let normalise (V(x,y,z) as v) = let len = (magnitude v) 
                                if(len=0.0) then
                                    V(0.0,0.0,0.0)
                                else
                                    V(x/len, y/len, z/len)

type Vector with
  static member ( ~- ) (V(x,y,z)) = V(-x,-y,-z)
  static member ( + ) (V(ux,uy,uz),V(vx,vy,vz)) = V(ux+vx,uy+vy,uz+vz)
  static member ( - ) (V(ux,uy,uz),V(vx,vy,vz)) = V(ux-vx,uy-vy,uz-vz)
  static member ( - ) (V(x, y, z), s: float) = V(x-s, y-s, z-s)
  static member ( * ) (s:float, v:Vector) = (multScalar v s)
  static member ( * ) (v:Vector,u:Vector) = (dotProduct v u)