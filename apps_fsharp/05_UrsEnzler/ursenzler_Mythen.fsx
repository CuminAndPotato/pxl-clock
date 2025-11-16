#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui
open Pxl.Ui.FSharp


(*

Idea and Design: Urs Enzler
Programming: Urs Enzler
Color optimizations: Urs Enzler

*)

let time hour minute =
    scene {
        let! ctx = getCtx()

        let timeText = text.var4x5($"%02d{hour}:%02d{minute}").color(Colors.white)
        let textWidth = timeText.measure()
        let marginLeft = (ctx.width - textWidth) / 2.0
        let marginTop = (ctx.height - timeText._data.fontSize - 1.0) / 2.0
        timeText
            .xy(marginLeft, marginTop)
            .color Colors.white
    }

let interpolate start ``end`` step steps =
    start + ((``end`` - start) * ((float step) / (float steps)))

let lines hour  =
    scene {
        let hTop, hBottom, saturationTop, saturationBottom, valueTop, valueBottom =
            match hour with
            | h when h < 7 -> 200.0, 200.0, 0.3, 0.5, 0.0, 0.4
            | h when h < 8 -> 216.0, 375.0, 0.42, 0.17, 0.76, 0.83
            | h when h < 9 -> 216.0, 213.0, 0.22, 0.27, 0.76, 0.83
            | h when h < 18 -> 216.0, 200.0, 0.51, 0.0, 0.78, 0.85
            | h when h < 22 -> 216.0, 375.0, 0.42, 0.17, (interpolate 0.78 0.0 (h-18) 4), (interpolate 0.83 0.4 (h-18) 4)
            | _ -> 200.0, 200.0, 0.3, 0.5, 0.0, 0.4

        for l in 0..19 do
            let step = l + 1
            line.p1p2(0, l, 24,l).stroke(
                Color.hsv(
                    interpolate hTop hBottom step 20,
                    interpolate saturationTop saturationBottom step 20,
                    interpolate valueTop valueBottom step 20
                )
            ).noAntiAlias()
    }



let mythen =
    let img = Image.loadFromAsset(__SOURCE_DIRECTORY__, "ursenzler_Mythen.png")
    image(img, 0, 0)

[<AppFSharpV1(name = "Mythen", includeInCycle = false, author = "Urs Enzler", description = "Mythen")>]
let all =
    scene {
        let! ctx = getCtx ()
        lines ctx.now.Hour
        mythen
        time ctx.now.Hour ctx.now.Minute
    }

all |> Simulator.start "localhost"

(*
Simulator.stop ()
*)
