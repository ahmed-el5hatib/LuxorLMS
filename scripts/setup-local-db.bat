@echo off
echo ========================================
echo   LuxorLMS Local Development Setup
echo ========================================
echo.

:: Check if PostgreSQL is installed
psql --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: PostgreSQL is not installed or not in PATH!
    echo.
    echo Please install PostgreSQL from: https://www.postgresql.org/download/windows/
    echo During installation, set password to: postgres
    echo.
    pause
    exit /b 1
)

echo PostgreSQL found!
echo.

:: Create databases
echo Creating databases...
psql -U postgres -c "CREATE DATABASE luxorlms_identity;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_academic;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_registration;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_forums;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_notifications;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_storage;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_attendance;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_grading;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_quizzes;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_analytics;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_reporting;" 2>nul
psql -U postgres -c "CREATE DATABASE luxorlms_administration;" 2>nul

echo.
echo ========================================
echo   Setup Complete!
echo ========================================
echo.
echo Next steps:
echo   1. Run start-services.bat to start all APIs
echo   2. Open frontend in another terminal: cd frontend ^&^& npm run dev
echo.
pause
