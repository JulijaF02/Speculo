<#
.SYNOPSIS
    Standardized script to build the project and run NUnit tests.
#>

Write-Host "--- Starting Speculo Build & Test Suite ---" -ForegroundColor Cyan

# 1. Restore
Write-Host "[1/3] Restoring packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Restore failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 2. Build
Write-Host "[2/3] Building solution..." -ForegroundColor Yellow
dotnet build --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Build failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 3. Test
Write-Host "[3/3] Running NUnit Tests..." -ForegroundColor Yellow
dotnet test --no-build --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "X Tests Failed!" -ForegroundColor Red
} else {
    Write-Host "V All Tests Passed!" -ForegroundColor Green
}

Write-Host "--- Process Complete ---" -ForegroundColor Cyan
