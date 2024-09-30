Function Assert-IsNotNull {
    <#
    .SYNOPSIS
        Asserts that an object is not null.
    .DESCRIPTION
        Throws an error if an object is null.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        # The object to assert.
        [object]$InputObject,

        # The error message for insertion failure, where '{0}' is a placeholder for the pipeline position.
        [string]$ErrorMessage,

        # Specifies the action that requires the assertion.
        [string]$CategoryActivity,

        # Specifies the name of the object that is being asserted.
        [string]$CategoryTargetName,

        # Specifies the object that is being processed during the assertion.
        [object]$TargetObject,

        # Returns the asserted object.
        [switch]$PassThru
    )

    Begin { $Position = -1 }

    Process {
        $Position++;
        if ($null -eq $InputObject) {
            if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message ($ErrorMessage -f $Position) -Category InvalidArgument -ErrorAction Stop;
            }
            if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message 'Value is null' -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message 'Value is null' -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message 'Value is null' -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message 'Value is null' -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
            }
            if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message 'Value is null' -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message 'Value is null' -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
            }
            if ($PSBoundParameters.ContainsKey('TargetObject')) {
                Write-Error -Message 'Value is null' -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
            }
            Write-Error -Message 'Value is null' -Category InvalidArgument -ErrorAction Stop;
        }
        if ($PassThru.IsPresent) { Write-Output -InputObject $InputObject -NoEnumerate }
    }
}

Function Assert-IsType {
    <#
    .SYNOPSIS
        Asserts that an object is of a specified type.
    .DESCRIPTION
        Throws an error if an object is of a specified type.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [object]$InputObject,

        [Parameter(Mandatory = $true)]
        [Type[]]$Type,

        # Allow null values
        [switch]$AllowNull,

        # The error message for insertion failure, where '{0}' is a placeholder for the invalid value, and {1} is the placeholder for the pipeline position.
        [string]$ErrorMessage,

        # Specifies the action that requires the assertion.
        [string]$CategoryActivity,

        # Specifies the name of the object that is being asserted.
        [string]$CategoryTargetName,

        # Specifies the object that is being processed during the assertion.
        [object]$TargetObject,

        # Returns the asserted object.
        [switch]$PassThru
    )

    Begin { $Position = -1 }

    Process {
        $Position++;
        if ($null -eq $InputObject) {
            if (-not $AllowNull.IsPresent) {
                $Msg = $null;
                if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                    $Msg = $ErrorMessage -f $InputObject, $Position;
                } else {
                    $Msg = 'Value is null';
                }
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
            }
        } else {
            $Failed = $true;
            foreach ($t in $Type) {
                if ($InputObject -is $t) {
                    $Failed = $false;
                    break;
                }
            }
            if ($Failed) {
                $Msg = $null;
                if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                    $Msg = $ErrorMessage -f $InputObject, $Position;
                } else {
                    $Msg = [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type[-1]);
                    if ($Type.Length -eq 2) {
                        $Msg = "$([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type[0])) or $Msg"
                    } else {
                        $Msg = "$((($Type | Select-Object -SkipLast 1) | ForEach-Object { [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($_) }) -join ', '), or $Msg"
                    }
                    $Msg = "Expected: $Msg; Actual: $([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($InputObject.GetType()))"
                }
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
            }
        }
        if ($PassThru.IsPresent) { Write-Output -InputObject $InputObject -NoEnumerate }
    }
}

Function Assert-IsString {
    <#
    .SYNOPSIS
        Asserts that an object is a string value.
    .DESCRIPTION
        Throws an error if an object is not an acceptable string value.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [object]$InputObject,

        # Allow null values
        [switch]$AllowNull,

        # Allow null values
        [switch]$AllowEmpty,

        # Allow null values
        [switch]$AllowWhiteSpace,

        # The error message for insertion failure, where '{0}' is a placeholder for the invalid value, and {1} is the placeholder for the pipeline position.
        [string]$ErrorMessage,

        # Specifies the action that requires the assertion.
        [string]$CategoryActivity,

        # Specifies the name of the object that is being asserted.
        [string]$CategoryTargetName,

        # Specifies the object that is being processed during the assertion.
        [object]$TargetObject,

        # Returns the asserted object.
        [switch]$PassThru
    )

    Begin { $Position = -1 }

    Process {
        $Position++;

        if ($null -eq $InputObject) {
            if (-not $AllowNull.IsPresent) {
                $Msg = $null;
                if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                    $msg = ($ErrorMessage -f $InputObject, $Position);
                } else {
                    $Msg = 'Value is null';
                }
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
            }
        } else {
            if ($InputObject -is [string]) {
                $IsEmpty = $InputObject.Length -eq 0;
                if ($IsEmpty -and $AllowEmpty.IsPresent) {
                    $IsEmpty = (-not $AllowWhiteSpace.IsPresent) -and [string]::IsNullOrWhiteSpace($InputObject);
                } else {
                    if (-not $IsEmpty) {
                        $IsEmpty = (-not $AllowWhiteSpace.IsPresent) -and [string]::IsNullOrWhiteSpace($InputObject);
                    }
                }
                if ($IsEmpty) {
                    $Msg = $null;
                    if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                        $msg = ($ErrorMessage -f $InputObject, $Position);
                    } else {
                        $Msg = 'Value is empty';
                    }
                    if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                        if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                            if ($PSBoundParameters.ContainsKey('TargetObject')) {
                                Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                            }
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                        }
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
                }
            } else {
                $Msg = $null;
                if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                    $msg = ($ErrorMessage -f $InputObject, $Position);
                } else {
                    $Msg = "Expected: $([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName([string])); Actual: $([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($InputObject.GetType()))";
                }
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
            }
        }
        if ($PassThru.IsPresent) { Write-Output -InputObject $InputObject -NoEnumerate }
    }
}

Function Assert-IsPsEnumerable {
    <#
    .SYNOPSIS
        Asserts that an object is an enumerable type (other than string and dictionary).
    .DESCRIPTION
        Throws an error if an [System.Management.Automation.LanguagePrimitives]::IsObjectEnumerable return false.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [object]$InputObject,

        # Allow null values
        [switch]$AllowNull,

        # Allow null values
        [switch]$AllowEmpty,

        # The error message for insertion failure, where '{0}' is a placeholder for the invalid value, and {1} is the placeholder for the pipeline position.
        [string]$ErrorMessage,

        # Specifies the action that requires the assertion.
        [string]$CategoryActivity,

        # Specifies the name of the object that is being asserted.
        [string]$CategoryTargetName,

        # Specifies the object that is being processed during the assertion.
        [object]$TargetObject,

        # Returns the asserted object.
        [switch]$PassThru
    )

    Begin { $Position = -1 }

    Process {
        $Position++;

        if ($null -eq $InputObject) {
            if (-not $AllowNull.IsPresent) {
                $Msg = $null;
                if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                    $msg = ($ErrorMessage -f $InputObject, $Position);
                } else {
                    $Msg = 'Value is null';
                }
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
            }
        } else {
            if ([System.Management.Automation.LanguagePrimitives]::IsObjectEnumerable($InputObject)) {
                if (-not ($AllowEmpty.IsPresent -or [System.Management.Automation.LanguagePrimitives]::GetEnumerator($InputObject).MoveNext())) {
                    $Msg = $null;
                    if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                        $msg = ($ErrorMessage -f $InputObject, $Position);
                    } else {
                        $Msg = 'Value is empty';
                    }
                    if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                        if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                            if ($PSBoundParameters.ContainsKey('TargetObject')) {
                                Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                            }
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                        }
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
                }
            } else {
                $Msg = $null;
                if ($PSBoundParameters.ContainsKey('ErrorMessage')) {
                    $msg = ($ErrorMessage -f $InputObject, $Position);
                } else {
                    $Msg = "Expected: $([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName([System.Collections.IEnumerable])); Actual: $([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($InputObject.GetType()))"
                }
                if ($PSBoundParameters.ContainsKey('CategoryActivity')) {
                    if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                        if ($PSBoundParameters.ContainsKey('TargetObject')) {
                            Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                        }
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                    }
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryActivity $CategoryActivity -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('CategoryTargetName')) {
                    if ($PSBoundParameters.ContainsKey('TargetObject')) {
                        Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -TargetObject $TargetObject -ErrorAction Stop;
                    }
                    Write-Error -Message $Msg -Category InvalidArgument -CategoryTargetName $CategoryTargetName -ErrorAction Stop;
                }
                if ($PSBoundParameters.ContainsKey('TargetObject')) {
                    Write-Error -Message $Msg -Category InvalidArgument -TargetObject $TargetObject -ErrorAction Stop;
                }
                Write-Error -Message $Msg -Category InvalidArgument -ErrorAction Stop;
            }
        }
        if ($PassThru.IsPresent) { Write-Output -InputObject $InputObject -NoEnumerate }
    }
}

Function Invoke-WhenNotNull {
    <#
    .SYNOPSIS
        Invokes a ScriptBlock when an object is not null.
    .DESCRIPTION
        Invokes a ScriptBlock when the current input object is not null, optionally invoking an alternate ScriptBlock when null.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        # The object to assert.
        [object]$InputObject,

        [Parameter(Mandatory = $true, Position = 0)]
        # Script that gets invoked when the value is not null, with the first argument being the non-null value, and the second argument is the pipeline position.
        [ScriptBlock]$WhenNotNull,

        [Parameter(Position = 1)]
        # Script that gets invoked when the is not null, with the first argument being the pipeline position.
        [ScriptBlock]$WhenNull
    )

    Begin { $Position = -1 }

    Process {
        $Position++;
        if ($null -eq $InputObject) {
            if ($PSBoundParameters.ContainsKey('WhenNull')) {
                $WhenNull.Invoke($Position) | Write-Output;
            }
        } else {
            $WhenNotNull.Invoke($InputObject, $Position) | Write-Output;
        }
    }
}

Function Invoke-WhenIsType {
    <#
    .SYNOPSIS
        Invokes a ScriptBlock when an object is of a specified type.
    .DESCRIPTION
        Invokes a ScriptBlock when the current input object is of a specified type, optionally invoking an alternate ScriptBlock otherwise.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        # The object to assert.
        [object]$InputObject,

        [Parameter(Mandatory = $true)]
        [Type[]]$Type,

        # Pass null values to 'WhenIsType'
        [switch]$AllowNull,

        [Parameter(Mandatory = $true, Position = 0)]
        # Script that gets invoked when the value is a matching type, with the first argument being the current value, the second argument is the pipeline position, and the third is the first matched type.
        [ScriptBlock]$WhenIsType,

        [Parameter(Position = 1)]
        # Script that gets invoked when the value is not a matching type, with the first argument being the current value, and the second argument is the pipeline position.
        [ScriptBlock]$WhenNotType
    )

    Begin { $Position = -1 }

    Process {
        $Position++;
        if ($null -eq $InputObject) {
            if ($AllowNull) {
                $WhenIsType.Invoke($InputObject, $Position, $null) | Write-Output;
            } else {
                if ($PSBoundParameters.ContainsKey('WhenNotType')) {
                    $WhenNotType.Invoke($InputObject, $Position) | Write-Output;
                }
            }
        } else {
            $NoMatch = $true;
            foreach ($t in $Type) {
                if ($InputObject -is $t) {
                    $NoMatch = $false;
                    $WhenIsType.Invoke($InputObject, $Position, $t) | Write-Output;
                    break;
                }
            }
            if ($NoMatch) {
                if ($PSBoundParameters.ContainsKey('WhenNotType')) {
                    $WhenNotType.Invoke($InputObject, $Position) | Write-Output;
                }
            }
        }
    }
}

Function Invoke-WhenIsString {
    <#
    .SYNOPSIS
        Invokes a ScriptBlock when an object is a string value.
    .DESCRIPTION
        Invokes a ScriptBlock when the current input object is an acceptable string value, optionally invoking an alternate ScriptBlock otherwise.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        # The object to assert.
        [object]$InputObject,

        [Parameter(Mandatory = $true)]
        [Type[]]$Type,

        # Pass null values to 'WhenString'
        [switch]$AllowNull,

        # Pass emnpty strings to 'WhenString'
        [switch]$AllowEmpty,

        # Pass strings with only whitespace to 'WhenString'
        [switch]$AllowWhiteSpace,

        [Parameter(Mandatory = $true, Position = 0)]
        # Script that gets invoked when the value is an acceptable string value, with the first argument being the current value, and the second argument is the pipeline position.
        [ScriptBlock]$WhenString,

        [Parameter(Position = 1)]
        # Script that gets invoked when the value is not an acceptable string value, with the first argument being the current value, and the second argument is the pipeline position.
        [ScriptBlock]$WhenNotString
    )

    Begin { $Position = -1 }

    Process {
        $Position++;
        if ($null -eq $InputObject) {
            if ($AllowNull) {
                $WhenString.Invoke($InputObject, $Position) | Write-Output;
            } else {
                if ($PSBoundParameters.ContainsKey('WhenNotString')) {
                    $WhenNotString.Invoke($InputObject, $Position) | Write-Output;
                }
            }
        } else {
            $IsValid = $InputObject -is [string];
            if ($IsValid) {
                $IsValid = $InputObject.Length -gt 0;
                if ((-not $IsValid) -and $AllowEmpty.IsPresent) {
                    $IsValid = $AllowWhiteSpace.IsPresent -or -not [string]::IsNullOrWhiteSpace($InputObject);
                } else {
                    if ($IsValid) {
                        $IsValid = $AllowWhiteSpace.IsPresent -or -not [string]::IsNullOrWhiteSpace($InputObject);
                    }
                }
            }
            if ($IsValid) {
                $WhenString.Invoke($InputObject, $Position) | Write-Output;
            } else {
                if ($PSBoundParameters.ContainsKey('WhenNotString')) {
                    $WhenNotString.Invoke($InputObject, $Position) | Write-Output;
                }
            }
        }
    }
}

Function Invoke-WhenIsPsEnumerable {
    <#
    .SYNOPSIS
        Invokes a ScriptBlock when an object is an enumerable type (other than string and dictionary).
    .DESCRIPTION
        Invokes a ScriptBlock whe [System.Management.Automation.LanguagePrimitives]::IsObjectEnumerable returns true for the current input object, optionally invoking an alternate ScriptBlock otherwise.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        # The object to assert.
        [object]$InputObject,

        [Parameter(Mandatory = $true)]
        [Type[]]$Type,

        # Pass null values to 'WhenEnumerable'
        [switch]$AllowNull,

        # Pass emnpty enumerables to 'WhenEnumerable'
        [switch]$AllowEmpty,

        [Parameter(Mandatory = $true, Position = 0)]
        # Script that gets invoked when the value is an acceptable enumerable value, with the first argument being the current value, and the second argument is the pipeline position.
        [ScriptBlock]$WhenEnumerable,

        [Parameter(Position = 1)]
        # Script that gets invoked when the value is not an acceptable enumerable value, with the first argument being the current value, and the second argument is the pipeline position.
        [ScriptBlock]$WhenNotEnumerable
    )

    Begin { $Position = -1 }

    Process {
        $Position++;
        if ($null -eq $InputObject) {
            if ($AllowNull) {
                $WhenEnumerable.Invoke($InputObject, $Position, $null) | Write-Output;
            } else {
                if ($PSBoundParameters.ContainsKey('WhenNotEnumerable')) {
                    $WhenNotEnumerable.Invoke($InputObject, $Position) | Write-Output;
                }
            }
        } else {
            if ([System.Management.Automation.LanguagePrimitives]::IsObjectEnumerable($InputObject)) {
                if ($AllowEmpty.IsPresent -or [System.Management.Automation.LanguagePrimitives]::GetEnumerator($InputObject).MoveNext()) {
                    $WhenEnumerable.Invoke($InputObject, $Position, $t) | Write-Output;
                } else {
                    if ($PSBoundParameters.ContainsKey('WhenNotEnumerable')) {
                        $WhenNotEnumerable.Invoke($InputObject, $Position) | Write-Output;
                    }
                }
            } else {
                if ($PSBoundParameters.ContainsKey('WhenNotEnumerable')) {
                    $WhenNotEnumerable.Invoke($InputObject, $Position) | Write-Output;
                }
            }
        }
    }
}
