#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui
open Pxl.Ui.FSharp


let time (now: DateTimeOffset) =
    scene {
        let! ctx = getCtx ()

        let timeText =
            text.var4x5($"%02d{now.Hour}:%02d{now.Minute}").color (Colors.white)

        let textWidth = timeText.measure ()
        let marginLeft = (ctx.width - textWidth) / 2.0

        timeText
            .xy(marginLeft, 4)
            .color Colors.white
    }

let image1 =
    [
        "                                   yyyy           g                                 "
        "                               yyyyyyyyyyyy    ggggg                                "
        "                                   yyyy       gggggggg                              "
        "                               yyyyyyyyyyyy     ggg ggg                             "
        "                                    y y          gg  g                              "
        "                                    y y                                             "
        "                                    y y                                             "
        "                                    y y                                             "
        "                                                                                    "
        "                                            e e                                     "
        "                                          e eeee                                    "
        "                                         eeeeeee    lll lll                         "
        "                                          eeeee     lfl lfl                         "
        "     _               ee  e               eeeeeeee   lfl lfl        _                "
        "    ___      _      eeeeeeee              e be e    ddddddd       ___      _        "
        "   _____    ___      eeeeee                 bb      dcdddcd      _____    ___       "
        "  ______________    eeeeeeee                bb      ddddddd     ______________      "
        "_________________    e be e                 bb      ddcccdd    ________________     "
        "___________________    bb                   bb       ddddd    __________________    "
        "_____________________  bb                   bb       ddddd   _____________________  "
        "_______________________bb           ________tdddddddddddddd_________________________"
        "_______________________bb__________________ttdddddddddddddd_________________________"
        "_______________________bb__________________tbdd_dd____dd_dd_________________________"
        "_____________________________________________dd_dd____dd_dd_________________________"
    ]
let image2 =
    [
        "                               yyyyyyyyyyyy      gg                                 "
        "                                   yyyy         ggggg                               "
        "                               yyyyyyyyyyyy    ggggggg                              "
        "                                   yyyy         ggg ggg                             "
        "                                   y y          gg  gg                              "
        "                                   y y                                              "
        "                                   y y                                              "
        "                                   y y                                              "
        "                                                                                    "
        "                                            e e                                     "
        "                                          e eeee                                    "
        "                                         eeeeeee    lll lll                         "
        "                                          eeeee     lfl lfl                         "
        "     _               ee  e               eeeeeeee   lfl lfl        _                "
        "    ___      _      eeeeeeee              e be e    ddddddd       ___      _        "
        "   _____    ___      eeeeee                 bb      dcdddcd      _____    ___       "
        "  ______________    eeeeeeee                bb      ddddddd     ______________      "
        "_________________    e be e                 bb      ddcccdd    ________________     "
        "___________________    bb                   bb       ddddd    __________________    "
        "_____________________  bb                   bb       ddddd   _____________________  "
        "_______________________bb           ________tdddddddddddddd_________________________"
        "_______________________bb__________________ttdddddddddddddd_________________________"
        "_______________________bb_________________ttbdd_dd____dd_dd_________________________"
        "______________________________________________dd_dd____dd_dd________________________"
    ]

let shift animation i =
    let shiftLine (line: string) =
        line.AsSpan().Slice(line.Length - 1 - i - 24, 24).ToString()
    let image =
        match animation with
        | 0 -> image1
        | 1 -> image2
        | _ -> image1
    image
    |> List.map shiftLine

let dog (minute: int) (second: int) =
    scene {
        // pxl.xy(second % 24, 1).stroke(Colors.blue)

        let calcColoredPixels () =
            let current =
                shift (second % 2) second
            let pixels =
                [
                    for l in 0..23 do
                        for pixel in current[l] do
                            yield pixel
                ]
            let coloredPixels =
                [
                    for pixel in pixels do
                        let color =
                            match pixel with
                            | 'b' -> Colors.brown
                            | 'y' -> Colors.yellow
                            | 'd' -> Colors.saddleBrown
                            | 'e' -> Colors.green
                            | 'g' -> Colors.gray
                            | 't' -> Colors.rosyBrown
                            | 'l' -> Colors.saddleBrown
                            | 'f' -> Colors.sandyBrown
                            | 'c' -> Colors.bisque
                            | '_' -> Colors.beige
                            | _ -> Colors.blue
                        color
                ]
            coloredPixels |> List.toArray

        // remember the colored pixels (initially) and be updated when second changes (below)
        let! coloredPixels = useState { calcColoredPixels () }

        // only calculate colored pixels when second changes
        let! trigger = Trigger.valueChanged(second)
        if trigger then
            coloredPixels.value <- calcColoredPixels ()

        // `pxl.xy` seems to be the performance issue in this app
        pxls.set(coloredPixels.value)
        // for c,l,color in coloredPixels.value do
        //     pxl.xy(c,l).stroke(color)
    }

let diffuser =
    scene {
        rect
            .xywh(0, 2, 24, 9)
            .fill(Color.argb (80, 0, 0, 0))
            .useAntiAlias ()

        rect
            .xywh(0, 3, 24, 7)
            .fill(Color.argb (80, 0, 0, 0))
            .useAntiAlias ()
    }

[<AppFSharpV1(name = "Nicos Dog", includeInCycle = false, author = "Urs Enzler", description = "Nicos Dog")>]
let all =
    scene {
        let! ctx = getCtx ()
        dog ctx.now.Minute ctx.now.Second
        diffuser
        time ctx.now
    }

all |> Simulator.start "localhost"

(*
Simulator.stop ()
*)

