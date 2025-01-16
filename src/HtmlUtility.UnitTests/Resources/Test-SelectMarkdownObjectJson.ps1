class ParameterMetaData {
    [bool]$NoRecursionOnMatch;
    [bool]$IsRecursive;
    [long]$EffectiveMinDepth;
    [AllowNull()]
    [Nullable[long]]$EffectiveMaxDepth;
    [bool]$WriteHtmlAttributes;
    [long]$MatchableTypeCount;
    [bool]$TypeContainsHtmlAttributes;
    [bool]$TypeContainsAny;
    static [ParameterMetaData] Import([PSCustomObject]$InputObject) {
        return [ParameterMetaData]@{
            NoRecursionOnMatch = $InputObject.NoRecursionOnMatch;
            IsRecursive = $InputObject.IsRecursive;
            EffectiveMinDepth = $InputObject.EffectiveMinDepth;
            EffectiveMaxDepth = $InputObject.EffectiveMaxDepth;
            WriteHtmlAttributes = $InputObject.WriteHtmlAttributes;
            MatchableTypeCount = $InputObject.MatchableTypeCount;
            TypeContainsHtmlAttributes = $InputObject.TypeContainsHtmlAttributes;
            TypeContainsAny = $InputObject.TypeContainsAny;
        }
    }
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
    static [DepthRangeParameterValues] Import([PSCustomObject]$InputObject) {
        return [DepthRangeParameterValues]@{
            MinDepth = $InputObject.MinDepth;
            MaxDepth = $InputObject.MaxDepth;
            IncludeAttributes = $InputObject.IncludeAttributes;
            MetaData = [ParameterMetaData]::Import($InputObject.MetaData);
            Type = $InputObject.Type;
            ProcessRecordMethod = $InputObject.ProcessRecordMethod;
        }
    }
    [string] ToString() {
        if ($null -eq $this.MinDepth) {
            if ($null -eq $this.MaxDepth) {
                if ([string]::IsNullOrEmpty($this.Type)) {
                    if ($this.IncludeAttributes) {
                        return "Select-MarkdownObject -IncludeAttributes";
                    }
                    return "Select-MarkdownObject";
                }
                if ($this.IncludeAttributes) {
                    return "Select-MarkdownObject -Type $($this.Type) -IncludeAttributes";
                }
                return "Select-MarkdownObject -Type $($this.Type)";
            }
            if ([string]::IsNullOrEmpty($this.Type)) {
                if ($this.IncludeAttributes) {
                    return "Select-MarkdownObject -MaxDepth $($this.MaxDepth) -IncludeAttributes";
                }
                return "Select-MarkdownObject -MaxDepth $($this.MaxDepth)";
            }
            if ($this.IncludeAttributes) {
                return "Select-MarkdownObject -MaxDepth $($this.MaxDepth) -Type $($this.Type) -IncludeAttributes";
            }
            return "Select-MarkdownObject -MaxDepth $($this.MaxDepth) -Type $($this.Type)";
        }
        if ($null -eq $this.MaxDepth) {
            if ([string]::IsNullOrEmpty($this.Type)) {
                if ($this.IncludeAttributes) {
                    return "Select-MarkdownObject -MinDepth $($this.MinDepth) -IncludeAttributes";
                }
                return "Select-MarkdownObject -MinDepth $($this.MinDepth)";
            }
            if ($this.IncludeAttributes) {
                return "Select-MarkdownObject -MinDepth $($this.MinDepth) -Type $($this.Type) -IncludeAttributes";
            }
            return "Select-MarkdownObject -MinDepth $($this.MinDepth) -Type $($this.Type)";
        }
        if ([string]::IsNullOrEmpty($this.Type)) {
            if ($this.IncludeAttributes) {
                return "Select-MarkdownObject -MinDepth $($this.MinDepth) -MaxDepth $($this.MaxDepth) -IncludeAttributes";
            }
            return "Select-MarkdownObject -MinDepth $($this.MinDepth) -MaxDepth $($this.MaxDepth)";
        }
        if ($this.IncludeAttributes) {
            return "Select-MarkdownObject -MinDepth $($this.MinDepth) -MaxDepth $($this.MaxDepth) -Type $($this.Type) -IncludeAttributes";
        }
        return "Select-MarkdownObject -MinDepth $($this.MinDepth) -MaxDepth $($this.MaxDepth) -Type $($this.Type)";
    }
}

class ExplicitDepthParameterValues : CommonParameterValues {
    [bool]$IncludeAttributes;
    [Nullable[long]]$Depth;
    static [ExplicitDepthParameterValues] Import([PSCustomObject]$InputObject) {
        return [ExplicitDepthParameterValues]@{
            Depth = $InputObject.Depth;
            IncludeAttributes = $InputObject.IncludeAttributes;
            MetaData = [ParameterMetaData]::Import($InputObject.MetaData);
            Type = $InputObject.Type;
            ProcessRecordMethod = $InputObject.ProcessRecordMethod;
        }
    }
    [string] ToString() {
        if ([string]::IsNullOrEmpty($this.Type)) {
            if ($this.IncludeAttributes) {
                return "Select-MarkdownObject -Depth $($this.Depth) -IncludeAttributes";
            }
            return "Select-MarkdownObject -Depth $($this.Depth)";
        }
        if ($this.IncludeAttributes) {
            return "Select-MarkdownObject -Depth $($this.Depth) -Type $($this.Type) -IncludeAttributes";
        }
        return "Select-MarkdownObject -Depth $($this.Depth) -Type $($this.Type)";
    }
}

class RecurseParameterValues : CommonParameterValues {
    [bool]$IncludeAttributes;
    static [RecurseParameterValues] Import([PSCustomObject]$InputObject) {
        return [RecurseParameterValues]@{
            IncludeAttributes = $InputObject.IncludeAttributes;
            MetaData = [ParameterMetaData]::Import($InputObject.MetaData);
            Type = $InputObject.Type;
            ProcessRecordMethod = $InputObject.ProcessRecordMethod;
        }
    }
    [string] ToString() {
        if ([string]::IsNullOrEmpty($this.Type)) {
            if ($this.IncludeAttributes) {
                return "Select-MarkdownObject -Recurse -IncludeAttributes";
            }
            return "Select-MarkdownObject -Recurse";
        }
        if ($this.IncludeAttributes) {
            return "Select-MarkdownObject -Type $($this.Type) -Recurse -IncludeAttributes";
        }
        return "Select-MarkdownObject -Type $($this.Type) -Recurse";
    }
}

class RecurseUnmatchedParameterValues : CommonParameterValues {
    [Nullable[long]]$MinDepth;
    [Nullable[long]]$MaxDepth;
    static [RecurseUnmatchedParameterValues] Import([PSCustomObject]$InputObject) {
        return [RecurseUnmatchedParameterValues]@{
            MinDepth = $InputObject.MinDepth;
            MaxDepth = $InputObject.MaxDepth;
            MetaData = [ParameterMetaData]::Import($InputObject.MetaData);
            Type = $InputObject.Type;
            ProcessRecordMethod = $InputObject.ProcessRecordMethod;
        }
    }
    [string] ToString() {
        if ($null -eq $this.MinDepth) {
            if ($null -eq $this.MaxDepth) {
                if ([string]::IsNullOrEmpty($this.Type)) {
                    return "Select-MarkdownObject -RecurseUnmatchedOnly";
                }
                return "Select-MarkdownObject -Type $($this.Type) -RecurseUnmatchedOnly";
            }
            if ([string]::IsNullOrEmpty($this.Type)) {
                return "Select-MarkdownObject -MaxDepth $($this.MaxDepth) -RecurseUnmatchedOnly";
            }
            return "Select-MarkdownObject -MaxDepth $($this.MaxDepth) -Type $($this.Type) -RecurseUnmatchedOnly";
        }
        if ($null -eq $this.MaxDepth) {
            if ([string]::IsNullOrEmpty($this.Type)) {
                return "Select-MarkdownObject -MinDepth $($this.MinDepth) -RecurseUnmatchedOnly";
            }
            return "Select-MarkdownObject -MinDepth $($this.MinDepth) -Type $($this.Type) -RecurseUnmatchedOnly";
        }
        if ([string]::IsNullOrEmpty($this.Type)) {
            return "Select-MarkdownObject -MinDepth $($this.MinDepth) -MaxDepth $($this.MaxDepth) -RecurseUnmatchedOnly";
        }
        return "Select-MarkdownObject -MinDepth $($this.MinDepth) -MaxDepth $($this.MaxDepth) -Type $($this.Type) -RecurseUnmatchedOnly";
    }
}

class ParameterSetValues {
    [DepthRangeParameterValues[]]$DepthRange;
    [ExplicitDepthParameterValues[]]$ExplicitDepth;
    [RecurseParameterValues[]]$Recurse;
    [RecurseUnmatchedParameterValues[]]$RecurseUnmatched;
    static [ParameterSetValues] Import([PSCustomObject]$InputObject) {
        return [ParameterSetValues]@{
            DepthRange = ([DepthRangeParameterValues[]]@($InputObject.DepthRange | ForEach-Object { [DepthRangeParameterValues]::Import($_) }));
            ExplicitDepth = ([ExplicitDepthParameterValues[]]@($InputObject.ExplicitDepth | ForEach-Object { [ExplicitDepthParameterValues]::Import($_) }));
            Recurse = ([RecurseParameterValues[]]@($InputObject.Recurse | ForEach-Object { [RecurseParameterValues]::Import($_) }));
            RecurseUnmatched = ([RecurseUnmatchedParameterValues[]]@($InputObject.RecurseUnmatched | ForEach-Object { [RecurseUnmatchedParameterValues]::Import($_) }));
        }
    }
}

class ProcessRecordMethod {
    [string]$Name;
    [string]$PrimaryParameterSet;
    [string]$Description;
    static [ProcessRecordMethod] Import([PSCustomObject]$InputObject) {
        return [ProcessRecordMethod]@{
            Name = $InputObject.Name;
            PrimaryParameterSet = $InputObject.PrimaryParameterSet;
            Description = $InputObject.Description;
        }
    }
}

class DeserializedValues {
    [ParameterSetValues]$ParameterSets;
    [ProcessRecordMethod[]]$ProcessRecordMethods;
    static [DeserializedValues] Import([PSCustomObject]$InputObject) {
        return [DeserializedValues]@{
            ParameterSets = [ParameterSetValues]::Import($InputObject.ParameterSets);
            ProcessRecordMethods = ([ProcessRecordMethod[]]@($InputObject.ProcessRecordMethods | ForEach-Object { [ProcessRecordMethod]::Import($_) }));
        }
    }
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

Function Assert-ValidProcessRecordMethods {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [ProcessRecordMethod]$InputObject
    )

    Begin {
        $MethodIndexes = @{};
        $Index = -1;
        $ParameterSetNames = @('DepthRange', 'ExplicitDepth', 'Recurse', 'RecurseUnmatched');
    }

    Process {
        $Index++;
        if ($null -ne $InputObject.Name -and $InputObject.Name -cmatch '^[A-z][A-Za-z\d]+$') {
            if ($MethodIndexes.ContainsKey($InputObject.Name)) {
                Write-Error -Message "ProcessRecordMethod Name '$($InputObject.Name)' at index $Index is duplicate of ProcessRecordMethod at index $($MethodIndexes[$InputObject.Name])" -Category InvalidData -ErrorId 'Duplicate' -TargetObject $InputObject;
            } else {
                $MethodIndexes[$InputObject.Name] = $Index;
                if ($ParameterSetNames -cnotcontains $InputObject.PrimaryParameterSet) {
                    Write-Error -Message "Invalid ProcessRecordMethod PrimaryParameterSet at index $Index" -Category InvalidData -ErrorId 'InvalidPrimaryParameterSet' -TargetObject $InputObject;
                }
            }
        } else {
            Write-Error -Message "Invalid ProcessRecordMethod Name at index $Index" -Category InvalidData -ErrorId 'InvalidName' -TargetObject $InputObject;
        }
    }
}

Function Assert-ValidParameterValues {
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

    $Index = -1;
    $AllParameterValues = @($DepthRange | ForEach-Object {
        $Index++;
        [PSCustomObject]@{
            ParameterSetName = 'DepthRange';
            CommonParameterValues = $_;
            Index = $Index;
        }
    });
    $Index = -1;
    $AllParameterValues += @($ExplicitDepth | ForEach-Object {
        $Index++;
        [PSCustomObject]@{
            ParameterSetName = 'ExplicitDepth';
            CommonParameterValues = $_;
            Index = $Index;
        }
    });
    $Index = -1;
    $AllParameterValues += @($Recurse | ForEach-Object {
        $Index++;
        [PSCustomObject]@{
            ParameterSetName = 'Recurse';
            CommonParameterValues = $_;
            Index = $Index;
        }
    });
    $Index = -1;
    $AllParameterValues += @($RecurseUnmatched | ForEach-Object {
        $Index++;
        [PSCustomObject]@{
            ParameterSetName = 'RecurseUnmatched';
            CommonParameterValues = $_;
            Index = $Index;
        }
    });
    $AllParameterValues | ForEach-Object {
        $Index++;
        [CommonParameterValues]$CommonParameterValues = $_.CommonParameterValues;
        [ParameterMetaData]$MetaData = $CommonParameterValues.MetaData;
        $n = $CommonParameterValues.ProcessRecordMethod;
        $ParameterSetName = $_.ParameterSetName;
        $Index = $_.Index;
        if ($null -eq ($ProcessRecordMethods | Where-Object { $_.Name -ceq $n } | Select-Object -First 1)) {
            Write-Error -Message "ProcessRecordMethod $n for $ParameterSetName at index $Index not found in ProcessRecordMethods" -Category InvalidData -ErrorId 'UnKnownProcessRecordMethod' -TargetObject $CommonParameterValues;
        } else {
            $ExpectedType = '';
            switch ($MetaData.MatchableTypeCount) {
                0 { break }
                1 {
                    $ExpectedType = "'Block'";
                    break;
                }
                2 {
                    $ExpectedType = "'Block', 'Inline'";
                    break;
                }
                default {
                    $ExpectedType = $null;
                    Write-Error -Message "Invalid MetaData.MatchableTypeCount $_ for $ParameterSetName at index $Index not found in ProcessRecordMethods" -Category InvalidData -ErrorId 'InvalidMatchableTypeCount' -TargetObject $CommonParameterValues;
                    break;
                }
            }
            if ($null -ne $ExpectedType) {
                if ($ExpectedType -eq '') {
                    if ($MetaData.TypeContainsAny) {
                        if ($MetaData.TypeContainsHtmlAttributes) {
                            if ([string]::IsNullOrEmpty($CommonParameterValues.Type) -or $CommonParameterValues.Type -cne "'HtmlAttributes', 'Any'") {
                                Write-Error -Message "Invalid Type for $ParameterSetName at index $Index. Expected: `"'HtmlAttributes', 'Any'`"; Actual: $($CommonParameterValues.Type | ConvertTo-Json); Data: $($CommonParameterValues | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidType' -TargetObject $CommonParameterValues;
                            }
                        } else {
                            if ([string]::IsNullOrEmpty($CommonParameterValues.Type) -or $CommonParameterValues.Type -cne "'Any'") {
                                Write-Error -Message "Invalid Type for $ParameterSetName at index $Index. Expected: `"'Any'`"; Actual: $($CommonParameterValues.Type | ConvertTo-Json); Data: $($CommonParameterValues | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidType' -TargetObject $CommonParameterValues;
                            }
                        }
                    } else {
                        if ($MetaData.TypeContainsHtmlAttributes) {
                            if ([string]::IsNullOrEmpty($CommonParameterValues.Type) -or $CommonParameterValues.Type -cne "'HtmlAttributes'") {
                                Write-Error -Message "Invalid Type for $ParameterSetName at index $Index. Expected: `"'HtmlAttributes', 'Any'`"; Actual: $($CommonParameterValues.Type | ConvertTo-Json); Data: $($CommonParameterValues | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidType' -TargetObject $CommonParameterValues;
                            }
                        } else {
                            if (-not [string]::IsNullOrEmpty($CommonParameterValues.Type)) {
                                Write-Error -Message "Invalid Type for $ParameterSetName at index $Index. Expected: null; Actual: $($CommonParameterValues.Type | ConvertTo-Json); Data: $($CommonParameterValues | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidType' -TargetObject $CommonParameterValues;
                            }
                        }
                    }
                } else {
                    if ($MetaData.TypeContainsHtmlAttributes) { $ExpectedType = "$ExpectedType, 'HtmlAttributes'" }
                    if ($MetaData.TypeContainsAny) { $ExpectedType = "$ExpectedType, 'Any'" }
                    if ([string]::IsNullOrEmpty($CommonParameterValues.Type) -or $CommonParameterValues.Type -cne $ExpectedType) {
                        Write-Error -Message "Invalid Type for $ParameterSetName at index $Index. Expected: $($ExpectedType | ConvertTo-Json); Actual: $($CommonParameterValues.Type | ConvertTo-Json); Data: $($CommonParameterValues | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidType' -TargetObject $CommonParameterValues;
                    }
                }
            }
        }
    }
    $Index = -1;
    $CommandHash = @{};
    $DepthRange | ForEach-Object {
        $Index++;
        $Expected = @{};
        if ($null -eq $_.MinDepth) {
            if ($null -eq $_.MaxDepth) {
                $Expected['NoRecursionOnMatch'] = $true;
                $Expected['IsRecursive'] = $false;
                $Expected['EffectiveMinDepth'] = 1;
                $Expected['EffectiveMaxDepth'] = 1;
            } else {
                if ($_.MaxDepth -lt 0 -or $_.MaxDepth -gt 2) {
                    Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                } else {
                    $Expected['NoRecursionOnMatch'] = $_.MaxDepth -lt 2;
                    $Expected['IsRecursive'] = $_.MaxDepth -eq 2;
                    $Expected['EffectiveMinDepth'] = ($_.MaxDepth -eq 0) ? $(0) : $(1);
                    $Expected['EffectiveMaxDepth'] = $_.MaxDepth;
                }
            }
        } else {
            if ($null -eq $_.MaxDepth) {
                if ($_.MinDepth -lt 0 -or $_.MinDepth -gt 2) {
                    Write-Error -Message "Invalid MinDepth $($_.MinDepth) for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMinDepth' -TargetObject $_;
                } else {
                    $Expected['NoRecursionOnMatch'] = $false;
                    $Expected['IsRecursive'] = $true;
                    $Expected['EffectiveMinDepth'] = $_.MinDepth;
                }
                $Expected['EffectiveMaxDepth'] = $null;
            } else {
                if ($_.MinDepth -gt $_.MaxDepth) {
                    Write-Error -Message "MaxDepth is less than MinDepth for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                } else {
                    $Expected['EffectiveMinDepth'] = $_.MinDepth;
                    $Expected['EffectiveMaxDepth'] = $_.MaxDepth;
                    if ($_.MinDepth -eq 0) {
                        if ($_.MaxDepth -eq 2) {
                            $Expected['NoRecursionOnMatch'] = $false;
                            $Expected['IsRecursive'] = $true;
                        } else {
                            if ($_.MaxDepth -gt 2) {
                                Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                                $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                            } else {
                                $Expected['IsRecursive'] = $false;
                                $Expected['NoRecursionOnMatch'] = $_.MaxDepth -eq 0;
                            }
                        }
                    } else {
                        if ($_.MinDepth -eq 1) {
                            if ($_.MaxDepth -eq 2) {
                                $Expected['NoRecursionOnMatch'] = $false;
                                $Expected['IsRecursive'] = $true;
                            } else {
                                if ($_.MaxDepth -gt 2) {
                                    Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                                    $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                                } else {
                                    $Expected['IsRecursive'] = $false;
                                    $Expected['NoRecursionOnMatch'] = $true;
                                }
                            }
                        } else {
                            if ($_.MinDepth -eq 2) {
                                if ($_.MaxDepth -eq 3) {
                                    $Expected['NoRecursionOnMatch'] = $false;
                                    $Expected['IsRecursive'] = $true;
                                } else {
                                    if ($_.MaxDepth -gt 3) {
                                        Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                                        $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                                    } else {
                                        $Expected['IsRecursive'] = $false;
                                        $Expected['NoRecursionOnMatch'] = $true;
                                    }
                                }
                            } else {
                                Write-Error -Message "Invalid MinDepth $($_.MinDepth) for DepthRange at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMinDepth' -TargetObject $_;
                                $Expected.Remove('EffectiveMinDepth') | Out-Null;
                                $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                            }
                        }
                    }
                }
            }
        }
        if ($Expected.ContainsKey('EffectiveMinDepth')) {
            $iv = $Expected['EffectiveMinDepth'];
            if ($_.MetaData.EffectiveMinDepth -ne $iv) {
                Write-Error -Message "Invalid EffectiveMinDepth for DepthRange at index $Index; Expected: $iv; Actual: $($_.MetaData.EffectiveMinDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMinDepth' -TargetObject $_;
            }
        }
        if ($Expected.ContainsKey('EffectiveMaxDepth')) {
            if ($null -eq $Expected['EffectiveMaxDepth']) {
                if ($null -ne $_.MetaData.EffectiveMaxDepth) {
                    Write-Error -Message "Invalid EffectiveMaxDepth for DepthRange at index $Index; Expected: null; Actual: $($_.MetaData.EffectiveMaxDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
                }
            } else {
                $iv = $Expected['EffectiveMaxDepth'];
                if ($null -eq $_.MetaData.EffectiveMaxDepth) {
                    Write-Error -Message "Invalid EffectiveMaxDepth for DepthRange at index $Index; Expected: $iv; Actual: null; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
                } else {
                    if ($_.MetaData.EffectiveMaxDepth -ne $iv) {
                        Write-Error -Message "Invalid EffectiveMaxDepth for DepthRange at index $Index; Expected: $iv; Actual: $($_.MetaData.EffectiveMaxDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
                    }
                }
            }
        }
        $bv = $_.IncludeAttributes -or $_.MetaData.TypeContainsHtmlAttributes;
        if ($_.MetaData.WriteHtmlAttributes -ne $bv) {
            Write-Error -Message "Invalid WriteHtmlAttributes for DepthRange at index $Index; Expected: $bv; Actual: $($_.MetaData.WriteHtmlAttributes); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidWriteHtmlAttributes' -TargetObject $_;
        }
        if ($Expected.ContainsKey('IsRecursive')) {
            $bv = $Expected['IsRecursive'];
            if ($_.MetaData.IsRecursive -ne $bv) {
                Write-Error -Message "Invalid IsRecursive for DepthRange at index $Index; Expected: $bv; Actual: $($_.MetaData.IsRecursive); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidIsRecursive' -TargetObject $_;
            }
        }
        if ($Expected.ContainsKey('NoRecursionOnMatch')) {
            $bv = $Expected['NoRecursionOnMatch'];
            if ($_.MetaData.NoRecursionOnMatch -ne $bv) {
                Write-Error -Message "Invalid NoRecursionOnMatch for DepthRange at index $Index; Expected: $bv; Actual: $($_.MetaData.NoRecursionOnMatch); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidNoRecursionOnMatch' -TargetObject $_;
            }
        }
        $key = $_.ToString();
        if ($CommandHash.ContainsKey($key)) {
            Write-Error -Message "DepthRange Parameters at index $Index are a duplicate of parameters at index $($CommandHash[$key]); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'DuplicateParameters' -TargetObject $_;
        } else {
            $CommandHash[$key] = $Index;
        }
    }
    $Index = -1;
    $CommandHash = @{};
    $ExplicitDepth | ForEach-Object {
        $Index++;
        if ($_.Depth -lt 0 -or $_.Depth -gt 2) {
            Write-Error -Message "Invalid Depth $($_.Depth) for ExplicitDepth at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidDepth' -TargetObject $_;
        } else {
            if ($_.MetaData.EffectiveMinDepth -ne $_.Depth) {
                Write-Error -Message "Invalid EffectiveMinDepth for ExplicitDepth at index $Index; Expected: $($_.Depth); Actual: $($_.MetaData.EffectiveMinDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMinDepth' -TargetObject $_;
            }
            if ($_.MetaData.EffectiveMaxDepth -ne $_.Depth) {
                Write-Error -Message "Invalid EffectiveMaxDepth for ExplicitDepth at index $Index; Expected: $($_.Depth); Actual: $($_.MetaData.EffectiveMaxDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
            }
            $bv = $_.IncludeAttributes -or $_.MetaData.TypeContainsHtmlAttributes;
            if ($_.MetaData.WriteHtmlAttributes -ne $bv) {
                Write-Error -Message "Invalid WriteHtmlAttributes for ExplicitDepth at index $Index; Expected: $bv; Actual: $($_.MetaData.WriteHtmlAttributes); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidWriteHtmlAttributes' -TargetObject $_;
            }
            if ($_.MetaData.IsRecursive) {
                Write-Error -Message "Invalid IsRecursive for ExplicitDepth at index $Index; Expected: false; Actual: true; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidIsRecursive' -TargetObject $_;
            }
            if (-not $_.MetaData.NoRecursionOnMatch) {
                Write-Error -Message "Invalid NoRecursionOnMatch for ExplicitDepth at index $Index; Expected: true; Actual: false; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidNoRecursionOnMatch' -TargetObject $_;
            }
        }
        $key = $_.ToString();
        if ($CommandHash.ContainsKey($key)) {
            Write-Error -Message "ExplicitDepth Parameters at index $Index are a duplicate of parameters at index $($CommandHash[$key]); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'DuplicateParameters' -TargetObject $_;
        } else {
            $CommandHash[$key] = $Index;
        }
    }
    $Index = -1;
    $CommandHash = @{};
    $Recurse | ForEach-Object {
        $Index++;
        if ($_.MetaData.EffectiveMinDepth -ne 1) {
            Write-Error -Message "Invalid EffectiveMinDepth for Recurse at index $Index; Expected: 1; Actual: $($_.MetaData.EffectiveMinDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMinDepth' -TargetObject $_;
        }
        if ($_.MetaData.EffectiveMaxDepth -ne $_.Depth) {
            Write-Error -Message "Invalid EffectiveMaxDepth for Recurse at index $Index; Expected: $($_.Depth); Actual: $($_.MetaData.EffectiveMaxDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
        }
        $bv = $_.IncludeAttributes -or $_.MetaData.TypeContainsHtmlAttributes;
        if ($_.MetaData.WriteHtmlAttributes -ne $bv) {
            Write-Error -Message "Invalid WriteHtmlAttributes for Recurse at index $Index; Expected: $bv; Actual: $($_.MetaData.WriteHtmlAttributes); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidWriteHtmlAttributes' -TargetObject $_;
        }
        if (-not $_.MetaData.IsRecursive) {
            Write-Error -Message "Invalid IsRecursive for Recurse at index $Index; Expected: true; Actual: false; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidIsRecursive' -TargetObject $_;
        }
        if ($_.MetaData.NoRecursionOnMatch) {
            Write-Error -Message "Invalid NoRecursionOnMatch for Recurse at index $Index; Expected: false; Actual: true; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidNoRecursionOnMatch' -TargetObject $_;
        }
        $key = $_.ToString();
        if ($CommandHash.ContainsKey($key)) {
            Write-Error -Message "Recurse Parameters at index $Index are a duplicate of parameters at index $($CommandHash[$key]); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'DuplicateParameters' -TargetObject $_;
        } else {
            $CommandHash[$key] = $Index;
        }
    }
    $Index = -1;
    $CommandHash = @{};
    $RecurseUnmatched | ForEach-Object {
        $Index++;
        $Expected = @{};
        if ($null -eq $_.MinDepth) {
            if ($null -eq $_.MaxDepth) {
                $Expected['IsRecursive'] = $true;
                $Expected['EffectiveMinDepth'] = 1;
                $Expected['EffectiveMaxDepth'] = $null;
                $Expected['NoRecursionOnMatch'] = $_.MetaData.TypeContainsAny -or $_.MetaData.MatchableTypeCount -gt 0 -or -not $_.MetaData.TypeContainsHtmlAttributes;
            } else {
                if ($_.MaxDepth -lt 0 -or $_.MaxDepth -gt 2) {
                    Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                } else {
                    $Expected['IsRecursive'] = $_.MaxDepth -eq 2;
                    $Expected['EffectiveMinDepth'] = ($_.MaxDepth -eq 0) ? $(0) : $(1);
                    $Expected['EffectiveMaxDepth'] = $_.MaxDepth;
                    $Expected['NoRecursionOnMatch'] = $_.MaxDepth -lt 2 -or $_.MetaData.TypeContainsAny -or $_.MetaData.MatchableTypeCount -gt 0 -or -not $_.MetaData.TypeContainsHtmlAttributes;
                }
            }
        } else {
            if ($null -eq $_.MaxDepth) {
                if ($_.MinDepth -lt 0 -or $_.MinDepth -gt 2) {
                    Write-Error -Message "Invalid MinDepth $($_.MinDepth) for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMinDepth' -TargetObject $_;
                } else {
                    $Expected['IsRecursive'] = $true;
                    $Expected['EffectiveMinDepth'] = $_.MinDepth;
                }
                $Expected['EffectiveMaxDepth'] = $null;
                $Expected['NoRecursionOnMatch'] = $_.MetaData.TypeContainsAny -or $_.MetaData.MatchableTypeCount -gt 0 -or -not $_.MetaData.TypeContainsHtmlAttributes;
            } else {
                if ($_.MinDepth -gt $_.MaxDepth) {
                    Write-Error -Message "MaxDepth is less than MinDepth for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                } else {
                    $Expected['EffectiveMinDepth'] = $_.MinDepth;
                    $Expected['EffectiveMaxDepth'] = $_.MaxDepth;
                    $Expected['NoRecursionOnMatch'] = $_.MinDepth -eq $_.MaxDepth -or $_.MetaData.TypeContainsAny -or $_.MetaData.MatchableTypeCount -gt 0 -or -not $_.MetaData.TypeContainsHtmlAttributes;
                    if ($_.MinDepth -eq 0) {
                        if ($_.MaxDepth -eq 2) {
                            $Expected['IsRecursive'] = $true;
                        } else {
                            if ($_.MaxDepth -gt 2) {
                                Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                                $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                            } else {
                                $Expected['IsRecursive'] = $false;
                            }
                        }
                    } else {
                        if ($_.MinDepth -eq 1) {
                            if ($_.MaxDepth -eq 2) {
                                $Expected['IsRecursive'] = $true;
                            } else {
                                if ($_.MaxDepth -gt 2) {
                                    Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                                    $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                                } else {
                                    $Expected['IsRecursive'] = $false;
                                }
                            }
                        } else {
                            if ($_.MinDepth -eq 2) {
                                if ($_.MaxDepth -eq 3) {
                                    $Expected['IsRecursive'] = $true;
                                } else {
                                    if ($_.MaxDepth -gt 3) {
                                        Write-Error -Message "Invalid MaxDepth $($_.MaxDepth) for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMaxDepth' -TargetObject $_;
                                        $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                                    } else {
                                        $Expected['IsRecursive'] = $false;
                                    }
                                }
                            } else {
                                Write-Error -Message "Invalid MinDepth $($_.MinDepth) for RecurseUnmatched at index $Index; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidMinDepth' -TargetObject $_;
                                $Expected.Remove('EffectiveMinDepth') | Out-Null;
                                $Expected.Remove('EffectiveMaxDepth') | Out-Null;
                            }
                        }
                    }
                }
            }
        }
        if ($Expected.ContainsKey('EffectiveMinDepth')) {
            $iv = $Expected['EffectiveMinDepth'];
            if ($_.MetaData.EffectiveMinDepth -ne $iv) {
                Write-Error -Message "Invalid EffectiveMinDepth for RecurseUnmatched at index $Index; Expected: $iv; Actual: $($_.MetaData.EffectiveMinDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMinDepth' -TargetObject $_;
            }
        }
        if ($Expected.ContainsKey('EffectiveMaxDepth')) {
            if ($null -eq $Expected['EffectiveMaxDepth']) {
                if ($null -ne $_.MetaData.EffectiveMaxDepth) {
                    Write-Error -Message "Invalid EffectiveMaxDepth for RecurseUnmatched at index $Index; Expected: null; Actual: $($_.MetaData.EffectiveMaxDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
                }
            } else {
                $iv = $Expected['EffectiveMaxDepth'];
                if ($null -eq $_.MetaData.EffectiveMaxDepth) {
                    Write-Error -Message "Invalid EffectiveMaxDepth for RecurseUnmatched at index $Index; Expected: $iv; Actual: null; Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
                } else {
                    if ($_.MetaData.EffectiveMaxDepth -ne $iv) {
                        Write-Error -Message "Invalid EffectiveMaxDepth for RecurseUnmatched at index $Index; Expected: $iv; Actual: $($_.MetaData.EffectiveMaxDepth); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidEffectiveMaxDepth' -TargetObject $_;
                    }
                }
            }
        }
        if ($_.MetaData.WriteHtmlAttributes -ne $_.MetaData.TypeContainsHtmlAttributes) {
            Write-Error -Message "Invalid WriteHtmlAttributes for RecurseUnmatched at index $Index; Expected: $($_.MetaData.TypeContainsHtmlAttributes); Actual: $($_.MetaData.WriteHtmlAttributes); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidWriteHtmlAttributes' -TargetObject $_;
        }
        if ($Expected.ContainsKey('IsRecursive')) {
            $bv = $Expected['IsRecursive'];
            if ($_.MetaData.IsRecursive -ne $bv) {
                Write-Error -Message "Invalid IsRecursive for RecurseUnmatched at index $Index; Expected: $bv; Actual: $($_.MetaData.IsRecursive); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidIsRecursive' -TargetObject $_;
            }
        }
        if ($Expected.ContainsKey('NoRecursionOnMatch')) {
            $bv = $Expected['NoRecursionOnMatch'];
            if ($_.MetaData.NoRecursionOnMatch -ne $bv) {
                Write-Error -Message "Invalid NoRecursionOnMatch for RecurseUnmatched at index $Index; Expected: $bv; Actual: $($_.MetaData.NoRecursionOnMatch); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'InvalidIsRecursive' -TargetObject $_;
            }
        }
        $key = $_.ToString();
        if ($CommandHash.ContainsKey($key)) {
            Write-Error -Message "RecurseUnmatched Parameters at index $Index are a duplicate of parameters at index $($CommandHash[$key]); Data: $($_ | ConvertTo-Json -Depth 4 -Compress)" -Category InvalidData -ErrorId 'DuplicateParameters' -TargetObject $_;
        } else {
            $CommandHash[$key] = $Index;
        }
    }
}

[DeserializedValues]$TestData = [DeserializedValues]::Import(((Get-Content -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Select-MarkdownObject.json')) | ConvertFrom-Json -Depth 5));

$TestData.ProcessRecordMethods | Assert-ValidProcessRecordMethods -ErrorAction Stop;
Assert-ValidParameterValues -DepthRange $TestData.ParameterSets.DepthRange -ExplicitDepth $TestData.ParameterSets.ExplicitDepth -Recurse $TestData.ParameterSets.Recurse -RecurseUnmatched $TestData.ParameterSets.RecurseUnmatched -ProcessRecordMethods $TestData.ProcessRecordMethods -ErrorAction Stop;
