#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui
open Pxl.Ui.FSharp



(*

The 3 lines indicate seconds (0-59), minutes(0-59) and hours(0-23).

Idea and Design: Urs Enzler
Programming: Urs Enzler
Color optimizations: Urs Enzler

*)


let time hour minute =
    scene {
        let! ctx = getCtx ()
        let timeText =
            text.var4x5($"%d{hour}:%02d{minute}").color (Color.hsva(200.0, 1.0, 1.0, 1.0))

        let textWidth = timeText.measure ()
        let marginLeft = (ctx.width - textWidth) / 2.0
        let marginTop = (ctx.height - timeText._data.fontSize - 1.0) / 2.0

        timeText
            .xy(marginLeft, marginTop)
            .color Colors.white

    }

let seconds seconds =
    let getColor s =
        let delta = seconds - s |> float
        let v = 0.8 * (59.0 - delta) / 60.0 + 0.2
        Color.hsva(190.0, 1.0, v, 1.0)
    scene {
        let seconds = seconds - 1
        for s in 0..(min 21 seconds) do
            pxl.xy(s + 1, 1).stroke(getColor s)
        for s in 22..(min 25 seconds) do
            pxl.xy(22, s - 20).stroke(getColor s)
        for s in 26..(min 29 seconds) do
            pxl.xy(22, s - 9).stroke(getColor s)
        for s in 30..(min 51 seconds) do
            pxl.xy( 52 - s, 21).stroke(getColor s)
        for s in 52..(min 55 seconds) do
            pxl.xy(1, 72 - s).stroke(getColor s)
        for s in 56..seconds do
            pxl.xy(1, 61 - s).stroke(getColor s)
    }

let minutes minutes =
    let getColor m =
        let delta = minutes - m |> float
        let v = 0.8 * (59.0 - delta) / 60.0 + 0.2
        Color.hsva(200.0, 1.0, v, 1.0)
    scene {
        let minutes = minutes - 1
        for s in 0..(min 19 minutes) do
            pxl.xy(s + 2, 2).stroke(getColor s)
        for s in 20..(min 24 minutes) do
            pxl.xy(21, s - 17).stroke(getColor s)
        for s in 25..(min 29 minutes) do
            pxl.xy(21, s - 10).stroke(getColor s)
        for s in 30..(min 49 minutes) do
            pxl.xy( 51 - s, 20).stroke(getColor s)
        for s in 50..(min 54 minutes) do
            pxl.xy(2, 69 - s).stroke(getColor s)
        for s in 55..minutes do
            pxl.xy(2, 62 - s).stroke(getColor s)
    }

let hours hour =
    let getColor h =
        let delta = hour - h |> float
        let v = 0.8 * (23.0 - delta) / 24.0 + 0.2
        Color.hsva(210.0, 1.0, v, 1.0)
    scene {
        let hour = hour - 1
        for s in 0..(min 9 hour) do
            pxl.xy(s + 7, 6).stroke(getColor s)
        for s in 10..(min 10 hour) do
            pxl.xy(16, s - 3).stroke(getColor s)
        for s in 11..(min 11 hour) do
            pxl.xy(16, s + 4).stroke(getColor s)
        for s in 12..(min 21 hour) do
            pxl.xy( 28 - s, 16).stroke(getColor s)
        for s in 22..(min 22 hour) do
            pxl.xy(7, 37 - s).stroke(getColor s)
        for s in 23..hour do
            pxl.xy(7, 30 - s).stroke(getColor s)
    }

[<AppFSharpV1(name = "Around The Clock", includeInCycle = false, author = "Urs Enzler", description = "Around The Clock")>]
let all =
    scene {
        bg.color(Color.hsva(195.0, 0.9, 0.2, 1.0))
        let! ctx = getCtx ()
        time ctx.now.Hour ctx.now.Minute
        seconds ctx.now.Second
        minutes ctx.now.Minute
        hours ctx.now.Hour
    }

all |> Simulator.start "localhost"

(*
Simulator.stop ()
*)
