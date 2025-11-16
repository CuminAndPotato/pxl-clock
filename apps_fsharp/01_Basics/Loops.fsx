#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui




// Iterating and yielding (stateful) components
// also works in a nested way.
scene {
    bg(Colors.darkBlue)

    for x in 0.0 .. 4.0 do
        for y in 0.0 .. 4.0 do
            // we can even have state inside loops
            let! dx = Anim.linear(1.0, 0.0, 10.0, repeat = Repeat.Loop)
            pxl.xy(x * 4.0 + dx.value, y * 4.0).stroke(Colors.blue)
}
|> Simulator.start "localhost"



(*
Simulator.stop ()
*)
