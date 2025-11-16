#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



// a list comprehension yields elements and returns a list of elements
[
    1
    2
    3
    for x in 4 .. 10 do
        x
]


let blinkingRect x y (fill: Color) =
    scene {
        let! rectOpacity = useState { 0.0 }

        rect.xywh(x, y, 12, 12).fill(fill.opacity(rectOpacity.value))
        
        rectOpacity.value <-
            let v = rectOpacity.value + 0.02
            if v > 1.0 then 0.0 else v
    }

scene {
    // bg(Colors.white)

    let! ctx = getCtx()

    pxl.xy(0.0, 0.0).stroke(Colors.yellow)
    pxl.xy(ctx.width - 1.0, 0).stroke(Colors.red)
    pxl.xy(ctx.width - 1.0, ctx.height - 1.0).stroke(Colors.green)
    pxl.xy(0, ctx.height - 1.0).stroke(Colors.blue)

    blinkingRect 2 2 Colors.red

    blinkingRect 5 5 Colors.blue

    // for x in 4 .. 10 do
    //     pxl.xy(x, 3).stroke(Colors.blueViolet)
    //     pxl.xy(x + 3, 4).stroke(Colors.darkKhaki)
}
|> Simulator.start "localhost"
// |> Simulator.start "192.168.178.52"

