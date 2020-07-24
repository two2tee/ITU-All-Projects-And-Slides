namespace TracerTestSuite

open API.API
open System
open System.Drawing

module Shapes =

  let private folder = "shapes"

  let renderSphere toScreen =
    let light = mkLight (mkPoint 0.0 0.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 0.0 4.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 1.0 2.0 2.0 500 500 in
    let sphere = mkSphere (mkPoint 0.0 0.0 0.0) 1.0 (mkMatTexture (mkMaterial (fromColor Color.Blue) 0.0)) in
    let scene = mkScene [sphere] [light] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "renderSphere.png"))

  let renderReflectiveSpheres toScreen =
    let light = mkLight (mkPoint 0.0 0.0 2.0) (mkColour 1. 1. 1.) 1.0
    let ambientLight = mkAmbientLight (mkColour 1. 1. 1.) 0.2
    let camera = mkCamera (mkPoint 0.0 0.0 2.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 1.0 2.0 2.0 500 500
    let sphere = mkSphere (mkPoint -0.8 0.0 0.0) 0.7 (mkMatTexture (mkMaterial (mkColour 0. 0. 1.) 0.7))
    let sphere2 = mkSphere (mkPoint 0.8 0.0 0.0) 0.7 (mkMatTexture (mkMaterial (mkColour 1. 0. 0.) 0.7))
    //let plane = mkPlane (mkPoint 0.0 0.0 0.0) (mkVector 1. 1. 1.) (mkMatTexture (mkMaterial (mkColour 0. 1. 0.) 0.8))
    let scene = mkScene [sphere;sphere2] [light] ambientLight camera 4
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "renderReflectiveSpheres.png"))

  let renderHollowCylinder toScreen =
    let light = mkLight (mkPoint 2.0 3.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 10.0 20.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 -0.5) 18.0 4.0 4.0 500 500 in
    let cylinder = mkHollowCylinder (mkPoint 0.0 0.0 0.0) 2.0 1.0 (mkMatTexture (mkMaterial (fromColor Color.Yellow) 0.0)) in
    let scene = mkScene [cylinder] [light] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "renderHollowCylinder.png"))

  let renderSolidCylinder toScreen =
    let light = mkLight (mkPoint 2.0 3.0 4.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 10.0 20.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 -0.5) 18.0 4.0 4.0 500 500 in
    let cylinder = 
      mkSolidCylinder (mkPoint 0.0 0.0 0.0) 2.0 1.0 (mkMatTexture (mkMaterial (fromColor Color.Yellow) 0.0))
        (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0)) (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0)) in
    let scene = mkScene [cylinder] [light] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "renderSolidCylinder.png"))

  let renderInsideSphere toScreen =
    let light = mkLight (mkPoint 0.0 0.0 0.0) (fromColor Color.White) 1.0 in
    let ambientLight = mkAmbientLight (fromColor Color.White) 0.1 in
    let camera = mkCamera (mkPoint 0.0 0.0 0.0) (mkPoint 0.0 0.0 4.0) (mkVector 0.0 1.0 0.0) 1.0 2.0 2.0 500 500 in
    let sphere = mkSphere (mkPoint 0.0 0.0 0.0) 1.0 (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0)) in
    let scene = mkScene [sphere] [light] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "renderInsideSphere.png"))


  let render toScreen =
    renderSphere toScreen;
    renderReflectiveSpheres toScreen;
    renderHollowCylinder toScreen;
    renderSolidCylinder toScreen;
    renderInsideSphere toScreen