if ((Get-PSSnapin -Name 'Microsoft.SharePoint.PowerShell' -ErrorAction Ignore) -eq $null) {
    Add-PSSnapin -Name 'Microsoft.SharePoint.PowerShell';
}

Function New-ChoiceDescription {
    [CmdletBinding()]
    [OutputType([System.Management.Automation.Host.ChoiceDescription])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Label,
        
        [Parameter(Mandatory = $false, Position = 1)]
        [AllowEmptyString()]
        [string]$HelpMessage
    )

    if ($PSBoundParameters.ContainsKey('')) {
        New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $Label, $HelpMessage | Write-Output;
    } else {
        New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $Label | Write-Output;
    }
}

Function Read-ChoiceSelection {
    [CmdletBinding(DefaultParameterSetName = 'Single')]
    [OutputType([System.Management.Automation.Host.ChoiceDescription])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Caption,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        [string]$Message,
        
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true)]
        [Alias('ChoiceDescription', 'ChoiceDescription', 'Choice')]
        [System.Management.Automation.Host.ChoiceDescription[]]$Choices,
        
        [Parameter(Mandatory = $false, Position = 3, ParameterSetName = 'Single')]
        [Alias('Default')]
        [int]$DefaultIndex = -1,
        
        [Parameter(Mandatory = $false, Position = 3, ParameterSetName = 'Multiple')]
        [Alias('Defaults')]
        [int[]]$DefaultIndexes,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Single')]
        [switch]$Single,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Multiple')]
        [switch]$Multiple,
        
        [Parameter(Mandatory = $false)]
        [switch]$AsIndex
    )

    Begin { $ChoiceDescriptionCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]' }

    Process {
        $Choices | ForEach-Object { $ChoiceDescriptionCollection.Add($_) }
    }

    End {
        $IndexCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Int32]';
        if ($PSCmdlet.ParameterSetName -eq 'Single') {
            if ($DefaultIndex -lt -1 -or $DefaultIndex -ge $ChoiceDescriptionCollection.Count) {
                $Index = $Host.UI.PromptForChoice($Caption, $Message, $ChoiceDescriptionCollection, -1);
            } else {
                $Index = $Host.UI.PromptForChoice($Caption, $Message, $ChoiceDescriptionCollection, $DefaultIndex);
            }

            if ($Index -ne $null -and $Index -ge 0 -and $Index -lt $ChoiceDescriptionCollection.Count) {
                $IndexCollection.Add($Index);
            }
        } else {
            if ($PSBoundParameters.ContainsKey('DefaultIndexes')) { $DefaultIndexes | ForEach-Object { $IndexCollection.Add($_) } }
            $IndexCollection = $Host.UI.PromptForChoice($Caption, $Message, $ChoiceDescriptionCollection, $IndexCollection);
        }
        if ($IndexCollection -ne $null -and $IndexCollection.Count -gt 0) {
            if ($AsIndex) {
                $IndexCollection | Write-Output;
            } else {
                $IndexCollection | ForEach-Object { $ChoiceDescriptionCollection[$_] | Write-Output };
            }
        }
    }

    <#
System.Collections.Generic.Dictionary`2[[System.String],[System.Management.Automation.PSObject]] Prompt (System.String caption, System.String message, System.Collections.ObjectModel.Collection`1[System.Management.Automation.Host.FieldDescription] descriptions)

System.Int32 PromptForChoice (System.String caption, System.String message, 
    System.Collections.ObjectModel.Collection`1[System.Management.Automation.Host.ChoiceDescription] choices, Int32 defaultChoice)

System.Collections.ObjectModel.Collection`1[System.Int32] PromptForChoice(System.String caption, System.String message,
    System.Collections.ObjectModel.Collection`1[System.Management.Automation.Host.ChoiceDescription] choices,
    System.Collections.Generic.IEnumerable`1[System.Int32] defaultChoices)
    #>
}

Function New-FieldDescription {
    [CmdletBinding(DefaultParameterSetName = 'DefaultValue')]
    [OutputType([System.Management.Automation.Host.FieldDescription])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Name,
        
        [Parameter(Mandatory = $false, Position = 1)]
        [string]$Label,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Type')]
        [Alias('ParameterType')]
        [System.Type]$Type,
        
        [Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'DefaultValue')]
        [System.Management.Automation.PSObject]$DefaultValue,
        
        [Parameter(Mandatory = $false, Position = 3)]
        [string]$HelpMessage,
        
        [Parameter(Mandatory = $false)]
        [Alias('IsMandatory')]
        [switch]$Mandatory
    )

    $FieldDescription = New-Object -TypeName 'System.Management.Automation.Host.FieldDescription' -ArgumentList $Name;
    $FieldDescription.IsMandatory = $Mandatory;
    if ($PSBoundParameters.ContainsKey('Label')) { $FieldDescription.Label = $Label }
    if ($PSBoundParameters.ContainsKey('DefaultValue')) { $FieldDescription.DefaultValue = $Label }
    if ($PSBoundParameters.ContainsKey('Type')) {
        $FieldDescription.SetParameterType($Type);
    } else {
        if ($PSBoundParameters.ContainsKey('DefaultValue')) {
            $FieldDescription.SetParameterType($DefaultValue.BaseObject.GetType());
            $FieldDescription.DefaultValue = $DefaultValue;
        }
    }

    $FieldDescription | Write-Output;
}

Function Read-Prompt {
    [CmdletBinding()]
    [OutputType([System.Collections.Generic.Dictionary[[System.String],[System.Management.Automation.PSObject]]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Caption,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        [string]$Message,
        
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true)]
        [Alias('FieldDescription', 'FieldDescriptions', 'Fields')]
        [System.Management.Automation.Host.FieldDescription[]]$Field
    )

    Begin { $FieldDescriptionCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.FieldDescription]' }

    Process { $Field | ForEach-Object { $FieldDescriptionCollection.Add($_) } }

    End { $Host.UI.Prompt($Caption, $Message, $FieldDescriptionCollection) }
}

Function Read-ServiceInstanceSelection {
    [CmdletBinding()]
    [OutputType([Microsoft.SharePoint.Administration.SPServiceInstance])]
    Param()

    $SPServiceInstanceArray = Get-SPServiceInstance -All;
    $SelectedObject = $SPServiceInstanceArray | Select-Object -Property 'DisplayName', 'Name', 'TypeName', 'Status', 'Instance', 'Hidden', 'SystemService', 'Description', 'Id' | Out-GridView -Title 'Select Service Instance' -OutputMode Single;
    if ($SelectedObject -ne $null) {
        $SPServiceInstanceArray | Where-Object { $_.Id -eq $SelectedObject.Id }
    }
}

Function Invoke-ServiceInstanceOption {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [Alias('Instance', 'SPServiceInstance')]
        [Microsoft.SharePoint.Administration.SPServiceInstance]$ServiceInstance
    )

    switch ($ServiceInstance.Status) {
        Disabled {
            $Choices = @((New-ChoiceDescription -Label 'Delete', 'Delete Service Instance'));
            break;
        }
        Offline {
            $Choices = @(
                (New-ChoiceDescription -Label 'Start', 'Start Service Instance'),
                (New-ChoiceDescription -Label 'Delete', 'Delete Service Instance')
            );
            break;
        }
        Online {
            $Choices = @((New-ChoiceDescription -Label 'Stop', 'Delete Service Instance'));
            break;
        }
        default {
            $Choices = $null;
            break;
        }
    }

    $ServiceInstance.Delete();
}


while (([Microsoft.SharePoint.Administration.SPServiceInstance]$SPServiceInstance = Read-ServiceInstanceSelection) -ne $null) {
    
}
[Microsoft.SharePoint.DistributedCaching.Utilities.SPDistributedCacheServiceInstance]$SPDistributedCacheServiceInstance = Get-SPServiceInstance -All | Where-Object { $_ -is [Microsoft.SharePoint.DistributedCaching.Utilities.SPDistributedCacheServiceInstance] }
