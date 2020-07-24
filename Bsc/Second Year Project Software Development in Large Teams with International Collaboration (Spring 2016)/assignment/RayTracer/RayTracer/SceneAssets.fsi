namespace RayTracer

module SceneAssets =
    type Point = Point.Point
    type Vector = Vector.Vector 
    type Ray = {origin:Point; direction:Vector}
    type Vertex = {point:Point; normal:Vector; u:float; v:float}

    [<Sealed>]
    type BoundingBox = 
        member lower : Point
        member higher : Point

    type Face = F of int * int * int
    
    [<Sealed>]
    type Colour =
        static member ( * ) : float * Colour -> Colour
        static member ( * ) : Colour * Colour -> Colour
        static member ( + ) : Colour * Colour -> Colour
    

    type LightType =
        Light of Point*Colour*float |AmbientLight of Colour*float 
    type Texture
    type Material

    // Interface used by all shapes with common functions 
    type IShape =
        abstract member HitFunc: Ray -> float Option*Point Option*Vector Option*(unit -> Material)
        abstract member IsInside: Point -> bool
        abstract member HitBbox: Ray -> bool
        abstract member Bbox: BoundingBox

    
    val mkRay : origin:Point -> direction:Vector -> Ray
    val getOriginPoint : ray:Ray -> Point
    val getDirection : ray:Ray -> Vector
    val mkBbox : Point -> Point -> BoundingBox
    val mkMaterial : colour : Colour -> float -> Material
    val black : unit -> Material
    val mkTexture : (float -> float -> Material) -> Texture

    val mkMatTexture : Material -> Texture

    val mkAmbientLight : colour : Colour -> intensity : float -> LightType
    
    val mkLight : position : Point -> colour : Colour -> intensity : float -> LightType
    
    val mkColour : r:float -> g:float -> b:float -> Colour

    val fromColor : c : System.Drawing.Color -> Colour

    val loadTexture : string -> Texture
    
    val getTextureColour: Texture -> u:float*v:float -> Material

    val getMaterialColour: Material -> Colour;

    val getMaterialReflexivity: Material -> float;

    val getColor : Colour -> System.Drawing.Color
    val getLightColour : LightType -> Colour
    val getLightIntensity : LightType -> float
    val getLightPosition : LightType -> Point

    //A vertex that also contains u and v coordinates and a normal vector used for triangle mesh
    val mkVertex : Point -> Vector -> float -> float -> Vertex
