@ECHO OFF
SET BatchPath=%~dp0
cd "%BatchPath%"
powershell -STA -ExecutionPolicy Bypass -File Setup.ps1
pause