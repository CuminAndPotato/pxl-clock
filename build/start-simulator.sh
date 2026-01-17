#!/bin/bash
set -e

echo "Downloading simulator ..."
dotnet tool restore

# Start simulator in background
dotnet Pxl.Simulator &
SIMULATOR_PID=$!

# Wait for the simulator to be reachable (max 10 seconds)
echo "⏳ Waiting for simulator..."
for i in {1..20}; do
    if curl -s --head http://127.0.0.1:5001 > /dev/null 2>&1; then
        echo "✅ Simulator ready at http://127.0.0.1:5001"
        break
    fi
    sleep 0.5
done

# Open browser (works on macOS, Linux, and WSL)
if command -v open &> /dev/null; then
    open http://127.0.0.1:5001
elif command -v xdg-open &> /dev/null; then
    xdg-open http://127.0.0.1:5001
elif command -v wslview &> /dev/null; then
    wslview http://127.0.0.1:5001
fi

# Wait for simulator process
wait $SIMULATOR_PID
