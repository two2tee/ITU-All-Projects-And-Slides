namespace RayTracer

module Transformation = 
    
    type Shape = SceneAssets.IShape
    
    type matrix = float[,]
    type Transformation = matrix*matrix
    type Ray = SceneAssets.Ray

    val translate : x:float -> y:float -> z:float -> Transformation 
    
    val rotateX : angle:float -> Transformation 
    val rotateY : angle:float -> Transformation 
    val rotateZ : angle:float -> Transformation 

    val sheareXY : distance:float -> Transformation 
    val sheareXZ : distance:float -> Transformation 
    val sheareYX : distance:float -> Transformation 
    val sheareYZ : distance:float -> Transformation 
    val sheareZX : distance:float -> Transformation 
    val sheareZY : distance:float -> Transformation 

    val scale : x:float -> y:float -> z:float -> Transformation 

    val mirrorX : Transformation 
    val mirrorY : Transformation
    val mirrorZ : Transformation 

    val transform : Shape -> Transformation -> Shape 
    val mergeTransformations : Transformation list -> Transformation
    val transformBbox : BoundingBox.BoundingBox -> Transformation -> BoundingBox.BoundingBox 


