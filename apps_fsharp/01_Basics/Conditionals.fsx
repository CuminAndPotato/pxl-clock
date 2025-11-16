#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui




// It is possible to render views conditionally.
// Please note that in an `else` branch,
// there are only 2 things allowed: `preserveState` and `discardState`.
// They both are required to tell the system if the (eventual) state
// of the components from the `if` branch should be kept for upcoming
// frames or not.
scene {
    let! cycle = Logic.count(0, 1)
    if cycle % 10 = 0 then
        let! innerCount = Logic.count(0, 1)
        text.mono6x6($"{innerCount}").color(Colors.white)
    else preserveState
}
|> Simulator.start "localhost"



(*
Simulator.stop ()
*)
