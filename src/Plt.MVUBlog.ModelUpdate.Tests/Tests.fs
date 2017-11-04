module Tests

open Expecto
open Plt.MVUBlog.ModelUpdate.Math
open Plt.MVUBlog.ModelUpdate.Library

let baseRecord = init
let valRecord x = { Value = x 
                    Primes = None}

[<Tests>]
let tests =
  testList "MVULibrary" [
    testList "Math" [
      testCase "Test Primes of 521 (only 1)" <| fun _ ->
        let x = primeFactors 521L
        Expect.equal x [521L] "Prime of 521 is 521"
      
      testCase "Test Primes 28 (2)" <| fun _ ->
          let x = primeFactors 28L
          Expect.equal x [7L;2L;2L] "Primes of 28 are 4,7"

      testCase "Test Negative Primes" <| fun _ ->
          let x = primeFactors -1L
          Expect.equal x [0L] "Primes of negative are 0"       
    ]
    
    testList "ModelUpdate" [
      testProperty "Plus/Minus Increment" <| fun a ->
        let operator = if a >= 0 then Add else Sub 
        let res = if a >= 0 then [0..(a-1)] else [(a+1)..0] 
                  |> List.fold (fun model _ -> update (Operation (operator,1L)) model) baseRecord
        res.Value = int64 a

      testProperty "Add" <| fun a ->
        let res = update (Operation (Add,a)) baseRecord
        res.Value = a
      
      testProperty "Sub" <| fun a ->
        let res = update (Operation (Sub,a)) baseRecord
        res.Value = -1L * a      

      testProperty "Mul" <| fun a b ->
        let mulRecord = valRecord b
        let res = update (Operation (Mul,a)) mulRecord
        res.Value = a*b
    ]
  ]