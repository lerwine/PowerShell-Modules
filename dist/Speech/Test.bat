@ECHO OFF
SET BatchPath=%~dp0

cd "%BatchPath%"

rem powershell -STA -WindowStyle Hidden -ExecutionPolicy Bypass -File Test.ps1
powershell -STA -ExecutionPolicy Bypass -File Test.ps1
pause