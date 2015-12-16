SET BatchPath=%~dp0
pushd
cd "%BatchPath%"
powershell -ExecutionPolicy Bypass -File CredentialManager.ps1 -STA
popd
pause