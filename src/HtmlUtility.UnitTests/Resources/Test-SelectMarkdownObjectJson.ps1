$TestData = (Get-Content -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Select-MarkdownObject.json')) | ConvertFrom-Json -Depth 5;
$InputObjectProcessorsByName = @{};
$InputObjectProcessorUsageCounts = @{};
$InputObjectProcessorPrimary = @{};
$TestData.InputObjectProcessors | ForEach-Object {
    if ($InputObjectProcessorsByName.ContainsKey($_.Name)) {
        Write-Warning -Message "Duplicate InputObjectProcessor name $($_.Name)";
    } else {
        $InputObjectProcessorsByName[$_.Name] = $_;
        $InputObjectProcessorUsageCounts[$_.Name] = 0;
    }
}
foreach ($ParameterSet in $TestData.ParameterSets.DepthRange) {
    $InputObjectProcessor = $ParameterSet.InputObjectProcessor
    if ($null -eq $InputObjectProcessor) {
        Write-Warning -Message "DepthRange parameter value set does not specify an InputObjectProcessor: $($ParameterSet | ConvertTo-Json -Depth 2)";
    } else {
        if ($InputObjectProcessorsByName.ContainsKey($InputObjectProcessor.Name)) {
            $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] = $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] + 1;
            if ($InputObjectProcessor.IsPrimary) {
                if ($InputObjectProcessorPrimary.ContainsKey($InputObjectProcessor.Name)) {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @($InputObjectProcessorPrimary[$InputObjectProcessor.Name]) + @([PSCustomObject]@{
                        Name = 'DepthRange';
                        Parameters = $ParameterSet;
                    });
                } else {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @([PSCustomObject]@{
                        Name = 'DepthRange';
                        Parameters = $ParameterSet;
                    });
                }
            }
        } else {
            Write-Warning -Message "DepthRange parameter value set does not specifies an InputObjectProcessor that was not found: $($ParameterSet | ConvertTo-Json -Depth 2)";
        }
    }
}
foreach ($ParameterSet in $TestData.ParameterSets.ExplicitDepth) {
    $InputObjectProcessor = $ParameterSet.InputObjectProcessor
    if ($null -eq $InputObjectProcessor) {
        Write-Warning -Message "ExplicitDepth parameter value set does not specify an InputObjectProcessor: $($ParameterSet | ConvertTo-Json -Depth 2)";
    } else {
        if ($InputObjectProcessorsByName.ContainsKey($InputObjectProcessor.Name)) {
            $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] = $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] + 1;
            if ($InputObjectProcessor.IsPrimary) {
                if ($InputObjectProcessorPrimary.ContainsKey($InputObjectProcessor.Name)) {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @($InputObjectProcessorPrimary[$InputObjectProcessor.Name]) + @([PSCustomObject]@{
                        Name = 'ExplicitDepth';
                        Parameters = $ParameterSet;
                    });
                } else {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @([PSCustomObject]@{
                        Name = 'ExplicitDepth';
                        Parameters = $ParameterSet;
                    });
                }
            }
        } else {
            Write-Warning -Message "ExplicitDepth parameter value set does not specifies an InputObjectProcessor that was not found: $($ParameterSet | ConvertTo-Json -Depth 2)";
        }
    }
}
foreach ($ParameterSet in $TestData.ParameterSets.Recurse) {
    $InputObjectProcessor = $ParameterSet.InputObjectProcessor
    if ($null -eq $InputObjectProcessor) {
        Write-Warning -Message "Recurse parameter value set does not specify an InputObjectProcessor: $($ParameterSet | ConvertTo-Json -Depth 2)";
    } else {
        if ($InputObjectProcessorsByName.ContainsKey($InputObjectProcessor.Name)) {
            $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] = $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] + 1;
            if ($InputObjectProcessor.IsPrimary) {
                if ($InputObjectProcessorPrimary.ContainsKey($InputObjectProcessor.Name)) {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @($InputObjectProcessorPrimary[$InputObjectProcessor.Name]) + @([PSCustomObject]@{
                        Name = 'Recurse';
                        Parameters = $ParameterSet;
                    });
                } else {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @([PSCustomObject]@{
                        Name = 'Recurse';
                        Parameters = $ParameterSet;
                    });
                }
            }
        } else {
            Write-Warning -Message "Recurse parameter value set does not specifies an InputObjectProcessor that was not found: $($ParameterSet | ConvertTo-Json -Depth 2)";
        }
    }
}
foreach ($ParameterSet in $TestData.ParameterSets.RecurseUnmatched) {
    $InputObjectProcessor = $ParameterSet.InputObjectProcessor
    if ($null -eq $InputObjectProcessor) {
        Write-Warning -Message "RecurseUnmatched parameter value set does not specify an InputObjectProcessor: $($ParameterSet | ConvertTo-Json -Depth 2)";
    } else {
        if ($InputObjectProcessorsByName.ContainsKey($InputObjectProcessor.Name)) {
            $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] = $InputObjectProcessorUsageCounts[$InputObjectProcessor.Name] + 1;
            if ($InputObjectProcessor.IsPrimary) {
                if ($InputObjectProcessorPrimary.ContainsKey($InputObjectProcessor.Name)) {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @($InputObjectProcessorPrimary[$InputObjectProcessor.Name]) + @([PSCustomObject]@{
                        Name = 'RecurseUnmatched';
                        Parameters = $ParameterSet;
                    });
                } else {
                    $InputObjectProcessorPrimary[$InputObjectProcessor.Name] = @([PSCustomObject]@{
                        Name = 'RecurseUnmatched';
                        Parameters = $ParameterSet;
                    });
                }
            }
        } else {
            Write-Warning -Message "RecurseUnmatched parameter value set specifies an InputObjectProcessor that was not found: $($ParameterSet | ConvertTo-Json -Depth 2)";
        }
    }
}
$TestData.InputObjectProcessors | ForEach-Object {
    $n = $_.Name;
    switch ($InputObjectProcessorUsageCounts[$n]) {
        0 {
            Write-Warning -Message "InputObjectProcessor $n is not used";
            break;
        }
        1 {
            Write-Information -MessageData "InputObjectProcessor $n is used only once" -InformationAction Continue;
            break;
        }
        2 { break }
    }
    if ($InputObjectProcessorPrimary.ContainsKey($n)) {
        $PrimaryValueSets = @($InputObjectProcessorPrimary[$n]);
        if ($PrimaryValueSets.Count -gt 1) {
            Write-Warning -Message "Duplicate InputObjectProcessor primary parameter value sets: $($PrimaryValueSets | ConvertTo-Json -Depth 5)";
        }
    } else {
        Write-Warning -Message "InputObjectProcessor $n does not have a primary parameter value set";
    }
}
