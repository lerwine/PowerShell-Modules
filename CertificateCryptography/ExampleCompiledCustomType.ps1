Function New-CompiledType {
    [CmdletBinding(DefaultParameterSetName = 'Implicit')]
    Param(
        [Parameter(Mandatory = $true)]
        [string[]]$SourceFiles,
        
        [Parameter(Mandatory = $false)]
        [string]$SourceDir,
        
        [Parameter(Mandatory = $false)]
        [string[]]$Resources,
        
        [Parameter(Mandatory = $false)]
        [string[]]$ReferencedAssemblies,
        
        [Parameter(Mandatory = $false)]
        [string[]]$Define,
        
        [Parameter(Mandatory = $false)]
        [string]$OutputAssembly,
        
        [Parameter(Mandatory = $false)]
        [Alias('DefineDebug')]
        [switch]$IsDebug,
        
        [Parameter(Mandatory = $false)]
        [switch]$PassThru
    )
    
    if ($PSBoundParameters.ContainsKey('SourceDir')) {
        $SourceBase = $SourceDir;
    } else {
        $SourceBase = Get-Location;
    }
    $CompilerOptions = @();
    $DefineArr = @($Define);
    if ($IsDebug) {
        $CompilerOptions += '/debug';
        if ($DefineArr -cnotcontains 'DEBUG') { $DefineArr = @('DEBUG') + $DefineArr }
    }
    $Assemblies = @($ReferencedAssemblies);
    if ($Assemblies.Count -eq 0 -or @($Assemblies | Where-Object { ($_ | Split-Path -Leaf) -ieq 'System.Management.Automation.dll' }).Count -eq 0) {
        $Assemblies = $Assemblies + [System.Management.Automation.PSObject].Assembly.Location;
    }
    if ($DefineArr.Count -gt 0) { $CompilerOptions = $CompilerOptions + ('/define:{0}' -f ($DefineArr -join ';')) }
    if ($Resources.Count -gt 0) { $CompilerOptions = $CompilerOptions + ($Resources | ForEach-Object { '/resource:"{0}"' -f [System.IO.Path]::Combine($SourceBase, $_) }) }
    $splat = @{
        Path = $SourceFiles | ForEach-Object { [System.IO.Path]::Combine($SourceBase, $_) };
        CompilerParameters = New-Object -TypeName 'System.CodeDom.Compiler.CompilerParameters';
    };
    if ($PSBoundParameters.ContainsKey('OutputAssembly')) {
        $splat.Add('OutputType', [Microsoft.PowerShell.Commands.OutputAssemblyType]::Library);
        $splat.Add('OutputAssembly', [System.IO.Path]::Combine($SourceBase, $OutputAssembly));
    }
    if ($CompilerOptions.Count -gt 0) { $splat.CompilerParameters.CompilerOptions = $CompilerOptions -join ' ' }
    $Assemblies | ForEach-Object { $splat.CompilerParameters.ReferencedAssemblies.Add($_) | Out-Null }
    if ($PassThru) { $splat.Add('PassThru', $PassThru) }
    Add-Type @splat;
}


New-CompiledType -OutputAssembly 'PSPassCustomTypes.dll' -SourceDir 'C:\Users\leonard.erwine\Downloads\PSPass-master\PSPassCustomTypes' -SourceFiles @(
    	'NotifyPropertyChanged.cs', 'PathConverter.cs', 'PSLogin.cs', 'PSPassFolder.cs', 'RegexPatterns.Designer.cs', 'Utility.cs'
    ) -Resources @(
        'RegexPatterns.resx',
        'Resources\PathWhitespaceNormaliztion.txt'
    ) -ReferencedAssemblies @('System.dll', 'System.Data.dll', 'System.Xml.dll') -Define 'PS2' -IsDebug -PassThru;
