Param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Platform = 'AnyCPU',

    [ValidateSet('Test', 'Deploy', 'None')]
    [string]$Action = 'None',

    [switch]$ForceRebuild
)

$csproj_name = 'CertificateCryptography.csproj';
$module_name = 'Erwine.Leonard.T.CertificateCryptography';

cls;

Add-Type -AssemblyName 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0' -ErrorAction Stop;
Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath 'MsBuildLogHelper.cs') -ReferencedAssemblies 'System', 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0', 'System.Management.Automation' -ErrorAction Stop;

$Script:LastProjectFile = '';

$Script:BuildOutput = [System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]]::new();

Function Out-BuildEventArgs {
    Param(
        [AllowEmptyString()]
        [string]$StatusText = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [int]$LineNumber = 0,
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [int]$ColumnNumber = 0,
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [int]$EndColumnNumber = 0,
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [int]$EndLineNumber = 0,
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$File = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$TargetFile = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$TargetName = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$TaskName = '',

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$ProjectFile = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$Subcategory = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$Message = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$Code = '',
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [DateTime]$TimeStamp,
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [Microsoft.Build.Framework.MessageImportance]$Importance,

        [string]$Type = ''
    )

    Process {
        $m = $Type;
        if ($StatusText.Length -gt 0) {
            if ($m.Length -eq 0) {
                $m = $StatusText;
            } else {
                $m = "$m $StatusText";
            }
        }
        if ($LineNumber -gt 0) {
            if ($m.Length -gt 0) { $m = "$m " }
            if ($ColumnNumber -gt 0) {
                if ($EndLineNumber -gt 0 -and $EndLineNumber -ne $LineNumber) {
                    if ($EndColumnNumber -gt 0 -and $ColumnNumber -ne $EndColumnNumber) {
                        $m = "$($m)Line $LineNumber, Column $ColumnNumber through Line $EndLineNumber, Column $EndColumnNumber";
                    } else {
                        $m = "$($m)Line $LineNumber through Line $EndLineNumber, Column $ColumnNumber";
                    }
                } else {
                    if ($EndColumnNumber -gt 0 -and $ColumnNumber -ne $EndColumnNumber) {
                        $m = "$($m)Line $LineNumber, Column $ColumnNumber through $EndColumnNumber";
                    } else {
                        $m = "$($m)Line $LineNumber, Column $ColumnNumber";
                    }
                }
            } else {
                if ($EndLineNumber -gt 0 -and $EndLineNumber -ne $LineNumber) {
                    $m = "$($m)Line $LineNumber through Line $EndLineNumber";
                } else {
                    $m = "$($m)Line $LineNumber";
                }
            }
        }
        if ($PSBoundParameters.ContainsKey('ProjectFile') -and $Script:LastProjectFile -ne $ProjectFile) {
            $Script:LastProjectFile = $ProjectFile;
            "Project: $LastProjectFile" | Write-Output;
        }
        $f = $File;
        if ([string]::IsNullOrEmpty($f)) { $f = $TargetFile }
        if ([string]::IsNullOrEmpty($f)) {
            if ([string]::IsNullOrEmpty($TargetName)) {
                if ([string]::IsNullOrEmpty($TaskName)) {
                    if ($m.Length -gt 0) { $m | Write-Output }
                } else {
                    "$TaskName`: $m".Trim() | Write-Output;
                }
            } else {
                "$TargetName`: $m".Trim() | Write-Output;
                if (-not [string]::IsNullOrEmpty($TaskName)) {
                    "  Task: $TaskName" | Write-Output;
                }
            }
        } else {
            "$f`: $m".Trim() | Write-Output;
            if (-not [string]::IsNullOrEmpty($TargetName)) {
                "  Name: $TargetName" | Write-Output;
            }
            if (-not [string]::IsNullOrEmpty($TaskName)) {
                "  Task: $TaskName" | Write-Output;
            }
        }
        if (-not [string]::IsNullOrEmpty($Code)) {
            "  Code: $Code" | Write-Output;
        }
        if (-not [string]::IsNullOrEmpty($Subcategory)) {
            "  Subcategory: $Subcategory" | Write-Output;
        }
        if ($PSBoundParameters.ContainsKey('Importance')) {
            "  Importance: $Importance" | Write-Output;
        }
        if (-not [string]::IsNullOrEmpty($Message)) {
            $arr = @($Message -split '\r\n?|\n');
            "  " + $arr[0] | Write-Output;
            for ($i = 1; $i -lt $arr.Count; $i++) { ("    " + $arr[$i]) | Write-Output; }
        }

        $Properties = @{
            Type = $Type;
            Code = $Code;
            LineNumber = $LineNumber;
            ColumnNumber = $ColumnNumber;
            Message = $StatusText;
        };
        if ($PSBoundParameters.ContainsKey('Timestamp')) {
            $Properties['Timestamp'] = $Timestamp;
        } else {
            $Properties['Timestamp'] = [DateTime]::Now;
        }
        
        if (-not [string]::IsNullOrEmpty($Message)) {
            if ($Properties['Message'].Length -gt 0) {
                $Properties['Message'] = "$($Properties['Message']): $Message";
            } else {
                $Properties['Message'] = $Message;
            }
        }
        if ($PSBoundParameters.ContainsKey('Importance')) {
            if ($Properties['Message'].Length -gt 0) {
                $Properties['Message'] = "$($Properties['Message'])`r`nImportance: $Importance";
            } else {
                $Properties['Message'] = "Importance: $Importance";
            }
        }
        if ([string]::IsNullOrEmpty($File)) {
            if ([string]::IsNullOrEmpty($TargetFile)) {
                $Properties['File'] = $ProjectFile;
            } else {
                $Properties['File'] = $TargetFile;
                if (-not [string]::IsNullOrEmpty($ProjectFile)) {
                    if ($Properties['Message'].Length -gt 0) {
                        $Properties['Message'] = "$($Properties['Message'])`r`nProject File: $ProjectFile";
                    } else {
                        $Properties['Message'] = "Project File: $ProjectFile";
                    }
                }
            }
        } else {
            $Properties['File'] = $File;
            if (-not [string]::IsNullOrEmpty($TargetFile)) {
                if ($Properties['Message'].Length -gt 0) {
                    $Properties['Message'] = "$($Properties['Message'])`r`nTarget File: $TargetFile";
                } else {
                    $Properties['Message'] = "Target File: $TargetFile";
                }
            }
            if (-not [string]::IsNullOrEmpty($ProjectFile)) {
                if ($Properties['Message'].Length -gt 0) {
                    $Properties['Message'] = "$($Properties['Message'])`r`nProject File: $ProjectFile";
                } else {
                    $Properties['Message'] = "Project File: $ProjectFile";
                }
            }
        }
        if ([string]::IsNullOrEmpty($TargetName)) {
            if ([string]::IsNullOrEmpty($TaskName)) {
                if ($Properties['File'].Length -eq 0) {
                    $Properties['Name'] = '';
                } else {
                    $Properties['Name'] = $Properties['File'] | Split-Path -Leaf;
                }
            } else {
                $Properties['Name'] = $TaskName;
            }
        } else {
            $Properties['Name'] = $TargetName;
            if (-not [string]::IsNullOrEmpty($TaskName)) {
                if ($Properties['Message'].Length -gt 0) {
                    $Properties['Message'] = "$($Properties['Message'])`r`nTask Name: $TaskName";
                } else {
                    $Properties['Message'] = "Task Name: $TaskName";
                }
            }
        }

        $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
    }
}

Function Write-Logger {
    Param(
        [Parameter(Mandatory = $true)]
        [LteDev.MSBuildLogHelper]$Logger
    )

    $Collection = $Logger.Read();
    if ($Collection.Count -gt 0) {
        foreach ($obj in $Collection) {
            switch ($obj) {
                { $_ -is [Microsoft.Build.Framework.BuildErrorEventArgs] } {
                    ($obj | Out-BuildEventArgs -Type 'BuildError') | ForEach-Object { $Host.UI.WriteErrorLine($_) }
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.BuildWarningEventArgs] } {
                    ($obj | Out-BuildEventArgs -Type 'BuildWarning') | ForEach-Object { $Host.UI.WriteWarningLine($_) }
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.BuildMessageEventArgs] } {
                    ($obj | Out-BuildEventArgs -Type 'BuildMessage') | ForEach-Object { $Host.UI.WriteLine($_) }
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.BuildStartedEventArgs] } {
                    $arr = @($obj.Message -split '\r\n?|\n');
                    $Host.UI.WriteLine("Build Started: $($arr[0])");
                    for ($i = 1; $i -lt $arr.Count; $i++) { $Host.UI.WriteLine("  " + $arr[$i]) }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                        Type = 'BuildStarted';
                        Code = '';
                        LineNumber = 0;
                        ColumnNumber = 0;
                        Message = $obj.Message;
                        Timestamp = $obj.Timestamp;
                        File = '';
                        Name = '';
                    }));
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.BuildFinishedEventArgs] } {
                    $Properties = @{
                        Type = 'BuildFinished';
                        Code = '';
                        LineNumber = 0;
                        ColumnNumber = 0;
                        Message = '';
                        Timestamp = $obj.Timestamp;
                        File = '';
                        Name = '';
                    };
                    $arr = @($obj.Message -split '\r\n?|\n');
                    if ($obj.Succeeded) {
                        $Host.UI.WriteLine("Build Succeeded: $($arr[0])");
                        for ($i = 1; $i -lt $arr.Count; $i++) { $Host.UI.WriteLine("  " + $arr[$i]) }
                        $Properties['Message'] = 'Build Succeeded';
                    } else {
                        $Host.UI.WriteLine("Build Failed: $($arr[0])");
                        for ($i = 1; $i -lt $arr.Count; $i++) { $Host.UI.WriteWarningLine("  " + $arr[$i]) }
                        $Properties['Message'] = 'Build Failed';
                    }
                    if (-not [string]::IsNullOrEmpty($obj.Message)) {
                        $Properties['Message'] = "$($Properties['Message']): $($obj.Message)";
                    }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.ProjectStartedEventArgs] } {
                    $Properties = @{
                        Type = 'ProjectStarted';
                        Code = '';
                        LineNumber = 0;
                        ColumnNumber = 0;
                        Message = $obj.Message;
                        Timestamp = $obj.Timestamp;
                        File = $obj.ProjectFile;
                    };
                    if ($Properties['File'].Length -eq 0) {
                        $Properties['Name'] = '';
                    } else {
                        $Properties['Name'] = $Properties['File'] | Split-Path -Leaf;
                    }
                    $ProjectStartedEventArgs.ProjectFile;
                    if (-not [string]::IsNullOrEmpty($obj.ToolsVersion)) {
                        if ($Properties['Message'].Length -eq 0) {
                            $Properties['Message'] = "Tools Version: $($Properties['ToolsVersion'])";
                        } else {
                            $Properties['Message'] = "$($Properties['Message'])`r`nTools Version: $($Properties['ToolsVersion'])";
                        }
                    }
                    $Script:LastProjectFile = $obj.ProjectFile;
                    $Host.UI.WriteLine("Project Started: $($obj.ProjectFile) ($($obj.ProjectId))");
                    if (-not [string]::IsNullOrEmpty($obj.Message)) {
                        @($obj.Message -split '\r\n?|\n') | ForEach-Object { $Host.UI.WriteLine("  " + $_) }
                    }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.ProjectFinishedEventArgs] } {
                    $Properties = @{
                        Type = 'ProjectFinished';
                        Code = '';
                        LineNumber = 0;
                        ColumnNumber = 0;
                        Message = '';
                        Timestamp = $obj.Timestamp;
                        File = $obj.ProjectFile;
                    };
                    if ($Properties['File'].Length -eq 0) {
                        $Properties['Name'] = '';
                    } else {
                        $Properties['Name'] = $Properties['File'] | Split-Path -Leaf;
                    }
                    if ($obj.Succeeded) {
                        $Host.UI.WriteLine("Project Succeeded: $($obj.ProjectFile)");
                        if (-not [string]::IsNullOrEmpty($obj.Message)) {
                            @($obj.Message -split '\r\n?|\n') | ForEach-Object { $Host.UI.WriteLine("  " + $_) }
                        }
                        $Properties['Message'] = 'Project Succeeded';
                    } else {
                        $Host.UI.WriteWarningLine("Project Failed: $($obj.ProjectFile)");
                        if (-not [string]::IsNullOrEmpty($obj.Message)) {
                            @($obj.Message -split '\r\n?|\n') | ForEach-Object { $Host.UI.WriteErrorLine("  " + $_) }
                        }
                        $Properties['Message'] = 'Project Failed';
                    }
                    if (-not [string]::IsNullOrEmpty($obj.Message)) {
                        $Properties['Message'] = "$($Properties['Message']): $($obj.Message)";
                    }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.TargetStartedEventArgs] } {
                    ($obj | Out-BuildEventArgs -Type 'TargetStarted' ) | ForEach-Object { $Host.UI.WriteLine($_) }
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.TargetFinishedEventArgs] } {
                    if ($obj.Succeeded) {
                        ($obj | Out-BuildEventArgs -Type 'TargetFinished' -StatusText 'Succeeded') | ForEach-Object { $Host.UI.WriteLine($_) }
                    } else {
                        ($obj | Out-BuildEventArgs -Type 'TargetFinished' -StatusText 'Failed') | ForEach-Object { $Host.UI.WriteErrorLine($_) }
                    }
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.TaskStartedEventArgs] } {
                    ($obj | Out-BuildEventArgs -Type 'TaskStarted') | ForEach-Object { $Host.UI.WriteLine($_) }
                    break;
                }
                { $_ -is [Microsoft.Build.Framework.TaskFinishedEventArgs] } {
                    if ($obj.Succeeded) {
                        ($obj | Out-BuildEventArgs -Type 'TaskFinished' -StatusText 'Succeeded') | ForEach-Object { $Host.UI.WriteLine($_) }
                    } else {
                        ($obj | Out-BuildEventArgs -Type 'TaskFinished' -StatusText 'Failed') | ForEach-Object { $Host.UI.WriteErrorLine($_) }
                    }
                    break;
                }
                { $_ -is [System.Management.Automation.ErrorRecord] } {
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                        Type = 'ErrorRecord';
                        Code = $obj.FullyQualifiedErrorId;
                        LineNumber = $obj.InvocationInfo.ScriptLineNumber;
                        ColumnNumber = $obj.InvocationInfo.OffsetInLine;
                        Message = $obj.ToString();
                        Timestamp = $obj.Timestamp;
                        File = $obj.InvocationInfo.ScriptName;
                    }));
                    if ($Properties['File'].Length -eq 0) {
                        $Properties['Name'] = '';
                    } else {
                        $Properties['Name'] = $Properties['File'] | Split-Path -Leaf;
                    }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
                    $arr = @($obj.ToString() -split '\r\n?|\n');
                    $Host.UI.WriteErrorLine("Error: $($arr[0])");
                    for ($i = 1; $i -lt $arr.Count; $i++) { $Host.UI.WriteErrorLine("  " + $arr[$i]) }
                    break;
                }
                { $_ -is [System.Management.Automation.WarningRecord] } {
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                        Type = 'WarningRecord';
                        Code = $obj.FullyQualifiedWarningId;
                        LineNumber = $obj.InvocationInfo.ScriptLineNumber;
                        ColumnNumber = $obj.InvocationInfo.OffsetInLine;
                        Message = $obj.Message;
                        Timestamp = [DateTime]::Now;
                        File = $obj.InvocationInfo.ScriptName;
                    }));
                    if ($Properties['File'].Length -eq 0) {
                        $Properties['Name'] = '';
                    } else {
                        $Properties['Name'] = $Properties['File'] | Split-Path -Leaf;
                    }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
                    $arr = @($obj.Message -split '\r\n?|\n');
                    $Host.UI.WriteWarningLine($arr[0]);
                    for ($i = 1; $i -lt $arr.Count; $i++) { $Host.UI.WriteWarningLine("  " + $arr[$i]) }
                    break;
                }
                { $_ -is [System.Exception] } {
                    $errObj = $obj;
                    if ($errObj -is [System.Management.Automation.RuntimeException] -and $errObj.InnerException -ne $null) {
                        $errObj = $errObj.InnerException;
                    }
                    $Properties = @{
                        Type = $errObj.GetType().Name;
                        Code = '';
                        LineNumber = 0;
                        ColumnNumber = 0;
                        Message = $errObj.Message;
                        Timestamp = [DateTime]::Now;
                        File = '';
                        Name = '';
                    }
                    $Script:BuildOutput.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property ));
                    $arr = @($obj.ToString() -split '\r\n?|\n');
                    $Host.UI.WriteErrorLine("Exception: $($arr[0])");
                    for ($i = 1; $i -lt $arr.Count; $i++) { $Host.UI.WriteErrorLine("  " + $arr[$i]) }
                    break;
                }
                { $_ -is [string] } {
                    Out-BuildEventArgs -Message $_;
                    break;
                }
                default {
                    $Host.UI.WriteLine($obj.GetType().FullName);
                    break;
                }
            }
        }
    }
}

if ($Project -ne $null) {
    $Project.ProjectCollection.UnloadAllProjects();
    $Project = $null;
}
$csproj_name = 'LteDev.csproj';
$csproj_path = $PSScriptRoot | Join-Path -ChildPath $csproj_name;
$Project = [Microsoft.Build.Evaluation.Project]::new($csproj_path);
if ($Project -eq $null) { return }
$Project.SetProperty('Configuration', $Configuration) | Out-Null;
$Project.SetProperty('Platform', $Platform) | Out-Null;
$Project.ReevaluateIfNecessary();
$ShouldBuild = $ForceRebuild;
if (-not $ShouldBuild) {
    $TargetPath = $Project.GetPropertyValue('TargetPath');
    if (Test-Path -Path $TargetPath) {
        $LastBuildTime = Get-ItemPropertyValue -Name 'LastWriteTimeUtc' -Path $TargetPath;
        if ((Get-ItemPropertyValue -Name 'LastWriteTimeUtc' -Path $csproj_path) -gt $LastBuildTime) {
            $ShouldBuild = $true;
        } else {
            $NewModifications = @($Project.Items | Where-Object {
                if (('Compile', 'Content', 'EmbeddedResource') -contains $_.ItemType) {
                    $p = $PSScriptRoot | Join-Path -ChildPath $_.EvaluatedInclude;
                    (Test-Path -LiteralPath $p) -and (Get-ItemPropertyValue -Name 'LastWriteTimeUtc' -Path $p) -gt $LastBuildTime;
                } else { $false }
            });
            $ShouldBuild = ($NewModifications.Count -gt 0);
        }
    } else {
        $ShouldBuild = $true;
    }
}
if ($ShouldBuild) {
    $Logger = [LteDev.MSBuildLogHelper]::new();
    $Logger.StartBuild($Project);
    $Logger.Verbosity = [Microsoft.Build.Framework.LoggerVerbosity]::Normal;
    $Output = @();
    while (-not $Logger.WaitBuild(1000)) {
        Write-Logger -Logger $Logger;
    }

    Write-Logger -Logger $Logger;

    $ResultMesage = $csproj_name;
    $Succeeded = $Logger.EndBuild();
    if ($Succeeded) {
        $ResultMesage = "$ResultMesage Build Succeeded";
        $ResultMesage | Write-Output;
    } else {
        $ResultMesage = "$ResultMesage Build Failed";
        $ResultMesage | Write-Warning;
    }
    for ($i = 0; $i -lt $Script:BuildOutput.Count; $i++) {
        $Script:BuildOutput[$i] | Add-Member -MemberType NoteProperty -Name 'Order' -Value $i;
    }
    $Script:BuildOutput | Out-GridView -Title $ResultMesage;
    if (-not $Succeeded) { return }
} else {
    'Build not necessary' | Write-Host;
}

$Error.Clear();

if ((Get-Module -Name $module_name) -ne $null) { Remove-Module -Name $module_name -ErrorAction Stop }
$CmdErrors = @();
$CmdWarnings = @();
$ModuleInfo = $null;
$ModuleInfo = Import-module -Name (($PSScriptRoot | Join-Path -ChildPath $Project.GetPropertyValue('OutputPath')) | Join-Path -ChildPath "$module_name.psd1") `
    -ErrorVariable 'CmdErrors' -WarningVariable 'CmdWarnings' -ErrorAction Continue -PassThru;
$ErrorObj = @();
if ($CmdErrors.Count -gt 0) {
    $ErrorObj = @(@($Error) | Select-Object -Property @{
        Label = 'Message'; Expression = { $_.ToString() } }, @{
        Label = 'InnerMessage'; Expression = {
            $m = $_.ToString();
            for ($e = $_.Exception; $e -ne $null; $e = $e.InnerException) {
                if ($e.Message -ne $m) { return $e.Message }
            }
            return '';
        } }, @{
        Label = 'Position'; Expression = { if ($_.InvocationInfo -eq $null) { return '' } $_.InvocationInfo.PositionMessage } }, @{
        Label = 'Category'; Expression = { if ($_.CategoryInfo -eq $null) { return '' } $_.CategoryInfo.GetMessage() } }, @{
        Label = 'Stack Trace'; Expression = { if ($_.ScriptStackTrace -eq $null) { return '' } $_.ScriptStackTrace } });
}
if ($CmdWarnings.Count -gt 0) { $CmdWarnings | Out-GridView -Title 'Module Load Warnings' }
if ($ErrorObj.Count -gt 0) { $ErrorObj | Out-GridView -Title 'Module Load Errors' }
if ($ModuleInfo -eq $null) {
    Write-Warning -Message 'Module load failed';
    return;
}
if ($ErrorObj.Count -gt 0) {
    Write-Warning -Message 'Test scripts not invoked due to errors during module load';
    return;
}

$TestScripts = @(Get-ChildItem -Path ($PSScriptRoot | Join-Path -ChildPath 'TestScripts') -Filter 'Test-*.ps1');
$ErrorObj = @();
$MessageObj = @();
$TestScripts | ForEach-Object {
    $CmdErrors = @();
    $CmdWarnings = @();
    $Name = $_.Name;
    Invoke-Expression -Command $_.FullName -ErrorVariable 'CmdErrors' -WarningVariable 'CmdWarnings' -ErrorAction Continue;
    if ($CmdErrors.Count -gt 0) {
        $ErrorObj = $ErrorObj + @(@($CmdErrors) | Select-Object -Property @{
            Label = 'Script'; Expression = { $Name } }, @{
            Label = 'Message'; Expression = { $_.ToString() } }, @{
            Label = 'InnerMessage'; Expression = {
                $m = $_.ToString();
                for ($e = $_.Exception; $e -ne $null; $e = $e.InnerException) {
                    if ($e.Message -ne $m) { return $e.Message }
                }
                return '';
            } }, @{
            Label = 'Position'; Expression = { if ($_.InvocationInfo -eq $null) { return '' } $_.InvocationInfo.PositionMessage } }, @{
            Label = 'Category'; Expression = { if ($_.CategoryInfo -eq $null) { return '' } $_.CategoryInfo.GetMessage() } }, @{
            Label = 'Stack Trace'; Expression = { if ($_.ScriptStackTrace -eq $null) { return '' } $_.ScriptStackTrace } });
    }
    if ($CmdWarnings.Count -gt 0) {
        $MessageObj = $MessageObj + @(@($CmdWarnings) | Select-Object -Property @{ Label = 'Type'; Expression = { 'Warning' } }, @{ Label = 'Script'; Expression = { $Name } }, 'Message');
    } else {
        if ($CmdErrors.Count -eq 0) { $MessageObj = $MessageObj + @([PSCustomObject]@{ Type = 'Success'; Script = $Name; Message = 'No errors or warnings' }) };
    }
}

if ($ErrorObj.Count -gt 0) {
    $ErrorObj | Out-GridView -Title 'Test Errors';
}

if ($WarningObj.Count -gt 0) {
    $WarningObj | Out-GridView -Title 'Test Warnings';
}