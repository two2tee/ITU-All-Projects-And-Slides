namespace RayTracer
open System
open Vector.Generic //Should this be generic?
module BoundingBox =
    
    type Point = SceneAssets.Point
    type Vector = Vector.Vector
    type BoundingBox = SceneAssets.BoundingBox
    type Ray = SceneAssets.Ray


    //======================Help functions======================


    //Folds over a list of floats, continuously compares them and finds the minimum point starting at infinity
    let findMin (list : float list) = List.fold (fun acc f -> min acc f) ((float)Single.PositiveInfinity) list

    //Folds over a list of floats, continuously compares the values and finds the maximum starting at 0
    let findMax (list : float list) = List.fold (fun acc f -> max acc f) ((float) Single.NegativeInfinity) list


    //epsilon represents a very small float point value
    let epsilon = FloatHelper.ZeroThreshold


    //======================Bounding box functions======================

    

    //--Triangle bounding box--
    //This help function calculates the upper x,y,z point triangle
    let findHigher (a : Point) (b : Point) (c : Point) = 
        let Hx = findMax [Point.getX a; Point.getX b; Point.getX c] + epsilon
        let Hy = findMax [Point.getY a; Point.getY b; Point.getY c] + epsilon
        let Hz = findMax [Point.getZ a; Point.getZ b; Point.getZ c] + epsilon
        Point.mkPoint Hx Hy Hz //Returns the point
    
    //This help function calculates the lower x,y,z point of a triangle 
    let findLower (a : Point) (b : Point) (c : Point) = 
        let Lx = findMin [Point.getX a; Point.getX b; Point.getX c] - epsilon
        let Ly = findMin [Point.getY a; Point.getY b; Point.getY c] - epsilon
        let Lz = findMin [Point.getZ a; Point.getZ b; Point.getZ c] - epsilon
        Point.mkPoint Lx Ly Lz //Returns the point

    //This function calculates the bounding box for a triangle
    let computeTriangleBbox a b c : BoundingBox = 
        let higher = findHigher a b c
        let lower = findLower a b c
        SceneAssets.mkBbox lower higher

    //This function calculates the bounding box for a sphere
    let computeSphereBbox (radius : float) : BoundingBox = 
        let center = Point.mkPoint 0.0 0.0 0.0
        let lower = Point.mkPoint (Point.getX center - radius - epsilon) (Point.getY center - radius - epsilon) (Point.getZ center - radius - epsilon)
        let higher = Point.mkPoint (Point.getX center + radius + epsilon) (Point.getY center + radius + epsilon) (Point.getZ center + radius + epsilon) 
        SceneAssets.mkBbox lower higher
        
    //This function calculates the bounding box for a box
    let computeBoxBbox (lower : Point) (higher : Point) =
        let lower = Point.mkPoint (Point.getX lower - epsilon) (Point.getY lower - epsilon) (Point.getZ lower - epsilon)
        let higher = Point.mkPoint (Point.getX higher + epsilon) (Point.getY higher + epsilon) (Point.getZ higher + epsilon)
        SceneAssets.mkBbox lower higher

    //This function calculates the bounding box for a disc
    let computeDiscBbox  (r : float) : BoundingBox =
        let c = Point.mkPoint 0.0 0.0 0.0
        let lower = Point.mkPoint (Point.getX c - r - epsilon) (Point.getY c - r - epsilon) (Point.getZ c - epsilon)
        let higher = Point.mkPoint (Point.getX c + r + epsilon) (Point.getY c + r + epsilon) (Point.getZ c + epsilon)
        SceneAssets.mkBbox lower higher

    //This function calculates the bounding box for a cylinder
    let computeCylinderBbox (r : float) (h : float) : BoundingBox = 
        let c = Point.mkPoint 0.0 0.0 0.0
        let lower = Point.mkPoint (Point.getX c - r - epsilon) (Point.getY c - h/2.0 - epsilon) (Point.getZ c - r - epsilon)
        let higher = Point.mkPoint (Point.getX c + r + epsilon) (Point.getY c + h/2.0 + epsilon) (Point.getZ c + r + epsilon)
        SceneAssets.mkBbox lower higher
        

    //This function calculates the bounding box for a rectangle
    let computeRectangleBbox (width : float) (height : float) : BoundingBox =
        let bottomLeft = Point.mkPoint 0.0 0.0 0.0
        let lower = Point.mkPoint (Point.getX bottomLeft - epsilon) (Point.getY bottomLeft - epsilon) (Point.getZ bottomLeft - epsilon)
        let higher = Point.mkPoint (Point.getX bottomLeft + width + epsilon) (Point.getY bottomLeft + height + epsilon) (Point.getZ bottomLeft + epsilon)
        SceneAssets.mkBbox lower higher


    //Finds the lowest coordinates of two points
    let findLowestPoint (p1 : Point) (p2 : Point) = 
        let x = if Point.getX p1 > Point.getX p2 then Point.getX p2 else Point.getX p1
        let y = if Point.getY p1 > Point.getY p2 then Point.getY p2 else Point.getY p1
        let z = if Point.getZ p1 > Point.getZ p2 then Point.getZ p2 else Point.getZ p1
        Point.mkPoint x y z

    //Finds the highest coordinates of two points
    let findHighestPoint (p1 : Point) (p2 : Point) = 
        let x = if Point.getX p1 > Point.getX p2 then Point.getX p1 else Point.getX p2
        let y = if Point.getY p1 > Point.getY p2 then Point.getY p1 else Point.getY p2
        let z = if Point.getZ p1 > Point.getZ p2 then Point.getZ p1 else Point.getZ p2
        Point.mkPoint x y z


    //This function takes a list of BoundingBoxes and based on their coords,
    //a bounding box of the entire scene is calculated.
    let computeSceneBbox  (bboxes : BoundingBox list) : BoundingBox = 
        let startPoint = (Point.mkPoint 0.0 0.0 0.0)
        let inf = (float) Single.PositiveInfinity
        let lowerPoint = List.fold (fun point (bbox:BoundingBox) -> findLowestPoint point bbox.lower) (Point.mkPoint inf inf inf) bboxes
        let higherPoint = List.fold (fun point (bbox:BoundingBox) -> findHighestPoint point bbox.higher) (Point.mkPoint 0.0 0.0 0.0) bboxes
        SceneAssets.mkBbox lowerPoint higherPoint

    let computeSceneBboxArr  (bboxes : BoundingBox[]) : BoundingBox = 
        let startPoint = (Point.mkPoint 0.0 0.0 0.0)
        let inf = (float) Single.PositiveInfinity
        let lowerPoint = Array.fold (fun point (bbox:BoundingBox) -> findLowestPoint point bbox.lower) (Point.mkPoint inf inf inf) bboxes
        let higherPoint = Array.fold (fun point (bbox:BoundingBox) -> findHighestPoint point bbox.higher) (Point.mkPoint 0.0 0.0 0.0) bboxes
        SceneAssets.mkBbox lowerPoint higherPoint

      //---- Ray intersection with bounding box ----


    //These two function will find the min and max  value based on 3 given values
    let findMaxCoordinate (x : float) (y : float) (z : float) = Math.Max(Math.Max (x, y),z)
    let findMinCoordinate (x : float) (y : float) (z : float) = Math.Min(Math.Min (x, y),z)
    
    //This function is a generic function that is used to calculate tx, ty and tz values for intersection with bbox computation
    let calculateTValue o H L d =  
        if(d >= 0.0) then //Based on the direction of a ray. 
            let t = (L - o)/d
            let t' = (H - o)/d
            (t,t')
        else
            let t = (H - o)/d
            let t' = (L - o)/d
            (t,t')


    let getBboxIntersection (bbox : BoundingBox) (ray : Ray) =
        //Defining all the variables
        let ox = Point.getX ray.origin // o is the origin point of a ray
        let oy = Point.getY ray.origin
        let oz = Point.getZ ray.origin

        let (dx,dy,dz) = Vector.getCoord (ray.direction) // d is the direction of the ray

        let Hx = Point.getX bbox.higher //H is the higher point of a bounding box
        let Hy = Point.getY bbox.higher
        let Hz = Point.getZ bbox.higher

        let Lx = Point.getX bbox.lower //L is the lower point of a bounding box
        let Ly = Point.getY bbox.lower
        let Lz = Point.getZ bbox.lower

        //Calculates t values (used to check if there is a hit)
        let txvalues = calculateTValue ox Hx Lx dx 
        let tx = fst(txvalues)
        let tx' = snd(txvalues)

        let tyvalues = calculateTValue oy Hy Ly dy
        let ty = fst(tyvalues)
        let ty' = snd(tyvalues)

        let tzvalues = calculateTValue oz Hz Lz dz
        let tz = fst(tzvalues)
        let tz' = snd(tzvalues)

        //Getting the smalles and largest t value
        let t = findMaxCoordinate tx ty tz
        let t' = findMinCoordinate tx' ty' tz'

        //Compare T values to check if there is a hit with the given bounding box
        let isHit = t < t' && t'> 0.0
        (isHit,t,t')


    //This function will check if a ray intersects with a given bounding box
    let checkBboxIntersection (bbox : BoundingBox) (ray : Ray) =
        let (isHit, t, t') = getBboxIntersection bbox ray
        isHit

    
    