namespace RayTracer
[<Sealed>]

module Camera =
    type Point = Point.Point
    type Vector = Vector.Vector
    type Camera 
    type Ray = RayTracer.SceneAssets.Ray
    type LightType = RayTracer.SceneAssets.LightType
    type Shape = Shape.IShape
    type Texture = RayTracer.SceneAssets.Texture

    val mkCamera : pos: Point -> look : Point -> up : Vector -> zoom : float -> 
        width : float -> height : float -> pwidth : int -> pheight : int -> Camera
    val createBitmap : shapes:Shape list -> lights:LightType list -> AmbientLight:LightType -> Camera -> max_reflections:int -> System.Drawing.Bitmap
