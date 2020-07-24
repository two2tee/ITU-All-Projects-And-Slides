namespace RayTracer

module TriangleMesh =
    type Tree = KDTree.Tree
    type BoundingBox = BoundingBox.BoundingBox
    type Shape = SceneAssets.IShape
    type Ray = SceneAssets.Ray
    type Point = Point.Point
    type Vector = Vector.Vector
    type Material = SceneAssets.Material
    

    val mkTriangleMesh : Shape list -> Shape
    val mkKDtree : Shape list -> Shape
    
    


    

