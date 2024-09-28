
Function Invoke-Thrower {
    [CmdletBinding(DefaultParameterSetName = 'WriteError')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [int]$Index,

        [string]$Position = '(root)',

        [Parameter(Mandatory = $true)]
        [ValidateSet('Begin', 'Process', 'End')]
        [string]$Stage,

        [Parameter(ParameterSetName = 'WriteError')]
        [System.Management.Automation.ActionPreference]$InnerErrorAction,

        [Parameter(Mandatory = $true, ParameterSetName = 'Throw')]
        [switch]$Throw,

        [switch]$Nested
    )

    Begin {
        "Enter { Position: $Position.Begin }" | Write-Host -ForegroundColor Green;
        if ($Stage -eq 'Begin') {
            if ($Throw.IsPresent) {
                if ($Nested.IsPresent) {
                    (0..2) | ForEach-Object | Invoke-Thrower -Position "$Position.$Stage" -Stage $Stage -Throw;
                } else {
                    throw "Throw { Position: $Position.$Stage }";
                }
            } else {
                if ($Nested.IsPresent) {
                    if ($PSBoundParameters.ContainsKey('InnerErrorAction')) {
                        (0..2) | Invoke-Thrower -Position "$Position.$Stage" -Stage $Stage -InnerErrorAction $InnerErrorAction;
                    } else {
                        (0..2) | Invoke-Thrower -Position "$Position.$Stage" -Stage $Stage;
                    }
                } else {
                    if ($PSBoundParameters.ContainsKey('InnerErrorAction')) {
                        Write-Error -Message "Error { Position: $Position.$Stage; InnerErrorAction: $InnerErrorAction }" -ErrorAction $InnerErrorAction;
                    } else {
                        Write-Error -Message "Error { Position: $Position.$Stage; InnerErrorAction: (none) }";
                    }
                }
            }
        }
        "Leave { Position: $Position.Begin }" | Write-Host -ForegroundColor Green;
    }

    Process {
        "Enter { Position: $Position.$Index }" | Write-Host -ForegroundColor Yellow;
        if ($Stage -eq 'Process') {
            if ($Throw.IsPresent) {
                if ($Nested.IsPresent) {
                    (0..2) | ForEach-Object | Invoke-Thrower -Position "$Position.$Index" -Stage $Stage -Throw;
                } else {
                    throw "Throw { Position: $Position.$Index }";
                }
            } else {
                if ($Nested.IsPresent) {
                    if ($PSBoundParameters.ContainsKey('InnerErrorAction')) {
                        (0..2) | Invoke-Thrower -Position "$Position.$Index" -Stage $Stage -InnerErrorAction $InnerErrorAction;
                    } else {
                        (0..2) | Invoke-Thrower -Position "$Position.$Index" -Stage $Stage;
                    }
                } else {
                    if ($PSBoundParameters.ContainsKey('InnerErrorAction')) {
                        Write-Error -Message "Error { Position: $Position.$Index; InnerErrorAction: $InnerErrorAction }" -ErrorAction $InnerErrorAction;
                    } else {
                        Write-Error -Message "Error { Position: $Position.$Index; InnerErrorAction: (none) }";
                    }
                }
            }
        }
        "Leave { Position: $Position.$Index }" | Write-Host -ForegroundColor Yellow;
    }

    End {
        "Enter { Position: $Position.End }" | Write-Host -ForegroundColor Magenta;
        if ($Stage -eq 'End') {
            if ($Throw.IsPresent) {
                if ($Nested.IsPresent) {
                    (0..2) | ForEach-Object | Invoke-Thrower -Position "$Position.$Stage" -Stage $Stage -Throw;
                } else {
                    throw "Throw { Position: $Position.$Stage }";
                }
            } else {
                if ($Nested.IsPresent) {
                    if ($PSBoundParameters.ContainsKey('InnerErrorAction')) {
                        (0..2) | Invoke-Thrower -Position "$Position.$Stage" -Stage $Stage -InnerErrorAction $InnerErrorAction;
                    } else {
                        (0..2) | Invoke-Thrower -Position "$Position.$Stage" -Stage $Stage;
                    }
                } else {
                    if ($PSBoundParameters.ContainsKey('InnerErrorAction')) {
                        Write-Error -Message "Error { Position: $Position.$Stage; InnerErrorAction: $InnerErrorAction }" -ErrorAction $InnerErrorAction;
                    } else {
                        Write-Error -Message "Error { Position: $Position.$Stage; InnerErrorAction: (none) }";
                    }
                }
            }
        }
        "Leave { Position: $Position.End }" | Write-Host -ForegroundColor Magenta;
    }
}


('Begin', 'Process', 'End') | ForEach-Object {
    $ArgList = "-Stage '$_'";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -InnerErrorAction Ignore";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -InnerErrorAction Ignore;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -InnerErrorAction SilentlyContinue";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -InnerErrorAction SilentlyContinue;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -InnerErrorAction Stop";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -InnerErrorAction Stop;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -Throw";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -Throw;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    
    $ArgList = "-Stage '$_' -ErrorAction Stop";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -ErrorAction Stop;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -InnerErrorAction Ignore -ErrorAction Stop";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -InnerErrorAction Ignore -ErrorAction Stop;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -InnerErrorAction SilentlyContinue -ErrorAction Stop";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -InnerErrorAction SilentlyContinue -ErrorAction Stop;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -InnerErrorAction Stop -ErrorAction Stop";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -InnerErrorAction Stop -ErrorAction Stop;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
    $ArgList = "-Stage '$_' -ErrorAction Stop -Throw";
    "Invoke-Thrower $ArgList" | Write-Host -ForegroundColor White;
    try {
        (0..3) | Invoke-Thrower -Stage $_ -ErrorAction Stop -Throw;
        "Not caught: $ArgList" | Write-Host -ForegroundColor Cyan;
    } catch {
        "Caught: $ArgList; $_" | Write-Host -ForegroundColor Cyan;
    }
}
<#

Invoke-Thrower -Stage 'Begin' -InnerErrorAction Stop
Invoke-Thrower -Stage 'Begin' -Throw
Invoke-Thrower -Stage 'Begin' -ErrorAction Stop
Invoke-Thrower -Stage 'Begin' -InnerErrorAction Stop -ErrorAction Stop
Invoke-Thrower -Stage 'Begin' -ErrorAction Stop -Throw
Invoke-Thrower -Stage 'Process' -InnerErrorAction Stop
Invoke-Thrower -Stage 'Process' -Throw
Invoke-Thrower -Stage 'Process' -ErrorAction Stop
Invoke-Thrower -Stage 'Process' -InnerErrorAction Stop -ErrorAction Stop
Invoke-Thrower -Stage 'Process' -ErrorAction Stop -Throw
#>