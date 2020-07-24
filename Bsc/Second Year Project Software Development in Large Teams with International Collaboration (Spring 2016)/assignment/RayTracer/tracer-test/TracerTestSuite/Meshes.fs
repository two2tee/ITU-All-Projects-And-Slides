namespace TracerTestSuite

open System
open System.Drawing
open API.API

module Meshes =
  let folder = "meshes"


  let renderBunny toScreen =
    let baseBunny = mkPLY "../../../ply/bunny.ply" true
    let t = mergeTransformations
              [rotateY (Math.PI / 4.0);
               scale 40.0 40.0 40.0;
               translate 0.0 -1.5 0.0] in
    let white = fromColor Color.White
    let bunny = mkShape baseBunny (mkMatTexture (mkMaterial white 0.0))
    let affineBunny = transform bunny t in
    let t' = scale 0.5 0.5 0.5
    let l1 = mkLight (mkPoint 6.0 2.0 6.0) white 0.5
    let l2 = mkLight (mkPoint -6.0 2.0 6.0) (fromColor Color.Red) 0.5
    let l3 = mkLight (mkPoint -3.5 12.0 4.0) white 0.7
    let p = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.Green) 0.5)))
              (rotateX (System.Math.PI/2.0))
    let c = mkCamera (mkPoint 4.0 8.0 16.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 4.0 2.5 2.5 1000 1000
    let ambientLight = mkAmbientLight (fromColor Color.Green) 0.1
    let scene = mkScene [p;affineBunny] [l1; l2; l3] ambientLight c 2
    Util.render' scene (folder, "bunny.png") toScreen

  let renderDragon toScreen =
    let baseDragon = mkPLY "../../../ply/dragon.ply" false
    let t = mergeTransformations
              [rotateY (Math.PI / 4.0);
               scale 40.0 40.0 40.0;
               translate 0.0 -2.0 0.0] in
    let white = fromColor Color.White
    let dragon = mkShape baseDragon (mkMatTexture (mkMaterial white 0.0))
    let affineDragon = transform dragon t in
    let t' = scale 0.5 0.5 0.5
    let l1 = mkLight (mkPoint 6.0 2.0 6.0) white 0.5
    let l2 = mkLight (mkPoint -6.0 2.0 6.0) (fromColor Color.Red) 0.5
    let l3 = mkLight (mkPoint -3.5 12.0 4.0) white 0.7
    let p = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.Green) 0.5)))
              (rotateX (System.Math.PI/2.0))
    let c = mkCamera (mkPoint 4.0 8.0 16.0) (mkPoint 0.0 0.0 0.0) (mkVector 0.0 1.0 0.0) 4.0 4.0 4.0 1000 1000
    let ambientLight = mkAmbientLight (fromColor Color.Green) 0.1
    let scene = mkScene [p; affineDragon] [l1; l2; l3] ambientLight c 2
    Util.render' scene (folder, "dragon.png") toScreen

  let renderHappy toScreen =
    let basehappy = mkPLY "../../../ply/happy.ply" false
    let t = mergeTransformations
              [rotateY (Math.PI / 4.0);
               scale 50.0 50.0 50.0;
               translate 0.0 -2.0 1.0] in
    let white = fromColor Color.White
    let happy = mkShape basehappy (mkMatTexture (mkMaterial white 0.0))
    let affinehappy = transform happy t in
    let t' = scale 0.5 0.5 0.5
    let l1 = mkLight (mkPoint 6.0 2.0 6.0) white 0.5
    let l2 = mkLight (mkPoint -6.0 2.0 6.0) (fromColor Color.Red) 0.5
    let l3 = mkLight (mkPoint -3.5 12.0 4.0) white 0.7
    let p = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.Green) 0.5)))
              (rotateX (System.Math.PI/2.0))
    let c = mkCamera (mkPoint 4.0 8.0 16.0) (mkPoint 0.0 0.0 -5.0) (mkVector 0.0 1.0 0.0) 4.0 4.0 4.0 1000 1000
    let ambientLight = mkAmbientLight (fromColor Color.Green) 0.1
    let scene = mkScene [p; affinehappy] [l1; l2; l3] ambientLight c 2
    Util.render' scene (folder, "happy.png") toScreen

  let renderPorsche toScreen =
    let baseporsche = mkPLY "../../../ply/porsche.ply" true
    let t = mergeTransformations
              [rotateY (-Math.PI / 4.0);
               scale 2.0 2.0 2.0;
               translate -2.0 4.5 0.0] in
    let white = fromColor Color.White
    let porsche = mkShape baseporsche (mkMatTexture (mkMaterial white 0.0))
    let affineporsche = transform porsche t in
    let t' = scale 0.5 0.5 0.5
    let l1 = mkLight (mkPoint 12.0 4.0 14.0) white 0.4
    let l2 = mkLight (mkPoint -6.0 2.0 6.0) (fromColor Color.Red) 0.5
    let l3 = mkLight (mkPoint -3.5 12.0 4.0) (fromColor Color.Blue) 0.5
    let p = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.Green) 0.5)))
              (rotateX (System.Math.PI/2.0))
    let c = mkCamera (mkPoint 4.0 8.0 25.0) (mkPoint 0.0 0.0 -5.0) (mkVector 0.0 1.0 0.0) 4.0 4.0 4.0 1000 1000
    let ambientLight = mkAmbientLight (fromColor Color.Green) 0.1
    let scene = mkScene [p; affineporsche] [l1; l2; l3] ambientLight c 2
    Util.render' scene (folder, "porsche.png") toScreen

  let renderHorse toScreen =
    let basehorse = mkPLY "../../../ply/horse.ply" true
    let t = mergeTransformations
              [rotateY (-Math.PI / 4.0);
               scale 20.0 20.0 20.0;
               translate 0.0 6.0 0.0] in
    let white = fromColor Color.White
    let horse = mkShape basehorse (mkMatTexture (mkMaterial white 0.0))
    let affinehorse = transform horse t in
    let t' = scale 0.5 0.5 0.5
    let l1 = mkLight (mkPoint 6.0 2.0 6.0) white 0.5
    let l2 = mkLight (mkPoint -6.0 2.0 6.0) (fromColor Color.Red) 0.5
    let l3 = mkLight (mkPoint -8.5 40.0 4.0) white 1.0
    let p = transform (mkPlane (mkMatTexture (mkMaterial (fromColor Color.Green) 0.5)))
              (rotateX (System.Math.PI/2.0))
    let c = mkCamera (mkPoint 4.0 8.0 25.0) (mkPoint 0.0 0.0 -5.0) (mkVector 0.0 1.0 0.0) 4.0 4.0 4.0 1000 1000
    let ambientLight = mkAmbientLight (fromColor Color.Green) 0.1
    let scene = mkScene [p; affinehorse] [l1; l2; l3] ambientLight c 2
    Util.render' scene (folder, "horse.png") toScreen


   

  let render toScreen =
    renderBunny toScreen
    renderDragon toScreen
    renderHappy toScreen
    renderPorsche toScreen
    renderHorse toScreen