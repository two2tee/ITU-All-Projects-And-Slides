namespace RayTracer
open RayTracer.Shape
open RayTracer.SceneAssets

module ConstructiveGeometry =
    
    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = Shape.Ray
    type Shape = Shape.IShape
    type Texture = RayTracer.SceneAssets.Texture

    let union (s1:Shape) (s2:Shape) = 
            { 
            //Make a new shape by combining two and drawing the first one you meet only
            new Shape with
                member this.HitFunc (ray : Ray) = 
                        let hit1 = s1.HitFunc ray
                        let hit2 = s2.HitFunc ray
                        let (d1,p1,_,_) = hit1
                        let (d2,p2,_,_) = hit2
                        match (d1,d2) with
                        |(Some dist1,Some dist2) when not (s1.IsInside p2.Value) && not (s2.IsInside p1.Value) -> // both hits and are not inside of eachother
                                if(dist1>dist2) then hit2
                                else hit1
                        |(Some dist,_) when not (s2.IsInside p1.Value)  -> hit1 // first hit hits and are not inside of eachother
                        |(_,Some dist) when not (s1.IsInside p2.Value) -> hit2 // second hit hits and are not inside of eachother
                        |(None,None) -> (None,None,None,SceneAssets.black) // there were no hits
                        |(_,_) -> // there were only hits inside the other shape
                            let d = min d1 d2
                            if(d.IsSome) then
                            let p = Point.move ray.origin ((d.Value+FloatHelper.ZeroThreshold)*(Vector.normalise ray.direction))
                            this.HitFunc (mkRay p ray.direction)
                            else
                                (None,None,None,SceneAssets.black)
                                                   
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

    let intersection (s1:Shape) (s2:Shape) = 
            { 
            //Make a new shape by taking only the intersection of two shapes
            new Shape with
                member this.HitFunc (ray : Ray) = 
                        let hit1 = s1.HitFunc ray
                        let hit2 = s2.HitFunc ray
                        let (_,p1,_,_) = hit1
                        let (_,p2,_,_) = hit2
                        match (p1,p2) with
                        |(Some p1,Some p2) -> 
                            if(s1.IsInside p2) then hit2 
                            else if (s2.IsInside p1) then hit1
                            else (None,None,None,SceneAssets.black)
                        |(_,_) -> (None,None,None,SceneAssets.black)
                                                   
                member this.HitBbox (ray : Ray) = s1.HitBbox ray && s2.HitBbox ray
                member this.Bbox = let s1Lower = s1.Bbox.lower
                                   let s1Higher = s1.Bbox.higher
                                   let s2Lower = s2.Bbox.lower
                                   let s2Higher = s2.Bbox.higher
                                   let xPointList = Array.sort [|Point.getX s1Lower;Point.getX s1Higher;Point.getX s2Lower;Point.getX s2Higher|]
                                   let yPointList = Array.sort [|Point.getY s1Lower;Point.getY s1Higher;Point.getY s2Lower;Point.getY s2Higher|]
                                   let zPointList = Array.sort [|Point.getZ s1Lower;Point.getZ s1Higher;Point.getZ s2Lower;Point.getZ s2Higher|]
                                   
                                   let middleX = Array.get xPointList 1
                                   let middleX2 = Array.get xPointList 2
                                   let middleY = Array.get yPointList 1
                                   let middleY2 = Array.get yPointList 2
                                   let middleZ = Array.get zPointList 1
                                   let middleZ2 = Array.get zPointList 2
                                   let newPoint = Point.mkPoint middleX middleY middleZ
                                   let newPoint2 = Point.mkPoint middleX2 middleY2 middleZ2
                                   SceneAssets.mkBbox newPoint newPoint2
                member this.IsInside p = s1.IsInside p && s2.IsInside p
            }

    let subtraction (s1:Shape) (s2:Shape) : Shape = 
        { 
            //Make a new shape by subtracting two shapes from eachother
            new Shape with
                member this.HitFunc (ray : Ray) = 
                        let hit1 = s1.HitFunc ray
                        let (_,p1,_,_) = hit1
                        if (p1.IsSome && s2.IsInside(p1.Value)) then
                            let hit2 = s2.HitFunc (mkRay p1.Value (getDirection ray))
                            let (d,p2,n,m) = hit2
                            if(p2.IsSome && s1.IsInside(p2.Value)) then
                                (d,p2,Some(n.Value),m)
                            else
                                (None,None,None,SceneAssets.black)
                        else
                            hit1
                                                   
                member this.HitBbox (ray : Ray) = s1.HitBbox ray
                member this.Bbox = s1.Bbox
                member this.IsInside p = s1.IsInside p && not (s2.IsInside p)
            }
