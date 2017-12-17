Import-module -Name ($PSScriptRoot | Join-Path -ChildPath "..\Install\Debug\Erwine.Leonard.T.NetworkUtility.psd1") -ErrorAction Stop;

$MediaType = New-Object -TypeName 'NetworkUtility.MediaType' -ArgumentList 'application/pdf';
$MediaType.ToString();