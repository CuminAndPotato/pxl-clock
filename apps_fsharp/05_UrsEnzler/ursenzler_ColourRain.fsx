#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui
open Pxl.Ui.FSharp



(*

Idea and Design: Nico und Urs Enzler
Programming: Nico und Urs Enzler
Color optimizations: Nico Enzler

*)

let time hour minute second =
    scene {
        text.var10x10($"%d{hour}").xy(0,1).color(Color.hsv(220.0, 0.5, 1))
        text.var10x10($"%02d{minute}").xy(2,10).color(Color.hsv(20.0, 0.5, 1))
        text.var4x5($"%02d{second}").xy(15, 19).color(Color.hsv(100, 0.5, 1))
    }

let offsets =
    [
        10; 4; 17; 7; 12; 1; 13; 19; 9; 14; 1; 7; 18; 9; 5; 17; 8; 4; 9; 19; 2; 6; 13; 17
    ]

let rain step =
    scene {
        for i in 0..23 do
            let offset = offsets[i]
            line.p1p2(i, (step + offset) % 24, i, 3 + (step + offset) % 24)
                .stroke(Color.hsv(0.0 + (float (i * 15)), 0.8, 1.0).opacity(0.6))
                .noAntiAlias()
    }

[<AppFSharpV1(name = "Colour Rain", includeInCycle = false, author = "Urs Enzler", description = "Colour Rain")>]
let all =
    scene {
        let! ctx = getCtx ()

        rain (ctx.now.Second % 24)
        time ctx.now.Hour ctx.now.Minute ctx.now.Second
    }

all |> Simulator.start "localhost"

(*
Simulator.stop ()
*)
