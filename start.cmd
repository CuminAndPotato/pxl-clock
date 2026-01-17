@echo off
where bash >nul 2>nul
if %ERRORLEVEL% == 0 (
    bash "%~dp0start.sh"
) else (
    echo.
    echo  Bash not found! Please install one of:
    echo.
    echo  1. Git for Windows: https://git-scm.com/download/win
    echo  2. WSL: https://learn.microsoft.com/windows/wsl/install
    echo.
    pause
)
