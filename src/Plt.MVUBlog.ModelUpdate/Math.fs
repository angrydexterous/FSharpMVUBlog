namespace Plt.MVUBlog.ModelUpdate

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
        if n > 0L then getFactor n 2L [] else [0L]
