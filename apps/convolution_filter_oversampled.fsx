#r "nuget: Pxl, 0.0.39"

open Pxl.Ui.CSharp
open type Pxl.Ui.CSharp.DrawingContext
open SkiaSharp

let conf = {|
    blurRadius = 2.0          // Size of blur kernel (in base pixels)
    sigma = 1.8               // Gaussian spread:
                              //     0.3-0.5 = tight/sharp glow
                              //     0.7-1.0 = natural soft blur,
                              //     1.5+ = very diffuse/foggy
    oversamplingFactor = 16   // Quality: higher = smoother but slower
|}


let pattern =  [

    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  1  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  1  -  -  -  -  -  -  -  -  -  -  -  -  -  1  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  1  2  1  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  1  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  2  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "
    "       -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -       "

]


let pallette =
    [|
        "-", Color.FromHsl(0.0, 0.5, 0.05)
        "1", Colors.Red
        "2", Colors.Blue
    |]


let data =
    let pallette = Map.ofArray pallette
    [|
        for line in pattern do
            let chars = line.Replace(" ", "").ToCharArray() |> Array.map (fun c -> c.ToString()) 
            for s in chars do
                pallette[s]
    |]

let blur radius sigma oversample (data: SKColor array) =
    let baseWidth, baseHeight = 24, 24
    let width, height = baseWidth * oversample, baseHeight * oversample
    
    // Upsample
    let upsampled =
        [|
            for y in 0 .. height - 1 do
                for x in 0 .. width - 1 do
                    data[y / oversample * baseWidth + x / oversample]
        |]
    
    // Blur at high resolution (Gaussian)
    let intRadius = int (radius * float oversample)
    let scaledSigma = sigma * float oversample
    let gaussian d = exp (-(d * d) / (2.0 * scaledSigma * scaledSigma))
    let blurred = [|
        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                let mutable sumR, sumG, sumB, sumWeight = 0.0, 0.0, 0.0, 0.0
                for dy in -intRadius .. intRadius do
                    for dx in -intRadius .. intRadius do
                        let nx, ny = x + dx, y + dy
                        if nx >= 0 && nx < width && ny >= 0 && ny < height then
                            let dist = sqrt (float (dx * dx + dy * dy))
                            let weight = gaussian dist
                            let c = upsampled.[ny * width + nx]
                            sumR <- sumR + float c.Red * weight
                            sumG <- sumG + float c.Green * weight
                            sumB <- sumB + float c.Blue * weight
                            sumWeight <- sumWeight + weight
                sumR / sumWeight, sumG / sumWeight, sumB / sumWeight
    |]
    
    // Downsample
    [|
        for y in 0 .. baseHeight - 1 do
            for x in 0 .. baseWidth - 1 do
                let mutable sumR, sumG, sumB = 0.0, 0.0, 0.0
                for sy in 0 .. oversample - 1 do
                    for sx in 0 .. oversample - 1 do
                        let (r, g, b) = blurred.[(y * oversample + sy) * width + (x * oversample + sx)]
                        sumR <- sumR + r
                        sumG <- sumG + g
                        sumB <- sumB + b
                let n = float (oversample * oversample)
                Color.FromRgb(byte (sumR / n), byte (sumG / n), byte (sumB / n))
    |]

let blurredData = blur conf.blurRadius conf.sigma conf.oversamplingFactor data

let scene () =
    Ctx.SetPixels(blurredData, blendMode = BlendMode.Source)



// PXL.Simulate(scene) |> Async.AwaitTask |> Async.RunSynchronously
// PXL.SendToDevice(scene, "192.168.178.52") |> Async.AwaitTask |> Async.RunSynchronously
PXL.SimulateAndSendToDevice(scene, "192.168.178.52") |> Async.AwaitTask |> Async.RunSynchronously
