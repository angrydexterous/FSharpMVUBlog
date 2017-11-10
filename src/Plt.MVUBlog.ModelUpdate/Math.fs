namespace Plt.MVUBlog.ModelUpdate

open Aether
module Math =
    // Taken from
    // https://jeremybytes.blogspot.ca/2016/07/getting-prime-factors-in-f-with-good.html
    let primeFactors (n:int64) =
        let rec getFactor num proposed acc =
            if proposed = num then
                proposed::acc
            elif num % proposed = 0L then
                getFactor (num/proposed) proposed (proposed::acc)
            else
                getFactor num (proposed+1L) acc
        match n with
        | x when x <= 0L -> [0L]
        | 1L -> [1L]
        | _ -> getFactor n 2L []
