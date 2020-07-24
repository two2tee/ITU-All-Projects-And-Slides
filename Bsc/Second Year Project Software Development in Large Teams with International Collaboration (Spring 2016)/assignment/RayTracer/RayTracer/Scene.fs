namespace RayTracer

open System.Drawing
open System.IO
open System
open System.Windows
open System.Windows.Forms
open System.Windows.Media.Imaging
open Point
open TriangleMesh
module Scene =
    type IShape = Shape.IShape
    type Camera = RayTracer.Camera.Camera
    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = SceneAssets.Ray
    type LightType = RayTracer.SceneAssets.LightType
    type Shape = Shape.IShape
    type Colour = RayTracer.SceneAssets.Colour
    type Scene = {shapes : Shape list; lights : LightType list;ambientLight:LightType; camera : Camera; max_reflect : int}

     

    let mkScene (sha : Shape list) lig amb cam max = 
        let (nonKD,shapes) = List.fold(fun ((nons: Shape list),(shapes: Shape list)) (shape: Shape)  ->
                                            let isNonKD = Point.distance shape.Bbox.higher shape.Bbox.lower    
                                            
                                            if (Vector.magnitude isNonKD)<FloatHelper.ZeroThreshold
                                            then (shape::nons,shapes)
                                            else (nons,shape::shapes)) ([],[]) sha
        let sceneTree = if shapes.Length > 0 
                        then [TriangleMesh.mkKDtree shapes]
                        else []
        let sceneShapes = sceneTree@nonKD
        {shapes = sceneShapes; lights = lig;ambientLight = amb; camera = cam; max_reflect = max}
    
    
    //creates a new form containing the image
    let renderToScreen (s:Scene) =
        //create the image with the camera
        let bitmap : Bitmap = RayTracer.Camera.createBitmap s.shapes s.lights s.ambientLight s.camera s.max_reflect
        //creating a windows form in the 
        let form = new Form(Width = bitmap.Width, Height = bitmap.Height, MaximizeBox = false , MinimizeBox = false,
                    FormBorderStyle = FormBorderStyle.FixedSingle, Text = "Ray Tracer")
        //getting graphics from form
        let graph : Graphics = form.CreateGraphics()
        
        //on paint, paint the image received
        let paint e =
            graph.DrawImage(bitmap, 0, 0)
        //add eventhandler
        form.Paint.Add paint
        //display form
        do Application.Run form
        ()  
    
    //saves the image as a file
    let renderToFile (s:Scene) filename =
        let bitmap : Bitmap = RayTracer.Camera.createBitmap s.shapes s.lights s.ambientLight s.camera s.max_reflect
        bitmap.Save(filename)
        ()

