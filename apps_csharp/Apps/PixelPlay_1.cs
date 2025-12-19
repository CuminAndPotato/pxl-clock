#:package Pxl@0.0.31

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

var scene = () =>
{
    // Blue background
    Ctx.Background.Solid(Colors.Blue);

    // Random colored pixels
    var random = new Random();
    for (int i = 0; i < Ctx.Pixels.Length; i++)
    {
        Ctx.Pixels[i] = Color.FromRgb(
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256));
    }

    // Black "HELLO" text at (0,10)
    Ctx.Text.Mono4x5("HELLO", 0, 10, Colors.Black);

    // Change black pixels to blue (actually, the text pixels)
    for (int i = 0; i < Ctx.Pixels.Length; i++)
    {
        var color = Ctx.Pixels[i];
        if (color.Red == 0 && color.Green == 0 && color.Blue == 0)
        {
            Ctx.Pixels[i] = Colors.Blue;
        }
    }
};




await PXL.Simulate(scene);
// await PXL.SendToDevice(scene, "DeviceIP_or_NameInNetwork");
