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


let pacmanLR = pizzaSpriteMap.animate [ 0,0; 0,1; 0,2; 0,1 ]
let pacmanRL = pizzaSpriteMap.animate [ 1,0; 1,1; 0,2; 1,1 ]

let ghostHunterRedLR = pizzaSpriteMap.animate [ 4,0; 4,1 ]
let ghostHunterRedRL = pizzaSpriteMap.animate [ 4,2; 4,3 ]

let ghostHunterPinkLR = pizzaSpriteMap.animate [ 5,0; 5,1 ]
let ghostHunterPinkRL = pizzaSpriteMap.animate [ 5,2; 5,3 ]

let ghostFrightenedRed = pizzaSpriteMap.animate [ 4,8; 4,9 ]
let ghostFrightenedPink = pizzaSpriteMap.animate [ 4,10; 4,11 ]

let pacGhostGhostLR offset =
    scene {
        let offset(col, space) = col * 20.0 + offset + space
        let y = 10

        image(pacmanLR, offset(0, 0), y)
        image(ghostHunterRedLR, offset(-1, -10), y-1)
        image(ghostHunterPinkLR, offset(-2, -10), y-1)
    }

let pacGhostGhostRL offset =
    scene {
        let offset(col, space) = col * 20.0 + offset + space
        let y = 10

        image(pacmanRL, offset(0, 0), y)
        image(ghostFrightenedPink, offset(-1, -10), y-1)
        image(ghostFrightenedRed, offset(-2, -10), y-1)
    }


let oneWayAnimationTimeSpan = 4.5

let ghostHuntingMampf =
    scene {
        let! ctx = getCtx()
        let! x = Anim.linear(oneWayAnimationTimeSpan, -16, 80., repeat = Repeat.Loop)
        pacGhostGhostLR x.value
    }



let mampfHuntingGhosts =
    scene {
        let! ctx = getCtx()
        let! x = Anim.linear(oneWayAnimationTimeSpan, 80, -16., repeat = Repeat.Loop)
        pacGhostGhostRL x.value
    }


// we could also start the partial scenes separately
// by evaluating just one of the 2 lines.
(*
ghostHuntingMampf |> Simulator.start address
mampfHuntingGhosts |> Simulator.start address
*)


[<AppFSharpV1(name = "Pizza Mampf", includeInCycle = true, author = "Cumin & Potato", description = "World Famous Pizza Mampf")>]
let finalScene =
    scene {
        let! switch = Anim.toggleValues(oneWayAnimationTimeSpan * 2.0, [0; 1], repeat = Repeat.Loop)

        if switch = 0 then
            ghostHuntingMampf
        else discardState

        if switch = 1 then
            mampfHuntingGhosts
        else discardState
    }


finalScene |> Simulator.start "localhost"
// finalScene |> Simulator.start "192.168.178.52"


(*
Simulator.stop ()
*)
