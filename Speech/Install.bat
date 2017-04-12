@ECHO OFF
cd "%~dp0"
powershell -STA -ExecutionPolicy Bypass -File "%~f0.ps1"
pause
