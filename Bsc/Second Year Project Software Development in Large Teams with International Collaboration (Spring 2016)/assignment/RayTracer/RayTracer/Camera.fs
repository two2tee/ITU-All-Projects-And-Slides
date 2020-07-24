namespace RayTracer
open System.Drawing
open System.IO
open System
open System.Windows
open System.Windows.Forms
open System.Windows.Media.Imaging
open RayTracer.SceneAssets
open System.Threading
open System.Threading.Tasks
open RayTracer.Shape


module Camera =
    type Point = Point.Point
    type Vector = Vector.Vector
    type Colour = RayTracer.SceneAssets.Colour
    type Camera = {position : Point; lookat : Point; up : Vector; zoom : float; unitHeight: float;
        unitWidth: float; pixelWidth : int; pixelHeight : int; image : Bitmap}
    type LightType = RayTracer.SceneAssets.LightType
    type Ray = Shape.Ray
    type Shape = Shape.IShape
    type Texture = RayTracer.SceneAssets.Texture

    let saturate value = min 1.0 (max 0.0 value)

    let mkCamera (pos : Point) (look : Point) (up : Vector) (zoom : float) (width : float)
                        (height : float) (pwidth : int) (pheight : int) : Camera = {position = pos; 
                                                                lookat = look; 
                                                                up = up ; 
                                                                zoom = zoom ; 
                                                                unitWidth = width ; 
                                                                unitHeight = height ; 
                                                                pixelWidth = pwidth ; 
                                                                pixelHeight = pheight;
                                                                image = new Bitmap (pwidth,pheight)}
    

    let findPixelSize (c : Camera) = 
        (c.unitWidth/(float) c.pixelWidth, c.unitHeight/(float) c.pixelHeight) 


    let rec applyLights (lights:LightType list) (s:Shape list) (hitPoint:Point) (normal:Vector) (rayVector:Vector) ambient=
        let hitPoint = Point.move hitPoint (FloatHelper.ZeroThreshold*normal)
        List.fold( fun acc light -> 
            let lightPos = getLightPosition light; //position of light
            let vectorToLight =  (Point.direction hitPoint lightPos) //vector from hitpoint to light.
            let shadowRay = mkRay hitPoint vectorToLight //the ray from hitpoint to light
            if List.exists(fun (shape:IShape) -> 
                                let (d,_,_,_) = shape.HitFunc shadowRay 
                                d.IsSome && d.Value>FloatHelper.ZeroThreshold && (Vector.magnitude (Point.distance hitPoint lightPos))>d.Value) s //this is true if it hits shape on the way
            then
                (acc+mkColour 0.0 0.0 0.0)
            else
                let L = Vector.normalise vectorToLight
                let N = Vector.normalise normal

                (acc+(saturate (Vector.dotProduct N L) * getLightIntensity light * getLightColour light))
        ) ambient lights
        

                        
    

    let fireRay ray (shapes:Shape list) =
        let possibleHits = List.map(fun (s:IShape) ->  (s.HitFunc ray)) shapes //make the hitFunc for all shapes
        let filteredHits = List.filter(fun ((d: float option),_,_,_) -> d.IsSome && d.Value>FloatHelper.ZeroThreshold ) possibleHits
        let sortedHits = List.sortBy(fun ((d: float option),_,_,_) -> d.Value ) filteredHits // sort in ascending order
        match sortedHits with
            |(Some dist,Some hit,Some normal,mat)::_->
                            Some(hit,normal,mat()) //return first hit if any
            |_ ->  None
                         

    let rec applyReflections reflections normal (v:Vector) shapes startPoint mat (lights:LightType list) ambient: Colour =
        let normal = Vector.normalise normal
        let v = Vector.normalise v
        //The vector going in the opposite direction from the vector that hits the
        //shape, in the opposite direction
        let reflectionVector = v - (2.0*(Vector.dotProduct v normal)*normal)
        //Normalized reflectionvector
        let dir = Vector.normalise(reflectionVector)
        let ref = getMaterialReflexivity mat
        let color = getMaterialColour mat
       //Make ray in the direction of reflection vector in order to see if there is anything out there
       //that needs to be reflected
        let ray = mkRay (Point.move startPoint (FloatHelper.ZeroThreshold*normal)) dir
        match reflections with
            | 0 ->
                (getMaterialColour mat) * (applyLights lights shapes startPoint normal v ambient)
            | _ -> 
                let hit = fireRay ray shapes
                //Check if the ray hits anything in the space that needs to be reflected onto this shape
                let light = (applyLights lights shapes startPoint normal v ambient)
                if(hit.IsSome) then
                    let (hitPoint,norm,matr) = hit.Value
                    //take the material of the current shape, blend in with lights, and blend
                    //with the color of the shape that has been hit + ambient light
                    (1.0-ref) * color * light + ref*(applyReflections (reflections-1) norm dir shapes hitPoint matr lights ambient)
                //otherwise, just blend lights with the material and ambient light
                else (1.0-ref) * color*light


    let generateRays (shapes:Shape list) (lights:LightType list) (ambientLight:LightType) (camera : Camera) (reflections:int) : Colour[] = 
        let p = camera.position
        let q = camera.lookat
        let u = camera.up
        let z = camera.zoom
        let l = Vector.normalise (Point.direction p q) //Normalized direction vector
        let r = Vector.normalise (Vector.crossProduct l u) //Normalized right direction vector of the image plane
        let d = Vector.normalise (Vector.crossProduct l r) //Normalized down direction vector of the image plane
        let center = Point.move p (z * l) //Center of image plane
        let top = Point.move center ((camera.unitHeight/2.0) * u) //Top center of image plane
        let left = Point.move top ((-camera.unitWidth/2.0) * r) //Top left of image plane from the scene's point of view
        let pixelSize = findPixelSize camera
        let W = fst pixelSize
        let H = snd pixelSize
        // Create points in middle of each pixel used for vectors 
        let tasks = [for i in 0..(camera.pixelWidth*camera.pixelHeight-1) 
                        do yield async { 
                                            let a = (float) (i%camera.pixelWidth)
                                            let b = (float) (i/camera.pixelWidth)
                                            let point = Point.move left (((a + 0.5)*W) * r)
                                            let point = Point.move point (((b + 0.5)*H) * d)
                                            let v = Vector.normalise (Point.direction camera.position point)
                                            let ray = mkRay camera.position v
                                            let hit = fireRay ray shapes
                                            
                                            if(hit.IsSome) then
                                                let (hitPoint,normal,mat) = hit.Value
                                                let ambient = (getLightIntensity ambientLight * getLightColour ambientLight)
                                                let color = applyReflections reflections (Vector.normalise normal) v shapes hitPoint mat lights ambient
                                                return color
                                            else
                                                return mkColour 0.0 0.0 0.0 //return black if no hits
                                        }

                    ]
        
        Async.RunSynchronously (Async.Parallel tasks)
    
    let generateRaysSync (shapes:Shape list) (lights:LightType list) (ambientLight:LightType) (camera : Camera) (reflections:int) : Colour[] = 
        let p = camera.position
        let q = camera.lookat
        let u = camera.up
        let z = camera.zoom
        let l = Vector.normalise (Point.direction p q) //Normalized direction vector
        let r = Vector.normalise (Vector.crossProduct l u) //Normalized right direction vector of the image plane
        let d = Vector.normalise (Vector.crossProduct l r) //Normalized down direction vector of the image plane
        let center = Point.move p (z * l) //Center of image plane
        let top = Point.move center ((camera.unitHeight/2.0) * u) //Top center of image plane
        let left = Point.move top ((-camera.unitWidth/2.0) * r) //Top left of image plane from the scene's point of view
        let pixelSize = findPixelSize camera
        let W = fst pixelSize
        let H = snd pixelSize
        // Create points in middle of each pixel used for vectors 
        let tasks = [|0..(camera.pixelWidth*camera.pixelHeight-1)|]; 
        Array.map(fun (i:int) ->  
                    let a = (float) (i%camera.pixelWidth)
                    let b = (float) (i/camera.pixelWidth)
                    let point = Point.move left (((a + 0.5)*W) * r)
                    let point = Point.move point (((b + 0.5)*H) * d)
                    let v = Vector.normalise (Point.direction camera.position point)
                    let ray = mkRay camera.position v
                    let hit = fireRay ray shapes
                                            
                    if(hit.IsSome) then
                        let (hitPoint,normal,mat) = hit.Value
                        let ambient = (getLightIntensity ambientLight * getLightColour ambientLight)
                        let color = applyReflections reflections (Vector.normalise normal) v shapes hitPoint mat lights ambient 
                        
                        color
                    else
                        (mkColour 0.0 0.0 0.0) //return black if no hits
                    ) tasks

        
    //creates an image of a bitmap
    let createBitmap (s:Shape list) (lights:LightType list) (ambientLight:LightType) (cam:Camera) (reflections:int) : Bitmap =      
        let w = cam.image.Width
        let h = cam.image.Height
        let colors = generateRays s lights ambientLight cam reflections;
        for i in 0..(w*h-1) do cam.image.SetPixel((i%w),(i/w),getColor(Array.get colors i)) 
        cam.image

