@echo off
setlocal

REM Deschide o fereastră pentru dotnet run
start "API Server" cmd /k "dotnet run"

REM Deschide o altă fereastră pentru ngrok
start "Ngrok Tunnel" cmd /k "ngrok http 5094"

endlocal
