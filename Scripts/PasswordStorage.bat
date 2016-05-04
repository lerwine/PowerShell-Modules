@ECHO OFF
SET BatchPath=%~dp0

cd "%BatchPath%"

powershell -STA -WindowStyle Hidden -ExecutionPolicy Bypass -File PasswordStorage.ps1
