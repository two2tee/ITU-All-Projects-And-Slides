namespace TracerTestSuite

open API.API
open System
open System.Drawing

module CSG =

  let private folder = "csg"

  let mkColourTexture c r = mkMatTexture (mkMaterial (fromColor c) r)
  let mkUnitBox t = mkBox (mkPoint -1.0 -1.0 -1.0) (mkPoint 1.0 1.0 1.0) t t t t t t
  let mkUnitCylinder t = mkSolidCylinder (mkPoint 0.0 0.0 0.0) 1.0 2.0 t t t
  let l1 = mkLight (mkPoint 4.0 0.0 4.0) (fromColor Color.White) 1.0 in
  let l2 = mkLight (mkPoint -4.0 0.0 4.0) (fromColor Color.White) 1.0 in
  let l3 = mkLight (mkPoint 0.0 0.0 0.0) (fromColor Color.White) 1.0 in
  let ambientLight = mkAmbientLight (fromColor Color.White) 0.2 in
  let camera = mkCamera (mkPoint 4.0 4.0 4.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 2.0 2.0 2.0 500 500 in
  let camera2 = mkCamera (mkPoint 0.0 0.0 4.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 2.0 2.0 2.0 500 500 in

  let cube = mkUnitBox (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0))
  let sphere = mkSphere (mkPoint 0.0 0.0 0.0) 1.3 (mkMatTexture (mkMaterial (fromColor Color.Blue) 0.0))
  let sphere1 = mkSphere (mkPoint 0.5 0.0 0.0) 1.0 (mkMatTexture (mkMaterial (fromColor Color.Blue) 0.0))
  let sphere2 = mkSphere (mkPoint -0.5 0.0 0.0) 1.0 (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0))

  let cross =
    let cy = transform (mkUnitCylinder (mkColourTexture Color.Yellow 0.0)) (scale 0.7 1.5 0.7)  in
    let cx = transform cy (rotateX (Util.degrees_to_radians 90.0)) in
    let cz = transform cy (rotateZ (Util.degrees_to_radians 90.0)) in 
      union cy (union cz cx)

  let renderUnion toScreen =
    let cube = mkUnitBox (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0)) in
    let scene = mkScene [union sphere1 sphere2] [l1; l2] ambientLight camera2 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "union.png"))

  let renderIntersection toScreen =
    let cube = mkUnitBox (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0)) in
    let scene = mkScene [intersection sphere1 sphere2] [l1; l2] ambientLight camera2 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "intersection.png"))

  let renderSubtraction toScreen =
    let cube = mkUnitBox (mkMatTexture (mkMaterial (fromColor Color.Red) 0.0)) in
    let scene = mkScene [subtraction sphere2 sphere1] [l1; l2] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "subtraction.png"))

  let renderCross toScreen =
    let scene = mkScene [cross] [l1; l2] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "cross.png"))

  let renderIntersection2 toScreen =
    let scene = mkScene [intersection cube sphere] [l1; l2] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "intersection2.png"))


  let renderLantern toScreen =
    let scene = mkScene [subtraction (intersection cube sphere) cross] [l1; l2; l3] ambientLight camera 0 in
    if toScreen then
      Util.render scene None
    else
      Util.render scene (Some (folder, "lantern.png"))

  let render toScreen =
    renderUnion toScreen;
    renderIntersection toScreen;
    renderSubtraction toScreen;
    renderCross toScreen;
    renderIntersection2 toScreen;
    renderLantern toScreen