#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



scene {
    let! ctx = getCtx()

    for x in 0.0 .. ctx.width do
        for y in 0.0 .. ctx.height do
            let xrel = float x / float ctx.width
            let yrel = float y / float ctx.height
            pxl.xy(x, y).stroke(Colors.blue.opacity(xrel * yrel))

    pxl.xy(0, 0).stroke(Colors.red).noAntiAlias()
    pxl.xy(ctx.width - 1.0, 0).stroke(Colors.blue)
    pxl.xy(0, ctx.height - 1.0).stroke(Colors.green)
    pxl.xy(ctx.width - 1.0, ctx.height - 1.0).stroke(Colors.yellow)
}
|> Simulator.start "localhost"


(*
Simulator.stop ()
*)
