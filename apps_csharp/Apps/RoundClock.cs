using System;
using Pxl.Ui.CSharp;
using SkiaSharp;
using static Pxl.Ui.CSharp.Drawing;

static class DemoApps
{
    public static Action RoundClock = () =>
    {
        var now = Ctx.Now;
        var isSmooth = true;

        var clockColor = Colors.White;
        var color1 = clockColor;
        var color2 = Colors.Black;

        // Swap colors every other minute
        var arcFg = now.Minute % 2 == 0 ? color1 : color2;
        var arcBg = now.Minute % 2 == 0 ? color2 : color1;

        // Background (Sekundenzeiger - second hand)
        Ctx.RectXyWh(0, 0, Ctx.Width, Ctx.Height).Fill.Solid(arcBg);

        // Calculate angle for the arc (second hand)
        var secFrac = isSmooth ? now.Millisecond : 0;
        var angleFor1s = (now.Second * 1000 + secFrac) * 360.0 / 60000.0;
        var angle = angleFor1s % 360.0;

        // Draw arc
        Ctx
            .Arc(-20, -20, Ctx.Width + 40.0, Ctx.Height + 40.0, -90, angle)
            .Fill.Solid(arcFg, isAntialias: isSmooth);

        // Draw black circle in the center
        var radius = Math.Min(Ctx.HalfWidth, Ctx.HalfHeight) - 1;
        Ctx
            .Circle(Ctx.HalfWidth, Ctx.HalfHeight, radius)
            .Fill.Solid(color2, isAntialias: isSmooth);

        var timeText = $"{now:HH}:{now:mm}";

        Ctx
            .Text().Var3x5(timeText, 3, 9)
            .Brush.Solid(color1);
    };
}
