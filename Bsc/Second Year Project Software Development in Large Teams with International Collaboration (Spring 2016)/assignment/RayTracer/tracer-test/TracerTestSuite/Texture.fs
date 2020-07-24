﻿namespace TracerTestSuite

open System
open System.Drawing
open API.API

module Texture =
  let folder = "texture"

  let mkTextureFromFile (tr : float -> float -> float * float) (file : string) =
    let img = new Bitmap(file)
    let width = img.Width - 1
    let height = img.Height - 1
    let widthf = float width
    let heightf = float height
    let texture x y =
      let (x', y') = tr x y
      let x'', y'' = int (widthf * x'), int (heightf * y')
      let c = lock img (fun () -> img.GetPixel(x'',y''))
      mkMaterial (fromColor c) 0.0
    mkTexture texture

  let renderEarth toScreen =
    let texture = mkTextureFromFile (fun x y -> (x,1.0-y)) "../../../textures/earth.jpg"
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 1.0 30.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 20.0 2.0 2.0 1000 1000 in
    let sphere = transform (mkSphere (mkPoint 0.0 0.0 0.0) 1.0 texture) 
                  (mergeTransformations [rotateY (System.Math.PI*1.0);rotateX (Math.PI/4.0)])
    let scene = mkScene [sphere] [light] ambientLight camera 3 in
    Util.render' scene (folder, "earth.png") toScreen

  let renderSphere toScreen =
    let white = mkMaterial (fromColor Color.Red) 0.5
    let black = mkMaterial (fromColor Color.Green) 0.5
    let checker x y =
        let abs' s f = if f < 0.0 then 1.0 - (f * s) else f * s
        if (int (abs' 64.0 x) + int (abs' 32.0 y)) % 2 = 0
        then white
        else black
    let texture = mkTexture checker
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 1.0 30.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 20.0 2.0 2.0 1000 1000 in
    let sphere = transform (mkSphere (mkPoint 0.0 0.0 0.0) 1.0 texture) (rotateX (Math.PI/4.0))
    let scene = mkScene [sphere] [light] ambientLight camera 3 in
    Util.render' scene (folder, "sphere.png") toScreen



  let renderCylinder toScreen =
    let white = mkMaterial (fromColor Color.Red) 0.0
    let black = mkMaterial (fromColor Color.Green) 0.0
    let checker x y =
        let abs' f = if f < 0.0 then 1.0 - (f*64.0) else f * 64.0
        if (int (abs' x) + int (abs' y)) % 2 = 0
        then white
        else black
    let cbase = mkSolidCylinder  (mkPoint 0.0 0.0 0.0) 0.5 1.9 (mkTexture checker) 
                  (mkTexture checker) (mkMatTexture (mkMaterial (fromColor Color.White) 0.0))
    let c = transform cbase (rotateX (Math.PI/4.0))
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 0.0 30.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 20.0 2.0 2.0 1000 1000 in
    let scene = mkScene [c] [light] ambientLight camera 2
    Util.render' scene (folder, "cylinder.png") toScreen

  let renderDiscs toScreen =
    let mkMat c = mkMaterial (fromColor c) 0.0
    let colours = Array.map mkMat [|Color.Green;Color.Red;Color.Blue;Color.Yellow;Color.Magenta;Color.Orange;Color.Cyan;Color.White|]
    let checker x' y' =
      let x = 2.0*x' - 1.0
      let y = 2.0*y' - 1.0
      let a = atan2 x y
      let a' = if a < 0.0 then a + 2.0 * Math.PI else a
      let d = int (4.0*(a' / Math.PI)) + if x * x + y * y <= 0.25 then 4 else 0
      colours.[d%8]
    let disc = mkDisc (mkPoint 0.0 0.0 0.0) 0.7  (mkTexture checker) 
    let d1 = transform disc (mergeTransformations [translate -0.5 -0.5 -0.5])
    let d2 = transform disc (mergeTransformations [rotateX (-Math.PI/4.0);translate 0.5 0.5 0.5])
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 0.0 30.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 20.0 2.0 2.0 1000 1000 in
    let scene = mkScene [d1;d2] [light] ambientLight camera 2
    Util.render' scene (folder, "discs.png") toScreen

  let mkColor c = mkMatTexture (mkMaterial (fromColor c) 0.0)

  let renderBox toScreen =
    let texture = mkTextureFromFile (fun x y -> (x,1.0-y)) "../../../textures/earth.jpg"
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 1.0 30.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 20.0 2.0 2.0 1000 1000 in
    let box = transform (mkBox (mkPoint -1.0 -1.0 -1.0) (mkPoint 1.0 1.0 1.0) 
                        (mkColor Color.Blue) (mkColor Color.Red) (mkColor Color.Green) 
                        (mkColor Color.Yellow) (mkColor Color.Purple) (mkColor Color.White)) 
                        (mergeTransformations [rotateY (System.Math.PI/4.0);rotateX (Math.PI/4.0)])
    let scene = mkScene [box] [light] ambientLight camera 3 in
    Util.render' scene (folder, "box.png") toScreen


  let renderBunny toScreen =
    let baseBunny = mkPLY "../../../ply/bunny_textured.ply" true
    let t = mergeTransformations
              [rotateY (Math.PI / 4.0);
               scale 6.0 6.0 6.0;
               translate 0.0 3.0 0.0] in
    let white = fromColor Color.White
    let tex = mkTextureFromFile (fun x y -> (y,x)) "../../../textures/bunny.png"
    let bunny = mkShape baseBunny tex
    let affineBunny = transform bunny t in
    let t' = scale 0.5 0.5 0.5
    let l1 = mkLight (mkPoint 6.0 2.0 6.0) white 0.5
    let l2 = mkLight (mkPoint -6.0 2.0 6.0) (fromColor Color.Red) 0.5
    let l3 = mkLight (mkPoint -3.5 12.0 4.0) white 1.0
    let p = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.Green) 0.5)))
              (rotateX (System.Math.PI/2.0))
    let c = mkCamera (mkPoint 4.0 8.0 16.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 4.0 4.0 4.0 1000 1000
    let ambientLight = mkAmbientLight (fromColor Color.Green) 0.1
    let scene = mkScene [p; affineBunny] [l1; l2; l3] ambientLight c 2
    Util.render' scene (folder, "bunny.png") toScreen


  let renderPlane toScreen =
    let white = mkMaterial (fromColor Color.Red) 0.5
    let black = mkMaterial (fromColor Color.Green) 0.5
    let checker x y =
        let abs' s f = if f < 0.0 then 1.0 - (f * s) else f * s
        if (int (abs' 64.0 x) + int (abs' 32.0 y)) % 2 = 0
        then white
        else black
    
    let reflect = mkMaterial (fromColor Color.White) 0.8
    let notreflect = mkMaterial (fromColor Color.Green) 0.0
    let checker2 x y =
        let abs' f = if f < 0.0 then 1.0 - (f*2.0) else f * 2.0
        if (int (abs' x) + int (abs' y)) % 2 = 0
        then reflect
        else notreflect
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 0.9 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 2.0 8.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 4.0 2.0 2.0 1000 1000 in
    let sphere = mkSphere (mkPoint 1.0 1.0 0.0) 1.0 (mkMatTexture (mkMaterial (fromColor Color.Blue) 0.2)) in
    let p' = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.White) 0.0)))
               (mergeTransformations [rotateX (System.Math.PI/2.0); translate 0.0 10.0 0.0])
    let p = transform (mkPlane (mkTexture checker2)) (rotateX (System.Math.PI/2.0))
    let scene = mkScene [sphere;p;p'] [light] ambientLight camera 3 in
    Util.render' scene (folder, "plane.png") toScreen

  let renderBoxes toScreen =
    let ftex c1 c2 c3 c4 = 
      let tfun x y = 
        if x < 0.5 
        then if y < 0.5 then mkMaterial c1 0.0 else mkMaterial c3 0.0
        else if y < 0.5 then mkMaterial c4 0.0 else mkMaterial c2 0.0
      mkTexture tfun
    let one = ftex (fromColor Color.White) (fromColor Color.Orange) (fromColor Color.Magenta) (fromColor Color.Blue)
    let two = ftex (fromColor Color.White) (fromColor Color.Orange) (fromColor Color.Magenta) (fromColor Color.Green)
    let three = ftex (fromColor Color.White) (fromColor Color.Orange) (fromColor Color.Magenta) (fromColor Color.Red)
    let white = mkColor Color.White
    let light = mkLight (mkPoint 0.0 1.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 1.0 30.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 20.0 4.0 2.0 2000 1000 in
    let box1 = transform (mkBox (mkPoint -1.0 -1.0 -1.0) (mkPoint 1.0 1.0 1.0) 
                           one white two white three white)
                        (mergeTransformations [rotateY (System.Math.PI/4.0);rotateX (Math.PI/4.0); translate -1.5 0.0 0.0])
    let box2 = transform (mkBox (mkPoint -1.0 -1.0 -1.0) (mkPoint 1.0 1.0 1.0) 
                           white two white three white one)
                        (mergeTransformations [rotateY (System.Math.PI + System.Math.PI/ 4.0);rotateX (Math.PI/ -4.0); translate 1.5 0.0 0.0])
    let scene = mkScene [box1;box2] [light] ambientLight camera 3 in
    Util.render' scene (folder, "boxes.png") toScreen


  let render toScreen =
    renderDiscs toScreen
    renderBox toScreen
    renderBoxes toScreen
    renderBunny toScreen
    renderCylinder toScreen
    renderEarth toScreen
    renderPlane toScreen
    renderSphere toScreen