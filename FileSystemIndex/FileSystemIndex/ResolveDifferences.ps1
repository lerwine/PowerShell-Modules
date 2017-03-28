$Script:NotepadPlusPlusPath = 'C:\Program Files\Notepad++\notepad++.exe';

Add-Type -TypeDefinition @'
namespace DiffReportCLR
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    public class WindowOwner : IWin32Window
    {
        private IntPtr _handle;
        public WindowOwner() : this(Process.GetCurrentProcess().MainWindowHandle) { }
        public WindowOwner(IntPtr handle) { _handle = handle; }
        public IntPtr Handle { get { return _handle; } }
    }
}
'@ -ReferencedAssemblies ([System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms').Location) -ErrorAction Stop;

Function Get-ComparisonItems {
    [CmdletBinding(DefaultParameterSetName = 'Any')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Any')]
        [string]$LPath,

        [Parameter(Mandatory = $true, ParameterSetName = 'Any')]
        [string]$RPath,

        [Parameter(Mandatory = $true, ParameterSetName = 'Directory')]
        [System.IO.DirectoryInfo]$LDirectory,

        [Parameter(Mandatory = $true, ParameterSetName = 'Directory')]
        [System.IO.DirectoryInfo]$RDirectory,

        [Parameter(Mandatory = $true, ParameterSetName = 'Directory')]
        [AllowEmptyString()]
        [string]$RelativePath
    )
    
    if ($PSCmdlet.ParameterSetName -eq 'Any') {
        if ([System.IO.Directory]::Exists($LPath)) {
            Get-ComparisonItems -LDirectory (New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $LPath) -RDirectory (New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $RPath) -RelativePath ''
            if ([System.IO.File]::Exists($RPath)) {
                $Properties = @{
                    Left = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $LPath;
                    Right = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $RPath;
                    Path = [System.IO.Path]::GetFileName($RPath);
                };
                $Properties['Length'] = $Properties['Right'].Length + 1
                New-Object -TypeName 'System.management.Automation.PSObject' -Property $Properties;
            }
        } else {
            if ([System.IO.Directory]::Exists($RPath)) {
                Get-ComparisonItems -LDirectory (New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $LPath) -RDirectory (New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $RPath) -RelativePath ''
                if ([System.IO.File]::Exists($LPath)) {
                    $Properties = @{
                        Left = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $LPath;
                        Right = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $RPath;
                        Path = [System.IO.Path]::GetFileName($LPath);
                    };
                    $Properties['Length'] = $Properties['Left'].Length + 1
                    New-Object -TypeName 'System.management.Automation.PSObject' -Property $Properties;
                }
            } else {
                if ([System.IO.File]::Exists($LPath) -or [System.IO.File]::Exists($RPath)) {
                    $Properties = @{
                        Left = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $LPath;
                        Right = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $RPath;
                        Path = [System.IO.Path]::GetFileName($LPath);
                    };
                    $Properties['Length'] = $Properties['Left'].Length + $Properties['Right'].Length + 2;
                    New-Object -TypeName 'System.management.Automation.PSObject' -Property $Properties;
                }
            }
        }
    } else {
        $Directories = @{}
        $Files = @{}
        $LDirectory.GetFiles() | ForEach-Object { $Files.Add($_.Name.Tolower(), @{ Left = $_; Right = $null; Length = $_.Length + 1; }) }
        $LDirectory.GetDirectories() | ForEach-Object { $Directories.Add($_.Name, @{ Left = $_; Right = $null; Length = 1; }) }
        $RDirectory.GetFiles() | ForEach-Object {
            $Key = $_.Name.ToLower();
            if ($Files.ContainsKey($Key)) {
                $Files[$Key]['Right'] = $_;
                $Files[$Key]['Length'] = $Files[$Key]['Length'] + $_.Length + 1;
            } else {
                $Files.Add($_.Name.Tolower(), @{
                    Left = (New-Object -TypeName 'System.IO.FileInfo' -ArgumentList ([System.IO.Path]::Combine($LDirectory.FullName, $_.Name)));
                    Right = $_;
                    Length = $_.Length + 1;
                });
            }
        }
        $RDirectory.GetDirectories() | ForEach-Object {
            $Key = $_.Name.ToLower();
            if ($Directories.ContainsKey($Key)) {
                $Directories[$Key]['Right'] = $_;
                $Directories[$Key]['Length'] = $Directories[$Key]['Length'] + 1;
            } else {
                $Directories.Add($_.Name.Tolower(), @{
                    Left = (New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($LDirectory.FullName, $_.Name)));
                    Right = $_;
                    Length = 1;
                });
            }
        }
        if ($RelativePath -eq '') {
            $Files.Keys | ForEach-Object { if ($Files[$_]['Left'].Exists) { $Files[$_]['Path'] = $Files[$_]['Left'].Name } else { $Files[$_]['Path'] = $Files[$_]['Right'].Name } }
            $Directories.Keys | ForEach-Object { if ($Directories[$_]['Left'].Exists) { $Directories[$_]['Path'] = $Directories[$_]['Left'].Name } else { $Directories[$_]['Path'] = $Directories[$_]['Right'].Name } }
        } else {
            $Files.Keys | ForEach-Object { if ($Files[$_]['Left'].Exists) { $Files[$_]['Path'] = [System.IO.Path]::Combine($RelativePath, $Files[$_]['Left'].Name) } else { $Files[$_]['Path'] = [System.IO.Path]::Combine($RelativePath, $Files[$_]['Right'].Name) } }
            $Directories.Keys | ForEach-Object { if ($Directories[$_]['Left'].Exists) { $Directories[$_]['Path'] = [System.IO.Path]::Combine($RelativePath, $Directories[$_]['Left'].Name) } else { $Directories[$_]['Path'] = [System.IO.Path]::Combine($RelativePath, $Directories[$_]['Right'].Name) } }
        }
        $Files.Keys | ForEach-Object {
            if ($Files[$_]['Right'] -eq $null) {
                $Files[$_]['Right'] = (New-Object -TypeName 'System.IO.FileInfo' -ArgumentList ([System.IO.Path]::Combine($RDirectory.FullName, $Files[$_]['Left'].Name)));
            }
            New-Object -TypeName 'System.management.Automation.PSObject' -Property $Files[$_];
        }
        $Directories.Keys | ForEach-Object {
            if ($Directories[$_]['Right'] -eq $null) {
                $Directories[$_]['Right'] = (New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($RDirectory.FullName, $Directories[$_]['Left'].Name)));
                New-Object -TypeName 'System.management.Automation.PSObject' -Property $Directories[$_];
            } else {
                if ($Directories[$_]['Left'].Exists) {
                    Get-ComparisonItems -LDirectory $Directories[$_]['Left'] -RDirectory $Directories[$_]['Right'] -RelativePath $Directories[$_]['Path'];
                } else {
                    New-Object -TypeName 'System.management.Automation.PSObject' -Property $Directories[$_];
                }
            }
        }
    }
}

Function Get-DiffReport {
    [CmdletBinding(DefaultParameterSetName = 'Simple')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Simple')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Progress')]
        [System.IO.FileSystemInfo]$Left,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Simple')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Progress')]
        [System.IO.FileSystemInfo]$Right,

        [Parameter(Mandatory = $true, ParameterSetName = 'Simple')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Progress')]
        [string]$Path,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Progress')]
        [long]$Length,

        [Parameter(Mandatory = $true, ParameterSetName = 'Progress')]
        [long]$TotalLength,
        
        [Parameter(ParameterSetName = 'Progress')]
        [switch]$ShowProgress
    )
    
    Begin { [long]$CompletedLength = 0 }

    Process {
        $Properties = @{ Path = $Path };
        
        if ($Left.Exists) {
            $Properties['LeftMod'] = $Left.LastWriteTime;
            if ($Right.Exists) {
                $Properties['RightMod'] = $Left.LastWriteTime;
                if ($Left -is [System.IO.FileInfo]) {
                    if ($Left.Name.EndsWith('.dll')) {
                        if ($Left.Length -ne $Right.Length) {
                            $Properties['Change'] = "Length $($Left.Length) != $($Right.Length)";
                        }
                    } else {
                        if ($ShowProgress) {
                            Write-Progress -Activity 'Looking for changes' -Status 'Reading File' -PercentComplete ([System.Convert]::ToInt32(([System.Convert]::ToDouble($CompletedLength) * 100.0) / [System.Convert]::ToDouble($TotalLength))) -CurrentOperation $Left.DirectoryName;
                        }
                        $LReader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Left.FullName;
                        $RReader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Right.FullName;
                        $Index = 0;
                        $ChangedLine = 0;
                        while ($ChangedLine -eq 0) {
                            $Index++;
                            if ($LReader.EndOfStream) {
                                if (-not $RReader.EndOfStream) { $ChangedLine = $Index }
                                break;
                            } else {
                                if ($RReader.EndOfStream -or $LReader.ReadLine() -ne $RReader.ReadLine()) {
                                    $ChangedLine = $Index;
                                }
                            }
                        }
                        if ($ChangedLine -gt 0) {
                            $Properties['Change'] = "Change starting at line $ChangedLine."
                        }
                    }
                }
            } else {
                $Properties['RightMod'] = '';
                if ($Left -is [System.IO.FileInfo]) {
                    $Properties['Change'] = 'Right file missing';
                } else {
                    $Properties['Change'] = 'Right folder missing';
                }
            }
        } else {
            if ($Right.Exists) {
                $Properties['LeftMod'] = '';
                $Properties['RightMod'] = $Left.LastWriteTime;
                if ($Right -is [System.IO.FileInfo]) {
                    $Properties['Change'] = 'Left file missing';
                } else {
                    $Properties['Change'] = 'Left folder missing';
                }
            }
        }
        if ($Properties['Change'] -ne $null) { New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties }
        $CompletedLength += $Length;
    }
}

Function Copy-DirectoryContents {
    Param(
        [Parameter(Mandatory = $true)]
        [System.IO.DirectoryInfo]$LDirectory,
        [Parameter(Mandatory = $true)]
        [System.IO.DirectoryInfo]$RDirectory
    )

    if (-not $RDirectory.Exists) {
        $RDirectory.Create();
        $RDirectory.Refresh();
    }

    $LDirectory.GetFiles() | ForEach-Object {
        [System.IO.File]::Copy($_.FullName, [System.IO.Path]::Combine($RDirectory.FullName, $_.Name), $true);
    }
    $LDirectory.GetDirectories() | ForEach-Object { Copy-DirectoryContents -LDirectory $_ -RDirectory ([System.IO.Path]::Combine($RDirectory.FullName, $_.Name)) }
}

Function Read-YesOrNo {
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Caption,
        [Parameter(Mandatory = $true)]
        [string]$Message
    )
    
    $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'Y', 'Yes'));
    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'No'));
    $Index = $Host.UI.PromptForChoice($Caption, $Message, $Choices, 1);
    if ($Index -ne $null) {
        if ($Index -eq 0) { $true } else { if ($Index -eq 1) { $false } }
    }
}

Function Read-FolderLocation {
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Description,

        [AllowEmptyString()]
        [AllowNull()]
        [string]$SelectedPath
    )
    $FolderBrowserDialog = New-Object -TypeName 'System.Windows.Forms.FolderBrowserDialog';
    $FolderBrowserDialog.Description = $Description;
    if ([String]::IsNullOrEmpty($SelectedPath)) {
        $FolderBrowserDialog.SelectedPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::Personal);
    } else {
        $FolderBrowserDialog.SelectedPath = $SelectedPath;
    }
    $FolderBrowserDialog.ShowNewFolderButton = $false;
    $DialogResult = $FolderBrowserDialog.ShowDialog((New-Object -TypeName 'DiffReportCLR.WindowOwner'));
    if ($DialogResult -eq [System.Windows.Forms.DialogResult]::OK) {
        $FolderBrowserDialog.SelectedPath | Write-Output;
    }
    $FolderBrowserDialog.Dispose();
}

$LPath = Read-FolderLocation -Description 'Select left side of comparison';
while ($LPath -ne $null) {
    $RPath = Read-FolderLocation -Description 'Select right side of comparison' -SelectedPath ([System.IO.Path]::GetDirectoryName($LPath));
    if ($RPath -eq $null) { break }
    Write-Progress -Activity 'Looking for changes' -Status 'Initializing' -PercentComplete 0 -CurrentOperation 'Reading directory contents';
    $Items = @(Get-ComparisonItems -LPath $LPath -RPath $RPath);
    [long]$TotalLength = 0;
    $Items | ForEach-Object { $TotalLength += $_.Length }
    $Differences = @($Items | Get-DiffReport -TotalLength $TotalLength -ShowProgress);
    Write-Progress -Activity 'Looking for changes' -Status 'Completed' -PercentComplete 100 -Completed;

    if ($Differences.Count -eq 0) {
        'No changes detected.' | Write-Output;
    } else {
        while ($Differences.Count -gt 0) {
            $SelectedItem = $Differences | Out-GridView -Title 'Differences' -PassThru;
            if ($SelectedItem -eq $null) { break }
            $LItem = $null;
            $Ritem = $null;
            if ($SelectedItem.LeftMod.Length -eq 0 -and $SelectedItem.RightMod.Length -eq 0) {
                $LItem = New-Object 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($LPath, $SelectedItem.Path));
                $RItem = New-Object 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($RPath, $SelectedItem.Path));
            } else {
                $LItem = New-Object 'System.IO.FileInfo' -ArgumentList ([System.IO.Path]::Combine($LPath, $SelectedItem.Path));
                $RItem = New-Object 'System.IO.FileInfo' -ArgumentList ([System.IO.Path]::Combine($RPath, $SelectedItem.Path));
            }
            
            $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
            $Message = $null;
            if ($LItem.Exists) {
                $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '>', 'Copy from left'));
                if ($Ritem.Exists) {
                    $Message = "$($SelectedItem.Change)`r`n`r`n$($LItem.FullName) ($($LItem.LastWriteTime.ToString()))`r`n => $($RItem.FullName) ($($RItem.LastWriteTime.ToString()))";
                    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '<', 'Copy from right'));
                    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'E', 'Edit Files'));
                } else {
                    $Message = "$($SelectedItem.Change)`r`n`r`n$($LItem.FullName) ($($LItem.LastWriteTime.ToString()))`r`n => $($RItem.FullName)?";
                    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'X', 'Delete from left'));
                }
            } else {
                $Message = "$($SelectedItem.Change)`r`n`r`n$($LItem.FullName)?`r`n => $($RItem.FullName) ($($RItem.LastWriteTime.ToString()))";
                $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '<', 'Copy from right'));
                $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'X', 'Delete from right'));
            }
            $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'I', 'Ignore'));
            $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'Do nothing'));
            $Index = $Host.UI.PromptForChoice('Select action', $Message, $Choices, $Choices.Count - 1);
            while ($Index -ne $null -and $Index -ge 0 -and $Index -lt ($Choices.Count - 1)) {
                $RemoveSelectedItem = $false;
                switch ($Choices[$Index].Label) {
                    '>' {
                        if ($LItem -is [System.IO.FileInfo]) {
                            if ((-not $Ritem.Exists) -or (Read-YesOrNo -Caption 'Confirm', "Are you sure you want to overwrite $($Ritem.FullName)?")) {
                                [System.IO.File]::Copy($LItem.FullName, $RItem.FullName, $true);
                                $RemoveSelectedItem = $true;
                            }
                        } else {
                            Copy-DirectoryContents -LDirectory $LItem -RDirectory $RItem;
                            $RemoveSelectedItem = $true;
                        }
                        break;
                    }
                    '<' {
                        if ($RItem -is [System.IO.FileInfo]) {
                            if ((-not $LItem.Exists) -or (Read-YesOrNo -Caption 'Confirm', "Are you sure you want to overwrite $($LItem.FullName)?")) {
                                [System.IO.File]::Copy($RItem.FullName, $LItem.FullName, $true);
                                $RemoveSelectedItem = $true;
                            }
                        } else {
                            Copy-DirectoryContents -LDirectory $Ritem -RDirectory $LItem;
                            $RemoveSelectedItem = $true;
                        }
                        break;
                    }
                    'E' {
                        $XmlDocument = New-Object 'System.Xml.XmlDocument';
                        $SessionElement = $XmlDocument.AppendChild($XmlDocument.CreateElement('NotepadPlus')).AppendChild($XmlDocument.CreateElement('Session'));
                        $SessionElement.Attributes.Append($XmlDocument.CreateAttribute('activeView')).Value = '0';
                        $MainViewElement = $SessionElement.AppendChild($XmlDocument.CreateElement('mainView'));
                        $MainViewElement.Attributes.Append($XmlDocument.CreateAttribute('activeIndex')).Value = '0';
                        $FileElement = $MainViewElement.AppendChild($XmlDocument.CreateElement('File'));
                        $FileElement.Attributes.Append($XmlDocument.CreateAttribute('filename')).Value = $LItem.FullName;
                        $FileElement = $MainViewElement.AppendChild($XmlDocument.CreateElement('File'));
                        $FileElement.Attributes.Append($XmlDocument.CreateAttribute('filename')).Value = $Ritem.FullName;
                        $subViewElement = $SessionElement.AppendChild($XmlDocument.CreateElement('subView'));
                        $subViewElement.Attributes.Append($XmlDocument.CreateAttribute('activeIndex')).Value = '0';
                        $subViewElement.IsEmpty = $true;
                        $TempFileName = [System.IO.Path]::GetTempFileName();
                        $XmlWriter = [System.Xml.XmlWriter]::Create($TempFileName, (New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
                            Indent = $true;
                            OmitXmlDeclaration = $true;
                            Encoding = New-Object -TypeName 'System.Text.UTF8Encoding' -ArgumentList $false;
                        }));
                        $XmlDocument.WriteTo($XmlWriter);
                        $XmlWriter.Flush();
                        $XmlWriter.Close();
                        Start-Process -FilePath $Script:NotepadPlusPlusPath -ArgumentList ('-multiInst', '-nosession', '-openSession', $TempFileName) -LoadUserProfile -Wait
                        [System.IO.File]::Delete($TempFileName);
                        $LItem.Refresh();
                        $RItem.Refresh();
                        $NewItem = Get-DiffReport -Left $LItem -Right $RItem -Path $SelectedItem.Path;
                        if ($NewItem -eq $null) {
                            $RemoveSelectedItem = $true;
                        } else {
                            $SelectedItem = $NewItem;
                            $Differences = @($Differences | ForEach-Object {
                                if ($_.Path -ne $SelectedItem.Path -or $_.Change -ne $SelectedItem.Change) {
                                    $_ | Write-Output;
                                } else {
                                    $NewItem | Write-Output;
                                }
                            });
                        }
                        break;
                    }
                    'X' {
                        if ($LItem.Exists) {
                            if ($LItem -is [System.IO.File]) {
                                if (Read-YesOrNo -Caption 'Confirm', "Are you sure you want to delete $($LItem.FullName)?") {
                                    $LItem.Delete();
                                    $RemoveSelectedItem = $true;
                                }
                            } else {
                                if (Read-YesOrNo -Caption 'Confirm', "Are you sure you want to delete $($LItem.FullName) and all its contents?") {
                                    $LItem.Delete($true);
                                    $RemoveSelectedItem = $true;
                                }
                            }
                        } else {
                            if ($Ritem -is [System.IO.File]) {
                                if (Read-YesOrNo -Caption 'Confirm', "Are you sure you want to delete $($Ritem.FullName)?") {
                                    $LItem.Delete();
                                    $RemoveSelectedItem = $true;
                                }
                            } else {
                                if (Read-YesOrNo -Caption 'Confirm', "Are you sure you want to delete $($Ritem.FullName) and all its contents?") {
                                    $LItem.Delete($true);
                                    $RemoveSelectedItem = $true;
                                }
                            }
                        }
                        break;
                    }
                    'I' {
                        $RemoveSelectedItem = $true;
                        break;
                    }
                }
                if ($RemoveSelectedItem) {
                    $Differences = @($Differences | Where-Object {
                        $_.Path -ne $SelectedItem.Path -or $_.Change -ne $SelectedItem.Change
                    });
                    break;
                }
                $Choices.Clear();
                if ($LItem.Exists) {
                    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '>', 'Copy from left'));
                    if ($Ritem.Exists) {
                        $Message = "$($SelectedItem.Change)`r`n`r`n$($LItem.FullName) ($($LItem.LastWriteTime.ToString()))`r`n  => $($RItem.FullName) ($($RItem.LastWriteTime.ToString()))";
                        $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '<', 'Copy from right'));
                        $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'E', 'Edit Files'));
                    } else {
                        $Message = "$($SelectedItem.Change)`r`n`r`n$($LItem.FullName) ($($LItem.LastWriteTime.ToString()))`r`n  => $($RItem.FullName)?";
                        $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'X', 'Delete from left'));
                    }
                } else {
                    $Message = "$($SelectedItem.Change)`r`n`r`n$($LItem.FullName)?`r`n => $($RItem.FullName) ($($RItem.LastWriteTime.ToString()))";
                    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '<', 'Copy from right'));
                    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'X', 'Delete from right'));
                }
                $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'I', 'Ignore'));
                $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'Do nothing'));
                $Index = $Host.UI.PromptForChoice('Select action', $Message, $Choices, $Choices.Count - 1);
            }
        }
    }
    $LPath = Read-FolderLocation -Description 'Select left side of comparison';
}