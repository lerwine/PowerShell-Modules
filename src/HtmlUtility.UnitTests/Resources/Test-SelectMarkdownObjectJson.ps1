class ParameterMetaData {
    [bool]$NoRecursionOnMatch;
    [bool]$IsRecursive;
    [long]$EffectiveMinDepth;
    [AllowNull()]
    [Nullable[long]]$EffectiveMaxDepth;
    [long]$MatchableTypeCount;
    [bool]$TypeContainsHtmlAttributes;
    [bool]$TypeContainsAny;
}

class CommonParameterValues {
    [ParameterMetaData]$MetaData;
    [AllowNull()]
    [string]$Type;
    [string]$ProcessRecordMethod;
}

class DepthRangeParameterValues : CommonParameterValues {
    [bool]$IncludeAttributes;
    [Nullable[long]]$MinDepth;
    [Nullable[long]]$MaxDepth;
}

class ExplicitDepthParameterValues : CommonParameterValues {
    [bool]$IncludeAttributes;
    [Nullable[long]]$Depth;
}

class RecurseParameterValues : CommonParameterValues {
    [bool]$IncludeAttributes;
}

class RecurseUnmatchedParameterValues : CommonParameterValues {
}

class ParameterSetValues {
    [DepthRangeParameterValues[]]$DepthRange;
    [ExplicitDepthParameterValues[]]$ExplicitDepth;
    [RecurseParameterValues[]]$Recurse;
    [RecurseUnmatchedParameterValues[]]$RecurseUnmatched;
}

class ProcessRecordMethod {
    [string]$Name;
    [string]$PrimaryParameterSet;
    [string]$Description;
}

class DeserializedValues {
    [ParameterSetValues]$ParameterSets;
    [ProcessRecordMethod[]]$ProcessRecordMethods;
}

Function Assert-ValueParameterValues {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CommonParameterValues]$InputObject,

        [Parameter(Mandatory = $true)]
        [ProcessRecordMethod[]]$ProcessRecordMethods
    )

    Begin {
        $Position = -1;
        $IsValid = $true;
    }

    Process {
        $Position++;
        if ($InputObject.MetaData.MatchableTypeCount -lt 0) {
            Write-Error -Message "'MatchableTypeCount' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        } else {
            if ($InputObject.MetaData.MatchableTypeCount -gt 2) {
                Write-Error -Message "'MatchableTypeCount' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                $IsValid = $false;
            } else {
                if ($InputObject -is [RecurseUnmatchedParameterValues] -and $InputObject.MetaData.MatchableTypeCount -eq 0 -and -not ($InputObject.MetaData.TypeContainsAny -or $InputObject.MetaData.TypeContainsHtmlAttributes)) {
                    Write-Error -Message "'MatchableTypeCount' is 0 while neither 'TypeContainsAny' nor 'TypeContainsHtmlAttributes' are true on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                    $IsValid = $false;
                }
            }
        }

        $Expected = @{};

        if ($InputObject -is [ExplicitDepthParameterValues]) {
            $Expected['NoRecursionOnMatch'] = $true;
            $Expected['IsRecursive'] = $false;
            if ($InputObject.Depth -lt 0) {
                Write-Error -Message "'Depth' is a negative value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                $IsValid = $false;
            } else {
                if ($InputObject.Depth -gt 2) {
                    Write-Error -Message "'Depth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                    $IsValid = $false;
                } else {
                    $Expected['EffectiveMinDepth'] = $InputObject.Depth;
                    $Expected['EffectiveMaxDepth'] = $InputObject.Depth;
                }
            }
        } else {
            if ($InputObject -is [RecurseParameterValues]) {
                $Expected['EffectiveMinDepth'] = 1;
                $Expected['EffectiveMaxDepth'] = $null;
                $Expected['NoRecursionOnMatch'] = $false;
                $Expected['IsRecursive'] = $true;
            } else {
                if ($InputObject -is [RecurseUnmatchedParameterValues]) {
                    if ($null -eq $InputObject.MinDepth) {
                        $Expected['EffectiveMinDepth'] = 1;
                    } else {
                        if ($InputObject.MinDepth -lt 0) {
                            Write-Error -Message "'MinDepth' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                        } else {
                            if ($InputObject.MinDepth -gt 2) {
                                Write-Error -Message "'MinDepth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                            } else {
                                $Expected['EffectiveMinDepth'] = $InputObject.MinDepth;
                            }
                        }
                    }
                    if ($null -eq $InputObject.MaxDepth) {
                        if ($Expected.ContainsKey('EffectiveMinDepth')) {
                            if ($null -eq $InputObject.MinDepth) {
                                $Expected['EffectiveMaxDepth'] = $null;
                                $Expected['IsRecursive'] = $true;
                                if ($InputObject.MetaData.MatchableTypeCount -ge 0 -and $InputObject.MetaData.MatchableTypeCount -le 2) {
                                    $Expected['NoRecursionOnMatch'] = $InputObject.MetaData.TypeContainsAny -or $InputObject.MetaData.MatchableTypeCount -gt 0
                                }
                            } else {
                                $Expected['EffectiveMaxDepth'] = $null;
                                $Expected['IsRecursive'] = $true;
                                if ($InputObject.MatchableTypeCount -ge 0 -and $InputObject.MatchableTypeCount -le 2) {
                                    $Expected['NoRecursionOnMatch'] = $InputObject.MetaData.TypeContainsAny -or $InputObject.MetaData.MatchableTypeCount -gt 0
                                }
                            }
                        }
                    } else {
                        if ($InputObject.MaxDepth -lt 0) {
                            Write-Error -Message "'MaxDepth' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                        } else {
                            if ($Expected.ContainsKey('EffectiveMinDepth')) {
                                if ($null -eq $InputObject.MinDepth) {
                                    if ($InputObject.MaxDepth -gt 2) {
                                        Write-Error -Message "'MaxDepth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                        $Expected.Remove('EffectiveMinDepth') | Out-Null;
                                        $IsValid = $false;
                                    } else {
                                        $Expected['EffectiveMinDepth'] = ($InputObject.MaxDepth -gt 0) ? $(1) : $(0);
                                        $Expected['EffectiveMaxDepth'] = $InputObject.MaxDepth;
                                        if ($InputObject.MaxDepth -lt 2) {
                                            $Expected['IsRecursive'] = $false;
                                            $Expected['NoRecursionOnMatch'] = $true;
                                        } else {
                                            $Expected['IsRecursive'] = $true;
                                            if ($InputObject.MetaData.MatchableTypeCount -ge 0 -and $InputObject.MetaData.MatchableTypeCount -le 2) {
                                                $Expected['NoRecursionOnMatch'] = $InputObject.MetaData.TypeContainsAny -or $InputObject.MetaData.MatchableTypeCount -gt 0
                                            }
                                        }
                                    }
                                } else {
                                    if ($InputObject.MaxDepth -lt $InputObject.MinDepth) {
                                        Write-Error -Message "'MaxDepth' is less than 'MinDepth' on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                        $Expected.Remove('EffectiveMinDepth') | Out-Null;
                                        $IsValid = $false;
                                    } else {
                                        $Expected['EffectiveMaxDepth'] = $InputObject.MaxDepth;
                                        if ($InputObject.MaxDepth -eq $InputObject.MinDepth) {
                                            $Expected['IsRecursive'] = $false;
                                            $Expected['NoRecursionOnMatch'] = $true;
                                        } else {
                                            $Expected['IsRecursive'] = $InputObject.MinDepth -gt 0 -or $InputObject.MaxDepth -gt 1;
                                            if ($InputObject.MetaData.MatchableTypeCount -ge 0 -and $InputObject.MetaData.MatchableTypeCount -le 2) {
                                                $Expected['NoRecursionOnMatch'] = $InputObject.MetaData.TypeContainsAny -or $InputObject.MetaData.MatchableTypeCount -gt 0
                                            }
                                        }
                                    }
                                }
                            } else {
                                if ($InputObject.MaxDepth -gt 3) {
                                    Write-Error -Message "'MaxDepth' is greater than 3 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                }
                            }
                        }
                    }
                } else {
                    if ($null -eq $InputObject.MinDepth) {
                        $Expected['EffectiveMinDepth'] = 1;
                    } else {
                        if ($InputObject.MinDepth -lt 0) {
                            Write-Error -Message "'MinDepth' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                        } else {
                            if ($InputObject.MinDepth -gt 2) {
                                Write-Error -Message "'MinDepth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                            } else {
                                $Expected['EffectiveMinDepth'] = $InputObject.MinDepth;
                            }
                        }
                    }
                    if ($null -eq $InputObject.MaxDepth) {
                        if ($Expected.ContainsKey('EffectiveMinDepth')) {
                            if ($null -eq $InputObject.MinDepth) {
                                $Expected['EffectiveMaxDepth'] = 1;
                                $Expected['IsRecursive'] = $false;
                                $Expected['NoRecursionOnMatch'] = $true;
                            } else {
                                $Expected['EffectiveMaxDepth'] = $null;
                                $Expected['IsRecursive'] = $true;
                                $Expected['NoRecursionOnMatch'] = $false;
                            }
                        }
                    } else {
                        if ($InputObject.MaxDepth -lt 0) {
                            Write-Error -Message "'MaxDepth' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                        } else {
                            if ($Expected.ContainsKey('EffectiveMinDepth')) {
                                if ($null -eq $InputObject.MinDepth) {
                                    if ($InputObject.MaxDepth -gt 2) {
                                        Write-Error -Message "'MaxDepth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                        $Expected.Remove('EffectiveMinDepth') | Out-Null;
                                        $IsValid = $false;
                                    } else {
                                        $Expected['EffectiveMinDepth'] = ($InputObject.MaxDepth -gt 0) ? $(1) : $(0);
                                        $Expected['EffectiveMaxDepth'] = $InputObject.MaxDepth;
                                        if ($InputObject.MaxDepth -lt 2) {
                                            $Expected['IsRecursive'] = $false;
                                            $Expected['NoRecursionOnMatch'] = $true;
                                        } else {
                                            $Expected['IsRecursive'] = $true;
                                            $Expected['NoRecursionOnMatch'] = $false;
                                        }
                                    }
                                } else {
                                    if ($InputObject.MaxDepth -lt $InputObject.MinDepth) {
                                        Write-Error -Message "'MaxDepth' is less than 'MinDepth' on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                        $Expected.Remove('EffectiveMinDepth') | Out-Null;
                                        $IsValid = $false;
                                    } else {
                                        $Expected['EffectiveMaxDepth'] = $InputObject.MaxDepth;
                                        if ($InputObject.MaxDepth -eq $InputObject.MinDepth) {
                                            $Expected['IsRecursive'] = $false;
                                            $Expected['NoRecursionOnMatch'] = $true;
                                        } else {
                                            $Expected['IsRecursive'] = $InputObject.MinDepth -gt 0 -or $InputObject.MaxDepth -gt 1;
                                            $Expected['NoRecursionOnMatch'] = $false;
                                        }
                                    }
                                }
                            } else {
                                if ($InputObject.MaxDepth -gt 3) {
                                    Write-Error -Message "'MaxDepth' is greater than 3 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                }
                            }
                        }
                    }
                }
            }
        }
        if ($Expected.ContainsKey('EffectiveMinDepth') -and $Expected['EffectiveMinDepth'] -ne $InputObject.EffectiveMinDepth) {
            Write-Error -Message "'EffectiveMinDepth' was not $($Expected['EffectiveMinDepth']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($Expected.ContainsKey('EffectiveMaxDepth') -and $Expected['EffectiveMaxDepth'] -ne $InputObject.EffectiveMaxDepth) {
            Write-Error -Message "'EffectiveMaxDepth' was not $($Expected['EffectiveMaxDepth']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($Expected.ContainsKey('IsRecursive') -and $Expected['IsRecursive'] -ne $InputObject.IsRecursive) {
            Write-Error -Message "'IsRecursive' was not $($Expected['IsRecursive']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($Expected.ContainsKey('NoRecursionOnMatch') -and $Expected['NoRecursionOnMatch'] -ne $InputObject.NoRecursionOnMatch) {
            Write-Error -Message "'NoRecursionOnMatch' was not $($Expected['NoRecursionOnMatch']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'Type') {
            $ExpectedType = $null;
            switch ($InputObject.MatchableTypeCount) {
                0 {
                    if ($InputObject.TypeContainsAny) {
                        if ($InputObject.TypeContainsHtmlAttributes) {
                            $ExpectedType = "'HtmlAttributes', 'Any'";
                        } else {
                            $ExpectedType = "'Any'";
                        }
                    } else {
                        if ($InputObject.TypeContainsHtmlAttributes) {
                            $ExpectedType = "'HtmlAttributes'";
                        } else {
                            $ExpectedType = '';
                        }
                    }
                    break;
                }
                1 {
                    if ($InputObject.TypeContainsAny) {
                        if ($InputObject.TypeContainsHtmlAttributes) {
                            $ExpectedType = "'Block', 'HtmlAttributes', 'Any'";
                        } else {
                            $ExpectedType = "'Block', 'Any'";
                        }
                    } else {
                        if ($InputObject.TypeContainsHtmlAttributes) {
                            $ExpectedType = "'Block', 'HtmlAttributes'";
                        } else {
                            $ExpectedType = "'Block'";
                        }
                    }
                    break;
                }
                2 {
                    if ($InputObject.TypeContainsAny) {
                        if ($InputObject.TypeContainsHtmlAttributes) {
                            $ExpectedType = "'Block', 'Inline', 'HtmlAttributes', 'Any'";
                        } else {
                            $ExpectedType = "'Block', 'Inline', 'Any'";
                        }
                    } else {
                        if ($InputObject.TypeContainsHtmlAttributes) {
                            $ExpectedType = "'Block', 'Inline', 'HtmlAttributes'";
                        } else {
                            $ExpectedType = "'Block', 'Inline'";
                        }
                    }
                    break;
                }
            }
            if ([string]::IsNullOrEmpty($InputObject.Type)) {
                if ($null -ne $ExpectedType -and $ExpectedType -ne '') {
                    Write-Error -Message "Expected $($ExpectedType | ConvertTo-Json -EscapeHandling) for 'Type'; actual is null on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                    $IsValid = $false;
                }
            } else {
                if ($null -ne $ExpectedType) {
                    if ($ExpectedType -eq '') {
                        Write-Error -Message "Expected null for 'Type'; actual is $($InputObject.Type | ConvertTo-Json) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                        $IsValid = $false;
                    } else {
                        if ($ExpectedType -cne $InputObject.Type) {
                            Write-Error -Message "Expected $($ExpectedType | ConvertTo-Json -EscapeHandling) for 'Type'; actual is $($InputObject.Type | ConvertTo-Json) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    }
                }
            }
        }

        if ($RecurseUnmatched.IsPresent) {
            if ($InputObject.WriteHtmlAttributes -ne $InputObject.TypeContainsHtmlAttributes) {
                Write-Error -Message "'WriteHtmlAttributes' is equal to 'TypeContainsHtmlAttributes' on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                $IsValid = $false;
            }
        } else {
            if ($InputObject.WriteHtmlAttributes -ne ($InputObject.TypeContainsHtmlAttributes -or $InputObject.IncludeAttributes)) {
                if ($InputObject.IncludeAttributes) {
                    Write-Error -Message "'WriteHtmlAttributes' is not true when 'IncludeAttributes' is true on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                    $IsValid = $false;
                } else {
                    if ($InputObject.TypeContainsHtmlAttributes) {
                        Write-Error -Message "'WriteHtmlAttributes' is not true when 'TypeContainsHtmlAttributes' is true on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                        $IsValid = $false;
                    } else {
                        Write-Error -Message "'WriteHtmlAttributes' is not false when both 'IncludeAttributes' and 'TypeContainsHtmlAttributes' are false on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                        $IsValid = $false;
                    }
                }
            }
        }
        
        if ($MemberNames -contains 'ProcessRecordMethod') {
            if ($null -eq $InputObject.ProcessRecordMethod) {
                Write-Error -Message "'ProcessRecordMethod' is null on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                $IsValid = $false;
            } else {
                [string[]]$MemberNames = (($InputObject.ProcessRecordMethod | Get-Member) | ForEach-Object { $_.Name });
                if ($MemberNames -contains 'Name') {
                    if ($InputObject.ProcessRecordMethod.Name -is [string]) {
                        $NoMatch = $true;
                        foreach ($m in $ProcessRecordMethods) {
                            if ($m.Name -ceq $InputObject.ProcessRecordMethod.Name) {
                                $NoMatch = $false;
                                break;
                            }
                        }
                        if ($NoMatch) {
                            Write-Error -Message "ProcessRecordMethod matching 'ProcessRecordMethod.Name' not found on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    } else {
                        Write-Error -Message "'ProcessRecordMethod.Name' is not a string value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                        $IsValid = $false;
                    }
                } else {
                    Write-Error -Message "Property 'ProcessRecordMethod.Name' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                }
                if ($MemberNames -contains 'IsPrimary') {
                    if ($InputObject.ProcessRecordMethod.IsPrimary -isnot [bool]) {
                        Write-Error -Message "'ProcessRecordMethod.IsPrimary' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                        $IsValid = $false;
                    }
                } else {
                    Write-Error -Message "Property 'ProcessRecordMethod.IsPrimary' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                    $IsValid = $false;
                }
            }
        }
    }

    End { $IsValid | Write-Output }
}

Function Test-ValueParameterCoverage {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [DepthRangeParameterValues[]]$DepthRange,
        [Parameter(Mandatory = $true)]
        [ExplicitDepthParameterValues[]]$ExplicitDepth,
        [Parameter(Mandatory = $true)]
        [RecurseParameterValues[]]$Recurse,
        [Parameter(Mandatory = $true)]
        [RecurseUnmatchedParameterValues[]]$RecurseUnmatched,
        [Parameter(Mandatory = $true)]
        [ProcessRecordMethod[]]$ProcessRecordMethods
    )

    $IsValid = $true;
    for ($i = 0; $i -lt $ProcessRecordMethods.Length; $i++) {
        $Obj = $ProcessRecordMethods[$i];
        for ($n = 0; $n -lt $i; $n++) {
            if ($ProcessRecordMethods[$n].Name -eq $Obj.Name) {
                Write-Error -Message "ProcessRecordMethods Property 'Name' at position $i is a duplicate of the name at position $n" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $Obj;
                $IsValid = $false;
                break;
            }
        }
    }
    if (-not ($DepthRange | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -DepthRange)) { $IsValid = $false }
    if (-not ($ExplicitDepth | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -ExplicitDepth)) { $IsValid = $false }
    if (-not ($Recurse | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -Recurse)) { $IsValid = $false }
    if (-not ($RecurseUnmatched | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -RecurseUnmatched)) { $IsValid = $false }
    if ($IsValid) {
        foreach ($n in @($ProcessRecordMethods | ForEach-Object { $_.Name })) {
            $UsedBy = @($MethodReferences | Where-Object { $_.Name -ceq $n });
            switch ($UsedBy.Count) {
                0 {
                    Write-Warning -Message "ProcessRecordMethod $($n | ConvertTo-Json) is not referenced by any parameter sets";
                    break;
                }
                1 {
                    Write-Information -MessageData "ProcessRecordMethod $($n | ConvertTo-Json) is used only once";
                    break;
                }
            }
        }
    }
    $IsValid | Write-Output;
}

[DeserializedValues]$TestData = (Get-Content -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Select-MarkdownObject.json')) | ConvertFrom-Json -Depth 5;

if (Test-ValueParameterCoverage -DepthRange $TestData.ParameterSets.DepthRange -ExplicitDepth $TestData.ParameterSets.ExplicitDepth -Recurse $TestData.ParameterSets.Recurse -RecurseUnmatched $TestData.ParameterSets.RecurseUnmatched -ProcessRecordMethods $TestData.ProcessRecordMethods -ErrorAction Stop) {
    Write-Information -MessageData "JSON data is valid" -InformationAction Continue;
}