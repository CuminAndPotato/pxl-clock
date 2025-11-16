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

[<Measure>]
type Degrees
type ColourSeeds = { Previous: float<Degrees>; Current: float<Degrees>; Next: float<Degrees> }

module Degrees =
    let getSmallerAngelAndDirection (a: float<Degrees>) (b: float<Degrees>) =
        let a = a % 360.0<Degrees>
        let b = b % 360.0<Degrees>
        let diff = abs (b - a)
        if diff > 180.0<Degrees> then
            let diff = 360.0<Degrees> - diff
            if abs((a + diff) % 360.0<Degrees> - b) < abs((b + diff) % 360.0<Degrees> - a) then diff, 1
            else diff, -1
        else
            if abs((a + diff) % 360.0<Degrees> - b) < abs((b + diff) % 360.0<Degrees> - a) then diff, 1
            else diff, -1


let time hour minute =
    scene {
        text.mono4x5($"%02d{hour}").color(Color.hsva(200.0, 0.0, 1.0, 1.0)).xy(6, 6)
        text.mono4x5($"%02d{minute}").color(Color.hsva(200.0, 0.0, 1.0, 1.0)).xy(9, 13)
    }

let backgroundColor = Color.hsva(195.0, 0.9, 0.2, 0.4)
let seconds minute second =
    let calcColoredPixels colorSeeds =
        let previousAngel, previousDirection = Degrees.getSmallerAngelAndDirection colorSeeds.Previous colorSeeds.Current
        let currentAngel, currentDirection = Degrees.getSmallerAngelAndDirection colorSeeds.Current colorSeeds.Next
        let previousAngelPerStep = previousAngel / 60.0
        let currentAngelPerStep = currentAngel / 60.0

        let getValue delta =
            max (1.0 - ((Math.Pow(2.0, Math.Pow(delta, 1.15) / 20.0) - 1.0) / 80.0)) 0.0

        let getColor (s: int) step =
            if (s <= second) then
                let value = (float (colorSeeds.Current + currentAngelPerStep * float s * float currentDirection))
                Color.hsva(
                    (if value > 0 then value else 360.0 + value),
                    1.0 - ((float step) * 0.15),
                    //(getValue (float (second - s)))
                    1.0 - float (second - s) / 100.0 - step * 0.05,
                    1.0
                )
            else
                let value = (float (colorSeeds.Previous + previousAngelPerStep * float s * float previousDirection))
                Color.hsva(
                    (if value > 0 then value else 360.0 + value),
                    1.0 - ((float step) * 0.15),
                    //(getValue (float (second - s + 60)))
                    1.0 - float ((second - s + 60) % 60) / 100.0 - step * 0.05,
                    1.0
                )

        let pixels = Array.init 576 (fun _ -> backgroundColor)

        let set x y color =
            pixels.[x + y * 24] <- color

        for s in 0..6 do
            set (12 + s) 4 (getColor s 0)
            set (12 + s) 3 (getColor s 1)
            set (12 + s) 2 (getColor s 2)
            set (12 + s) 1 (getColor s 3)
            set (12 + s) 0 (getColor s 4)
        for s in 7..7 do
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
        for s in 8..8 do
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

        for s in 8..21 do
            set 19 (s - 3) (getColor s 0)
            set 20 (s - 3) (getColor s 1)
            set 21 (s - 3) (getColor s 2)
            set 22 (s - 3) (getColor s 3)
            set 23 (s - 3) (getColor s 4)
        for s in 22..22 do
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
        for s in 23..23 do
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
        for s in 23..36 do
            set (41 - s) 19 (getColor s 0)
            set (41 - s) 20 (getColor s 1)
            set (41 - s) 21 (getColor s 2)
            set (41 - s) 22 (getColor s 3)
            set (41 - s) 23 (getColor s 4)
        for s in 37..37 do
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
        for s in 38..38 do
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
        for s in 38..51 do
            set 4 (56 - s) (getColor s 0)
            set 3 (56 - s) (getColor s 1)
            set 2 (56 - s) (getColor s 2)
            set 1 (56 - s) (getColor s 3)
            set 0 (56 - s) (getColor s 4)
        for s in 52..52 do
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
        for s in 53..53 do
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
        for s in 53..59 do
            set (s - 48) 4 (getColor s 0)
            set (s - 48) 3 (getColor s 1)
            set (s - 48) 2 (getColor s 2)
            set (s - 48) 1 (getColor s 3)
            set (s - 48) 0 (getColor s 4)

        pixels

    scene {
        let! colourSeeds = useState { { Previous = Random.Shared.NextDouble() * 360.0<Degrees>; Current = Random.Shared.NextDouble() * 360.0<Degrees>; Next = Random.Shared.NextDouble() * 360.0<Degrees> } }
        let! coloredPixels = useState { calcColoredPixels colourSeeds.value }

        let! colorSeedTrigger = Trigger.valueChanged(minute)
        let! frameTrigger = Trigger.valueChanged(second)

        if colorSeedTrigger then
            colourSeeds.value <- { Previous = colourSeeds.value.Current; Current = colourSeeds.value.Next; Next = Random.Shared.NextDouble() * 360.0<Degrees> }

        if frameTrigger then
            coloredPixels.value <- calcColoredPixels colourSeeds.value

        pxls.set(coloredPixels.value)
    }

[<AppFSharpV1(name = "Colour Wheel Random", includeInCycle = false, author = "Urs Enzler", description = "Colour Wheel Random")>]
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
