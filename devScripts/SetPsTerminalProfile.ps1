<#
    .SYNOPSIS
        Create / Modify PowerShell Profile for build task.
    
    .DESCRIPTION
        This script was written for windows hosts to create or modify the PowerShell profile script for
        the current user on all hosts so that the terminal width is increased. This serves as a work-around
        for an issue that may be encountered during a build where the problem matcher fails to detect the full
        path to a source code file due to line wrapping.

        This script was created for windows hosts and will need to be modified to run on other operating systems. 
#>
Param(
    [int]$MinWidth = 2048,
    [int]$MinHeight = 6000
)
$old_WarningPreference = $WarningPreference;
$old_InformationPreference = $InformationPreference;
$old_ErrorActionPreference = $ErrorActionPreference;
$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop;
$WarningPreference = [System.Management.Automation.ActionPreference]::Continue;
$InformationPreference = [System.Management.Automation.ActionPreference]::Ignore;
$IsNonInteractive = [System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle -eq [System.IntPtr]::Zero;

try {
    $CurrentHost = Get-Host;
    $Abort = $false;
    $ScriptText = @();
    $FileExists = $profile.CurrentUserAllHosts | Test-Path -PathType Leaf;
    if ($FileExists) {
        if ($IsNonInteractive) {
            $Abort = $true;
            Write-Warning -Message "Cannot modify PowerShell profile script $($profile.CurrentUserAllHosts): File already exists and session is non-interactive.";
            $CurrentHost.SetShouldExit(1);
        } else {
            $ScriptText = @(Get-Content -Path $profile.CurrentUserAllHosts);
        }
    } else {
        if ($profile.CurrentUserAllHosts | Test-Path) {
            $Abort = $true;
            Write-Warning -Message "Cannot create PowerShell profile script. Subdirectory $($profile.CurrentUserAllHosts) exists where a file should be.";
            $CurrentHost.SetShouldExit(2);
        }
    }

    if (-not $Abort) {
        $SelectedLabel = 'Cancel';
        if ($IsNonInteractive) {
            if ($FileExists -and $ScriptText.Count -gt 0 -and ($ScriptText | Out-String).Trim().Length -gt 0) {
                $SelectedLabel = 'Prepend';
            } else {
                $SelectedLabel = 'Create';
            }
        } else {
            [System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]]$ChoiceCollection = New-Object -TypeName '[System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]]';
            $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('Cancel', 'Do not make any changes'));
            $SelectedIndex = -1;
            if ($FileExists) {
                if ($ScriptText.Count -eq 0 -or ($ScriptText | Out-String).Trim().Length -eq 0) {
                    $ScriptText = @();
                    Write-Warning -Message "An empty profile script already exists at $($profile.CurrentUserAllHosts).";
                } else {
                    $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('Prepend', 'Insert changes to beginning of existing profile script'));
                    $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('Append', 'Add changes to end of existing profile script'));
                    Write-Warning -Message "Profile script already exists";
                    $InformationPreference = [System.Management.Automation.ActionPreference]::Continue;
                    Write-Information -MessageData @(
                        '------- BEGIN Current Profile Script -------',
                        $ScriptText,
                        '------- END Current Profile Script -------'
                    );
                    $InformationPreference = [System.Management.Automation.ActionPreference]::Ignore;
                    Write-Warning -Message "A profile script already exists at $($profile.CurrentUserAllHosts).";
                }
                $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('Overwrite', 'Replace existing profile script'));
                $SelectedIndex = $CurrentHost.UI.PromptForChoice('Modify Existing Profile Script', "A profile script already exists at $($profile.CurrentUserAllHosts).`r`n`r`nWould you like it to be modified?", $ChoiceCollection, 0);
            } else {
                $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('Create', 'Create new profile script'));
                $SelectedIndex = $CurrentHost.UI.PromptForChoice("Create Profile Script", "Create new profile script at $($profile.CurrentUserAllHosts)?", $ChoiceCollection, 0);
            }
            if ($SelectedIndex -gt 0 -and $SelectedIndex -lt $ChoiceCollection.Count) { $SelectedLabel = $ChoiceCollection[$SelectedIndex].Label }
        }
        $SetWideBufferCode = @();
        
        if ($ScriptText.Count -gt 0 -and $SelectedLabel -ne 'Overwrite') {
            $SetWideBufferCode = @(
                '# Modify buffer size if environment variable was set by VS Code Build Task',
                'if ($Env:VSCodeNeedsWideBuffer -eq "true") {',
                '    # Execute in script block so it won''t conflict with other possible script variables',
                '    &{',
                '        $HostRawUI = (Get-Host).UI.RawUI;',
                '        if ($HostRawUI.BufferSize -eq $null) {',
                '            $HostRawUI.BufferSize = New-Object -TypeName ''System.Management.Automation.Host.Size'' -ArgumentList ' + $MinWidth + ', ' + $MinHeight + ';',
                '        } else {',
                '            if ($UiBufferSize.Width -lt ' + $MinWidth + ') { $UiBufferSize.Width = ' + $MinWidth + ' }',
                '            if ($UiBufferSize.Height -lt ' + $MinHeight + ') { $UiBufferSize.Height = ' + $MinHeight + ' }',
                '        }',
                '        if ($Host.UI.RawUI.MaxWindowSize -ne $null) {',
                '            if ($Host.UI.RawUI.MaxWindowSize.Width -lt $UiBufferSize.Width) {',
                '                $Host.UI.RawUI.MaxWindowSize.Width = $UiBufferSize.Width;',
                '            }',
                '            if ($Host.UI.RawUI.MaxWindowSize.Height -lt $UiBufferSize.Height) {',
                '                $Host.UI.RawUI.MaxWindowSize.Height = $UiBufferSize.Height;',
                '            }',
                '        }',
                '    };',
                '}'
            );
        } else {
            if ($SelectedLabel -eq 'Overwrite') { $SelectedLabel = 'Create' }
            $SetWideBufferCode = @(
                '# Modify buffer size if environment variable was set by VS Code Build Task',
                'if ($Env:VSCodeNeedsWideBuffer -eq "true") {',
                '    $HostRawUI = (Get-Host).UI.RawUI;',
                '    if ($HostRawUI.BufferSize -eq $null) {',
                '        $HostRawUI.BufferSize = New-Object -TypeName ''System.Management.Automation.Host.Size'' -ArgumentList ' + $MinWidth + ', ' + $MinHeight + ';',
                '    } else {',
                '        if ($UiBufferSize.Width -lt ' + $MinWidth + ') { $UiBufferSize.Width = ' + $MinWidth + ' }',
                '        if ($UiBufferSize.Height -lt ' + $MinHeight + ') { $UiBufferSize.Height = ' + $MinHeight + ' }',
                '    }',
                '    if ($Host.UI.RawUI.MaxWindowSize -ne $null) {',
                '        if ($Host.UI.RawUI.MaxWindowSize.Width -lt $UiBufferSize.Width) {',
                '            $Host.UI.RawUI.MaxWindowSize.Width = $UiBufferSize.Width;',
                '        }',
                '        if ($Host.UI.RawUI.MaxWindowSize.Height -lt $UiBufferSize.Height) {',
                '            $Host.UI.RawUI.MaxWindowSize.Height = $UiBufferSize.Height;',
                '        }',
                '    }',
                '}'
            );
        }
        switch ($SelectedLabel) {
            'Create' {
                $ScriptText = $SetWideBufferCode;
                break;
            }
            'Prepend' {
                if ($ScriptText[0] -eq $null -or $ScriptText[0].Trim().Length -eq 0) {
                    $ScriptText = $SetWideBufferCode + $ScriptText;
                } else {
                    $ScriptText = $SetWideBufferCode + @('') + $ScriptText;
                }
                break;
            }
            'Append' {
                if ($ScriptText[$ScriptText.Length - 1] -eq $null -or $ScriptText[$ScriptText.Length - 1].Trim().Length -eq 0) {
                    $ScriptText = $ScriptText + $SetWideBufferCode;
                } else {
                    $ScriptText = $ScriptText + @('') + $SetWideBufferCode;
                }
                break;
            }
            default {
                $Abort = $true;
                Write-Warning -Message "Aborted. No changes were made.";
                break;
            }
        }
        if ($Abort) {
            $CurrentHost.SetShouldExit(3);
        } else {
            $InformationPreference = [System.Management.Automation.ActionPreference]::Continue;
            Write-Information -MessageData @(
                '------- BEGIN Proposed Profile Script -------',
                $ScriptText,
                '------- END Proposed Profile Script -------'
            );
            $InformationPreference = [System.Management.Automation.ActionPreference]::Ignore;
            $SelectedIndex = 1;
            if ($IsNonInteractive) {
                $SelectedIndex = 0;
            } else {
                $ChoiceCollection.Clear();
                $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('Yes', 'Write new profile script.'));
                $ChoiceCollection.Add([System.Management.Automation.Host.ChoiceDescription]::new('No', 'Cancel'));
                $SelectedIndex = $CurrentHost.UI.PromptForChoice('Write Profile Script', "Write proposed profile script to $($profile.CurrentUserAllHosts)?", $ChoiceCollection, 0);
            }
            if ($SelectedIndex -eq 0) {
                try {
                    Set-Content -Path $profile.CurrentUserAllHosts -Value $ScriptText -Force;
                    $InformationPreference = [System.Management.Automation.ActionPreference]::Continue;
                    Write-Information -MessageData "$($ScriptText.Count) lines written to $($profile.CurrentUserAllHosts).";
                    $InformationPreference = [System.Management.Automation.ActionPreference]::Ignore;
                } catch {
                    $Abort = $true;
                    throw;
                } finally {
                    if ($Abort) { Write-Warning -Message "Unexpected failure writing to $($profile.CurrentUserAllHosts). Changes may not have been saved." }
                }
            } else {
                Write-Warning -Message "Aborted. No changes were made.";
                $CurrentHost.SetShouldExit(3);
            }
        }
    }
} finally {
    $InformationPreference = $old_InformationPreference;
    $WarningPreference = $old_WarningPreference;
    $ErrorActionPreference = $old_ErrorActionPreference;
}
