#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



(*
    What we want here:
    The given text "s" shall be shown at x,y.
    When the text changes, the old text shall fly out and faded outto the right,
    and it should be faded in opacity.
    Shortly after the flyout has begun,
    the new text shall be faded in.
*)

let animateTextChange (currText: string, x: int, y: int, color: Color) =
    scene {
        let outDur = 0.4
        let inDur = 0.3
        let inDelay = 0.2
        let flyOutWidth = 10

        // In order to detect changes, we need to keep the last text.
        let! lastText = useState { currText }

        // For a better understanding, we keep explicit states for the animations.
        let! fadeInText = useState { currText }
        let! flyOutText = useState { currText }

        // We know that we need (according to our spec above)
        // 3 animations: one for the flyout+fadeout, and one for the fade in.
        let! flyOutAnim = Anim.easeOutCubic(outDur, x, flyOutWidth, autoStart = false)
        let! fadeOutAnim = Anim.easeOutCubic(outDur, color.opacity(), 0.0, autoStart = false)
        let! fadeInAnim = Anim.easeIn(inDur, 0.0, color.opacity(), autoStart = false)

        if currText <> lastText.value then
            flyOutText.value <- lastText.value
            fadeInText.value <- currText
            lastText.value <- currText
            flyOutAnim.restart()
            fadeOutAnim.restart()

        // We have to "delay" the fadeIn animation a bit,
        // according to the progress flyOut animation,
        // so we need a trigger to control the fadeIn animation.
        let! startFadeIn = Trigger.thresholdUp(inDelay, flyOutAnim.elapsedRel)
        if startFadeIn then
            fadeInAnim.restart()

        // We need a maximum of 2 text elements to be drawn:
        // The last text and the current text.
        // The current text is always bound to the fade in animation;
        // the last text is bound to the fly out animation.
        if flyOutAnim.isRunning || fadeOutAnim.isRunning then
            text.mono6x6(flyOutText.value).xy(flyOutAnim.valuei, y).color(color.opacity(fadeOutAnim.value))
        if fadeInAnim.isRunning then
            text.mono6x6(fadeInText.value).xy(x, y).color(color.opacity(fadeInAnim.value))

        // From the start of the flyOut animation
        // until the end of the fadeIn animation,
        // the static text shall not be drawn.
        let! showStaticText = Trigger.startAndHold(fadeInAnim.isPaused, flyOutAnim.isRunning)
        if showStaticText then
            text.mono6x6(currText).xy(x, y).color(color)
    }





// Let's test that animation with an incremental a number after a short while
// !!!!! Make sure that all the code above is evaluated,
// select this block here and evaluate it.
scene {
    let! count = useState { 0 }
    let! displayValue = useState { count.value }

    if count.value % 80 = 0 then
        displayValue.value <- count.value

    count.value <- count.value + 1

    animateTextChange($"{displayValue.value}", 0, 0, Colors.white)
}
|> Simulator.start "localhost"




// or we can program a more fancy clock
// !!!!! Make sure that all the code above is evaluated,
// without the animation test above,
// select this block here and evaluate it.
scene {
    let color = Colors.white
    let! ctx = getCtx ()
    let row row = row * (Fonts.mono6x6.height + 1.0) |> int

    // TODO: something like "every half second" or "every 3 seconds"
    let hour = ctx.now.ToString("HH")
    let minute = ctx.now.ToString("mm")
    let second = ctx.now.ToString("ss")

    text.mono6x6(hour).xy(0, 0).color(color)

    text.mono6x6($"{minute}").xy(0, row 1).color(color)

    animateTextChange(second, 0, row 2, color)
}
|> Simulator.start "localhost"



(*
Simulator.stop ()
*)
