#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui


let finalScene isSmooth =
    scene {
        let! ctx = getCtx()
        let now = ctx.now

        let color1 = Color.rgb(200, 200, 200)
        let color2 = Colors.black

        let arcFg, arcBg =
            if now.Minute % 2 = 0
            then color1, color2
            else color2, color1

        // Sekundenzeiger
        bg(arcBg)

        let angle =
            let secFrac = if isSmooth then now.Millisecond else 0
            let angleFor1s = (float (now.Second * 1000 + secFrac)) * 360.0 / 60000.0
            angleFor1s % 360.0
        arc.xywh(-20, -20, ctx.width + 40.0, ctx.height + 40.0)
            .angle(angle)
            .fill(arcFg)
            .isAntiAlias(isSmooth)

        // a black circle
        let margin = 1.0
        circle
            .xyr(
                ctx.halfWidth, ctx.halfHeight,
                (min ctx.halfWidth ctx.halfHeight) - margin)
            .fill(Colors.black)
            .isAntiAlias(isSmooth)

        // a centered HH:mm
        let font = Fonts.var3x5
        let timeText = text.var3x5($"{now:HH}:{now:mm}").color(Colors.white)
        let textWidth = timeText.measure()
        let marginLeft = (ctx.width - textWidth) / 2.0
        let marginTop = (ctx.height  - font.height - 1.0) / 2.0
        timeText.xy(marginLeft, marginTop)
    }


finalScene true |> Simulator.start "localhost"

(*
Simulator.stop ()
*)
