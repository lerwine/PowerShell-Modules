. '..\..\VsProject.ps1';
$OutputDir = $MyInvocation.MyCommand.Definition | Split-Path -Parent;
Invoke-BuildVsProject -SourceProject (($OutputDir | Split-Path -Parent) | Join-Path -ChildPath 'GDIPlus/GDIPlus.csproj') -OutputDir $OutputDir;