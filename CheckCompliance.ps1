Add-Type -AssemblyName 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0' -ErrorAction Stop;$SlnContent = [System.IO.File]::ReadAllText(($PSScriptRoot | Join-Path -ChildPath 'PowerShellModules.sln'));
$StartOfNewLine = '(^[\r\n]*|\G[\r\n]+)';
$KeyValuePairPattern = '(?<key>[^=\s]+)[ \t]*=[ \t]*(\"(?<value>[^"]*)\"|(?<value>[^\s\r\n]*([ \t]+[^\s\r\n]+)*))';
$ProjectPattern = 'Project\(\"\{(?<typeGuid>[\da-f]{8}(-[\da-f]{4}){4}[\da-f]{8})\}\"\)[ \t]*=[ \t]*\"(?<projectName>[^\"]*)\"[ \t]*,[ \t]*\"(?<projectPath>[^\"]*)\"[ \t]*,[ \t]*\"\{(?<projectGuid>[\da-f]{8}(-[\da-f]{4}){4}[\da-f]{8})\}\"(?<otherProperties>[^\r\n]+)?(?<content>([\r\n]+(?!EndProject)[^\r\n]*)*)(\r\n?|\n)EndProject';
$GlobalPattern = 'Global(?<properties>[^\r\n]+)?(?<globalContent>([\r\n]+(?!EndGlobal)[^\r\n]*)*)(\r\n?|\n)EndGlobal';
$OtherPattern = '[^\r\n]+';

<#
$ProjectRegex = [System.Text.RegularExpressions.Regex]::new(@'
(^
((?![\r\n]*Project)
    [\r\n]*[^\r\n]*
    (?=[\r\n])
)*

|\G(\r\n?|\n))Project\("\{(?<typeGuid>[a-f\d]{8}(-[a-f\d]{4})[4][a-f\d]{8})\}"\)[ \t]*=[ \t]*"(?<projName>[^"]*)"[ \t]*,[ \t]*"(?<projPath>[^"]*)"[ \t]*,[ \t]*"\{(?<projGuid>[a-f\d]{8}(-[a-f\d]{4})[4][a-f\d]{8})\}"(?<otherProperties>[^\r\n]+)?(?<content>[\r\n]+[ \t]+[^\r\n]*)*(\r\n?|\n)EndProject
'@, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$ProjectRegex.Matches($SlnContent);

$Regex = [System.Text.RegularExpressions.Regex]::new($StartOfNewLine + $KeyValuePairPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$m = $Regex.Match('VisualStudioVersion = 14.0.25420.1');
$m.Success;
if ($m.Success) { "Key: `"$($m.Groups['key'].Value)`"; Value: `"$($m.Groups['value'].Value)`"" }

$Regex = [System.Text.RegularExpressions.Regex]::new($StartOfNewLine + $ProjectPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$m = $Regex.Match(@'
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{A19AF169-44DD-4737-82DD-AB83CF18B77A}"
	ProjectSection(SolutionItems) = preProject
		Common.ttinc = Common.ttinc
		CommonTypes.xsd = CommonTypes.xsd
		LICENSE = LICENSE
		ManifestToAbout.xslt = ManifestToAbout.xslt
		ManifestToPsd.xslt = ManifestToPsd.xslt
		PSModule.testsettings = PSModule.testsettings
		PsModuleManifestGeneration.ttinc = PsModuleManifestGeneration.ttinc
		README.md = README.md
		Setup-New.ps1 = Setup-New.ps1
		Setup.bat = Setup.bat
		Setup.ps1 = Setup.ps1
		SetupBindings.xsd = SetupBindings.xsd
		SetupManifestData.xsd = SetupManifestData.xsd
		VsProject.ps1 = VsProject.ps1
	EndProjectSection
EndProject
'@);
$m.Success;
if ($m.Success) { "typeGuid: `"$($m.Groups['typeGuid'].Value)`"; projectName: `"$($m.Groups['projectName'].Value)`"; projectPath: `"$($m.Groups['projectPath'].Value)`"; projectGuid: `"$($m.Groups['projectGuid'].Value)`"; content: `"$($m.Groups['content'].Value)`"" }

$Regex = [System.Text.RegularExpressions.Regex]::new($StartOfNewLine + $GlobalPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$m = $Regex.Match(@'
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		CD_ROM|Any CPU = CD_ROM|Any CPU
		Debug|Any CPU = Debug|Any CPU
		DebugPS2|Any CPU = DebugPS2|Any CPU
		DebugPS3|Any CPU = DebugPS3|Any CPU
		DVD-5|Any CPU = DVD-5|Any CPU
		Release|Any CPU = Release|Any CPU
		ReleasePS2|Any CPU = ReleasePS2|Any CPU
		ReleasePS3|Any CPU = ReleasePS3|Any CPU
		SingleImage|Any CPU = SingleImage|Any CPU
	EndGlobalSection
EndGlobal
'@);
$m.Success;
if ($m.Success) { "globalContent: `"$($m.Groups['globalContent'].Value)`"" }

#>

if ($Script:VsProjectGuids -eq $null) {
    $XmlDocument = [System.Xml.XmlDocument]::new();
    $XmlDocument.Load(($PSScriptRoot | Join-Path -ChildPath 'LteDev\VsProjectTypes.xml'));
    $Script:VsProjectGuids = @{};
    $XmlDocument.SelectNodes('/VsProjectTypes/ProjectType') | ForEach-Object {
        $Properties = @{
            Guid = [Guid]::Parse($_.SelectSingleNode('@Guid').Value);
            Key = $_.SelectSingleNode('@Key').Value;
            Description = $_.SelectSingleNode('@Description').Value;
            Extensions = [System.Collections.ObjectModel.Collection[System.String]]::new();
        };
        $a = $_.SelectSingleNode('@Extension');
        if ($a -ne $null) {
            $Properties.Extensions.Add($a.Value);
            $a = $_.SelectSingleNode('@AltExt');
            if ($a -ne $null) {
                $Properties.Extensions.Add($a.Value);
            }
        }
        $Item = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
        if (-not $Script:VsProjectGuids.ContainsKey($Item.Guid)) {
            $Script:VsProjectGuids[$Item.Guid] = [System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]]::new();
        }
        $Script:VsProjectGuids[$Item.Guid].Add($Item);
    }
}

Function Test-Proiect {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Guid]$TypeGuid,
        
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [string]$Path,
        
        [Parameter(Mandatory = $true)]
        [System.Guid]$ProjectGuid
    )
    
    $FullProjectPath = $PSScriptRoot | Join-Path -ChildPath $Path;
    $IsCsProject = $false;
    $ProjectDescription = "{$($Item.ProjectGuid)} Project";
    $Issues = @();
    if (-not $Script:VsProjectGuids.ContainsKey($TypeGuid)) {
        "Unknown project type Guid";
    } else {
        $IsCsProject = (@($Script:VsProjectGuids[$TypeGuid] | Where-Object { $_.Extensions -icontains '.csproj' }).Count -gt 0);
        $ProjectDescription = ($Script:VsProjectGuids[$TypeGuid] | ForEach-Object { $_.Description }) -join ' / ';
    }
    if (-not ($FullProjectPath | Test-Path -PathType Leaf)) {
        "Project file $Path not found.";
        return;
    }
    try { $Project = [Microsoft.Build.Evaluation.Project]::new($FullProjectPath) }
    catch {
        "Failed to load project file $Path`: $_.";
        return;
    }
    if ($Project -eq $null) {
        "Failed to load project file $Path.";
        return;
    }
    $Project.ReevaluateIfNecessary();
    $OutputPath = $Project.GetProperty('OutputPath').UnevaluatedValue;
    $AssemblyName = $Project.GetProperty('AssemblyName').UnevaluatedValue;
    $p = $Project.GetProperty('DefineConstants');
    $DefineConstants = @();
    if ($p -ne $null -and $p.UnevaluatedValue.Length -gt 0) {
        $DefineConstants = @($p.UnevaluatedValue.split(';') | ForEach-Object { $_.Trim() } | Where-Object { $_.Length -gt 0 });
    }
    $OutputType = $Project.GetProperty('OutputType').UnevaluatedValue;
    $ProjectDir = $Project.GetPropertyValue('ProjectDir');
    Test-PropertyValue -Project $Project -Name 'PostBuildEvent' -Expected @'
del "$(TargetDir)/*.nlp"
del "$(TargetDir)/mscorlib.dll"
'@;
    Test-PropertyValue -Project $Project -Name 'ProjectGuid' -Expected $Item.ProjectGuid.ToString('B');
    Test-PropertyValue -Project $Project -Name 'Configuration' -Expected '';
    Test-PropertyValue -Project $Project -Name 'AppDesignerFolder' -Expected 'Properties';
    Test-PropertyValue -Project $Project -Name 'TargetFrameworkVersion' -Expected 'v4.6';
    Test-PropertyValue -Project $Project -Name 'DocumentationFile' -Expected "$OutputPath$AssemblyName.XML";
    Test-PropertyValue -Project $Project -Name 'CodeAnalysisRuleSet' -Expected 'MinimumRecommendedRules.ruleset';
    $Items = $Project.GetItems('Content');
    $ExpectedItems = @();
    $ModuleManifestItems = @($Items | Where-Object { $_.UnevaluatedInclude.EndsWith('.psd1') });
    $ScriptModuleItems = @($Items | Where-Object { $_.UnevaluatedInclude.EndsWith('.psm1') });
    $ScriptModuleInfo = @{};
    $ModuleManifestItems | ForEach-Object {
        $bn = $_.UnevaluatedInclude.Substring(0, $_.UnevaluatedInclude.Length - 4);
        $p = "about_$bn.help";
        $ExpectedItems += @(
            @{ Include = $_.UnevaluatedInclude; Type = 'Content'; CopyToOutputDirectory = 'Always' }
            @{ Include = "about_$bn.help"; Type = 'Content'; CopyToOutputDirectory = 'Always' }
        );
    }
    $ScriptModuleItems | ForEach-Object {
        $ExpectedItems += @(
            @{ Include = $_.UnevaluatedInclude; Type = 'Content'; CopyToOutputDirectory = 'Always' }
        );
        $mmp = $_.UnevaluatedInclude.Substring(0, $_.UnevaluatedInclude.Length - 4) + '.psd1';
        if (@($ExpectedItems | Where-Object { $_.Include -eq $mmp }).Count -eq 0) {
            @{ Include = $mmp; Type = 'Content'; CopyToOutputDirectory = 'Always' }
        }
    }
    $ExpectedItems += @(@{ Include = 'Build.ps1'; Type = 'None' });
    if ($Project.GetPropertyValue('MSBuildProjectName') -ne 'LteDev') {
        $ExpectedItems += @(@{ Include = 'MSBuildLogHelper.cs'; Type = 'None' });
    } else {
        $ExpectedItems += @(@{ Include = 'MSBuildLogHelper.cs'; Type = 'Compile' });
    }
    
    $ByType = $ExpectedItems | Group-Object -Property 'Type' -AsHashTable;
    ($ByType.Keys) | ForEach-Object {
        $ExpectedItems += @(
            @{ Include = $_.UnevaluatedInclude; Type = 'Content'; CopyToOutputDirectory = 'Always' }
        );
        $bn = $_.UnevaluatedInclude.Substring(0, $_.UnevaluatedInclude.Length - 4);
        $mmp = "$bn.psd1";
        if (@($ModuleManifestItems | Where-Object { $_.UnevaluatedInclude -eq $mm}).Count -eq 0) {
            $Issues += "Module manifest $mmp not in project.";
        } else {
            $mle = $null;
            $mm = $null;
            $mm = Test-ModuleManifest -Path ($ProjectDir | Join-Path -ChildPath $mpp) -ErrorAction Continue -ErrorVariable 'mle';
            if ($mle -ne $null) {
                $Issues += "Errer loading Module manifest $mmp`: $mle";
            } else {
                if ($mm -eq $null) {
                    $Issues += "Errer loading Module manifest $mmp`: faiked to load";
                } else {
                    $ScriptModuleInfo[$mmp] = $mm;
                }
            }
        }
    }

}

Function Test-BinaryProject {
    Param(
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]$Project,
        
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [System.Guid]$Guid,
        
        [Parameter(Mandatory = $true)]
        [string]$Description,
        
        [Parameter(Mandatory = $true)]
        [bool]$IsCsProject
    )
}

Function Test-ScriptProject {
    Param(
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]$Project,
        
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [System.Guid]$Guid,
        
        [Parameter(Mandatory = $true)]
        [string]$Description,
        
        [Parameter(Mandatory = $true)]
        [bool]$IsCsProject
    )
}

Function Test-PropertyValue {
    Param(
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]$Project,
        
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Explicit')]
        [AllowNull()]
        [AllowEmptyString()]
        [string[]]$Expected,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Pattern')]
        [string]$Pattern,

        [Parameter(ParameterSetName = 'Pattern')]
        [System.Text.RegularExpressions.RegexOptions[]]$Options,
        
        [Parameter(ParameterSetName = 'Pattern')]
        [switch]$AllowNulls,
        
        [Parameter(ParameterSetName = 'Explicit')]
        [switch]$CaseSensitive,
        
        [switch]$EvaluatedValue
    )

    $Value = $null;
    $P = $Project.GetProperty($Name);
    if ($P -ne $null) {
        if ($EvaluatedValue) {
            $Value = $P.EvaluatedValue;
        } else {
            $Value = $P.UnevaluatedValue;
        }
    }
    $av = '';
    $es = '';
    if ($Value -eq $null) {
                $av = 'null';
            } else {
                $av = "`"$($Value -replace '\"', '\"')`"";
            }
    $IsMatch = $false;
    if ($PsCmdlet.ParameterSetName -eq 'Explicit') {
        $es = ($Expected | ForEach-Object { if ($_ -eq $null) {
                'null';
            } else {
                "`"$($_ -replace '\"', '\"')`"";
            } }) -join ', ';
        if ($Expected.Length -gt 1) { $es = "Any of ($es)" }
        if  ($Value -eq $null) {
            $IsMatch = (@($Expected | Where-Object { $_ -eq $null }).Count -gt 0);
        } else {
            if ($CaseSensitive) {
                $IsMatch = (@($Expected | Where-Object { $_ -ne $null -and $_ -ceq $Value }).Count -gt 0);
            } else {
                $IsMatch = (@($Expected | Where-Object { $_ -ne $null -and $_ -ceq $Value }).Count -gt 0);
            }
        }
    } else {
        $es = ($Pattern | ForEach-Object { "`"$($_ -replace '\"', '\"')`"" }) -join ', ';
        if ($Expected.Length -gt 1) { $es = "Matching any of ($es)" } else { $es = "Matching $es" }
        if  ($Value -eq $null) {
            $IsMatch = $AllowNulls;
        } else {
            if ($PSBoundParameters.ContainsKey('Options')) {
                $o = [System.Text.RegularExpressions.RegexOptions]::None;
                $Options | ForEach-Object { [System.Text.RegularExpressions.RegexOptions]$o = $o -bor $_ }
                $IsMatch = (@($Pattern | Where-Object { [System.Text.RegularExpressions.Regex]::IsMatch($Value, $_, $o) }).Count -gt 0);
            } else {
                $IsMatch = (@($Pattern | Where-Object { [System.Text.RegularExpressions.Regex]::IsMatch($Value, $_) }).Count -gt 0);
            }
        }
    }
    if (-not $IsMatch) {
        "$($Project.GetPropertyValue('MSBuildProjectName')): Property $Name` Expected: $es; Actual: $av";
    }
}

[Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadAllProjects();
$FullPattern = "$StartOfNewLine($ProjectPattern|$GlobalPattern|$KeyValuePairPattern|[^\r\n]+)[ \t]*";
$Regex = [System.Text.RegularExpressions.Regex]::new($FullPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$MatchCollection = $Regex.Matches($SlnContent);
$MatchCollection | ForEach-Object {
    if ($_.Groups['typeGuid'].Success) {
        if ($FullProjectPath | Test-Path -PathType Leaf) {
            $Project = [Microsoft.Build.Evaluation.Project]::new($FullProjectPath);
            if ($Project -eq $null) {
                $Issues += "Failed to load project file.";
            } else {
                $Project.ReevaluateIfNecessary();
                Test-Proiect -Project $Project -Description $ProjectDescription -IsCsProject $IsCsProject -Guid $Item.ProjectGuid -Name $Item.ProjectName;
                $OutputPath = $Project.GetProperty('OutputPath').UnevaluatedValue;
                $AssemblyName = $Project.GetProperty('AssemblyName').UnevaluatedValue;
                $AssemblyName = $Project.GetProperty('AssemblyName').UnevaluatedValue;
                $OutputType = $Project.GetProperty('OutputType').UnevaluatedValue;
                $ProjectDir = $Project.GetPropertyValue('ProjectDir');
                $Issues += @(Test-PropertyValue -Project $Project -Name 'PostBuildEvent' -Expected @'
del "$(TargetDir)/*.nlp"
del "$(TargetDir)/mscorlib.dll"
'@);
                $Issues += @(Test-PropertyValue -Project $Project -Name 'ProjectGuid' -Expected $Item.ProjectGuid.ToString('B'));
                $Issues += @(Test-PropertyValue -Project $Project -Name 'Configuration' -Expected '';
                $Issues += @(Test-PropertyValue -Project $Project -Name 'AppDesignerFolder' -Expected 'Properties';
                $Issues += @(Test-PropertyValue -Project $Project -Name 'TargetFrameworkVersion' -Expected 'v4.6';
                $Issues += @(Test-PropertyValue -Project $Project -Name 'DocumentationFile' -Expected "$OutputPath$AssemblyName.XML";
                $Issues += @(Test-PropertyValue -Project $Project -Name 'CodeAnalysisRuleSet' -Expected 'MinimumRecommendedRules.ruleset';
                $Items = $Project.GetItems('Content');
                $ModuleManifestItems = @($Items | Where-Object { $_.UnevaluatedInclude.EndsWith('.psd1') });
                $ScriptModuleItems = @($Items | Where-Object { $_.UnevaluatedInclude.EndsWith('.psm1') });
                $ScriptModuleInfo = @{};
                $ModuleManifestItems | ForEach-Object {
                    $bn = $_.UnevaluatedInclude.Substring(0, $_.UnevaluatedInclude.Length - 4);
                    $p = "about_$bn.help";
                    if (@($Items | Where-Object { $_.UnevaluatedInclude -eq $p}).Count -eq 0) {
                        $Issues += "Module overview help file $p not in project.";
                    }
                }
                $ScriptModuleItems | ForEach-Object {
                    $bn = $_.UnevaluatedInclude.Substring(0, $_.UnevaluatedInclude.Length - 4);
                    $mmp = "$bn.psd1";
                    if (@($ModuleManifestItems | Where-Object { $_.UnevaluatedInclude -eq $mm}).Count -eq 0) {
                        $Issues += "Module manifest $mmp not in project.";
                    } else {
                        $mle = $null;
                        $mm = $null;
                        $mm = Test-ModuleManifest -Path ($ProjectDir | Join-Path -ChildPath $mpp) -ErrorAction Continue -ErrorVariable 'mle';
                        if ($mle -ne $null) {
                            $Issues += "Errer loading Module manifest $mmp`: $mle";
                        } else {
                            if ($mm -eq $null) {
                                $Issues += "Errer loading Module manifest $mmp`: faiked to load";
                            } else {
                                $ScriptModuleInfo[$mmp] = $mm;
                            }
                        }
                    }
                }
                if ($OutputType -eq 'Library') {
                    if ($ModuleManifestItems.Count -gt 0) {
                        $Items = $Project.GetItems('Reference');
                        $Matching = @($Items | Where-Object { $_.UnevaluatedInclude.StartsWith('System.Management.Automation') });
                        if ($Matching.Count -eq 0) {
                            $Issues += "No reference to System.Management.Automation.";
                        } else {
                            $Assembly = [PSObject].Assembly;
                            $Expected = "$($Assembly.FullName), processorArchitecture=MSI";
                            $Actual = $Matching[0].UnevaluatedInclude;
                            if ($Actual -ne $Assembly.FullName -and $Actual -ne $Expected) {
                                $Issues += "$($Assembly.GetName().Name) reference expected: $Expected; Actual: $Actual";
                            } else {
                                $md = $Matching[0].DirectMetadata['SpecificVersion'];
                                if ($md -eq $null) {
                                    $Issues += "SpecificVersion for $($Assembly.GetName().Name) reference expected: False; Actual not defined";
                                } else {
                                    if ($md.UnevaluatedValue -ne 'False') {
                                        $Issues += "SpecificVersion for $($Assembly.GetName().Name) reference expected: False; Actual: $($md.UnevaluatedValue)";
                                    }
                                }
                                $md = $Matching[0].DirectMetadata['HintPath'];
                                if ($md -eq $null) {
                                    $Issues += "HintPath for $($Assembly.GetName().Name) reference expected: $($Assembly.Location); Actual not defined";
                                } else {
                                    if ($md.UnevaluatedValue -ne 'False') {
                                        $Issues += "HintPath for $($Assembly.GetName().Name) reference expected: $($Assembly.Location); Actual: $($md.UnevaluatedValue)";
                                    }
                                }
                            }
                        }
                        @([Microsoft.PowerShell.Commands.OutStringCommand], [Microsoft.PowerShell.Commands.GetContentCommand]) | ForEach-Object {
                            $Assembly = $_.Assembly;
                            $an = $Assembly.GetName();
                            $Matching = @($Items | Where-Object { $_.UnevaluatedInclude.StartsWith($an) });
                            if ($Matching.Count -gt 0) {
                                $Expected = "$an, processorArchitecture=MSI";
                                $Actual = $Matching[0].UnevaluatedInclude;
                                if ($Actual -ne $Assembly.FullName -and $Actual -ne $Expected) {
                                    $Issues += "$an reference expected: $Expected; Actual: $Actual";
                                } else {
                                    $md = $Matching[0].DirectMetadata['SpecificVersion'];
                                    if ($md -eq $null) {
                                        $Issues += "SpecificVersion for $an reference expected: False; Actual not defined";
                                    } else {
                                        if ($md.UnevaluatedValue -ne 'False') {
                                            $Issues += "SpecificVersion for $an reference expected: False; Actual: $($md.UnevaluatedValue)";
                                        }
                                    }
                                    $md = $Matching[0].DirectMetadata['HintPath'];
                                    if ($md -eq $null) {
                                        $Issues += "HintPath for $an reference expected: $($Assembly.Location); Actual not defined";
                                    } else {
                                        if ($md.UnevaluatedValue -ne 'False') {
                                            $Issues += "HintPath for $an reference expected: $($Assembly.Location); Actual: $($md.UnevaluatedValue)";
                                        }
                                    }
                                }
                            }
                        }
                    } else {
                        @([PSObject], [Microsoft.PowerShell.Commands.OutStringCommand], [Microsoft.PowerShell.Commands.GetContentCommand]) | ForEach-Object {
                            $Assembly = $_.Assembly;
                            $an = $Assembly.GetName();
                            $Matching = @($Items | Where-Object { $_.UnevaluatedInclude.StartsWith($an) });
                            if ($Matching.Count -gt 0) {
                                $Expected = "$an, processorArchitecture=MSI";
                                $Actual = $Matching[0].UnevaluatedInclude;
                                if ($Actual -ne $Assembly.FullName -and $Actual -ne $Expected) {
                                    $Issues += "$an reference expected: $Expected; Actual: $Actual";
                                } else {
                                    $md = $Matching[0].DirectMetadata['SpecificVersion'];
                                    if ($md -eq $null) {
                                        $Issues += "SpecificVersion for $an reference expected: False; Actual not defined";
                                    } else {
                                        if ($md.UnevaluatedValue -ne 'False') {
                                            $Issues += "SpecificVersion for $an reference expected: False; Actual: $($md.UnevaluatedValue)";
                                        }
                                    }
                                    $md = $Matching[0].DirectMetadata['HintPath'];
                                    if ($md -eq $null) {
                                        $Issues += "HintPath for $an reference expected: $($Assembly.Location); Actual not defined";
                                    } else {
                                        if ($md.UnevaluatedValue -ne 'False') {
                                            $Issues += "HintPath for $an reference expected: $($Assembly.Location); Actual: $($md.UnevaluatedValue)";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    $Items = $Project.GetItems('None');
                    $Matching = @($Items | Where-Object { $_.UnevaluatedInclude -eq 'Build.ps1' });
                    if ($Matching.Count -eq 0) {
                        $Issues += "No build script in project.";
                    }
                }
            }
        } else {
            $Issues += "Project file not found.";
        }
    } else {
        if ($_.Groups['globalContent'].Success) {
            Write-Debug -Message "globalContent.Length: `"$($_.Groups['globalContent'].Length)`"";
        } else {
            if ($_.Groups['key'].Success) {
                Write-Debug -Message "Key: `"$($_.Groups['key'].Value)`"; Value: `"$($_.Groups['value'].Value)`"";
            }
        }
    }
}