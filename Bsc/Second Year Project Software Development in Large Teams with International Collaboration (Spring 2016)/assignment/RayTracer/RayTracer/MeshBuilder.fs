namespace RayTracer

(*
    This module is responsible for creating triangle based on the 
    information given by an existing PLY file.
*)

open RayTracer.SceneAssets
open RayTracer.TriangleMesh
open RayTracer.Shape
module MeshBuilder = 
    type TriangleMesh = TriangleMesh
    type Shape = Shape.IShape
    type Vector = Vector.Vector
    type Vertex = SceneAssets.Vertex
    type Face = SceneAssets.Face
    type Point = SceneAssets.Point
    type Texture = SceneAssets.Texture

    //This function calculates the normals for each vertices within a triangle
    let calcVertexNormalsForSmoothShading faces (vertices : Vertex[]) =
        let normals : Vector[] = Array.init vertices.Length (fun i -> Vector.mkVector 0.0 0.0 0.0)
        Array.iter (fun face -> match face with 
                                | Face.F(A,B,C) -> //Calculating normal
                                                   let pointA = (Array.get vertices A).point
                                                   let pointB = (Array.get vertices B).point
                                                   let pointC = (Array.get vertices C).point

                                                   let u = Point.distance pointB pointA //The side of the triangle u
                                                   let v = Point.distance pointC pointA //The side of the triangle v
                                                   let normal = Vector.normalise(Vector.crossProduct u v)
                                                   //Adds normal to the existing sum of normals in the normals array
                                                   Array.set normals A ((Array.get normals A) + normal)
                                                   Array.set normals B ((Array.get normals B) + normal)
                                                   Array.set normals C ((Array.get normals C) + normal)) faces

        Array.iteri (fun i normal -> let sumNormal = Array.get normals i 
                                     Array.set normals i (Vector.normalise(sumNormal))) normals


        Array.iteri2 (fun i newNormal (oldVertex:Vertex) -> let newVertex = mkVertex oldVertex.point newNormal oldVertex.u oldVertex.v
                                                            Array.set vertices i newVertex) normals vertices 

    //------- BUILD FUNCTION -------//

    //This function takes an existing PLY file, parse it to the PLYparser and return a list of triangle where each of their
    // respective vertex is normalized
    let buildTriangleMesh (texture:Texture) (isSmoothShade: bool) filename  = 
        let (vertices, faces) = PlyParser.parsePLY filename

    
        //Check if returned vertices contains normals. If not calculate them else use exisiting
        let checkNormal = Vector.getCoord ((Array.get vertices 0).normal)
        match checkNormal with
        | (0.0,0.0,0.0) -> if isSmoothShade then calcVertexNormalsForSmoothShading faces vertices
                           else ()
                            
        | (_,_,_) -> ()
        

        //Create triangle abstraction based on existing faces and return them in an array
        let triangles = List.init faces.Length (fun i ->  let face = Array.get faces i
                                                          match face with
                                                          | Face.F(A,B,C) -> let vertexA = Array.get vertices A
                                                                             let vertexB = Array.get vertices B
                                                                             let vertexC = Array.get vertices C
                                                                             (mkMeshTriangle vertexA vertexB vertexC texture isSmoothShade))

       
        //Optimize structure by putting it into a KDtree and returns and ISHAPE 
        TriangleMesh.mkTriangleMesh triangles 



