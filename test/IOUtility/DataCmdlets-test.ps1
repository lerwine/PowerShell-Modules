Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.LteDev/Erwine.Leonard.T.IOUtility.psd1');

Describe 'Testing New-DataTable Function', {
    @(
        @(),
        @({'MyTable'}),
        @(@{ TableName = {'MyTable'} }),
        @(@{ Name = {'MyTable'} }),
        @(@{ TableName = {'MyTable'} }, 'CaseSensitive'),
        @(@{ Name = {'MyTable'} }, 'CaseSensitive'),
        @(@{ TableName = {'MyTable'} }, @{ TableNamespace = {'http://mysite.corg'} }),
        @(@{ Name = {'MyTable'} }, @{ Namespace = {'http://mysite.corg'} }),
        @(@{ TableName = {'MyTable'} }, @{ TableNamespace = {'http://mysite.corg'} }, @{ Prefix = {'tbl'} }),
        @(@{ Name = {'MyTable'} }, @{ Namespace = {'http://mysite.corg'} }, @{ Prefix = {'tbl'} }),
        @(@{ TableName = {'MyTable'} }, @{ TableNamespace = {'http://mysite.corg'} }, 'CaseSensitive'),
        @(@{ Name = {'MyTable'} }, @{ Namespace = {'http://mysite.corg'} }, 'CaseSensitive'),
        @(@{ TableName = {'MyTable'} }, @{ TableNamespace = {'http://mysite.corg'} }, @{ Prefix = {'tbl'} }, 'CaseSensitive'),
        @(@{ Name = {'MyTable'} }, @{ Namespace = {'http://mysite.corg'} }, @{ Prefix = {'tbl'} }, 'CaseSensitive')
    ) | ForEach-Object {
        $NamedParameterValues = @{ CaseSensitive = {$false}; Prefix = {''}; TableName = {''}; Namespace = {''} };
        $PositionalParameterValues = @();
        $ScriptText = 'New-DataTable';
        $_ | ForEach-Object {
            if ($_ -is [string]) {
                $NamedParameterValues[$_] = {$true};
                $ScriptText = "$ScriptText -$_";
            } else {
                if ($_ -is [scriptblock]) {
                    $ScriptText = "$ScriptText $($_.ToString().Trim())";
                    $PositionalParameterValues += $_;
                } else {
                    $h = $_;
                    $h.Keys | ForEach-Object {
                        $ScriptText = "$ScriptText -$_ $($h[$_].ToString().Trim())";
                        $NamedParameterValues[$_] = $h[$_];
                    }
                }
            }
        }
        
        if ($NamedParameterValues['Name'] -ne $null) { $NamedParameterValues['TableName'] = $NamedParameterValues['Name'] } else { if ($PositionalParameterValues.Count -gt 0) { $NamedParameterValues['TableName'] = $PositionalParameterValues[0] } }
        if ($NamedParameterValues['TableNamespace'] -ne $null) { $NamedParameterValues['Namespace'] = $NamedParameterValues['TableNamespace'] }
        [System.Data.DataTable]$DataTable = $null;
        It "($ScriptText) should return type [System.Data.DataTable]" {
            $Result = [ScriptBlock]::Create($ScriptText).InvokeReturnAsIs();
            ,$Result | Should -BeOfType ([System.Data.DataTable]);
            if ($Result -ne $null -and $Result -is [System.Data.DataTable]) { $DataTable = $Result }
        }
        $Tests = @(
            @("TableName should be $($NamedParameterValues['TableName'].ToString().Trim())", {
                if ($NamedParameterValues['TableName'].Length -eq 0) {
                    $DataTable.TableName | Should -BeNullOrEmpty;
                } else {
                    $DataTable.TableName | Should -Not -BeNullOrEmpty;
                    $DataTable.TableName | Should -BeExactly ($NamedParameterValues['TableName'].Invoke());
                }
            }),
            @("Namespace should be $($NamedParameterValues['Namespace'].ToString().Trim())", {
                if ($NamedParameterValues['Namespace'].Length -eq 0) {
                    $DataTable.Namespace | Should -BeNullOrEmpty;
                } else {
                    $DataTable.Namespace | Should -Not -BeNullOrEmpty;
                    $DataTable.Namespace | Should -BeExactly ($NamedParameterValues['Namespace'].Invoke());
                }
            }),
            @("Prefix should be $($NamedParameterValues['Prefix'].ToString().Trim())", {
                if ($NamedParameterValues['Prefix'].Length -eq 0) {
                    $DataTable.Prefix | Should -BeNullOrEmpty;
                } else {
                    $DataTable.Prefix | Should -Not -BeNullOrEmpty;
                    $DataTable.Prefix | Should -BeExactly ($NamedParameterValues['Prefix'].Invoke());
                }
            }),
            @("CaseSensitive should be $($NamedParameterValues['CaseSensitive'].ToString().Trim())", {
                $DataTable.CaseSensitive | Should -BeExactly ($NamedParameterValues['CaseSensitive'].Invoke());
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
        @({'MyColumn1'}),
        @({'MyColumn2'}, {[string]}),
        @({'MyColumn3'}, {[int]}, {'7'}),
        @({'MyColumn4'}, {[bool]}, {'TRUE'}, {[System.Data.MappingType]::Attribute}),
        @(@{ ColumnName = {'MyColumn5'} }),
        @(@{ Name = {'MyColumn6'} }),
        @(@{ ColumnName = {'MyColumn7'} }, @{ DataType = {[System.Int64]} }),
        @(@{ Name = {'MyColumn8'} }, @{ DataType = {[System.DateTime]} }),
        @(@{ ColumnName = {'MyColumn9'} }, @{ DataType = {[float]} }, @{ Expr = {'3.5'} }),
        @(@{ Name = {'MyColumn0'} }, @{ DataType = {[string]} }, @{ Expression = {'"test"'} }),
        @(@{ ColumnName = {'MyColumnA'} }, @{ DataType = {[int]} }, @{ Expr = {'1'} }, @{ Type = {[System.Data.MappingType]::SimpleContent} }),
        @(@{ Name = {'MyColumnB'} }, @{ DataType = {[System.DateTime]} }, @{ Expression = {'NOW()'} }, @{ MappingType = {[System.Data.MappingType]::Element} })
    ) | ForEach-Object {
        $ArgInfo = @{ ScriptText = 'Add-DataColumn'; Parameters = @{ ColumnName = {''}; DataType = {[System.String]}; Type = {[System.Data.MappingType]::Element}; AllowDBNull = {$false}; ReadOnly = {$false}; Unique = {$false} } };
        $Position = 0;
        if ($_.Count -gt 0) {
            if ($_[0] -is [ScriptBlock]) {
                $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) `$DataTable";
            } else {
                $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -DataTable `$DataTable";
            }
            $_ | ForEach-Object {
                if ($_ -is [ScriptBlock]) {
                    $Position++;
                    $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) $($_.ToString().Trim())";
                    $sb = $_;
                    switch ($Position) {
                        1 {
                            $ArgInfo.Parameters['ColumnName'] = $sb;
                            break;
                        }
                        2 {
                            $ArgInfo.Parameters['DataType'] = $sb;
                            break;
                        }
                        3 {
                            $ArgInfo.Parameters['Expression'] = $sb;
                            break;
                        }
                        default {
                            $ArgInfo.Parameters['Type'] = $sb;
                            break;
                        }
                    }
                } else {
                    if ($_ -is [string]) {
                        $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -$_";
                        $ArgInfo.Parameters[$_] = {$true};
                    } else {
                        $h = $_;
                        $h.Keys | ForEach-Object {
                            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -$_ $($h[$_].ToString().Trim())";
                            switch ($_) {
                                'Name' { $ArgInfo.Parameters['ColumnName'] = $h[$_]; break; }
                                'Expr' { $ArgInfo.Parameters['Expression'] = $h[$_]; break; }
                                'MappingType' { $ArgInfo.Parameters['Type'] = $h[$_]; break; }
                                default { $ArgInfo.Parameters[$_] = $h[$_]; break; }
                            }
                        }
                    }
                }
            }
        }
        $ArgInfo.Parameters['Caption'] = $ArgInfo.Parameters['ColumnName'];
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
    } | ForEach-Object {
        $Prev = $_;
        $Prev | Write-Output;
        $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
        $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
        $ArgInfo.Parameters['Caption'] = {'My Col'};
        $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -Caption $($ArgInfo.Parameters['Caption'].ToString().Trim())";
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
    } | ForEach-Object {
        $Prev = $_;
        $Prev | Write-Output;
        if ($Prev.Parameters['Expression'] -eq $null) {
            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
            $ArgInfo.Parameters['AllowDBNull'] = {$true};
            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -AllowDBNull";
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
        }
    }) | ForEach-Object {
        $Prev = $_;
        $Prev | Add-Member -MemberType NoteProperty -Name 'DataType' -Value $Prev.Parameters['DataType'].InvokeReturnAsIs();
        $Prev | Add-Member -MemberType NoteProperty -Name 'AllowDBNull' -Value $Prev.Parameters['AllowDBNull'].InvokeReturnAsIs();
        $Prev | Write-Output;
        if ($Prev.DataType -eq [System.DateTime]) {
            $ArgInfo = @{ ScriptText = $Prev.ScriptText; Parameters = @{ } };
            $Prev.Parameters.Keys | ForEach-Object { $ArgInfo.Parameters[$_] = $Prev.Parameters[$_] }
            $ArgInfo.Parameters['DateTimeMode'] = {[System.Data.DataSetDateTime]::Utc};
            $ArgInfo.ScriptText = "$($ArgInfo.ScriptText) -DateTimeMode $($ArgInfo.Parameters['DateTimeMode'].ToString().Trim())";
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ArgInfo;
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
                ,$Result | Should -BeOfType ([System.Data.DataColumn]);
                if ($Result -ne $null -and $Result -is [System.Data.DataColumn]) { $DataColumn = $Result }
            }
            $Tests = @(
                @("Result object 'Table' property should be same instance as `$DataTable", {
                    ,$DataColumn.Table | Should -Not -Be $null;
                    [object]::ReferenceEquals($DataTable, $DataColumn.Table) | Should -Be $true;
                })
            ) + @($ArgInfo.Parameters.Keys | ForEach-Object {
                @("Result object '$_' property should be $($ArgInfo.Parameters[$_].ToString().Trim())", {
                    $Actual = $DataColumn.($_);
                    $Actual | Should -Be $ArgInfo.Parameters[$_].InvokeReturnAsIs();
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