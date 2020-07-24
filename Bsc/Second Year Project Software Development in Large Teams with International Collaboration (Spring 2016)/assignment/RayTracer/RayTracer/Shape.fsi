namespace RayTracer
//This module is responsible for performing functions related to shapes
module Shape =
    type Point = Point.Point
    type Texture = SceneAssets.Texture
    type Vector = Vector.Vector
    type BoundingBox = BoundingBox.BoundingBox
    type Ray = SceneAssets.Ray
    type Material = RayTracer.SceneAssets.Material
    type hitBboxFunc = Ray -> bool
    type Vertex = SceneAssets.Vertex
    type IShape = SceneAssets.IShape

    
    // Shape creation functions 
    val mkSphere: Point -> float -> Texture -> IShape
    val mkTriangle : Point -> Point -> Point -> Texture -> IShape 
    val mkMeshTriangle : Vertex -> Vertex -> Vertex-> Texture -> bool -> IShape
    val mkPlane : Texture -> IShape 
    val mkBox : low : Point -> high : Point -> front : Texture -> back : Texture ->
              top : Texture -> bottom : Texture -> left : Texture -> right : Texture  -> IShape 
    val mkDisc: center : Point -> radius : float -> Texture -> IShape 
    val mkCylinder: radius : float -> height:  float -> Texture -> IShape 
    val mkSolidCylinder : r : float -> h : float -> t : Texture -> top : Texture -> bottom : Texture -> IShape
    val mkRectangle: bottomLeft:  Point -> width : float -> height : float -> Texture -> IShape 
    val group : IShape -> IShape  -> IShape 

    //val mkImplicit: string -> IShape



  

