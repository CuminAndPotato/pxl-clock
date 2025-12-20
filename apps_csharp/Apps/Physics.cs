/*
    A physics simulation of a 2D grid in 3D space of spring-connected entities,
    with periodic "drops" that create waves propagating through the grid.

    Nice parameters:
        - slow drop-in, quick drop-out: A star is born and then quickly collapses
*/

#:package Pxl@0.0.32

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// parameters
var springStrength = 10.0;          // Strong coupling between neighbors
var groundSpringStiffness = 0.5;    // Weak ground anchor (allows wave to spread)
var damping = 0.99;                 // Very light damping for sustained waves
var mass = 0.3;                     // Higher = slower/sluggish waves, lower = faster/snappier waves

var dropHeight = 40.0;               // corresponds to maxDisplayHeight
var dropTimeInterval = 5.0;
var dropEaseInDuration = 5.5;
var dropStayDuration = 1.2;
var dropEaseOutDuration = 1.5;

var baseBrightness = 0.2;          // Base brightness (0-1) - sets the gray-tone around the zero-height
var maxDisplayHeight = 20.0;       // Height that maps to maxLightness (use negative to flip black/white)
var velocityToSaturation = 0.2;    // How much velocity affects saturation

var hueOffset = 10.0;       // Height offset for hue calculation
var hueScale = 12.0;        // How much height affects hue
var minSaturation = 0.0;    // Minimum saturation
var maxSaturation = 0.3;    // Maximum saturation
var minLightness = 0.1;     // Minimum lightness
var maxLightness = 0.9;     // Maximum lightness



const int DisplaySize = 24;
const int PhysicalSize = 44;
const int Offset = 10;

var random = new Random();
var elapsedTime = 0.0;
var lastDropTime = -dropTimeInterval;  // Trigger first drop immediately

// Calculated: how much height affects brightness
var brightnessFactor = (maxLightness - baseBrightness) / maxDisplayHeight;

// Active drops: (x, y, startTime)
var drops = new List<(int X, int Y, double StartTime)>();

var grid = new (double height, double velocity, double acceleration)[PhysicalSize, PhysicalSize];
var newGrid = new (double height, double velocity, double acceleration)[PhysicalSize, PhysicalSize];

// Ease function (smooth step)
var easeInOut = (double t) => t * t * (3 - 2 * t);

var getDropHeight = (double startTime) =>
{
    var age = elapsedTime - startTime;
    var totalDuration = dropEaseInDuration + dropStayDuration + dropEaseOutDuration;

    if (age < 0 || age > totalDuration)
        return 0.0;

    if (age < dropEaseInDuration)
    {
        // Ease in
        var t = age / dropEaseInDuration;
        return easeInOut(t) * dropHeight;
    }
    else if (age < dropEaseInDuration + dropStayDuration)
    {
        // Stay
        return dropHeight;
    }
    else
    {
        // Ease out
        var t = (age - dropEaseInDuration - dropStayDuration) / dropEaseOutDuration;
        return (1 - easeInOut(t)) * dropHeight;
    }
};

var createDrop = () =>
{
    var x = Offset + random.Next(4, DisplaySize - 4);
    var y = Offset + random.Next(4, DisplaySize - 4);
    drops.Add((x, y, elapsedTime));
};

var getHeight = (int x, int y) =>
    x < 0 || x >= PhysicalSize || y < 0 || y >= PhysicalSize
        ? 0.0
        : grid[x, y].height;

// Check if position has an active drop
var getActiveDropHeight = (int x, int y) =>
{
    foreach (var drop in drops)
    {
        if (drop.X == x && drop.Y == y)
            return getDropHeight(drop.StartTime);
    }
    return (double?)null;
};

var updatePhysics = (double dt) =>
{
    // First pass: calculate new velocities and heights based on current state
    foreach (var (x, y) in Grid(PhysicalSize))
    {
        var entity = grid[x, y];

        // Check if this is an active drop position
        var activeHeight = getActiveDropHeight(x, y);
        if (activeHeight.HasValue)
        {
            // Keep the entity at the forced height, but preserve velocity for smooth release
            newGrid[x, y] = (activeHeight.Value, entity.velocity, 0.0);
            continue;
        }

        // Spring forces from neighbors
        // If a neighbor is higher than us, it pulls us up (positive force)
        // If a neighbor is lower than us, we get pulled down (negative force)
        var springForce = 0.0;
        springForce += getHeight(x - 1, y) - entity.height;
        springForce += getHeight(x + 1, y) - entity.height;
        springForce += getHeight(x, y - 1) - entity.height;
        springForce += getHeight(x, y + 1) - entity.height;
        springForce *= springStrength;

        // Ground spring pulls entity back to rest height (0)
        var groundForce = groundSpringStiffness * (0.0 - entity.height);

        var totalForce = springForce + groundForce;
        var acceleration = totalForce / mass;
        var newVelocity = (entity.velocity + acceleration * dt) * damping;
        var newHeight = entity.height + newVelocity * dt;

        var measuredAcceleration = (newVelocity - entity.velocity) / dt;
        newGrid[x, y] = (newHeight, newVelocity, measuredAcceleration);
    }

    // Second pass: copy new state back to grid
    foreach (var (x, y) in Grid(PhysicalSize))
        grid[x, y] = newGrid[x, y];
};

var scene = () =>
{
    var dt = 1.0 / 30.0;
    elapsedTime += dt;

    // Create new drop if interval passed
    if (elapsedTime - lastDropTime >= dropTimeInterval)
    {
        createDrop();
        lastDropTime = elapsedTime;
    }

    // Remove finished drops
    var totalDropDuration = dropEaseInDuration + dropStayDuration + dropEaseOutDuration;
    drops.RemoveAll(d => elapsedTime - d.StartTime > totalDropDuration);

    updatePhysics(dt);

    // Display only the middle 24x24 square
    foreach (var (x, y) in Grid(DisplaySize))
    {
        var gx = x + Offset;
        var gy = y + Offset;
        var cell = grid[gx, gy];
        var height = cell.height;
        var velocity = cell.velocity;
        var acceleration = cell.acceleration;

        // Hue based on acceleration
        var hue = (acceleration + hueOffset) * hueScale;  // Map acceleration to hue (0-360)
        hue = ((hue % 360) + 360) % 360; // Wrap around

        // Saturation based on velocity magnitude
        var saturation = Math.Clamp(Math.Abs(velocity) * velocityToSaturation, minSaturation, maxSaturation);

        // Lightness based on height with base brightness
        var lightness = Math.Clamp(baseBrightness + height * brightnessFactor, minLightness, maxLightness);

        var pixelIndex = x * DisplaySize + y;
        Ctx.Pixels[pixelIndex] = Color.FromHsl360(hue, saturation, lightness);
    }
};


await PXL.Simulate(scene);
// await PXL.SendToDevice(scene, "192.168.178.52");


