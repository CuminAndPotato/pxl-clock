#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



// The system uses the concept of layers that are accessible and
// controlleable from the view. Layers can be rotated, scaled, etc.
scene {
    let! ctx = getCtx()
    let rectCenter(margin, color) =
        rect.xywh(
            margin,
            margin,
            ctx.width - 2.0 * margin,
            ctx.height - 2.0 * margin)
            .fill(color)

    // draw a red rect - this is not rorated
    rectCenter(1, Colors.red)

    scene {
        // set up the transformation _before_ drawing in this scene
        Layer.rotate(45.0)

        // ... and draw the green rect, which will be rotated
        rectCenter(2, Colors.green)
    }

    // ...this rect is NOT rotated
    rectCenter(6, Colors.blue)
}
|> Simulator.start "localhost"


// // access merged yielded "combine" and copy the 0,0 pixel to the right
// vide {
//     let dim = 8
//     let! mergedLayer = vide {
//         rect(0, 0, dim, dim, Colors.blue)
//         rect(1, 1, dim-2, dim-2, Colors.red)
//     }

//     // do something with the merged layer
//     mergedLayer.rotateDeg(45.0)
// }
// |> Eval.plot canvas


// TODO: Blending of layers
