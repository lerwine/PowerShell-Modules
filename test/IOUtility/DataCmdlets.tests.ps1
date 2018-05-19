Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.IOUtility') -ErrorAction Stop;

Function New-TestArg {
    [CmdletBinding(DefaultParameterSetName = 'Named')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Named')]
        [string]$Name,

        [Parameter(Mandatory = $true, ParameterSetName = 'Positional')]
        [Parameter(ParameterSetName = 'Named')]
        [ScriptBlock]$Value,

        [Parameter(Mandatory = $true, ParameterSetName = 'Positional')]
        [string]$Key,

        [Parameter(ParameterSetName = 'Named')]
        [string]$Alias,

        [ScriptBlock]$Expected
    )

    <#
        Name: string | ""
        Key: string
        ValueCode: string | null
        ExpectedCode: string
        ExpectedValue: any
    #>
    $Properties = @{ };
    if ($PSBoundParameters.ContainsKey('Key')) {
        $Properties['Name'] = '';
        $Properties['Key'] = $Key;
    } else {
        $Properties['Key'] = $Name;
        if ($PSBoundParameters.ContainsKey('Alias')) { $Properties['Name'] = $Alias } else { $Properties['Name'] = $Name }
    }
    if ($PSBoundParameters.ContainsKey('Value')) {
        $s = $Value.ToString().Trim();
        if ($s.Length -eq 0) { $Properties['ValueCode'] = '$null' } else { $Properties['ValueCode'] = $s }
    } else {
        $Properties['ValueCode'] = $null;
    }
    
    if ($PSBoundParameters.ContainsKey('Expected')) {
        $Properties['ExpectedCode'] = $Expected.ToString().Trim();
        $Properties['ExpectedValue'] = $Expected.InvokeReturnAsIs();
    } else {
        if ($PSBoundParameters.ContainsKey('Value')) {
            $Properties['ExpectedCode'] = $Properties['ValueCode'];
            $Properties['ExpectedValue'] = $Value.InvokeReturnAsIs();
        } else {
            $Properties['ExpectedCode'] = '$true';
            $Properties['ExpectedValue'] = $true;
        }
    }
    (New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties) | Add-Member -MemberType CodeMethod -Name 'ToString' -Value {
        if ($this.Name.Length -eq 0) { return $this.ValueCode }
        if ($this.ValueCode -eq $null) { return "-$($this.Name)" }
        return "-$($this.Name) $($this.ValueCode)"
    } -Force -PassThru;
}

Function Add-TestArg {
    [CmdletBinding(DefaultParameterSetName = 'Named')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Named')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Obj')]
        [AllowEmptyString()]
        [string]$Name,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Obj')]
        [AllowNull()]
        [string]$ValueCode,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Obj')]
        [string]$ExpectedCode,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Obj')]
        [AllowEmptyCollection()]
        [AllowEmptyString()]
        [AllowNull()]
        [object]$ExpectedValue,

        [Parameter(Mandatory = $true, ParameterSetName = 'Positional')]
        [Parameter(ParameterSetName = 'Named')]
        [ScriptBlock]$Value,

        [Parameter(Mandatory = $true, ParameterSetName = 'Positional')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Obj')]
        [string]$Key,

        [Parameter(ParameterSetName = 'Named')]
        [string]$Alias,

        [Parameter(ParameterSetName = 'Named')]
        [Parameter(ParameterSetName = 'Positional')]
        [ScriptBlock]$Expected,

        [psobject]$ArgSet,

        [Parameter(ParameterSetName = 'Named')]
        [Parameter(ParameterSetName = 'Positional')]
        [switch]$ValueOnly
    )

    Begin {
        $CurrentArgSet = $ArgSet;
        if (-not $PSBoundParameters.ContainsKey('ArgSet')) {
            $CurrentArgSet = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Ordered = $Prev.Ordered; ByName = (New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]'); };
        }
    }

    Process {
        $Item = $null;
        if ($PSBoundParameters.ContainsKey('ValueCode')) {
            $Item = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                Name = $Name;
                Key = $Key;
                ValueCode = $ValueCode;
                ExpectedCode = $ExpectedCode;
                ExpectedValue = $ExpectedValue;
            }
        } else {
            $splat = @{};
            foreach ($k in $PSBoundParameters.Keys) {
                if ($k -ine 'ValueOnly' -and $k -ine 'ArgSet') { $splat[$k] = $PSBoundParameters[$k] }
            }
            $Item = New-TestArg @splat;
        }
        if (-not $ValueOnly) { $CurrentArgSet.Ordered.Add($Item); }
        $CurrentArgSet.ByName[$Item.Key] = $Item;
    }

    End { $CurrentArgSet | Write-Output; }
    <#
        Name: string | ""
        Key: string
        ValueCode: string | null
        ExpectedCode: string
        ExpectedValue: any
    #>
}


Describe 'Testing New-DataTable Function', {
    @(
        @(),
        @((New-TestArg -Value {'MyTable'} -Key 'TableName')),
        @((New-TestArg -Name 'TableName' -Value {'MyTable'})),
        @((New-TestArg -Name 'Name' -Value {'MyTable'} -Alias 'TableName')),
        @((New-TestArg -Name 'TableName' -Value {'MyTable'}), (New-TestArg -Name 'CaseSensitive')),
        @((New-TestArg -Name 'Name' -Value {'MyTable'} -Alias 'TableName'), (New-TestArg -Name 'CaseSensitive')),
        @((New-TestArg -Name 'TableName' -Value {'MyTable'}), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg')),
        @((New-TestArg -Name 'Name' -Value {'MyTable'} -Alias 'TableName'), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg' -Alias 'Namespace')),
        @((New-TestArg -Name 'TableName' -Value {'MyTable'}), (New-TestArg -Name 'CaseSensitive'), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg')),
        @((New-TestArg -Name 'ExpectedName' -Value {'MyTable'} -Alias 'TableName'), (New-TestArg -Name 'CaseSensitive'), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg' -Alias 'Namespace')),
        @((New-TestArg -Name 'TableName' -Value {'MyTable'}), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg'), (New-TestArg -Name 'Prefix' -Value 'tbl')),
        @((New-TestArg -Name 'Name' -Value {'MyTable'} -Alias 'TableName'), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg' -Alias 'Namespace'), (New-TestArg -Name 'Prefix' -Value 'tbl')),
        @((New-TestArg -Name 'TableName' -Value {'MyTable'}), (New-TestArg -Name 'CaseSensitive'), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg'), (New-TestArg -Name 'Prefix' -Value 'tbl')),
        @((New-TestArg -Name 'Name' -Value {'MyTable'} -Alias 'TableName'), (New-TestArg -Name 'CaseSensitive'), (New-TestArg -Name 'TableNamespace' -Value 'http://mysite.corg' -Alias 'Namespace'), (New-TestArg -Name 'Prefix' -Value 'tbl'))
    ) | ForEach-Object {
        $ArgsByKey = @{};
        $CommandText = (@('New-DataTable') + @($_ | ForEach-Object {
            $ArgsByKey[$_.Key] = $_;
            $_.ToString() | Write-Output;
        })) -join ' ';
        if ($ArgsByKey['CaseSensitive'] -eq $null) {
            $ArgsByKey['CaseSensitive'] = New-TestArg -Name 'CaseSensitive' -Expected {$false};
        }
        $DataTable = $null;
        It "($CommandText) should return type [System.Data.DataTable]" {
            $Result = [ScriptBlock]::Create($CommandText).InvokeReturnAsIs();
            ,$Result | Should BeOfType ([System.Data.DataTable]);
            if ($Result -ne $null -and $Result -is [System.Data.DataTable]) { $DataTable = $Result }
        }

        $Tests = @(
            @("TableName should be $($ArgsByKey['TableName'].ShouldBe)", {
                if ($ArgsByKey['TableName'] -eq $null -or [string]::IsNullOrEmpty($ArgsByKey['TableName'].Expected)) {
                    $DataTable.TableName | Should BeNullOrEmpty;
                } else {
                    $DataTable.TableName | Should Not BeNullOrEmpty;
                    $DataTable.TableName | Should BeExactly $ArgsByKey['TableName'].Expected;
                }
            }),
            @("TableName should be $($ArgsByKey['TableNamespace'].ShouldBe)", {
                if ($ArgsByKey['TableNamespace'] -eq $null -or [string]::IsNullOrEmpty($ArgsByKey['TableNamespace'].Expected)) {
                    $DataTable.Namespace | Should BeNullOrEmpty;
                } else {
                    $DataTable.Namespace | Should Not BeNullOrEmpty;
                    $DataTable.Namespace | Should BeExactly $ArgsByKey['TableNamespace'].Expected;
                }
            }),
            @("TableName should be $($ArgsByKey['Prefix'].ShouldBe)", {
                if ($ArgsByKey['Prefix'] -eq $null -or [string]::IsNullOrEmpty($ArgsByKey['Prefix'].Expected)) {
                    $DataTable.Prefix | Should BeNullOrEmpty;
                } else {
                    $DataTable.Prefix | Should Not BeNullOrEmpty;
                    $DataTable.Prefix | Should BeExactly $ArgsByKey['Prefix'].Expected;
                }
            }),
            @("CaseSensitive should be $($ArgsByKey['CaseSensitive'].ShouldBe)", {
                $DataTable.CaseSensitive | Should BeExactly ($NamedParameterValues['CaseSensitive'].Invoke());
            })
        )
        if ($DataTable -eq $null) {
            $Tests | ForEach-Object { It @_ -Skip }
        } else {
            $Tests | ForEach-Object { It @_ }
        }
    }
}

Describe 'Testing Add-DataColumn Function', {
    @(@(@(
        @(),
        @((New-TestArg -Value {'MyCol1'} -Key 'ColumnName')),
        @((New-TestArg -Value {'MyCol2'} -Key 'ColumnName'), (New-TestArg -Value {[string]} -Key 'DataType')),
        @((New-TestArg -Value {'MyCol3'} -Key 'ColumnName'), (New-TestArg -Value {[int]} -Key 'DataType'), (New-TestArg -Value {'7'} -Key 'Expr')),
        @((New-TestArg -Value {'MyCol4'} -Key 'ColumnName'), (New-TestArg -Value {[bool]} -Key 'DataType'), (New-TestArg -Value {'TRUE'} -Key 'Expr'),
            (New-TestArg -Value {[System.Data.MappingType]::SimpleContent} -Key 'Type')),
        @((New-TestArg -Name 'ColumnName' -Value {'MyCol5'})),
        @((New-TestArg -Name 'ColumnName' -Value {'MyCol6'}), (New-TestArg -Name 'DataType' -Value {[string]})),
        @((New-TestArg -Name 'ColumnName' -Value {'MyCol7'}), (New-TestArg -Name 'DataType' -Value {[string]}), (New-TestArg -Name 'Expr' -Value {'"test"'})),
        @((New-TestArg -Name 'ColumnName' -Value {'MyCol8'}), (New-TestArg -Name 'DataType' -Value {[datetime]}), (New-TestArg -Name 'Expr' -Value {'NOW()'}),
            (New-TestArg -Name 'Type' -Value {[System.Data.MappingType]::SimpleContent})),
        @((New-TestArg -Name 'ColumnName' -Value {'MyCol9'} -Alias 'Name')),
        @((New-TestArg -Name 'ColumnName' -Value {'MyColA'} -Alias 'Name'), (New-TestArg -Name 'DataType' -Value {[string]})),
        @((New-TestArg -Name 'ColumnName' -Value {'MyColB'} -Alias 'Name'), (New-TestArg -Name 'DataType' -Value {[string]}),
            (New-TestArg -Name 'Expr' -Value {'"test"'} -Alias 'Expression')),
        @((New-TestArg -Name 'ColumnName' -Value {'MyColC'} -Alias 'Name'), (New-TestArg -Name 'DataType' -Value {[datetime]}),
            (New-TestArg -Name 'Expr' -Value {'NOW()'} -Alias 'Expression'),
            (New-TestArg -Name 'Type' -Value {[System.Data.MappingType]::SimpleContent} -Alias 'MappingType'))
    ) | ForEach-Object {
        $ArgInfo = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Ordered = $_; ByName = (New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]'); };
        if ($_.Count -gt 0) {
            $_ | ForEach-Object {
                $ArgInfo.ByName[$_.Key] = $_;
            }
        }
        $ArgInfo | Write-Output;
    } | ForEach-Object {
        $Prev = $_;
        if ($Prev.ByName['ColumnName'] -eq $null) { $Prev.ByName['ColumnName'] = New-TestArg -Name 'ColumnName' -Value {''} }
        $ArgInfo = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Ordered = $Prev.Ordered; ByName = (New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]'); };
        $Prev.ByName.Keys | ForEach-Object { $ArgInfo.ByName[$_] = $Prev.ByName[$_] }
        $Prev.ByName['Caption'] = New-TestArg -Name 'Caption' -Value ([ScriptBlock]::Create($Prev.ByName['ColumnName'].Value)) -Expected ([ScriptBlock]::Create($Prev.ByName['ColumnName'].Expected))
        $Prev | Write-Output;
        $a = New-TestArg -Name 'Caption' -Value {'MyCol'};
        $ArgInfo.ByName['Caption'] = $a;
        $argInfo.Ordered.Add($a);
        $ArgInfo | Write-Output;
    } | ForEach-Object {
        $Prev = $_;
        if ($Prev.ByName['Expr'] -eq $null) {
            $ArgInfo = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Ordered = $Prev.Ordered; ByName = (New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]'); };
            $Prev.ByName.Keys | ForEach-Object { $ArgInfo.ByName[$_] = $Prev.ByName[$_] }
            $Prev.ByName['AllowDBNull'] = New-TestArg -Name 'AllowDBNull' -Expected {$false};
            $Prev | Write-Output;
            $a = New-TestArg -Name 'AllowDBNull';
            $ArgInfo.ByName['Caption'] = $a;
            $argInfo.Ordered.Add($a);
            $ArgInfo | Write-Output;
        } else {
            $Prev.ByName['AllowDBNull'] = New-TestArg -Name 'AllowDBNull' -Expected {$false};
            $Prev | Write-Output;
        }
    }) | ForEach-Object {
        $Prev = $_;
        if ($Prev.ByName['DataType'] -eq $null) { $Prev.ByName['DataType'] = New-TestArg -Name 'DataType' -Value {[string]} }
        $Prev | Write-Output;
        if ($Prev.ByName['DataType'].Expected -eq [System.DateTime]) {
            $ArgInfo = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Ordered = $Prev.Ordered; ByName = (New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]'); };
            $Prev.ByName.Keys | ForEach-Object { $ArgInfo.ByName[$_] = $Prev.ByName[$_] }
            $a = New-TestArg -Name 'DateTimeMode' -Value {[System.Data.DataSetDateTime]::Utc};
            $ArgInfo.ByName['DateTimeMode'] = $a;
            $argInfo.Ordered.Add($a);
            $ArgInfo | Write-Output;
        } else {
            if ($Prev.ByName['Expr'] -eq $null -and (-not $Prev.ByName['AllowDBNull']) -and ($Prev.ByName['DataType'].Expected -eq [int] -or $Prev.ByName['DataType'].Expected -eq [System.Int64])) {
                $ArgInfo = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Ordered = $Prev.Ordered; ByName = (New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]'); };
                $Prev.ByName.Keys | ForEach-Object { $ArgInfo.ByName[$_] = $Prev.ByName[$_] }
                $a = New-TestArg -Name 'DateTimeMode' -Value {[System.Data.DataSetDateTime]::Utc};
                $ArgInfo.ByName['DateTimeMode'] = $a;
                $argInfo.Ordered.Add($a);
                $ArgInfo | Write-Output;
            }
        }
        
        if ($Prev.DataType -eq [System.DateTime]) {
        } else {
            if ($Prev.Parameters['Expression'] -eq $null -and (-not $Prev.AllowDBNull) -and ($Prev.DataType -eq [int] -or $Prev.DataType -eq [System.Int64])) {
                $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
                $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
                $ArgInfo.Parameters['AutoIncrement'] = {$true};
                $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -AutoIncrement";
                $Prev = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
                $Prev | Write-Output;
                $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
                $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
                $ArgInfo.Parameters['AutoIncrementSeed'] = {123456};
                $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -AutoIncrementSeed $($ArgInfo.Parameters['AutoIncrementSeed'].ToString().Trim())";
                New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
            }
        }
    }) | ForEach-Object {
        $Prev = $_;
        if ($Prev.Parameters['AutoIncrement'] -eq $null) {
            $Prev | Add-Member -MemberType NoteProperty -Name 'AutoIncrement' -Value $false -PassThru;
        } else {
            $Prev | Add-Member -MemberType NoteProperty -Name 'AutoIncrement' -Value $Prev.Parameters['AutoIncrement'].InvokeReturnAsIs();
        }
        $Prev | Write-Output;
        if ($Prev.AutoIncrement) {
            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
            $ArgInfo.Parameters['AutoIncrementStep'] = {100};
            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -AutoIncrementStep";
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
        } else {
            if ($Prev.Parameters['Expression'] -eq $null) {
                if ($Prev.DataType -eq [int] -or $Prev.DataType -eq [System.Int64]) {
                    $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
                    $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
                    $ArgInfo.Parameters['DefaultValue'] = {8675309};
                    $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -DefaultValue $($ArgInfo.Parameters['DefaultValue'].ToString().Trim())";
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
                } else {
                    if ($Prev.DataType -eq [string]) {
                        $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
                        $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
                        $ArgInfo.Parameters['DefaultValue'] = {"Test"};
                        $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -DefaultValue $($ArgInfo.Parameters['DefaultValue'].ToString().Trim())";
                        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
                    } else {
                        if ($Prev.DataType -eq [bool]) {
                            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
                            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
                            $ArgInfo.Parameters['DefaultValue'] = {$true};
                            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -DefaultValue $($ArgInfo.Parameters['DefaultValue'].ToString().Trim())";
                            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
                        }
                    }
                }
            }
        }
    } | ForEach-Object {
        $Prev = $_;
        $Prev | Write-Output;
        if ($Prev.Parameters['Expression'] -eq $null -and $Prev.DataType -eq [string]) {
            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
            $ArgInfo.Parameters['MaxLength'] = {255};
            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -MaxLength $($ArgInfo.Parameters['MaxLength'].ToString().Trim())";
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
        }
    } | ForEach-Object {
        $Prev = $_;
        $Prev | Write-Output;
        if ($Prev.Parameters['Expression'] -eq $null) {
            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
            $ArgInfo.Parameters['ReadOnly'] = {$true};
            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -ReadOnly";
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
        }
    } | ForEach-Object {
        $Prev = $_;
        $Prev | Write-Output;
        if ($Prev.Parameters['Expression'] -eq $null -and -not $Prev.AllowDBNull) {
            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
            $ArgInfo.Parameters['Unique'] = {$true};
            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -Unique";
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
        }
    } | ForEach-Object {
        $ArgInfo = $_;
        Context $ArgInfo.ScriptText {
            $DataColumn = $null;
            $DataTable = New-DataTable;
            It "Function should return type [System.Data.DataColumn]" {
                $Result = [ScriptBlock]::Create($ArgInfo.ScriptText).InvokeReturnAsIs();
                ,$Result | Should BeOfType ([System.Data.DataColumn]);
                if ($Result -ne $null -and $Result -is [System.Data.DataColumn]) { $DataColumn = $Result }
            }
            $Tests = @(
                @("Result object 'Table' property should be same instance as `$DataTable", {
                    ,$DataColumn.Table | Should Not Be $null;
                    [object]::ReferenceEquals($DataTable, $DataColumn.Table) | Should Be $true;
                })
            ) + @($ArgInfo.Parameters.Keys | ForEach-Object {
                @("Result object '$_' property should be $($ArgInfo.Parameters[$_].ToString().Trim())", {
                    $Actual = $DataColumn.($_);
                    $Actual | Should Be $ArgInfo.Parameters[$_].InvokeReturnAsIs();
                })
            });
            if ($DataColumn -eq $null) {
                $Tests | ForEach-Object { It @_ -Skip }
            } else {
                $Tests | ForEach-Object { It @_ }
            }
        }
    }
}