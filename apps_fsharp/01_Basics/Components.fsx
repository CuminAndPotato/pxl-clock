#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui




// ...we can also make it reusable
let counter(x, y, color) =
    scene {
        let! count = useState { 0 }
        text.mono6x6($"{count.value}").xy(x, y).color(color)
        do count.value <- count.value + 1
    }

// and actually use it over a nice background:
let finalScene = 
    scene {
        bg(Colors.darkBlue)
        counter(0, 0, Colors.white)
        counter(0, 8, Colors.red)
        counter(0, 16, Colors.green)
    }

finalScene |> Simulator.start "localhost"


(*
Simulator.stop ()
*)
