@ECHO OFF
SET BatchPath=%~dp0

pushd

cd "%BatchPath%"

powershell -STA -ExecutionPolicy Bypass -File PasswordStorage.ps1

popd

pause