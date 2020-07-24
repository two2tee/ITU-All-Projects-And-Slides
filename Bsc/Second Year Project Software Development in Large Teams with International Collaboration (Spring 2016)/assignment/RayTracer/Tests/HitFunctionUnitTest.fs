//module HitFunctionUnitTest
//
//open System
//
//module HitFunctionTests = 
//
//    open Xunit
//    open Swensen.Unquote
//    open RayTracer//.HitFunction
//    
//    module secondDegreeUnitTest = 
//
//        [<Theory>]
//        [<InlineData(3.0, 50.0, 10.0, -16.46420728)>]
//        let ``hit function returns correct result when discriminant is positive with two hits`` (a: float) (b: float) (c: float) (result: Some float) = 
//            let actualSolution = HitFunction.sndDegree a b c 
//            let expectedResult = result 
//            test <@ expectedResult = actualSolution @>
//    
//
////    //a = 3.0 b = 50.0 c = 10.0 should -> -16.46420728
//////Solves second degree polynomium
////let sndDegree a b c =
////    let disc = b*b - (4.0*a*c)
////    if disc < 0.0
////    then None
////    elif disc = 0.0
////    then Some ((-1.0*b) / 2.0*a)
////    else
////        let dist1 = (-1.0*b + System.Math.Sqrt(disc))/(2.0*a)
////        let dist2 = (-1.0*b - System.Math.Sqrt(disc))/(2.0*a)
////        if dist1 < dist2
////        then Some dist1
////        else
////            Some dist2
//
