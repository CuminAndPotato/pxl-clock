#:package Pxl@0.0.31

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// parameters
var springStrength = 50.0;   // Strong coupling between neighbors
var groundSpring = 2.5;      // Weak ground anchor (allows wave to spread)
var damping = 0.98;         // Very light damping for sustained waves
var mass = 1.0;

var dropHeight = 20.0;
var dropTimeInterval = 5.0;
var baseBrightness = 50.0;  // Brightness at Height=0



const int DisplaySize = 24;
const int GridSize = DisplaySize * 3;  // 72x72 physics grid
const int Offset = DisplaySize;        // Middle square starts at (24, 24)

var random = new Random();

// Pick one fixed entity at start (within display area)
var fixedX = Offset + random.Next(4, DisplaySize - 4);
var fixedY = Offset + random.Next(4, DisplaySize - 4);

var grid = new (double Height, double Velocity, double RestHeight)[GridSize, GridSize];
var newGrid = new (double Height, double Velocity, double RestHeight)[GridSize, GridSize];

var getHeight = (int x, int y) =>
    x < 0 || x >= GridSize || y < 0 || y >= GridSize
        ? 0.0
        : grid[x, y].Height;

var updatePhysics = (double dt) =>
{
    // First pass: calculate new velocities and heights based on current state
    for (var x = 0; x < GridSize; x++)
    {
        for (var y = 0; y < GridSize; y++)
        {
            // Skip the fixed entity
            if (x == fixedX && y == fixedY)
            {
                newGrid[x, y] = (dropHeight, 0.0, 0.0);
                continue;
            }

            var entity = grid[x, y];

            // Spring forces from neighbors
            // If a neighbor is higher than us, it pulls us up (positive force)
            // If a neighbor is lower than us, we get pulled down (negative force)
            var springForce = 0.0;
            springForce += getHeight(x - 1, y) - entity.Height;
            springForce += getHeight(x + 1, y) - entity.Height;
            springForce += getHeight(x, y - 1) - entity.Height;
            springForce += getHeight(x, y + 1) - entity.Height;
            springForce *= springStrength;

            // Ground spring pulls entity back to rest height
            var groundForce = groundSpring * (entity.RestHeight - entity.Height);

            var totalForce = springForce + groundForce;
            var acceleration = totalForce / mass;
            var newVelocity = (entity.Velocity + acceleration * dt) * damping;
            var newHeight = entity.Height + newVelocity * dt;

            newGrid[x, y] = (newHeight, newVelocity, entity.RestHeight);
        }
    }

    // Second pass: copy new state back to grid
    for (var x = 0; x < GridSize; x++)
    {
        for (var y = 0; y < GridSize; y++)
        {
            grid[x, y] = newGrid[x, y];
        }
    }
};

var scene = () =>
{
    var dt = 1.0 / 30.0;

    updatePhysics(dt);

    // Display only the middle 24x24 square
    for (var x = 0; x < DisplaySize; x++)
    {
        for (var y = 0; y < DisplaySize; y++)
        {
            var brightness = baseBrightness + grid[x + Offset, y + Offset].Height * 20;
            var colorValue = (byte)Math.Clamp(brightness, 0, 255);
            var pixelIndex = x * DisplaySize + y;
            Ctx.Pixels[pixelIndex] = Color.FromRgb(colorValue, colorValue, colorValue);
        }
    }
};

await PXL.Simulate(scene);