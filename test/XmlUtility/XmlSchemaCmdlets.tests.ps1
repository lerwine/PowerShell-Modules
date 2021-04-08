Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.XmlUtility/Erwine.Leonard.T.XmlUtility.psd1');

$SchemasRootPath = (($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.XmlUtility/Schemas') | Resolve-Path).Path;

Describe 'Test loading XML schema' {
    It 'Get-ModuleSchemaFiles should return multiple files' {
        $Files = @(Get-ModuleSchemaFiles);
        $Files.Count | Should Not Be 0;
        $Files.Count | Should Not Be 1;
    }
};