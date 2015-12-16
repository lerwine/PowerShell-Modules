SET BatchPath=%~dp0
pushd
cd "%BatchPath%"
powershell -ExecutionPolicy Bypass -File DecryptFile.ps1 -STA
popd
pause