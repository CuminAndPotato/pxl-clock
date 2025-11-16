#r "nuget: Pxl, 0.0.18"

open System
open Pxl
open Pxl.Ui



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


let finalScene =
    scene {
        let! ctx = getCtx()

        let waveFunction = wave(ctx.halfWidth, ctx.halfHeight, 1, 8.0, 0.7, 0.0)
        
        shaderGenerative (fun p ->
            let pxl = waveFunction ctx.elapsed.TotalSeconds p.x p.y
            Colors.white.brightness(pxl)
        )
    }


finalScene |> Simulator.start "localhost"


(*
Simulator.stop ()
*)

