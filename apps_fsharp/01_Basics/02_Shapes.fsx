#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui



// ...or some rectangles ...
scene {
    let! ctx = getCtx()

    rect.xywh(2, 2, 20, 20).fill(Colors.blue)
    rect.xywh(4, 4, 16, 16).fill(Colors.white)

    // oval.xywh(10, 10, 20, 20).fill(Colors.red)

    // polygon
    //     .define(fun path ->
    //         path.MoveTo(f32 ctx.halfWidth + 0.5f, f32 ctx.height)
    //         path.LineTo(0f, 0f)
    //         path.LineTo(0f, f32 ctx.height)
    //         path.Close()
    //     )
    //     .fill(Colors.gray)

}
|> Simulator.start "localhost"


(*
Simulator.stop ()
*)
