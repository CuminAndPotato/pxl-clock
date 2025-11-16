#r "nuget: Pxl, 0.0.21"

open System
open Pxl
open Pxl.Ui



(*
    How to test this?
        1. evaluate the above code only (by selecting it and pressing Alt+Enter)
        2. evaluate one of the following code snippets (2 lines each) by selecting it and pressing Alt+Enter
        3. repeat step 2 for each code snippet
*)


scene { text.var3x5("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.mono3x5("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.var4x5("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.mono4x5("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.mono6x6("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.mono7x10("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.var10x10("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.mono10x10("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

scene { text.mono16x16("abk").xy(0, 0).color(Colors.gray) }
|> Simulator.start "localhost"

// let f = Fonts.mono10x10
let f = Fonts.getBuiltinTypeface "10x10-monospaced-font.ttf" |> Fonts.buildFont 16 (Some -6)
scene { text("ABK").xy(0, 0).color(Colors.gray).font(f) } |> Simulator.start "localhost"
