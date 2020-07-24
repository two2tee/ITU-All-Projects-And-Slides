module HitFunctionTest

open System

module HitFunctionTests = 

    open Xunit
    open Swensen.Unquote
    open RayTracer //.HitFunction
    open PolynomialFormulas
    
    module secondDegreeUnitTest = 

        [<Theory>]
        [<InlineData(3.0, 50.0, 10.0, -16.46420728)>]
        let ``hit function returns correct result when discriminant is positive with two hits`` (a: float) (b: float) (c: float) (result: float option) = 
            let actualSolution = solveSecondDegree a b c 
            let expectedResult = result 
            test <@ expectedResult = actualSolution @>
    
