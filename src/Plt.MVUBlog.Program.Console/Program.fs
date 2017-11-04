module Plt.MVUBlog.Program.Executable

open System
open System.Threading
open Plt.MVUBlog.ModelUpdate.Library
open Plt.MVUBlog.Program.Console.Utils

let readConsole dispatch = async {
    Console.BackgroundColor <- ConsoleColor.Cyan
    Console.ForegroundColor <- ConsoleColor.Red
    Console.ReadLine() |> fun i -> printfn "input %s" i; i
    |> function
       | Regex @"Add (.*)" [x] -> Operation (Add, int x)
       | Regex @"Sub (.*)" [x] -> Operation (Sub, int x)
       | Regex @"Mul (.*)" [x] -> Operation (Mul, int x)
       | "Factors On" -> ShowPrimes true
       | "Factors Off" -> ShowPrimes false
       | "Reset" -> Reset
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
            printfn "Primes %O" xs
        | None -> ()        
        readConsole dispatch |> Async.Start


    let agent init update view = 
        MailboxProcessor.Start(fun inbox ->
            view init <| inbox.Post
            let rec waitForMsg model = async {
                let! msg = inbox.Receive()
                return! 
                    waitForMsg <| 
                        let m = update msg model
                        view m <| inbox.Post
                        m }
            waitForMsg init)
    agent init update view |> ignore
    
    holdOpen.WaitOne() |> ignore
    0 // return an integer exit code
