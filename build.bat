@ECHO OFF
SET BatchPath=%~dp0
cd "%BatchPath%"
MODE CON: COLS=2048
powershell -STA -ExecutionPolicy Bypass -File Build.ps1 %*