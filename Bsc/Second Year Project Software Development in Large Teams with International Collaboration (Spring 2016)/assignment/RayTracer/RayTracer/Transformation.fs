
namespace RayTracer

(*
    This module is used to perform affine transformations on shapes. 
*)

module Transformation = 
    open RayTracer.SceneAssets
    open System
    type Point = Point.Point
    type Vector = Vector.Vector
    type Shape = SceneAssets.IShape
    type Ray = SceneAssets.Ray
    type matrix = float[,]
    type Transformation = matrix*matrix 

    // The transpose of a matrix swaps all rows with columns and is used to transform the normal 
    let transpose (m:matrix) : matrix =     let arrayOfArrays =[|[| m.[0,0]; m.[0,1]; m.[0,2]; m.[0,3]|];
                                                                [|m.[1,0]; m.[1,1]; m.[1,2]; m.[1,3]|];
                                                                [|m.[2,0]; m.[2,1]; m.[2,2]; m.[2,3]|];
                                                                [|m.[3,0]; m.[3,1]; m.[3,2]; m.[3,3]|]|]
                                            Array2D.init 4 4 (fun i j -> arrayOfArrays.[i].[j]) 


    let init () : matrix =  let arrayOfArrays = [|[| 1.0; 0.0; 0.0; 0.0|];
                                                    [|0.0; 1.0; 0.0; 0.0|];
                                                    [|0.0; 0.0; 1.0; 0.0|];
                                                    [|0.0; 0.0; 0.0; 1.0|]|]
                            Array2D.init 4 4 (fun i j -> arrayOfArrays.[i].[j]) 

    // Translates/moves shape
    let translate (x : float) (y : float) (z : float) : Transformation = 
        let matrix = init()
        matrix.[3,0] <- x
        matrix.[3,1] <- y     
        matrix.[3,2] <- z
        let invmatrix = init()
        invmatrix.[3,0] <- -x
        invmatrix.[3,1] <- -y     
        invmatrix.[3,2] <- -z
        (matrix,invmatrix)

    // The below functions are used to rotate shapes around the (x,y,z) axis using a radian angle 

    // Rotate around the x-axis 
    let rotateX (angle : float) : Transformation = 
        let matrix = init()
        matrix.[1,1] <- Math.Cos(angle)
        matrix.[2,1] <- -Math.Sin(angle)
        matrix.[1,2] <- Math.Sin(angle) 
        matrix.[2,2] <- Math.Cos(angle)
        let invmatrix = init()
        invmatrix.[1,1] <- Math.Cos(angle)
        invmatrix.[2,1] <- Math.Sin(angle)
        invmatrix.[1,2] <- -Math.Sin(angle) 
        invmatrix.[2,2] <- Math.Cos(angle)
        (matrix,invmatrix)

    // Rotate around y-axis 
    let rotateY (angle : float) : Transformation = 
        let matrix = init()
        matrix.[0,0] <- Math.Cos(angle)
        matrix.[2,0] <- Math.Sin(angle)
        matrix.[0,2] <- -Math.Sin(angle) 
        matrix.[2,2] <- Math.Cos(angle)
        let invmatrix = init()
        invmatrix.[0,0] <- Math.Cos(angle)
        invmatrix.[2,0] <- -Math.Sin(angle) 
        invmatrix.[0,2] <- Math.Sin(angle) 
        invmatrix.[2,2] <- Math.Cos(angle)
        (matrix,invmatrix)

    // Rotate around z-axis 
    let rotateZ (angle : float) : Transformation = 
        let matrix = init()
        matrix.[0,0] <- Math.Cos(angle)
        matrix.[1,0] <- -Math.Sin(angle)
        matrix.[0,1] <- Math.Sin(angle)
        matrix.[1,1] <- Math.Cos(angle)
        let invmatrix = init()
        invmatrix.[0,0] <- Math.Cos(angle)
        invmatrix.[1,0] <- Math.Sin(angle) 
        invmatrix.[0,1] <- -Math.Sin(angle) 
        invmatrix.[1,1] <- Math.Cos(angle)
        (matrix,invmatrix)

    // The below functions are used to shear a shape in indicated direction

    // Shear in XY direction (Sxy) 
    let sheareXY (distance : float) : Transformation = 
        let matrix = init()
        matrix.[0,1] <- distance 
        let invMatrix = init()
        invMatrix.[0,1] <- -distance 
        (matrix, invMatrix) 
    
    // Shear in XZ direction (Szx)
    let sheareXZ (distance : float) : Transformation = 
        let matrix = init()
        matrix.[0,2] <- distance
        let invMatrix = init()
        invMatrix.[0,2] <- -distance 
        (matrix, invMatrix) 

    // Shear in YX direction (Syx) 
    let sheareYX (distance : float) : Transformation =
        let matrix = init()
        matrix.[1,0] <- distance
        let invMatrix = init()
        invMatrix.[1,0] <- -distance 
        (matrix, invMatrix) 

    // Shear in YZ direction (Syz)
    let sheareYZ (distance : float) : Transformation =
        let matrix = init()
        matrix.[1,2] <- distance
        let invMatrix = init()
        invMatrix.[1,2] <- -distance 
        (matrix, invMatrix) 

    // Shear in ZX direction (Szx) 
    let sheareZX (distance : float) : Transformation = 
        let matrix = init()
        matrix.[2,0] <- distance
        let invMatrix = init()
        invMatrix.[2,0] <- -distance 
        (matrix, invMatrix) 

    // Shear in ZY direction (Szy)
    let sheareZY (distance : float) : Transformation =
        let matrix = init()
        matrix.[2,1] <- distance
        let invMatrix = init()
        invMatrix.[2,1] <- -distance 
        (matrix, invMatrix)

    // Scale shape in all axes using floating point numbers 
    // Numbers lower than one shrinks a shape and numbers greater than one grows it (e.g. 2.0 is double size)
    let scale (x : float) (y : float) (z : float) : Transformation =
        let matrix = init() // Creates initially empty matrix of dimension 4x4 
        matrix.[0,0] <- x
        matrix.[1,1] <- y
        matrix.[2,2] <- z
        let invmatrix = init() // Creates initially empty matrix of dimension 4x4 
        invmatrix.[0,0] <- 1.0/x
        invmatrix.[1,1] <- 1.0/y
        invmatrix.[2,2] <- 1.0/z
        (matrix, invmatrix)

    // The below functions are used to mirror a shape by inverting the sign of a point

    // Mirror shape around x axis 
    let mirrorX : Transformation =
        let matrix = init()
        matrix.[0,0] <- -1.0 // Invert sign of a (x-axis) to - 1 
        (matrix, matrix) // Mirror is its own inverse

    // Mirror shape around y axis 
    let mirrorY : Transformation =
        let matrix = init()
        matrix.[1,1] <- -1.0 // Invert sign of b (y-axis) to - 1 
        (matrix,matrix) // Mirror is its own inverse

    // Mirror shape around z axis 
    let mirrorZ : Transformation =
        let matrix = init()
        matrix.[2,2] <- -1.0 // // Invert sign of c (z-axis) to - 1 
        (matrix,matrix) // Mirror is its own inverse



    let transformPoint (p : Point.Point) (m:matrix) : Point.Point =  
        let (x,y,z) = Point.getCoord p
        let m2 = [x;y;z;1.0] 
        Point.mkPoint   (m2.[0]*m.[0,0]+m2.[1]*m.[1,0]+m2.[2]*m.[2,0]+m2.[3]*m.[3,0]) 
                        (m2.[0]*m.[0,1]+m2.[1]*m.[1,1]+m2.[2]*m.[2,1]+m2.[3]*m.[3,1]) 
                        (m2.[0]*m.[0,2]+m2.[1]*m.[1,2]+m2.[2]*m.[2,2]+m2.[3]*m.[3,2])

    let transformVector v (m:matrix) : Vector = 
        let (x,y,z) = Vector.getCoord v
        let m2 = [x;y;z;0.0] 
        Vector.mkVector (m2.[0]*m.[0,0]+m2.[1]*m.[1,0]+m2.[2]*m.[2,0]+m2.[3]*m.[3,0]) 
                        (m2.[0]*m.[0,1]+m2.[1]*m.[1,1]+m2.[2]*m.[2,1]+m2.[3]*m.[3,1]) 
                        (m2.[0]*m.[0,2]+m2.[1]*m.[1,2]+m2.[2]*m.[2,2]+m2.[3]*m.[3,2])


    let transformHit hit trans =  
        let (m,inv) = trans
        match hit with
            |(dist,Some hit,Some normal,mat) -> 
                        let normal = Vector.normalise (transformVector normal (m))
                        (dist,Some (transformPoint hit m),Some normal,mat)
            |_ -> hit
    
    let transformBbox (bbox : BoundingBox.BoundingBox) (tr : Transformation) = 
        let higher = bbox.higher
        let lower = bbox.lower
        let minHighFront = Point.mkPoint (Point.getX lower) (Point.getY higher) (Point.getZ higher)
        let maxHighFront = higher
        let minLowFront = Point.mkPoint (Point.getX lower) (Point.getY lower)(Point.getZ higher)
        let maxLowFront = Point.mkPoint (Point.getX higher) (Point.getY lower) (Point.getZ higher)
        let minHighBack = Point.mkPoint (Point.getX lower) (Point.getY higher) (Point.getZ lower)
        let maxHighBack = Point.mkPoint (Point.getX higher) (Point.getY higher) (Point.getZ lower)
        let minLowBack = lower
        let maxLowBack = Point.mkPoint (Point.getX higher) (Point.getY lower) (Point.getZ lower)
        let (matrix, inverse) = tr
        let tMinHighFront = transformPoint minHighFront matrix
        let tMaxHighFront = transformPoint maxHighFront matrix
        let tMinLowFront =  transformPoint minLowFront matrix
        let tMaxLowFront =  transformPoint maxLowFront matrix
        let tMinHighBack =  transformPoint minHighBack matrix
        let tMaxHighBack =  transformPoint maxHighBack matrix
        let tMinLowBack = transformPoint minLowBack matrix
        let tMaxLowBack = transformPoint maxLowBack matrix

        let posInf = (float) Single.PositiveInfinity
        let negInf = (float) Single.NegativeInfinity
        let (min, max) = List.fold (fun (currentMin, currentMax) point -> 
                            ((BoundingBox.findLowestPoint currentMin point), (BoundingBox.findHighestPoint currentMax point))) 
                                    ((Point.mkPoint posInf  posInf posInf),(Point.mkPoint negInf negInf negInf))
                                        [tMinHighFront; tMaxHighFront; tMinLowFront; tMaxLowFront; tMinHighBack; tMaxHighBack; tMinLowBack; tMaxLowBack] 
        SceneAssets.mkBbox min max
 
    let transform (sh : Shape) (tr : Transformation) : Shape = 
        
        { 
            new Shape with
                member this.HitFunc (ray : Ray) = 
                        let p = getOriginPoint ray
                        let v = getDirection ray
                        let (m,inv) = tr
                        transformHit (sh.HitFunc (mkRay (transformPoint p inv) (transformVector v inv))) (m,inv)
                        
                member this.Bbox = transformBbox sh.Bbox tr
                member this.HitBbox ray = BoundingBox.checkBboxIntersection this.Bbox ray
                member this.IsInside p = 
                    let (m,inv) = tr
                    sh.IsInside (transformPoint p inv)
        }

    // Merge two transformations into one transformation
    let mergeMatrices (m1:matrix) (m2:matrix) : matrix = 
                                            let merge (row:int) (column:int):float =
                                                m1.[0,row]*m2.[column,0]+m1.[1,row]*m2.[column,1]+ m1.[2,row]*m2.[column,2]+ m1.[3,row]*m2.[column,3]
                                            let arrayOfArrays = [|[| merge 0 0; merge 1 0; merge 2 0; merge 3 0|];
                                                                [|merge 0 1; merge 1 1; merge 2 1; merge 3 1|];
                                                                [|merge 0 2; merge 1 2; merge 2 2; merge 3 2|];
                                                                [|merge 0 3; merge 1 3; merge 2 3; merge 3 3|]|]
                                            Array2D.init 4 4 (fun i j -> arrayOfArrays.[i].[j]) 
    
    // Merge multiple transformations into one transformation
    let mergeTransformations (transformations: Transformation list) = 
        List.fold (fun ((m1:matrix),(i1:matrix)) ((m2:matrix),(i2:matrix)) ->  (mergeMatrices m2 m1, mergeMatrices i1 i2)) (init(),init()) transformations 
