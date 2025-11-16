#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui


let blinkingCursor =
    scene {
        let! color = 
            Anim.toggleValues(
                0.25, 
                [ 
                    Colors.black
                    Colors.white 
                ], 
                Repeat.Loop)
        text.var4x5("_", 0, 0).color(color)
    }

blinkingCursor |> Simulator.start "localhost"

