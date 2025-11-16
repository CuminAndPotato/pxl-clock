#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui



let finalScene =
    let arrow(x, y, height) =
        scene {
            let x = x + 2
            line.p1p2(x, y, x, y + height).stroke(Colors.white).strokeThickness(1).noAntiAlias()
            line.p1p2(x - 2, y + height - 3, x + 1, y + height).stroke(Colors.white).strokeThickness(1).noAntiAlias()
            line.p1p2(x + 3, y + height - 3, x, y + height).stroke(Colors.white).strokeThickness(1).noAntiAlias()
        }

    let arrowAndButton(isPushed) =
        scene {
            let offsetY = if isPushed then 1 else 0
            Layer.offset(2, 1 + offsetY)

            arrow(0, 0, 7)
            rect().xy(0, 8).wh(5, 7).fill(Colors.red)
        }

    scene {
        let! buttonAmin = Anim.linear(2, 0, 1.5, repeat = Repeat.Loop)
        let isPushed = buttonAmin.value < 0.5
        arrowAndButton(isPushed)

        line.p1p2(1, 17, 23, 17).stroke(Colors.white).strokeThickness(2)
        text.var4x5($"WPS").xy(8, 10).color(Colors.gray)
    }




finalScene |> Simulator.start "localhost"

(*
Simulator.stop ()
*)

