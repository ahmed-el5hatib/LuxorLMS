@echo off
echo ========================================
echo   Starting LuxorLMS Services...
echo ========================================
echo.

setlocal enabledelayedexpansion

:: Start Identity API
echo [1/13] Starting Identity API on port 5001...
start "Identity API" cmd /c "cd src/LuxorLMS.Identity.Api && dotnet run --urls http://localhost:5001"
timeout /t 3 /nobreak >nul

:: Start Academic API
echo [2/13] Starting Academic API on port 5002...
start "Academic API" cmd /c "cd src/LuxorLMS.Academic.Api && dotnet run --urls http://localhost:5002"
timeout /t 2 /nobreak >nul

:: Start Registration API
echo [3/13] Starting Registration API on port 5003...
start "Registration API" cmd /c "cd src/LuxorLMS.Registration.Api && dotnet run --urls http://localhost:5003"
timeout /t 2 /nobreak >nul

:: Start Forums API
echo [4/13] Starting Forums API on port 5004...
start "Forums API" cmd /c "cd src/LuxorLMS.Forums.Api && dotnet run --urls http://localhost:5004"
timeout /t 2 /nobreak >nul

:: Start Notifications API
echo [5/13] Starting Notifications API on port 5005...
start "Notifications API" cmd /c "cd src/LuxorLMS.Notifications.Api && dotnet run --urls http://localhost:5005"
timeout /t 2 /nobreak >nul

:: Start Storage API
echo [6/13] Starting Storage API on port 5006...
start "Storage API" cmd /c "cd src/LuxorLMS.Storage.Api && dotnet run --urls http://localhost:5006"
timeout /t 2 /nobreak >nul

:: Start Attendance API
echo [7/13] Starting Attendance API on port 5007...
start "Attendance API" cmd /c "cd src/LuxorLMS.Attendance.Api && dotnet run --urls http://localhost:5007"
timeout /t 2 /nobreak >nul

:: Start Grading API
echo [8/13] Starting Grading API on port 5008...
start "Grading API" cmd /c "cd src/LuxorLMS.Grading.Api && dotnet run --urls http://localhost:5008"
timeout /t 2 /nobreak >nul

:: Start Quizzes API
echo [9/13] Starting Quizzes API on port 5009...
start "Quizzes API" cmd /c "cd src/LuxorLMS.Quizzes.Api && dotnet run --urls http://localhost:5009"
timeout /t 2 /nobreak >nul

:: Start Analytics API
echo [10/13] Starting Analytics API on port 5010...
start "Analytics API" cmd /c "cd src/LuxorLMS.Analytics.Api && dotnet run --urls http://localhost:5010"
timeout /t 2 /nobreak >nul

:: Start Reporting API
echo [11/13] Starting Reporting API on port 5011...
start "Reporting API" cmd /c "cd src/LuxorLMS.Reporting.Api && dotnet run --urls http://localhost:5011"
timeout /t 2 /nobreak >nul

:: Start Administration API
echo [12/13] Starting Administration API on port 5012...
start "Administration API" cmd /c "cd src/LuxorLMS.Administration.Api && dotnet run --urls http://localhost:5012"
timeout /t 2 /nobreak >nul

:: Start Frontend Dev Server
echo [13/13] Starting Frontend Dev Server on port 5173...
start "Frontend" cmd /c "cd frontend && npm run dev"

echo.
echo ========================================
echo   All Services Started!
echo ========================================
echo.
echo Services running on:
echo   Identity API:        http://localhost:5001
echo   Academic API:        http://localhost:5002
echo   Registration API:    http://localhost:5003
echo   Forums API:          http://localhost:5004
echo   Notifications API:   http://localhost:5005
echo   Storage API:         http://localhost:5006
echo   Attendance API:      http://localhost:5007
echo   Grading API:         http://localhost:5008
echo   Quizzes API:         http://localhost:5009
echo   Analytics API:       http://localhost:5010
echo   Reporting API:       http://localhost:5011
echo   Administration API:  http://localhost:5012
echo   Frontend:            http://localhost:5173
echo.
echo Login credentials:
echo   Email:    admin@luxorlms.com
echo   Password: Admin@123456
echo.
echo Press any key to stop all services...
pause >nul

:: Kill all started processes
taskkill /FI "WINDOWTITLE eq LuxorLMS*" /F >nul 2>&1
echo.
echo All services stopped.
pause
