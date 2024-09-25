class CharacterClass {
    [ValidateNotNullOrWhiteSpace()]
    [string]$Name;

    [ValidateNotNull()]
    [ScriptBlock]$FilterScript;

    [ValidateNotNull()]
    [System.Collections.ObjectModel.Collection[CharacterClass]]$RelatedClasses = [System.Collections.ObjectModel.Collection[CharacterClass]]::new();

    [ulong] GetFlags() { return [ulong]::MinValue }
    
    [bool] ShouldHaveAsciiChar() { return $false }

    [bool] HasAsciiChar() { return $false }

    [bool] ShouldHaveNonAsciiChar() { return $false }

    [bool] HasNonAsciiChar() { return $false }

    [bool] ShouldContainCategory([System.Globalization.UnicodeCategory]$Category) { return $false }

    [bool] ContainsCategory([System.Globalization.UnicodeCategory]$Category) { return $false }
}

class PrimaryCharacterClass : CharacterClass {
    [ValidateRange(0, 63)]
    [int]$BitIndex;

    [ValidateNotNull()]
    [System.Collections.ObjectModel.Collection[char]]$AllCharacters = [System.Collections.ObjectModel.Collection[char]]::new();

    [AllowNull()]
    [System.Globalization.UnicodeCategory[]]$AllCategories = $null;

    [AllowNull()]
    [System.Globalization.UnicodeCategory[]]$ContainedCategories = $null;

    [ValidateNotNull()]
    [ValidateCount(1, [int]::MaxValue)]
    [char[]]$TestCharacters;

    hidden [System.Nullable[bool]]$_ShouldHaveAsciiChar = $null;

    hidden [System.Nullable[bool]]$_HasAsciiChar = $null;

    hidden [System.Nullable[bool]]$_ShouldHaveNonAsciiChar = $null;

    hidden [System.Nullable[bool]]$_HasNonAsciiChar = $null;

    [ulong] GetFlags() { return ([ulong]1) -shl $this.BitIndex }
    
    [bool] ShouldHaveAsciiChar() {
        if ($null -ne $this._ShouldHaveAsciiChar) {
            $this._ShouldHaveAsciiChar = [char]::IsAscii($this.AllCharacters[0]);
            if ($this._ShouldHaveAsciiChar) {
                $this._ShouldHaveNonAsciiChar = $false;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._ShouldHaveNonAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._ShouldHaveNonAsciiChar = $true;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._ShouldHaveAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._ShouldHaveAsciiChar;
    }

    [bool] HasAsciiChar() {
        if ($null -ne $this._HasAsciiChar) {
            $this._HasAsciiChar = [char]::IsAscii($this.TestCharacters[0]);
            if ($this._HasAsciiChar) {
                $this._HasNonAsciiChar = $false;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._HasNonAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._HasNonAsciiChar = $true;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._HasAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._HasAsciiChar;
    }

    [bool] ShouldHaveNonAsciiChar() {
        if ($null -ne $this._ShouldHaveNonAsciiChar) {
            $this._ShouldHaveNonAsciiChar = -not [char]::IsAscii($this.AllCharacters[0]);
            if ($this._ShouldHaveNonAsciiChar) {
                $this._ShouldHaveAsciiChar = $false;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._ShouldHaveAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._ShouldHaveAsciiChar = $true;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._ShouldHaveNonAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._ShouldHaveNonAsciiChar;
    }

    [bool] HasNonAsciiChar() {
        if ($null -ne $this._HasNonAsciiChar) {
            $this._HasNonAsciiChar = -not [char]::IsAscii($this.TestCharacters[0]);
            if ($this._HasNonAsciiChar) {
                $this._HasAsciiChar = $false;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._HasAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._HasAsciiChar = $true;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._HasNonAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._HasNonAsciiChar;
    }

    [bool] ShouldContainCategory([System.Globalization.UnicodeCategory]$Category) {
        if ($null -eq $this.AllCategories) {
            $this.AllCategories = ($this.AllCharacters | ForEach-Object { [char]::GetUnicodeCategory($_) } | Sort-Object -Unique);
        }
        return $this.AllCategories -contains $Category;
    }

    [bool] ContainsCategory([System.Globalization.UnicodeCategory]$Category) {
        if ($null -eq $this.ContainedCategories) {
            $this.ContainedCategories = ($this.TestCharacters | ForEach-Object { [char]::GetUnicodeCategory($_) } | Sort-Object -Unique);
        }
        return $this.ContainedCategories -contains $Category;
    }
}

class AggregateCharacterClass : CharacterClass {
    [ValidateNotNull()]
    [ValidateCount(1, [int]::MaxValue)]
    [CharacterClass[]]$SubClasses;
    
    [ulong] GetFlags() {
        $Flags = [ulong]::MinValue;
        foreach ($c in $this.SubClasses) { $Flags = $Flags -bor $c.GetFlags() }
        return $Flags;
    }
    
    [bool] ShouldHaveAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.ShouldHaveAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] HasAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.HasAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] ShouldHaveNonAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.ShouldHaveNonAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] HasNonAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.HasNonAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] ShouldContainCategory([System.Globalization.UnicodeCategory]$Category) {
        foreach ($c in $this.SubClasses) {
            if ($c.ShouldContainCategory($Category)) { return $true }
        }
        return $false;
    }

    [bool] ContainsCategory([System.Globalization.UnicodeCategory]$Category) {
        foreach ($c in $this.SubClasses) {
            if ($c.ContainsCategory($Category)) { return $true }
        }
        return $false;
    }
}

Function ConvertTo-PSLiteral {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char]$Value,

        [switch]$ForceDoubleQuotes
    )

    Process {
        switch ($Value) {
            "`a" { return '"`a"' }
            "`b" { return '"`b"' }
            "`t" { return '"`t"' }
            "`n" { return '"`n"' }
            "`v" { return '"`v"' }
            "`f" { return '"`f"' }
            "`r" { return '"`r"' }
            "`e" { return '"`e"' }
            "'" { return "`"'`"" }
            '`' {
                if ($ForceDoubleQuotes.IsPresent) { return "'``'" }
                return "'``'";
            }
            '$' {
                if ($ForceDoubleQuotes.IsPresent) { return "'`$'" }
                return "'`$'";
            }
            '"' {
                if ($ForceDoubleQuotes.IsPresent) { return "'`"'" }
                return "'`"'";
            }
        }
        if ([char]::IsControl($Value)) { return "`"``u{$(([int]$Value).ToString('x4'))}`"" }
        if ([char]::IsAscii($Value) -or [char]::IsLetterOrDigit($Value) -or [char]::IsNumber($Value) -or [char]::IsPunctuation($Value) -or [char]::IsSymbol($Value)) {
            if ($ForceDoubleQuotes.IsPresent) { return "`"$Value`"" }
            return "'$Value'";
        }
        return "`"``u{$(([int]$Value).ToString('x4'))}`"";
    }
}

Function Test-CharacterClass {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char[]]$InputValue,

        [CharacterClass]$Class,

        [switch]$Aggregated
    )

    Process {
        if ($Aggregated.IsPresent) {
            foreach ($c in $InputValue) {
                if (-not ($Class.FilterScript.InvokeWithContext($null, ([System.Collections.Generic.List[PSVariable]]@([PSVariable]::new('_', $c, [System.Management.Automation.ScopedItemOptions]::ReadOnly)))))) { return $false }
            }
        } else {
            foreach ($c in $InputValue) {
                if ($Class.FilterScript.InvokeWithContext($null, ([System.Collections.Generic.List[PSVariable]]@([PSVariable]::new('_', $c, [System.Management.Automation.ScopedItemOptions]::ReadOnly))))) {
                    $true | Write-Output;
                } else {
                    $false | Write-Output;
                }
            }
        }
    }

    End {
        if ($Aggregated.IsPresent) { $true | Write-Output }
    }
}

Function Get-AllCharacters {
    [CmdletBinding()]
    [OutputType([char[]])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses
    )

    Process {
        foreach ($c in $InputClasses) {
            if ($c -is [PrimaryCharacterClass]) {
                $c.AllCharacters | Write-Output;
            } else {
                $c.SubClasses | Get-TestCharacters;
            }
        }
    }
}

Function Get-TestCharacters {
    [CmdletBinding()]
    [OutputType([char[]])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputClasses
    )

    Process {
        foreach ($c in $InputClasses) {
            if ($c -is [PrimaryCharacterClass]) {
                $c.TestCharacters | Write-Output;
            } else {
                $c.SubClasses | Get-TestCharacters;
            }
        }
    }
}

Function Assert-UniqueReference {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [object[]]$InputObject,

        [swtich]$PassThru
    )

    Begin { $Emitted = @() }

    Process {
        if ($PassThru.IsPresent) {
            foreach ($obj in $InputObject) {
                foreach ($E in $Emitted) {
                    if ([object]::ReferenceEquals($E, $obj)) {
                        Write-Error -Message "Item of same reference already exists" -Category ResourceExists -ErrorId 'DupilcateReferenceDetected' `
                            -TargetObject $obj -CategoryActivity "ReferenceEquals" -CategoryReason "Current object is same instance as previous object" -ErrorAction Stop;
                    }
                }
                $obj | Write-Output;
                $Emitted += $obj;
            }
        } else {
            foreach ($obj in $InputObject) {
                foreach ($E in $Emitted) {
                    if ([object]::ReferenceEquals($E, $obj)) {
                        Write-Error -Message "Item of same reference already exists" -Category ResourceExists -ErrorId 'DupilcateReferenceDetected' `
                            -TargetObject $obj -CategoryActivity "ReferenceEquals" -CategoryReason "Current object is same instance as previous object" -ErrorAction Stop;
                    }
                }
                $Emitted += $obj;
            }
        }
    }
}

Function Assert-ValidRelatedClassHierarchy {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses,

        [switch]$ReturnRelated
    )

    Process {
        if ($ReturnRelated.IsPresent) {
            foreach ($CharacterClass in $InputClasses) {
                try { $CharacterClass.RelatedClasses | Assert-UniqueReference -ErrorAction Stop }
                catch {
                    Write-Error -Exception $_.Exception -Message "Character class $($CharacterClass.Name) has duplicate related classes" -Category $_.CategoryInfo.Category -ErrorId 'DuplicateRelatedClass' `
                        -TargetObject $_.TargetObject -CategoryActivity $_.CategoryInfo.Activity -CategoryReason 'Assert-UniqueReference threw an error on RelatedClasses property' -ErrorAction Stop;
                }
                foreach ($RelatedClass in $CharacterClass.RelatedClasses) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    $RelatedClass | Write-Output;
                    $SubRelated = @($RelatedClass | Assert-ValidRelatedClassHierarchy -ReturnRelated);
                    foreach ($r in $SubRelated) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                        $r | Write-Output;
                    }
                }
            }
        } else {
            foreach ($CharacterClass in $InputClasses) {
                try { $CharacterClass.RelatedClasses | Assert-UniqueReference -ErrorAction Stop }
                catch {
                    Write-Error -Exception $_.Exception -Message "Character class $($CharacterClass.Name) has duplicate related classes" -Category $_.CategoryInfo.Category -ErrorId 'DuplicateRelatedClass' `
                        -TargetObject $_.TargetObject -CategoryActivity $_.CategoryInfo.Activity -CategoryReason 'Assert-UniqueReference threw an error on RelatedClasses property' -ErrorAction Stop;
                }
                foreach ($RelatedClass in $CharacterClass.RelatedClasses) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    foreach ($r in ($RelatedClass | Assert-ValidRelatedClassHierarchy -ReturnRelated)) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                    }
                }
            }
        }
    }
}

Function Assert-ValidSubClassHierarchy {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [AggregateCharacterClass[]]$InputClasses,

        [switch]$ReturnSub
    )

    Process {
        if ($ReturnSub.IsPresent) {
            foreach ($CharacterClass in $InputClasses) {
                try { $CharacterClass.SubClasses | Assert-UniqueReference -ErrorAction Stop }
                catch {
                    Write-Error -Exception $_.Exception -Message "Character class $($CharacterClass.Name) has duplicate sub-classes" -Category $_.CategoryInfo.Category -ErrorId 'DuplicateSubClass' `
                        -TargetObject $_.TargetObject -CategoryActivity $_.CategoryInfo.Activity -CategoryReason 'Assert-UniqueReference threw an error on SubClasses property' -ErrorAction Stop;
                }
                foreach ($RelatedClass in ($CharacterClass.SubClasses | Where-Object { $_ -is [AggregateCharacterClass] })) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "SubClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    $RelatedClass | Write-Output;
                    $SubRelated = @($RelatedClass | Assert-ValidSubClassHierarchy -ReturnSub);
                    foreach ($r in $SubRelated) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested SubClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                        $r | Write-Output;
                    }
                }
            }
        } else {
            foreach ($CharacterClass in $InputClasses) {
                foreach ($RelatedClass in ($CharacterClass.SubClasses | Where-Object { $_ -is [AggregateCharacterClass] })) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "SubClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    foreach ($r in ($RelatedClass | Assert-ValidSubClassHierarchy -ReturnSub)) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested SubClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                    }
                }
            }
        }
    }
}

Function Get-RelatedClasses {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses,

        [switch]$Recurse,

        [switch]$CheckCircularReferences
    )

    Process {
        if ($Recurse.IsPresent) {
            foreach ($CharacterClass in $InputClasses) {
                foreach ($RelatedClass in $CharacterClass.RelatedClasses) {
                    $RelatedClass | Write-Output;
                    $RelatedClass | Get-RelatedClasses -Recurse;
                }
            }
        } else {
            foreach ($CharacterClass in $InputClasses) {
                $CharacterClass.RelatedClasses | Write-Output;
            }
        }
    }
}

Function Assert-ValidCharacterClass {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses
    )

    Process {
        foreach ($CharacterClass in $InputClasses) {
            $AllChars = @($CharacterClass | Get-AllCharacters);
            foreach ($c in $AllChars) {
                if (-not ($c | Test-CharacterClass -Class $CharacterClass)) {
                    Write-Error -Message "FilterScript for character class $($CharacterClass.Name) fails to match character $($c | ConvertTo-PSLiteral): $($CharacterClass.FilterScript)" -Category InvalidOperation -ErrorId 'FilterScriptValidationFailure' `
                        -TargetObject $c -CategoryActivity "Test-CharacterClass" -CategoryReason "Test-CharacterClass using $($CharacterClass.Name) returned false" -ErrorAction Stop;
                }
            }
            if ($CharacterClass -is [PrimaryCharacterClass]) {
                foreach ($c in ($CharacterClass | Get-TestCharacters)) {
                    if ($AllChars -inotcontains $c) {
                        Write-Error -Message "Test character $($c | ConvertTo-PSLiteral) does not belong to character class $($CharacterClass.Name)" -Category InvalidData -ErrorId 'TestCharacterValidationFailure' `
                            -TargetObject $c -CategoryActivity "-inotcontains" -CategoryReason "Results of Get-AllCharacters for $($CharacterClass.Name) does not contain test character" -ErrorAction Stop;
                    }
                }
                if ($CharacterClass.ShouldHaveAsciiChar()) {
                    if (-not $CharacterClass.HasAsciiChar()) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes one or more ASCII characters, but the test characters do not" -Category InvalidData -ErrorId 'TestCharacterValidationFailure' `
                            -TargetObject $c -CategoryActivity "HasAsciiChar()" -CategoryReason "ShouldHaveAsciiChar() retrned true, but HasAsciiChar() returned false" -ErrorAction Stop;
                    }
                }
                if ($CharacterClass.ShouldHaveNonAsciiChar()) {
                    if (-not $CharacterClass.HasNonAsciiChar()) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes one or more non-ASCII characters, but the test characters do not" -Category InvalidData -ErrorId 'TestCharacterValidationFailure' `
                            -TargetObject $c -CategoryActivity "HasNonAsciiChar()" -CategoryReason "ShouldHaveNonAsciiChar() retrned true, but HasNonAsciiChar() returned false" -ErrorAction Stop;
                    }
                }
            } else {
                $CharacterClass | Assert-ValidSubClassHierarchy -ErrorAction Stop;
            }
            $CharacterClass | Assert-ValidRelatedClassHierarchy -ErrorAction Stop;
            foreach ($RelatedClass in ($CharacterClass | Get-RelatedClasses)) {
                foreach ($c in $AllChars) {
                    if ($c | Test-CharacterClass -Class $RelatedClass) {
                        if (($RelatedClass | Get-AllCharacters) -contains $c) {
                            Write-Error -Message "Character class $($CharacterClass.Name) and related class $($RelatedClass.Name) both include the $($c | ConvertTo-PSLiteral) character" -Category InvalidData -ErrorId 'RelatedCharacterValidationFailure' `
                                -TargetObject $c -CategoryActivity "Test-CharacterClass" -CategoryReason "Detected overlap between character classes $($CharacterClass.Name) and $($RelatedClass.Name)" -ErrorAction Stop;
                        }
                    }
                }
            }
        }
    }
}

Function New-PrimaryCharacterClass {
    [CmdletBinding()]
    [OutputType([PrimaryCharacterClass])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[A-Z][a-z]+$')]
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [ValidateRange(0, 63)]
        [int]$BitIndex,

        [Parameter(Mandatory = $true, Position = 2)]
        [ScriptBlock]$FilterScript,

        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char[]]$InputCharacters,

        [Hashtable]$AddTo,

        [switch]$PassThru
    )
    
    Begin { $AllCharacters = [System.Collections.ObjectModel.Collection[char]]::new() }

    Process {
        foreach ($c in $InputCharacters) { $AllCharacters.Add($c) }
    }

    End {
        $CharacterClass = [PrimaryCharacterClass]@{
            Name = $Name;
            BitIndex = $BitIndex;
            FilterScript = $FilterScript;
            TestCharacters = ([char[]]($AllCharacters | Sort-Object -Unique -CaseSensitive));
        };
        if ($PSBoundParameters.ContainsKey('AddTo')) {
            $AddTo.Add($Name, $CharacterClass);
            if ($PassThru.IsPresent) { $CharacterClass | Write-Output }
        } else {
            $CharacterClass | Write-Output;
        }
    }
}

Function New-AggregateCharacterClass {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    [OutputType([AggregateCharacterClass])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[A-Z][a-z]+$')]
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 2)]
        [ScriptBlock]$FilterScript,

        [Parameter(ValueFromPipeline = $true)]
        [CharacterClass[]]$SubClasses,

        [Hashtable]$AddTo,

        [switch]$PassThru
    )
    
    Begin { $AllClasses = @{} }

    Process {
        if ($PSBoundParameters.ContainsKey('SubClasses')) {
            foreach ($c in $SubClasses) {
                if ($AllClasses.ContainsKey($c.Name)) {
                    if (-not [object]::ReferenceEquals($AllClasses[$c.Name], $c)) {
                        Write-Error -Message "Another sub-class with the name $($.Name) was already included.";
                    }
                } else {
                    $AllClasses.Add($c.Name, $c);
                }
            }
        }
    }

    End {
        $CharacterClass = [AggregateCharacterClass]@{
            Name = $Name;
            FilterScript = $FilterScript;
            SubClasses = ($AllClasses.Values | Sort-Object -Property @{ Expression = { $_.GetFlags() }});
        };
        if ($PSBoundParameters.ContainsKey('AddTo')) {
            $AddTo.Add($Name, $CharacterClass);
            if ($PassThru.IsPresent) { $CharacterClass | Write-Output }
        } else {
            $CharacterClass | Write-Output;
        }
    }
}

Function Add-SimilarCharacterClass {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [PrimaryCharacterClass]$CharacterClass,
        
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputObject
    )
}

Function ConvertTo-ExportableCharacterClass {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputObject
    )

    Process {
        foreach ($CharacterClass in $InputObject) {
            $Item =  [PSCustomObject]@{
                Name = $CharacterClass.Name;
                FilterScript = $CharacterClass.FilterScript;
                RelatedClasses = ([string[]]@($CharacterClass.RelatedClasses | ForEach-Object { $_.Name }));
            };
            if  ($CharacterClass -is [PrimaryCharacterClass]) {
                $Item | Add-Member -MemberType NoteProperty -Name 'BitIndex' -Value $CharacterClass.BitIndex;
                $Item | Add-Member -MemberType NoteProperty -Name 'TestCharacters' -Value $CharacterClass.TestCharacters -PassThru;
            } else {
                $Item | Add-Member -MemberType NoteProperty -Name 'SubClasses' -Value ([string[]]@($CharacterClass.SubClasses | ForEach-Object { $_.Name })) -PassThru;
            }
        }
    }
}

Function ConvertFrom-ImportedCharacterClass {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSObject[]]$InputObject,
        
        [Parameter(Mandatory = $true)]
        [Hashtable]$AllCharacterClasses
    )

    Begin {
        $AggregateMap = @{};
    }

    Process {
        foreach ($obj in $InputObject) {
            if ($null -ne $obj.BitIndex) {
                New-PrimaryCharacterClass -Name $obj.Name -BitIndex $obj.BitIndex -FilterScript $obj.FilterScript -InputCharacters $obj.TestCharacters -AddTo $AllCharacterClasses;
            } else {
                New-AggregateCharacterClass -Name $obj.Name -FilterScript $obj.FilterScript -AddTo $AllCharacterClasses;
                $AggregateMap[$obj.Name] = $obj.SubClasses;
            }
        }
    }
    
    End {
        foreach ($Name in $AggregateMap.Keys) {
            [AggregateCharacterClass]$CharacterClass = $AllCharacterClasses[$Name];
            $SubClasses = @();
            foreach ($n in $AggregateMap[$Name]) {
                if ($AllCharacterClasses.ContainsKey($n)) {
                    $SubClasses += $AllCharacterClasses[$n];
                } else {
                    Write-Error -Message "$Name contains a reference to non-existent class $n" -Category InvalidArgument -ErrorId 'CharacterClassNotFound' -TargetObject $n -ErrorAction Stop;
                }
            }
            $CharacterClass.SubClasses = ($SubClasses | Sort-Object -Property @{ Expression = { $_.GetFlags() }});
        }
    }
}

Function Import-CharacterClasses {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [Hashtable]$AllCharacterClasses,

        [switch]$Force
    )

    $Path = $PSScriptRoot | Join-Path -ChildPath 'temp.xml';
    if (($Path |Test-Path -PathType Leaf) -and -not $Force.IsPresent) {
        ((Import-Clixml -LiteralPath $Path -ErrorAction Stop) | ConvertFrom-ImportedCharacterClass -AllCharacterClasses $AllCharacterClasses -ErrorAction Stop) | Sort-Object -Property @{ Expression = { $_.GetFlags() } }
    } else {
        New-PrimaryCharacterClass -Name 'HexLetterUpper'                -BitIndex 0 -FilterScript { [char]::IsAsciiHexDigitUpper($_) -and -not [char]::IsAsciiDigit($_) } `
            -InputCharacters 'A', 'B', 'C', 'D', 'E', 'F' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'HexLetterLower'                -BitIndex 1 -FilterScript { [char]::IsAsciiHexDigitLower($_) -and -not [char]::IsAsciiDigit($_) } `
            -InputCharacters 'a', 'b', 'c', 'd', 'e', 'f' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonHexAsciiLetterUpper'        -BitIndex 2 -FilterScript { [char]::IsAsciiLetterUpper($_) -and -not [char]::IsAsciiHexDigitUpper($_) } `
            -InputCharacters 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiLetterUpper'           -BitIndex 3 -FilterScript  { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterUpper($_) } `
            -InputCharacters 'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonHexAsciiLetterLower'        -BitIndex 4 -FilterScript { [char]::IsAsciiLetterLower($_) -and -not [char]::IsAsciiHexDigitLower($_) } `
            -InputCharacters 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiLetterLower'           -BitIndex 5 -FilterScript { [char]::IsLower($_) -and -not [char]::IsAsciiLetterLower($_) } `
            -InputCharacters 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'TitlecaseLetter'               -BitIndex 6 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::TitlecaseLetter } `
            -InputCharacters 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'ModifierLetter'                -BitIndex 7 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierLetter } `
            -InputCharacters 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'OtherLetter'                   -BitIndex 8 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherLetter } `
            -InputCharacters 'ª', 'ƻ', 'ǁ', 'ǂ', 'ח', 'ך', 'נ' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'BinaryDigitNumber'             -BitIndex 9 -FilterScript { $_ -eq '0' -or $_ -eq '1' } `
            -InputCharacters '0', '1' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonBinaryOctalDigit'           -BitIndex 10 -FilterScript { $_ -gt '1' -and $_ -le '7' } `
            -InputCharacters '2', '3', '4', '5', '6', '7' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonOctalAsciiDigit'            -BitIndex 11 -FilterScript { $_ -eq '8' -or $_ -eq '9' } `
            -InputCharacters '8', '9' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiDigit'                 -BitIndex 12 -FilterScript { [char]::IsDigit($_) -and -not [char]::IsAsciiDigit($_) } `
            -InputCharacters '٤', '۶', '۸', '߁', '߂', '߃', '߄' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'LetterNumber'                  -BitIndex 13 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LetterNumber } `
            -InputCharacters 'ᛯ', 'ᛰ', "`u{2160}", 'Ⅱ', 'Ⅳ', "`u{2164}", "`u{2169}", "`u{216c}", "`u{216d}", "`u{216e}", "`u{216f}", "`u{2170}", 'ⅱ', 'ⅳ', "`u{2174}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'OtherNumber'                   -BitIndex 14 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNumber } `
            -InputCharacters '²', '¾', '৹', '௱', '౹', '౺' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonSpacingMark'                -BitIndex 15 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::NonSpacingMark } `
            -InputCharacters "`u{0300}", "`u{0308}", "`u{0310}", "`u{0314}", "`u{0317}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'SpacingCombiningMark'          -BitIndex 16 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpacingCombiningMark } `
            -InputCharacters "`u{0903}", "`u{093b}", "`u{0949}", "`u{094f}", "`u{0982}", "`u{09be}", "`u{09c8}", "`u{0a3e}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'EnclosingMark'                 -BitIndex 17 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::EnclosingMark } `
            -InputCharacters "`u{1abe}", "`u{20dd}", "`u{20de}", "`u{20df}", "`u{20e0}", "`u{20e2}", "`u{20e3}", "`u{20e4}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiConnectorPunctuation'     -BitIndex 18 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($_) } `
            -InputCharacters '_' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiConnectorPunctuation'  -BitIndex 19 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($_) } `
            -InputCharacters '‿', '⁀', '⁔', '︳', '︴' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiDashPunctuation'          -BitIndex 20 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($_) } `
            -InputCharacters '-' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiDashPunctuation'       -BitIndex 21 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($_) } `
            -InputCharacters '֊', '־', '᠆', '—', '⸺', '〰', '﹣' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiOpenPunctuation'          -BitIndex 22 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($_) } `
            -InputCharacters '(', '[', '{' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiOpenPunctuation'       -BitIndex 23 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($_) } `
            -InputCharacters '„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiClosePunctuation'         -BitIndex 24 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($_) } `
            -InputCharacters ')', ']', '}' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiClosePunctuation'      -BitIndex 25 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($_) } `
            -InputCharacters '⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'InitialQuotePunctuation'       -BitIndex 26 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::InitialQuotePunctuation } `
            -InputCharacters '«', '“', '‟', '⸂', '⸄', '⸉', '⸌', '⸜', '⸠' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'FinalQuotePunctuation'         -BitIndex 27 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::FinalQuotePunctuation } `
            -InputCharacters '»', '”', '⸃', '⸅', '⸊', '⸍', '⸝', '⸡' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'OtherAsciiPunctuation'         -BitIndex 28 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($_) } `
            -InputCharacters '!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'OtherNonAsciiPunctuation'      -BitIndex 29 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($_) } `
            -InputCharacters '¡', '§', '¿', '՟', '׆', '؉', '؛', '؝' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiMathSymbol'               -BitIndex 30 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and [char]::IsAscii($_) } `
            -InputCharacters '+', '<', '=', '>', '|', '~' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiMathSymbol'            -BitIndex 31 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and -not [char]::IsAscii($_) } `
            -InputCharacters '±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiCurrencySymbol'           -BitIndex 32 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and [char]::IsAscii($_) } `
            -InputCharacters '$' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiCurrencySymbol'        -BitIndex 33 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and -not [char]::IsAscii($_) } `
            -InputCharacters '¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiModifierSymbol'           -BitIndex 34 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and [char]::IsAscii($_) } `
            -InputCharacters '^', '`' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiModifierSymbol'        -BitIndex 35 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and -not [char]::IsAscii($_) } `
            -InputCharacters '˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'OtherSymbol'                   -BitIndex 36 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherSymbol -and -not [char]::IsAscii($_) } `
            -InputCharacters '©', '®', '°', '҂', '۞', '۩', '۾', '߶', '৺', '୰', '౿' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiSpaceSeparator'           -BitIndex 37 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($_) } `
            -InputCharacters ' ' -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiSpaceSeparator'        -BitIndex 38 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($_) } `
            -InputCharacters "`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'LineSeparator'                 -BitIndex 39 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LineSeparator -and -not [char]::IsAscii($_) } `
            -InputCharacters "`u{2028}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'ParagraphSeparator'            -BitIndex 40 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ParagraphSeparator -and -not [char]::IsAscii($_) } `
            -InputCharacters "`u{2029}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'AsciiControl'                  -BitIndex 41 -FilterScript { [char]::IsControl($_) -and [char]::IsAscii($_) } `
            -InputCharacters "`t", "`n", "`v", "`f", "`r" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'NonAsciiControl'               -BitIndex 42 -FilterScript { [char]::IsControl($_) -and -not [char]::IsAscii($_) } `
            -InputCharacters "`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'Format'                        -BitIndex 43 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::Format } `
            -InputCharacters "`u{00ad}", "`u{0601}", "`u{0605}", "`u{061c}", "`u{070f}", "`u{200d}", "`u{2060}", "`u{206a}", "`u{206f}", "`u{feff}", "`u{fffb}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'HighSurrogate'                 -BitIndex 44 -FilterScript { [char]::IsHighSurrogate($_) } `
            -InputCharacters "`u{d800}", "`u{d803}", "`u{d806}", "`u{d809}", "`u{d80c}", "`u{d80f}", "`u{d812}", "`u{d815}", "`u{d818}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'LowSurrogate'                  -BitIndex 45 -FilterScript { [char]::IsLowSurrogate($_) } `
            -InputCharacters "`u{dc00}", "`u{dc02}", "`u{dc05}", "`u{dc08}", "`u{dc0b}", "`u{dc0e}", "`u{dc11}", "`u{dc14}", "`u{dc17}", "`u{dc18}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'PrivateUse'                    -BitIndex 46 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::PrivateUse } `
            -InputCharacters "`u{e002}", "`u{e005}", "`u{e008}", "`u{e00b}", "`u{e00e}", "`u{e011}", "`u{e014}", "`u{e017}", "`u{e018}" -AddTo $AllCharacterClasses;
        New-PrimaryCharacterClass -Name 'OtherNotAssigned'              -BitIndex 47 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNotAssigned } `
            -InputCharacters "`u{0378}", "`u{0381}", "`u{038b}", "`u{0530}", "`u{058b}", "`u{05c8}", "`u{05cb}", "`u{05ce}", "`u{05ec}" -AddTo $AllCharacterClasses;
        
        New-AggregateCharacterClass -Name 'HexLetter' -FilterScript { [char]::IsAsciiHexDigit($_) -and -not [char]::IsAsciiDigit($_) } `
            -SubClasses $AllCharacterClasses['HexLetterUpper'], $AllCharacterClasses['HexLetterLower'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiLetterUpper' -FilterScript { [char]::IsAsciiLetterUpper($_) } `
            -SubClasses $AllCharacterClasses['HexLetterUpper'], $AllCharacterClasses['NonHexAsciiLetterUpper'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'UppercaseLetter' -FilterScript { [char]::IsLower($_) -and -not [char]::IsAsciiLetterUpper($_) } `
            -SubClasses $AllCharacterClasses['AsciiLetterUpper'], $AllCharacterClasses['NonAsciiLetterUpper'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiLetterLower' -FilterScript { [char]::IsAsciiLetterLower($_) } `
            -SubClasses $AllCharacterClasses['HexLetterLower'], $AllCharacterClasses['NonHexAsciiLetterLower'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonHexAsciiLetter' -FilterScript { [char]::IsAsciiLetter($_) -and -not [char]::IsAsciiHexDigit($_) } `
            -SubClasses $AllCharacterClasses['NonHexAsciiLetterUpper'], $AllCharacterClasses['NonHexAsciiLetterLower'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiLetter' -FilterScript { [char]::IsAsciiLetter($_) } `
            -SubClasses $AllCharacterClasses['HexLetterUpper'], $AllCharacterClasses['HexLetterLower'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'LowercaseLetter' -FilterScript { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterLower($_) } `
            -SubClasses $AllCharacterClasses['AsciiLetterLower'], $AllCharacterClasses['NonAsciiLetterLower'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiLetter' -FilterScript { [char]::IsLetter($_) -and -not [char]::IsAsciiLetter($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiLetterUpper'], $AllCharacterClasses['NonAsciiLetterLower'], $AllCharacterClasses['TitlecaseLetter'], $AllCharacterClasses['ModifierLetter'], $AllCharacterClasses['OtherLetter'] `
            -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Letter' -FilterScript { [char]::IsLetter($_) } `
            -SubClasses $AllCharacterClasses['AsciiLetter'], $AllCharacterClasses['NonAsciiLetter'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'OctalDigit' -FilterScript { $_ -ge '0' -and $_ -le '7' } `
            -SubClasses $AllCharacterClasses['BinaryDigitNumber'], $AllCharacterClasses['NonBinaryOctalDigit'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiDigit' -FilterScript { [char]::IsAsciiDigit($_) } `
            -SubClasses $AllCharacterClasses['OctalDigit'], $AllCharacterClasses['NonOctalAsciiDigit'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiHexDigitUpper' -FilterScript { [char]::IsAsciiHexDigitUpper($_) } `
            -SubClasses $AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['HexLetterUpper'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiHexDigitLower' -FilterScript { [char]::IsAsciiHexDigitLower($_) } `
            -SubClasses $AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['HexLetterLower'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiHexDigit' -FilterScript { [char]::IsAsciiHexDigit($_) } `
            -SubClasses $AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['HexLetter'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiLetterOrDigit' -FilterScript { [char]::IsAsciiLetterOrDigit($_) } `
            -SubClasses $AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['AsciiLetter'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiLetterOrDigit' -FilterScript { [char]::IsAsciiLetterOrDigit($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiDigit'], $AllCharacterClasses['NonAsciiLetter'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Digit' -FilterScript { [char]::IsDigit($_) } `
            -SubClasses $AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['NonAsciiDigit'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'LetterOrDigit' -FilterScript { [char]::IsLetterOrDigit($_) } `
            -SubClasses $AllCharacterClasses['AsciiLetterOrDigit'], $AllCharacterClasses['NonAsciiLetterOrDigit'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiNumber' -FilterScript { [char]::IsNumber($_) -and -not [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiDigit'], $AllCharacterClasses['LetterNumber'], $AllCharacterClasses['OtherNumber'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Number' -FilterScript { [char]::IsNumber($_) } `
            -SubClasses $AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['NonAsciiNumber'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Mark' -FilterScript { switch ([char]::GetUnicodeCategory($_)) { NonSpacingMark { return $true } SpacingCombiningMark { return $true } EnclosingMark { return $true } default { return $false } } } `
            -SubClasses $AllCharacterClasses['NonSpacingMark'], $AllCharacterClasses['SpacingCombiningMark'], $AllCharacterClasses['EnclosingMark'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'ConnectorPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation } `
            -SubClasses $AllCharacterClasses['AsciiConnectorPunctuation'], $AllCharacterClasses['NonAsciiConnectorPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'DashPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation } `
            -SubClasses $AllCharacterClasses['AsciiDashPunctuation'], $AllCharacterClasses['NonAsciiDashPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'OpenPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation } `
            -SubClasses $AllCharacterClasses['AsciiOpenPunctuation'], $AllCharacterClasses['NonAsciiOpenPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'ClosePunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation } `
            -SubClasses $AllCharacterClasses['AsciiClosePunctuation'], $AllCharacterClasses['NonAsciiClosePunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiPunctuation' -FilterScript { [char]::IsPunctuation($_) -and [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['AsciiConnectorPunctuation'], $AllCharacterClasses['AsciiDashPunctuation'], $AllCharacterClasses['AsciiOpenPunctuation'], $AllCharacterClasses['AsciiClosePunctuation'],
            $AllCharacterClasses['OtherAsciiPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiPunctuation' -FilterScript { [char]::IsPunctuation($_) -and -not [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiConnectorPunctuation'], $AllCharacterClasses['NonAsciiDashPunctuation'], $AllCharacterClasses['NonAsciiOpenPunctuation'], $AllCharacterClasses['NonAsciiClosePunctuation'],
                $AllCharacterClasses['InitialQuotePunctuation'], $AllCharacterClasses['FinalQuotePunctuation'], $AllCharacterClasses['OtherNonAsciiPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'OtherPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation } `
            -SubClasses $AllCharacterClasses['OtherAsciiPunctuation'], $AllCharacterClasses['OtherNonAsciiPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Punctuation' -FilterScript { [char]::IsPunctuation($_) } `
            -SubClasses $AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['NonAsciiPunctuation'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'MathSymbol' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol } `
            -SubClasses $AllCharacterClasses['AsciiMathSymbol'], $AllCharacterClasses['NonAsciiMathSymbol'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'CurrencySymbol' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol } `
            -SubClasses $AllCharacterClasses['AsciiCurrencySymbol'], $AllCharacterClasses['NonAsciiCurrencySymbol'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiSymbol' -FilterScript { [char]::IsSymbol($_) -and [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['AsciiMathSymbol'], $AllCharacterClasses['AsciiCurrencySymbol'], $AllCharacterClasses['AsciiModifierSymbol'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'ModifierSymbol' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol } `
            -SubClasses $AllCharacterClasses['AsciiModifierSymbol'], $AllCharacterClasses['NonAsciiModifierSymbol'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiSymbol' -FilterScript { [char]::IsSymbol($_) -and -not [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiMathSymbol'], $AllCharacterClasses['NonAsciiCurrencySymbol'], $AllCharacterClasses['NonAsciiModifierSymbol'], $AllCharacterClasses['OtherSymbol'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Symbol' -FilterScript { [char]::IsSymbol($_) } `
            -SubClasses $AllCharacterClasses['AsciiSymbol'], $AllCharacterClasses['NonAsciiSymbol'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'SpaceSeparator' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator } `
            -SubClasses $AllCharacterClasses['AsciiSpaceSeparator'], $AllCharacterClasses['NonAsciiSpaceSeparator'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiSeparator' -FilterScript { [char]::IsSeparator($_) -and -not [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiSpaceSeparator'], $AllCharacterClasses['LineSeparator'], $AllCharacterClasses['ParagraphSeparator'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Separator' -FilterScript { [char]::IsSeparator($_) } `
            -SubClasses $AllCharacterClasses['AsciiSpaceSeparator'], $AllCharacterClasses['NonAsciiSeparator'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'AsciiWhiteSpace' -FilterScript { [char]::IsWhiteSpace($_) -and [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['AsciiSpaceSeparator'], $AllCharacterClasses['AsciiControl'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Ascii' -FilterScript { [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['AsciiLetterOrDigit'], $AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['AsciiSymbol'], $AllCharacterClasses['AsciiWhiteSpace'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAsciiWhiteSpace' -FilterScript { [char]::IsWhiteSpace($_) -and -not [char]::IsAscii($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiSeparator'], $AllCharacterClasses['NonAsciiControl'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Control' -FilterScript { [char]::IsControl($_) } `
            -SubClasses $AllCharacterClasses['AsciiControl'], $AllCharacterClasses['NonAsciiControl'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'WhiteSpace' -FilterScript { [char]::IsWhiteSpace($_) } `
            -SubClasses $AllCharacterClasses['AsciiWhiteSpace'], $AllCharacterClasses['NonAsciiWhiteSpace'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'Surrogate' -FilterScript { [char]::IsSurrogate($_) } `
            -SubClasses $AllCharacterClasses['HighSurrogate'], $AllCharacterClasses['LowSurrogate'] -AddTo $AllCharacterClasses;
        New-AggregateCharacterClass -Name 'NonAscii' -FilterScript { [char]::IsSurrogate($_) } `
            -SubClasses $AllCharacterClasses['NonAsciiLetterOrDigit'], $AllCharacterClasses['Mark'], $AllCharacterClasses['NonAsciiPunctuation'], $AllCharacterClasses['NonAsciiSymbol'], $AllCharacterClasses['NonAsciiWhiteSpace'],
                $AllCharacterClasses['Format'], $AllCharacterClasses['Surrogate'], $AllCharacterClasses['PrivateUse'], $AllCharacterClasses['OtherNotAssigned'] -AddTo $AllCharacterClasses;
            
        ($AllCharacterClasses['AsciiHexDigitLower'], $AllCharacterClasses['NonHexAsciiLetterUpper'], $AllCharacterClasses['NonAsciiLetterUpper']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['HexLetterUpper'];
        ($AllCharacterClasses['AsciiHexDigitUpper'], $AllCharacterClasses['NonHexAsciiLetterLower'], $AllCharacterClasses['NonAsciiLetterLower']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['HexLetterLower'];
        ($AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['NonHexAsciiLetterLower'], $AllCharacterClasses['NonAsciiLetterUpper']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonHexAsciiLetterUpper'];
        ($AllCharacterClasses['AsciiLetterUpper'], $AllCharacterClasses['NonAsciiLetterLower'], $AllCharacterClasses['TitlecaseLetter'], $AllCharacterClasses['ModifierLetter'], $AllCharacterClasses['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiLetterUpper'];
        ($AllCharacterClasses['NonHexAsciiLetterUpper'], $AllCharacterClasses['TitlecaseLetter'], $AllCharacterClasses['ModifierLetter'], $AllCharacterClasses['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonHexAsciiLetterLower'];
        ($AllCharacterClasses['AsciiLetterUpper'], $AllCharacterClasses['AsciiLetterLower']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiLetterLower'];
        ($AllCharacterClasses['UppercaseLetter'], $AllCharacterClasses['LowercaseLetter'], $AllCharacterClasses['ModifierLetter'], $AllCharacterClasses['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['TitlecaseLetter'];
        ($AllCharacterClasses['UppercaseLetter'], $AllCharacterClasses['LowercaseLetter'], $AllCharacterClasses['TitlecaseLetter'], $AllCharacterClasses['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['ModifierLetter'];
        ($AllCharacterClasses['UppercaseLetter'], $AllCharacterClasses['LowercaseLetter'], $AllCharacterClasses['TitlecaseLetter'], $AllCharacterClasses['ModifierLetter']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['OtherLetter'];
        ($AllCharacterClasses['NonBinaryOctalDigit'], $AllCharacterClasses['NonOctalAsciiDigit'], $AllCharacterClasses['HexLetter'], $AllCharacterClasses['NonAsciiDigit']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['BinaryDigitNumber'];
        ($AllCharacterClasses['BinaryDigitNumber'], $AllCharacterClasses['NonOctalAsciiDigit'], $AllCharacterClasses['HexLetter'], $AllCharacterClasses['NonAsciiDigit']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonBinaryOctalDigit'];
        ($AllCharacterClasses['OctalDigit'], $AllCharacterClasses['HexLetter'], $AllCharacterClasses['NonAsciiDigit']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonOctalAsciiDigit'];
        ($AllCharacterClasses['AsciiDigit'], $AllCharacterClasses['LetterNumber'], $AllCharacterClasses['OtherNumber']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiDigit'];
        ($AllCharacterClasses['Digit'], $AllCharacterClasses['OtherNumber']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['LetterNumber'];
        ($AllCharacterClasses['Digit'], $AllCharacterClasses['LetterNumber']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['OtherNumber'];
        ($AllCharacterClasses['SpacingCombiningMark'], $AllCharacterClasses['EnclosingMark']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonSpacingMark'];
        ($AllCharacterClasses['NonSpacingMark'], $AllCharacterClasses['EnclosingMark']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['SpacingCombiningMark'];
        ($AllCharacterClasses['NonSpacingMark'], $AllCharacterClasses['EnclosingMark']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['EnclosingMark'];
        ($AllCharacterClasses['AsciiDashPunctuation'], $AllCharacterClasses['AsciiOpenPunctuation'], $AllCharacterClasses['AsciiClosePunctuation'], $AllCharacterClasses['OtherAsciiPunctuation'], $AllCharacterClasses['NonAsciiPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiConnectorPunctuation'];
        ($AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['NonAsciiDashPunctuation'], $AllCharacterClasses['NonAsciiOpenPunctuation'], $AllCharacterClasses['NonAsciiClosePunctuation'], $AllCharacterClasses['OtherNonAsciiPunctuation'], $AllCharacterClasses['InitialQuotePunctuation'],
            $AllCharacterClasses['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiConnectorPunctuation'];
        ($AllCharacterClasses['AsciiConnectorPunctuation'], $AllCharacterClasses['AsciiOpenPunctuation'], $AllCharacterClasses['AsciiClosePunctuation'], $AllCharacterClasses['OtherAsciiPunctuation'], $AllCharacterClasses['NonAsciiPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiDashPunctuation'];
        ($AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['NonAsciiConnectorPunctuation'], $AllCharacterClasses['NonAsciiOpenPunctuation'], $AllCharacterClasses['NonAsciiClosePunctuation'], $AllCharacterClasses['OtherNonAsciiPunctuation'], $AllCharacterClasses['InitialQuotePunctuation'],
            $AllCharacterClasses['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiDashPunctuation'];
        ($AllCharacterClasses['AsciiConnectorPunctuation'], $AllCharacterClasses['AsciiDashPunctuation'], $AllCharacterClasses['AsciiClosePunctuation'], $AllCharacterClasses['OtherAsciiPunctuation'], $AllCharacterClasses['NonAsciiPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiOpenPunctuation'];
        ($AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['NonAsciiConnectorPunctuation'], $AllCharacterClasses['NonAsciiDashPunctuation'], $AllCharacterClasses['NonAsciiClosePunctuation'], $AllCharacterClasses['OtherNonAsciiPunctuation'], $AllCharacterClasses['InitialQuotePunctuation'],
            $AllCharacterClasses['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiOpenPunctuation'];
        ($AllCharacterClasses['AsciiConnectorPunctuation'], $AllCharacterClasses['AsciiDashPunctuation'], $AllCharacterClasses['AsciiOpenPunctuation'], $AllCharacterClasses['OtherAsciiPunctuation'], $AllCharacterClasses['NonAsciiPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiClosePunctuation'];
        ($AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['NonAsciiConnectorPunctuation'], $AllCharacterClasses['NonAsciiDashPunctuation'], $AllCharacterClasses['NonAsciiOpenPunctuation'], $AllCharacterClasses['OtherNonAsciiPunctuation'], $AllCharacterClasses['InitialQuotePunctuation'],
            $AllCharacterClasses['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiClosePunctuation'];
        ($AllCharacterClasses['ConnectorPunctuation'], $AllCharacterClasses['DashPunctuation'], $AllCharacterClasses['OpenPunctuation'], $AllCharacterClasses['ClosePunctuation'], $AllCharacterClasses['FinalQuotePunctuation'], $AllCharacterClasses['OtherPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['InitialQuotePunctuation'];
        ($AllCharacterClasses['ConnectorPunctuation'], $AllCharacterClasses['DashPunctuation'], $AllCharacterClasses['OpenPunctuation'], $AllCharacterClasses['ClosePunctuation'], $AllCharacterClasses['InitialQuotePunctuation'], $AllCharacterClasses['OtherPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['FinalQuotePunctuation'];
        ($AllCharacterClasses['AsciiConnectorPunctuation'], $AllCharacterClasses['AsciiDashPunctuation'], $AllCharacterClasses['AsciiOpenPunctuation'], $AllCharacterClasses['AsciiClosePunctuation'], $AllCharacterClasses['NonAsciiPunctuation']) `
            | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['OtherAsciiPunctuation'];
        ($AllCharacterClasses['AsciiPunctuation'], $AllCharacterClasses['NonAsciiConnectorPunctuation'], $AllCharacterClasses['NonAsciiDashPunctuation'], $AllCharacterClasses['NonAsciiOpenPunctuation'], $AllCharacterClasses['NonAsciiClosePunctuation'], $AllCharacterClasses['InitialQuotePunctuation'],
            $AllCharacterClasses['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['OtherNonAsciiPunctuation'];
        ($AllCharacterClasses['AsciiCurrencySymbol'], $AllCharacterClasses['AsciiModifierSymbol'], $AllCharacterClasses['NonAsciiSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiMathSymbol'];
        ($AllCharacterClasses['AsciiSymbol'], $AllCharacterClasses['NonAsciiCurrencySymbol'], $AllCharacterClasses['NonAsciiModifierSymbol'], $AllCharacterClasses['OtherSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiMathSymbol'];
        ($AllCharacterClasses['AsciiMathSymbol'], $AllCharacterClasses['AsciiModifierSymbol'], $AllCharacterClasses['NonAsciiSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiCurrencySymbol'];
        ($AllCharacterClasses['AsciiSymbol'], $AllCharacterClasses['NonAsciiMathSymbol'], $AllCharacterClasses['NonAsciiModifierSymbol'], $AllCharacterClasses['OtherSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiCurrencySymbol'];
        ($AllCharacterClasses['AsciiMathSymbol'], $AllCharacterClasses['AsciiCurrencySymbol'], $AllCharacterClasses['NonAsciiSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiModifierSymbol'];
        ($AllCharacterClasses['AsciiSymbol'], $AllCharacterClasses['NonAsciiMathSymbol'], $AllCharacterClasses['NonAsciiCurrencySymbol'], $AllCharacterClasses['OtherSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiModifierSymbol'];
        ($AllCharacterClasses['MathSymbol'], $AllCharacterClasses['CurrencySymbol'], $AllCharacterClasses['ModifierSymbol']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['OtherSymbol'];
        ($AllCharacterClasses['NonAsciiWhiteSpace']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiSpaceSeparator'];
        ($AllCharacterClasses['AsciiSpaceSeparator'], $AllCharacterClasses['LineSeparator'], $AllCharacterClasses['ParagraphSeparator'], $AllCharacterClasses['Control']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiSpaceSeparator'];
        ($AllCharacterClasses['SpaceSeparator'], $AllCharacterClasses['ParagraphSeparator'], $AllCharacterClasses['Control']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['LineSeparator'];
        ($AllCharacterClasses['SpaceSeparator'], $AllCharacterClasses['LineSeparator'], $AllCharacterClasses['Control']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['ParagraphSeparator'];
        ($AllCharacterClasses['Separator'], $AllCharacterClasses['NonAsciiControl']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['AsciiControl'];
        ($AllCharacterClasses['Separator'], $AllCharacterClasses['AsciiControl']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['NonAsciiControl']; 
        ($AllCharacterClasses['WhiteSpace'], $AllCharacterClasses['Surrogate']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['Format'];
        ($AllCharacterClasses['WhiteSpace'], $AllCharacterClasses['LowSurrogate']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['HighSurrogate'];
        ($AllCharacterClasses['WhiteSpace'], $AllCharacterClasses['HighSurrogate']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['LowSurrogate'];
        ($AllCharacterClasses['OtherNotAssigned'], $AllCharacterClasses['WhiteSpace']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['PrivateUse'];
        ($AllCharacterClasses['PrivateUse'], $AllCharacterClasses['WhiteSpace']) | Add-SimilarCharacterClass -CharacterClass $AllCharacterClasses['OtherNotAssigned'];

        [System.Linq.Enumerable]::Range([int]0, [int]65536) | ForEach-Object {
            [char]$c = $_;
            [int]$pct = [Math]::Floor((100.0 * $_) / 65536.0);
            if ($pct -ne $PercentComplete) {
                $PercentComplete = $pct;
                Write-Progress -Activity 'Hash Character Types' -Status "$($_ + 1) of 65536" -PercentComplete $pct;
            }
            [System.Globalization.UnicodeCategory]$Category = [char]::GetUnicodeCategory($c);
            $Key = $Category.ToString('F');
            switch ($Category) {
                Surrogate {
                    if ([char]::IsHighSurrogate($c)) {
                        $Key = 'HighSurrogate';
                    } else {
                        if ([char]::IsLowSurrogate($c)) { $Key = 'LowSurrogate' }
                    }
                    break;
                }
                UppercaseLetter {
                    if ([char]::IsAsciiHexDigitUpper($c)) {
                        $Key = 'HexLetterUpper';
                    } else {
                        if ([char]::IsAsciiLetterUpper($c)) { $Key = 'NonHexAsciiLetterUpper' } else {  $Key = 'NonAsciiLetterUpper' }
                    }
                    break;
                }
                LowercaseLetter {
                    if ([char]::IsAsciiHexDigitLower($c)) {
                        $Key = 'HexLetterLower';
                    } else {
                        if ([char]::IsAsciiLetterLower($c)) { $Key = 'NonHexAsciiLetterLower' } else {  $Key = 'NonAsciiLetterLower' }
                    }
                    break;
                }
                DecimalDigitNumber {
                    if ([char]::IsAsciiDigit($c)) {
                        if ($c -le '1') {
                            $Key = 'BinaryDigitNumber';
                        } else {
                            if ($c -gt '7') { $Key = 'NonOctalAsciiDigit' } else { $Key = 'NonBinaryOctalDigit' }
                        }
                    } else {
                        $Key = 'NonAsciiDigit';
                    }
                    break;
                }
                ConnectorPunctuation {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiConnectorPunctuation' } else { $Key = 'NonAsciiConnectorPunctuation' }
                    break;
                }
                DashPunctuation {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiDashPunctuation' } else { $Key = 'NonAsciiDashPunctuation' }
                    break;
                }
                OpenPunctuation {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiOpenPunctuation' } else { $Key = 'NonAsciiOpenPunctuation' }
                    break;
                }
                ClosePunctuation {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiClosePunctuation' } else { $Key = 'NonAsciiClosePunctuation' }
                    break;
                }
                OtherPunctuation {
                    if ([char]::IsAscii($c)) { $Key = 'OtherAsciiPunctuation' } else { $Key = 'OtherNonAsciiPunctuation' }
                    break;
                }
                MathSymbol {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiMathSymbol' } else { $Key = 'NonAsciiMathSymbol' }
                    break;
                }
                CurrencySymbol {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiCurrencySymbol' } else { $Key = 'NonAsciiCurrencySymbol' }
                    break;
                }
                ModifierSymbol {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiModifierSymbol' } else { $Key = 'NonAsciiModifierSymbol' }
                    break;
                }
                SpaceSeparator {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiSpaceSeparator' } else { $Key = 'NonAsciiSpaceSeparator' }
                    break;
                }
                Control {
                    if ([char]::IsAscii($c)) { $Key = 'AsciiControl' } else { $Key = 'NonAsciiControl' }
                    break;
                }
            }
            if ($AllCharacterClasses.ContainsKey($Key)) {
                $CharacterClass = $AllCharacterClasses[$Key];
                if ($CharacterClass -isnot [PrimaryCharacterClass]) {
                    Write-Error -Message "Character $($c | ConvertTo-PSLiteral) mapped to non-primary class $Key" -Category InvalidOperation -ErrorId 'InvalidKey' -TargetObject $c -ErrorAction Stop;
                }
                $CharacterClass.AllCharacters.Add($c);
            } else {
                Write-Error -Message "Character $($c | ConvertTo-PSLiteral) mapped to non-existent primary class $Key" -Category InvalidOperation -ErrorId 'InvalidKey' -TargetObject $c -ErrorAction Stop;
            }
        }
        Write-Progress -Activity 'Hash Character Types' -Status "65536 characters added" -PercentComplete 100 -Completed;
        $OrderedClasses = @($AllCharacterClasses.Values | Sort-Object -Property @{ Expression = { $_.GetFlags() } });
        $OrderedClasses | ConvertTo-ExportableCharacterClass | Export-Clixml -LiteralPath $Path -Force;
        $OrderedClasses | Write-Output;
    }
}

Function Get-CharacterClassPsCode {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputObject
    )

    Begin { $AllItems = @() }

    Process { $AllItems += $InputObject }

    End {
        $AllItems = @($AllItems | Sort-Object -Property @{ Expression = { $_.GetFlags() } });
        $LongestName = 1;
        $AllItems | ForEach-Object {
            $Len = $_.Name.Length;
            if ($Len -gt $LongestName) { $LongestName = $Len }
        }
        $LongestName++;
        [char]$sp = ' ';

        (
            'Enum CharacterClass : ulong {',
            "    # $($AllItems[0].FilterScript.ToString().Trim())",
            "    $($AllItems[0].Name) =$([string]::new($sp, $LongestName - $AllItems[0].Name.Length))0x$($AllItems[0].GetFlags().ToString('x13'));"
        ) | Write-Output;
        
        ($AllItems | Select-Object -Skip 1) | ForEach-Object {
            (
                '',
                "    # $($_.FilterScript.ToString().Trim())",
                "    $($_.Name) =$([string]::new($sp, $LongestName - $_.Name.Length))0x$($_.GetFlags().ToString('x13'));"
            ) | Write-Output;
        }
        '}' | Write-Output;
    }
}

Function Write-PesterItStatement {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char]$Matching,
        
        [Parameter(Mandatory = $true)]
        [bool]$ShouldBe,
        
        [Parameter(Mandatory = $true)]
        [string]$Flags,
        
        [string]$Description,
        
        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer,

        [switch]$IsNot
    )

    Begin {
        $Desc = $Description;
        if (-not $PSBoundParameters.ContainsKey('Description')) { $Desc = $Flags }
        $AllChars = [System.Collections.ObjectModel.Collection[char]]::new();
    }

    Process { $AllChars.Add($Matching) }

    End {
        $Writer.WriteLine();
        $AllChars = @($AllChars | Sort-Object -CaseSensitive);
        switch ($AllChars.Count) {
            1 {
                $Writer.Write("        It '$(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes)) should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            $Actual = Test-CharacterClass -Value ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes));
                $Writer.WriteLine(';');
                $Writer.WriteLine('        }');
                break;
            }
            2 {
                $Writer.Write("        It '$(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes)) and $(($AllChars[1] | ConvertTo-PSLiteral -ForceDoubleQuotes)) should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            $Actual = Test-CharacterClass -Value ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes));
                $Writer.WriteLine(';');
                $Writer.Write('            $Actual = Test-CharacterClass -Value ');
                $Writer.Write(($AllChars[1] | ConvertTo-PSLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[1] | ConvertTo-PSLiteral -ForceDoubleQuotes));
                $Writer.WriteLine(';');
                $Writer.WriteLine('        }');
                break;
            }
            default {
                $Writer.Write("        It '$Desc characters should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            foreach ($c in @(([char[]](');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral));
                foreach ($c in ($AllChars | Select-Object -Skip 1)) {
                    $Writer.Write(', ');
                    $Writer.Write(($c | ConvertTo-PSLiteral));
                }
                $Writer.WriteLine(', "")))) {');
                $Writer.Write('                $Actual = Test-CharacterClass -Value $c');
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('                $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(" -Because `"0x([int]`$c).ToString('x4')`";");
                $Writer.WriteLine('            }');
                $Writer.WriteLine('        }');
                break;
            }
        }
    }
}

Function Write-PesterDescribeStatement {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass]$InputClass,

        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer
    )
    
    Process {
        $Writer.WriteLine();
        $Writer.Write("Describe 'Test-CharacterClass -Flags ");
        $Writer.Write($InputClass.Name);
        $Writer.WriteLine("' {");
        $Writer.Write("    Context 'IsNot.Present = `$false' {");
        [char[]]$TestCharacters = ($InputClass | Get-TestCharacters | Sort-Object -CaseSensitive);
        Write-PesterItStatement -Matching $TestCharacters -ShouldBe $true -Flags $InputClass.Name -Writer $Writer;
        $AllChars = [System.Collections.ObjectModel[char]]::new();
        $TestCharacters | ForEach-Object { $AllChars.Add($_) }
        $AllRelatedClases = @($InputClass | Get-RelatedClasses);
        foreach ($RelatedClass in $AllRelatedClases) {
            [char[]]$Matching = ($RelatedClass | Get-TestCharacters);
            Write-PesterItStatement -Matching $Matching -ShouldBe $false -Flags $RelatedClass.Name -Writer $Writer;
            $Matching | ForEach-Object { $AllChars.Add($_) }
        }
        [char[]]$Other = @($Script:CommonCharacters | Where-Object { $AllChars -cnotcontains $_ });
        Write-PesterItStatement -Matching $Other -ShouldBe $false -Flags 'Other' -Writer $Writer;
        $Writer.WriteLine("    }");
        $Writer.WriteLine();
        $Writer.Write("    Context 'IsNot.Present = `$true' {");
        Write-PesterItStatement -Matching $TestCharacters -ShouldBe $false -Flags $InputClass.Name -Writer $Writer;
        foreach ($RelatedClass in $AllRelatedClases) {
            Write-PesterItStatement -Matching ($RelatedClass | Get-TestCharacters) -ShouldBe $true -Flags $RelatedClass.Name -Writer $Writer;
        }
        Write-PesterItStatement -Matching $Other -ShouldBe $true -Flags 'Other' -Writer $Writer;
        $Writer.WriteLine("    }");
        $Writer.WriteLine("}");
    }
}

Function Write-CharacterClassTestCode {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputClass,
        
        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer
    )

    Begin { $AllItems = @() }

    Process { $AllItems += $InputClass }
    
    End {
        $Writer.WriteLine("Import-Module -Name (`$PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;");
        $Writer.WriteLine();
        $Writer.WriteLine('<#');
        $Writer.WriteLine('Import-Module Pester');
        $Writer.WriteLine('#>');
        ($AllItems | Sort-Object -Property @{ Expression = { $_.GetFlags() } }) | Write-PesterDescribeStatement;
    }
}

$Script:CommonCharacters = ([char[]]@("`t", "`n", ' ', '0', '9', 'A', 'F', 'N', 'Z', 'a', 'f', 'n', 'z', '_', '(', '[', ']', '}', '+', '=', '$','^', '`', '˅', '˥',  '¢', '£', '⁀', '︴', '٤', '߁', 'À', 'Ç', 'µ',
    'ß', 'ǅ', 'ᾫ', '©', '°', 'ʺ', 'ˇ', 'ǂ', 'ח', "`u{2160}", 'Ⅱ', '²', '¾', '֊', '־', '〰', '„', '⁅', '⁆', '⁾', '«', '“', '»', '”', '¡', '§', '±', '⅀', "`u{008c}", "`u{0090}", "`u{d806}", "`u{dc14}",
    "`u{0308}", "`u{0310}", "`u{0982}", "`u{09be}", "`u{20e2}", "`u{20e3}", "`u{2000}", "`u{3000}", "`u{2028}", "`u{2029}", "`u{0605}", "`u{fffb}", "`u{e00b}", "`u{e00e}", "`u{05ce}", "`u{05ec}"));

$AllCharacterClasses = @{};
[CharacterClass[]]$OrderedCharacterClasses = @(Import-CharacterClasses -AllCharacterClasses $AllCharacterClasses);

$OrderedCharacterClasses | Get-CharacterClassPsCode;

$Writer = [System.IO.StreamWriter]::new(($PSScriptRoot | Join-Path -ChildPath 'Test-CharacterClass.tests.ps1'), $false, [System.Text.Encoding.Utf8Encoding]::new($false, $false));
try {
    $OrderedCharacterClasses | Write-CharacterClassTestCode -Writer $Writer;
    $Writer.Flush();
} finally {
    $Writer.Close();
}