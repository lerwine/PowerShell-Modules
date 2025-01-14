Function Assert-ValueParameterValues {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [object]$InputObject,

        [Parameter(Mandatory = $true, ParameterSetName = 'DepthRange')]
        [switch]$DepthRange,

        [Parameter(Mandatory = $true, ParameterSetName = 'ExplicitDepth')]
        [switch]$ExplicitDepth,

        [Parameter(Mandatory = $true, ParameterSetName = 'Recurse')]
        [switch]$Recurse,

        [Parameter(Mandatory = $true, ParameterSetName = 'RecurseUnmatched')]
        [switch]$RecurseUnmatched,

        [Parameter(Mandatory = $true)]
        [object[]]$ProcessRecordMethods
    )

    Begin {
        $Position = -1;
        $IsValid = $true;
    }

    Process {
        $Position++;
        [string[]]$MemberNames = (($InputObject | Get-Member -MemberType Properties) | ForEach-Object { $_.Name });
        foreach ($n in ('NoRecursionOnMatch', 'IsRecursive', 'EffectiveMinDepth', 'EffectiveMaxDepth', 'WriteHtmlAttributes', 'TypeContainsAny', 'MatchableTypeCount', 'TypeContainsHtmlAttributes', 'Type', 'ProcessRecordMethod')) {
            if ($MemberNames -notcontains $n) {
                Write-Error -Message "Property '$n' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                $IsValid = $false;
            }
        }
        if ($MemberNames -contains 'NoRecursionOnMatch' -and $InputObject.NoRecursionOnMatch -isnot [bool]) {
            Write-Error -Message "'NoRecursionOnMatch' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'IsRecursive' -and $InputObject.IsRecursive -isnot [bool]) {
            Write-Error -Message "'IsRecursive' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'EffectiveMinDepth' -and $InputObject.EffectiveMinDepth -isnot [long]) {
            Write-Error -Message "'EffectiveMinDepth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($null -ne $InputObject.EffectiveMaxDepth -and $InputObject.EffectiveMaxDepth -isnot [long]) {
            Write-Error -Message "'EffectiveMaxDepth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'WriteHtmlAttributes' -and $InputObject.WriteHtmlAttributes -isnot [bool]) {
            Write-Error -Message "'WriteHtmlAttributes' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'TypeContainsAny' -and $InputObject.TypeContainsAny -isnot [bool]) {
            Write-Error -Message "'TypeContainsAny' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'TypeContainsHtmlAttributes' -and $InputObject.TypeContainsHtmlAttributes -isnot [bool]) {
            Write-Error -Message "'TypeContainsHtmlAttributes' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'MatchableTypeCount') {
            if ($null -eq $InputObject.MatchableTypeCount -or $InputObject.MatchableTypeCount -isnot [long]) {
                Write-Error -Message "'MatchableTypeCount' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                $IsValid = $false;
            } else {
                if ($InputObject.MatchableTypeCount -lt 0) {
                    Write-Error -Message "'MatchableTypeCount' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                    $IsValid = $false;
                } else {
                    if ($InputObject.MatchableTypeCount -gt 2) {
                        Write-Error -Message "'MatchableTypeCount' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                        $IsValid = $false;
                    } else {
                        if ($RecurseUnmatched.IsPresent -and $InputObject.MatchableTypeCount -eq 0 -and -not ($InputObject.TypeContainsAny -or $InputObject.TypeContainsHtmlAttributes)) {
                            Write-Error -Message "'MatchableTypeCount' is 0 while neither 'TypeContainsAny' nor 'TypeContainsHtmlAttributes' are true on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    }
                }
            }
        }

        $Expected = @{ };

        switch ($PSCmdlet.ParameterSetName) {
            'ExplicitDepth' {
                $Expected['NoRecursionOnMatch'] = $true;
                $Expected['IsRecursive'] = $false;
                if ($MemberNames -contains 'Depth') {
                    if ($InputObject.Depth -isnot [long]) {
                        Write-Error -Message "'Depth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId (($null -eq $InputObject.Depth) ? $('MissingProperty') : $('InvalidPropertyType')) -TargetObject $InputObject;
                        $IsValid = $false;
                    } else {
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
                    }
                } else {
                    Write-Error -Message "Property 'Depth' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                    $IsValid = $false;
                }
                break;
            }
            'Recurse' {
                $Expected['EffectiveMinDepth'] = 1;
                $Expected['EffectiveMaxDepth'] = $null;
                $Expected['NoRecursionOnMatch'] = $false;
                $Expected['IsRecursive'] = $true;
                break;
            }
            'RecurseUnmatched' {
                if ($MemberNames -contains 'MinDepth') {
                    if ($null -eq $InputObject.MinDepth) {
                        $Expected['EffectiveMinDepth'] = 1;
                    } else {
                        if ($InputObject.MinDepth -is [long]) {
                            if ($InputObject.MinDepth -lt 0) {
                                Write-Error -Message "'MinDepth' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                            } else {
                                if ($InputObject.MinDepth -gt 2) {
                                    Write-Error -Message "'MinDepth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                } else {
                                    $Expected['EffectiveMinDepth'] = $InputObject.MinDepth;
                                }
                            }
                        } else {
                            Write-Error -Message "'MinDepth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    }
                } else {
                    Write-Error -Message "Property 'MinDepth' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                    $IsValid = $false;
                }
                if ($MemberNames -contains 'MaxDepth') {
                    if ($null -eq $InputObject.MaxDepth) {
                        if ($Expected.ContainsKey('EffectiveMinDepth')) {
                            if ($null -eq $InputObject.MinDepth) {
                                $Expected['EffectiveMaxDepth'] = $null;
                                $Expected['IsRecursive'] = $true;
                                if ($InputObject.TypeContainsAny -is [bool] -and $InputObject.MatchableTypeCount -is [int] -and $InputObject.MatchableTypeCount -ge 0 -and $InputObject.MatchableTypeCount -le 2) {
                                    $Expected['NoRecursionOnMatch'] = $InputObject.TypeContainsAny -or $InputObject.MatchableTypeCount -gt 0
                                }
                            } else {
                                $Expected['EffectiveMaxDepth'] = $null;
                                $Expected['IsRecursive'] = $true;
                                if ($InputObject.TypeContainsAny -is [bool] -and $InputObject.MatchableTypeCount -is [int] -and $InputObject.MatchableTypeCount -ge 0 -and $InputObject.MatchableTypeCount -le 2) {
                                    $Expected['NoRecursionOnMatch'] = $InputObject.TypeContainsAny -or $InputObject.MatchableTypeCount -gt 0
                                }
                            }
                        }
                    } else {
                        if ($InputObject.MaxDepth -is [long]) {
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
                                                if ($InputObject.TypeContainsAny -is [bool] -and $InputObject.MatchableTypeCount -is [int] -and $InputObject.MatchableTypeCount -ge 0 -and $InputObject.MatchableTypeCount -le 2) {
                                                    $Expected['NoRecursionOnMatch'] = $InputObject.TypeContainsAny -or $InputObject.MatchableTypeCount -gt 0
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
                                                if ($InputObject.TypeContainsAny -is [bool] -and $InputObject.MatchableTypeCount -is [int] -and $InputObject.MatchableTypeCount -ge 0 -and $InputObject.MatchableTypeCount -le 2) {
                                                    $Expected['NoRecursionOnMatch'] = $InputObject.TypeContainsAny -or $InputObject.MatchableTypeCount -gt 0
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
                        } else {
                            Write-Error -Message "'MaxDepth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    }
                } else {
                    Write-Error -Message "Property 'MaxDepth' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                    $IsValid = $false;
                }
            }
            default {
                if ($MemberNames -contains 'MinDepth') {
                    if ($null -eq $InputObject.MinDepth) {
                        $Expected['EffectiveMinDepth'] = 1;
                    } else {
                        if ($InputObject.MinDepth -is [long]) {
                            if ($InputObject.MinDepth -lt 0) {
                                Write-Error -Message "'MinDepth' is negative on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                            } else {
                                if ($InputObject.MinDepth -gt 2) {
                                    Write-Error -Message "'MinDepth' is greater than 2 on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                                } else {
                                    $Expected['EffectiveMinDepth'] = $InputObject.MinDepth;
                                }
                            }
                        } else {
                            Write-Error -Message "'MinDepth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    }
                } else {
                    Write-Error -Message "Property 'MinDepth' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                    $IsValid = $false;
                }
                if ($MemberNames -contains 'MaxDepth') {
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
                        if ($InputObject.MaxDepth -is [long]) {
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
                        } else {
                            Write-Error -Message "'MaxDepth' is not an integer value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                            $IsValid = $false;
                        }
                    }
                } else {
                    Write-Error -Message "Property 'MaxDepth' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                    $IsValid = $false;
                }
                break;
            }
        }
        if ($Expected.ContainsKey('EffectiveMinDepth') -and $InputObject.EffectiveMinDepth -is [int] -and $Expected['EffectiveMinDepth'] -ne $InputObject.EffectiveMinDepth) {
            Write-Error -Message "'EffectiveMinDepth' was not $($Expected['EffectiveMinDepth']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($Expected.ContainsKey('EffectiveMaxDepth') -and $InputObject.EffectiveMaxDepth -is [bool] -and $Expected['EffectiveMaxDepth'] -ne $InputObject.EffectiveMaxDepth) {
            Write-Error -Message "'EffectiveMaxDepth' was not $($Expected['EffectiveMaxDepth']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($Expected.ContainsKey('IsRecursive') -and $InputObject.IsRecursive -is [bool] -and $Expected['IsRecursive'] -ne $InputObject.IsRecursive) {
            Write-Error -Message "'IsRecursive' was not $($Expected['IsRecursive']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($Expected.ContainsKey('NoRecursionOnMatch') -and $InputObject.NoRecursionOnMatch -is [bool] -and $Expected['NoRecursionOnMatch'] -ne $InputObject.NoRecursionOnMatch) {
            Write-Error -Message "'NoRecursionOnMatch' was not $($Expected['NoRecursionOnMatch']) on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
            $IsValid = $false;
        }
        if ($MemberNames -contains 'Type') {
            $ExpectedType = $null;
            if ($null -ne $InputObject.MatchableTypeCount -and $InputObject.MatchableTypeCount -is [long]) {
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
            }
            if ($null -ne $InputObject.Type) {
                if ($InputObject.Type -isnot [string]) {
                    Write-Error -Message "'Type' is not a string value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                    $IsValid = $false;
                } else {
                    if ($null -ne $ExpectedType) {
                        if ($ExplicitDepth -eq '') {
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
            } else {
                if ($null -ne $ExpectedType -and $ExpectedType -ne '') {
                    Write-Error -Message "Expected $($ExpectedType | ConvertTo-Json -EscapeHandling) for 'Type'; actual is null on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'PropertyValueOutOfRange' -TargetObject $InputObject;
                    $IsValid = $false;
                }
            }
        }

        if ($RecurseUnmatched.IsPresent) {
            if ($InputObject.WriteHtmlAttributes -is [bool] -and $InputObject.TypeContainsHtmlAttributes -is [bool] -and $InputObject.WriteHtmlAttributes -ne $InputObject.TypeContainsHtmlAttributes) {
                Write-Error -Message "'WriteHtmlAttributes' is equal to 'TypeContainsHtmlAttributes' on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                $IsValid = $false;
            }
        } else {
            if ($MemberNames -contains 'IncludeAttributes') {
                if ($InputObject.IncludeAttributes -is [bool]) {
                    if ($InputObject.WriteHtmlAttributes -is [bool] -and $InputObject.TypeContainsHtmlAttributes -is [bool] -and $InputObject.WriteHtmlAttributes -ne ($InputObject.TypeContainsHtmlAttributes -or $InputObject.IncludeAttributes)) {
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
                } else {
                    Write-Error -Message "'IncludeAttributes' is not a boolean value on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'InvalidPropertyType' -TargetObject $InputObject;
                    $IsValid = $false;
                }
            } else {
                Write-Error -Message "Property 'IncludeAttributes' does not exist on $($PSCmdlet.ParameterSetName) at position $Position`: $($InputObject | ConvertTo-Json -Depth 3 -Compress)" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $InputObject;
                $IsValid = $false;
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
        [object[]]$DepthRange,

        [Parameter(Mandatory = $true)]
        [object[]]$ExplicitDepth,

        [Parameter(Mandatory = $true)]
        [object[]]$Recurse,

        [Parameter(Mandatory = $true)]
        [object[]]$RecurseUnmatched,

        [Parameter(Mandatory = $true)]
        [object[]]$ProcessRecordMethods
    )

    $IsValid = $true;
    for ($i = 0; $i -lt $ProcessRecordMethods.Length; $i++) {
        $Obj = $ProcessRecordMethods[$i];
        [string[]]$MemberNames = (($Obj | Get-Member) | ForEach-Object { $_.Name });
        if ($MemberNames -contains 'Name') {
            if ($Obj.Name -isnot [string]) {
                Write-Error -Message "Property 'Name' is not a string value on ProcessRecordMethods at position $i" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $Obj;
            }
        } else {
            Write-Error -Message "Property 'Name' does not exist on ProcessRecordMethods at position $i" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $Obj;
            $IsValid = $false;
        }
        for ($n = 0; $n -lt $i; $n++) {
            if ($ProcessRecordMethods[$n].Name -eq $Obj.Name) {
                Write-Error -Message "ProcessRecordMethods Property 'Name' at position $i is a duplicate of the name at position $n" -Category InvalidData -ErrorId 'MissingProperty' -TargetObject $Obj;
                $IsValid = $false;
                break;
            }
        }
    }
    if ($IsValid -and ($DepthRange | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -DepthRange) -and ($ExplicitDepth | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -ExplicitDepth) `
            -and ($Recurse | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -Recurse) -and ($RecurseUnmatched | Assert-ValueParameterValues -ProcessRecordMethods $ProcessRecordMethods -RecurseUnmatched)) {
        $MethodReferences = @((0..($ExplicitDepth.Length - 1)) | ForEach-Object {
            $m = $ExplicitDepth[$_].ProcessRecordMethod;
            [PSCustomObject]@{
                Position = $_;
                ParameterSet = 'ExplicitDepth';
                Name = $m.Name;
                IsPrimary = $m.IsPrimary;
            }
        }) + @((0..($Recurse.Length - 1)) | ForEach-Object {
            $m = $Recurse[$_].ProcessRecordMethod;
            [PSCustomObject]@{
                Position = $_;
                ParameterSet = 'Recurse';
                Name = $m.Name;
                IsPrimary = $m.IsPrimary;
            }
        }) + @((0..($DepthRange.Length - 1)) | ForEach-Object {
            $m = $DepthRange[$_].ProcessRecordMethod;
            [PSCustomObject]@{
                Position = $_;
                ParameterSet = 'DepthRange';
                Name = $m.Name;
                IsPrimary = $m.IsPrimary;
            }
        }) + @((0..($RecurseUnmatched.Length - 1)) | ForEach-Object {
            $m = $RecurseUnmatched[$_].ProcessRecordMethod;
            [PSCustomObject]@{
                Position = $_;
                ParameterSet = 'RecurseUnmatched';
                Name = $m.Name;
                IsPrimary = $m.IsPrimary;
            }
        });
        foreach ($n in @($ProcessRecordMethods | ForEach-Object { $_.Name })) {
            $UsedBy = @($MethodReferences | Where-Object { $_.Name -ceq $n });
            switch ($UsedBy.Count) {
                0 {
                    Write-Warning -Message "ProcessRecordMethod $($n | ConvertTo-Json) is not referenced by any parameter sets";
                    break;
                }
                1 {
                    Write-Information -MessageData "ProcessRecordMethod $($n | ConvertTo-Json) is used only once";
                    if (-not $UsedBy[0].IsPrimary) {
                        Write-Warning -Message "$($UsedBy[0].ParameterSet)[$($UsedBy[0].Position)].IsPrimary should be true";
                        $IsValid = $false;
                    }
                    break;
                }
                default {
                    $Primary = @($UsedBy | Where-Object { $_.IsPrimary });
                    switch ($Primary.Count) {
                        0 {
                            Write-Warning -Message ("ProcessRecordMethod $($n | ConvertTo-Json) has no primary parameter value set, and is referenced at: " + (($UsedBy | ForEach-Object { "$($_.ParameterSet)[$($_.Position)]" }) -join ', '));
                            $IsValid = $false;
                            break;
                        }
                        1 { break }
                        default {
                            Write-Warning -Message ("ProcessRecordMethod $($n | ConvertTo-Json) has multiple primary parameter value sets referenced at: " + (($UsedBy | ForEach-Object { "$($_.ParameterSet)[$($_.Position)]" }) -join ', '));
                            $IsValid = $false;
                        }
                    }
                    break;
                }
            }
        }
    }
    $IsValid | Write-Output;
}
$TestData = (Get-Content -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Select-MarkdownObject.json')) | ConvertFrom-Json -Depth 5;
if (Test-ValueParameterCoverage -DepthRange $TestData.ParameterSets.DepthRange -ExplicitDepth $TestData.ParameterSets.ExplicitDepth -Recurse $TestData.ParameterSets.Recurse -RecurseUnmatched $TestData.ParameterSets.RecurseUnmatched -ProcessRecordMethods $TestData.ProcessRecordMethods -ErrorAction Stop) {
    Write-Information -MessageData "JSON data is valid" -InformationAction Continue;
}