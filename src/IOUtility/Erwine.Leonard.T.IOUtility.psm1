. $PSScriptRoot/Text-Character-And-String.ps1

. $PSScriptRoot/Type-Assertion.ps1

. $PSScriptRoot/System-IO-Stream.ps1

. $PSScriptRoot/Base64.ps1

. $PSScriptRoot/Filesystem.ps1

. $PSScriptRoot/GZip.ps1

Function Optimize-DateTime {
    <#
    .SYNOPSIS
        Optimizes Date/Time values.
    .DESCRIPTION
        Ensures Date/Time values are optimized to specific parameters.
    .EXAMPLE
        $Unspecified = [DateTime]::new(2024, 11, 16, 14, 56, 31, [DateTimeKind]::Unspecified);
        $DateTimeUtc = $DateTimeUnspecified | Optimize-DateTime -AssumeKind Local -Utc;
        # Assumes that $Unspecified represents local date/time and converts it to UTC.
    #>

    [CmdletBinding(DefaultParameterSetName = 'NoConversion')]
    Param(
        # The Date/Time values to optimize.
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('DateTime')]
        [DateTime[]]$InputValue,

        # Converts Date/Time values to Universal Time.
        [Parameter(Mandatory = $true, ParameterSetName = 'Utc')]
        [switch]$Utc,

        # Converts Date/Time values to Local Time.
        [Parameter(Mandatory = $true, ParameterSetName = 'Local')]
        [switch]$Local,

        # Truncates date/time components by setting smaller increments to zero.
        [ValidateSet('Days', 'Hours', 'Minutes', 'Seconds', 'Milliseconds')]
        [string]$TruncateTo,

        # Specifies the [DateTimeKind] value to assume when the Kind property of the InputValue is Unspecified.
        # The default is Utc.
        [ValidateScript({ $_ -ne [DateTimeKind]::Unspecified })]
        [Parameter(ParameterSetName = 'Utc')]
        [Parameter(ParameterSetName = 'Local')]
        [string]$AssumeKind = [DateTimeKind]::Utc,

        # Specifies the [DateTimeKind] value to change to when the Kind property of the InputValue is Unspecified.
        [ValidateScript({ $_ -ne [DateTimeKind]::Unspecified })]
        [Parameter(ParameterSetName = 'NoConversion')]
        [string]$UnspecifiedTo
    )

    Process {
        if ($PSBoundParameters.ContainsKey('TruncateTo')) {
            if ($Utc.IsPresent) {
                if ($AssumeKind -eq [DateTimeKind]::Utc) {
                    switch ($TruncateTo) {
                        'Days' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime().Date | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).Date | Write-Output;
                                    } else {
                                        $_.Date | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                        'Hours' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, 0, 0, 0, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                        'Minutes' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, 0, 0, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                        'Seconds' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, 0, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                        default {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, $_.Millisecond, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                    }
                } else {
                    switch ($TruncateTo) {
                        'Days' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime().Date | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).ToUniversalTime().Date | Write-Output;
                                    } else {
                                        $_.Date | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                        'Hours' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).ToUniversalTime() | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, 0, 0, 0, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                        'Minutes' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).ToUniversalTime() | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, 0, 0, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                        'Seconds' {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).ToUniversalTime() | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, 0, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                        default {
                            $InputValue | ForEach-Object {
                                if ($_.Kind -eq [DateTimeKind]::Local) {
                                    $_.ToUniversalTime() | Write-Output;
                                } else {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).ToUniversalTime() | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                }
                            } | ForEach-Object {
                                [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, $_.Millisecond, 0, $_.Kind) | Write-Output;
                            }
                            break;
                        }
                    }
                }
            } else {
                if ($AssumeKind -eq [DateTimeKind]::Local) {
                    if ($Local.IsPresent) {
                        switch ($TruncateTo) {
                            'Days' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime().Date | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).Date | Write-Output;
                                        } else {
                                            $_.Date | Write-Output;
                                        }
                                    }
                                }
                                break;
                            }
                            'Hours' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, 0, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Minutes' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Seconds' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            default {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, $_.Millisecond, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                        }
                    } else {
                        switch ($TruncateTo) {
                            'Days' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).Date | Write-Output;
                                    } else {
                                        $_.Date | Write-Output;
                                    }
                                }
                                break;
                            }
                            'Hours' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, 0, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Minutes' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Seconds' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            default {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, $_.Millisecond, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                        }
                    }
                } else {
                    if ($Local.IsPresent) {
                        switch ($TruncateTo) {
                            'Days' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime().Date | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime().Date | Write-Output;
                                        } else {
                                            $_.Date | Write-Output;
                                        }
                                    }
                                }
                                break;
                            }
                            'Hours' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime() | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, 0, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Minutes' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime() | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Seconds' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime() | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime() | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            default {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Utc) {
                                        $_.ToLocalTime() | Write-Output;
                                    } else {
                                        if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                            [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime() | Write-Output;
                                        } else {
                                            $_ | Write-Output;
                                        }
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, $_.Millisecond, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                        }
                    } else {
                        switch ($TruncateTo) {
                            'Days' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).Date | Write-Output;
                                    } else {
                                        $_.Date | Write-Output;
                                    }
                                }
                                break;
                            }
                            'Hours' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, 0, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Minutes' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, 0, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            'Seconds' {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, 0, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                            default {
                                $InputValue | ForEach-Object {
                                    if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                        [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                                    } else {
                                        $_ | Write-Output;
                                    }
                                } | ForEach-Object {
                                    [DateTime]::new($_.Year, $_.Month, $_.Day, $_.Hour, $_.Minute, $_.Second, $_.Millisecond, 0, $_.Kind) | Write-Output;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        } else {
            if ($Utc.IsPresent) {
                if ($AssumeKind -eq [DateTimeKind]::Utc) {
                    $InputValue | ForEach-Object {
                        if ($_.Kind -eq [DateTimeKind]::Local) {
                            $_.ToUniversalTime() | Write-Output;
                        } else {
                            if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                            } else {
                                $_ | Write-Output;
                            }
                        }
                    }
                } else {
                    $InputValue | ForEach-Object {
                        if ($_.Kind -eq [DateTimeKind]::Local) {
                            $_.ToUniversalTime() | Write-Output;
                        } else {
                            if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                [DateTime]::SpecifyKind($_, [DateTimeKind]::Local).ToUniversalTime() | Write-Output;
                            } else {
                                $_ | Write-Output;
                            }
                        }
                    }
                }
            } else {
                if ($AssumeKind -eq [DateTimeKind]::Local) {
                    if ($Local.IsPresent) {
                        $InputValue | ForEach-Object {
                            if ($_.Kind -eq [DateTimeKind]::Utc) {
                                $_.ToLocalTime() | Write-Output;
                            } else {
                                if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                    [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                                } else {
                                    $_ | Write-Output;
                                }
                            }
                        }
                    } else {
                        $InputValue | ForEach-Object {
                            if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                [DateTime]::SpecifyKind($_, [DateTimeKind]::Local) | Write-Output;
                            } else {
                                $_ | Write-Output;
                            }
                        }
                    }
                } else {
                    if ($Local.IsPresent) {
                        $InputValue | ForEach-Object {
                            if ($_.Kind -eq [DateTimeKind]::Utc) {
                                $_.ToLocalTime() | Write-Output;
                            } else {
                                if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                    [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc).ToLocalTime() | Write-Output;
                                } else {
                                    $_ | Write-Output;
                                }
                            }
                        }
                    } else {
                        $InputValue | ForEach-Object {
                            if ($_.Kind -eq [DateTimeKind]::Unspecified) {
                                [DateTime]::SpecifyKind($_, [DateTimeKind]::Utc) | Write-Output;
                            } else {
                                $_ | Write-Output;
                            }
                        }
                    }
                }
            }
        }
    }
}
