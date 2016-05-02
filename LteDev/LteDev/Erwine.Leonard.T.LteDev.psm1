if ((Get-Module -Name 'Erwine.Leonard.T.IOUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.IOUtility' -ErrorAction Stop }
$Script:IndentText = '  ';

Function Get-ClrSimpleName {
    <#
		.SYNOPSIS
			Get simple CLR full name
 
		.DESCRIPTION
			Displays simple CLR full name of a type

		.EXAMPLE
			Get-ClrSimpleFullName -InputType [System.Uri]

		.EXAMPLE
			$MyObject.GetType() | Get-ClrSimpleFullName -NameOnly

		.EXAMPLE
			$MyObject | Get-ClrSimpleFullName -ForceFullName

		.NOTES
			Makes type names simpler, omitting fully qualified assembly names and generic parameter count references.
	#>
	[CmdletBinding(DefaultParameterSetName = 'Type')]
	[OutputType([string])]
	Param(
		# Type of CLR object
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Type')]
		[System.Type]$InputType,
        
		# Object for get CRL full name
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Object')]
		[object[]]$InputObject,
        
		# Always include namespace
        [Parameter(Mandatory = $false)]
        [switch]$ForceFullName,
        
		# Do not include the namespace
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

Function Get-EventUsage {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Reflection.EventInfo]$EventInfo
    )
    
    Process {
        '{0}: {1}' -f $EventInfo.Name, ($EventInfo.EventHandlerType | Get-ClrSimpleName);
        $SimpleName = $EventInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
        '{0}${1}.add_{2}({{' -f $Script:IndentText, $SimpleName, $EventInfo.Name;
        $MethodInfo = $EventInfo.EventHandlerType.GetMethod('Invoke');
        $Parameters = @($MethodInfo.GetParameters());
        if ($Parameters.Count -gt 0) {
            '{0}{0}Param(' -f $Script:IndentText;
            for ($i = 0; $i -lt $Parameters.Length; $i++) {
                '{0}{0}{0}[Parameter(Mandatory = $true, Position = {1})]' -f $Script:IndentText, $i;
                $n = '{0}{0}{0}[{1}]${2}{3}' -f $Script:IndentText, ($Parameters[$i].ParameterType | Get-ClrSimpleName), $Parameters[$i].Name.Substring(0, 1).ToUpper(), $Parameters[$i].Name.Substring(1);
                if ($i -lt ($Parameters.Length - 1)) {
                    $n + ',';
                } else {
                    $n;
                }
            }
            '{0}{0})' -f $Script:IndentText;
        }
        '';
        '{0}}});' -f $Script:IndentText
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
            '{0}: {1}' -f $FieldInfo.Name, ($FieldInfo.FieldType | Get-ClrSimpleName);
            if ($FieldInfo.IsStatic) {
                $FullName = $FieldInfo.DeclaringType | Get-ClrSimpleName;
                '{0}[{1}]${2} = [{3}]::{4};' -f  $Script:IndentText, ($FieldInfo.FieldType | Get-ClrSimpleName), ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly), $FullName, $FieldInfo.Name;
                if (-not ($FieldInfo.IsInitOnly -or $FieldInfo.IsLiteral)) {
                    '{0}[{1}]::{2} = ${3};' -f $Script:IndentText, $FullName, $FieldInfo.Name, ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly);
                }
            } else {
                $SimpleName = $FieldInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
                '{0}[{1}]${2} = ${3}.{4};' -f $Script:IndentText, ($FieldInfo.FieldType | Get-ClrSimpleName), ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly), $SimpleName, $FieldInfo.Name;
                if (-not ($FieldInfo.IsInitOnly -or $FieldInfo.IsLiteral)) {
                    '{0}${1}.{2} = ${3};' -f $Script:IndentText, $SimpleName, $FieldInfo.Name, ($FieldInfo.FieldType | Get-ClrSimpleName -NameOnly);
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
            '{0}: {1}' -f $PropertyInfo.Name, ($PropertyInfo.PropertyType | Get-ClrSimpleName);
            $CanRead = $PropertyInfo.CanRead;
            $CanWrite = $PropertyInfo.CanWrite;
            if ($CanRead -and $PropertyInfo.GetGetMethod($false) -eq $null) { $CanRead = $false }
            if ($CanWrite -and $PropertyInfo.GetSetMethod($false) -eq $null) { $CanWrite = $false }
            if ($PropertyInfo.IsStatic) {
                $FullName = $PropertyInfo.DeclaringType | Get-ClrSimpleName;
                if ($CanRead) {
                    '{0}[{1}]${2} = [{3}]::{4};' -f  $Script:IndentText, ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly), $FullName, $PropertyInfo.Name;
                } else {
                    '{0}[{1}]${2} = $value;' -f  $Script:IndentText, ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
                if ($CanWrite) {
                    '{0}[{1}]::{2} = ${3};' -f $Script:IndentText, $FullName, $PropertyInfo.Name, ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
            } else {
                $SimpleName = $PropertyInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
                if ($CanRead) {
                    '{0}[{1}]${2} = ${3}.{4};' -f $Script:IndentText, ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly), $SimpleName, $PropertyInfo.Name;
                } else {
                    '{0}[{1}]${2} = $value;' -f  $Script:IndentText, ($PropertyInfo.PropertyType | Get-ClrSimpleName), ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
                }
                if ($CanWrite) {
                    '{0}${1}.{2} = ${3};' -f $Script:IndentText, $SimpleName, $PropertyInfo.Name, ($PropertyInfo.PropertyType | Get-ClrSimpleName -NameOnly);
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
        Get-ParameterUsage -Method $ConstructorInfo -IsInitVars | Out-IndentedText -IndentText $Script:IndentText;
        $ArgList = Get-ParameterUsage -Method $ConstructorInfo;
        $FullName = $ConstructorInfo.ReflectedType | Get-ClrSimpleName;
        $SimpleName = $ConstructorInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
        if ($ArgList.Length -eq 0) {
            '{0}${1} = New-Object -TypeName ''{2}''' -f $Script:IndentText, $SimpleName, $FullName;
        } else {
            '{0}${1} = New-Object -TypeName ''{2}'' -ArgumentList {3}' -f $Script:IndentText, $SimpleName, $FullName, $ArgList;
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
    
    Begin { $LastName = '' }
    
    Process {
        if ($IncludeSpecialNamed -or -not $MethodInfo.IsSpecialName) {
            '{0}: {1}' -f $MethodInfo.Name, $MethodInfo.ToString();
            Get-ParameterUsage -Method $MethodInfo -IsInitVars | Out-IndentedText -IndentText $Script:IndentText;
            $ArgList = Get-ParameterUsage -Method $MethodInfo;
            $Pre = '';
            if ($MethodInfo.ReturnType -ne [System.Void]) { $Pre = '[{0}]${1} = ' -f ($MethodInfo.ReturnType | Get-ClrSimpleName), ($MethodInfo.ReturnType | Get-ClrSimpleName -NameOnly) }
            if ($MethodInfo.IsStatic) {
                '{0}{1}[{2}]::{3}({4})' -f $Script:IndentText, $Pre, ($MethodInfo.DeclaringType | Get-ClrSimpleName), $MethodInfo.Name, $ArgList;
            } else {
                '{0}{1}${2}.{3}({4})' -f $Script:IndentText, $Pre, ($MethodInfo.ReflectedType | Get-ClrSimpleName -NameOnly), $MethodInfo.Name, $ArgList;
            }
            if ($ArgList.Length -gt 0 -and ($ArgList = Get-ParameterUsage -Method $MethodInfo -NonOptional).Length -gt 0) {
                if ($MethodInfo.IsStatic) {
                    '{0}{1}[{2}]::{3}({4})' -f $Script:IndentText, $Pre, ($MethodInfo.DeclaringType | Get-ClrSimpleName), $MethodInfo.Name, $ArgList;
                } else {
                    '{0}{1}${2}.{3}({4})' -f $Script:IndentText, $Pre, ($MethodInfo.ReflectedType | Get-ClrSimpleName -NameOnly), $MethodInfo.Name, $ArgList;
                }
            }
        }
    }
}

Function Get-TypeUsage {
	<#
		.SYNOPSIS
			Get object type sample usage
 
		.DESCRIPTION
			Displays sample code text for an object type, to demonstrate usage

		.EXAMPLE
			Get-TypeUsage -InputType [System.Uri]

		.EXAMPLE
			$MyObject.GetType() | Get-TypeUsage -Methods

		.EXAMPLE
			$MyObject | Get-TypeUsage

		.NOTES
			If none of the optional switches are used, then all information will be returned.
	#>
	[CmdletBinding(DefaultParameterSetName = 'Type')]
	[OutputType([string[]])]
	Param(
		# Type of CLR object
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Type')]
		[Type]$InputType,
        
		# Object for which to get usage info
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Object')]
		[object[]]$InputObject,
        
		# Return constructors
		[Parameter(Mandatory = $false)]
		[switch]$Constructors,
        
		# Return methods
		[Parameter(Mandatory = $false)]
		[switch]$Methods,
        
		# Return properties
		[Parameter(Mandatory = $false)]
		[switch]$Properties,
        
		# Return fields
		[Parameter(Mandatory = $false)]
		[switch]$Fields,
        
		# Return events
		[Parameter(Mandatory = $false)]
        $Events,
        
		# Return static members
		[Parameter(Mandatory = $false)]
		[switch]$Static,
        
		# Return instance (non-static) members
		[Parameter(Mandatory = $false)]
		[switch]$Instance,
        
		# Include inherited members
		[Parameter(Mandatory = $false)]
		[switch]$Inherited,
        
		# Include base type names
		[Parameter(Mandatory = $false)]
		[switch]$ShowBaseTypes
	)
	
    Begin {
        $Options = @{
            Constructors = $Constructors.IsPresent;
            Methods = $Methods.IsPresent;
            Properties = $Properties.IsPresent;
            Fields = $Fields.IsPresent;
            Events = $Events.IsPresent;
            Static = $Static.IsPresent;
            Instance = $Instance.IsPresent;
            Inherited = $Inherited.IsPresent;
            ShowBaseTypes = $Inheritance.ShowBaseTypes;
        };
        $IsExplicit = $false;
        foreach ($key in $Options.Keys) { if ($Options[$key]) { $IsExplicit = $true; break; } }
        if ($IsExplicit) {
            if (-not ($Constructors -or $Methods -or $Properties -or $Fields -or $Events)) {
                $Options.Constructors = $true;
                $Options.Methods = $true;
                $Options.Properties = $true;
                $Options.Fields = $true;
                $Options.Events = $true;
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
                if ($InputType.BaseType.BaseType -ne $null) {
                    $Script:IndentText + 'Inherits From:';
                    for ($t = $InputType.BaseType; $t.BaseType -ne $null; $t = $t.BaseType) { '{0}{0}$obj -is [{1}]' -f $Script:IndentText, ($t | Get-ClrSimpleName) };
                }
                $Interfaces = $InputType.GetInterfaces();
                if ($Interfaces.Length -gt 0) {
                    $Script:IndentText + 'Implements:';
                    $Interfaces | Get-ClrSimpleName | Foreach-Object { '{0}{0}$obj -is [{1}]' -f $Script:IndentText, $_ } | Out-IndentedText -IndentText $Script:IndentText;
                }
            }
            if ($Options.Events) {
                $Events = @($InputType.GetEvents($BindingFlags) | Sort-Object -Property 'Name');
                if ($Events.Count -gt 0) {
                    $Script:IndentText + 'Events:';
                    $Events | Get-EventUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Fields) {
                $Fields = @($InputType.GetFields($BindingFlags) | Sort-Object -Property 'Name');
                if ($Fields.Count -gt 0) {
                    $Script:IndentText + 'Fields:';
                    $Fields | Get-FieldUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Properties) {
                $Properties = @($InputType.GetProperties($BindingFlags) | Sort-Object -Property 'Name');
                if ($Properties.Count -gt 0) {
                    $Script:IndentText + 'Properties:';
                    $Properties | Get-PropertyUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Constructors -and -not ($InputType.IsAbstract -or $InputType.IsInterface)) {
                $Constructors = $InputType.GetConstructors();
                if ($Constructors.Count -gt 0) {
                    $Script:IndentText + 'Constructors:';
                    $Constructors | Get-ConstructorUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Methods) {
                $Methods = $InputType.GetMethods($BindingFlags) | Where-Object { -not $_.IsSpecialName } | Group-Object -Property 'Name' | Sort-Object -Property 'Name';
                if ($Methods.Count -gt 0) {
                    $Script:IndentText + 'Methods:';
                    foreach ($GroupInfo in $Methods) {
                        if ($GroupInfo.Group.Count -eq 1) {
                            $GroupInfo.Group[0] | Get-MethodUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                        } else {
                            '{0}{0}{1} {2} Overrides' -f $Script:IndentText, $GroupInfo.Name, $GroupInfo.Group.Count;
                            $GroupInfo.Group | Get-MethodUsage | Out-IndentedText -IndentText $Script:IndentText -Level 3;
                        }
                    }
                }
            }
        }
	}
}