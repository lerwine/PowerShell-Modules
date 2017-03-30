$Script:NotepadPlusPlusPath = 'C:\Program Files\Notepad++\notepad++.exe';

Add-Type -Path (($MyInvocation.MyCommand.Definition | Split-Path -Parent) | Join-Path -ChildPath 'ResolveDifferences.cs') -ReferencedAssemblies ([System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms').Location) -ErrorAction Stop;

<#
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

Function New-DifferenceItem {
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        
        [Parameter(Mandatory = $true)]
        [string]$Change,

        [System.DateTime]$LeftMod,

        [System.DateTime]$RightMod
    )

    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        Diff = $DiffItem;
        IsSelected = $IsSelected;
        Index = $Index;
        LItem = $LItem;
        RItem = $RItem;
    };
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

#>

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
    $DialogResult = $FolderBrowserDialog.ShowDialog((New-Object -TypeName 'FileSystemIndexLib.WindowOwner'));
    if ($DialogResult -eq [System.Windows.Forms.DialogResult]::OK) {
        $FolderBrowserDialog.SelectedPath | Write-Output;
    }
    $FolderBrowserDialog.Dispose();
}

<#
Function New-DifferenceItemSelection {
    Param(
        [Parameter(Mandatory = $true)]
        [PSObject]$DiffItem,
        
        [Parameter(Mandatory = $true)]
        [bool]$IsSelected,
        
        [Parameter(Mandatory = $true)]
        [int]$Index,

        [System.IO.FileSystemInfo]$LItem,

        [System.IO.FileSystemInfo]$RItem
    )

    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        Diff = $DiffItem;
        IsSelected = $IsSelected;
        Index = $Index;
        LItem = $LItem;
        RItem = $RItem;
    };
}

Function Select-DifferenceItem {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSObject]$DiffItem
    )

    Begin { $DiffCollection = @() }

    Process { $DiffCollection += @($DiffItem) }

    End {
        $SelectedItems = @($DiffCollection | Out-GridView -Title 'Differences' -OutputMode Multiple);
        $Index = 0;
        $DiffCollection | ForEach-Object {
            $Properties = @{ Diff = $_; IsSelected = ($SelectedItems -contains $_); Index = $Index };
            $Index++;
            if ($Properties['IsSelected']) {
                if ($_.LeftMod.Length -eq 0 -and $_.RightMod.Length -eq 0) {
                    $Properties['LItem'] = New-Object 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($LPath, $_.Path));
                    $Properties['RItem'] = New-Object 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($RPath, $_.Path));
                } else {
                    $Properties['LItem'] = New-Object 'System.IO.FileInfo' -ArgumentList ([System.IO.Path]::Combine($LPath, $_.Path));
                    $Properties['RItem'] = New-Object 'System.IO.FileInfo' -ArgumentList ([System.IO.Path]::Combine($RPath, $_.Path));
                }
            } else {
                $Properties['LItem'] = $null;
                $Properties['RItem'] = $null;
            }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
    }
}

Function Select-DifferenceAction {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSObject]$SelectionItem
    )

    Begin {
        $ChoiceOptions = @{
            '>' = 'Copy from left';
            '<' = 'Copy from right';
            'E' = 'Edit Files';
            'Del L' = 'Delete from left';
            'Del R' = 'Delete from left';
            'I' = 'Ignore';
            'N' = 'Do Nothing';
        };
        $MessageLines = @();
    }

    Process {
        if ($SelectionItem.IsSelected) {
            $MessageLines = $MessageLines + @($SelectedItem.Diff.Change);
            if ($SelectedItem.RItem.Exists) {
                $ChoiceOptions.Remove('Del L');
                if ($SelectedItem.LItem.Exists) {
                    $MessageLines = $MessageLines + @(
                        "`t$($SelectedItem.LItem.FullName) ($($SelectedItem.LItem.LastWriteTime.ToString()))",
                        "`t=> $($SelectedItem.RItem.FullName) ($($SelectedItem.RItem.LastWriteTime.ToString()))"
                    );
                    $ChoiceOptions.Remove('Del R');
                } else {
                    $MessageLines = $MessageLines + @(
                        "`t[$($SelectedItem.LItem.FullName)]",
                        "`t=> $($SelectedItem.RItem.FullName) ($($SelectedItem.RItem.LastWriteTime.ToString()))"
                    );
                    $ChoiceOptions.Remove('E');
                    $ChoiceOptions.Remove('>');
                }
            } else {
                $MessageLines = $MessageLines + @(
                    "`t$($SelectedItem.LItem.FullName) ($($SelectedItem.LItem.LastWriteTime.ToString()))",
                    "`t=> [$($SelectedItem.RItem.FullName)]"
                );
                $ChoiceOptions.Remove('E');
                $ChoiceOptions.Remove('<');
            }
        }
    }

    End {
        $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
        $ChoiceOptions.Keys | ForEach-Object { $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $_, $ChoiceOptions[$_])) }
        $Index = $Host.UI.PromptForChoice('Select action', $Message, $Choices, $Choices.Count - 1);
        if ($Index -gt 0 -and $Index -le $Choices.Count) { $Choices[$Index].Label | Write-Output }
    }
}

#>

$LPath = Read-FolderLocation -Description 'Select left side of comparison';
while ($LPath -ne $null) {
    $RPath = Read-FolderLocation -Description 'Select right side of comparison' -SelectedPath ([System.IO.Path]::GetDirectoryName($LPath));
    if ($RPath -eq $null) { break }
    [FileSystemIndexLib.DiffResult[]]$Differences = @([FileSystemIndexLib.DiffResult]::GetResults(0, 0, "Searching folders", $Host.UI, $LPath, $RPath));

    if ($Differences.Count -eq 0) {
        'No changes detected.' | Write-Output;
    } else {
        while ($Differences.Count -gt 0) {
            [FileSystemIndexLib.DiffResult[]]$SelectedItems = @($Differences | Out-GridView -Title 'Differences' -OutputMode Multiple);
            if ($SelectedItems.Length -eq 0) { break; }
            [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = @([FileSystemIndexLib.DiffSelection]::Create($LPath, $RPath, $Differences, $SelectedItems));
            $ChoiceText = [FileSystemIndexLib.DiffSelection]::GetUserChoice($Host.UI, $DiffSelectionItems);
            while ($ChoiceText -ne [FileSystemIndexLib.DiffSelection]::ChoiceLabel_None) {
                switch ($ChoiceText) {
                    { $_ -eq [FileSystemIndexLib.DiffSelection]::ChoiceLabel_CopyLeft } {
                        $OverWrite = @($DiffSelectionItems | Where-Object { $_.IsSelected -and $_.Left.Exists } | ForEach-Object { $_.Left.FullName });
                        if ($OverWrite.Count -eq 0) {
                            [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::CopyToLeft($DiffSelectionItems);
                        } else {
                            if ($OverWrite.Count -eq 1) {
                                if (Read-YesOrNo -Caption 'Confirm' -Message "Are you sure you want to overwrite $($OverWrite[0])?") {
                                    [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::CopyToLeft($DiffSelectionItems);
                                }
                            } else {
                                if (Read-YesOrNo -Caption 'Confirm' -Message "The following will be overwritten:`r`n$($OverWrite -join "`r`n`t")`r`nAre you sure you want to overwrite?") {
                                    [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::CopyToLeft($DiffSelectionItems);
                                }
                            }
                        }
                        break;
                    }
                    { $_ -eq [FileSystemIndexLib.DiffSelection]::ChoiceLabel_CopyRight } {
                        $OverWrite = @($DiffSelectionItems | Where-Object { $_.IsSelected -and $_.Right.Exists } | ForEach-Object { $_.Right.FullName });
                        if ($OverWrite.Count -eq 0) {
                            [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::CopyToRight($DiffSelectionItems);
                        } else {
                            if ($OverWrite.Count -eq 1) {
                                if (Read-YesOrNo -Caption 'Confirm' -Message "Are you sure you want to overwrite $($OverWrite[0])?") {
                                    [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::CopyToRight($DiffSelectionItems);
                                }
                            } else {
                                if (Read-YesOrNo -Caption 'Confirm' -Message "The following will be overwritten:`r`n$($OverWrite -join "`r`n`t")`r`nAre you sure you want to overwrite?") {
                                    [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::CopyToRight($DiffSelectionItems);
                                }
                            }
                        }
                        break;
                    }
                    { $_ -eq [FileSystemIndexLib.DiffSelection]::ChoiceLabel_Edit } {
                        $XmlDocument = New-Object 'System.Xml.XmlDocument';
                        $SessionElement = $XmlDocument.AppendChild($XmlDocument.CreateElement('NotepadPlus')).AppendChild($XmlDocument.CreateElement('Session'));
                        $SessionElement.Attributes.Append($XmlDocument.CreateAttribute('activeView')).Value = '0';
                        $MainViewElement = $SessionElement.AppendChild($XmlDocument.CreateElement('mainView'));
                        $MainViewElement.Attributes.Append($XmlDocument.CreateAttribute('activeIndex')).Value = '0';
                        $DiffSelectionItems | Where-Object { $_.IsSelected } | ForEach-Object { 
                            $FileElement = $MainViewElement.AppendChild($XmlDocument.CreateElement('File'));
                            $FileElement.Attributes.Append($XmlDocument.CreateAttribute('filename')).Value = $_.Left.FullName;
                            $FileElement = $MainViewElement.AppendChild($XmlDocument.CreateElement('File'));
                            $FileElement.Attributes.Append($XmlDocument.CreateAttribute('filename')).Value = $_.Right.FullName;
                        }
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
                        [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::Refresh($DiffSelectionItems, $LPath, $RPath);
                        break;
                    }
                    { $_ -eq [FileSystemIndexLib.DiffSelection]::ChoiceLabel_Delete } {
                        $ToDelete = @($DiffSelectionItems | Where-Object { $_.IsSelected } | ForEach-Object { if ($_.Left.Exists) { $_.Left.FullName } else { $_.Right.FullName } });
                        if ($ToDelete.Count -eq 0) {
                            [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::Delete($DiffSelectionItems);
                        } else {
                            if ($ToDelete.Count -eq 1) {
                                if (Read-YesOrNo -Caption 'Confirm' -Message "Are you sure you want to delete $($ToDelete[0])?") {
                                    [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::Delete($DiffSelectionItems);
                                }
                            } else {
                                if (Read-YesOrNo -Caption 'Confirm' -Message "The following will be deleted:`r`n$($ToDelete -join "`r`n`t")`r`nAre you sure you want to delete?") {
                                    [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = [FileSystemIndexLib.DiffSelection]::Delete($DiffSelectionItems);
                                }
                            }
                        }
                        break;
                    }
                    'I' {
                        [FileSystemIndexLib.DiffSelection[]]$DiffSelectionItems = @($DiffSelectionItems | Where-Object { -not $_.IsSelected });
                        break;
                    }
                }
                $ChoiceText = [FileSystemIndexLib.DiffSelection]::GetUserChoice($Host.UI, $DiffSelectionItems);
            }
            [FileSystemIndexLib.DiffResult[]]$Differences = @($DiffSelectionItems | ForEach-Object { $_.Diff });
        }
    }
    $LPath = Read-FolderLocation -Description 'Select left side of comparison';
}