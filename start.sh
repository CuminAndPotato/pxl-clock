#!/bin/bash
# filepath: /Users/ronald/repos/github.CnP/PXL-Clock/start.sh
# Start Simulator and C# Watcher concurrently

# Run setup check
./build/setup-check.sh
if [ $? -ne 0 ]; then
    exit 1
fi

echo ""
echo "🚀 Starting PXL Clock development environment..."
echo ""

# Start the Simulator in the background
./build/start-simulator.sh &

# Start the C# Watcher in the background
./build/start-watcher.sh &

echo ""
echo "💡 Save any .cs or .fsx file in the apps/ folder to send it to the simulator"
echo ""

# Wait for both processes to finish (if they ever do)
wait
