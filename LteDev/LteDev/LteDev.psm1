<#
.SYNOPSIS
Get simple CLR full name
 
.DESCRIPTION
Displays simple CLR full name of a type

.PARAMETER InputType
Type of CLR object

.PARAMETER InputObject
Object for get CRL full name

.PARAMETER ForceFullName
Always include namespace

.PARAMETER NameOnly
Do not include the namespace

.EXAMPLE
Get-ClrSimpleFullName -InputType [System.Uri]

.EXAMPLE
$MyObject.GetType() | Get-ClrSimpleFullName -NameOnly

.EXAMPLE
$MyObject | Get-ClrSimpleFullName -ForceFullName

.NOTES
Makes type names simpler, omitting fully qualified assembly names and generic parameter count references.
#>
Function Get-ClrSimpleName {
	[CmdletBinding(DefaultParameterSetName = 'Type')]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Type')]
		[System.Type]$InputType,
        
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Object')]
		[object[]]$InputObject,
        
        [Parameter(Mandatory = $false)]
        [switch]$ForceFullName,
        
        [Parameter(Mandatory = $false)]
        [switch]$NameOnly
    )
    
    Process {
        if ($PSBoundParameters.ContainsKey('InputObject')) {
            if ($ForceFullName) {
                $InputObject | ForEach-Object { $_.GetType() } | Get-ClrSimpleName -ForceFullName;
            } else {
                $InputObject | ForEach-Object { $_.GetType() } | Get-ClrSimpleName;
            }
        } else {
            if ($InputType.IsArray) {
                if ($ForceFullName) {
                    '{0}[{1}]' -f ($InputType.GetElementType() | Get-ClrSimpleName -ForceFullName), (New-Object -TypeName 'System.String' -ArgumentList (',', ($InputType.GetArrayRank() - 1)));
                } else {
                    '{0}[{1}]' -f ($InputType.GetElementType() | Get-ClrSimpleName), (New-Object -TypeName 'System.String' -ArgumentList (',', ($InputType.GetArrayRank() - 1)));
                }
            } else {
                if ($InputType.IsGenericParameter) {
                    $InputType.Name;
                } else {
                    $n = $InputType.Namespace;
                    if ($NameOnly -or $n -eq $null) {
                        $n = '';
                    } else {
                        $n += '.';
                    }
                    $n += $InputType.Name;
                    if ($InputType.IsGenericType) {
                        if ($n -match '`\d+$') { $n = $n.SubString(0, $n.Length - $Matches[0].Length) }
                        if ($NameOnly) {
                            $n
                        } else {
                            '{0}[{1}]' -f $n, (($InputType.GetGenericArguments() | Get-ClrSimpleName -ForceFullName) -join ', ');
                        }
                    } else {
                        if ($ForceFullName -or $NameOnly) {
                            $n;
                        } else {
                            switch ($n) {
                                'System.String' { 'string'; break; }
                                'System.Boolean' { 'bool'; break; }
                                'System.Int32' { 'int'; break; }
                                'System.Int64' { 'long'; break; }
                                'System.Single' { 'float'; break; }
                                'System.Double' { 'double'; break; }
                                'System.Byte' { 'byte'; break; }
                                'System.Char' { 'char'; break; }
                                'System.Collections.Hashtable' { 'hashtable'; break; }
                                'Object' { 'object'; break; }
                                'System.Management.Automation.SwitchParameter' { 'switch'; break; }
                                default { $n; break; }
                            }
                        }
                    }
                }
            }
        }
    }
}

Function Get-FieldUsage {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Reflection.FieldInfo]$FieldInfo,
        
        [Parameter(Mandatory = $false)]
        [switch]$IncludeSpecialNamed
    )
    
    Process {
        if ($IncludeSpecialNamed -or -not $FieldInfo.IsSpecialName) {
            $FieldInfo.ToString();
            if ($FieldInfo.IsStatic) {
                $FullName = $FieldInfo.DeclaringType | Get-ClrSimpleName;
                "`t" + '[{0}]${1} = [{2}]::{3};' -f  ($FieldInfo.FieldType | Get-ClrSimpleName), ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly), $FullName, $FieldInfo.Name;
                if (-not ($FieldInfo.IsInitOnly -or $FieldInfo.IsLiteral)) {
                    "`t" + '[{0}]::{1} = ${2};' -f $FullName, $FieldInfo.Name, ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly);
                }
            } else {
                $SimpleName = $FieldInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
                "`t" + '[{0}]${1} = ${2}.{3};' -f ($FieldInfo.FieldType | Get-ClrSimpleName), ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly), $SimpleName, $FieldInfo.Name;
                if (-not ($FieldInfo.IsInitOnly -or $FieldInfo.IsLiteral)) {
                    "`t" + '${0}.{1} = ${2};' -f $SimpleName, $FieldInfo.Name, ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly);
                }
            }
        }
    }
}

Function Get-PropertyUsage {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Reflection.PropertyInfo]$PropertyInfo,
        
        [Parameter(Mandatory = $false)]
        [switch]$IncludeSpecialNamed
    )
    
    Process {
        if ($IncludeSpecialNamed -or -not $PropertyInfo.IsSpecialName) {
            $PropertyInfo.ToString();
            $CanRead = $PropertyInfo.CanRead;
            $CanWrite = $PropertyInfo.CanWrite;
            if ($CanRead -and $PropertyInfo.GetGetMethod($false) -eq $null) { $CanRead = $false }
            if ($CanWrite -and $PropertyInfo.GetSetMethod($false) -eq $null) { $CanWrite = $false }
            if ($PropertyInfo.IsStatic) {
                $FullName = $PropertyInfo.DeclaringType | Get-ClrSimpleName;
                if ($CanRead) {
                    "`t" + '[{0}]${1} = [{2}]::{3};' -f  ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly), $FullName, $PropertyInfo.Name;
                } else {
                    "`t" + '[{0}]${1} = $value;' -f  ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
                if ($CanWrite) {
                    "`t" + '[{0}]::{1} = ${2};' -f $FullName, $PropertyInfo.Name, ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
            } else {
                $SimpleName = $PropertyInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
                if ($CanRead) {
                    "`t" + '[{0}]${1} = ${2}.{3};' -f ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly), $SimpleName, $PropertyInfo.Name;
                } else {
                    "`t" + '[{0}]${1} = $value;' -f  ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
                if ($CanWrite) {
                    "`t" + '${0}.{1} = ${2};' -f $SimpleName, $PropertyInfo.Name, ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
            }
        }
    }
}

Function Get-ParameterUsage {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Reflection.MethodBase]$Method,
        
		[Parameter(Mandatory = $false)]
		[switch]$IsInitVars,
        
		[Parameter(Mandatory = $false)]
		[switch]$NonOptional
    )
    Process {
        $Parameters = @($Method.GetParameters());
        if ($NonOptional) {
            $OptionalParameters = @($Parameters | Where-Object { -not $_.IsOptional });
            if ($OptionalParameters.Count -eq $Parameters.Count) { $Parameters = @() }
        }
        if ($Parameters.Count -gt 0) {
            if ($IsInitVars) {
                for ($i = 0; $i -lt $Parameters.Length; $i++) {
                    '[{0}]${1} = $value{2}' -f ($Parameters[$i].ParameterType | Get-ClrSimpleName), $Parameters[$i].Name, $i;
                }
            } else {
                ($Parameters | ForEach-Object { '$' + $_.Name  }) -join ', ';
            }
        } else {
            if (-not $IsInitVars) { '' };
        }
    }
}

Function Get-ConstructorUsage {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Reflection.ConstructorInfo]$ConstructorInfo
    )
    
    Process {
        $ConstructorInfo.ToString();
        Get-ParameterUsage -Method $ConstructorInfo -IsInitVars | Get-IndentedLines;
        $ArgList = Get-ParameterUsage -Method $ConstructorInfo;
        $FullName = $ConstructorInfo.ReflectedType | Get-ClrSimpleName;
        $SimpleName = $ConstructorInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
        if ($ArgList.Length -eq 0) {
            "`t" + '${0} = New-Object -TypeName ''{1}''' -f $SimpleName, $FullName;
        } else {
            "`t" + '${0} = New-Object -TypeName ''{1}'' -ArgumentList {2}' -f $SimpleName, $FullName, $ArgList;
            $ArgList = Get-ParameterUsage -Method $ConstructorInfo -NonOptional;
            if ($ArgList.Length -gt 0) {'${0} = New-Object -TypeName ''{1}'' -ArgumentList {2}' -f $SimpleName, $FullName, $ArgList  }
        }
    }
}

Function Get-MethodUsage {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Reflection.MethodInfo]$MethodInfo,
        
        [Parameter(Mandatory = $false)]
        [switch]$IncludeSpecialNamed
    )
    
    Process {
        if ($IncludeSpecialNamed -or -not $MethodInfo.IsSpecialName) {
            $MethodInfo.ToString();
            Get-ParameterUsage -Method $MethodInfo -IsInitVars | Get-IndentedLines;
            $ArgList = Get-ParameterUsage -Method $MethodInfo;
            $Pre = '';
            if ($MethodInfo.ReturnType -ne [System.Void]) { $Pre = '[{0}]${1} = ' -f ($MethodInfo.ReturnType | Get-ClrSimpleName), ($MethodInfo.ReturnType | Get-ClrSimpleName -NameOnly) }
            if ($MethodInfo.IsStatic) {
                "`t" + '{0}[{1}]::{2}({3})' -f $Pre, ($MethodInfo.DeclaringType | Get-ClrSimpleName), $MethodInfo.Name, $ArgList;
            } else {
                "`t" + '{0}${1}.{2}({3})' -f $Pre, ($MethodInfo.ReflectedType | Get-ClrSimpleName -NameOnly), $MethodInfo.Name, $ArgList;
            }
            if ($ArgList.Length -gt 0 -and ($ArgList = Get-ParameterUsage -Method $MethodInfo -NonOptional).Length -gt 0) {
                if ($MethodInfo.IsStatic) {
                    "`t" + '{0}[{1}]::{2}({3})' -f $Pre, ($MethodInfo.DeclaringType | Get-ClrSimpleName), $MethodInfo.Name, $ArgList;
                } else {
                    "`t" + '{0}${1}.{2}({3})' -f $Pre, ($MethodInfo.ReflectedType | Get-ClrSimpleName -NameOnly), $MethodInfo.Name, $ArgList;
                }
            }
        }
    }
}

<#
.SYNOPSIS
Get object type sample usage
 
.DESCRIPTION
Displays sample code text for an object type, to demonstrate usage

.PARAMETER InputType
Type of CLR object

.PARAMETER InputObject
Object for which to get usage info

.PARAMETER Constructors
Return constructors

.PARAMETER Methods
Return methods

.PARAMETER Properties
Return properties

.PARAMETER Fields
Return fields

.PARAMETER Static
Return static members

.PARAMETER Instance
Return instance members

.PARAMETER Inherited
Include inherited members

.PARAMETER ShowBaseTypes
Include base type names

.EXAMPLE
Get-TypeUsage -InputType [System.Uri]

.EXAMPLE
$MyObject.GetType() | Get-TypeUsage -Methods

.EXAMPLE
$MyObject | Get-TypeUsage

.NOTES
If none of the optional switches are used, then all information will be returned.
#>
Function Get-TypeUsage {
	[CmdletBinding(DefaultParameterSetName = 'Type')]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Type')]
		[Type]$InputType,
        
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Object')]
		[object[]]$InputObject,
        
		[Parameter(Mandatory = $false)]
		[switch]$Constructors,
        
		[Parameter(Mandatory = $false)]
		[switch]$Methods,
        
		[Parameter(Mandatory = $false)]
		[switch]$Properties,
        
		[Parameter(Mandatory = $false)]
		[switch]$Fields,
        
		[Parameter(Mandatory = $false)]
		[switch]$Static,
        
		[Parameter(Mandatory = $false)]
		[switch]$Instance,
        
		[Parameter(Mandatory = $false)]
		[switch]$Inherited,
        
		[Parameter(Mandatory = $false)]
		[switch]$ShowBaseTypes
	)
	
    Begin {
        $Options = @{
            Constructors = $Constructors.IsPresent;
            Methods = $Methods.IsPresent;
            Properties = $Properties.IsPresent;
            Fields = $Fields.IsPresent;
            Static = $Static.IsPresent;
            Instance = $Instance.IsPresent;
            Inherited = $Inherited.IsPresent;
            ShowBaseTypes = $Inheritance.ShowBaseTypes;
        };
        $IsExplicit = $false;
        foreach ($key in $Options.Keys) { if ($Options[$key]) { $IsExplicit = $true; break; } }
        if ($IsExplicit) {
            if (-not ($Constructors -or $Methods -or $Properties -or $Fields)) {
                $Options.Constructors = $true;
                $Options.Methods = $true;
                $Options.Properties = $true;
                $Options.Fields = $true;
            }
            if (-not ($Static -or $Instance)) {
                $Options.Static = $true;
                $Options.Instance = $true;
            }
        } else {
            foreach ($key in @($Options.Keys)) { $Options[$key] = $true; }
        }
        
        $BindingFlags = [System.Reflection.BindingFlags]::Public;
        if ($Options.Static) { [System.Reflection.BindingFlags]$BindingFlags = $BindingFlags -bor [System.Reflection.BindingFlags]::Static }
        if ($Options.Instance) { [System.Reflection.BindingFlags]$BindingFlags = $BindingFlags -bor [System.Reflection.BindingFlags]::Instance }
        if (-not $Options.Inherited) { [System.Reflection.BindingFlags]$BindingFlags = $BindingFlags -bor [System.Reflection.BindingFlags]::DeclaredOnly }
    }

	Process {
        if ($PSBoundParameters.ContainsKey('InputObject')) {
            $splat = @{ InputType = $null }
            foreach ($key in $Options) { if ($Options[$key]) { $splat.Add($key, (New-Object -Typename 'System.Management.Automation.SwitchParameter' -ArgumentList $true)) } }
            foreach ($obj in $InputObject) {
                $splat.InputType = $obj.GetType();
                Get-TypeUsage @splat;
            }
            break;
        } else {
            $FullName = $InputType | Get-ClrSimpleName;
            $FullName;
            $SimpleName = $InputType | Get-ClrSimpleName -NameOnly;
            if ($Options.ShowbaseTypes) {
                for ($t = $InputType; $t -ne $null; $t = $t.BaseType) { "`t`$obj -is [{0}]" -f ($t | Get-ClrSimpleName) };
                $InputType.GetInterfaces() | Get-ClrSimpleName | Foreach-Object { '$obj -is [{0}]' -f $_ } | Get-IndentedLines;
            }
            if ($Options.Fields) {
                $Fields = $InputType.GetFields($BindingFlags);
                if ($Fields.Count -gt 0) { $Fields | Get-FieldUsage | Get-IndentedLines }
            }
            if ($Options.Properties) {
                $Properties = $InputType.GetProperties($BindingFlags);
                if ($Properties.Count -gt 0) { $Properties | Get-PropertyUsage | Get-IndentedLines }
            }
            if ($Options.Constructors -and -not ($InputType.IsAbstract -or $InputType.IsInterface)) {
                $Constructors = $InputType.GetConstructors();
                if ($Constructors.Count -gt 0) { $Constructors | Get-ConstructorUsage | Get-IndentedLines }
            }
            if ($Options.Methods) {
                $Methods = $InputType.GetMethods($BindingFlags);
                if ($Methods.Count -gt 0) { $Methods | Get-MethodUsage | Get-IndentedLines }
            }
        }
	}
}