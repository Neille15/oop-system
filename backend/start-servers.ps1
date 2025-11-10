# PowerShell script to start both servers
Write-Host "Starting both servers..." -ForegroundColor Green
Write-Host ""

# Start Python Face Recognition Server
Write-Host "Starting Python Face Recognition Server on port 5000..." -ForegroundColor Yellow
$pythonProcess = Start-Process -FilePath "python" -ArgumentList "api.py" -WorkingDirectory "python-face-recog" -PassThru -WindowStyle Normal
Start-Sleep -Seconds 2

# Start C# Backend Server
Write-Host "Starting C# Backend Server on port 5207..." -ForegroundColor Yellow
$dotnetProcess = Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory "oop_backend" -PassThru -WindowStyle Normal

Write-Host ""
Write-Host "Both servers are starting..." -ForegroundColor Green
Write-Host "Python Face Recognition: http://127.0.0.1:5000" -ForegroundColor Cyan
Write-Host "C# Backend API: http://localhost:5207" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop both servers" -ForegroundColor Yellow

# Wait for user to stop
try {
    while ($true) {
        Start-Sleep -Seconds 1
        # Check if processes are still running
        if (-not $pythonProcess.HasExited -and -not $dotnetProcess.HasExited) {
            continue
        } else {
            Write-Host "One or both servers have stopped." -ForegroundColor Red
            break
        }
    }
} finally {
    # Cleanup: Kill processes if still running
    if (-not $pythonProcess.HasExited) {
        Write-Host "Stopping Python server..." -ForegroundColor Yellow
        Stop-Process -Id $pythonProcess.Id -Force
    }
    if (-not $dotnetProcess.HasExited) {
        Write-Host "Stopping C# server..." -ForegroundColor Yellow
        Stop-Process -Id $dotnetProcess.Id -Force
    }
    Write-Host "All servers stopped." -ForegroundColor Green
}

