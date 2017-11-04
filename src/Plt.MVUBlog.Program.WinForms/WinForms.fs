module Plt.MVUBlog.Program.WinForms

open System
open System.Threading
open System.Windows.Forms
open System.Drawing
open Plt.MVUBlog.ModelUpdate.Library

type View() as form = 
    inherit Form()
    let mutable oldEventHndl = List<Control*EventHandler>.Empty
    let clearEvents () = 
        oldEventHndl |> List.iter (fun (c,hndl) -> c.Click.RemoveHandler(hndl))
        oldEventHndl <- []
    
    let addEvent (c:Control) cont = 
        let hndl = new EventHandler(cont)
        c.Click.AddHandler(hndl)
        oldEventHndl <- List.append oldEventHndl [c,hndl]

    // let setClickHandler = 
    let tlp = new TableLayoutPanel()
    let valueLabel = new Label()
    let inputBox = new TextBox()
    let valueBox = new TextBox()
    let factorsBox = new TextBox()
    let add = new Button()
    let sub = new Button()
    let mul = new Button()
    let factors = new Button()
    let reset = new Button()
    
    do form.InitializeForm

    // member definitions
    member this.InitializeForm =
        this.FormBorderStyle <- FormBorderStyle.Sizable
        this.Text <- "MVU Blog"
        this.Width <- 600
        tlp.Dock <- DockStyle.Fill
        tlp.ColumnCount<-5
        tlp.RowCount<-5

        valueLabel.Text <- "Input"
        valueBox.ReadOnly <- true
        factorsBox.ReadOnly <- true
        add.Text <- "Add"
        sub.Text <- "Sub"
        mul.Text <- "Mul"
        factors.Text <- "Factors"
        factorsBox.Dock <- DockStyle.Fill
        reset.Text <- "Reset'"
        // Add controls to form
        tlp.Controls.Add(valueLabel,1,1)
        tlp.Controls.Add(inputBox,1,2)
        tlp.Controls.Add(valueBox,1,3)
        tlp.Controls.Add(factorsBox,2,3)
        tlp.SetColumnSpan(factorsBox,3)
        tlp.Controls.Add(add,1,4)
        tlp.Controls.Add(sub,2,4)
        tlp.Controls.Add(mul,3,4)
        tlp.Controls.Add(factors,4,4)
        tlp.Controls.Add(reset,5,4)
        this.Controls.Add(tlp)

    member this.view (model,dispatch) = 
        clearEvents()
        valueBox.Text <- string model.Value
        addEvent add (fun _ _ -> Operation (Add, int64 inputBox.Text) |> dispatch)
        addEvent sub (fun _ _ -> Operation (Sub, int64 inputBox.Text) |> dispatch)
        addEvent mul (fun _ _ -> Operation (Mul, int64 inputBox.Text) |> dispatch)
        match model.Primes with
        | Some fs -> 
            addEvent factors (fun _ _ -> ShowPrimes false |> dispatch)
            factorsBox.Text <- List.fold (fun acc s -> acc + (string s) + ",") "" fs
                               |> fun s -> s.TrimEnd(',')
                               |> sprintf "Primes : [%s]" 
        | None -> 
            addEvent factors (fun _ _ -> ShowPrimes true |> dispatch)
            factorsBox.Text <- ""
        addEvent reset (fun _ _ -> Reset |> dispatch)
        inputBox.Text <- string 0
        ()     

[<EntryPoint>]
[<STAThread>]
let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
 
    use frm = new View()
    let view model dispatch = frm.view (model,dispatch)
    
    runner init update view |> ignore
    
    Application.Run(frm);
    0 // return an integer exit code
