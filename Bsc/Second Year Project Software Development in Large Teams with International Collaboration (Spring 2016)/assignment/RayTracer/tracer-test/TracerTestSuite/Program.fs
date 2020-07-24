open TracerTestSuite

let renderAll toScreen =
  printfn "Shapes"
  Shapes.render toScreen
  printfn "Affine transforms"
  AffineTransformations.render toScreen
  printfn "Implicit"
  ImplicitSurfaces.render toScreen
  printfn "Meshes"
  Meshes.render toScreen
  printfn "Texture"
  Texture.render toScreen
  printfn "Light"
  Light.render toScreen
  printfn "CSG"
  CSG.render toScreen


[<EntryPoint>]
let main argv =
    Util.init();
    renderAll false;
    Util.finalize();
    0 // return an integer exit code