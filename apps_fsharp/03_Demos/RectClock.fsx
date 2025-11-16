#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui




let rectClock(now: DateTimeOffset) =
    scene {
        let! ctx = getCtx()

        // a centered HH:mm
        let timeText = text.var3x5($"{DateTime.Now:HH}:{DateTime.Now:mm}").color(Colors.white)
        let textWidth = timeText.measure()
        let marginLeft = (ctx.width - textWidth) / 2.0
        let marginTop = (ctx.height - (timeText._data.fontSize) - 1.0) / 2.0
        timeText.xy(marginLeft, marginTop)

        // the partial rectangle to display the seconds
        // We need to "oversample" the seconds (i.e. also take fractions of a second into account)
        // so that the pixels will have a smoothly animated opacity.

        // We have 60 seconds that have to be mapped to 92 pixels.
        // We do it in a way so that only max. 1 pixel is between 0..1 brightness.
        // (no stretching of that part over more than 1 pxl).
        // 2 steps:
        //  1: a 1D-map with constant velocity. More concrete:
        //       There must be 2 points to be calculated that are
        //         - The frontier between the elapsed color (e.g. white),
        //         - and the shaded color
        //         - and the frontier to the rest (that is, since we constrain
        //           the mapping to 1 pxl, only a pixel shift).
        //       Issue here: The ticks are not 1s, so it's misleading, and
        //       a stretched ending would look smoother.
        //  2: map that 1D coord back to 2D space

        let to96ths idx = idx * 96 / 60

        let pxlMap =
            [
                // we start center-top (and that won't work in case of odd dimensions)
                // also: the dimension ends with the last point of that axes
                let mutable overallOffset = 0.0
                for x in 0.0 .. (ctx.width / 2.0)  do
                    overallOffset, (ctx.width / 2.0 + x, 0.0)
                    overallOffset <- overallOffset + 1.0
                for y in 1.0.. ctx.height do
                    overallOffset, (ctx.width - 1.0, y)
                    overallOffset <- overallOffset + 1.0
                for x in ctx.width - 1.0 .. -1.0.. 0.0 do
                    overallOffset, (x, ctx.height - 1.0)
                    overallOffset <- overallOffset + 1.0
                for y in ctx.height - 1.0 .. -1.0 .. 0.0 do
                    overallOffset, (0.0, y)
                    overallOffset <- overallOffset + 1.0
                for x in 1.0 .. (ctx.width / 2.0 + 2.0)  do
                    overallOffset, (x, 0.0)
                    overallOffset <- overallOffset + 1.0
            ]
            |> Map.ofList
        let mapToPixel idx = pxlMap[idx]

        let pxl1d idx fillColor =
            let px, py = mapToPixel idx
            pxl.xy(px, py).stroke(fillColor).noAntiAlias()

        for x in 0 ..  to96ths now.Second do
            pxl1d x Colors.white

        // the shaded last pixel
        let lastIdx = now.Second
        pxl1d (to96ths lastIdx) (Colors.white.brightness(float now.Millisecond / 1000.0))
    }


let finalScene =
    scene {
        let! ctx = getCtx()
        rectClock(ctx.now)
    }


finalScene |> Simulator.start "localhost"

(*
Simulator.stop ()
*)
