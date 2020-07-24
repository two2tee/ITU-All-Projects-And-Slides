namespace RayTracer

open KDTree
module TriangleMesh =

    type Tree = KDTree.Tree
    type BoundingBox = BoundingBox.BoundingBox
    type Shape = SceneAssets.IShape
    type Ray = SceneAssets.Ray
    type Point = Point.Point
    type Vector = Vector.Vector
    type Material = SceneAssets.Material
    type Vertex = SceneAssets.Vertex
    
      
    let TriangleMesh(tree : Tree, (hitBbox : Ray -> bool), bbox : BoundingBox) = 
        {   
            new Shape with 
                member this.Bbox = bbox
                member this.HitBbox (ray : Ray) = hitBbox ray
                member this.HitFunc (ray : Ray) =  
                    let res = traverse tree bbox ray
                    match res with
                    | Some(hit,normal,mat) -> (Some (Vector.magnitude (Point.distance ray.origin hit)),Some(hit),Some(normal),(fun () -> mat))
                    | None -> (None,None,None,SceneAssets.black)
                member this.IsInside p = failwith "Shape is not solid!"
        }


    let mkKDtree (shapes : Shape list) =
        let (tree, bbox) = buildKdTree shapes 
        let hitBb = BoundingBox.checkBboxIntersection bbox
        TriangleMesh(tree, hitBb, bbox) 

    let mkTriangleMesh (shapes : Shape list)  = mkKDtree shapes //For the sake of abstraction
