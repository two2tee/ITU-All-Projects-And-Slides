namespace RayTracer
module BoundingBox =
    type Point = SceneAssets.Point
    type Vector = Vector.Vector
    type BoundingBox = SceneAssets.BoundingBox
    type Ray = SceneAssets.Ray
      
    
    val computeTriangleBbox : Point -> Point -> Point -> BoundingBox
    val computeSphereBbox : float -> BoundingBox
    val computeBoxBbox : Point -> Point -> BoundingBox
    val computeDiscBbox :  float -> BoundingBox
    val computeCylinderBbox  :  float -> float -> BoundingBox
    val computeRectangleBbox : float -> float -> BoundingBox
    val getBboxIntersection: bbox:BoundingBox -> ray:Ray -> bool*float*float
    val checkBboxIntersection: bbox:BoundingBox -> ray:Ray -> bool
    val computeSceneBbox: BoundingBox list -> BoundingBox
    val computeSceneBboxArr: BoundingBox[] -> BoundingBox
    val findLowestPoint : Point -> Point -> Point
    val findHighestPoint : Point -> Point -> Point 
    
