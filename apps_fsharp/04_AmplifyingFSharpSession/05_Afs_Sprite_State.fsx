#r "nuget: Pxl, 0.0.18"

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

let pizzaLR = pizzaSpriteMap.animate [ 0,0; 0,1; 0,2; 0,1 ]
let ghostHunterRedLR = pizzaSpriteMap.animate [ 4,0; 4,1 ]
let ghostHunterPinkLR = pizzaSpriteMap.animate [ 5,0; 5,1 ]

let pizzaGhostGhostLR offset =
    scene {
        let offset(col, space) = col * 20.0 + offset + space
        let y = 5

        image(pizzaLR, offset(0, 0), y)
        image(ghostHunterRedLR, offset(-1, -10), y-1)
        image(ghostHunterPinkLR, offset(-2, -10), y-1)
    }

let finalScene =
    scene {
        let! x = Anim.linear(4.0, -16, 80., repeat = Repeat.Loop)
        pizzaGhostGhostLR x.value
    }

 
finalScene |> Simulator.start "localhost"
// finalScene |> Simulator.start "192.168.178.52"


(*
Simulator.stop ()
*)
