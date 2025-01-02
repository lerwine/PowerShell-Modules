
class ParameterDetail {
    [string]$Name;
    [object]$Value;
}
class ParameterSetInfo {
    [string]$ParameterSetName;
    [System.Collections.ObjectModel.Collection[ParameterDetail]]$Parameters;
    [object] GetParameterValue([string]$Name) {
        foreach ($ParameterDetail in $this.Parameters) {
            if ($ParameterDetail.Name -eq $Name) { return $ParameterDetail.Value }
        }
        return $null;
    }
    [bool] HasAnyType() {
        $Value = $this.GetParameterValue('Type');
        return $null -ne $Value -and $Value -Contains 'Any';
    }
    [int] GetSearchableTypeCount() {
        $Value = $this.GetParameterValue('Type');
        if ($null -eq $Value) { return 0 }
        return @($Value | Where-Object { $_ -ne 'Any' -and $_ -ne 'HtmlAttributes' }).Count;
    }
    [int] IncludeAttributes() {
        if ($null -ne $this.GetParameterValue('IncludeAttributes')) { return $true }
        $Value = $this.GetParameterValue('Type');
        return $null -ne $Value -and $Value -contains 'HtmlAttributes';
    }
    [int] GetEffectiveMinDepth() {
        if ($this.ParameterSetName -eq 'ExplicitDepth') {
            $Value = $this.GetParameterValue('Depth');
            if ($null -eq $Value) { return 1 }
            return $Value;
        }
        $Value = $this.GetParameterValue('MinDepth');
        if ($null -ne $Value) { return $Value }
        $Value = $this.GetParameterValue('MaxDepth');
        if ($null -ne $Value -and $Value -eq 0) { return 0 }
        return 1;
    }
    [bool] IsRecursive() {
        if ($this.ParameterSetName -eq 'ExplicitDepth') { return $false }
        $MaxDepth = $this.GetParameterValue('MaxDepth');
        if ($null -ne $MaxDepth) {
            $MinDepth = $this.GetParameterValue('MinDepth');
            if ($null -eq $MinDepth) { return $MaxDepth -gt 1 }
            return $MaxDepth -gt $MinDepth;
        }
        return $this.ParameterSetName -ne 'DepthRange' -or $null -ne $this.GetParameterValue('MinDepth');
    }
    [string] ToString() {
        if ($this.Parameters.Count -eq 0) { return "$($this.ParameterSetName): (no parameters)" }
        return "$($this.ParameterSetName): " + (($this.Parameters | ForEach-Object {
            if ($_.Value -is [System.Management.Automation.SwitchParameter]) {
                "-$($_.Name)"
            } else {
                if ($_.Value -is [Array]) {
                    "-$($_.Name) $($_.Value -join ', ')";
                } else {
                    "-$($_.Name) $($_.Value)";
                }
            }
        }) -join ' ');
    }
}

[System.Collections.ObjectModel.Collection[ParameterSetInfo]]$ParameterSets = [System.Collections.ObjectModel.Collection[ParameterSetInfo]]::new();
@('DepthRange', 'ExplicitDepth', 'Recurse', 'RecurseUnmatched') | ForEach-Object {
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('Block', 'Inline', 'HtmlAttributes') }); };
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('Block', 'HtmlAttributes') }); };
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('Any', 'HtmlAttributes') }); };
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('Block', 'Inline') }); };
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('Block') }); };
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('Any') }); };
    [ParameterSetInfo]@{ ParameterSetName = $_; Parameters = @([ParameterDetail]@{ Name = 'Type'; Value = @('HtmlAttributes') }); };
    if ($_ -ne 'RecurseUnmatched') {
        [ParameterSetInfo]@{
            ParameterSetName = $_;
            Parameters = @();
        };
    }
} | ForEach-Object {
    if ($_.ParameterSetName -eq 'ExplicitDepth') {
        $ParameterSetInfo = [ParameterSetInfo]@{
            ParameterSetName = 'ExplicitDepth';
            Parameters = @($_.Parameters);
        };
        $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'Depth'; Value = 2 });
        $ParameterSetInfo | Write-Output;
        
        $ParameterSetInfo = [ParameterSetInfo]@{
            ParameterSetName = 'ExplicitDepth';
            Parameters = @($_.Parameters);
        };
        $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'Depth'; Value = 1 });
        $ParameterSetInfo | Write-Output;
        $_.Parameters.Add([ParameterDetail]@{ Name = 'Depth'; Value = 0 });
    }
    if ($_.ParameterSetName -eq 'DepthRange' -or $_.ParameterSetName -eq 'RecurseUnmatched') {
        for ($MinDepth = 0; $MinDepth -le 2; $MinDepth++) {
            $ParameterSetInfo = [ParameterSetInfo]@{
                ParameterSetName = $_.ParameterSetName;
                Parameters = @($_.Parameters);
            };
            $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'MinDepth'; Value = $MinDepth });
            $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'MaxDepth'; Value = $MinDepth });
            $ParameterSetInfo | Write-Output;
            
            $ParameterSetInfo = [ParameterSetInfo]@{
                ParameterSetName = $_.ParameterSetName;
                Parameters = @($_.Parameters);
            };
            $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'MinDepth'; Value = $MinDepth });
            $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'MaxDepth'; Value = $MinDepth + 1 });
            $ParameterSetInfo | Write-Output;
            
            $ParameterSetInfo = [ParameterSetInfo]@{
                ParameterSetName = $_.ParameterSetName;
                Parameters = @($_.Parameters);
            };
            $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'MinDepth'; Value = $MinDepth });
            $ParameterSetInfo | Write-Output;
        }
        for ($MaxDepth = 0; $MaxDepth -le 2; $MaxDepth++) {
            $ParameterSetInfo = [ParameterSetInfo]@{
                ParameterSetName = $_.ParameterSetName;
                Parameters = @($_.Parameters);
            };
            $ParameterSetInfo.Parameters.Add([ParameterDetail]@{ Name = 'MaxDepth'; Value = $MaxDepth });
            $ParameterSetInfo | Write-Output;
        }
    }
    $_ | Write-Output;
} | ForEach-Object {
    $Parameters = @($_.Parameters);
    $ParameterDetail = $Parameters | Where-Object { $_.Name -eq 'Type' } | Select-Object -First 1;
    if ($null -ne $ParameterDetail -and $ParameterDetail.Value -contains 'HtmlAttributes') {
        if ($ParameterDetail.Value.Count -eq 1) {
            $Parameters = @($Parameters | Where-Object { $_.Name -ne 'Type' })
        } else {
            $Parameters = @($Parameters | ForEach-Object {
                if ($_.Name -eq 'Type') {
                    [ParameterDetail]@{ Name = 'Type'; Value = @($_.Value | Where-Object { $_ -ne 'HtmlAttributes' }) };
                } else {
                     $_ | Write-Output;
                }
            });
        }
        $Parameters += @([ParameterDetail]@{ Name = 'IncludeAttributes'; Value = [System.Management.Automation.SwitchParameter]::Present })
        [ParameterSetInfo]@{ ParameterSetName = $_.ParameterSetName; Parameters = $Parameters };
    }
    $_ | Write-Output;
} | ForEach-Object {
    if ($_.ParameterSetName -eq 'Recurse') {
        $_.Parameters.Add([ParameterDetail]@{ Name = 'Recurse'; Value = [System.Management.Automation.SwitchParameter]::Present });
    } else {
        if ($_.ParameterSetName -eq 'RecurseUnmatched') {
            $_.Parameters.Add([ParameterDetail]@{ Name = 'RecurseUnmatchedOnly'; Value = [System.Management.Automation.SwitchParameter]::Present });
        }
    }
    $ParameterSets.Add($_);
}
$ParameterSets | Where-Object {
    $_.ParameterSetName -eq 'DepthRange' -and (-not $_.HasAnyType()) -and $_.IsRecursive() -and $_.GetSearchableTypeCount() -eq 1 -and $null -eq $_.GetParameterValue('MaxDepth') -and `
         $_.GetEffectiveMinDepth() -eq 0 -and $_.IncludeAttributes()
    # $_.ParameterSetName -eq 'RecurseUnmatched' -and (-not $_.HasAnyType()) -and $_.IsRecursive() -and $_.GetSearchableTypeCount() -gt 1# -and $_.GetEffectiveMinDepth() -eq 0# -and $_.IncludeAttributes()
    # $_.ParameterSetName -eq 'RecurseUnmatched' -and (-not $_.HasAnyType()) -and $_.IsRecursive()
} | ForEach-Object { "// $_" }
