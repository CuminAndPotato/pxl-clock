#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



type WifiStatusWidgetState = Connected | Wps


let sceneSwitcher (scene1: Vide<_,_>) (scene2: Vide<_,_>) duration =
    scene {
        let! ctx = getCtx ()

        let! currSceneNo = useState { 0 }
        let! timeLeft = Anim.linear(duration, 0, 1, repeat = Repeat.Loop, autoStart = true)
        let! swipeOffsetAnim = Anim.easeInOutCubic(
            0.7,
            0,
            0,
            repeat = Repeat.StopAtEnd,
            autoStart = false)

        scene {
            Layer.offset(swipeOffsetAnim.value, 0)
            scene1
        }

        scene {
            Layer.offset(swipeOffsetAnim.value + float ctx.width, 0)
            scene2
        }

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


let pushWpsButton =
    let img = Image.loadFromAsset(__SOURCE_DIRECTORY__, "buttons_set_main_colRed_other.png")
    let imgReleased = img.cropLRWH(36, 1, 20, 17)
    let imgPressed = img.cropLRWH(59, 1, 20, 17)

    let pushButtonFrames =
        [
            imgReleased.withDuration(400)
            imgPressed.withDuration(300)
            imgReleased.withDuration(700)
        ]

    let pushButtonAnimation =
        useMemo { pushButtonFrames }

    scene {
        image(pushButtonAnimation, 2, 1)
        tickerText("router -> push wps", Colors.black, 26.0, vAlign = VAlign.Bottom)
    }


let wifiReconnectIcon =

    let wave(cx, cy, amplitude, wavelength, period, phaseShift) =
        let period = -period

        // Constants derived from parameters
        let k = 2.0 * Math.PI / wavelength  // Wave number
        let omega = 2.0 * Math.PI / period  // Angular frequency

        // The wave function u(time, x, y)
        let waveFunction (time: float) (x: float) (y: float) =
            let r = sqrt ((x - cx) ** 2.0 + (y - cy) ** 2.0)
            let amplitude' = amplitude * sin(k * r - omega * time + phaseShift)
            // Normalize amplitude to range [0,1]
            let normalizedAmplitude = (amplitude' + amplitude) / (2.0 * amplitude)
            normalizedAmplitude

        waveFunction

    scene {
        let! ctx = getCtx()

        let waveFunction = wave(ctx.halfWidth, ctx.height, 1, 8.0, 0.7, 0.0)
        shaderGenerative (fun p ->
            let pxl = waveFunction ctx.elapsed.TotalSeconds p.x p.y
            Colors.white.brightness(pxl)
        )

        // TODO: This is so strange - I cant use white here 2 times

        polygon
            .define(fun path ->
                path.MoveTo(f32 ctx.halfWidth + 0.5f, f32 ctx.height)
                path.LineTo(0f, 0f)
                path.LineTo(0f, f32 ctx.height)
                path.Close()
            )
            .fill(Colors.gray)

        polygon
            .define(fun path ->
                path.MoveTo(f32 ctx.halfWidth - 0.5f, f32 ctx.height)
                path.LineTo(f32 ctx.width, 0f)
                path.LineTo(f32 ctx.width, f32 ctx.height)
                path.Close()
            )
            .fill(Colors.gray)
    }


let finalScene =
    scene {
        bg(Colors.gray)
        sceneSwitcher pushWpsButton wifiReconnectIcon 5
    }



finalScene |> Simulator.start "localhost"


(*
Simulator.stop ()
*)

