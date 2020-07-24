namespace RayTracer

// Implementation file of shape signature module

open BoundingBox // Used to compute bounding box in shapes 
open SceneAssets
open System
open PolynomialFormulas
open Transformation
module Shape = 
    type IShape = SceneAssets.IShape
    type Point = Point.Point
    type Expr = ExprParse.expr
    type Vector = Vector.Vector
    type BoundingBox = BoundingBox.BoundingBox
    type Texture = SceneAssets.Texture
    type Ray = SceneAssets.Ray
    type Material = SceneAssets.Material
    type hitBboxFunc = (Ray -> bool)
    type Vertex = SceneAssets.Vertex


    let getNormal (n:Vector) (r:Ray) =
        let v = Vector.normalise r.direction
        let angle = System.Math.Acos ((Vector.dotProduct n v)/(Vector.magnitude n * Vector.magnitude v))
        if(angle>Math.PI/2.0) 
        then 
            n
        else
            -n

    let getNearestHit (shapes: IShape list) ray =
        let possibleHits = List.map(fun (s:IShape) ->  s.HitFunc ray) shapes //make the hitFunc for all shapes
        let filteredHits = List.filter(fun ((d: float option),_,_,_) -> d.IsSome ) possibleHits
        let sortedHits = List.sortBy(fun ((d: float option),_,_,_) -> d.Value ) filteredHits // sort in ascending order
        match sortedHits with
            |(Some dist,Some hit,Some normal,mat)::_ when dist>FloatHelper.ZeroThreshold ->
                            (Some dist,Some hit,Some normal,mat) //return first hit if any
            |_ ->  (None, None, None,SceneAssets.black)


    // Shapes and functions used to create them  

    let Sphere(center : Point, radius : float, texture : Texture, (hitBbox : Ray -> bool), bbox : BoundingBox) = 
        {
            new IShape with 
                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = hitBbox ray
                member this.IsInside p = (Vector.magnitude (Point.distance p center))<=radius
                member this.HitFunc (ray : Ray) = 
                    let p = center
                    let r = radius
                    //help variables for simplicity
                    let px = Point.getX(p) - Point.getX(ray.origin)
                    let py = Point.getY(p) - Point.getY(ray.origin)
                    let pz = Point.getZ(p) - Point.getZ(ray.origin)
                    let dir = Vector.normalise ray.direction
        
                    let b = -2.0*(px*Vector.getX(dir) + 
                                    py*Vector.getY(dir) + 
                                    pz*Vector.getZ(dir))
                    let c = px*px + py*py + pz*pz - r*r // x^2+y^2+z^2=r^2 equation for a sphere

                    let dist = solveSecondDegree 1.0 b c // a is always 1

                    //Check whether the distance is a valid value
                    if(dist.IsSome) then
                        let hitPoint = Point.move ray.origin (dist.Value*dir)
                        let normal =  Vector.mkVector (2.0 * (Point.getX hitPoint)) (2.0 * (Point.getY hitPoint)) (2.0 * (Point.getZ hitPoint))
                        let matFun = (fun () -> 
                            let (hitX,hitY,hitZ) = Point.getCoord hitPoint
                            let pi = System.Math.PI
                            let nx = hitX /r
                            let ny = hitY /r
                            let nz = hitZ /r
                            let theta = acos ny
                            let phi' = atan2 nx nz
                            let phi =   
                                if phi' < 0.0
                                then 
                                    phi'+2.0*pi
                                else 
                                    phi'
                            getTextureColour texture (phi/(2.0*pi),1.0-theta/pi))
                        (dist,Some(hitPoint), Some (getNormal normal ray),matFun)
                        
                    else
                            (None, None, None,(fun () -> getTextureColour texture (0.0,0.0)))


            }

    let mkSphere (center : Point) (radius : float) (tex : Texture) = 
        let bbox = BoundingBox.computeSphereBbox radius
        let hitBb = BoundingBox.checkBboxIntersection bbox
        let (x,y,z) = Point.getCoord center
        transform (Sphere(Point.mkPoint 0.0 0.0 0.0,radius,tex, hitBb, bbox)) (translate x y z)


    let Triangle(A : Point,B : Point,C : Point,texture : Texture, (hitBbox : Ray -> bool), bbox : BoundingBox) =
        {
            new IShape with
                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = hitBbox ray
                member this.HitFunc (ray : Ray) =

                    // Triangle sides (u and v) and normal vector 
                    let us = Point.distance B A 
                    let vs = Point.distance C A 
                    let normal = Vector.normalise(Vector.crossProduct us vs)
                    let dir = Vector.normalise ray.direction
                    //Solving equation with 3 unknowns based on cramer's rule
                    
                    // β (ax −bx)+γ(ax −cx)+tdx =ax −ox
                    //First equation with unknown, ax + by + cz = d
                    let a = (Point.getX A) - (Point.getX B) 
                    let b = (Point.getX A) - (Point.getX C)
                    let c = (Vector.getX dir)
                    let d = (Point.getX A) - (Point.getX ray.origin)

                    // β (ay − by)+γ(ay −cy)+tdy =ay −oy
                    //Second equation with unknown, ez + fy + gz = h
                    let e = (Point.getY A) - (Point.getY B)
                    let f = (Point.getY A) - (Point.getY C)
                    let g = (Vector.getY dir)
                    let h = (Point.getY A) - (Point.getY ray.origin)

                    // β (az −bz)+γ(az −cz)+tdz =az −oz
                    //Third equation with unknown, ix + jy + kz = l
                    let i = (Point.getZ A) - (Point.getZ B)
                    let j = (Point.getZ A) - (Point.getZ C)
                    let k = (Vector.getZ dir)
                    let l = (Point.getZ A) - (Point.getZ ray.origin)

                    //Finding the unknowns and calculates if there is any hit based previous calculations
                    let D = a*(f*k - g*j) + b*(g*i-e*k) + c*(e*j - f*i) 
                    let texture = getTextureColour texture (0.0,0.0) // Calculate material 

                    if(D <> 0.0) then 
                        let beta = (d*(f*k - g*j) + b*(g*l - h*k) + c*(h*j - f*l))/D //Corresponds to x
                        let gamma = (a*(h*k-g*l) + d*(g*i-e*k) + c*(e*l-h*i))/D //Corresponds to y
                        let t = (a*(f*l - h*j) + b*(h*i - e*l) + d*(e*j-f*i))/D //Corresponds to z
                        let alpha = 1.0 - beta - gamma
                        
                        if beta >= 0.0 && gamma >= 0.0 && gamma + beta <= 1.0 // Need sum to be 1 and in between 0 and 1  
                         then
                           let distance = Some t 
                           let hitPoint = Some(Point.move ray.origin (t*dir) )
                           let normal = Some (normal)
                           // Returns distance to hit point, normal of hit point and material of hit point 
                           (distance, hitPoint, normal, fun () -> texture)
                         else (None, None, None, fun() -> texture) 

                    else (None, None, None,(fun () -> texture)) // No division by zero, thus no hit                      

               
                member this.IsInside p = failwith "Shape is not solid!"
        }


    let mkTriangle (a : Point) (b : Point) (c : Point) (tex : Texture) = 
        let bbox = BoundingBox.computeTriangleBbox a b c
        let hitBb = BoundingBox.checkBboxIntersection (bbox)
        Triangle(a,b,c, tex, hitBb, bbox) 

    let meshTriangle (cornerA, cornerB, cornerC, (tex : Texture), (hitBbox : Ray -> bool), (bbox : BoundingBox), (isSmoothShade : bool)) =
        {
            new IShape with
                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = hitBbox ray
                member this.HitFunc (ray : Ray) =

                    // Triangle sides (u and v) and normal vector 
                    let us = Point.distance cornerB.point cornerA.point 
                    let vs = Point.distance cornerC.point cornerA.point 
                    let aNorm = cornerA.normal
                    let bNorm = cornerB.normal
                    let cNorm = cornerC.normal
                    let dir = Vector.normalise ray.direction
                    //Solving equation with 3 unknowns based on cramer's rule
                    
                    // β (ax −bx)+γ(ax −cx)+tdx =ax −ox
                    //First equation with unknown, ax + by + cz = d
                    let a = (Point.getX cornerA.point) - (Point.getX cornerB.point) 
                    let b = (Point.getX cornerA.point) - (Point.getX cornerC.point)
                    let c = (Vector.getX dir)
                    let d = (Point.getX cornerA.point) - (Point.getX ray.origin)

                    // β (ay − by)+γ(ay −cy)+tdy =ay −oy
                    //Second equation with unknown, ez + fy + gz = h
                    let e = (Point.getY cornerA.point) - (Point.getY cornerB.point)
                    let f = (Point.getY cornerA.point) - (Point.getY cornerC.point)
                    let g = (Vector.getY dir)
                    let h = (Point.getY cornerA.point) - (Point.getY ray.origin)

                    // β (az −bz)+γ(az −cz)+tdz =az −oz
                    //Third equation with unknown, ix + jy + kz = l
                    let i = (Point.getZ cornerA.point) - (Point.getZ cornerB.point)
                    let j = (Point.getZ cornerA.point) - (Point.getZ cornerC.point)
                    let k = (Vector.getZ dir)
                    let l = (Point.getZ cornerA.point) - (Point.getZ ray.origin)

                    //Finding the unknowns and calculates if there is any hit based previous calculations
                    let D = a*(f*k - g*j) + b*(g*i-e*k) + c*(e*j - f*i) 
                    
                    if(D <> 0.0) then 
                        let beta = (d*(f*k - g*j) + b*(g*l - h*k) + c*(h*j - f*l))/D //Corresponds to x
                        let gamma = (a*(h*k-g*l) + d*(g*i-e*k) + c*(e*l-h*i))/D //Corresponds to y
                        let t = (a*(f*l - h*j) + b*(h*i - e*l) + d*(e*j-f*i))/D //Corresponds to z
                        let alpha = 1.0 - beta - gamma
                        
                        if (beta >= 0.0) && (gamma >= 0.0) && ((gamma + beta) <= 1.0) // Need sum to be 1 and in between 0 and 1  
                         then
                           let distance = Some t 
                           let hitPoint = Some(Point.move ray.origin (t*dir))
                           let normal = if isSmoothShade then
                                            let norm = alpha*aNorm + beta*bNorm + gamma*cNorm
                                            Some (getNormal norm ray)
                                        else 
                                            Some(getNormal (Vector.normalise(Vector.crossProduct us vs)) ray)
                           // Returns distance to hit point, normal of hit point and material of hit point 
                           (distance, hitPoint, normal, 
                            fun () -> 
                               let v = alpha * cornerA.v + beta * cornerB.v + gamma * cornerC.v
                               let u = alpha * cornerA.u + beta * cornerB.u + gamma * cornerC.u
                               getTextureColour tex (u,v))
                         else (None, None, None, SceneAssets.black) 

                    else (None, None, None, SceneAssets.black) // No division by zero, thus no hit                      

                member this.IsInside p = failwith "Shape is not solid!"
        }

    let mkMeshTriangle (cornerA:Vertex) (cornerB:Vertex) (cornerC:Vertex) (tex:Texture) (isSmoothShade : bool) = 
        let bbox = BoundingBox.computeTriangleBbox cornerA.point cornerB.point cornerC.point 
        let hitBb = BoundingBox.checkBboxIntersection (bbox)
        meshTriangle(cornerA,cornerB,cornerC, tex, hitBb, bbox, isSmoothShade) 
    

    let Plane(point : Point, normalVector : Vector, texture : Texture, (hitBbox : Ray -> bool*float*float), bbox : BoundingBox) =
        {
            new IShape with
                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = match hitBbox ray with (isHit,_,_) -> isHit
                member this.HitFunc (ray:Ray) = 
                    // point is the direction of axis z we move in and u = Px and v = Py (u,v) coordinates 
                    let dir = ray.direction
                    let pz = Point.getZ ray.origin
                    let vz = Vector.getZ dir
                    let dist = -pz/vz;
                    if(dist>=0.0) 
                    then
                        let hitPoint = Point.move ray.origin (dist * dir) // Scalar used to get hit point with direction vector from origin to plane 
                        (Some dist, Some hitPoint, Some (getNormal normalVector ray),
                                (fun () ->  
                                    let (hitX,hitY,hitZ) = Point.getCoord hitPoint
                                    let pi = System.Math.PI
                                    
                                    let u = 
                                        let u = (hitX - Point.getX point) % 1.0
                                        if(u<0.0) then
                                            u+1.0
                                        else
                                            u
                                    let v = 
                                        let v = (hitY - Point.getY point) % 1.0
                                        if(v<0.0) then
                                            v+1.0
                                        else
                                            v
                                    getTextureColour texture (u,v)))  
                    else 
                        (None, None, None, SceneAssets.black) 
            
                member this.IsInside p = failwith "Shape is not solid!"
    }
     
    let mkPlane (t : Texture) =
        let bbox = SceneAssets.mkBbox (Point.mkPoint 0.0 0.0 0.0) (Point.mkPoint 0.0 0.0 0.0)
        let hitBb = fun (r : Ray) -> (true,0.0,0.0)
        Plane((Point.mkPoint 0.0 0.0 0.0), (Vector.mkVector 0.0 0.0 1.0), t, hitBb, bbox) 

    let Disc(radius : float, texture : Texture, (hitBbox : Ray -> bool), bbox) =

        let plane = mkPlane texture
        let center = Point.mkPoint 0.0 0.0 0.0
        { 
           new IShape with
               member this.Bbox = bbox
               member this.HitBbox (ray : Ray) = hitBbox ray 
               member this.HitFunc (ray:Ray) = 
                    // Check for intersection between ray and plane 
                    let hitPoint = plane.HitFunc ray                 
                    let dist, hit, normal,_ = hitPoint 
                    if  hit.IsSome then
                        let distance = Vector.magnitude (Point.distance center hit.Value) // Distance between center of disc and hit point 
                        // Check if hit disc within plane 
                        if distance <= radius then 
                            (dist, hit, normal,
                                (fun () -> 
                                    let (hitX,hitY,hitZ) = Point.getCoord hit.Value
                                    let pi = System.Math.PI
                                    let u = (hitX+radius)/(2.0*radius)
                                    let v = (hitY+radius)/(2.0*radius)
                                    getTextureColour texture (u,v)))
                        else 
                            (None, None, None,SceneAssets.black)
                    else 
                        (None, None, None,SceneAssets.black)

               member this.IsInside p = failwith "Shape is not solid!"
        }

    let mkDisc (center : Point) (radius : float) (tex : Texture) = 
        let bbox = BoundingBox.computeDiscBbox radius
        let hitBb = BoundingBox.checkBboxIntersection bbox
        let x,y,z = Point.getCoord center
        transform (Disc(radius, tex, hitBb, bbox)) (translate x y z)

    let Cylinder(radius : float, height : float, texture : Texture, (hitBbox : Ray -> bool), bbox : BoundingBox) = 
        {
            new IShape with
                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = hitBbox ray
                member this.HitFunc (ray : Ray) = 
                        // Infinite cylinder formula: px^2+pz^2 - r^2 = 0 
                        // Ray hit point formula: p + td with hit point p, direction vector d and distance t 
                        let dir = Vector.normalise ray.direction
                        let checkHitPoint hitPoint = 
                            let hitPy = Point.getY hitPoint 
                            //Check for hit within the height of cylinder

                            if hitPy <= height/2.0 && hitPy >= -height/2.0 then 
                                let normalX = (Point.getX hitPoint) / radius
                                let normalY = 0.0
                                let normalZ = (Point.getZ hitPoint) / radius
                                (Some(Vector.magnitude(Point.distance(ray.origin) (hitPoint))), Some hitPoint, Some (getNormal (Vector.mkVector normalX normalY normalZ) ray), //p/r -> normal TODO:// zerothreshhold on normal vector
                                    (fun () ->  
                                        let (hitX,hitY,hitZ) = Point.getCoord hitPoint
                                        let pi = System.Math.PI
                                        let nx = hitX /radius
                                        let ny = hitY /radius
                                        let nz = hitZ /radius
                                        let phi' = atan2 nx nz
                                        let phi = if phi' < 0.0
                                                    then phi'+2.0*pi
                                                    else phi'
                                        let u = phi/(2.0*pi)
                                        let v = (hitY/height)+0.5
                                        getTextureColour texture (u,v))) 
                            else (None, None, None,SceneAssets.black)
                        //Formula variables
                        let px = Point.getX ray.origin
                        let pz = Point.getZ ray.origin
                        let (dx,_,dz) = Vector.getCoord dir

                        //Discriminant variables
                        let a = ((dx*dx) + (dz*dz))
                        let b = 2.0*px*dx + 2.0*pz*dz
                        let c = ((px*px) + (pz*pz) - radius*radius)
    
                        //Distance
                        let t = solveSecondDegree a b c 
                        if t.IsSome then
                            let hitPoint = Point.move ray.origin (t.Value * dir)
                            let hit1 = checkHitPoint(hitPoint)
                            match hit1 with
                            |(None,_,_,_) -> 
                                let point = Point.move hitPoint  (FloatHelper.ZeroThreshold * dir)
                                let px = Point.getX point
                                let pz = Point.getZ point
                                let a = ((dx*dx) + (dz*dz))
                                let b = 2.0*px*dx + 2.0*pz*dz
                                let c = ((px*px) + (pz*pz) - radius*radius)
                                let t2 = solveSecondDegree a b c 
                                if t2.IsSome then
                                    let hitPoint = Point.move point (t2.Value * dir)
                                    let (dist,hitPoint,normal,mat) = checkHitPoint(hitPoint)
                                    if(normal.IsSome) then
                                        (dist,hitPoint,Some(normal.Value),mat)
                                    else
                                        (None, None, None,SceneAssets.black)
                                else
                                    (None, None, None,SceneAssets.black) 
                            |_ -> hit1
                            
                        else (None, None, None,SceneAssets.black) 
            
                member this.IsInside p = failwith "Shape is not solid!"
        }                 

    let mkCylinder (radius : float) (height : float) (tex : Texture) = 
        let bbox = BoundingBox.computeCylinderBbox radius height
        let hitBb = BoundingBox.checkBboxIntersection bbox
        Cylinder(radius, height, tex, hitBb, bbox) 

    let SolidCylinder(top : IShape, bottom : IShape, openCylinder : IShape, height: float, radius: float) =
        let parts = TriangleMesh.mkKDtree [bottom;top;openCylinder]
        { 
            new IShape with
                member this.Bbox = openCylinder.Bbox
                member this.HitBbox (ray : Ray) = openCylinder.HitBbox ray
                member this.HitFunc (ray : Ray) = 
                    parts.HitFunc ray


                member this.IsInside p = 
                    let py = Point.getY p
                    if (py<=(height/2.0)) && (py>=(-height/2.0)) then
                        (Vector.magnitude (Point.distance p (Point.mkPoint 0.0 py 0.0)))<=radius
                    else
                        false
        }

     
    let mkSolidCylinder radius height tex toptex bottex = 

        let cyl = mkCylinder radius height tex

        let transTop = mergeTransformations [(rotateX (Math.PI/2.0));(translate  0.0 (height/2.0) 0.0)]
        let top = transform (mkDisc (Point.mkPoint 0.0 0.0 0.0) radius toptex) transTop

        let transBot = mergeTransformations [(rotateX (Math.PI/2.0));(translate  0.0 (-height/2.0) 0.0)]
        let bottom = transform (mkDisc (Point.mkPoint 0.0 0.0 0.0) radius bottex) transBot
        SolidCylinder (top,bottom,cyl,height,radius)

    let Rectangle(width : float, height : float, texture : Texture, (hitBbox : Ray -> bool), bbox : BoundingBox) = 
        let bottomLeft = Point.mkPoint 0.0 0.0 0.0
        let plane = mkPlane texture 
        { 
            new IShape with
                member this.HitFunc (ray : Ray) =
                    let norm = Vector.mkVector 0.0 0.0 0.0
                    
                    let topRight = Point.move bottomLeft (Vector.mkVector width height 0.0)
                    let (distance,hitPoint,normal,_) = plane.HitFunc ray // Check fi hit hit rectangle within infinite plane 
                    match hitPoint with
                    |Some point -> 
                                   let x,y,z = Point.getCoord point

                                   // Range variables to check if hit is within rectangle 
                                   let xInRange = ((Point.getX bottomLeft)<= x && x <= (Point.getX topRight))
                                   let yInRange = ((Point.getY bottomLeft)<= y && y <= (Point.getY topRight))
                                   // If his is within rectangle return hit 
                                   if xInRange  && yInRange then (distance, hitPoint, normal, fun () ->  
                                        let u = x/width
                                        let v = y/height
                                        getTextureColour texture (u,v))
                                   else (None, None, None, SceneAssets.black)
                    |None -> (None, None, None, SceneAssets.black)

                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = hitBbox ray
                member this.IsInside p = failwith "Shape is not solid!"
       }

    let mkRectangle (bottomLeft : Point) (width : float) (height : float) (tex : Texture) = 
        let bbox = BoundingBox.computeRectangleBbox width height
        let hitBb = BoundingBox.checkBboxIntersection bbox
        let x,y,z = Point.getCoord bottomLeft
        transform (Rectangle(width, height, tex, hitBb, bbox) ) (translate x y z)
        
    let Box(lowerCorner : Point, upperCorner : Point, sides: IShape list, (hitBbox : Ray -> bool), bbox : BoundingBox) = 
        
        {
            new IShape with
                member this.HitFunc (ray : Ray) =
                    getNearestHit sides ray
                member this.Bbox = bbox

                member this.HitBbox (ray : Ray) = hitBbox ray
           
                member this.IsInside p = 
                    let (x2,y2,z2) = Point.getCoord upperCorner
                    let (x1,y1,z1) = Point.getCoord lowerCorner
                    let (px,py,pz) = Point.getCoord p
                    x1<=px && px<=x2 && y1 <= py && py <=y2 && z1 <= pz && pz <= z2
        }

    let mkBox (low : Point) (high : Point) (front : Texture) (back : Texture) (top : Texture) (bottom : Texture) (left : Texture)  (right : Texture)   = 
        let bbox = BoundingBox.computeBoxBbox low high
        let hitBb = BoundingBox.checkBboxIntersection (bbox)
        let toRad n = n*Math.PI/180.0
        let xRot = rotateX (toRad 90.0)
        let yRot = rotateY (toRad -90.0)
        let lx,ly,lz = Point.getCoord low
        let hx,hy,hz = Point.getCoord high
        let w,h,d = hx-lx,hy-ly,hz-lz

        let back = mkRectangle (Point.mkPoint 0.0 0.0 0.0) w h back
        let back = transform back (mergeTransformations [mirrorZ; translate lx ly lz])

        let front = mkRectangle (Point.mkPoint 0.0 0.0 0.0) w h front
        let front = transform front (mergeTransformations [translate lx ly hz])

        let left = mkRectangle (Point.mkPoint 0.0 0.0 0.0) d h left
        let left = transform left (mergeTransformations [yRot; translate lx ly lz])

        let right = mkRectangle (Point.mkPoint 0.0 0.0 0.0) d h right
        let right = transform right (mergeTransformations [mirrorZ; yRot; translate hx ly lz])

        let bottom = mkRectangle (Point.mkPoint 0.0 0.0 0.0) w d bottom
        let bottom = transform bottom (mergeTransformations [mirrorZ; xRot;translate lx ly lz])

        let top = mkRectangle (Point.mkPoint 0.0 0.0 0.0) w d top
        let top = transform top (mergeTransformations [mirrorZ; xRot; translate lx hy lz])

        Box(low, high, [bottom;
                        top;
                        left;
                        back;
                        front;
                        right
                        ], hitBb, bbox) 

    let group (s1:Shape) (s2:Shape) =
        { 
            //Make a new shape by combining two and drawing the first one you meet only
            new Shape with
                member this.HitFunc (ray : Ray) = 
                        let hit1 = s1.HitFunc ray
                        let hit2 = s2.HitFunc ray
                        let (d1,p1,_,_) = hit1
                        let (d2,p2,_,_) = hit2
                        match (d1,d2) with
                        |(Some dist1,Some dist2) -> // both hits and are not inside of eachother
                                if(dist1>dist2) then hit2
                                else hit1
                        |(Some dist,_) -> hit1 // first hit hits and are not inside of eachother
                        |(_,Some dist) -> hit2 // second hit hits and are not inside of eachother
                        |(_,_) -> (None,None,None,SceneAssets.black) // there were no hits
                                                   
                //Make new bounding box for the new shape - take lowest point and highest point of the union
                member this.HitBbox (ray : Ray) = s1.HitBbox ray || s2.HitBbox ray
                member this.Bbox = let s1Lower = s1.Bbox.lower
                                   let s1Higher = s1.Bbox.higher
                                   let s2Lower = s2.Bbox.lower
                                   let s2Higher = s2.Bbox.higher
                                   let newLower = BoundingBox.findLowestPoint s1Lower s2Lower
                                   let newHigher = BoundingBox.findHighestPoint s1Higher s2Higher
                                   SceneAssets.mkBbox newLower newHigher
                member this.IsInside p = s1.IsInside p || s2.IsInside p
            }
