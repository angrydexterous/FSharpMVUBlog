module Plt.MVUBlog.Program.Executable

open System
open System.Threading
open Plt.MVUBlog.ModelUpdate.Library
open Plt.MVUBlog.Program.Console.Utils

let readConsole dispatch = async {
    Console.BackgroundColor <- ConsoleColor.Cyan
    Console.ForegroundColor <- ConsoleColor.Red
    Console.ReadLine() |> fun i -> printfn "Processing....(please wait): '%s'" i; i
    |> function
       | Regex @"[a,A]dd (.*)" [x] -> Operation (Add, int64 x)
       | Regex @"[s,S]ub (.*)" [x] -> Operation (Sub, int64 x)
       | Regex @"[m,M]ul (.*)" [x] -> Operation (Mul, int64 x)
       | Regex "[f,F]actors (.*)" [b] -> 
           b.ToLower()
           |> function
              | "on" -> ShowPrimes true
              | "off" ->  ShowPrimes false
              | _ -> failwith "badinput"
       | "Reset" | "reset" -> Reset
       | _ -> failwith "badinput"
    |> dispatch
    return ()
    }

[<EntryPoint>]
let main argv =
    let holdOpen = new AutoResetEvent(false)
    Console.CancelKeyPress.Add(fun args -> args.Cancel <- true; holdOpen.Set() |> ignore)

    let view model dispatch = 
        Console.BackgroundColor <- ConsoleColor.Black
        Console.ForegroundColor <- ConsoleColor.Black
        Console.Clear()
        Console.BackgroundColor <- ConsoleColor.DarkBlue
        printfn "Help: Add x, Sub x, Mul x, Factors On/Off, Reset"
        Console.BackgroundColor <- ConsoleColor.DarkRed
        printfn "Ctrl+c to quit."
        Console.BackgroundColor <- ConsoleColor.Blue
        printfn "Value: %d" model.Value
        match model.Primes with
        | Some xs-> 
            Console.ForegroundColor <- ConsoleColor.DarkRed
            List.fold (fun acc s -> acc + (string s) + ",") "" xs
            |> fun s -> s.TrimEnd(',')
            |> printfn "Primes : [%s]"  
        | None -> ()        
        readConsole dispatch |> Async.Start

    runner init update view |> ignore
    
    holdOpen.WaitOne() |> ignore
    0 // return an integer exit code
