namespace RayTracer 

open System
open Point
open BoundingBox
open SceneAssets

//The module represents the KD-tree and contains its functions
module KDTree =
    type Shape = SceneAssets.IShape
    type Scene = SceneAssets
    type BoundingBox = RayTracer.BoundingBox.BoundingBox
    type Point = Point.Point
    type Vector = Vector.Vector
    type Ray = SceneAssets.Ray
    type Material = SceneAssets.Material

    type Tree =
        | Leaf of Shape list
        | Node of int * float * Tree * Tree
  
    //---- Help Functions ----

    //Orders tuple based on d
    let order (d, left, right) =
        if d > 0.0
        then (left,right)
        else (right,left)

    //Retrieves a tuple stored in a node
    let getNodeTuple node = 
        match node with
        | Node(dim,value,left,right) -> (dim, value, left, right)
        | Leaf l -> failwith "Can't return tuple from Leaf"


    //Compute bounding box of whole tree
    let computeTreeBbox (shapes : Shape list) : BoundingBox = 
        let startPoint = (Point.mkPoint 0.0 0.0 0.0)
        let inf = (float) Single.PositiveInfinity
        let negInf = (float) Single.NegativeInfinity
        let lowerPoint = List.fold (fun point (shape : Shape)-> findLowestPoint point shape.Bbox.lower) (Point.mkPoint inf inf inf) shapes
        let higherPoint = List.fold (fun point (shape : Shape) -> findHighestPoint point shape.Bbox.higher) (Point.mkPoint negInf negInf negInf) shapes
        mkBbox lowerPoint higherPoint


    //Gets coordinate based on what dimension to retrieve
    let getCoordByDimension (dimension : int) =
        match dimension with
        | 1 -> Point.getX
        | 2 -> Point.getY
        | 3 -> Point.getZ
        | _ -> failwith "Invalid dimension"

    //Get coordinate from vector by a given dimension
    let getVectorCoord (dimension : int) =
        match dimension with
        | 1 -> Vector.getX
        | 2 -> Vector.getY
        | 3 -> Vector.getZ
        | _ -> failwith "Invalid dimension"

    //Calculates the split coordinate by calculating a mean based on a given dimension and a list of bounding boxes (Used to find the split point)
    let findMean (dimension : int) (shapes : Shape list) = 
        let sumOfCoordinates = (List.fold (fun sum (shape : Shape) -> sum + (getCoordByDimension dimension (shape.Bbox.lower) + getCoordByDimension dimension (shape.Bbox.higher))/2.0) 0.0 shapes)
        let mean = sumOfCoordinates/ float (shapes.Length)
        mean


    //Calculate longest dimension of a given bounding box 
    let findLargestDimension (bbox : BoundingBox.BoundingBox) =
     //Calculate dimension size
        let xDim = Point.getX bbox.higher - Point.getX bbox.lower
        let yDim = Point.getY bbox.higher - Point.getY bbox.lower
        let zDim = Point.getZ bbox.higher - Point.getZ bbox.lower
        let x = 1
        let y = 2
        let z = 3

        //Compares dimensions
        if(xDim >= yDim && xDim >= zDim)        //return x
            then x
        else if (yDim >= xDim && yDim >= zDim)  //return y
            then y
        else                                    //return z
                 z

    //Distribute shapes based on their bounding box and mean
    let rec distributeBboxes (dimension : int) (mean : float) (shapes : Shape list) = 
        List.fold (fun (lower, both, upper) (shape : Shape)
                        -> 
                           let higherBboxPoint = getCoordByDimension dimension shape.Bbox.higher
                           let lowerBboxPoint = getCoordByDimension dimension shape.Bbox.lower
                        
                           if      higherBboxPoint < mean  then   (shape :: lower, both, upper) //Shape is below mean
                           else if lowerBboxPoint > mean   then   (lower, both, shape :: upper) //Shape is above mean
                           else                                   (shape :: lower, both + 1, shape :: upper) //Shape is inbetween
                           ) ([], 0, []) shapes
    
    //Finds the closest coordinate above and below the mean - Used to calculate innerEmptySpace
    let rec findInnerEmptySpaceCoords (dimension : int) (mean : float) (shapes : Shape list) =
            List.fold (fun (lowerMax, upperMin) (shape : Shape)
                        -> 
                           //Find the two points that are closest to the mean above an below
                           let upperBboxPoint = getCoordByDimension dimension shape.Bbox.higher
                           let lowerBboxPoint = getCoordByDimension dimension shape.Bbox.lower

                           //Shape is below the mean
                           if (upperBboxPoint < mean) then
                                let newLowerMax = if upperBboxPoint > lowerMax 
                                                  then upperBboxPoint
                                                  else lowerMax
                                (newLowerMax, upperMin)

                           //Shape is above the mean
                           else if  (lowerBboxPoint > mean) then 
                                let newUpperMin = if lowerBboxPoint < upperMin 
                                                  then lowerBboxPoint
                                                  else upperMin
                                (lowerMax, newUpperMin)
                         
                           //Shape is in between the mean 
                           else 
                                let newLowerMax = if lowerMax < upperBboxPoint //upper point becomes lower max
                                                  then upperBboxPoint
                                                  else lowerMax
                                let newUpperMin = if upperMin > lowerBboxPoint //lower point becomes upper min
                                                    then lowerBboxPoint
                                                    else upperMin

                                (newLowerMax, upperMin)) ((float) Single.NegativeInfinity, (float) Single.PositiveInfinity) shapes 
    
    //Creates upper and lower child bounding boxes based on dimensions
    let findChildBboxes dimension mean (outerBbox : BoundingBox.BoundingBox) = 
        match dimension with
        //X axis
        | 1 -> ((mkBbox //Lower Bbox
                                    //Lower Point
                                    (outerBbox.lower)
                                    //Higher Point
                                    (Point.mkPoint 
                                        mean 
                                        (Point.getY outerBbox.higher) 
                                        (Point.getZ outerBbox.higher))), 

                (mkBbox //Upper Bbox
                                    //Lower Point
                                    (Point.mkPoint 
                                        mean 
                                        (Point.getY outerBbox.lower) 
                                        (Point.getZ outerBbox.lower))
                                        //Higher Point
                                    (outerBbox.higher)))
        //Y axis
        | 2 -> ((mkBbox //Lower Bbox
                                    //Lower Point
                                    (outerBbox.lower)
                                    //Higher Point)
                                    (Point.mkPoint 
                                        (Point.getX outerBbox.higher) 
                                        mean
                                        (Point.getZ outerBbox.higher))),

                (mkBbox //Upper Bbox
                                    //Lower Point
                                    (Point.mkPoint 
                                        (Point.getX outerBbox.lower) 
                                        mean 
                                        (Point.getZ outerBbox.lower)) 
                                    //Higher Point
                                    (outerBbox.higher)))
        //Z axis
        | 3 -> ((mkBbox //Lower Bbox
                                    //Lower Point
                                    (outerBbox.lower)
                                    //Higher Point
                                    (Point.mkPoint 
                                        (Point.getX outerBbox.higher) 
                                        (Point.getY outerBbox.higher) 
                                        mean)),

                (mkBbox //upper Bbox
                                    //Lower Point
                                    (Point.mkPoint 
                                        (Point.getX outerBbox.lower) 
                                        (Point.getY outerBbox.lower)
                                        (mean)) 
                                        //Higher Point
                                    (outerBbox.higher)))

        | _ -> failwith "Invalid dimension"

    //Finds the outer empty space
    let getOuterEmptySpace (outerBbox : BoundingBox.BoundingBox) (shapes : Shape list) dimension =
        let (l, h) = List.fold (fun (lowerOuter, higherOuter) (shape : Shape) ->
                                   let bboxLower = getCoordByDimension dimension shape.Bbox.lower
                                   let bboxOuter = getCoordByDimension dimension shape.Bbox.higher
                                   let lower = if bboxLower < lowerOuter then bboxLower
                                               else lowerOuter
                                   let outer = if bboxOuter > higherOuter then bboxOuter
                                               else higherOuter
                                   (lower, outer)
                                    ) ((float)Single.PositiveInfinity, (float) Single.NegativeInfinity) shapes
        let lowerDist = l - getCoordByDimension dimension outerBbox.lower
        let higherDist = getCoordByDimension dimension outerBbox.higher - h
        (lowerDist, l, higherDist, h)


    //---- HEURISTICS ----
    let splitHeuristic = 0.60 //Heuristic percentage used to check splitting.
    let emptySpaceHeuristic = 0.10//Heuristic percentage used to cut off empty space.
    // -------------------

    //Move the current mean so that it takes inner empty space into account and return the new position
    let computeNearestMean lowerMax upperMin mean bboxLength =
        let lowerDistRatio = (mean-lowerMax)/bboxLength
        let upperDistRatio = (upperMin - mean)/bboxLength
        match (lowerDistRatio, upperDistRatio) with
                           | (lowerDist, upperDist) 
                                when  lowerDistRatio > emptySpaceHeuristic && lowerMax < mean && lowerDistRatio >= upperDistRatio
                                    -> lowerMax
                           | (lowerDist, upperDist)
                                when upperDistRatio > emptySpaceHeuristic && upperMin > mean
                                    -> upperMin
                           | _ -> mean

    //Calculate the ratio for how many shapes that overlaps the mean
    let calculatePlacementRatios lowerCount upperCount bothCount = 
        let bl = (float bothCount/float lowerCount)
        let br = (float bothCount/float upperCount)
        (bl, br)



    //Main split function
    let rec splitOnDimension (outerBbox : BoundingBox) (shapes : Shape list) c =
       
        //Finds the dimension, which is largest (this is the one we split on)
        let largestDimension = findLargestDimension outerBbox  
        let dimLength = (getCoordByDimension largestDimension outerBbox.higher - getCoordByDimension largestDimension outerBbox.lower)

        //Computes the empty space that there is from the edges of the outer bbox to the closest shapes.
        let (lowerDist, lower, higherDist, higher) = getOuterEmptySpace outerBbox shapes largestDimension
        //If the distance from the lower end of the bbox is larger than the distance from the higher end
        //and it satisfies the heuristics, then split on this, leaving empty space on the right.
        if (lowerDist/dimLength) >= (higherDist/dimLength) && lowerDist >= emptySpaceHeuristic then
            let splitValue = lower - FloatHelper.ZeroThreshold
            let (bboxLower, bboxHigher) = findChildBboxes largestDimension splitValue outerBbox
            splitOnDimension bboxHigher shapes (fun leftTree -> c (Node(largestDimension, splitValue, Leaf (List.Empty), leftTree)))  

        //Else if the distance from the higher end is largest and satisfies heuristics, split on this value
        //leaving empty space to the left.
        else if ((higherDist/dimLength) >= emptySpaceHeuristic) then
            let splitValue = higher + FloatHelper.ZeroThreshold
            let (bboxLower, bboxHigher) = findChildBboxes largestDimension splitValue outerBbox
            splitOnDimension bboxLower shapes (fun rightTree -> c (Node(largestDimension, splitValue, rightTree, Leaf (List.Empty))))
        else
        //If the empty space is not large enough to satisfy heuristics, split normally
            splitOnMean largestDimension shapes outerBbox dimLength c


    and splitOnMean largestDimension shapes outerBbox dimLength c = 
            //Returns the split value based on where the bboxes are situated (the mean)
            let mean = findMean largestDimension shapes

            //Calculates which shapes lie within the lower half, upper half and a count of how many lie within both.
            //In addition, returns the point of the shapes that lie closest to the mean on either side of the mean
            let (lowerShapes : Shape list, bothCount, upperShapes : Shape list) = distributeBboxes largestDimension mean shapes
            let (lowerMax, upperMin) = findInnerEmptySpaceCoords largestDimension mean shapes

            //Computes the ratio of how many shapes lie in the lower and upper half compared to how many lie in both
            let (blRatio, brRatio) = calculatePlacementRatios lowerShapes.Length upperShapes.Length bothCount

            //Avoid dividing along a plane which causes many overlaps (i.e. with many shapes on both sides)
            if (blRatio > splitHeuristic || brRatio > splitHeuristic)
                then c (Leaf(shapes))
            else 
                //Calculates the best mean, taking empty space ito account
                let closestMean = computeNearestMean lowerMax upperMin mean dimLength
                //Calculates bboxes of the areas split by the mean
                let (lowerBbox, upperBbox) = findChildBboxes largestDimension closestMean outerBbox
                                    
                //Calculates KDTree based on above calculations
                splitOnDimension lowerBbox lowerShapes 
                    (fun rightTree -> splitOnDimension upperBbox upperShapes (fun leftTree -> c (Node(largestDimension, closestMean, rightTree, leftTree))))                            
                //Node(largestDimension, closestMean, splitOnDimension lower (lowerShapes), splitOnDimension upper (upperShapes))
    
                
//----- tree traversal functions ------//
    
    let closestHit (leaf : Tree) (ray:Ray) = 
        match leaf with 
        | Leaf shapes -> let possibleHits = List.map(fun (s : Shape)-> s.HitFunc ray) shapes //make the hitFunc for all shapes
                         let filteredHits = List.filter(fun ((d: float option),_,_,_) -> d.IsSome ) possibleHits
                         let sortedHits = List.sortBy(fun ((d: float option),_,_,_) -> d.Value ) filteredHits // sort in ascending order
                         match sortedHits with
                         |(Some dist,Some hit,Some normal,mat)::_ when dist>FloatHelper.ZeroThreshold ->
                            Some(dist, hit,normal,mat()) //return first hit if any
                         |_ ->  None
        | _ -> failwith "Expects a leaf but the given param is not a leaf"
        
    let isLeaf (node : Tree) = 
        match node with
        | Leaf l -> true
        | _ -> false

    let searchLeaf leaf (ray:Ray) (t':float) = 
        let hit = closestHit leaf ray
        match hit with
        | Some (dist, hit, normal, mat) -> Some (hit, normal, mat)
        | _ -> None

            
    let rec traverse (tree:Tree) (bbox:BoundingBox.BoundingBox) ray = 
            let (intersects,t,t') = BoundingBox.getBboxIntersection bbox ray //Returns a tuple of (bool,t,t') //check bounding box of tree t and 't is the distance to enter and exit
            if intersects then search tree ray t t'
            else None
    and search node (ray:Ray) (t:float) (t':float) =
        if isLeaf node 
        then 
            searchLeaf  node ray t'
        else
            searchNode node ray t t'

    and searchNode node (ray:Ray) t t' = 
        let (axis,value,left,right) = getNodeTuple node
        let vectorCoordDim = getVectorCoord axis (ray.direction)
        let originCoordDim = getCoordByDimension axis ray.origin 
        if vectorCoordDim = 0.0 then 
            if originCoordDim <= value then
                search left ray t (t')
            else
                search right ray t t'
        else
            let tHit = (value - originCoordDim) / vectorCoordDim
            let (fst, snd) = order(vectorCoordDim, left, right) 
            if tHit <=  t || tHit < 0.0 then
                search snd ray t t'
            else if tHit >= t' then
                search fst ray t t'
            else 
                match (search fst ray t tHit) with 
                | Some hit -> Some hit
                | None -> search snd ray tHit t'


    //This function builds a KD-tree based on a list of shapes
    //Mainly used for Triangle meshes but is also used for the scene
    let buildKdTree (shapes : Shape list) =
        let kdBbox = computeTreeBbox shapes
        (splitOnDimension kdBbox shapes id, kdBbox)//Cut off empty space, check if it is even necessary to split

   

