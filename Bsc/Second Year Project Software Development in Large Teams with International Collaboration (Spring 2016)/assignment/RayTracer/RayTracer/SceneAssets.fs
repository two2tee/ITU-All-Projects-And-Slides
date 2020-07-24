namespace RayTracer
open System.Drawing
open System

module SceneAssets =
    type Point = Point.Point
    type Vector = Vector.Vector 
    type Colour = {red:float;green:float;blue:float}
    type LightType =
        Light of Point*Colour*float |AmbientLight of Colour*float 
    type Material = {color:Colour;reflexivity:float}
    type Texture = {textureFunction:(float -> float -> Material)}
    type Ray = {origin:Point; direction:Vector}
    type BoundingBox = {L : Point; H : Point}
    type BoundingBox with
        member this.lower = this.L
        member this.higher = this.H

    // Shape interface implemented by all shapes with common features 
    type IShape =
        abstract member HitFunc: Ray -> float Option*Point Option*Vector Option*(unit -> Material)
        abstract member HitBbox: Ray -> bool
        abstract member Bbox: BoundingBox
        abstract member IsInside: Point -> bool
    
    let mkBbox (lower : Point) (higher : Point) : BoundingBox =
        {L = lower; H = higher}

    type Vertex = {point:Point; normal:Vector; u:float; v:float}
    type Face = F of int * int * int
    

    //Generates a ray based on an origin point and a direction
    let mkRay p dir = {origin=p;direction=dir}
    let getOriginPoint  (ray:Ray) = ray.origin
    let getDirection (ray:Ray) = ray.direction
    
    let mkTexture func = {textureFunction = func}

    let mkMaterial col r = {color = col; reflexivity = r;}

    //Gamma correction for colors
    let Gamma = 0.95
    
    let mkMatTexture mat = {textureFunction = (fun _ _ -> mat)}

    let mkLight center color intensity = Light(center,color,intensity)

    let mkColour  (r:float) (g:float) (b:float) = 
        let gamma = Gamma
        {red = Math.Pow(r,gamma); green = Math.Pow(g,gamma); blue = Math.Pow(b,gamma)}

    //Help function for points that need to be colored black, to make things easier
    let black () = mkMaterial (mkColour 0.0 0.0 0.0) 0.0 

    let fromColor (c:System.Drawing.Color) = mkColour ((float) c.R/255.0) ((float) c.G/255.0) ((float) c.B/255.0)

    let mkAmbientLight colour intensity = AmbientLight(colour,intensity)
    
    let getTextureColour (t:Texture) (u:float,v:float) = t.textureFunction u v

    let getColor (c:Colour) = 
        let gamma = 1.0/Gamma
        Color.FromArgb(min ((int)(Math.Pow(c.red,gamma)*255.0)) 255,min ((int)(Math.Pow(c.green,gamma)*255.0)) 255, min ((int)(Math.Pow(c.blue,gamma)*255.0)) 255)

    let getMaterialColour (m:Material) = m.color;

    let getMaterialReflexivity (m:Material) = m.reflexivity;

    let getLightColour (l:LightType) = 
        match l with
        | AmbientLight(colour,_) -> colour
        | Light(_,colour,_) -> colour

    let getLightIntensity (l:LightType) = 
        match l with
        | AmbientLight(_,intensity) -> intensity
        | Light(_,_,intensity) -> intensity

    let getLightPosition (l:LightType) = 
        match l with
        | Light(position,_,_) -> position
        | _ -> failwith "This light has no position!"

    type Colour with
        static member ( * ) (s:float, c:Colour) = (mkColour (c.red*s) (c.green*s) (c.blue*s))
        static member ( * ) (c1:Colour,c2:Colour) = (mkColour (c1.red*c2.red) (c1.green*c2.green) (c1.blue*c2.blue))
        static member ( + ) (c1:Colour,c2:Colour) = (mkColour (c1.red+c2.red) (c1.green+c2.green) (c1.blue+c2.blue))

    let loadTexture (file : string): Texture =
        let img = new Bitmap(file)
        let widthf = float (img.Width - 1)
        let heightf = float (img.Height - 1)
        let texture (u : float) (v : float) = 
            lock img (fun () ->
            let color = img.GetPixel
                            (int (widthf * u), int (heightf * (1.0 - v)))
            
            mkMaterial (fromColor color) 0.0
            )
        mkTexture(texture)

    //Vertex that also contains u and v coordinates
    let mkVertex (p:Point) (n:Vector) (u:float) (v:float) = {point=p;normal=n;u=u;v=v}
