#!/bin/bash

# PXL Clock - Setup Verification Script
# This script checks if all prerequisites are installed

echo "🔧 PXL Clock - Setup Verification"
echo "=================================="
echo ""

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Track if all checks pass
ALL_CHECKS_PASSED=true

# Check for .NET SDK
echo "Checking for .NET SDK..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓${NC} .NET SDK found: version $DOTNET_VERSION"
    
    # Check if it's .NET 10 or higher
    MAJOR_VERSION=$(echo $DOTNET_VERSION | cut -d'.' -f1)
    if [ "$MAJOR_VERSION" -ge 10 ]; then
        echo -e "${GREEN}✓${NC} .NET SDK 10+ detected"
    else
        echo -e "${YELLOW}⚠${NC}  .NET SDK $DOTNET_VERSION found, but .NET 10+ is recommended"
        echo "   Download: https://dotnet.microsoft.com/en-us/download/dotnet/10.0"
        ALL_CHECKS_PASSED=false
    fi
else
    echo -e "${RED}✗${NC} .NET SDK not found"
    echo "   Please install .NET SDK 10:"
    echo "   https://dotnet.microsoft.com/en-us/download/dotnet/10.0"
    ALL_CHECKS_PASSED=false
fi

echo ""

# Check for Visual Studio Code
echo "Checking for Visual Studio Code..."
if command -v code &> /dev/null; then
    echo -e "${GREEN}✓${NC} VS Code found"
    
    # Check for C# DevKit extension
    echo ""
    echo "Checking for VS Code extensions..."
    
    if code --list-extensions 2>/dev/null | grep -q "ms-dotnettools.csdevkit"; then
        echo -e "${GREEN}✓${NC} C# Dev Kit extension installed"
    else
        echo -e "${YELLOW}⚠${NC}  C# Dev Kit extension not found"
        echo "   VS Code will prompt you to install recommended extensions"
        echo "   Or install manually: code --install-extension ms-dotnettools.csdevkit"
    fi
    
    if code --list-extensions 2>/dev/null | grep -q "ionide.ionide-fsharp"; then
        echo -e "${GREEN}✓${NC} Ionide F# extension installed"
    else
        echo -e "${YELLOW}⚠${NC}  Ionide F# extension not found (optional)"
        echo "   Install: code --install-extension ionide.ionide-fsharp"
    fi
else
    echo -e "${YELLOW}⚠${NC}  VS Code CLI not found (VS Code may be installed without CLI)"
    echo "   If VS Code is not installed, download from: https://code.visualstudio.com/"
fi

echo ""
echo "=================================="

if [ "$ALL_CHECKS_PASSED" = true ]; then
    echo -e "${GREEN}✓ All prerequisites are installed!${NC}"
    echo ""
    echo "You're ready to start developing:"
    echo "  1. Run: ./start.sh"
    echo "  2. Open: http://localhost:5001"
    echo "  3. Edit: apps/Program.cs"
else
    echo -e "${YELLOW}⚠ Some prerequisites are missing or outdated${NC}"
    echo ""
    echo "Please install the missing components and run this script again."
fi

echo ""
