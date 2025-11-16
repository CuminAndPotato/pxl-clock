#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui


let marcDuikersGif =
    Asset.load(__SOURCE_DIRECTORY__, "MarcDuiker_Outrun.gif")
    |> Image.loadFrames


// scene comprehension yields pxls (or other elements) and triggers side effects (paint)
scene {
    // bg(Colors.white)
    image(marcDuikersGif, 0, 0)
}
|> Simulator.start "localhost"

