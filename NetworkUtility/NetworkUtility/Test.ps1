Function Initialize-CurrentModule {
    [CmdletBinding()]
    Param()
	
	$Local:BaseName = $PSScriptRoot | Join-Path -ChildPath $MyInvocation.MyCommand.Module.Name;
	
    $Local:ModuleManifest = Test-ModuleManifest -Path ($PSScriptRoot | Join-Path -ChildPath ('{0}.psd1' -f $MyInvocation.MyCommand.Module.Name));
    $Local:Assemblies = @($Local:ModuleManifest.PrivateData.CompilerOptions.AssemblyReferences | ForEach-Object {
        (Add-Type -AssemblyName $_ -PassThru)[0].Assembly.Location
    });
    $Local:Splat = @{
        TypeName = 'System.CodeDom.Compiler.CompilerParameters';
        ArgumentList = (,$Local:Assemblies);
        Property = @{
            IncludeDebugInformation = $Local:ModuleManifest.PrivateData.CompilerOptions.IncludeDebugInformation;
        }
    };
    if ($Local:ModuleManifest.PrivateData.CompilerOptions.ConditionalCompilationSymbols -ne '') {
        $Local:Splat.Property.CompilerOptions = '/define:' + $Local:ModuleManifest.PrivateData.CompilerOptions.ConditionalCompilationSymbols;
    }

    $Script:AssemblyPath = @(Add-Type -Path ($Local:ModuleManifest.PrivateData.CompilerOptions.CustomTypeSourceFiles | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) -CompilerParameters (New-Object @Local:Splat) -PassThru)[0].Assembly.Location;
}

$Assemblies = @([System.Runtime.InteropServices.DllImportAttribute].Assembly.Location, [System.Uri].Assembly.Location, 
    [System.Xml.XmlDocument].Assembly.Location, (Add-Type -AssemblyName 'System.Web' -PassThru)[0].Assembly.Location,
    (Add-Type -AssemblyName 'System.Web.Extensions' -PassThru)[0].Assembly.Location);
$CompilerParameters = New-Object -TypeName 'System.CodeDom.Compiler.CompilerParameters' -ArgumentList (,$Local:Assemblies);
$CompilerParameters.IncludeDebugInformation = $true;
$CompilerParameters.CompilerOptions = '/define:DEBUG;TRACE';
Add-Type -Path ((Get-ChildItem -Path $PSScriptRoot -Filter '*.cs') | ForEach-Object { $_.FullName }) -CompilerParameters $CompilerParameters -ErrorAction Stop
$MediaType = New-Object -TypeName 'NetworkUtilityCLR.MediaType' -ArgumentList 'application/pdf';
$MediaType.ToString();