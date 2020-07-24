namespace API
open RayTracer

module API =
  type vector = Vector.Vector
  type point = Point.Point
  type colour = SceneAssets.Colour
  type material = SceneAssets.Material
  type shape = Shape.IShape
  type texture = SceneAssets.Texture

  type camera = Camera.Camera
  
  type scene = Scene.Scene
  type light = SceneAssets.LightType
  type ambientLight = SceneAssets.LightType
  type transformation = Transformation.Transformation
  type baseShape = (texture -> shape)

  let mkVector (x : float) (y : float) (z : float) : vector = Vector.mkVector x y z
  let mkPoint (x : float) (y : float) (z : float) : point = Point.mkPoint x y z
  let fromColor (c : System.Drawing.Color) : colour = SceneAssets.fromColor c


  let mkColour (r : float) (g : float) (b : float) : colour = SceneAssets.mkColour r g b

  let mkMaterial (c : colour) (r : float) : material = SceneAssets.mkMaterial c r
  let mkTexture (f : float -> float -> material) : texture = SceneAssets.mkTexture f
  let mkMatTexture (m : material) : texture = SceneAssets.mkMatTexture m

  let mkShape (b : baseShape) (t : texture) : shape = b t
  let mkSphere (p : point) (r : float) (t : texture) : shape = Shape.mkSphere p r t
  let mkTriangle (a:point) (b:point) (c:point) (t : texture) : shape = Shape.mkTriangle a b c t
  let mkRectangle (corner : point) (width : float) (height : float) (t : texture) : shape = Shape.mkRectangle corner width height t
  let mkPlane (t : texture) : shape = Shape.mkPlane t
  let mkImplicit (s : string) : baseShape = (fun tex -> ImplicitSurface.mkImplicit s tex)
  let mkPLY (filename : string) (smooth : bool) : baseShape = (fun tex -> MeshBuilder.buildTriangleMesh tex smooth filename)

  
  let mkDisc (c : point) (r : float) (t : texture) : shape = Shape.mkDisc c r t
  let mkBox (low : point) (high : point) (front : texture) (back : texture) (top : texture) (bottom : texture) (left : texture) (right : texture) : shape = Shape.mkBox low high front back top bottom left right
 
  let group (s1 : shape) (s2 : shape) : shape = Shape.group s1 s2
  let union (s1 : shape) (s2 : shape) : shape = ConstructiveGeometry.union s1 s2
  let intersection (s1 : shape) (s2 : shape) : shape = ConstructiveGeometry.intersection s1 s2
  let subtraction (s1 : shape) (s2 : shape) : shape = ConstructiveGeometry.subtraction s1 s2

  let mkCamera (pos : point) (look : point) (up : vector) (zoom : float) (width : float)
    (height : float) (pwidth : int) (pheight : int) : camera = Camera.mkCamera pos look up zoom width height pwidth pheight
  let mkLight (p : point) (c : colour) (i : float) : light = SceneAssets.mkLight p c i
  let mkAmbientLight (c : colour) (i : float) : ambientLight = SceneAssets.mkAmbientLight c i

  let mkScene (s : shape list) (l : light list) (a : ambientLight) (c : camera) (m : int) : scene = Scene.mkScene s l a c m
  let renderToScreen (sc : scene) : unit = Scene.renderToScreen sc
  let renderToFile (sc : scene) (path : string) : unit = Scene.renderToFile sc path

  let translate (x : float) (y : float) (z : float) : transformation = Transformation.translate x y z
  let rotateX (angle : float) : transformation = Transformation.rotateX angle
  let rotateY (angle : float) : transformation = Transformation.rotateY angle
  let rotateZ (angle : float) : transformation = Transformation.rotateZ angle
  let sheareXY (distance : float) : transformation = Transformation.sheareXY distance
  let sheareXZ (distance : float) : transformation = Transformation.sheareXZ distance
  let sheareYX (distance : float) : transformation = Transformation.sheareYX distance
  let sheareYZ (distance : float) : transformation = Transformation.sheareYZ distance
  let sheareZX (distance : float) : transformation = Transformation.sheareZX distance
  let sheareZY (distance : float) : transformation = Transformation.sheareZY distance
  let scale (x : float) (y : float) (z : float) : transformation = Transformation.scale x y z
  let mirrorX : transformation = Transformation.mirrorX
  let mirrorY : transformation = Transformation.mirrorY
  let mirrorZ : transformation = Transformation.mirrorZ 
  let mergeTransformations (ts : transformation list) : transformation = Transformation.mergeTransformations ts
  let transform (sh : shape) (tr : transformation) : shape = Transformation.transform sh tr


  let mkHollowCylinder (c : point) (r : float) (h : float) (t : texture) : shape = 
        let x,y,z = Point.getCoord c
        transform (Shape.mkCylinder r h t) (translate x y z)
  let mkSolidCylinder (c : point) (r : float) (h : float) (t : texture) (bottom : texture) (top : texture) : shape = 
        let x,y,z = Point.getCoord c
        transform (Shape.mkSolidCylinder r h t top bottom) (translate x y z)