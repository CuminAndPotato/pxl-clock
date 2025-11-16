#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui



// TODO: We have bug in here, so please be patient - we will fix it soon
// and update the repository. Thank you!
let finalScene =
    scene {
        shaderGenerative (fun p ->
            if p.x % 2.0 = 0 
            then Colors.red
            else Colors.blue
        )

        text.var4x5("abc").color(Colors.white)

        shaderEffect (fun inp ->
            if inp.pxlColor = Colors.red
            then Colors.green
            else inp.pxlColor
        )
    }


finalScene |> Simulator.start "localhost"


(*
Simulator.stop ()
*)

