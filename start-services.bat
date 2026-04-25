@echo off
echo Starting BaseCore Microservices...

echo.
echo Starting AuthService (Port 5002)...
start "AuthService" cmd /c "cd /d %~dp0BaseCore.AuthService && dotnet run --urls http://localhost:5002"

timeout /t 3 /nobreak > nul

echo.
echo Starting APIService (Port 5001)...
start "APIService" cmd /c "cd /d %~dp0BaseCore.APIService && dotnet run --urls http://localhost:5001"

timeout /t 3 /nobreak > nul

echo.
echo Starting ApiGateway (Port 5000)...
start "ApiGateway" cmd /c "cd /d %~dp0BaseCore.ApiGateway && dotnet run --urls http://localhost:5000"

echo.
echo All services started!
echo.
echo Access URLs:
echo - API Gateway: http://localhost:5000
echo - Admin App:    http://localhost:5000/admin/
echo - User App:     http://localhost:5000/user/
echo - API Docs:     http://localhost:5000/swagger
echo.
pause