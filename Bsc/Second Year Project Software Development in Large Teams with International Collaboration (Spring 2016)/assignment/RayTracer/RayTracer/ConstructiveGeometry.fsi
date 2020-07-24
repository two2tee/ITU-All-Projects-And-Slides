namespace RayTracer

module ConstructiveGeometry =
    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = Shape.Ray
    type Shape = Shape.IShape
    type Texture = RayTracer.SceneAssets.Texture

    val union : s1:Shape -> s2:Shape -> Shape
    val intersection : s1:Shape -> s2:Shape -> Shape
    val subtraction : s1:Shape -> s2:Shape -> Shape