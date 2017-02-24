Function New-VsProjectNamespaceManager {
    <#
        .SYNOPSIS
			Create XmlNamespaceManager for parsing Visual Studio Project Files.
         
        .DESCRIPTION
			Returns a new XmlNamespaceManager with the MSBuild namespace added, for parsing Visual Studio Project Files.
        
        .OUTPUTS
			System.Xml.XmlNamespaceManager. XmlNamespaceManager with the MSBuild namespace added.
        
        .LINK
            Get-VsProjectProperties
        
        .LINK
            Get-VsProjectItems
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlnamespacemanager.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Xml.XmlNamespaceManager])]
    Param(
        [Parameter(Mandatory = $true)]
        # Visual Studio project file loaded as XML.
        [System.Xml.XmlDocument]$ProjectFile,
        
        [Parameter(Mandatory = $true)]
        [ValidateScript({ [System.Xml.XmlConvert]::EncodeLocalName($_) -eq $_ })]
        # Namespace prefix to use.
        [string]$Prefix
    )
    
    $VsProjectNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $ProjectFile.NameTable;
    $VsProjectNamespaceManager.AddNamespace($Prefix, 'http://schemas.microsoft.com/developer/msbuild/2003');
    return @(,$VsProjectNamespaceManager);
}

Function Parse-VsProjectCondition {
    <#
        .SYNOPSIS
			Parses a Visual Studio conditional statement.
         
        .DESCRIPTION
			Returns a collection of objects representing Visual Studio conditional statement tokens.
            This currently does not support $if$, $else$, $endif$ expressions.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. Custom objects representing Visual Studio conditional statement tokens.
        
        .LINK
            Test-VsProjectCondition
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true)]
        # Conditional statement to be parsed.
        [string]$Condition,

        # Optional starting index for parsing.
        [int]$StartIndex = 0,

        # Use this switch to parse the Condition as the content of a string definition.
        [switch]$IsString
    )

    if ($IsString) {
        $Text = '';
        for ($i = $StartIndex; $i -lt $Condition.Length; $i++) {
            if ($Condition[$i] -eq "'") {
                New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'StringContent'; Index = $StartIndex; Content = $Condition.Substring($StartIndex, $i - $StartIndex); Code = $Text };
                New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'StringDelimiter'; Index = $i; Content = "'"; Code = '"' };
                break;
            }
            if ($Condition[$i] -eq '$') {
                if ($i -lt ($Condition.Length - 1) -and $Condition[$i + 1] -eq '(') {
                    $n = $Condition.IndexOf(')', $i + 1);
                    if ($n -lt 0) { throw "Unexpected token at index $i." }
                    $i += 2;
                    $Text += '${';
                    $Text += $Condition.Substring($i, $n - $i);
                    $Text += '}';
                    $i = $n;
                } else {
                    $Text += ('`' + $Condition[$i]);
                }
            } else {
                if ($Condition[$i] -eq '`') {
                    $Text += '``';
                } else {
                    $Text += $Condition[$i];
                }
            }
        }
    } else {
        $Index = 0;
        while ($StartIndex -lt $Condition.Length) {
            switch ([System.Char]::ToLowerInvariant($Condition[$StartIndex])) {
                '=' {
                    $StartIndex++;
                    if ($StartIndex -eq $Condition.Length) { throw "Unexpected end of code while parsing equality operator." }
                    if ($Condition[$StartIndex] -ne '=')  { throw "Unexpected token at index $StartIndex." }
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '=='; Code = ' -eq ' };
                    $StartIndex++;
                    break;
                }
                '<' {
                    $StartIndex++;
                    if ($StartIndex -lt $Condition.Length -and $Condition[$StartIndex] -ne '=') {
                        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '<='; Code = ' -le ' };
                        $StartIndex++;
                    } else {
                        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '<'; Code = ' -lt ' };
                    }
                    break;
                }
                '>' {
                    $StartIndex++;
                    if ($StartIndex -lt $Condition.Length -and $Condition[$StartIndex] -ne '=') {
                        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '>='; Code = ' -ge ' };
                        $StartIndex++;
                    } else {
                        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '>'; Code = ' -gt ' };
                    }
                    break;
                }
                '!' {
                    $StartIndex++;
                    if ($StartIndex -lt $Condition.Length) {
                        if ($Condition[$StartIndex] -eq '=') {
                            $StartIndex++;
                            New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '!='; Code = ' -ne ' };
                        } else {
                            New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = '!'; Code = ' -not ' };
                        }
                    } else {
                        throw "Unexpected end of code while parsing operator."
                    }
                    break;
                }
                '(' {
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Grouping'; Index = $StartIndex; Content = '('; Code = ' (' };
                    $Tokens = @(Parse-VsProjectCondition -Condition $Condition -StartIndex ($StartIndex + 1));
                    if ($Tokens.Count -eq 0) { throw "Unexpected end of code while parsing grouping construct." }
                    $StartIndex = $Tokens[-1].Index + $Tokens[-1].Content.Length;
                    $Tokens | Write-Output;
                    break;
                }
                ')' {
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Grouping'; Index = $StartIndex; Content = ')'; Code = ') ' };
                    $StartIndex++;
                    $Condition = '';
                    break;
                }
                "'" {
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'StringDelimiter'; Index = $StartIndex; Content = "'"; Code = '"' };
                    $Tokens = @(Parse-VsProjectCondition -Condition $Condition -StartIndex ($StartIndex + 1) -IsString);
                    if ($Tokens.Count -eq 0) { throw "Unexpected end of code while parsing string." }
                    $StartIndex = $Tokens[-1].Index + $Tokens[-1].Content.Length;
                    $Tokens | Write-Output;
                    break;
                }
                'o' {
                    if (($StartIndex + 3) -ge $Condition.Length) { throw "Unexpected end of code while parsing operand." }
                    if ($Condition[$StartIndex + 1] -ine 'r' -or (@(' ', '(', "'") -notcontains $Condition[$StartIndex + 2])) { throw "Unexpected token at index $StartIndex." }
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = $Condition.Substring($StartIndex, 2); Code = ' -or ' };
                    $StartIndex += 2;
                    break;
                }
                'a' {
                    if (($StartIndex + 3) -ge $Condition.Length) { throw "Unexpected end of code while parsing operand." }
                    if ($Condition.Substring($StartIndex + 1, 2) -ine 'nd' -or (@(' ', '(', "'") -notcontains $Condition[$StartIndex + 3])) { throw "Unexpected token at index $StartIndex." }
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'Operator'; Index = $StartIndex; Content = $Condition.Substring($StartIndex, 3); Code = ' -and ' };
                    $StartIndex += 3;
                    break;
                }
                'e' {
                    # And
                    # Exists('')
                    if (($StartIndex + 10) -gt $Condition.Length) { throw "Unexpected end of code while parsing operand." }
                    if ($Condition.Substring($StartIndex + 1, 5) -ine 'xists')  { throw "Unexpected token at index $StartIndex." }
                    $i = $StartIndex + 6;
                    while ($i -lt $Condition.Length -and $Condition[$i] -eq ' ') { $i++ }
                    if ($i -eq $Condition.Length) { throw "Unexpected end of code while parsing operand." }
                    if ([System.Char]::IsLetterOrDigit($Condition[$i])) { throw "Unexpected token at index $StartIndex." }
                    if ($Condition[$i] -ne '(') { throw "Unexpected token at index $i." }
                    $i++;
                    while ($i -lt $Condition.Length -and $Condition[$i] -eq ' ') { $i++ }
                    if ($i -eq $Condition.Length) { throw "Unexpected end of code while parsing argument." }
                    if ($Condition[$i] -ne "'") { throw "Unexpected token at index $i." }
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'InvocationStart'; Index = $StartIndex; Content = $Condition.Substring($StartIndex, $i - $StartIndex); Code = ' (Test-Path -Path ' };
                    $StartIndex = $i;
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'StringDelimiter'; Index = $StartIndex; Content = "'"; Code = '"' };
                    $Tokens = @(Parse-VsProjectCondition -Condition $Condition -StartIndex ($StartIndex + 1) -IsString);
                    if ($Tokens.Count -eq 0) { throw "Unexpected end of code while parsing string." }
                    $i = $Tokens[-1].Index + $Tokens[-1].Content.Length;
                    while ($i -lt $Condition.Length -and $Condition[$i] -eq ' ') { $i++ }
                    if ($i -eq $Condition.Length) { throw "Unexpected end of code while parsing argument." }
                    if ($Condition[$i] -ne ')') { throw "Unexpected token at index $i." }
                    $Tokens | Write-Output;
                    $StartIndex = $i;
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'InvocationEnd'; Index = $StartIndex; Content = ')'; Code = ') ' };
                    $StartIndex++;
                    break;
                }
                'h' {
                    # HasTrailingSlash
                    # Exists('')
                    if (($StartIndex + 20) -gt $Condition.Length) { throw "Unexpected end of code while parsing operand." }
                    if ($Condition.Substring($StartIndex + 1, 15) -ine 'asTrailingSlash')  { throw "Unexpected token at index $StartIndex." }
                    $i = $StartIndex + 16;
                    while ($i -lt $Condition.Length -and $Condition[$i] -eq ' ') { $i++ }
                    if ($i -eq $Condition.Length) { throw "Unexpected end of code while parsing operand." }
                    if ([System.Char]::IsLetterOrDigit($Condition[$i])) { throw "Unexpected token at index $StartIndex." }
                    if ($Condition[$i] -ne '(') { throw "Unexpected token at index $i." }
                    $i++;
                    while ($i -lt $Condition.Length -and $Condition[$i] -eq ' ') { $i++ }
                    if ($i -eq $Condition.Length) { throw "Unexpected end of code while parsing argument." }
                    if ($Condition[$i] -ne "'") { throw "Unexpected token at index $i." }
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'InvocationStart'; Index = $StartIndex; Content = $Condition.Substring($StartIndex, $i - $StartIndex); Code = ' ' };
                    $StartIndex = $i;
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'StringDelimiter'; Index = $StartIndex; Content = "'"; Code = '"' };
                    $Tokens = @(Parse-VsProjectCondition -Condition $Condition -StartIndex ($StartIndex + 1) -IsString);
                    if ($Tokens.Count -eq 0) { throw "Unexpected end of code while parsing string." }
                    $i = $Tokens[-1].Index + $Tokens[-1].Content.Length;
                    while ($i -lt $Condition.Length -and $Condition[$i] -eq ' ') { $i++ }
                    if ($i -eq $Condition.Length) { throw "Unexpected end of code while parsing argument." }
                    if ($Condition[$i] -ne ')') { throw "Unexpected token at index $i." }
                    $Tokens | Write-Output;
                    $StartIndex = $i;
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Type = 'InvocationEnd'; Index = $StartIndex; Content = ')'; Code = '.Trim().Replace(''/'', ''\'').EndsWith(''\'') ' };
                    $StartIndex++;
                    break;
                }
                default {
                    if (-not [System.Char]::IsWhiteSpace($Condition[$StartIndex])) { throw "Unexpected token at index $StartIndex." }
                    $StartIndex++;
                    break;
                }
            }
        }
    }
}

Function Test-VsProjectCondition {
    <#
        .SYNOPSIS
			Tests a Visual Studio conditional statement.
         
        .DESCRIPTION
			Returns true if the conditional statement returns true; otherwise returns false.
            This currently does not support $if$, $else$, $endif$ expressions.
            All parameters besides the Condition parameter define variables that may be referenced by conditional statements.
        
        .OUTPUTS
			System.Boolean. True if the conditional statement returns true; otherwise returns false.
        
        .LINK
            Parse-VsProjectCondition
    #>
    [CmdletBinding(DefaultParameterSetName = 'Explicit')]
    [OutputType([bool])]
    Param(
        [AllowEmptyString()]
        # Conditional statement to be tested.
        [string]$Condition = '',
        
        # Defines the name of configuration to test against, Usually "Debug" or "Release.".
        [string]$Configuration = '',
        
        [Parameter(ParameterSetName = 'Explicit')]
        [AllowEmptyString()]
        # Defines the operating system platform to test against, usually "AnyCPU", "x86", or "x64".
        [string]$Platform = '',
        
        [Parameter(ParameterSetName = 'Explicit')]
        [AllowEmptyString()]
        # Defines the processor architecture that is used when assembly references are resolved. Values are usually "msil," "x86," "amd64," or "ia64."
        [string]$ProcessorArchitecture = '',
        
        [AllowEmptyString()]
        # Defines the name of the final output assembly after the project is built (minus the file extension).
        [string]$AssemblyName = '',
        
        [AllowEmptyString()]
        # Defines the root namespace to use when you name an embedded resource. This namespace is part of the embedded resource manifest name.
        [string]$RootNamespace = '',
        
        [AllowEmptyString()]
        # Specifies the base path for the output file.
        [string]$BaseOutputPath = '',
        
        [AllowEmptyString()]
        # Specifies the path to the output directory, relative to the BaseOutputPath.
        [string]$OutputPath = '',
        
        [AllowEmptyString()]
        # Defines the file format of the output file. Values are usually "Library", "Exe", "Module" and "Winexe".
        [string]$OutputType = '',
        
        [AllowEmptyString()]
        # Defines the name of the file that is generated as the XML documentation file. This name includes only the file name and has no path information.
        [string]$DocumentationFile = '',
        
        [AllowEmptyString()]
        # Currently, this is not implemented (Specifies the file that's used to sign the assembly (.snk or .pfx) and that's passed to the ResolveKeySource Task to generate the actual key that's used to sign the assembly).
        [string]$AssemblyOriginatorKeyFile = '',
        
        [Parameter(Mandatory = $true, ParameterSetName = 'MSIL')]
        # Using this switch is the same as setting Platform to 'AnyCPU' and ProcessorArchitecture to 'msil'.
        [switch]$AnyCpu,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'X86')]
        # Using this switch is the same as setting Platform to 'x86' and ProcessorArchitecture to 'x86'.
        [switch]$X86,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Amd64')]
        # Using this switch is the same as setting Platform to 'x64' and ProcessorArchitecture to 'amd64'.
        [switch]$Amd64,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'IA64')]
        # Using this switch is the same as setting Platform to 'x64' and ProcessorArchitecture to 'ia64'.
        [switch]$IA64,
        
        [AllowEmptyCollection()]
        # Defines the collection of conditional compilation symbols to use. Currently defined values are not supported.
        [string[]]$Define = @(),
        
        # Indicates whether symbols are generated by the build.
        [bool]$DebugSymbols = $false,
        
        # Indicates whether the DEBUG constant is to be defined.
        [bool]$DefineDebug = $false,
        
        # Indicates whether the TRACE constant is to be defined.
        [bool]$DefineTrace = $false,
        
        # Currently, this is not implemented (Indicates whether the assembly will be signed).
        [bool]$SignAssembly = $false
    )

    $PsCode = '';
    $Tokens = Parse-VsProjectCondition -Condition $Condition | ForEach-Object {
        if ($_.Code.StartsWith(' ') -and $PsCode.EndsWith(' ')) { $PsCode += $_.Code.TrimStart() } else { $PsCode += $_.Code }
    }
    $ScriptBlock = $null;
    try { $ScriptBlock = [System.Management.Automation.ScriptBlock]::Create($PsCode.Trim()) }
    catch { throw "Parsed condition resulted in invalid script block ($_)" }
    
    if (-not $PSCmdlet.ParameterSetName -eq 'Explicit') {
        if ($IA64) {
            $Platform = 'x64';
            $ProcessorArchitecture = 'ia64';
        } else {
            if ($Amd64) {
                $Platform = 'x64';
                $ProcessorArchitecture = 'amd64';
            } else {
                if ($X86) {
                    $Platform = 'x86';
                    $ProcessorArchitecture = 'x86';
                } else {
                    if ($AnyCpu) {
                        $Platform = 'AnyCPU';
                        $ProcessorArchitecture = 'msil';
                    }
                }
            }
        }
    }
    $DefineConstants = '';
    if ($PSBoundParameters.ContainsKey('Define')) { $DefineConstants = $Define -join ';' }
    if ($PSBoundParameters.ContainsKey('BaseOutputPath')) {
        if (-not $PSBoundParameters.ContainsKey('OutputPath')) { $OutputPath = $Configuration }
    } else {
        if ($PSBoundParameters.ContainsKey('OutputPath')) {
            $BaseOutputPath = $Script:ProjectDir;
        } else {
            $BaseOutputPath = $Script:ProjectDir | Split-Path -Parent;
            $OutputPath = $Script:ProjectDir | Split-Path -Leaf;
        }
    }

    try { (. $ScriptBlock) -eq $true }
    catch { throw "Parsed condition resulted in script block execution error ($_)" }
}

Function Get-VsProjectProperties {
    <#
        .SYNOPSIS
			Gets property definitions from a Visual Studio project file.
         
        .DESCRIPTION
			This returns properties filtered by conditional expressions defined within the project file.
            This currently does not support $if$, $else$, $endif$ expressions.
            All parameters besides the ProjectFile parameter define variables that may be referenced by conditional statements.
        
        .OUTPUTS
			System.Management.Automation.PSObject. An object with properties added which correspond to project properties.
        
        .LINK
            Invoke-BuildVsProject

        .LINK
            Get-VsProjectItems

        .LINK
            Test-VsProjectCondition
    #>
    [CmdletBinding(DefaultParameterSetName = 'Explicit')]
    [OutputType([System.Management.Automation.PSObject])]
    Param(
        [Parameter(Mandatory = $true)]
        # Visual Studio project file loaded as an XML document.
        [System.Xml.XmlDocument]$ProjectFile,
        
        # Defines the name of configuration to use, Usually "Debug" or "Release.".
        [string]$Configuration = '',
        
        [Parameter(ParameterSetName = 'Explicit')]
        [AllowEmptyString()]
        # Defines the operating system platform to use, usually "AnyCPU", "x86", or "x64".
        [string]$Platform = '',
        
        [Parameter(ParameterSetName = 'Explicit')]
        [AllowEmptyString()]
        # Defines the processor architecture that is used when assembly references are resolved. Values are usually "msil," "x86," "amd64," or "ia64."
        [string]$ProcessorArchitecture = '',
        
        [AllowEmptyString()]
        # Defines the name of the final output assembly after the project is built (minus the file extension).
        [string]$AssemblyName = '',
        
        [AllowEmptyString()]
        # Defines the root namespace to use when you name an embedded resource. This namespace is part of the embedded resource manifest name.
        [string]$RootNamespace = '',
        
        [AllowEmptyString()]
        # Specifies the base path for the output file.
        [string]$BaseOutputPath = '',
        
        [AllowEmptyString()]
        # Specifies the path to the output directory, relative to the BaseOutputPath.
        [string]$OutputPath = '',
        
        [AllowEmptyString()]
        # Defines the file format of the output file. Values are usually "Library", "Exe", "Module" and "Winexe".
        [string]$OutputType = '',
        
        [AllowEmptyString()]
        # Defines the name of the file that is generated as the XML documentation file. This name includes only the file name and has no path information.
        [string]$DocumentationFile = '',
        
        [AllowEmptyString()]
        # Currently, this is not implemented (Specifies the file that's used to sign the assembly (.snk or .pfx) and that's passed to the ResolveKeySource Task to generate the actual key that's used to sign the assembly).
        [string]$AssemblyOriginatorKeyFile = '',
        
        [Parameter(Mandatory = $true, ParameterSetName = 'MSIL')]
        # Using this switch is the same as setting Platform to 'AnyCPU' and ProcessorArchitecture to 'msil'.
        [switch]$AnyCpu,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'X86')]
        # Using this switch is the same as setting Platform to 'x86' and ProcessorArchitecture to 'x86'.
        [switch]$X86,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Amd64')]
        # Using this switch is the same as setting Platform to 'x64' and ProcessorArchitecture to 'amd64'.
        [switch]$Amd64,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'IA64')]
        # Using this switch is the same as setting Platform to 'x64' and ProcessorArchitecture to 'ia64'.
        [switch]$IA64,
        
        [AllowEmptyCollection()]
        # Defines the collection of conditional compilation symbols to use. Currently defined values are not supported.
        [string[]]$Define = @(),
        
        # Indicates whether symbols are generated by the build.
        [bool]$DebugSymbols = $false,
        
        # Indicates whether the DEBUG constant is to be defined.
        [bool]$DefineDebug = $false,
        
        # Indicates whether the TRACE constant is to be defined.
        [bool]$DefineTrace = $false,
        
        # Currently, this is not implemented (Indicates whether the assembly will be signed).
        [bool]$SignAssembly = $false
    )
    
    $TestSplat = @{
        Configuration = $Configuration;
        AssemblyName = $AssemblyName;
        RootNamespace = $RootNamespace;
        BaseOutputPath = $BaseOutputPath;
        OutputPath = $OutputPath;
        OutputType = $OutputType;
        Define = $Define;
        DebugSymbols = $DebugSymbols;
        DefineDebug = $DefineDebug;
        DefineTrace = $DefineTrace;
    };
    $Properties = @{
        Platform = $Platform;
        PlatformTarget = $Platform;
        Configuration = $Configuration;
        OutputType = $OutputType;
        RootNamespace = $RootNamespace;
        AssemblyName = $AssemblyName;
        OutputPath = $OutputPath;
        DefineConstants = $Define;
        DebugSymbols = $DebugSymbols;
    };
    if ($IA64) {
        $TestSplat['IA64'] = $IA64;
        $Properties['Platform'] = 'x64';
        $Properties['ProcessorArchitecture'] = 'ia64';
    } else {
        if ($Amd64) {
            $TestSplat['Amd64'] = $Amd64;
            $Properties['Platform'] = 'x64';
            $Properties['ProcessorArchitecture'] = 'amd64';
        } else {
            if ($X86) {
                $TestSplat['X86'] = $X86;
                $Properties['Platform'] = 'x86';
                $Properties['ProcessorArchitecture'] = 'x86';
            } else {
                if ($AnyCpu) {
                    $TestSplat['AnyCpu'] = $AnyCpu;
                    $Properties['Platform'] = 'AnyCPU';
                    $Properties['ProcessorArchitecture'] = 'msil';
                } else {
                    $TestSplat['Platform'] = $Platform;
                    $TestSplat['ProcessorArchitecture'] = $ProcessorArchitecture;
                }
            }
        }
    }
    $nsmgr = New-VsProjectNamespaceManager -ProjectFile $ProjectFile -Prefix 'b';
    @($ProjectFile.SelectNodes('/b:Project/b:PropertyGroup', $nsmgr)) | Where-Object {
        $XmlAttribute = $_.SelectSingleNode('@Condition');
        $XmlAttribute -eq $null -or ($TestSplat['Condition'] = $XmlAttribute.Value.Trim()).Length -eq 0 -or (Test-VsProjectCondition @TestSplat);
    } | Foreach-Object { @($_.SelectNodes('b:*', $nsmgr)) } | Where-Object {
        $XmlAttribute = $_.SelectSingleNode('@Condition');
        $XmlAttribute -eq $null -or ($TestSplat['Condition'] = $XmlAttribute.Value.Trim()).Length -eq 0 -or (Test-VsProjectCondition @TestSplat);
    } | ForEach-Object {
        if ($_.IsEmpty) {
            $Properties[$_.LocalName] = $null;
        } else {
            if ($_.LocalName -eq 'DefineConstants') {
                if ($_.InnerText.Trim().Length -eq 0) {
                    $Properties[$_.LocalName] = @();
                } else {
                    $Properties[$_.LocalName] = @($_.InnerText.Split(';') | ForEach-Object { $_.Trim() } | Where-Object { $_.Length -gt 0 });
                }
            } else {
                if ($_.LocalName -eq 'DebugSymbols' -or $_.Localname -eq 'SignAssembly') {
                    try { $Properties[$_.LocalName] = [System.Xml.XmlConvert]::ToBoolean($_.InnerText) } catch { }
                } else {
                    $Properties[$_.LocalName] = $_.InnerText;
                }
            }
        }
    }

    $Count = $PSBoundParameters.Count;
    if ($PSBoundParameters.ContainsKey('BaseOutputPath')) { $Count-- }
    if ($PSBoundParameters.ContainsKey('Configuration') -and $Configuration.Length -eq 0) {
        $Count--;
        if ($PSBoundParameters.ContainsKey('Platform')) { $Count-- }
    } else {
        if ($PSBoundParameters.ContainsKey('Platform') -and $Platform.Length -eq 0) {
            $Count--;
            if ($PSBoundParameters.ContainsKey('Configuration')) { $Count-- }
        }
    }
    
    if ($Count -gt 1) {
        if ($PSBoundParameters.ContainsKey('BaseOutputPath')) {
            $Properties['BaseOutputPath'] = $BaseOutputPath;
            if ($PSBoundParameters.ContainsKey('OutputPath')) {
                $Properties['OutputPath'] = $OutputPath;
            } else {
                if ($Properties.ContainsKey('Configuration')) {
                    $Properties['OutputPath'] = $Properties['Configuration'];
                } else {
                    $Properties['OutputPath'] = $Configuration;
                }
            }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
    } else {
        $TestSplat = @{ };
        ('Platform', 'ProcessorArchitecture', 'Configuration', 'AssemblyName', 'RootNamespace', 'OutputType', 'DocumentationFile', 'AssemblyOriginatorKeyFile') | ForEach-Object {
            if ($Properties.ContainsKey($_) -and $Properties[$_] -ne $null -and $Properties[$_].Length -gt 0) { $TestSplat[$_] = $Properties[$_] }
        }
        if ($Properties.ContainsKey('DebugSymbols') -and $Properties['DebugSymbols'] -ne $null) { $TestSplat['DebugSymbols'] = $Properties['DebugSymbols'] }
        if ($Properties.ContainsKey('SignAssembly') -and $Properties['SignAssembly'] -ne $null) { $TestSplat['SignAssembly'] = $Properties['SignAssembly'] }
        if ($Properties.ContainsKey('DefineConstants') -and $Properties['DefineConstants'] -ne $null -and $Properties['DefineConstants'].Count -gt 0) {
            $TestSplat['DefineDebug'] = $Properties['DefineConstants'] -icontains 'DEBUG';
            $TestSplat['DefineTrace'] = $Properties['DefineConstants'] -icontains 'TRACE';
            $TestSplat['Define'] = @($Properties['DefineConstants'] | Where-Object { $_ -ine 'DEBUG' -and $_ -ine 'TRACE' });
        }
        if ($PSBoundParameters.ContainsKey('BaseOutputPath')) { $Count-- }
        if ($TestSplat.Count -eq 0) {
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
        } else {
            if ($PSBoundParameters.ContainsKey('BaseOutputPath')) { $TestSplat['BaseOutputPath'] = $BaseOutputPath }
            $TestSplat['ProjectFile'] = $ProjectFile;
            Get-VsProjectProperties @TestSplat;
        }
    }
}

Function Get-VsProjectItems {
    <#
        .SYNOPSIS
			Gets included items from a Visual Studio project file.
         
        .DESCRIPTION
			This returns items filtered by conditional expressions defined within the project file.
            This currently does not support $if$, $else$, $endif$ expressions.
        
        .OUTPUTS
			System.String[]. Items retrieved from the project file.
        
        .LINK
            Invoke-BuildVsProject

        .LINK
            Get-VsProjectProperties
    #>
    [CmdletBinding(DefaultParameterSetName = 'MSIL')]
    [OutputType([string[]])]
    Param(
        [Parameter(Mandatory = $true)]
        # Visual Studio project file loaded as an XML document.
        [System.Xml.XmlDocument]$ProjectFile,
        
        [ValidateSet('Reference', 'Compile', 'Folder', 'Content', 'EmbeddedResource', 'None')]
        # Type of item to return
        [string]$Type = 'None',

        # Object containing values which correspond to a Visual Studio project configuration.
        [System.Management.Automation.PSObject]$VsProjectProperties
    )

    if (-not $PSBoundParameters.ContainsKey('VsProjectProperties')) { $VsProjectProperties = Get-VsProjectProperties }
    
    $TestSplat = @{ Configuration = ''; Platform = '' };
    if ($VsProjectProperties.Configuration -ne $null) { $TestSplat['Configuration'] = $VsProjectProperties.Configuration }
    if ($VsProjectProperties.Platform -ne $null) { $TestSplat['Platform'] = $VsProjectProperties.Platform }
    if ($VsProjectProperties.AssemblyName -ne $null) { $TestSplat['AssemblyName'] = $VsProjectProperties.AssemblyName }
    if ($VsProjectProperties.RootNamespace -ne $null) { $TestSplat['RootNamespace'] = $VsProjectProperties.RootNamespace }
    if ($VsProjectProperties.BaseOutputPath -ne $null) { $TestSplat['BaseOutputPath'] = $VsProjectProperties.BaseOutputPath }
    if ($VsProjectProperties.OutputPath -ne $null) { $TestSplat['OutputPath'] = $VsProjectProperties.OutputPath }
    if ($VsProjectProperties.OutputType -ne $null) { $TestSplat['OutputType'] = $VsProjectProperties.OutputType }
    if ($VsProjectProperties.DocumentationFile -ne $null) { $TestSplat['DocumentationFile'] = $VsProjectProperties.DocumentationFile }
    if ($VsProjectProperties.AssemblyOriginatorKeyFile -ne $null) { $TestSplat['AssemblyOriginatorKeyFile'] = $VsProjectProperties.AssemblyOriginatorKeyFile }
    if ($VsProjectProperties.DebugSymbols -ne $null) { $TestSplat['DebugSymbols'] = $VsProjectProperties.DebugSymbols }
    if ($VsProjectProperties.SignAssembly -ne $null) { $TestSplat['SignAssembly'] = $VsProjectProperties.SignAssembly }
    if ($VsProjectProperties.DefineConstants -ne $null -and $VsProjectProperties.DefineConstants.Count -gt 0) {
        $TestSplat['DefineDebug'] = $VsProjectProperties.DefineConstants -icontains 'DEBUG';
        $TestSplat['DefineTrace'] = $VsProjectProperties.DefineConstants -icontains 'TRACE';
        $TestSplat['Define'] = @($VsProjectProperties.DefineConstants | Where-Object { $_ -ine 'DEBUG' -and $_ -ine 'TRACE' });
    }

    $nsmgr = New-VsProjectNamespaceManager -ProjectFile $ProjectFile -Prefix 'b';
    @(@($ProjectFile.SelectNodes('/b:Project/b:ItemGroup', $nsmgr)) | Where-Object {
        $XmlAttribute = $_.SelectSingleNode('@Condition');
        $XmlAttribute -eq $null -or ($TestSplat['Condition'] = $XmlAttribute.Value.Trim()).Length -eq 0 -or (Test-VsProjectCondition @TestSplat);
    } | Foreach-Object { @($_.SelectNodes(('b:' + $Type), $nsmgr)) } | Where-Object {
        $XmlAttribute = $_.SelectSingleNode('@Condition');
        $XmlAttribute -eq $null -or ($TestSplat['Condition'] = $XmlAttribute.Value.Trim()).Length -eq 0 -or (Test-VsProjectCondition @TestSplat);
    }) | ForEach-Object {
        $XmlAttribute = $_.SelectSingleNode('@Include');
        if ($XmlAttribute -ne $null -and $XmlAttribute.Value.Trim().Length -gt 0) { $XmlAttribute.Value.Trim() }
    }
}

Function Invoke-BuildVsProject {
    <#
        .SYNOPSIS
			Creates an assembly from a Visual Studio project file.
         
        .DESCRIPTION
			This creates an asssembly according to the configuration, platform and other conditional expressions defined within the project file.
        
        .LINK
            Get-VsProjectItems

        .LINK
            Get-VsProjectProperties
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $_.EndsWith('.csproj') -or $_.EndsWith('.vbproj') })]
        # Path to Visual Studio project to build.
        [string]$SourceProject,
        
        # Specifies the output directory to use. Otherwise, the output directory is determined by a path configured in the project file, and is relative to the project folder.
        [string]$OutputDir,
        
        # Defines the name of configuration to build, Usually "Debug" or "Release.".
        [string]$Configuration
    )

    $PreviousLocation = Get-Location;
    $ProjectDir = $SourceProject | Split-Path -Parent;
    Set-Location -Path $ProjectDir;
    try {
        $CsProj = New-Object -TypeName 'System.Xml.XmlDocument';
        $CsProj.Load($SourceProject);
        $VsProjectProperties = $null;
        if ($PSBoundParameters.ContainsKey('Configuration')) {
            $VsProjectProperties = Get-VsProjectProperties -ProjectFile $CsProj -Configuration $Configuration -BaseOutputPath $ProjectDir;
        } else {
            $VsProjectProperties = Get-VsProjectProperties -ProjectFile $CsProj -BaseOutputPath $ProjectDir;
        }
        if (-not $PSBoundParameters.ContainsKey('OutputDir')) {
            if ($VsProjectProperties.BaseOutputPath -eq $null -or $VsProjectProperties.BaseOutputPath.Length -eq 0) {
                $OutputDir = $SourceProject | Split-Path -Parent;
            } else {
                $OutputDir = $VsProjectProperties.BaseOutputPath;
            }
            if (-not ($OutputDir | Test-Path -PathType Container)) {
                New-Item -Path $OutputDir -ItemType 'directory' -ErrorAction Stop | Out-Null;
            }
            if ($VsProjectProperties.OutputPath -ne $null -and $VsProjectProperties.OutputPath.Length -gt 0) {
                $OutputDir = $OutputDir | Join-Path $VsProjectProperties.OutputPath;
            }
        }
        if (-not ($OutputDir | Test-Path -PathType Container)) {
            New-Item -Path $OutputDir -ItemType 'directory' -ErrorAction Stop | Out-Null;
        }
        $Path = @(Get-VsProjectItems -ProjectFile $CsProj -Type Compile -VsProjectProperties $VsProjectProperties);
        $EmbeddedResources = @(Get-VsProjectItems -ProjectFile $CsProj -Type EmbeddedResource -VsProjectProperties $VsProjectProperties);
        $ReferencedAssemblies = @((Get-VsProjectItems -ProjectFile $CsProj -Type Reference -VsProjectProperties $VsProjectProperties) | ForEach-Object {
            $n = New-Object -TypeName 'System.Reflection.AssemblyName' -ArgumentList $_;
            $a = [System.Reflection.Assembly]::LoadWithPartialName($n.Name);
            if ($a -ne $null) { $a.Location }
        });

        $CompilerParameters = New-Object -TypeName 'System.CodeDom.Compiler.CompilerParameters';
        $CompilerParameters.OutputAssembly = $OutputDir | Join-Path -ChildPath ($VsProjectProperties.AssemblyName + '.dll');
        if ($ReferencedAssemblies.Count -gt 0) {
            $CompilerParameters.ReferencedAssemblies.AddRange($ReferencedAssemblies);
        }
        if ($EmbeddedResources.Count -gt 0) {
            $CompilerParameters.EmbeddedResources.AddRange($EmbeddedResources);
        }
        $CompilerOptions = @();
        if ($VsProjectProperties.DefineConstants -ne $null -and $VsProjectProperties.DefineConstants.Count -gt 0) {
            $CompilerOptions = @('/define:' + ($VsProjectProperties.DefineConstants -join ';'));
        }
        if ($VsProjectProperties.DocumentationFile -ne $null -and $VsProjectProperties.DocumentationFile.Length -gt 0) {
            $CompilerOptions += ('/doc:"' + ($OutputDir | Join-Path -ChildPath ($VsProjectProperties.AssemblyName + '.XML')) + '"');
        }
        if ($VsProjectProperties.DebugSymbols) {
            $CompilerParameters.IncludeDebugInformation = $true;
            $CompilerOptions += ('/pdb:"' + ($OutputDir | Join-Path -ChildPath ($VsProjectProperties.AssemblyName + '.pdb')) + '"');
        }
        if ($CompilerOptions.Count -gt 0) {
            $CompilerParameters.CompilerOptions = $CompilerOptions -join ' ';
        }

        Add-Type -Path $Path -CompilerParameters $CompilerParameters -ErrorAction Stop;
    } finally { Set-Location $PreviousLocation }
}