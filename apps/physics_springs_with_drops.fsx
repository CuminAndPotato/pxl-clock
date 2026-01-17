#r "nuget: Pxl, 0.0.39"

open Pxl.Ui.CSharp
open type Pxl.Ui.CSharp.DrawingContext

type HueMode =
    | Acceleration   // Rainbow based on acceleration (uses hueOffset, hueScale)
    | HeightSign     // Smooth hue based on height (uses maxDisplayHeight for range)
    | VelocityDirection  // Smooth hue based on velocity
    | Monochrome     // Grayscale (saturation forced to 0)
    | Fixed          // Fixed baseHue

let conf = {|
    // Physics
    springStrength = 20.0           // Stiffness of springs between neighbors (higher = faster wave propagation)
    groundSpringStiffness = 0.5     // Stiffness of spring to ground (higher = waves dampen spatially faster)
    damping = 0.99                  // Velocity damping per frame, 0-1 (lower = waves die out faster)
    mass = 0.3                      // Entity mass (higher = slower/sluggish waves, lower = faster/snappier)
    
    // Drop
    dropSpawnRect = {| x = 3; y = 3; width = 18; height = 10 |}  // Rect within display (x,y=top-left corner)
    dropHeight = 20.0               // Max height of drop (should match maxDisplayHeight for full brightness)
    dropEaseInDuration = 10.5       // Seconds to rise to dropHeight
    dropStayDuration = 5.0          // Seconds to hold at dropHeight
    dropEaseOutDuration = 0.5       // Seconds to fall back to 0
    dropTimeMode = 1                // 0=diff (interval after drop ends), 1=abs (fixed interval)
    dropTimeIntervalOrDiff = 10.0   // Seconds between drops (mode 1) or after drop ends (mode 0)
    
    // Display
    baseBrightness = 0.15           // Lightness at height=0 (0-1)
    maxDisplayHeight = 10.0         // Height that maps to maxLightness (negative = invert)
    velocityToSaturation = 0.5      // Multiplier: velocity → saturation (higher = more color at low speeds)
    hueOffset = 10.0                // Acceleration offset for hue (mode 0 only)
    hueScale = 0.0                  // 0-1: acceleration sensitivity for hue (mode 0 only)
    baseHue = 0.0                   // Fixed hue 0-360 (used when hueMode > 2)
    hueRange = {| min = 180.0; max = 220.0 |}         // Hue range (min at low, max at high value)
    saturationRange = {| min = 0.0; max = 1.0 |}     // Saturation range: min at rest, max at high velocity
    lightnessRange = {| min = 0.0; max = 1.0 |}      // Lightness range: min=floor, max=ceiling for height mapping
    hueMode = HeightSign
|}


let DisplaySize = 24
let PhysicalSize = 44
let Offset = (PhysicalSize - DisplaySize) / 2

let random = System.Random()
let mutable elapsedTime = 0.0
let mutable lastDropTime = System.Double.MinValue  // Trigger first drop immediately

// Calculated: how much height affects brightness
let brightnessFactor () = (conf.lightnessRange.max - conf.baseBrightness) / conf.maxDisplayHeight

type Drop = { X: int; Y: int; StartTime: float; InitialHeight: float }
let drops = ResizeArray<Drop>()

type Cell = { Height: float; Velocity: float; Acceleration: float }
let grid = Array2D.init PhysicalSize PhysicalSize (fun _ _ -> { Height = 0.0; Velocity = 0.0; Acceleration = 0.0 })
let newGrid = Array2D.init PhysicalSize PhysicalSize (fun _ _ -> { Height = 0.0; Velocity = 0.0; Acceleration = 0.0 })

// Ease function (smooth step)
let easeInOut t = t * t * (3.0 - 2.0 * t)

let getDropHeight startTime initialHeight =
    let age = elapsedTime - startTime
    let totalDuration = conf.dropEaseInDuration + conf.dropStayDuration + conf.dropEaseOutDuration

    if age < 0.0 || age > totalDuration then
        0.0
    elif age < conf.dropEaseInDuration then
        // Ease in from initial height to dropHeight
        let t = age / conf.dropEaseInDuration
        initialHeight + easeInOut(t) * (conf.dropHeight - initialHeight)
    elif age < conf.dropEaseInDuration + conf.dropStayDuration then
        // Stay
        conf.dropHeight
    else
        // Ease out
        let t = (age - conf.dropEaseInDuration - conf.dropStayDuration) / conf.dropEaseOutDuration
        (1.0 - easeInOut(t)) * conf.dropHeight

let createDrop () =
    // x controls row, y controls column in the pixel layout
    let x = Offset + conf.dropSpawnRect.y + random.Next(0, conf.dropSpawnRect.height)  // row
    let y = Offset + conf.dropSpawnRect.x + random.Next(0, conf.dropSpawnRect.width)   // column
    let initialHeight = grid[x, y].Height
    drops.Add({ X = x; Y = y; StartTime = elapsedTime; InitialHeight = initialHeight })

// Get height at position, accounting for active drops
let getHeight x y =
    if x < 0 || x >= PhysicalSize || y < 0 || y >= PhysicalSize then
        0.0
    else
        // Check for active drop at this position
        let activeDrop = 
            drops 
            |> Seq.tryFind (fun drop -> drop.X = x && drop.Y = y)
        match activeDrop with
        | Some drop -> getDropHeight drop.StartTime drop.InitialHeight
        | None -> grid[x, y].Height

// Check if position has an active drop
let getActiveDropHeight x y =
    drops 
    |> Seq.tryFind (fun drop -> drop.X = x && drop.Y = y)
    |> Option.map (fun drop -> getDropHeight drop.StartTime drop.InitialHeight)

let updatePhysics dt =
    // First pass: calculate new velocities and heights based on current state
    for x in 0 .. PhysicalSize - 1 do
        for y in 0 .. PhysicalSize - 1 do
            let entity = grid[x, y]

            // Check if this is an active drop position
            match getActiveDropHeight x y with
            | Some activeHeight ->
                // Keep the entity at the forced height, but preserve velocity for smooth release
                newGrid[x, y] <- { Height = activeHeight; Velocity = entity.Velocity; Acceleration = 0.0 }
            | None ->
                // Spring forces from neighbors
                // If a neighbor is higher than us, it pulls us up (positive force)
                // If a neighbor is lower than us, we get pulled down (negative force)
                let mutable springForce = 0.0
                springForce <- springForce + getHeight (x - 1) y - entity.Height
                springForce <- springForce + getHeight (x + 1) y - entity.Height
                springForce <- springForce + getHeight x (y - 1) - entity.Height
                springForce <- springForce + getHeight x (y + 1) - entity.Height
                springForce <- springForce * conf.springStrength

                // Ground spring pulls entity back to rest height (0)
                let groundForce = conf.groundSpringStiffness * (0.0 - entity.Height)

                let totalForce = springForce + groundForce
                let acceleration = totalForce / conf.mass
                let newVelocity = (entity.Velocity + acceleration * dt) * conf.damping
                let newHeight = entity.Height + newVelocity * dt

                let measuredAcceleration = (newVelocity - entity.Velocity) / dt
                newGrid[x, y] <- { Height = newHeight; Velocity = newVelocity; Acceleration = measuredAcceleration }

    // Second pass: copy new state back to grid
    for x in 0 .. PhysicalSize - 1 do
        for y in 0 .. PhysicalSize - 1 do
            grid[x, y] <- newGrid[x, y]

let scene () =
    let dt = 1.0 / 30.0
    elapsedTime <- elapsedTime + dt

    // Create new drop based on timing mode
    let totalDropDuration = conf.dropEaseInDuration + conf.dropStayDuration + conf.dropEaseOutDuration
    let spawnInterval = 
        if conf.dropTimeMode = 0 then
            totalDropDuration + conf.dropTimeIntervalOrDiff  // diff: wait for drop to finish + interval
        else
            conf.dropTimeIntervalOrDiff                       // abs: fixed interval
    
    if elapsedTime - lastDropTime >= spawnInterval then
        createDrop ()
        lastDropTime <- elapsedTime

    // Remove finished drops
    drops.RemoveAll(fun drop -> elapsedTime - drop.StartTime > totalDropDuration) |> ignore

    updatePhysics dt

    // Display only the middle 24x24 square
    for x in 0 .. DisplaySize - 1 do
        for y in 0 .. DisplaySize - 1 do
            let gx = x + Offset
            let gy = y + Offset
            let cell = grid[gx, gy]
            let height = cell.Height
            let velocity = cell.Velocity
            let acceleration = cell.Acceleration

            // Hue based on selected mode
            let hue = 
                match conf.hueMode with
                | Acceleration -> ((acceleration + conf.hueOffset) * conf.hueScale * 360.0 % 360.0 + 360.0) % 360.0
                | HeightSign -> conf.hueRange.min + (conf.hueRange.max - conf.hueRange.min) * (System.Math.Clamp((height / conf.maxDisplayHeight + 1.0) / 2.0, 0.0, 1.0))
                | VelocityDirection -> conf.hueRange.min + (conf.hueRange.max - conf.hueRange.min) * (System.Math.Clamp((velocity / 50.0 + 1.0) / 2.0, 0.0, 1.0))
                | Monochrome | Fixed -> conf.baseHue

            // Saturation based on velocity magnitude (0 for monochrome mode)
            let saturation = 
                match conf.hueMode with
                | Monochrome -> 0.0
                | _ -> System.Math.Clamp(System.Math.Abs(velocity) * conf.velocityToSaturation, conf.saturationRange.min, conf.saturationRange.max)

            // Lightness based on height with base brightness
            let lightness = System.Math.Clamp(conf.baseBrightness + height * (brightnessFactor ()), conf.lightnessRange.min, conf.lightnessRange.max)

            let pixelIndex = x * DisplaySize + y
            Ctx.Pixels[pixelIndex] <- Color.FromHsl360(hue, saturation, lightness)




// PXL.Simulate(scene) |> Async.AwaitTask |> Async.RunSynchronously
// PXL.SendToDevice(scene, "192.168.178.52") |> Async.AwaitTask |> Async.RunSynchronously
PXL.SimulateAndSendToDevice(scene, "192.168.178.52") |> Async.AwaitTask |> Async.RunSynchronously

