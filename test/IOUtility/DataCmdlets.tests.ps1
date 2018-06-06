Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.IOUtility') -ErrorAction Stop;

$DataTableTestData = @(
@{ EvalCode = @({New-DataTable}); ResultProperties = {@{ CaseSensitive = $false }} },
@{ EvalCode = @({New-DataTable -CaseSensitive}); ResultProperties = {@{ CaseSensitive = $true }} },
@{ EvalCode = @({New-DataTable "MyTable"}, {New-DataTable -TableName "MyTable"}, {New-DataTable -Name "MyTable"}); ResultProperties = {@{
    CaseSensitive = $false;
    TableName = "MyTable";
    Namespace = '';
    Prefix = '';
}} },
@{ EvalCode = @({New-DataTable "X" -CaseSensitive}, {New-DataTable -TableName "X" -CaseSensitive}, {New-DataTable -Name "X" -CaseSensitive},
                {New-DataTable "X" "" -CaseSensitive}, {New-DataTable -TableName "X" -Namespace "" -CaseSensitive},
                {New-DataTable -Name "X" -Namespace "" -CaseSensitive}, {New-DataTable -TableName "X" -TableNamespace "" -CaseSensitive},
                {New-DataTable -Name "X" -TableNamespace "" -CaseSensitive}); ResultProperties = {@{
    CaseSensitive = $true;
    TableName = "X";
    Namespace = '';
    Prefix = '';
}} },
@{ EvalCode = @({New-DataTable "X" "http://www.erwinefamily.net/test/namespace"},
                {New-DataTable -TableName "X" -Namespace "http://www.erwinefamily.net/test/namespace"},
                {New-DataTable -Name "X" -Namespace "http://www.erwinefamily.net/test/namespace"},
                {New-DataTable -TableName "X" -TableNamespace "http://www.erwinefamily.net/test/namespace"},
                {New-DataTable -Name "X" -TableNamespace "http://www.erwinefamily.net/test/namespace"}); ResultProperties = {@{
    CaseSensitive = $false;
    TableName = "X";
    Namespace = 'http://www.erwinefamily.net/test/namespace';
    Prefix = '';
}} },
@{ EvalCode = @({New-DataTable "X" "urn:erwinefamily.net:test/namespace" -CaseSensitive},
                {New-DataTable -TableName "X" -Namespace "urn:erwinefamily.net:test/namespace" -CaseSensitive},
                {New-DataTable -Name "X" -TableNamespace "urn:erwinefamily.net:test/namespace" -CaseSensitive},
                {New-DataTable "X" "urn:erwinefamily.net:test/namespace" "" -CaseSensitive},
                {New-DataTable -TableName "X" -Namespace "urn:erwinefamily.net:test/namespace" -Prefix "" -CaseSensitive},
                {New-DataTable -Name "X" -TableNamespace "urn:erwinefamily.net:test/namespace" -Prefix "" -CaseSensitive}); ResultProperties = {@{
    CaseSensitive = $true;
    TableName = "X";
    Namespace = '';
    Prefix = '';
}} },
@{ EvalCode = @({New-DataTable "X.Y" "http://www.erwinefamily.net/test/namespace", "T"},
                {New-DataTable -TableName "X.Y" -Namespace "http://www.erwinefamily.net/test/namespace" -Prefix "T"},
                {New-DataTable -Name "X.Y" -Namespace "http://www.erwinefamily.net/test/namespace" -Prefix "T"},
                {New-DataTable -TableName "X.Y" -TableNamespace "http://www.erwinefamily.net/test/namespace" -Prefix "T"},
                {New-DataTable -Name "X.Y" -TableNamespace "http://www.erwinefamily.net/test/namespace" -Prefix "T"}); ResultProperties = {@{
    CaseSensitive = $false;
    TableName = "X.Y";
    Namespace = 'http://www.erwinefamily.net/test/namespace';
    Prefix = 'T';
}} }
);
Describe 'Testing New-DataTable' {
    $ReturnType = [System.Data.DataTable];
    $ReturnName = [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($ReturnType);
    $DataTableTestData | ForEach-Object {
        $ResultProperties = $_.ResultProperties;
        $_.EvalCode | ForEach-Object {
            It "($_) should return $ReturnName $ReturnProperties" {
                $Result = $_.InvokeReturnAsIs();
                ,$Result | Should BeOfType $ReturnType;
                $Expected = $ResultProperties.InvokeReturnAsIs();
                foreach ($pn in $Expected.Keys) {
                    if ($Expected[$pn] -is [bool]) {
                        if ($Expected[$pn]) {
                            $Result.($pn) | Should BeTrue;
                        } else {
                            $Result.($pn) | Should BeFalse;
                        }
                    } else {
                        if ($Expected[$pn] -is [string]) {
                            $Result.($pn) | Should BeExactly $Expected[$pn];
                        } else {
                            $Result.($pn) | Should Be $Expected[$pn];
                        }
                    }
                }
            }
        }
    }
}

$AddColumnData = @(
@{ EvalCode = @({Add-DataColumn $DataTable}); ResultProperties = {@{
    Ordinal = 0;
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
}}; ReturnsTable = $false; },
@{ EvalCode = @({Add-DataColumn "MyCol"}, {Add-DataColumn -Name "MyCol"}, {Add-DataColumn -ColumnName "MyCol"}); ResultProperties = {@{
    Ordinal = 0;
    ColumnName = "MyCol";
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
    Prefix = '';
}}; ReturnsTable = $false; },
@{ EvalCode = @({Add-DataColumn "Z" [bool]}, {Add-DataColumn -Name "Z" -DataType [bool]}, {Add-DataColumn -ColumnName "Z" -DataType [bool]}); ResultProperties = {@{
    Ordinal = 0;
    ColumnName = "Z";
    DataType = [bool];
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
    Prefix = '';
}}; ReturnsTable = $false; },
@{ EvalCode = @({Add-DataColumn "Z" [string] [System.Data.MappingType]::SimpleContent },
                {Add-DataColumn -Name "Z" -DataType [string] -Type SimpleContent},
                {Add-DataColumn -ColumnName "Z" -DataType [string] -Type SimpleContent},
                {Add-DataColumn -Name "Z" -DataType [string] -MappingType SimpleContent},
                {Add-DataColumn -ColumnName "Z" -DataType [string] -MappingType SimpleContent}); ResultProperties = {@{
    Ordinal = 0;
    ColumnName = "Z";
    DataType = [string];
    ColumnMapping = [System.Data.MappingType]::SimpleContent;
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
    Prefix = '';
}}; ReturnsTable = $false; },
@{ EvalCode = @({Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn "C" [int] 'A+B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -Name "C" -DataType [string] -Expr 'A+B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -ColumnName "C" -DataType [string] -Expr 'A+B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -Name "C" -DataType [string] -Expression 'A+B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -ColumnName "C" -DataType [string] -Expression 'A+B'}); ResultProperties = {@{
    Ordinal = 2;
    ColumnName = "C";
    DataType = [int];
    Expression = 'A+B';
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
    Prefix = '';
}}; ReturnsTable = $false; },
@{ EvalCode = @({Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn "C" [int] 'A+B' Element}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -Name "C" -DataType [string] -Expr 'A+B' -Type Element}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -ColumnName "C" -DataType [string] -Expr 'A+B' -MappingType Element}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -Name "C" -DataType [string] -Expression 'A+B' -MappingType Element}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -ColumnName "C" -DataType [string] -Expression 'A+B' -Type Element}); ResultProperties = {@{
    Ordinal = 2;
    ColumnName = "C";
    DataType = [int];
    Expression = 'A+B';
    ColumnMapping = [System.Data.MappingType]::Element;
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
    Prefix = '';
}}; ReturnsTable = $false; },
@{ EvalCode = @({Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn "C" [int] 'A+B' Attribute -Caption 'A plus B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -Name "C" -DataType [string] -Expr 'A+B' -Type Attribute -Caption 'A plus B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -ColumnName "C" -DataType [string] -Expr 'A+B' -MappingType Attribute -Caption 'A plus B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -Name "C" -DataType [string] -Expression 'A+B' -MappingType Attribute -Caption 'A plus B'}, {Add-DataColumn "A" [int]; Add-DataColumn "B" [int];
    Add-DataColumn -ColumnName "C" -DataType [string] -Expression 'A+B' -Type Attribute -Caption 'A plus B'}); ResultProperties = {@{
    Ordinal = 2;
    ColumnName = "C";
    DataType = [int];
    Expression = 'A+B';
    ColumnMapping = [System.Data.MappingType]::Attribute;
    AutoIncrement = $false;
    ReadOnly = $false;
    Unique = $false;
    Prefix = '';
}}; ReturnsTable = $false; }
);
$AddColumnData = @($AddColumnData | ForEach-Object {
    $h = $_.ResultProperties.InvokeReturnAsIs();
    if ($h.ContainsKey('AllowDBNull')) {
        $_ | Write-Output;
    } else {
        $c = $_.ResultProperties.ToString();
        $i = $c.LastIndexOf('}');
        if ($i -gt 0) { $c = $c.Substring(0, $i).TrimEnd(); }
        if ($c.EndsWith(';')) { $c = $c.Substring(0, $c.Length - 1) }
        @{ EvalCode = [scriptblock]::Create("$($_.EvalCode) -AllowDBNull"); ResultProperties = [scriptblock]::Create(@"
$c;
    AllowDBNull = `$false;
}
"@); ReturnsTable = $false }
        $_ | Write-Output;
        if ($h.ContainsKey('ColumnName') -and -not $h.ContainsKey('Expression')) {
            @{ EvalCode = [scriptblock]::Create("$($_.EvalCode) -AllowDBNull"); ResultProperties = [scriptblock]::Create(@"
$c;
    AllowDBNull = `$true;
}
"@); ReturnsTable = $false }
        }
    }
});
$AddColumnData = @($AddColumnData | ForEach-Object {
    $_ | Write-Output;
    @{ EvalCode = [scriptblock]::Create("$($_.EvalCode) -PassThru"); ResultProperties = $_.ResultProperties; ReturnsTable = $true }
});
<#
dataColumn.AllowDBNull;
dataColumn.AutoIncrement;
dataColumn.AutoIncrementSeed;
dataColumn.AutoIncrementStep;
dataColumn.Caption;
dataColumn.ColumnMapping;
dataColumn.ColumnName;
dataColumn.DataType;
dataColumn.DateTimeMode;
dataColumn.DefaultValue;
dataColumn.Expression;
dataColumn.MaxLength;
dataColumn.Ordinal;
dataColumn.Prefix;
dataColumn.ReadOnly;
dataColumn.Unique;
#>
Describe 'Testing Add-DataColum' {
    $ReturnType = [System.Data.DataTable];
    $ReturnName = [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($ReturnType);
    $AddColumnData | ForEach-Object {
        $ResultProperties = $_.ResultProperties;
        $ReturnsTable = $_.ReturnsTable;
        $_.EvalCode | ForEach-Object {
            $Expected = $ResultProperties.InvokeReturnAsIs();
            $Description = "column";
            if ($Expected.Ordinal -gt 0) { $Description += "s" }
            if ($ReturnsTable) { $Description = "$Description and return $ReturnName" } else { $Description = "$Description and return `$null" }
            if ($Expected.Ordinal -gt 0) { $Description = "$Description with column $($Expected.Ordinal + 1)" } else { $Description = "$Description with column" }
            It "($_) should add $($Expected.Ordinal) $Description Properties set to $ResultProperties" {
                $EvalCode = [scriptblock]::Create("Param([System.Data.DataTable]`$DataTable) $_");
                $DataTable = New-DataTable;
                $Result = $EvalCode.InvokeReturnAsIs($DataTable);
                if ($ReturnsTable) {
                    ,$Result | Should BeOfType $ReturnType;
                    [Object]::ReferenceEquals($Result, $DataTable) | Should BeTrue;
                } else {
                    ,$Result | Should Be $null;
                }
                $DataTable.Columns.Count | Should Be ($Expected.Ordinal + 1);
                $Col = $DataTable.Columns[$Expected.Ordinal];
                foreach ($pn in $Expected.Keys) {
                    if ($Expected[$pn] -is [bool]) {
                        if ($Expected[$pn]) {
                            $Col.($pn) | Should BeTrue;
                        } else {
                            $Col.($pn) | Should BeFalse;
                        }
                    } else {
                        if ($Expected[$pn] -is [string]) {
                            $Col.($pn) | Should BeExactly $Expected[$pn];
                        } else {
                            $Col.($pn) | Should Be $Expected[$pn];
                        }
                    }
                }
            }
        }
    }
}


<#
Function ConvertTo-IndentedLines {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [string[]]$Text,

        [int]$Level = 1
    )

    Begin {
        if ($Script:__ConvertToIndentedLinesRegex -eq $null) {
            $Script:__ConvertToIndentedLinesRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '\r\n?|\n';
        }
        $Lines = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.String]';
        $IndentText = '  ';
        for ($i = 1; $i -lt $Level; $i++) { $IndentText += '  ' }
    }

    Process {
        if ($Text -ne $null -and $Text.Length -gt 0) { $Text | Where-Object { $_ -ne $null } | ForEach-Object {
            if ($_.Length -eq 0) {
                $Lines.Add('');
            } else {
                $Script:__ConvertToIndentedLinesRegex.Split($_) | Select-Object { $_.TrimEnd() } | ForEach-Object {
                    if ($_.Length -gt 0) { $Lines.Add("$IndentText$_") } else { $Lines.Add($_) }
                }
            }
        } }
    }

    End {
        if ($Lines.Count -gt 0) {
            if ($Lines.Count -eq 1) { $Lines[0] | Write-Output } else { ($Lines -join "`n") | Write-Output }
        }
    }
}

Function ConvertTo-PSLiteral {
    [CmdletBinding(DefaultParameterSetName = 'BoxingOpt')]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [object]$Value,

        [int]$MaxDepth = 32,

        [int]$MaxItems = 1024,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'ImplictType')]
        [Type]$ImplictType,

        
        [Parameter(ParameterSetName = 'BoxingOpt')]
        [switch]$NoBoxing
    )

    if ($Value -eq $null) { return '$null'; }
    if ($Value -is [int] -or $Value -is [double]) { return $Value.ToString() }
    if ($Value -is [bool]) {
        if ($Value) { return '$true' }
        return '$false';
    }
    if ($Value -is [string]) {
        if ($Value.Length -eq 0) { return "''"; }
        if ($Script:__ConvertToPSStringCtrlRegex -eq $null) {
            $Script:__ConvertToPSStringCtrlRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '[\u0000-\u001f]', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
            $Script:__ConvertToPSStringEscapeRegex1 = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '[\u0000-\u001f\$`"]', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
            $Script:__ConvertToPSStringEscapeRegex2 = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '[\$`"]', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
        }
        if ($Script:__ConvertToPSStringCtrlRegex.IsMatch($Value)) {
            return '"' + $Script:__ConvertToPSStringEscapeRegex1.Replace($Value, {
                Param([System.Text.RegularExpressions.Match]$e)
                switch ($e.Value) {
                    "`0" { return '`0'; }
                    "`a" { return '`a'; }
                    "`b" { return '`b'; }
                    "`f" { return '`f'; }
                    "`n" { return '`n'; }
                    "`r" { return '`r'; }
                    "`t" { return '`t'; }
                    "`v" { return '`v'; }
                    "`"" { return '`"'; }
                    "`$" { return '`$'; }
                    "``" { return '``'; }
                }
                return "`$([char]$([int]([char]$e.Value)))";
            }) + '"';
        }
        if ($Value.Contains("'")) {
            if ($Script:__ConvertToPSStringEscapeRegex2.IsMatch($Value)) {
                return "`"$($Script:__ConvertToPSStringEscapeRegex2.Replace($Value, {
                    Param([System.Text.RegularExpressions.Match]$e)
                    return "``$($e.Value)";
                }))`"";
            }
            return "`"$Value`"";
        }
        return "'$Value'";
    }

    $Type = $Value.GetType();
    $Code = $null;
    switch ($Value) {
        { $_ -is [char] } { $Code = ConvertTo-PSLiteral -Value (New-Object -TypeName 'System.String' -ArgumentList ([char[]]@($Value))); break; }
        { $_ -is [type] } { $Code = [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Value.FullName); $Type = $null; break; }
        { $_ -is [uri] } { $Code = ConvertTo-PSLiteral -Value $Value.OriginalString; break; }
        { $_ -is [xml] } { $Code = ConvertTo-PSLiteral -Value $Value.OuterXml; break; }
        { $_ -is [guid] } { $Code = ConvertTo-PSLiteral -Value $Value.ToString('N'); break; }
        { $_ -is [datetime] } { $Code = ConvertTo-PSLiteral -Value $Value.ToString('o'); break; }
        { $_ -is [dbnull] } { $Code = '[System.DBNull]::Value'; $Type = $null; break; }
        { $_ -is [version] } {
            if ($Value.Revision -eq -1) {
                if ($Value.Build -eq -1) {
                    $Code = "'$($Value.Major).$($Value.Minor)'"
                } else {
                    $Code = "'$($Value.Major).$($Value.Minor).$($Value.Build)'";
                }
            } else {
                $Code = "'$($Value.Major).$($Value.Minor).$($Value.Build).$($Value.Revision)'";
            }
            break;
        }
        { $_ -is [System.Collections.IDictionary] } {
            if ($Value.Count -eq 0 -or $MaxItems -lt 1) { return '@{}' }
            if ($MaxDepth -lt 0) { return $null }
            if ($Script:__ConvertToPSStringKeyRegex -eq $null) {
                $Script:__ConvertToPSStringKeyRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^[a-z_][a-z\d_]*$', ([System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor [System.Text.RegularExpressions.RegexOptions]::Compiled);
            }
            if ($MaxDepth -gt 0) {
                return "@{" + (($Value.Keys | ForEach-Object {
                    if ($MaxItems -gt 0 -and ($MaxItems = $MaxItems - 1) -gt 0) {
                        if ($_ -is [string] -and $Script:__ConvertToPSStringKeyRegex.IsMatch($_)) {
                            "`n  $_ = $(((ConvertTo-PSLiteral -Value $Value[$_] -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems) | ConvertTo-IndentedLines -Level 2).Trim())";
                        } else {
                            "`n  $(ConvertTo-PSLiteral -Value $_ -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems) = $(((ConvertTo-PSLiteral -Value $Value[$_] -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems) | ConvertTo-IndentedLines).Trim())";
                        }
                    }
                }) -join ';') + "`n}";
            }
            return "@{" + (($Value.Keys | ForEach-Object {
                if ($MaxItems -gt 0 -and ($MaxItems = $MaxItems - 1) -gt 0) {
                    $c = ((ConvertTo-PSLiteral -Value $Value[$_] -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems -NoBoxing) | ConvertTo-IndentedLines -Level 2).Trim();
                    if ($c -ne $null) {
                        if ($_ -is [int] -or $_ -is [double] -or ($_ -is [string] -and $Script:__ConvertToPSStringKeyRegex.IsMatch($_))) {
                            "`n  $_ = $c";
                        } else {
                            $k = ConvertTo-PSLiteral -Value $_ -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems;
                            if ($k -ne $null) {
                                "`n  $k = $c";
                            }
                        }
                    }
                }
            }) -join ';') + "`n}";
        }
        { $_ -is [System.Collections.ICollection] -or $Type.IsArray } {
            if (@($Value).Count -eq 0 -or $MaxItems -lt 1) {
                if ($Type.IsArray -and $Type.GetElementType().AssemblyQualifiedName -ne [object].AssemblyQualifiedName) {
                    return "$([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type.FullName))@()";
                }
                return '@()';
            }
            if ($MaxDepth -lt 0) { return $null }
            $ElementType = [object];
            if ($Type.IsArray) {
                $ElementType = $Type.GetElementType();
            } else {
                $n = [System.Collections.Generic.ICollection`1].AssemblyQualifiedName;
                $tArr = @($Type.GetInterfaces() | Where-Object { $_.IsGenericType -and $_.GetGenericTypeDefinition().AssemblyQualifiedName -ceq $n });
                [System.Collections.Generic.ICollection`1]
                if ($tArr.Count -gt 0) {
                    $ElementType = ($tArr[0].GetGenericArguments())[0];
                    $Type = $ElementType.MakeArrayType();
                    if ($ElementType.IsGenericType -and $ElementType.GetGenericTypeDefinition().AssemblyQualifiedName -eq [System.Nullable`1].AssemblyQualifiedName) {
                        $ElementType = [System.Nullable]::GetUnderlyingType($ElementType);
                    }
                } else {
                    $eArr = @(@($Value) | ForEach-Object { if ($_ -ne $null) { $_.GetType() } } | Select-Object -Unique);
                    if ($eArr.Count -eq 1) {
                        $ElementType = $eArr[0];
                        if ($ElementType.IsValueType -and @($_ | Where-Object { $_ -eq $null }).Count -gt 0) {
                            $Type = [System.Nullable`1].MakeGenericType($ElementType).MakeArrayType();
                        } else {
                            $Type = $ElementType.MakeArrayType();
                        }
                    }
                }
            }
            if ($ElementType.AssemblyQualifiedName -ne [object].AssemblyQualifiedName) {
                $Code = '@(' + ((@($Value) | ForEach-Object {
                    if ($MaxItems -gt 0 -and ($MaxItems = $MaxItems - 1) -gt 0) {
                        $c = $null;
                        $c = ((ConvertTo-PSLiteral -Value $_ -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems -ImplictType $ElementType) | ConvertTo-IndentedLines).Trim();
                        if ($c -ne $null) { "`n$c" }
                    }
                }) -join ',') + ' )';
            } else {
                return '@(' + ((@($Value) | ForEach-Object {
                    if ($MaxItems -gt 0 -and ($MaxItems = $MaxItems - 1) -gt 0) {
                        $c = ((ConvertTo-PSLiteral -Value $_ -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems) | ConvertTo-IndentedLines).Trim();
                        if ($c -ne $null) { "`n$c" }
                    }
                }) -join ',') + ' )';
            }
            break;
        }
        { $Type.IsEnum } {
            if ($_.GetType().GetCustomAttributes([System.FlagsAttribute], $false).Length -eq 0) {
                if ($PSBoundParameters.ContainsKey('ImplictType') -and $ImplictType.AssemblyQualifiedName -ceq $Type.AssemblyQualifiedName) {
                    $Code = [System.Enum]::GetName($Type, $Value);
                } else {
                    $Code = "$([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type.FullName))::$([System.Enum]::GetName($Type, $Value))";
                    $Type = $null;
                }
            } else {
                $nArr = @();
                if ($PSBoundParameters.ContainsKey('ImplictType') -and $ImplictType.AssemblyQualifiedName -ceq $Type.AssemblyQualifiedName) {
                    $nArr = @([System.Enum]::GetNames($Type) | ForEach-Object {
                        $v = [System.Enum]::Parse($Type, $_);
                        if ($Code -eq $null) {
                            if ($v -eq $Value) {
                                $Code = "'$_'";
                            } else {
                                if ($v -ne 0 -and ($v -band $Value) -eq $v) { "'$_'" | Write-Output }
                            }
                        }
                    });
                } else {
                    $nArr = @([System.Enum]::GetNames($Type) | ForEach-Object {
                        $v = [System.Enum]::Parse($Type, $_);
                        if ($Code -eq $null) {
                            if ($v -eq $Value) {
                                $Code = "$([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type.FullName))::$_";
                            } else {
                                if ($v -ne 0 -and ($v -band $Value) -eq $v) { "$([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type.FullName))::$_" | Write-Output }
                            }
                        }
                    });
                }
                if ($Code -eq $null) {
                    if ($nArr.Count -gt 0) {
                        if ($PSBoundParameters.ContainsKey('ImplictType') -and $ImplictType.AssemblyQualifiedName -ceq $Type.AssemblyQualifiedName) {
                            return $nArr -join ', ';
                        } else {
                            $Code = "($($nArr -join ' -bor '))";
                        }
                    } else {
                        $Code = "default($([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type.FullName)))";
                        $Type = $null;
                    }
                } else {
                    $Type = $null;
                }
            }
            break;
        }
        { $_ -is [System.IConvertible] -and $Type.IsValueType } {
            $Code = $Value.ToString();
            if (-not $Type.IsPrimitive) { $Code = ConvertTo-PSLiteral -Value $Code }
            break;
        }
        default {
            $Properties = @{};
            $Value | Get-Member -MemberType Properties | ForEach-Object {
                $Properties[$_.Name] = $Value.$($_.Name);
            }
            if ($Properties.Count -gt 0) {
                $Code = ConvertTo-PSLiteral -Value $Properties -MaxDepth ($MaxDepth - 1) -MaxItems $MaxItems -NoBoxing;
                if ($Code -ne $null) {
                    $Code = "[pscustomobject]$Code";
                } else {
                    $Code = '[pscustomobject]@{}';
                }
            } else {
                $Code = '[pscustomobject]@{}';
            }
            $Type = $null;
            break;
        }
    }
    if ($PSBoundParameters.ContainsKey('ImplictType') -and ($Type -eq $null -or $ImplictType.AssemblyQualifiedName -ceq $Type.FullName)) { return $Code }
    if ($Type -ne $null) { $Code = "$([System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type.FullName))$Code" }
    if ($NoBoxing) { return $Code }
    return "($Code)";
}

Function New-AddDataColumnArgs {
    [CmdletBinding(DefaultParameterSetName = "Opt")]
    Param(
        [ValidateSet('Pipline', 'Positional', 'ByName', 'AltName')]
        [string]$TableSpec = 'Pipline',
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [Parameter(Mandatory = $true, ParameterSetName = "Expression")]
        [string]$ColumnName,

        [ValidateSet('Positional', 'ByName', 'AltName')]
        [string]$ColSpec = 'Positional',
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [Parameter(Mandatory = $true, ParameterSetName = "Expression")]
        [Type]$DataType,
        
        [switch]$DataTypeByName,

        [Parameter(Mandatory = $true, ParameterSetName = "Expression")]
        [Parameter(ParameterSetName = "Clone")]
        [string]$Expression,
        
        [Parameter(ParameterSetName = "Clone")]
        [Parameter(ParameterSetName = "Expression")]
        [ValidateSet('Positional', 'ByName', 'AltName')]
        [string]$ExprSpec = 'Positional',
        
        [Parameter(ParameterSetName = "Clone")]
        [Parameter(ParameterSetName = "Expression")]
        [System.Data.MappingType]$MappingType,
        
        [Parameter(ParameterSetName = "Clone")]
        [Parameter(ParameterSetName = "Expression")]
        [ValidateSet('Positional', 'ByName', 'AltName')]
        [string]$TypeSpec = 'Positional',

        [string]$Caption,

        [switch]$AllowDBNull,
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [switch]$AutoIncrement,
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [long]$AutoIncrementSeed,
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [long]$AutoIncrementStep,

        [System.Data.DataSetDateTime]$DateTimeMode,

        [object]$DefaultValue,
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [int]$MaxLength,
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [switch]$ReadOnly,
        
        [Parameter(ParameterSetName = "Opt")]
        [Parameter(ParameterSetName = "Clone")]
        [switch]$Unique,
        
        [switch]$IsPassThru,
        
        [Parameter(ValueFromPipeline = $true, ParameterSetName = 'Clone')]
        [psobject]$Clone
    )

    Process {
        if ($PSBoundParameters.ContainsKey('Clone')) {
            $Splat = @{};
            ($Clone | Get-Member -MemberType Properties) | ForEach-Object {
                if ($_.Name -ne 'Code' -and $_.Name -ne 'Should') {
                    $Splat[$_.Name] = $Clone.($_.Name)
                }
            }
            $PSBoundParameters.Keys | ForEach-Object {
                if ($_ -ne 'Clone') { $Splat[$_] = $PSBoundParameters[$_] }
            }
            New-AddDataColumnArgs @splat;
        } else {
            $ArgStrings = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.String]';
            $ArgStrings.Add('Add-DataColumn');
            $Properties = @{ };
            $ShouldAddStrings = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.String]';
            if ($TableSpec -eq 'Positional') { $ArgStrings.Add('$DataTable') }
            if ($PSBoundParameters.ContainsKey('ColumnName') -and $ColSpec -eq 'Positional') { $ArgStrings.Add((ConvertTo-PSLiteral -Value $ColumnName)) }
            if ($PSBoundParameters.ContainsKey('DataType') -and -not $DataTypeByName) { $ArgStrings.Add((ConvertTo-PSLiteral -Value $DataType)) }
            if ($PSBoundParameters.ContainsKey('Expression') -and $ExprSpec -eq 'Positional') { $ArgStrings.Add((ConvertTo-PSLiteral -Value $Expression)) }
            if ($PSBoundParameters.ContainsKey('MappingType') -and $TypeSpec -eq 'Positional') { $ArgStrings.Add((ConvertTo-PSLiteral -Value $MappingType -ImplictType ([System.Data.MappingType]))) }
            
            if ($TableSpec -eq 'ByName') {
                $ArgStrings.Add('-Table $DataTable');
            } else {
                if ($ColSpec -eq 'AltName') {
                    $ArgStrings.Add('-DataTable $DataTable');
                } else {
                    $ArgStrings[0] = ",`$DataTable | $($ArgStrings[0])";
                }
            }
            if ($PSBoundParameters.ContainsKey('TableSpec')) { $Properties['TableSpec'] = $TableSpec } 
            if ($PSBoundParameters.ContainsKey('ColumnName')) {
                $ShouldAddStrings.Add("ColumnName = $(ConvertTo-PSLiteral -Value $ColumnName)");
                $Properties['ColumnName'] = $ColumnName;
                if ($ColSpec -eq 'ByName') {
                    $ArgStrings.Add("-ColumnName $(ConvertTo-PSLiteral -Value $ColumnName)");
                } else {
                    if ($ColSpec -eq 'AltName') { $ArgStrings.Add("-Name $(ConvertTo-PSLiteral -Value $ColumnName)") }
                }
            }
            if ($PSBoundParameters.ContainsKey('ColSpec')) { $Properties['ColSpec'] = $ColSpec } 
            if ($PSBoundParameters.ContainsKey('DataType')) {
                $ShouldAddStrings.Add("DataType = $(ConvertTo-PSLiteral -Value $DataType -NoBoxing)");
                $Properties['DataType'] = $DataType;
                if ($DataTypeByName) { $ArgStrings.Add("-DataType $(ConvertTo-PSLiteral -Value $DataType)") }
            }
            if ($PSBoundParameters.ContainsKey('DataTypeByName')) { $Properties['DataTypeByName'] = $DataTypeByName } 
            if ($PSBoundParameters.ContainsKey('Expression')) {
                $ShouldAddStrings.Add("Expression = $(ConvertTo-PSLiteral -Value $Expression)");
                $Properties['Expression'] = $Expression;
                if ($ExprSpec -eq 'ByName') {
                    $ArgStrings.Add("-Expr $(ConvertTo-PSLiteral -Value $Expression)");
                } else {
                    if ($ExprSpec -eq 'AltName') { $ArgStrings.Add("-Expression $(ConvertTo-PSLiteral -Value $Expression)") }
                }
            }
            if ($PSBoundParameters.ContainsKey('ExprSpec')) { $Properties['ExprSpec'] = $ExprSpec } 
            if ($PSBoundParameters.ContainsKey('MappingType')) {
                $ShouldAddStrings.Add("ColumnMapping = $(ConvertTo-PSLiteral -Value $MappingType)");
                $Properties['MappingType'] = $MappingType;
                if ($TypeSpec -eq 'ByName') {
                    $ArgStrings.Add("-Type $(ConvertTo-PSLiteral -Value $MappingType -ImplictType ([System.Data.MappingType]))");
                } else {
                    if ($TypeSpec -eq 'AltName') { $ArgStrings.Add("-MappingType $(ConvertTo-PSLiteral -Value $MappingType -ImplictType ([System.Data.MappingType]))") }
                }
            }
            if ($PSBoundParameters.ContainsKey('TypeSpec')) { $Properties['TypeSpec'] = $TypeSpec } 
            if ($PSBoundParameters.ContainsKey('Caption')) {
                $ShouldAddStrings.Add("Caption = $(ConvertTo-PSLiteral -Value $Caption)");
                $Properties['Caption'] = $Caption;
                $ArgStrings.Add("-Caption $(ConvertTo-PSLiteral -Value $Caption)");
            }
            if (-not $PSBoundParameters.ContainsKey('Expression')) {
                $ShouldAddStrings.Add("AllowDBNull = $(ConvertTo-PSLiteral -Value $AllowDBNull.IsPresent -NoBoxing)");
                $ShouldAddStrings.Add("AutoIncrement = $(ConvertTo-PSLiteral -Value $AutoIncrement.IsPresent -NoBoxing)");
            }
            if ($AllowDBNull.IsPresent) { $ArgStrings.Add('-AllowDBNull') }
            if ($AutoIncrement.IsPresent) { $ArgStrings.Add('-AutoIncrement') }
            if ($PSBoundParameters.ContainsKey('AutoIncrementSeed')) {
                $ShouldAddStrings.Add("AutoIncrementSeed = $(ConvertTo-PSLiteral -Value $AutoIncrementSeed -ImplictType ([long]))");
                $Properties['AutoIncrementSeed'] = $AutoIncrementSeed;
                $ArgStrings.Add("-AutoIncrementSeed $(ConvertTo-PSLiteral -Value $AutoIncrementSeed -ImplictType ([long]))");
            }
            if ($PSBoundParameters.ContainsKey('AutoIncrementStep')) {
                $ShouldAddStrings.Add("AutoIncrementStep = $(ConvertTo-PSLiteral -Value $AutoIncrementStep -ImplictType ([long]))");
                $Properties['AutoIncrementStep'] = $AutoIncrementStep;
                $ArgStrings.Add("-AutoIncrementStep $(ConvertTo-PSLiteral -Value $AutoIncrementStep -ImplictType ([long]))");
            }
            if ($PSBoundParameters.ContainsKey('DateTimeMode')) {
                $ShouldAddStrings.Add("DateTimeMode = $(ConvertTo-PSLiteral -Value $DateTimeMode -ImplictType ([System.Data.DataSetDateTime]))");
                $Properties['DateTimeMode'] = $DateTimeMode;
                $ArgStrings.Add("-DateTimeMode $(ConvertTo-PSLiteral -Value $DateTimeMode -ImplictType ([System.Data.DataSetDateTime]))");
            }
            if ($PSBoundParameters.ContainsKey('DefaultValue')) {
                $ShouldAddStrings.Add("DefaultValue = $(ConvertTo-PSLiteral -Value $DefaultValue)");
                $Properties['DefaultValue'] = $DefaultValue;
                $ArgStrings.Add("-DefaultValue $(ConvertTo-PSLiteral -Value $DefaultValue)");
            }
            if ($PSBoundParameters.ContainsKey('MaxLength')) {
                $ShouldAddStrings.Add("MaxLength = $(ConvertTo-PSLiteral -Value $MaxLength -NoBoxing)");
                $Properties['MaxLength'] = $MaxLength;
                $ArgStrings.Add("-MaxLength $(ConvertTo-PSLiteral -Value $MaxLength)");
            }
            if (-not $PSBoundParameters.ContainsKey('Expression')) {
                $ShouldAddStrings.Add("ReadOnly = $(ConvertTo-PSLiteral -Value $ReadOnly.IsPresent -NoBoxing)");
                $ShouldAddStrings.Add("Unique = $(ConvertTo-PSLiteral -Value $Unique.IsPresent -NoBoxing)");
            }
            if ($ReadOnly.IsPresent) {
                $Properties['ReadOnly'] = $ReadOnly;
                $ArgStrings.Add('-ReadOnly');
            }
            if ($Unique.IsPresent) {
                $Properties['Unique'] = $Unique;
                $ArgStrings.Add('-Unique');
            }
            $Properties['Should'] = "Add new $(ConvertTo-PSLiteral -Value ([System.Data.DataColumn]))@{`n  $($ShouldAddStrings -join ";`n  ");`n}";
            if ($IsPassThru.IsPresent) {
                $Properties['IsPassThru'] = $IsPassThru;
                $ArgStrings.Add('-PassThru');
                $Properties['Should'] = "$($Properties['Should']) and return the $(ConvertTo-PSLiteral -Value ([System.Data.DataTable]))";
            } else {
                $Properties['Should'] = "$($Properties['Should']) and return `$null";
            }
            $Properties['Code'] = $ArgStrings -join ' ';
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
        }
    }
}

$LastMappingType = [System.Data.MappingType]::Attribute;
$counter = 0;
@(
    (New-AddDataColumnArgs)
    (New-AddDataColumnArgs -TableSpec Positional -ColumnName 'MyCol'),
    (New-AddDataColumnArgs -ColumnName 'C' -DataType ([string])),
    (New-AddDataColumnArgs -TableSpec Positional -ColumnName 'TheSum' -DataType ([int]) -Expression '[C1]+[C2]'),
    (New-AddDataColumnArgs -ColumnName 'Avg2' -DataType ([float]) -Expression '[C1]/[C2]' -MappingType Element),
    (New-AddDataColumnArgs -TableSpec Positional -ColumnName 'ID' -DataType ([long]) -AutoIncrement),
    (New-AddDataColumnArgs -ColumnName 'id' -DataType ([System.UInt64]) -AutoIncrement -AutoIncrementSeed 512),
    (New-AddDataColumnArgs -TableSpec Positional -ColumnName 'Id' -DataType ([System.UInt32]) -AutoIncrement -AutoIncrementSeed 8000 -AutoIncrementStep 100),
    (New-AddDataColumnArgs -ColumnName 'Active' -DataType ([bool]) -DefaultValue $true)
    (New-AddDataColumnArgs -TableSpec ByName)
    (New-AddDataColumnArgs -TableSpec Pipline -ColumnName 'MyCol' -ColSpec ByName),
    (New-AddDataColumnArgs -TableSpec AltName -ColumnName 'MyCol' -ColSpec AltName -DataType ([DateTime]) -DataTypeByName),
    (New-AddDataColumnArgs -TableSpec AltName -ColumnName 'MyCol' -ColSpec AltName -DataType ([DateTime]) -DataTypeByName -DateTimeMode Local),
    (New-AddDataColumnArgs -TableSpec ByName -ColumnName 'MyCol' -ColSpec ByName -DataType ([int]) -DataTypeByName -Expression '[C1]+[C2]' -ExprSpec ByName),
    (New-AddDataColumnArgs -TableSpec Pipline -ColumnName 'MyCol' -ColSpec AltName -DataType ([float]) -DataTypeByName -Expression '[C1]/[C2]' -ExprSpec AltName -MappingType Attribute -TypeSpec ByName),
    (New-AddDataColumnArgs -TableSpec AltName -ColumnName 'MyCol' -ColSpec ByName -DataType ([long]) -DataTypeByName -AutoIncrement),
    (New-AddDataColumnArgs -TableSpec ByName -ColumnName 'MyCol' -ColSpec AltName -DataType ([System.UInt64]) -DataTypeByName -AutoIncrement -AutoIncrementSeed 512),
    (New-AddDataColumnArgs -TableSpec Pipline -ColumnName 'MyCol' -ColSpec ByName -DataType ([System.UInt32]) -DataTypeByName -AutoIncrement -AutoIncrementSeed 8000 -AutoIncrementStep 100),
    (New-AddDataColumnArgs -TableSpec AltName -ColumnName 'MyCol' -ColSpec AltName -DataType ([bool]) -DataTypeByName -DefaultValue $true)
) | ForEach-Object {
    $_ | Write-Output;
    $splat = @{ Clone = $_ };
    if ($_.AutoIncrementSeed -ne $null) { $splat['AutoIncrementSeed'] = 0 }
    if ($_.MappingType -ne $null) {
        if ($LastMappingType -eq [System.Data.MappingType]::Element) {
            $LastMappingType = [System.Data.MappingType]::Attribute;
        } else {
            if ($LastMappingType -eq [System.Data.MappingType]::Attribute) {
                $LastMappingType = [System.Data.MappingType]::Hidden;
            } else {
                if ($LastMappingType -eq [System.Data.MappingType]::Hidden) {
                    $LastMappingType = [System.Data.MappingType]::SimpleContent;
                } else {
                    $LastMappingType = [System.Data.MappingType]::Element;
                }
            }
        }
        $splat['MappingType'] = $LastMappingType;
        if ($_.TypeSpec -ne $null) {
            $counter++;
            if (($counter % 3) -eq 1) {
                $splat['TypeSpec'] = 'ByName';
            } else {
                $splat['TypeSpec'] = 'AltName';
            }
        }
        #New-AddDataColumnArgs @splat -Caption '' -DefaultValue '' -AllowDBNull -Unique -ReadOnly -MaxLength;
    }
    if ($_.ColumnName -ne $null) {
        $counter++;
        if ($counter % 4 -eq 1) { 
            switch ($_.ColumnName) {
                'ID' { $splat['Caption'] = 'Identifier'; }
                'C' { $splat['Caption'] = 'Celcius'; }
                'Avg2' { $splat['Caption'] = '2-Value Average'; }
                'Active' { $splat['Caption'] = 'Is Active'; }
                default { $splat['Caption'] = 'My Column'; }
            }
        }
    }
    if ($splat.Count -gt 1) { New-AddDataColumnArgs @splat }
} | ForEach-Object {
    $_ | Write-Output;
    
    New-AddDataColumnArgs -Clone $_ -IsPassThru;
} | ForEach-Object { $_.Code; $_.Should }
#>