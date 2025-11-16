#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui


(*

    This is the successor of the `./PxlApps/Tutorial/clockWithAnimatedSeconds_v1.fsx` example.

    -------------------------------------------------------------
    
    ...continued from the previous example:

    We can reach the same goal as in the previous example,
    by taking advantage of the Trigger module.
    The requirements remain the same, but the implementation and the
    idea behind it is different: Instead of having an imperative approach
    of controlling animations with start, stop, etc., the idea here is
    to define one-shot animation that is then restarted as a whole
    when the text changes. That simplifies things, because inside the
    one-shot animation, many values that looked like state, are now
    invariant values.
*)

let animateTextChange (currText: string, x: int, y: int, color: Color, font) =
    scene {
        let outDur = 0.4
        let inDur = 0.3
        let inDelay = 0.2
        let flyOutWidth = 10

        let! lastText = Logic.delayBy1("", currText)

        Trigger.restartWhenValueChanges(
            currText,
            scene {
                let! currText = useMemo { currText }
                let! lastText = useMemo { lastText }

                let! flyOutAnim = Anim.easeOutCubic(outDur, x, flyOutWidth, autoStart = true)
                let! fadeOutAnim = Anim.easeOutCubic(outDur, color.opacity(), 0.0, autoStart = true)
                let! fadeInAnim = Anim.easeIn(inDur, 0.0, color.opacity(), autoStart = false)

                let! startFadeIn = Trigger.thresholdUp(inDelay, fadeOutAnim.elapsedRel)
                if startFadeIn then
                    fadeInAnim.restart()

                text(lastText).xy(flyOutAnim.valuei, y).color(color.opacity(fadeOutAnim.value)).font(font)
                text(currText).xy(x, y).color(color.opacity(fadeInAnim.value)).font(font)
            })
    }


// and again, our clock:
let finalScene =
    scene {
        let fgColor = Colors.white.opacity(0.5)
        let! ctx = getCtx ()

        let font = Fonts.mono6x6

        let row row = 1 + (row * (float font.height + 2.0) |> int)

        text(" " + ctx.now.ToString("HH")).xy(0, row 0).color(fgColor).font(font)
        text(":" + ctx.now.ToString("mm")).xy(0, row 1).color(fgColor).font(font)
        animateTextChange(":" + ctx.now.ToString("ss"), 0, row 2, fgColor, Fonts.mono6x6)
    }



finalScene |> Simulator.start "localhost"


(*
Simulator.stop ()
*)
