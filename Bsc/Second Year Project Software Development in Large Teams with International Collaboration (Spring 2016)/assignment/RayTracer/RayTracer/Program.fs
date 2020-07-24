open RayTracer.SceneAssets 
open RayTracer.Camera
open RayTracer.Scene
open RayTracer.Shape
open RayTracer.Transformation
open ExprParse
open PolynomialFormulas
open ExprToPoly
open RayTracer.ConstructiveGeometry
open RayTracer.MeshBuilder
open Point
open RayTracer.BoundingBox
open RayTracer.KDTree
open RayTracer.TriangleMesh
open RayTracer.PlyParser
open ImplicitSurface

[<EntryPoint>]
let main argv = 
    //----------SETTINGS: camera, texture and light 
    printfn "Setting Camera...";
    let cam = mkCamera (Point.mkPoint 0.0 0.0 22.0) (Point.mkPoint 0.0 0.0 0.0) (Vector.mkVector 0.0 1.0 0.0) 
                         10.0 10.0 10.0 1000 1000 // pos(0,0,0) look(0,100,0) zoom=100 w=1000px/10 height=1000px/10
    let ambient = mkAmbientLight(mkColour 1.0 1.0 1.0) 0.1

    printfn "Loading Textures...";
    let texturePath = "../../../Resources/" //Change path to your resource
    let woodTexture = loadTexture (texturePath+"wood_tex.jpg")
    let concreteTexture = loadTexture(texturePath+"concrete_tex.jpg")
    let cracksTexture = loadTexture(texturePath+"cracks_tex.jpg")
    let stripesTexture = loadTexture(texturePath+"stripes_tex.jpg")
    let dennisTexture = loadTexture(texturePath+"dennis_tex.jpg")
    let meatTexture = loadTexture(texturePath+"meat_tex.jpg")
    let roadTexture = loadTexture(texturePath+"road.jpg")
    let bunnyTexture = loadTexture(texturePath+"bunny_tex.jpg")
    let red = mkMatTexture (mkMaterial (mkColour 1.0 0.0 0.0) 0.5)
    let white = mkMatTexture (mkMaterial (mkColour 1.0 1.0 1.0) 0.5)
    let tex = mkMatTexture (mkMaterial (mkColour 0.0 0.0 1.0) 0.6 )
    let tex2 = mkMatTexture (mkMaterial (mkColour 0.0 1.0 0.4) 0.5 )

    printfn "Placing lights...";
    let l1 = mkLight (Point.mkPoint 10.0 50.0 60.0) (mkColour 1.0 1.0 1.0) 0.7
    let l2 = mkLight (Point.mkPoint 20.0 20.0 50.0) (mkColour 1.0 1.0 1.0) 1.0
    let l3 = mkLight (Point.mkPoint 0.0 00.0 -30.0) (mkColour 1.0 1.0 1.0) 1.0
    


    
    //-----------BASE SHAPES ---------------------
    let basePlane = mkPlane woodTexture
    let s = mkSphere (Point.mkPoint 0.0 0.0 0.0) 3.0 tex
    let s3 = mkSphere (Point.mkPoint 0.0 0.0 0.0) 10.0 woodTexture 
    let s2 = transform (mkSphere (Point.mkPoint 0.0 0.0 0.0) 5.0 tex) (translate 1.0 0.0 0.0)
    let t = mkTriangle (Point.mkPoint 0.0 0.0 0.0) (Point.mkPoint 10.0 0.0 0.0) (Point.mkPoint 0.0 10.0 0.0) tex
    let c = mkCylinder 5.0 20.0 meatTexture 
    
    let c2 = transform (mkCylinder 5.0 10.0 tex2) (translate 14.0 0.0 0.0)
    let sc = mkSolidCylinder 5.0 10.0 tex tex tex  //topDisc botDisc baseCylinder tex  
    let sc2 = transform (mkSolidCylinder 5.0 10.0 tex2 tex2 tex2) (translate 8.0 0.0 0.0)  //topDisc botDisc baseCylinder tex  
    let un = intersection sc sc2
    

    //-----------TRIANGLEMESH---------------------
    (*
        Add or find additional PLY files in:
        *\Second_Year_Project_Ray_Tracer\RayTracer\Resources
    *)

    printfn "TriangleMesh is building... "
    let filename = "../../../Resources/posche.ply"
    let isSmoothShade = true
    let TriangleMesh = buildTriangleMesh red isSmoothShade filename

    let filename = "../../../Resources/bunny.ply"
    let bunny = buildTriangleMesh white isSmoothShade filename
    //printfn "TriangleMesh done... "



    //-----------TRANSFORMATION-------------------
    // Rotate needs shape to be in origo (0,0,0) and rays need to be in correct direction (e.g. camera needs to be on z direction if testing rotateZ)
    // Not working, black screen possibly due to direction of rays???
    printfn "transforming shapes...";
    
    //----------- SCENE ---------------------
    (*
        Guidelines 
        modify mkScene to define your scene
        mkSchene [shapes][lights] ambient cam

        eg
        mkScene [ shape 1 ; shape 2 ; shape N ] [ light 1 ; light 2 ; light N ] ambient cam 1

    *)

    let transformedMesh = transform TriangleMesh (mergeTransformations [rotateY (-System.Math.PI/2.0); translate 0.0 -8.0 -12.0])
    let bunny = transform bunny (mergeTransformations [rotateY (System.Math.PI/4.0); scale 12.0 12.0 12.0;translate 0.0 -11.0 -5.0])
    let road =  (mkRectangle (mkPoint -50.0 -33.0 -50.0) 100.0 80.0 roadTexture)
    printfn "Rendering scene...";
    let shapes = [road;transformedMesh;bunny]  //<-- ADD SHAPES HERE
    let lights = [l1]
    let reflections = 0

    let scene = mkScene  shapes lights ambient cam reflections
    renderToFile scene "Final scene.png" 
    printfn "Image rendered"
    1 // return an integer exit code
   
