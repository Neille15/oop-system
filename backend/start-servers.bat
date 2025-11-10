@echo off
echo Starting both servers...
echo.

REM Start Python Face Recognition Server
echo Starting Python Face Recognition Server on port 5000...
start "Python Face Recognition Server" cmd /k "cd python-face-recog && python api.py"
timeout /t 2 /nobreak >nul

REM Start C# Backend Server
echo Starting C# Backend Server on port 5207...
start "C# Backend Server" cmd /k "cd oop_backend && dotnet run"

echo.
echo Both servers are starting...
echo Python Face Recognition: http://127.0.0.1:5000
echo C# Backend API: http://localhost:5207
echo.
echo Press any key to close this window (servers will continue running)...
pause >nul

