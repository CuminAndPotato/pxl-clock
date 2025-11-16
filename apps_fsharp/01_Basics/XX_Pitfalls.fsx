#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



// get "now"
// Don't use DateTime.Now in Pxl code. Use the time from the context instead:
let useNow =
    scene {
        let! ctx = getCtx ()
        let now = ctx.now

        // "now" is DateTime - use this from here on.
        ()
    }


// Sometimes a "do if" is required (not only an "if") to make the code work.
// This is usually the case when nothing is drawn on the canvases in the "if" block.
// In that case you get any errors that are somehow connected to "if", try "do if" instead.
let doIfPitfall =
    scene {
        let! ctx = getCtx ()

        let! currSceneNo = useState { 0 }
        let! timeLeft = Anim.linear(10, 0, 1, repeat = Repeat.Loop, autoStart = true)
        let! swipeOffsetAnim = Anim.easeInOutCubic(
            0.7,
            0,
            0,
            repeat = Repeat.StopAtEnd,
            autoStart = false)

        do if timeLeft.isAtEndTrigger then
            if currSceneNo.value = 0 then
                swipeOffsetAnim.startValue <- 0
                swipeOffsetAnim.endValue <- -24
            else
                swipeOffsetAnim.startValue <- -24
                swipeOffsetAnim.endValue <- 0

            swipeOffsetAnim.restart()

            currSceneNo.value <- currSceneNo.value + 1
            if currSceneNo.value = 2 then
                currSceneNo.value <- 0
    }


// It is possible to render views conditionally.
// Please note that in an `else` branch,
// there are only 2 things allowed: `preserveState` and `discardState`.
// They both are required to tell the system if the (eventual) state
// of the components from the `if` branch should be kept for upcoming
// frames or not.
let isWithAnElseState =
    scene {
        let! cycle = Logic.count(0, 1)
        if cycle % 10 = 0 then
            let! innerCount = Logic.count(0, 1)
            text.mono6x6($"{innerCount}").color(Colors.white)
        else preserveState
        // else discardState
    }



(*

Pitfalls (others)
===


An “else preserve” or “else discard” must NOT be used.

Data Types
===
int, float, float32

It is best to always use float, as it is the most widely supported type.
e.g., 1.0 or 2.4 - but also 0.0

For paths (polygon), unfortunately, float32 must still be used because the SKIA-Sharp Library is used here.
e.g., 1.0f or 2.4f - but also 0.0f

*)
