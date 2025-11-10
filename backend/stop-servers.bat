@echo off
echo Stopping all servers...
echo.

REM Kill Python processes (api.py)
echo Stopping Python Face Recognition Server...
taskkill /FI "WINDOWTITLE eq Python Face Recognition Server*" /T /F >nul 2>&1
taskkill /F /IM python.exe /FI "WINDOWTITLE eq *api.py*" >nul 2>&1

REM Kill C# .NET processes
echo Stopping C# Backend Server...
taskkill /FI "WINDOWTITLE eq C# Backend Server*" /T /F >nul 2>&1
taskkill /F /IM dotnet.exe >nul 2>&1

echo.
echo All servers stopped.
timeout /t 2 /nobreak >nul

