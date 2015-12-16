SET BatchPath=%~dp0
pushd
cd "%BatchPath%"
powershell -ExecutionPolicy Bypass -File EncryptFile.ps1 -STA
popd
pause