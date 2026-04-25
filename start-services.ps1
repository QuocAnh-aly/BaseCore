# Start BaseCore Microservices
Write-Host "Starting BaseCore Microservices..." -ForegroundColor Green

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Start AuthService
Write-Host "Starting AuthService (Port 5002)..." -ForegroundColor Yellow
$authJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    dotnet run --urls "http://localhost:5002"
} -ArgumentList "$scriptDir\BaseCore.AuthService"

Start-Sleep -Seconds 3

# Start APIService
Write-Host "Starting APIService (Port 5001)..." -ForegroundColor Yellow
$apiJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    dotnet run --urls "http://localhost:5001"
} -ArgumentList "$scriptDir\BaseCore.APIService"

Start-Sleep -Seconds 3

# Start ApiGateway
Write-Host "Starting ApiGateway (Port 5000)..." -ForegroundColor Yellow
$gatewayJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    dotnet run --urls "http://localhost:5000"
} -ArgumentList "$scriptDir\BaseCore.ApiGateway"

Write-Host ""
Write-Host "All services started!" -ForegroundColor Green
Write-Host ""
Write-Host "Access URLs:" -ForegroundColor Cyan
Write-Host "- API Gateway: http://localhost:5000"
Write-Host "- Admin App:    http://localhost:5000/admin/"
Write-Host "- User App:     http://localhost:5000/user/"
Write-Host "- API Docs:     http://localhost:5000/swagger"
Write-Host ""
Write-Host "Press Ctrl+C to stop all services" -ForegroundColor Yellow

# Wait for user input to stop
try {
    Read-Host "Press Enter to stop all services"
} finally {
    Write-Host "Stopping services..." -ForegroundColor Red
    Stop-Job $authJob, $apiJob, $gatewayJob
    Remove-Job $authJob, $apiJob, $gatewayJob
}