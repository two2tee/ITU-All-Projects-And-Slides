namespace RayTracer
(*
    This module is responsible for creating triangle based on the 
    information given by an existing PLY file.
*)

module MeshBuilder = 
    type Shape = Shape.IShape
    type TriangleMesh = TriangleMesh
    type Vector = Vector.Vector
    type Vertex = SceneAssets.Vertex
    type Face = SceneAssets.Face
    type Texture= SceneAssets.Texture

    val buildTriangleMesh:  Texture -> bool -> string  -> Shape