#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



// and some text!
scene {
    let y row = (Fonts.mono3x5.height + 2.0) * row

    text.mono3x5("You").y(y 0).color(Colors.white)
    text.mono3x5("love").y(y 1).color(Colors.white)
    text.mono6x6("PXL").y(y 2).color(Colors.white)
}
|> Simulator.start "localhost"


(*
Simulator.stop ()
*)
