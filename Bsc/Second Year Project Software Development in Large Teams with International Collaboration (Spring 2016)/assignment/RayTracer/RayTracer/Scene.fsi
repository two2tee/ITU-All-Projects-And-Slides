namespace RayTracer

module Scene =
    type Point = Point.Point
    type Vector = Vector.Vector
    type Camera = Camera.Camera
    type Shape = Shape.IShape
    type LightType = RayTracer.SceneAssets.LightType
    type Scene = {shapes : Shape list; lights : LightType list; ambientLight : LightType; camera : Camera; max_reflect : int}
    type Ray = SceneAssets.Ray
    type IShape = RayTracer.Shape.IShape

  
    type Colour = RayTracer.SceneAssets.Colour
  

    val mkScene : shapes : IShape list -> lights : LightType list -> ambientLight:LightType -> Camera -> max_reflect : int -> Scene
  
    val renderToScreen : Scene -> unit
    val renderToFile : Scene -> filename:string -> unit