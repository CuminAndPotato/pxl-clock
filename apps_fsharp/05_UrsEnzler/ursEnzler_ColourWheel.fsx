#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui
open Pxl.Ui.FSharp


(*
    This clock needs 6 minutes to walk through the whole HSV colour space once.
    Every second, the hue value of the second hand is increased by one.
    During every minute the visualized seconds are dimmed.
*)


let time hour minute =
    scene {
        text.mono4x5($"%02d{hour}").color(Color.hsva(200.0, 0.0, 1.0, 1.0)).xy(6, 6)
        text.mono4x5($"%02d{minute}").color(Color.hsva(200.0, 0.0, 1.0, 1.0)).xy(9, 13)
    }

let backgroundColor = Color.hsva(195.0, 0.9, 0.2, 0.4)

let seconds minute second =
    let getColor (s: int) step =
        Color.hsva(
            float ((minute * 60) + s),
            1.0 - ((float step) * 0.15),
            1.0 - float (second - s) / 100.0,
            1.0
        )

    let calcColoredPixels () =
        let pixels = Array.init 576 (fun _ -> backgroundColor)

        let set x y color =
            pixels.[x + y * 24] <- color

        for s in 0..(min 6 second) do
            set (12 + s) 4 (getColor s 0)
            set (12 + s) 3 (getColor s 1)
            set (12 + s) 2 (getColor s 2)
            set (12 + s) 1 (getColor s 3)
            set (12 + s) 0 (getColor s 4)
        for s in 7..(min 7 second) do
            set 19 4 (getColor s 0)
            set 20 3 (getColor s 1)
            set 21 2 (getColor s 2)
            set 22 1 (getColor s 3)
            set 23 0 (getColor s 4)

            set 19 3 (getColor s 1)
            set 19 2 (getColor s 2)
            set 19 1 (getColor s 3)
            set 19 0 (getColor s 4)

            set 20 2 (getColor s 2)
            set 20 1 (getColor s 3)
            set 20 0 (getColor s 4)

            set 21 1 (getColor s 3)
            set 21 0 (getColor s 4)

            set 22 0 (getColor s 4)
        for s in 8..(min 8 second) do
            set 20 4 (getColor s 1)
            set 21 4 (getColor s 2)
            set 22 4 (getColor s 3)
            set 23 4 (getColor s 4)
            set 21 3 (getColor s 2)
            set 22 3 (getColor s 3)
            set 23 3 (getColor s 4)
            set 22 2 (getColor s 3)
            set 23 2 (getColor s 4)
            set 23 1 (getColor s 4)

        for s in 8..(min 21 second) do
            set 19 (s - 3) (getColor s 0)
            set 20 (s - 3) (getColor s 1)
            set 21 (s - 3) (getColor s 2)
            set 22 (s - 3) (getColor s 3)
            set 23 (s - 3) (getColor s 4)
        for s in 22..(min 22 second) do
            set 19 19 (getColor s 0)
            set 20 20 (getColor s 1)
            set 21 21 (getColor s 2)
            set 22 22 (getColor s 3)
            set 23 23 (getColor s 4)
            set 20 19 (getColor s 1)
            set 21 19 (getColor s 2)
            set 22 19 (getColor s 3)
            set 23 19 (getColor s 4)
            set 21 20 (getColor s 2)
            set 22 20 (getColor s 3)
            set 23 20 (getColor s 4)
            set 22 21 (getColor s 3)
            set 23 21 (getColor s 4)
            set 23 22 (getColor s 4)
        for s in 23..(min 23 second) do
            set 19 20 (getColor s 1)
            set 19 21 (getColor s 2)
            set 19 22 (getColor s 3)
            set 19 23 (getColor s 4)
            set 20 21 (getColor s 2)
            set 20 22 (getColor s 3)
            set 20 23 (getColor s 4)
            set 21 22 (getColor s 3)
            set 21 23 (getColor s 4)
            set 22 23 (getColor s 4)
        for s in 23..(min 36 second) do
            set (41 - s) 19 (getColor s 0)
            set (41 - s) 20 (getColor s 1)
            set (41 - s) 21 (getColor s 2)
            set (41 - s) 22 (getColor s 3)
            set (41 - s) 23 (getColor s 4)
        for s in 37..(min 37 second) do
            set 4 19 (getColor s 0)
            set 3 20 (getColor s 1)
            set 2 21 (getColor s 2)
            set 1 22 (getColor s 3)
            set 0 23 (getColor s 4)
            set 4 20 (getColor s 1)
            set 4 21 (getColor s 2)
            set 4 22 (getColor s 3)
            set 4 23 (getColor s 4)
            set 3 21 (getColor s 2)
            set 3 22 (getColor s 3)
            set 3 23 (getColor s 4)
            set 2 22 (getColor s 3)
            set 2 23 (getColor s 4)
            set 1 23 (getColor s 4)
        for s in 38..(min 38 second) do
            set 3 19 (getColor s 1)
            set 2 19 (getColor s 2)
            set 1 19 (getColor s 3)
            set 0 19 (getColor s 4)
            set 2 20 (getColor s 2)
            set 1 20 (getColor s 3)
            set 0 20 (getColor s 4)
            set 1 21 (getColor s 3)
            set 0 21 (getColor s 4)
            set 0 22 (getColor s 4)
        for s in 38..(min 51 second) do
            set 4 (56 - s) (getColor s 0)
            set 3 (56 - s) (getColor s 1)
            set 2 (56 - s) (getColor s 2)
            set 1 (56 - s) (getColor s 3)
            set 0 (56 - s) (getColor s 4)
        for s in 52..(min 52 second) do
            set 4 4 (getColor s 0)
            set 3 3 (getColor s 1)
            set 2 2 (getColor s 2)
            set 1 1 (getColor s 3)
            set 0 0 (getColor s 4)
            set 3 4 (getColor s 1)
            set 2 4 (getColor s 2)
            set 1 4 (getColor s 3)
            set 0 4 (getColor s 4)
            set 2 3 (getColor s 2)
            set 1 3 (getColor s 3)
            set 0 3 (getColor s 4)
            set 1 2 (getColor s 3)
            set 0 2 (getColor s 4)
            set 0 1 (getColor s 4)
        for s in 53..(min 53 second) do
            set 4 3 (getColor s 1)
            set 4 2 (getColor s 2)
            set 4 1 (getColor s 3)
            set 4 0 (getColor s 4)
            set 3 2 (getColor s 2)
            set 3 1 (getColor s 3)
            set 3 0 (getColor s 4)
            set 2 1 (getColor s 3)
            set 2 0 (getColor s 4)
            set 1 0 (getColor s 4)
        for s in 53..(min 59 second) do
            set (s - 48) 4 (getColor s 0)
            set (s - 48) 3 (getColor s 1)
            set (s - 48) 2 (getColor s 2)
            set (s - 48) 1 (getColor s 3)
            set (s - 48) 0 (getColor s 4)

        pixels

    scene {
        let! coloredPixels = useState { calcColoredPixels () }

        let! trigger = Trigger.valueChanged(second)
        if trigger then
            coloredPixels.value <- calcColoredPixels ()

        pxls.set(coloredPixels.value)
    }

[<AppFSharpV1(name = "Colour Wheel", includeInCycle = false, author = "Urs Enzler", description = "Colour Wheel")>]
let all =
    scene {
        bg.color(Color.hsva(195.0, 0.9, 0.2, 0.4))
        let! ctx = getCtx ()
        seconds ctx.now.Minute ctx.now.Second
        time ctx.now.Hour ctx.now.Minute
    }

all |> Simulator.start "localhost"

(*
Simulator.stop ()
*)
