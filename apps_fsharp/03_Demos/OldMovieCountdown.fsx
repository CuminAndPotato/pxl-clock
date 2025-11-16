#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui




// A complete countdown component,
// returning a boolean indicating if the countdown has finished.
let countdown (duration: int) =
    scene {
        bg(Colors.lightGray)

        let! ctx = getCtx ()

        circle.xyr(ctx.halfWidth, ctx.halfHeight, 12)
            .stroke(Colors.black)
            .strokeThickness(1.0)

        let! angle = Anim.linear(1, 0, 360, Repeat.Loop)
        let mkArc () =
            arc.xywh(-ctx.width, -ctx.height, 3.0 * ctx.width, 3.0 * ctx.height)
                .startAngle(-90)
                .angle(angle.value)
        mkArc().fill(Colors.red.opacity(0.5))
        mkArc().noAntiAlias().stroke(Color.mono 50).strokeThickness(1.0)

        let! remainingSecs = useState { duration }

        text($"{remainingSecs.value}").xy(5, 4).color(Colors.black).fontSize(18)

        let! endReachedOrCrossed = Trigger.falseToTrue angle.isAtEndTrigger
        if endReachedOrCrossed then
            remainingSecs.value <- remainingSecs.value - 1

        return remainingSecs.value <= 0
    }

// The component for the plant rising animation.
let plantRising =
    let plantImg = Image.loadFromAsset(__SOURCE_DIRECTORY__, "plant.png")
    scene {
        let! y = Anim.easeInOutSine(4, 24, 0, Repeat.StopAtEnd)
        image(plantImg, 0, y.value)
    }

// Finally, we compose the countdown and the plant rising animation.
let finalAnimation (duration) =
    scene {
        let! hasCountdownFinished = countdown duration

        if hasCountdownFinished then
            bg(Colors.black)
            plantRising
        else
            preserveState
    }


finalAnimation 9 |> Simulator.start "localhost"


(*
Simulator.stop ()
*)
