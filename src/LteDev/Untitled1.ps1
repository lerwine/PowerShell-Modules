Function Convert-ToTeamworksUri {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Uri]$Uri
    )
    
    Process {
        # https://teamworksportal.gdit.com/SitePages/,DanaInfo=thezone.spspi.gdit.com,SSL+Home.aspx
        # https://thezone.spspi.gdit.com/SitePages/Home.aspx
        
        # https://teamworksportal.gdit.com/owa/,DanaInfo=mail.gdit.com,SSL,SSO=P+#path=/mail
        # https://mail.gdit.com/owa/Home.aspx?P+#path=/mail
        
        # https://teamworksportal.gdit.com/Lists/Tasks/,DanaInfo=thezone.spspi.gdit.com,SSL+AllItems.aspx?View={c92e263c-ac1d-4dc7-83c8-d6133b9b85aa}&SortField=AssignedTo&SortDir=Asc
    }
}

Function ConvertTo-UriBuilder {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Uri
    )
    
    Process {
    }
}
Function ConvertFrom-UrlEncoded {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputText
    )
    
    Process {
        if ($InputText.StartsWith('?')) {
            ConvertFrom-UrlEncoded -InputText $InputText.SubString(1);
        } else {
            $Hashtable = @{};
            $InputText.Split('&') | ForEach-Object {
                $KeyValuePair = $_.Split('=', 2);
                if ($KeyValuePair.Length -eq 1) {
                    $Hashtable[[System.Uri]::UrlDecode($KeyValuePair[0])] = $null;
                } else {
                    $Hashtable[[System.Uri]::UrlDecode($KeyValuePair[0])] = [System.Uri]::UrlDecode($KeyValuePair[1]);
                }
            }
        }
    }
}

'https://teamworksportal.gdit.com/Lists/Tasks/,DanaInfo=thezone.spspi.gdit.com,SSL+AllItems.aspx?View={c92e263c-ac1d-4dc7-83c8-d6133b9b85aa}&SortField=AssignedTo&SortDir=Asc';
Function Push-LineText {
    [CmdletBinding(DefaultParameterSetName = 'AllLines_TrimEnd')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputText,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$Insert,
        
        [int]$SkipChars = 0,
        
        [ValidateRange(0, 2147483647)]
        [int]$SkipLines = 0,
        
        [ValidateRange(1, 2147483647)]
        [int]$Count = 1,
        
        [Parameter(ParameterSetName = 'AllLines_TrimEnd')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_TrimEnd')]
        [switch]$TrimEnd,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'AllLines_TrimStart')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_TrimStart')]
        [switch]$TrimStart,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'AllLines_Trim')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_Trim')]
        [switch]$Trim,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'AllLines_NoTrim')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_NoTrim')]
        [switch]$NoTrim,
        
        [Parameter(ParameterSetName = 'AllLines_TrimEnd')]
        [Parameter(ParameterSetName = 'AllLines_TrimStart')]
        [Parameter(ParameterSetName = 'AllLines_Trim')]
        [Parameter(ParameterSetName = 'AllLines_NoTrim')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_TrimEnd')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_TrimStart')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_Trim')]
        [Parameter(Mandatory = $true, ParameterSetName = 'SkipLines_NoTrim')]
        [switch]$SplitLines
    )
    
    Begin { if ($Count -gt 1) { $InsertedText = -join @(for ($i = 0; $i -lt $Count; $i++) { $Insert }) } else { $InsertedText = $Insert } }
    
    Process {
        if ($SplitLines) { $Text = @($InputText -split '\r\n?|\n') } else { $Text = @($InputText) }
        if ($SkipLines -gt 0) {
            if ($SkipLines -lt $Text.Length) {
                $AllLines = $Text;
                $Text = @();
            } else {
                $AllLines = $Text[0..($SkipLines - 1)]
                $Text = @($Text[$SkipLines..($a.Length - 1)]);
            }
        } else {
            $AllLines = @();
        }
        if ($Text.Count -gt 0) {
            if ($SkipChars -eq 0) {
                $AllLines += @($Text | ForEach-Object { $InsertedText + $_ });
            } else {
                $AllLines += @($Text | ForEach-Object {
                    $i = $_.Length + $SkipChars;
                    if ($i -lt 1) {
                        $InsertedText + $_;
                    } else {
                        if ($i -lt $_.Length) {
                            $_.SubString(0, $i) + $InsertedText + $_.SubString($i);
                        } else {
                            $_ + $InsertedText;
                        }
                    }
                });
            }
        }
        switch ($PSCmdlet.ParameterSetName.Split('_')[1]) {
            'TrimEnd' { $AllLines | ForEach-Object { $_.TrimEnd() } | Write-Output; break; }
            'TrimStart' { $AllLines | ForEach-Object { $_.TrimStart() } | Write-Output; break; }
            'Trim' { $AllLines | ForEach-Object { $_.Trim() } | Write-Output; break; }
            default { $AllLines | Write-Output; break; }
        }
    }
}

$Script:GenericNameParamCountRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^(?<name>[^`]+)(`(?<count>\d+))?$';

Function Get-PSTypeFullName {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Type]$InputType,
        
        [switch]$NoAccelerators
    )
    
    Begin {
        if (-not $NoAccelerators) {
            $TypeAccelerators = [System.Type]::GetType('System.Management.Automation.TypeAccelerators')::Get;
            $Mapping = @{};
            $TypeAccelerators.Keys | ForEach-Object { $Mapping[$TypeAccelerators[$_].AssemblyQualifiedName] = $_ }
        }
    }
    
    Process {
        if ($InputType.AssemblyQualifiedName -eq $null -or $NoAccelerators -or -not $Mapping.ContainsKey($InputType.AssemblyQualifiedName)) {
            switch ($InputType) {
                { $_.HasElementType } {
                    $ElementType = $InputType.GetElementType();
                    if ($NoAccelerators) {
                        (Get-PSTypeFullName -InputType $ElementType -NoAccelerators) + $InputType.Name.Substring($ElementType.Name.Length);
                    } else {
                        (Get-PSTypeFullName -InputType $ElementType) + $InputType.Name.Substring($ElementType.Name.Length);
                    }
                    break;
                }
                { $_.IsGenericParameter } {
                    $InputType.Name;
                }
                { $_.IsGenericType } {
                    $Name = $Script:GenericNameParamCountRegex.Match($InputType.Name).Groups['name'].Value;
                    if ($InputType.IsNestedType) {
                        if ($NoAccelerators) {
                            $Name = '{0}+{1}' -f (Get-PSTypeFullName -InputType $InputType.DeclaringType -NoAccelerators), $Name;
                        } else {
                            $Name = '{0}+{1}' -f (Get-PSTypeFullName -InputType $InputType.DeclaringType), $Name;
                        }
                    } else {
                        if (-not [System.String]::IsNullOrEmpty($InputType.Namespace)) {
                            $Name = '{0}.{1}' -f $InputType.Namespace, $Name;
                        }
                    }
                    '{0}[{1}]' -f $Name, (($InputType.GetGenericArguments() | Get-PSTypeFullName) -join ',');
                }
                default {
                    if ($InputType.IsNestedType) {
                        if ($NoAccelerators) {
                            '{0}+{1}' -f (Get-PSTypeFullName -InputType $InputType.DeclaringType), $InputType.Name;
                        } else {
                            '{0}+{1}' -f (Get-PSTypeFullName -InputType $InputType.DeclaringType), $InputType.Name;
                        }
                    } else {
                        if ([System.String]::IsNullOrEmpty($InputType.Namespace)) {
                            $InputType.Name;
                        } else {
                            '{0}.{1}' -f $InputType.Namespace, $InputType.Name;
                        }
                    }
                }
            }
        } else {
            $Mapping[$InputType.AssemblyQualifiedName];
        }
    }
}

Function Format-PropertyInfo {
    [CmdletBinding(DefaultParameterSetName = 'Static')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Reflection.PropertyInfo]$Property,
        
        [Parameter(ParameterSetName = 'Static')]
        [switch]$Static,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Instance')]
        [switch]$Instance
    )
    
    Process {
        $GetMethod = $Property.GetGetMethod();
        $SetMethod = $Property.GetSetMethod();
        if ($GetMethod -eq $null) {
            $IsStatic = $SetMethod.IsStatic;
        } else {
            $IsStatic = $GetMethod.IsStatic;
        }
        
        if (($Static -and $IsStatic) -or ($Instance -and -not $IsStatic) -or -not ($Static -or $Instance)) {
            $CanRead = $Property.CanRead -and $GetMethod -ne $null -and $GetMethod.IsPublic;
            $CanWrite = $Property.CanWrite -and $SetMethod -ne $null -and $SetMethod.IsPublic;
            $t = $Property.PropertyType;
            while ($t.HasElementType) { $t = $t.GetElementType() }
            $VarName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
            '';
            '# Property: {0}' -f $Property.Name;
            $PropertyType = Get-PSTypeFullName -InputType $Property.PropertyType;
            $Parameters = $Property.GetIndexParameters();
            if ($Parameters.Length -eq 0) {
                if ($IsStatic) {
                    if ($CanWrite) {
                        if ($CanRead) {
                            ('[{0}]${1} = [{2}]::{3};', '[{2}]::{3} = ${1};') | ForEach-Object { $_ -f $PropertyType, $VarName, (Get-PSTypeFullName -InputType $Property.ReflectedType), $Property.Name }
                        } else {
                            ('[{0}]${1} = $obj;', '[{2}]::{3} = ${1};') | ForEach-Object { $_ -f $PropertyType, $VarName, (Get-PSTypeFullName -InputType $Property.ReflectedType), $Property.Name }
                        }
                    } else {
                        '[{0}]${1} = [{2}]::{3};' -f $PropertyType, $VarName, (Get-PSTypeFullName -InputType $Property.ReflectedType), $Property.Name;
                    }
                } else {
                    $t = $Property.ReflectedType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    $ObjectName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                    if ($CanWrite) {
                        if ($CanRead) {
                            ('[{0}]${1} = ${2}.{3};', '${2}.{3} = ${1};') | ForEach-Object { $_ -f $PropertyType, $VarName, $ObjectName, $Property.Name }
                        } else {
                            ('[{0}]${1} = $obj;', '${2}.{3} = ${1};')| ForEach-Object { $_  -f $PropertyType, $VarName, $ObjectName, $Property.Name }
                        }
                    } else {
                        '[{0}]${1} = ${2}.{3};' -f $PropertyType, $VarName, $ObjectName, $Property.Name;
                    }
                }
            } else {
                $StringBuilder = New-Object -TypeName 'System.Text.StringBuilder';
                $Parameters | ForEach-Object {
                    if ($_.Name -eq $null) {
                        $t = $_.ParameterType;
                        while ($t.HasElementType) { $t = $t.GetElementType() }
                        if ($Paramters.Count -eq 1) {
                            $Name = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                        } else {
                            $Name = '{0}_{1}' -f $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value, $_.Position;
                        }
                    } else {
                        $Name = $_.Name.Substring(0, 1).ToUpper() + $_.Name.Substring(1);
                    }
                    if ($StringBuilder.Length -gt 0) { $StringBuilder.Append(', ') | Out-Null }
                    $StringBuilder.Append('$') | Out-Null;
                    $StringBuilder.Append($Name) | Out-Null;
                    $t = $_.ParameterType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    '[{0}]${1} = ${2}Value;' -f (Get-PSTypeFullName -InputType $_.ParameterType), $Name, ($Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value);
                }
                if ($IsStatic) {
                    if ($CanWrite) {
                        if ($CanRead) {
                            ('[{0}]${1} = [{2}]::[{3}];', '[{2}]::[{3}] = ${1};') | ForEach-Object { $_ -f $PropertyType, $VarName, (Get-PSTypeFullName -InputType $Property.ReflectedType), $StringBuilder.ToString() }
                        } else {
                            ('[{0}]${1} = $obj;', '[{2}]::[{3}] = ${1};') | ForEach-Object { $_ -f $PropertyType, $VarName, (Get-PSTypeFullName -InputType $Property.ReflectedType), $StringBuilder.ToString() }
                        }
                    } else {
                        '[{0}]${1} = [{2}]::[{3}];' -f $PropertyType, $VarName, (Get-PSTypeFullName -InputType $Property.ReflectedType), $StringBuilder.ToString();
                    }
                } else {
                    $t = $Property.ReflectedType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    $ObjectName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                    if ($CanWrite) {
                        if ($CanRead) {
                            ('[{0}]${1} = ${2}[{3}];', '${2}[{3}] = ${1};') | ForEach-Object { $_ -f $PropertyType, $VarName, $ObjectName, $StringBuilder.ToString() }
                        } else {
                            ('[{0}]${1} = $obj;', '${2}[{3}] = ${1};')| ForEach-Object { $_  -f $PropertyType, $VarName, $ObjectName, $StringBuilder.ToString() }
                        }
                    } else {
                        '[{0}]${1} = ${2}[{3}];' -f $PropertyType, $VarName, $ObjectName, $StringBuilder.ToString();
                    }
                }
            }
        }
    }
}

Function Format-FieldInfo {
    [CmdletBinding(DefaultParameterSetName = 'Static')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Reflection.FieldInfo]$Field,
        
        [Parameter(ParameterSetName = 'Static')]
        [switch]$Static,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Instance')]
        [switch]$Instance
    )
    
    Process {
        if (($Static -and $Field.IsStatic) -or ($Instance -and -not $Field.IsStatic) -or -not ($Static -or $Instance)) {
            $t = $Field.FieldType;
            while ($t.HasElementType) { $t = $t.GetElementType() }
            $VarName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
            $FieldType = Get-PSTypeFullName -InputType $Field.FieldType;
            if ($Field.ReflectedType.IsEnum -and $Field.IsStatic) {
                '${0} = [{1}]::{2};' -f $VarName, (Get-PSTypeFullName -InputType $Field.ReflectedType), $Field.Name;
                $UnderlyingType = [System.Enum]::GetUnderlyingType($Field.ReflectedType);
                $Value = [System.Convert]::ChangeType([System.Enum]::Parse($Field.ReflectedType, $Field.Name), $UnderlyingType);
                '[{0}]$Value = {1};' -f (Get-PSTypeFullName -InputType $UnderlyingType), $Value;
                '[{0}]${1} = $Value;' -f $FieldType, $VarName;
            } else {
                if (-not $Field.ReflectedType.IsEnum) { '' }
                '# Field: {0}' -f $Field.Name;
                if ($Field.IsStatic) {
                    if ($Field.IsInitOnly -or $Field.IsLiteral) {
                        '[{0}]${1} = [{2}]::{3};' -f $FieldType, $VarName, (Get-PSTypeFullName -InputType $Field.ReflectedType), $Field.Name;
                    } else {
                        ('[{0}]${1} = [{2}]::{3};', '[{2}]::{3} = [{0}]${1};') | ForEach-Object { $_ -f $FieldType, $VarName, (Get-PSTypeFullName -InputType $Field.ReflectedType), $Field.Name }
                    }
                } else {
                    $t = $Field.ReflectedType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    $ObjectName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                    if ($Field.IsInitOnly -or $Field.IsLiteral) {
                        '[{0}]${1} = ${2}.{3};' -f $FieldType, $VarName, $ObjectName, $Field.Name;
                    } else {
                        ('[{0}]${1} = ${2}.{3};', '${2}.{3} = [{0}]${1};') | ForEach-Object { $_ -f $FieldType, $VarName, $ObjectName, $Field.Name }
                    }
                }
            }
        }
    }
}

Function Format-EventInfo {
    [CmdletBinding(DefaultParameterSetName = 'Static')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Reflection.EventInfo]$Event,
        
        [Parameter(ParameterSetName = 'Static')]
        [switch]$Static,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Instance')]
        [switch]$Instance
    )
    
    Process {
        $AddMethod = $Event.GetAddMethod();
        $RemoveMethod = $Event.GetRemoveMethod();
        if ($AddMethod -eq $null) {
            $IsStatic = $RemoveMethod.IsStatic;
        } else {
            $IsStatic = $AddMethod.IsStatic;
        }
        
        if (($Static -and $IsStatic) -or ($Instance -and -not $IsStatic) -or -not ($Static -or $Instance)) {
            '';
            'Event: {0}' -f $Event.Name;
            $Method = $Event.EventHandlerType.GetMethod('Invoke');
            if ($IsStatic) {
            } else {
                $t = $Event.ReflectedType;
                while ($t.HasElementType) { $t = $t.GetElementType() }
                $ObjName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                '${0}.add_{1}({{' -f $ObjName, $Event.Name;
                $Parameters = $Method.GetParameters();
                if ($Parameters.Count -gt 0) {
                    '    Param('
                    for ($p = 0; $p -lt $Parameters.Count; $p++) {
                        if ($p -gt 0) { '' }
                        '        [Parameter(Mandatory = $true, Position = {0})]'  -f $p;
                        if ($p -lt ($Parameters.Count - 1)) {
                            '        [{0}]${1},' -f (Get-PSTypeFullName -InputType $Parameters[$p].ParameterType), ($Parameters[$p].Name.Substring(0, 1).ToUpper() + $Parameters[$p].Name.Substring(1));
                        } else {
                            '        [{0}]${1}' -f (Get-PSTypeFullName -InputType $Parameters[$p].ParameterType), ($Parameters[$p].Name.Substring(0, 1).ToUpper() + $Parameters[$p].Name.Substring(1));
                        }
                    }
                    '    )';
                    if ($Method.ReturnType -ne [System.Void]) { '' }
                }
                if ($Method.ReturnType -ne [System.Void]) {
                    $t = $Method.ReturnType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    '    [{0}]${1} = $obj;' -f (Get-PSTypeFullName -InputType $Method.ReturnType), $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                    '    return ${0};' -f $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value
                }
                '});';
            }
            #$Event.EventHandlerType | Format-TypeMembers | Push-LineText -Insert '    ';
        }
    }
}

Function Format-MethodInfo {
    [CmdletBinding(DefaultParameterSetName = 'Static')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Reflection.MethodInfo]$Method,
        
        [Parameter(ParameterSetName = 'Static')]
        [switch]$Static,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Instance')]
        [switch]$Instance
    )
    
    Process {
        if (($Static -and $Method.IsStatic) -or ($Instance -and -not $Method.IsStatic) -or -not ($Static -or $Instance -or $Method.IsSpecialName)) {
            $MethodName = $Script:GenericNameParamCountRegex.Match($Method.Name).Groups['name'].Value;
            '';
            '# Method: {0}' -f $MethodName;
            $StringBuilder = New-Object -TypeName 'System.Text.StringBuilder';
            if ($Method.IsStatic) {
                if ($Method.ReturnType -eq [System.Void]) {
                    $StringBuilder.AppendFormat('[{0}]::', (Get-PSTypeFullName -InputType $Method.ReflectedType)) | Out-Null;
                } else {
                    $t = $Method.ReturnType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    $StringBuilder.AppendFormat('[{0}]${1} = [{2}]::', (Get-PSTypeFullName -InputType $Method.ReturnType), ($Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value), (Get-PSTypeFullName -InputType $Method.ReflectedType)) | Out-Null;
                }
            } else {
                $t = $Method.ReflectedType;
                while ($t.HasElementType) { $t = $t.GetElementType() }
                $ObjName = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                if ($Method.ReturnType -eq [System.Void]) {
                    $StringBuilder.AppendFormat('${0}.', $ObjName) | Out-Null;
                } else {
                    $t = $Method.ReturnType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    $StringBuilder.AppendFormat('[{0}]${1} = ${2}.', (Get-PSTypeFullName -InputType $Method.ReturnType), ($Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value), $ObjName) | Out-Null;
                }
            }
            $StringBuilder.Append($MethodName) | Out-Null;
            $StringBuilder.Append('(') | Out-Null;
            $LastLength = $StringBuilder.Length;
            $Parameters = $Method.GetParameters();
            if ($Parameters.Length -gt 0) {
                $Parameters | ForEach-Object {
                    $t = $_.ParameterType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    $PTypename = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                    if ($_.Name -eq $null) {
                        if ($Parameters.Length -eq 1) {
                            $Name = $PTypename;
                        } else {
                            $Name = '{0}_{1}' -f $PTypename, $_.Position;
                        }
                    } else {
                        $Name = $_.Name.Substring(0, 1).ToUpper() + $_.Name.Substring(1);
                    }
                    if ($StringBuilder.Length -gt $LastLength) { $StringBuilder.Append(', ') | Out-Null }
                    if ($_.IsOut -or $_.IsRetval) { $StringBuilder.Append('[ref]') | Out-Null }
                    $StringBuilder.Append('$') | Out-Null;
                    $StringBuilder.Append($Name) | Out-Null;
                    '[{0}]${1} = ${2}Value;' -f (Get-PSTypeFullName -InputType $_.ParameterType), $Name, $PTypename;
                }
            }
            $StringBuilder.Append(');') | Out-Null;
            $StringBuilder.ToString();
        }
    }
}

Function Format-ConstructorInfo {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Reflection.ConstructorInfo]$Constructor
    )
    
    Process {
        $Parameters = $Constructor.GetParameters();
        '';
        '# Constructor:';
        if ($Parameters.Length -gt 0) {
            $StringBuilder = New-Object -TypeName 'System.Text.StringBuilder';
            $Parameters | ForEach-Object {
                if ($_.Name -eq $null) {
                    $t = $_.ParameterType;
                    while ($t.HasElementType) { $t = $t.GetElementType() }
                    if ($Paramters.Count -eq 1) {
                        $Name = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
                    } else {
                        $Name = '{0}_{1}' -f $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value, $_.Position;
                    }
                } else {
                    $Name = $_.Name.Substring(0, 1).ToUpper() + $_.Name.Substring(1);
                }
                if ($StringBuilder.Length -gt 0) { $StringBuilder.Append(', ') | Out-Null }
                $StringBuilder.Append('$') | Out-Null;
                $StringBuilder.Append($Name) | Out-Null;
                $t = $_.ParameterType;
                while ($t.HasElementType) { $t = $t.GetElementType() }
                '[{0}]${1} = ${2}Value;' -f (Get-PSTypeFullName -InputType $_.ParameterType), $Name, ($Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value);
            }
        }
        $t = $Constructor.ReflectedType;
        while ($t.HasElementType) { $t = $t.GetElementType() }
        if ($Parameters.Length -gt 0) {
            '${0} = New-Object -TypeName ''[{1}]'' -ArgumentList {2};' -f ($Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value), (Get-PSTypeFullName -InputType $Constructor.ReflectedType), $StringBuilder.ToString();
        } else {
            '${0} = New-Object -TypeName ''[{1}]'';' -f ($Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value), (Get-PSTypeFullName -InputType $Constructor.ReflectedType);
        }
    }
}

Function Format-TypeMembers {
    [CmdletBinding(DefaultParameterSetName = 'Static')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Type]$InputType,
        
        [Parameter(Position = 1)]
        [System.Reflection.MemberTypes[]]$MemberType = [System.Reflection.MemberTypes]::All,
        
        [ValidateSet('All', 'BaseType', 'Interface')]
        [string]$Inheritance = 'All',
        
        [Parameter(ParameterSetName = 'Static')]
        [switch]$Static,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Instance')]
        [switch]$Instance
    )
    
    Begin {
        $MemberTypes = $MemberType[0];
        for ($i = 1; $i -lt $MemberType.Count; $i++) { [System.Reflection.MemberTypes]$MemberTypes = $MemberTypes -bor $MemberType[$i] }
    }
    
    Process {
        '# [{0}]' -f (Get-PSTypeFullName -InputType $InputType);
        $t = $InputType;
        while ($t.HasElementType) { $t = $t.GetElementType() }
        $Name = $Script:GenericNameParamCountRegex.Match($t.Name).Groups['name'].Value;
        $FullName = Get-PSTypeFullName -InputType $InputType;
        if ($Inheritance -ne 'Interface') {
            for ($b = $InputType.BaseType; $b -ne $null -and $b -ne [System.Object] -and $b -ne [System.ValueType] -and $b -ne [System.Array] -and $b -ne [System.Enum]; $b = $b.BaseType) {
                '    [{0}].IsAssignableFrom([{1}]); # BaseType' -f (Get-PSTypeFullName -InputType $b), $FullName;
            }
            if ($InputType.HasElementType) {
                '    ${0}[0] -is [{1}]; # ElementType' -f $Name, (Get-PSTypeFullName -InputType $InputType.GetElementType());
            }
            if ($InputType.IsGenericType -and -not $InputType.IsGenericTypeDefinition) {
                for ($b = $InputType.GetGenericTypeDefinition(); $b -ne $null -and $b -ne [System.Object] -and $b -ne [System.ValueType] -and $b -ne [System.Array] -and $b -ne [System.Enum]; $b = $b.BaseType) {
                    if ($b.IsGenericTypeDefinition) {
                        '    # GenericTypeDefinition: [{0}]' -f (Get-PSTypeFullName -InputType $b);
                    } else {
                        '    [{0}].IsAssignableFrom([{1}]); # BaseType' -f (Get-PSTypeFullName -InputType $b), $FullName;
                    }
                }
            }
        }
        if ($Inheritance -ne 'BaseType') {
            $InputType.GetInterfaces() | ForEach-Object { '    [{0}].IsAssignableFrom([{1}]); # Interface' -f (Get-PSTypeFullName -InputType $_), $FullName; }
        }
        if ($InputType.IsEnum) { '' }
        $Lines = @{};
        $LastCount = 0;
        if (($MemberTypes -band [System.Reflection.MemberTypes]::Event) -ne 0) {
            if ($Instance) {
                $InputType.GetEvents() | Format-EventInfo -Instance | Push-LineText -Insert '    ';
            } else {
                if ($Static) {
                    $InputType.GetEvents() | Format-EventInfo -Static | Push-LineText -Insert '    ';
                } else {
                    $InputType.GetEvents() | Format-EventInfo | Push-LineText -Insert '    ';
                }
            }
        }
        if (($MemberTypes -band [System.Reflection.MemberTypes]::Field) -ne 0) {
            if ($Instance) {
                $InputType.GetFields() | Format-FieldInfo -Instance | Push-LineText -Insert '    ';
            } else {
                if ($Static) {
                    $InputType.GetFields() | Format-FieldInfo -Static | Push-LineText -Insert '    ';
                } else {
                    $InputType.GetFields() | Format-FieldInfo | Push-LineText -Insert '    ';
                }
            }
        }
        if (($MemberTypes -band [System.Reflection.MemberTypes]::Property) -ne 0) {
            if ($Instance) {
                $InputType.GetProperties() | Format-PropertyInfo -Instance | Push-LineText -Insert '    ';
            } else {
                if ($Static) {
                    $InputType.GetProperties() | Format-PropertyInfo -Static | Push-LineText -Insert '    ';
                } else {
                    $InputType.GetProperties() | Format-PropertyInfo | Push-LineText -Insert '    ';
                }
            }
        }
        if (($MemberTypes -band [System.Reflection.MemberTypes]::Constructor) -ne 0) {
            $InputType.GetConstructors() | Format-ConstructorInfo | Push-LineText -Insert '    ';
        }
        if (($MemberTypes -band [System.Reflection.MemberTypes]::Method) -ne 0) {
            $InputType.GetMethods() | Format-MethodInfo | Push-LineText -Insert '    ';
        }
        if (($MemberTypes -band [System.Reflection.MemberTypes]::NestedType) -ne 0) {
            ($InputType.GetNestedTypes() | Get-PSTypeFullName | ForEach-Object { '[{0}]$NestedType;' -f $_ }) | Push-LineText -Insert '    ';
        }
    }
}

[System.Collections.Generic.Dictionary[int, string]] | Format-TypeMembers;