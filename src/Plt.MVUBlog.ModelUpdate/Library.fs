namespace Plt.MVUBlog.ModelUpdate

module Library = 
    open Aether
    open Math

    type Op = | Add
              | Sub
              | Mul

    type Msg = | Operation of Op*value:int
               | ShowPrimes of bool
               | Reset

    type Model = { Value : int 
                   Primes : int list option } with
        static member Value_ = 
            (fun a -> a.Value), (fun b a -> { a with Value = b })
        static member Primes_ = 
            (fun a -> a.Primes), (fun b a -> { a with Primes = b })

    let init = { Value = 0 
                 Primes = None}

    let update msg model =
        match msg with
        | Operation (op,x) ->
            match op with
            | Add -> Optic.set Model.Value_ (model.Value + x) model
            | Sub -> Optic.set Model.Value_ (model.Value - x) model
            | Mul -> Optic.set Model.Value_ (model.Value * x) model
        | ShowPrimes b -> Optic.set Model.Primes_ (if b then Some List.empty else None) model
        | Reset -> { model with Value = 0 } 
        |> fun m -> (Option.map (fun _ -> primeFactors m.Value) m.Primes, m)
        ||> Optic.set Model.Primes_