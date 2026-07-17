# LuxorLMS Local Development Setup Script
# This script starts all backend APIs and frontend dev server

param(
    [int]$FrontendPort = 5173
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  LuxorLMS Local Development" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to start a service
function Start-Service {
    param(
        [string]$Name,
        [int]$Port,
        [string]$Path
    )
    
    Write-Host "[$Name] Starting on port $Port..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"$Path`"; dotnet run --urls `"http://localhost:$Port`"" -WindowTitle "$Name"
    Start-Sleep -Seconds 2
}

# Start all backend APIs
Write-Host "Starting Backend APIs..." -ForegroundColor Green
Write-Host ""

Start-Service -Name "Identity API" -Port 5001 -Path "src/LuxorLMS.Identity.Api"
Start-Service -Name "Academic API" -Port 5002 -Path "src/LuxorLMS.Academic.Api"
Start-Service -Name "Registration API" -Port 5003 -Path "src/LuxorLMS.Registration.Api"
Start-Service -Name "Forums API" -Port 5004 -Path "src/LuxorLMS.Forums.Api"
Start-Service -Name "Notifications API" -Port 5005 -Path "src/LuxorLMS.Notifications.Api"
Start-Service -Name "Storage API" -Port 5006 -Path "src/LuxorLMS.Storage.Api"
Start-Service -Name "Attendance API" -Port 5007 -Path "src/LuxorLMS.Attendance.Api"
Start-Service -Name "Grading API" -Port 5008 -Path "src/LuxorLMS.Grading.Api"
Start-Service -Name "Quizzes API" -Port 5009 -Path "src/LuxorLMS.Quizzes.Api"
Start-Service -Name "Analytics API" -Port 5010 -Path "src/LuxorLMS.Analytics.Api"
Start-Service -Name "Reporting API" -Port 5011 -Path "src/LuxorLMS.Reporting.Api"
Start-Service -Name "Administration API" -Port 5012 -Path "src/LuxorLMS.Administration.Api"

Write-Host ""
Write-Host "Starting Frontend Dev Server..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"frontend`"; npm run dev" -WindowTitle "Frontend Dev Server"
Start-Sleep -Seconds 3

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  All Services Started!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Services running on:" -ForegroundColor Cyan
Write-Host "  Identity API:        http://localhost:5001" -ForegroundColor White
Write-Host "  Academic API:        http://localhost:5002" -ForegroundColor White
Write-Host "  Registration API:    http://localhost:5003" -ForegroundColor White
Write-Host "  Forums API:          http://localhost:5004" -ForegroundColor White
Write-Host "  Notifications API:   http://localhost:5005" -ForegroundColor White
Write-Host "  Storage API:         http://localhost:5006" -ForegroundColor White
Write-Host "  Attendance API:      http://localhost:5007" -ForegroundColor White
Write-Host "  Grading API:         http://localhost:5008" -ForegroundColor White
Write-Host "  Quizzes API:         http://localhost:5009" -ForegroundColor White
Write-Host "  Analytics API:       http://localhost:5010" -ForegroundColor White
Write-Host "  Reporting API:       http://localhost:5011" -ForegroundColor White
Write-Host "  Administration API:  http://localhost:5012" -ForegroundColor White
Write-Host "  Frontend:            http://localhost:$FrontendPort" -ForegroundColor White
Write-Host ""
Write-Host "Login credentials:" -ForegroundColor Yellow
Write-Host "  Email:    admin@luxorlms.com" -ForegroundColor White
Write-Host "  Password: Admin@123456" -ForegroundColor White
Write-Host ""
Write-Host "Press any key to stop all services..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Kill all services
Get-Process | Where-Object { $_.MainWindowTitle -like "*LuxorLMS*" -or $_.MainWindowTitle -like "*Frontend*" } | Stop-Process -Force
Write-Host ""
Write-Host "All services stopped." -ForegroundColor Green
