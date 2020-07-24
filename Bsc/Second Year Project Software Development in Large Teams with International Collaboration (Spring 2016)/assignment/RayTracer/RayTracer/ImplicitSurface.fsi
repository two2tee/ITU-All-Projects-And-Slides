module ImplicitSurface

    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = RayTracer.Shape.Ray
    type Material = RayTracer.SceneAssets.Material
    type BoundingBox = BoundingBox//.BoundingBox
    type Texture = RayTracer.SceneAssets.Texture
    // Interface used by all shapes with common functions 
    type IShape = RayTracer.Shape.IShape

    val mkImplicit: string -> Texture -> IShape
