# LuxorLMS Local Development Setup Script
# This script sets up PostgreSQL databases for local development

param(
    [string]$PgPassword = "postgres",
    [string]$PgUser = "postgres",
    [string]$PgHost = "localhost",
    [int]$PgPort = 5432
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  LuxorLMS Local Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if PostgreSQL is installed
$pgInstalled = Get-Command "psql" -ErrorAction SilentlyContinue
if (-not $pgInstalled) {
    Write-Host "ERROR: PostgreSQL is not installed or not in PATH!" -ForegroundColor Red
    Write-Host "Please install PostgreSQL from: https://www.postgresql.org/download/windows/" -ForegroundColor Yellow
    Write-Host "During installation, set password to: $PgPassword" -ForegroundColor Yellow
    exit 1
}

Write-Host "PostgreSQL found: $($pgInstalled.Source)" -ForegroundColor Green
Write-Host ""

# Set PGPASSWORD environment variable for this session
$env:PGPASSWORD = $PgPassword

# Databases to create
$databases = @(
    "luxorlms_identity",
    "luxorlms_academic",
    "luxorlms_registration",
    "luxorlms_forums",
    "luxorlms_notifications",
    "luxorlms_storage",
    "luxorlms_attendance",
    "luxorlms_grading",
    "luxorlms_quizzes",
    "luxorlms_analytics",
    "luxorlms_reporting",
    "luxorlms_administration"
)

Write-Host "Creating databases..." -ForegroundColor Yellow
foreach ($db in $databases) {
    $checkDb = psql -h $PgHost -p $PgPort -U $PgUser -tc "SELECT 1 FROM pg_database WHERE datname = '$db'" 2>&1
    if ($checkDb -match "1") {
        Write-Host "  Database '$db' already exists, skipping..." -ForegroundColor DarkGray
    } else {
        $result = psql -h $PgHost -p $PgPort -U $PgUser -c "CREATE DATABASE $db;" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  Created database: $db" -ForegroundColor Green
        } else {
            Write-Host "  Failed to create database: $db" -ForegroundColor Red
            Write-Host "  Error: $result" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Update appsettings.json files with your PostgreSQL credentials" -ForegroundColor White
Write-Host "2. Run the APIs using the commands below" -ForegroundColor White
Write-Host ""
Write-Host "To run all APIs:" -ForegroundColor Yellow
Write-Host "  cd src/LuxorLMS.Identity.Api && dotnet run --urls 'http://localhost:5001'" -ForegroundColor White
Write-Host "  cd src/LuxorLMS.Academic.Api && dotnet run --urls 'http://localhost:5002'" -ForegroundColor White
Write-Host "  cd src/LuxorLMS.Registration.Api && dotnet run --urls 'http://localhost:5003'" -ForegroundColor White
Write-Host "  # ... etc" -ForegroundColor White
Write-Host ""
Write-Host "To run frontend:" -ForegroundColor Yellow
Write-Host "  cd frontend && npm run dev" -ForegroundColor White
Write-Host ""
