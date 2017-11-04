namespace Plt.MVUBlog.ModelUpdate

module Math = 
    // Taken from 
    // https://jeremybytes.blogspot.ca/2016/07/getting-prime-factors-in-f-with-good.html
    let primeFactors n =
        let rec getFactor num proposed acc =
            if proposed = num then
                proposed::acc
            elif num % proposed = 0 then
                getFactor (num/proposed) proposed (proposed::acc)
            else
                getFactor num (proposed+1) acc
        getFactor n 2 []                      