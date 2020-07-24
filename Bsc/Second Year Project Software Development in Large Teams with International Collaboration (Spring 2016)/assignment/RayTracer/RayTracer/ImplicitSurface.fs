module ImplicitSurface
    open RayTracer.SceneAssets
    open RayTracer.Shape
    open ExprParse
    open ExprToPoly
    open PolynomialFormulas

    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = RayTracer.Shape.Ray
    type Material = RayTracer.SceneAssets.Material
    type IShape = RayTracer.Shape.IShape
    type BoundingBox = BoundingBox
    type Texture = RayTracer.SceneAssets.Texture

    //Expression tree from substituting x, y & z with ray direction and origin point values.
    let implicitSubstExpr (expression:expr) =
        let ex = FAdd(FVar "px", FMult(FVar "t", FVar "dx"))
        let ey = FAdd(FVar "py", FMult(FVar "t", FVar "dy"))
        let ez = FAdd(FVar "pz", FMult(FVar "t", FVar "dz"))
        
        let subX = subst expression ("x", ex)
        let subY = subst subX ("y", ey)
        subst subY ("z", ez)

    //The substituting of direction- & originpoint-variables with values from param ray values, in the param expr.
    let insertDirOriValues (ray : Ray) (ex:expr) =
        let dex = subst ex ("dx", FNum (Vector.getX ray.direction))
        let dey = subst dex ("dy", FNum (Vector.getY ray.direction))
        let dez = subst dey ("dz", FNum (Vector.getZ ray.direction))
        let pex = subst dez ("px", FNum (Point.getX ray.origin))
        let pey = subst pex ("py", FNum (Point.getY ray.origin))
        subst pey ("pz", FNum (Point.getZ ray.origin))
    
    let findNormal (e:expr) s v =
        let ex = match s with
                 | "x" -> subst (deriveExpr (subst (subst e ("y", FInt 0)) ("z", FInt 0))) ("x", FNum v)
                 | "y" -> subst (deriveExpr (subst (subst e ("x", FInt 0)) ("z", FInt 0))) ("y", FNum v)
                 | "z" -> subst (deriveExpr (subst (subst e ("y", FInt 0)) ("x", FInt 0))) ("z", FNum v)
        getExprValue (simplifyToExpr ex)

    //Creates IShape given param s (polynomial). Uses mat yellow texture.
    let mkImplicit (s:string) (texture:Texture) =
        let expression = s |> scan |> insertMult |> parse
        let subZ = implicitSubstExpr expression |> simplifyToExpr
        let dgr = getExprDegree subZ 0

        let implicit =
            {
                new IShape with
                    member this.HitFunc (ray : Ray) =
                        let finalExpr = insertDirOriValues ray subZ
                        let dist = match dgr with
                                   | 0 -> None //Not a shape.
                                   | 1 -> solveFirstDgr finalExpr
                                   | 2 -> let poly = exprToPoly finalExpr "t"
                                          solveSecondDegreeP poly
                                   | _ -> None //not working
//                                          match sturmSeq finalExpr with
//                                          | Some (s, e) -> newtonsMethod finalExpr (deriveExpr finalExpr) (s + ((e - s)/2.0)) 0 //not implemented fully.
//                                          | None -> None
                       
                        match dist with
                        | Some f when f >= 0.0 -> 
                                   let hitPoint = Point.move ray.origin (Vector.multScalar ray.direction dist.Value)
                                   let normal = Vector.mkVector (findNormal finalExpr "x" (Point.getX hitPoint)) (findNormal finalExpr "y" (Point.getY hitPoint)) (findNormal finalExpr "z" (Point.getZ hitPoint))
                                   (dist,Some(hitPoint), Some(normal),(fun () -> getTextureColour texture (0.0,0.0)))
                        | _ -> (None, None, None,(fun () -> getTextureColour texture (0.0,0.0)))

                    member this.Bbox = mkBbox (Point.mkPoint 0.0 0.0 0.0) (Point.mkPoint 0.0 0.0 0.0)

                    member this.HitBbox (ray : Ray) = failwith "Not imeplemented."

                    member this.IsInside p = failwith "Shape is not solid!"
            }
        implicit
