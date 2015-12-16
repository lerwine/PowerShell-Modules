@ECHO OFF
SET BatchPath=%~dp0
pushd
cd "%BatchPath%"
powershell -ExecutionPolicy Bypass -File Install.ps1
popd
pause