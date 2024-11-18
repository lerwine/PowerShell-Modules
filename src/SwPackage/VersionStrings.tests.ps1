Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../IOUtility/Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;
Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.SwPackage.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Compare-SemanticVersion' {
    
}