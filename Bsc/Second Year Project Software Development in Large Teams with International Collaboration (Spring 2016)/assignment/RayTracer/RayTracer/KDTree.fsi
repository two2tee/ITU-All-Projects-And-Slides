namespace RayTracer

module KDTree = 
    type Shape = SceneAssets.IShape
    type Scene = SceneAssets
    type BoundingBox = RayTracer.BoundingBox.BoundingBox
    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = SceneAssets.Ray
    type Material = SceneAssets.Material

    type Tree =
        | Leaf of Shape list
        | Node of int * float * Tree * Tree

    val traverse:  Tree -> BoundingBox -> Ray ->  (Point*Vector*Material) option
    val buildKdTree: Shape list -> Tree * BoundingBox.BoundingBox