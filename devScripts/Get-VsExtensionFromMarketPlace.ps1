if ($null -eq (Get-Module -Name 'Erwine.Leonard.T.SwPackage')) { Remove-Module -Name 'Erwine.Leonard.T.SwPackage' }
if ($null -eq (Get-Module -Name 'Erwine.Leonard.T.IOUtility')) { Remove-Module -Name 'Erwine.Leonard.T.IOUtility' }
Import-Module '../dist/Erwine.Leonard.T.IOUtility' -ErrorAction Stop;
Import-Module '../dist/Erwine.Leonard.T.SwPackage' -ErrorAction Stop;
