#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui


(*
----------------------------------------------------------
PLEASE FOLLOW THE INSTRUCTIONS FOR SPRITES IN THE README
----------------------------------------------------------
*)
let pizzaSpriteMap =
    Image.loadFromAsset(__SOURCE_DIRECTORY__, "pizzaMampf.png")
        .crop(456, 0, 0, 0)
        .makeSpriteMap(16, 16, 50)

let ghost =
    pizzaSpriteMap.animate [ 4,0; 4,1 ]

let finalScene =
    scene {
        image(ghost, 0, 0)
        image(ghost, 12, 6)
        image(ghost, 0, 6)
    }

 
finalScene |> Simulator.start "localhost"
// finalScene |> Simulator.start "192.168.178.52"


(*
Simulator.stop ()
*)
