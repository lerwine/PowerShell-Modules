if ($null -eq $Script:SingleQuotedLiteralToEscapeRegex) {
    New-Variable -Name 'SingleQuotedLiteralToEscapeRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new("['‘’]"));
    New-Variable -Name 'DoubleQuotedLiteralToEscapeRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('[\x00-0x19\$"`\x7f-\u2017\u201a-\uffff]'));
    New-Variable -Name 'AnyLiteralToEscapeRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('([\$"`“”''‘‘’’])'));
    New-Variable -Name 'SingleQuoteIncompatibleRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('[\x00-0x19\x7f-\u2017\u201a-\u201b\u201e-\uffff]'));
    New-Variable -Name 'EscapeChar' -Option Constant -Scope 'Script' -Value ([char]"`e");
    New-Variable -Name 'DeleteChar' -Option Constant -Scope 'Script' -Value ([char]"`u{7f}");
    New-Variable -Name 'IntegerBase10PadFormats' -Option ReadOnly -Scope 'Script' -Value ( @{
        [byte] = "d$([byte]::MaxValue().ToString().Length)";
        [sbyte] = "d$([sbyte]::MaxValue().ToString().Length)";
        [short] = "d$([short]::MaxValue().ToString().Length)";
        [ushort] = "d$([ushort]::MaxValue().ToString().Length)";
        [int] = "d$([int]::MaxValue().ToString().Length)";
        [uint] = "d$([uint]::MaxValue().ToString().Length)";
        [long] = "d$([long]::MaxValue().ToString().Length)";
        [ulong] = "d$([ulong]::MaxValue().ToString().Length)";
    });
    New-Variable -Name 'IntegerHexPadFormats' -Option ReadOnly -Scope 'Script' -Value (@{
        [byte] = "x$([byte]::MaxValue('x').ToString().Length)";
        [sbyte] = "x$([sbyte]::MaxValue('x').ToString().Length)";
        [short] = "x$([short]::MaxValue('x').ToString().Length)";
        [ushort] = "x$([ushort]::MaxValue('x').ToString().Length)";
        [int] = "x$([int]::MaxValue().ToString('x').Length)";
        [uint] = "x$([uint]::MaxValue().ToString('x').Length)";
        [long] = "x$([long]::MaxValue().ToString('x').Length)";
        [ulong] = "x$([ulong]::MaxValue().ToString('x').Length)";
    });
    New-Variable -Name 'IntegerBinaryPadFormats' -Option ReadOnly -Scope 'Script' -Value (@{
        [byte] = "b$([byte]::MaxValue('b').ToString().Length)";
        [sbyte] = "b$([sbyte]::MaxValue('b').ToString().Length)";
        [short] = "b$([short]::MaxValue('b').ToString().Length)";
        [ushort] = "b$([ushort]::MaxValue('b').ToString().Length)";
        [int] = "b$([int]::MaxValue().ToString('b').Length)";
        [uint] = "b$([uint]::MaxValue().ToString('b').Length)";
        [long] = "b$([long]::MaxValue().ToString('b').Length)";
        [ulong] = "b$([ulong]::MaxValue().ToString('b').Length)";
    });
    New-Variable -Name 'MamlSchema' -Option ReadOnly -Scope 'Script' -Value ( New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        msh = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            ns = 'http://msh';
            rootElement = 'helpItems';
        };
        maml = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            ns = 'http://schemas.microsoft.com/maml/2004/10';
        };
        command = 'http://schemas.microsoft.com/maml/dev/command/2004/10';
        dev = 'http://schemas.microsoft.com/maml/dev/2004/10';
    });
    New-Variable -Name 'IndentText' -Option Constant -Scope 'Script' -Value '  ';
}

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
                    if ($NameOnly -or $null -eq $n) {
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
            if ($CanRead -and $null -eq $null -eq $PropertyInfo.GetGetMethod($false)) { $CanRead = $false }
            if ($CanWrite -and $null -eq $PropertyInfo.GetSetMethod($false)) { $CanWrite = $false }
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
        [switch]$NonOptional,
        
        [Parameter(Mandatory = $false)]
        [switch]$IncludeType
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
                if ($IncludeType) {
                    ($Parameters | ForEach-Object { '[{0}]${1}' -f ($_.ParameterType | Get-ClrSimpleName), $_.Name  }) -join ', ';
                } else {
                    ($Parameters | ForEach-Object { '$' + $_.Name  }) -join ', ';
                }
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
        $ArgList = Get-ParameterUsage -Method $ConstructorInfo;
        $FullName = $ConstructorInfo.ReflectedType | Get-ClrSimpleName;
        $SimpleName = $ConstructorInfo.ReflectedType | Get-ClrSimpleName -NameOnly;
        '{0}({1});' -f $SimpleName, ((Get-ParameterUsage -Method $ConstructorInfo -IncludeType) -join ', ');
        Get-ParameterUsage -Method $ConstructorInfo -IsInitVars | Out-IndentedText -IndentText $Script:IndentText;
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
    
    Process {
        if ($IncludeSpecialNamed -or -not $MethodInfo.IsSpecialName) {
            $ArgList = Get-ParameterUsage -Method $MethodInfo;
            '{0}: {1} {0}({2});' -f $MethodInfo.Name, ($MethodInfo.ReturnType | Get-ClrSimpleName), ((Get-ParameterUsage -Method $MethodInfo -IncludeType) -join ', ');
            Get-ParameterUsage -Method $MethodInfo -IsInitVars | Out-IndentedText -IndentText $Script:IndentText;
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
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Type')]
        # Type of CLR object
        [Type]$InputType,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Object')]
        # Object for which to get usage info
        [object[]]$InputObject,
        
        [Parameter(Mandatory = $false)]
        # Return constructors
        [switch]$Constructors,
        
        [Parameter(Mandatory = $false)]
        # Return methods
        [switch]$Methods,
        
        [Parameter(Mandatory = $false)]
        # Return properties
        [switch]$Properties,
        
        [Parameter(Mandatory = $false)]
        # Return fields
        [switch]$Fields,
        
        [Parameter(Mandatory = $false)]
        # Return events
        $Events,
        
        [Parameter(Mandatory = $false)]
        # Return static members
        [switch]$Static,
        
        [Parameter(Mandatory = $false)]
        # Return instance (non-static) members
        [switch]$Instance,
        
        [Parameter(Mandatory = $false)]
        # Include inherited members
        [switch]$Inherited,
        
        [Parameter(Mandatory = $false)]
        # Include base type names
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
            # $FullName = $InputType | Get-ClrSimpleName;
            # $SimpleName = $InputType | Get-ClrSimpleName -NameOnly;
            if ($Options.ShowbaseTypes) {
                if ($null -ne $InputType.BaseType.BaseType) {
                    $Script:IndentText + 'Inherits From:';
                    for ($t = $InputType.BaseType; $null -ne $t.BaseType; $t = $t.BaseType) { '{0}{0}$obj -is [{1}]' -f $Script:IndentText, ($t | Get-ClrSimpleName) };
                }
                $InterfaceArray = $InputType.GetInterfaces();
                if ($InterfaceArray.Length -gt 0) {
                    $Script:IndentText + 'Implements:';
                    $InterfaceArray | Get-ClrSimpleName | Foreach-Object { '{0}{0}$obj -is [{1}]' -f $Script:IndentText, $_ } | Out-IndentedText -IndentText $Script:IndentText;
                }
            }
            if ($Options.Events) {
                $EventArray = @($InputType.GetEvents($BindingFlags) | Sort-Object -Property 'Name');
                if ($EventArray.Count -gt 0) {
                    $Script:IndentText + 'Events:';
                    $EventArray | Get-EventUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Fields) {
                $FieldArray = @($InputType.GetFields($BindingFlags) | Sort-Object -Property 'Name');
                if ($FieldArray.Count -gt 0) {
                    $Script:IndentText + 'Fields:';
                    $FieldArray | Get-FieldUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Properties) {
                $PropertyArray = @($InputType.GetProperties($BindingFlags) | Sort-Object -Property 'Name');
                if ($PropertyArray.Count -gt 0) {
                    $Script:IndentText + 'Properties:';
                    $PropertyArray | Get-PropertyUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Constructors -and -not ($InputType.IsAbstract -or $InputType.IsInterface)) {
                $ConstructorArray = $InputType.GetConstructors();
                if ($ConstructorArray.Count -gt 0) {
                    $Script:IndentText + 'Constructors:';
                    $ConstructorArray | Get-ConstructorUsage | Out-IndentedText -IndentText $Script:IndentText -Level 2;
                }
            }
            if ($Options.Methods) {
                $MethodArray = $InputType.GetMethods($BindingFlags) | Where-Object { -not $_.IsSpecialName } | Group-Object -Property 'Name' | Sort-Object -Property 'Name';
                if ($MethodArray.Count -gt 0) {
                    $Script:IndentText + 'Methods:';
                    foreach ($GroupInfo in $MethodArray) {
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

Function Find-ProjectTypeInfo {
    <#
        .SYNOPSIS
            Searches for matching Visual Studio Project type information.
 
        .DESCRIPTION
			Searches module XML database for matching project type records.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Guid')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'key')]
        [string[]]$Key,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'FileExtension')]
		[AllowEmptyString()]
		[ValidatePattern('^(\..+)$')]
        [string[]]$FileExtension,
        
        [Parameter(ParameterSetName = 'Guid')]
        [System.Guid[]]$Guid
    )

    Begin {
        if ($null -eq $Script:ProjectTypesDocument) {
			$Script:ProjectTypesDocument = [System.Xml.XmlDocument]::new();
			$Script:ProjectTypesDocument.Load(($PSScriptRoot | Join-Path -ChildPath 'VsProjectTypes.xml'));
        }
    }

    Process {
		$ElementArray = @();
		switch ($PSCmdlet.ParameterSetName) {
			'key' {
				$ElementArray = @($Key | ForEach-Object { @($Script:ProjectTypesDocument.DocumentElement.SelectNodes(('ProjectType[translate(@Key,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]|ProjectType/AltDescription[translate(@Key,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]' -f $_.Replace('&', '&amp;').Replace('"', '&quot;').ToLower()))) });
				break;
			}
			'FileExtension' {
				$ElementArray = @($FileExtension | ForEach-Object { @($Script:ProjectTypesDocument.DocumentElement.SelectNodes(('ProjectType[translate(@Extension,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]|ProjectType/AltExt[translate(.,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]' -f $_.Replace('&', '&amp;').Replace('"', '&quot;').ToLower()))) });
				break;
			}
			default {
				if ($PSBoundParameters.ContainsKey('')) {
					$ElementArray = @($Guid | ForEach-Object { @($Script:ProjectTypesDocument.DocumentElement.SelectNodes(('ProjectType[@Guid="{0}"]|ProjectType/AltGuid[.="{0}"]' -f $_.ToString()))) });
				}
				break;
			}
		}

		$ElementArray | ForEach-Object {
			$e = $_;
			$Properties = @{ };
			switch ($e.LocalName) {
				'AltDescription' {
					$e = $_.ParentNode;
					$Properties['Guid']  = $e.SelectSingleNode('@Guid').Value;
					$Properties['Key'] = $_.SelectSingleNode('@Key').Value;
					$a = $_.SelectSingleNode('@Extension');
					if ($null -eq $a) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
					$Properties['Description'] = $_.InnerText;
					$Properties['AltExtensions'] = @($_.SelectNodes('AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @(@{
						Key = $e.SelectSingleNode('@Key').Value;
						Description = $e.SelectSingleNode('@Description').Value;
					}) + @($_.SelectNodes('preceding-sibling::AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}}) + @($_.SelectNodes('followng-sibling::AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($_.SelectNodes('AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
				'AltExt' {
					$e = $_.ParentNode;
					$Properties['Guid']  = $e.SelectSingleNode('@Guid').Value;
					$Properties['Key']  = $e.SelectSingleNode('@Key').Value;
					$Properties['Extension'] = $_.InnerText;
					$Properties['Description'] = $e.SelectSingleNode('@Description').Value;
					$Properties['AltExtensions'] = @($e.SelectSingleNode('@Extension').Value) + @($_.SelectNodes('preceding-sibling::AltExt') | ForEach-Object { $_.InnerText }) + @($_.SelectNodes('followng-sibling::AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @($_.SelectNodes('AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($_.SelectNodes('AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
				'AltGuid' {
					$e = $_.ParentNode;
					$Properties['Guid'] = $_.InnerText;
					$Properties['Key']  = $e.SelectSingleNode('@Key').Value;
					$a = $_.SelectSingleNode('@Extension');
					if ($null -eq $a) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
					$Properties['Description'] = $e.SelectSingleNode('@Description').Value;
					$Properties['AltExtensions'] = @($_.SelectNodes('AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @($_.SelectNodes('AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($e.SelectSingleNode('@Guid').Value) + @($_.SelectNodes('preceding-sibling::AltGuid') | ForEach-Object { $_.InnerText }) + @($_.SelectNodes('followng-sibling::AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
				default {
					$e = $_.ParentNode;
					$Properties['Guid']  = $e.SelectSingleNode('@Guid').Value;
					$Properties['Key'] = $_.SelectSingleNode('@Key').Value;
					$a = $_.SelectSingleNode('@Extension');
					if ($null -eq $a) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
					$Properties['Description'] = $e.SelectSingleNode('@Description').Value;
					$Properties['AltExtensions'] = @($_.SelectNodes('AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @($_.SelectNodes('AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($_.SelectNodes('AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
			}
			$a = $_.SelectSingleNode('@Language');
			if ($null -eq $a) { $Properties['Language'] = '' } else { $Properties['Language'] = $a.Value }
			$a = $_.SelectSingleNode('@Package');
			if ($null -eq $a) { $Properties['Package'] = $null } else { $Properties['Package'] = [Guid]::Parse($a.Value) }
			$Properties['Guid'] = [Guid]::Parse($Properties['Guid']);
			$Properties['AltGuids'] = @($Properties['AltGuids'] | ForEach-Object { [Guid]::Parse($_) });
			New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
		}
    }
}

Function New-PSMaml {
    <#
        .SYNOPSIS
            Get PowerShell help MAML.
 
        .DESCRIPTION
            Creates a new Xml document for PowerShell MAML help.

        .EXAMPLE
            $MamDocument = New-PSMaml;
            $CommandElement = Add-CommandMaml -Maml $MamDocument -Verb 'Invoke' -Noun 'MyCommand';

        .LINK
            Add-CommandMaml

    #>
    [CmdletBinding()]
    [OutputType([xml])]
    Param()

    $PSMaml = New-Object -TypeName 'System.Xml.XmlDocument';
    $PSMaml.AppendChild($PSMaml.CreateElement($Script:MamlSchema.msh.rootElement, $Script:MamlSchema.msh.ns)) | Out-Null;
    $PSMaml | Write-Output;
}

Function Import-PSMaml {
    <#
        .SYNOPSIS
            Imports PowerShell help MAML.
 
        .DESCRIPTION
            Loads PowerShell help MAM into a new Xml document.

        .EXAMPLE
            $MamDocument = (Get-Content -Path 'MyModule.dll-Help.xml') | Import-PSMaml;

        .LINK
            New-CommandMaml
    #>
    [CmdletBinding()]
    [OutputType([xml])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Content')]
        [AllowEmptyString()]
        [string[]]$Content,

        [Parameter(Mandatory = $true, ParameterSetName = 'Stream')]
        [System.IO.Stream]$Stream,

        [Parameter(Mandatory = $true, ParameterSetName = 'TextReader')]
        [System.IO.TextReader]$TextReader,

        [Parameter(Mandatory = $true, ParameterSetName = 'XmlReader')]
        [System.Xml.XmlReader]$XmlReader
    )

    Begin {
        $PSMaml = New-Object -TypeName 'System.Xml.XmlDocument';
        $AllContent = @();
    }

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Stream' {
                $PSMaml.Load($Stream);
                break;
            }
            'TextReader' {
                $PSMaml.Load($TextReader);
                break;
            }
            'XmlReader' {
                $PSMaml.Load($XmlReader);
                break;
            }
            default {
                $AllContent = $AllContent + @($Content);
                break;
            }
        }
    }
    End {
        if ($PSCmdlet.ParameterSetName -eq 'Content') {
            $PSMaml.LoadXml(($AllContent | Out-String).Trim());
        }
        $PSMaml | Write-Output;
    }
}

Function Test-PSMaml {
    <#
        .SYNOPSIS
            Checks whether XML represents PowerShell help MAML.
 
        .DESCRIPTION
            Checks whether the XML document element name is 'helpItems' and the namespace of that element is 'http://msh'.
    #>
    [CmdletBinding()]
    [OutputType([bool], [string])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [Alias('maml')]
        [xml]$PSMaml
    )

    Begin {
        $Success = $true;
    }
    Process {
        if ($Success) {
            foreach ($x in $PSMaml) {
                if ($null -eq $x -or $null -eq $PSMaml.DocumentElement -or $PSMaml.DocumentElement.LocalName -ne $Script:MamlSchema.msh.rootElement -or $PSMaml.DocumentElement.NamespaceURI -ne $Script:MamlSchema.msh.ns) {
                    $Success = $false;
                    break;
                }
            }
        }
    }
    End {
        $Success | Write-Output;
    }
}

Function Test-CommandVerb {
    <#
        .SYNOPSIS
            Test command verb text.
 
        .DESCRIPTION
            Checks validity of command verb text or gets proper letter-case commnd verb string.

        .EXAMPLE
            $VerbText = Read-Host -Prompt 'Enter verb';
            $IsVerbValid = $VerbText | Test-CommandVerb;

        .EXAMPLE
            $VerbText = Read-Host -Prompt 'Enter verb';
            $VerbText = $VerbText | Test-CommandVerb -GetMatching;
            if ($null -eq $VerbText) {
                Write-Warning -Message "'$VerbText' is not a valid verb.";
            }
    #>
    [CmdletBinding()]
    [OutputType([bool], [string])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [string[]]$Verb,

        [switch]$GetMatching
    )

    Begin {
        if ($null -eq $Script:__Test_CommandVerb) {
            $Script:__Test_CommandVerb = @(Get-Verb | ForEach-Object { $_.Verb })
        }
        $Success = $true;
    }
    Process {
        if ($GetMatching) {
            foreach ($v in $Verb) {
                if ($null -ne $v) {
                    $s = $v.Trim();
                    $Script:__Test_CommandVerb | Where-Object { $_ -ieq $s }
                }
            }
        } else {
            if ($Success) {
                foreach ($v in $Verb) {
                    if ($null -eq $v -or $Script:__Test_CommandVerb -inotcontains $v) {
                        $Success = $false;
                        break;
                    }
                }
            }
        }
    }
    End {
        if (-not $GetMatching) { $Success | Write-Output }
    }
}

Function Get-CommandMaml {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlElement])]
    Param(
        [Parameter(Mandatory = $true)]
        [Alias('maml')]
        [ValidateScript({ $_ | Test-PSMaml })]
        [xml]$PSMaml,
        
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'InputNames')]
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'InputVerbs')]
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'InputNouns')]
        [string[]]$InputString,
        
        [Parameter(ParameterSetName = 'VerbNoun')]
        [string[]]$Verb,
        
        [Parameter(ParameterSetName = 'VerbNoun')]
        [string]$Noun,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Name')]
        [string[]]$Name,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'InputName')]
        [switch]$Names,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'InputVerbs')]
        [switch]$Verbs,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'InputNouns')]
        [switch]$Nouns
    )



    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'InputNames' {
                
            }
        }
    }
}
Function Add-CommandMaml {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlElement])]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({})]
        [string]$Verb
    )

}

Function ConvertTo-PsStringLiteral {
    [CmdletBinding(DefaultParameterSetName = 'PreferSingle')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The string to be converted to a PowerShell string literal.
        [object]$InputString,

        [Parameter(ParameterSetName = 'PreferSingle')]
        # Result literal is surrounded by single quotes unless a double-quoted literal is shorter or a character is a control character or non-ASCII.
        [switch]$PreferSingleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'PreferDouble')]
        # Result literal is surrounded by double quotes unless a single-quoted literal is shorter in length.
        [switch]$PreferDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'AlwaysDouble')]
        # Result literal is always surrounded by double quotes.
        [switch]$AlwaysDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'OmitQuotes')]
        # Do not include leading and trailing quotes. Characters are escapes as though they were part of a double-quoted string.
        [switch]$OmitQuotes
    )

    Process {
        if ($InputString -eq '') {
            if ($OmitQuotes.IsPresent) {
                $InputString | Write-Output;
            } else {
                if ($StringQuotes -eq 'PreferSingle') {
                    "''" | Write-Output;
                } else {
                    '""' | Write-Output;
                }
            }
        } else {
            $UseDoubleQuote = $true;
            switch ($StringQuotes) {
                'PreferSingle' {
                    if (-not $Script:SingleQuoteIncompatibleRegex.IsMatch($InputString)) {
                        if ($MatchCollection.Count -eq 0) {
                            $UseDoubleQuote = $false;
                        } else {
                            $d = $s = 0;
                            $MatchCollection | ForEach-Object {
                                switch ($_.Value) {
                                    "‘" { $s++; break; }
                                    "’" { $s++; break; }
                                    "'" { $s++; break; }
                                    '$' { $d++; break; }
                                    '"' { $d++; break; }
                                    '`' { $d++; break; }
                                    '“' { $d++; break; }
                                    '”' { $d++; break; }
                                }
                            }
                            if ($d -ge $s) { $UseDoubleQuote = $false }
                        }
                    }
                    break;
                }
                'PreferDouble' {
                    if (-not $Script:SingleQuoteIncompatibleRegex.IsMatch($InputString)) {
                        $MatchCollection = $Script:AnyLiteralToEscapeRegex.Matches($InputString);
                        if ($MatchCollection.Count -gt 0) {
                            $d = $s = 0;
                            $MatchCollection | ForEach-Object {
                                switch ($_.Value) {
                                    "‘" { $s++; break; }
                                    "’" { $s++; break; }
                                    "'" { $s++; break; }
                                    '$' { $d++; break; }
                                    '"' { $d++; break; }
                                    '`' { $d++; break; }
                                    '“' { $d++; break; }
                                    '”' { $d++; break; }
                                }
                            }
                            if ($d -gt $s) { $UseDoubleQuote = $false }
                        }
                    }
                    break;
                }
            }
            if ($UseDoubleQuote) {
                $m = $Script:DoubleQuotedLiteralToEscapeRegex.Match($InputString);
                if ($m.Success) {
                    $StringBuilder = [System.Text.StringBuilder]::new();
                    if (-not $OmitQuotes.IsPresent) { $StringBuilder.Append('"') | Out-Null }
                    switch ($m.Value) {
                        "`0" {
                            $StringBuilder.Append('`0') | Out-Null;
                            break;
                        }
                        "`a" {
                            $StringBuilder.Append('`a') | Out-Null;
                            break;
                        }
                        "`b" {
                            $StringBuilder.Append('`b') | Out-Null;
                            break;
                        }
                        "`t" {
                            $StringBuilder.Append('`t') | Out-Null;
                            break;
                        }
                        "`n" {
                            $StringBuilder.Append('`n') | Out-Null;
                            break;
                        }
                        "`v" {
                            $StringBuilder.Append('`v') | Out-Null;
                            break;
                        }
                        "`f" {
                            $StringBuilder.Append('`f') | Out-Null;
                            break;
                        }
                        "`r" {
                            $StringBuilder.Append('`r') | Out-Null;
                            break;
                        }
                        "`e" {
                            $StringBuilder.Append('`e') | Out-Null;
                            break;
                        }
                        '“' {
                            $StringBuilder.Append('““') | Out-Null;
                            break;
                        }
                        '”' {
                            $StringBuilder.Append('””') | Out-Null;
                            break;
                        }
                        default {
                            $c = $m.Value[0]
                            if ($c -lt $Script:EscapeChar -or $c -ge $Script:DeleteChar) {
                                $StringBuilder.Append('`u{').Append(([int]$c).ToString('x')).Append('}') | Out-Null;
                            } else {
                                $StringBuilder.Append('`').Append($c) | Out-Null;
                            }
                            break;
                        }
                    }
                    $StartAt = $m.Length;
                    while ($StartAt -lt $InputString.Length) {
                        $m = $Script:DoubleQuotedLiteralToEscapeRegex.Match($InputString, $StartAt);
                        if ($m.Success) {
                            $Len = $m.Index - $StartAt;
                            if ($Len -gt 0) {
                                $StringBuilder.Append($InputString.Substring($StartAt, $Len)) | Out-Null;
                            }
                            switch ($m.Value) {
                                "`0" {
                                    $StringBuilder.Append('`0') | Out-Null;
                                    break;
                                }
                                "`a" {
                                    $StringBuilder.Append('`a') | Out-Null;
                                    break;
                                }
                                "`b" {
                                    $StringBuilder.Append('`b') | Out-Null;
                                    break;
                                }
                                "`t" {
                                    $StringBuilder.Append('`t') | Out-Null;
                                    break;
                                }
                                "`n" {
                                    $StringBuilder.Append('`n') | Out-Null;
                                    break;
                                }
                                "`v" {
                                    $StringBuilder.Append('`v') | Out-Null;
                                    break;
                                }
                                "`f" {
                                    $StringBuilder.Append('`f') | Out-Null;
                                    break;
                                }
                                "`r" {
                                    $StringBuilder.Append('`r') | Out-Null;
                                    break;
                                }
                                "`e" {
                                    $StringBuilder.Append('`e') | Out-Null;
                                    break;
                                }
                                '“' {
                                    $StringBuilder.Append('““') | Out-Null;
                                    break;
                                }
                                '”' {
                                    $StringBuilder.Append('””') | Out-Null;
                                    break;
                                }
                                default {
                                    $c = $m.Value[0]
                                    if ($c -lt $Script:EscapeChar -or $c -ge $Script:DeleteChar) {
                                        $StringBuilder.Append('`u{').Append(([int]$c).ToString('x')).Append('}') | Out-Null;
                                    } else {
                                        $StringBuilder.Append('`').Append($c) | Out-Null;
                                    }
                                    break;
                                }
                            }
                            $StartAt = $m.Index + $m.Length;
                        } else {
                            $StringBuilder.Append($InputString.Substring($StartAt)) | Out-Null;
                            break;
                        }
                    }
                    if ($OmitQuotes.IsPresent) {
                        $StringBuilder.ToString() | Write-Output;
                    } else {
                        $StringBuilder.Append('"').ToString() | Write-Output;
                    }
                } else {
                    if ($OmitQuotes.IsPresent) {
                        $InputString | Write-Output;
                    } else {
                        "`"$InputString`"" | Write-Output;
                    }
                }
            } else {
                $m = $Script:SingleQuotedLiteralToEscapeRegex.Match($InputString);
                if ($m.Success) {
                    $StringBuilder = [System.Text.StringBuilder]::new("'").Append($m.Value).Append($m.Value);
                    $StartAt = $m.Length;
                    while ($StartAt -lt $InputString.Length) {
                        $m = $Script:SingleQuotedLiteralToEscapeRegex.Match($InputString, $StartAt);
                        if ($m.Success) {
                            $Len = $m.Index - $StartAt;
                            if ($Len -gt 0) {
                                $StringBuilder.Append($InputString.Substring($StartAt, $Len)) | Out-Null;
                            }
                            $StringBuilder.Append($m.Value).Append($m.Value) | Out-Null;
                            $StartAt = $m.Index + $m.Length;
                        } else {
                            $StringBuilder.Append($InputString.Substring($StartAt)) | Out-Null;
                            break;
                        }
                    }
                    $StringBuilder.Append("'").ToString() | Write-Output;
                } else {
                    "'$InputString'" | Write-Output;
                }
            }
        }
    }

}

Function ConvertTo-PsCharLiteral {
    [CmdletBinding(DefaultParameterSetName = 'PreferSingle')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        # Character value to convert to a PowerShell code literal.
        [char]$InputValue,

        [Parameter(ParameterSetName = 'PreferSingle')]
        # Result literal is surrounded by single quotes unless a double-quoted literal is shorter or a character is a control character or non-ASCII.
        [switch]$PreferSingleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'PreferDouble')]
        # Result literal is surrounded by double quotes unless a single-quoted literal is shorter in length.
        [switch]$PreferDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'AlwaysDouble')]
        # Result literal is always surrounded by double quotes.
        [switch]$AlwaysDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'OmitQuotes')]
        # Do not include leading and trailing quotes. Characters are escapes as though they were part of a double-quoted string.
        [switch]$OmitQuotes
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'OmitQuotes' {
                switch ($InputValue) {
                    "`0" { '`0' | Write-Output; break; }
                    "`a" { '`a' | Write-Output; break; }
                    "`b" { '`b' | Write-Output; break; }
                    "`t" { '`t' | Write-Output; break; }
                    "`n" { '`n' | Write-Output; break; }
                    "`v" { '`v' | Write-Output; break; }
                    "`f" { '`f' | Write-Output; break; }
                    "`r" { '`r' | Write-Output; break; }
                    "`e" { '`e' | Write-Output; break; }
                    '$' { '`$' | Write-Output; break; }
                    '"' { '`"' | Write-Output; break; }
                    '`' { '``' | Write-Output; break; }
                    "‘" { "‘" | Write-Output; break; }
                    "’" { "’" | Write-Output; break; }
                    '“' { '`“' | Write-Output; break; }
                    '”' { '`”' | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "``u{$(([int]$InputValue).ToString('x'))}" | Write-Output;
                        } else {
                            $InputValue.ToString() | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
            'PreferDouble' {
                switch ($InputValue) {
                    "`0" { '"`0"' | Write-Output; break; }
                    "`a" { '"`a"' | Write-Output; break; }
                    "`b" { '"`b"' | Write-Output; break; }
                    "`t" { '"`t"' | Write-Output; break; }
                    "`n" { '"`n"' | Write-Output; break; }
                    "`v" { '"`v"' | Write-Output; break; }
                    "`f" { '"`f"' | Write-Output; break; }
                    "`r" { '"`r"' | Write-Output; break; }
                    "`e" { '"`e"' | Write-Output; break; }
                    "'" { "`"'`"" | Write-Output; break; }
                    '$' { "'`$'" | Write-Output; break; }
                    '"' { "'`"'" | Write-Output; break; }
                    '`' { "'``'" | Write-Output; break; }
                    "‘" { "`"‘`"" | Write-Output; break; }
                    "’" { "`"’`"" | Write-Output; break; }
                    '“' { "'““'" | Write-Output; break; }
                    '”' { "'””'" | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "`"``u{$(([int]$InputValue).ToString('x'))}`"" | Write-Output;
                        } else {
                            "`"$InputValue`"" | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
            'AlwaysDouble' {
                switch ($InputValue) {
                    "`0" { '"`0"' | Write-Output; break; }
                    "`a" { '"`a"' | Write-Output; break; }
                    "`b" { '"`b"' | Write-Output; break; }
                    "`t" { '"`t"' | Write-Output; break; }
                    "`n" { '"`n"' | Write-Output; break; }
                    "`v" { '"`v"' | Write-Output; break; }
                    "`f" { '"`f"' | Write-Output; break; }
                    "`r" { '"`r"' | Write-Output; break; }
                    "`e" { '"`e"' | Write-Output; break; }
                    "'" { "`"'`"" | Write-Output; break; }
                    '$' { "`"```$`"" | Write-Output; break; }
                    '"' { "`"```"`"" | Write-Output; break; }
                    '`' { "`"`````"" | Write-Output; break; }
                    "‘" { "`"‘`"" | Write-Output; break; }
                    "’" { "`"’`"" | Write-Output; break; }
                    '“' { "`"``““`"" | Write-Output; break; }
                    '”' { "`"``””`"" | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "`"``u{$(([int]$InputValue).ToString('x'))}`"" | Write-Output;
                        } else {
                            "`"$InputValue`"" | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
            default {
                switch ($InputValue) {
                    "`0" { '"`0"' | Write-Output; break; }
                    "`a" { '"`a"' | Write-Output; break; }
                    "`b" { '"`b"' | Write-Output; break; }
                    "`t" { '"`t"' | Write-Output; break; }
                    "`n" { '"`n"' | Write-Output; break; }
                    "`v" { '"`v"' | Write-Output; break; }
                    "`f" { '"`f"' | Write-Output; break; }
                    "`r" { '"`r"' | Write-Output; break; }
                    "`e" { '"`e"' | Write-Output; break; }
                    "'" { "`"'`"" | Write-Output; break; }
                    '$' { "'`$'" | Write-Output; break; }
                    '"' { "'`"'" | Write-Output; break; }
                    '`' { "'``'" | Write-Output; break; }
                    "‘" { "`"‘`"" | Write-Output; break; }
                    "’" { "`"’`"" | Write-Output; break; }
                    '“' { "'““'" | Write-Output; break; }
                    '”' { "'””'" | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "`"``u{$(([int]$InputValue).ToString('x'))}`"" | Write-Output;
                        } else {
                            "'$InputValue'" | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}

Function ConvertTo-PsIntegerLiteral {
    [CmdletBinding(DefaultParameterSetName = 'Base10')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [ValidateScript({ $_ -is [byte] -or $_ -is [sbyte] -or $_ -is [ushort] -or $_ -is [short] -or $_ -is [int] -or $_ -is [uint] -or $_ -is [long] -or $_ -is [ulong] -or $_ -is [bigint] })]
        [object]$InputValue,

        [ValidateSet('PreferSingle', 'PreferDouble', 'AlwaysDouble')]
        [string]$StringQuotes = 'PreferSingle',

        [switch]$OmitNumericSuffix,

        [Parameter(ParameterSetName = 'Base10')]
        [switch]$Base10,

        [Parameter(Mandatory = $true, ParameterSetName = 'Hexidecimal')]
        [switch]$Hexidecimal,

        [Parameter(Mandatory = $true, ParameterSetName = 'Binary')]
        [switch]$Binary,

        # Pad with zeroes according to bit length. This is ignored for [bigint] values.
        [switch]$ZeroPadded,

        [Parameter(ParameterSetName = 'Hexidecimal', HelpMessage = 'Omits the leading "0x"')]
        [Parameter(ParameterSetName = 'Binary', HelpMessage = 'Omits the leading "0b"')]
        [switch]$OmitPrefix,

        [ValidateSet('short', 'int', 'long')]
        [string]$MinBits
    )
    
    Process {
        $Value = $null;
        if ($PSBoundParameters.ContainsKey('MinBits')) {
            switch ($MinBits) {
                'short' {
                    switch ($InputValue) {
                        { $_ -is [byte] } { [uint]$Value = $InputValue; break; }
                        { $_ -is [sbyte] } { [int]$Value = $InputValue; break; }
                        { $_ -is [short] -or $_ -is [ushort] } { $Value = $InputValue; break; }
                        { $_ -is [uint] -or $_ -is [ulong] } {
                            if ($InputValue -le [short]::MaxValue) {
                                [short]$Value = $InputValue;
                            } else {
                                if ($InputValue -le [ushort]::MaxValue) { [ushort]$Value = $InputValue } else { $Value = $InputValue }
                            }
                            break;
                        }
                        default {
                            if ($InputValue -lt 0) {
                                if ($InputValue -ge [short]::MinValue) { [short]$Value = $InputValue } else { $Value = $InputValue }
                            } else {
                                if ($InputValue -lt [short]::MaxValue) {
                                    [int]$Value = $InputValue
                                } else {
                                    if ($InputValue -le [ushort]::MaxValue) { [ushort]$Value = $InputValue } else { $Value = $InputValue }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
                'long' {
                    switch ($InputValue) {
                        { $_ -is [byte] -or $_ -is [ushort] -or $_ -is [uint] } { [ulong]$Value = $InputValue; break; }
                        { $_ -is [sbyte] -or $_ -is [short] -or $_ -is [int] } { [long]$Value = $InputValue; break; }
                        { $_ -is [bigint] } {
                            if ($InputValue -lt 0) {
                                if ($InputValue -ge [long]::MinValue) { [long]$Value = $InputValue } else { $Value = $InputValue }
                            } else {
                                if ($InputValue -lt [long]::MaxValue) {
                                    [long]$Value = $InputValue
                                } else {
                                    if ($InputValue -le [ulong]::MaxValue) { [ulong]$Value = $InputValue } else { $Value = $InputValue }
                                }
                            }
                            break;
                        }
                        default { $Value = $InputValue; break; }
                    }
                    break;
                }
                default {
                    switch ($InputValue) {
                        { $_ -is [byte] -or $_ -is [ushort] } { [uint]$Value = $InputValue; break; }
                        { $_ -is [sbyte] -or $_ -is [short] } { [int]$Value = $InputValue; break; }
                        { $_ -is [int] -or $_ -is [uint] } { $Value = $InputValue; break; }
                        { $_ -is [ulong] } {
                            if ($InputValue -le [int]::MaxValue) {
                                [int]$Value = $InputValue;
                            } else {
                                if ($InputValue -le [uint]::MaxValue) { [uint]$Value = $InputValue } else { $Value = $InputValue }
                            }
                            break;
                        }
                        default {
                            if ($InputValue -lt 0) {
                                if ($InputValue -ge [int]::MinValue) { [int]$Value = $InputValue } else { $Value = $InputValue }
                            } else {
                                if ($InputValue -lt [int]::MaxValue) {
                                    [int]$Value = $InputValue
                                } else {
                                    if ($InputValue -le [uint]::MaxValue) { [uint]$Value = $InputValue } else { $Value = $InputValue }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        } else {
            $Value = $InputValue;
        }
        switch ($PSCmdlet.ParameterSetName) {
            'Hexidecimal' {
                if ($OmitNumericSuffix.IsPresent) {
                    if ($ZeroPadded.IsPresent) {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString($Script:IntegerHexPadFormats[$Value.GetType()]) | Write-Output;
                        } else {
                            "0x$($Value.ToString($Script:IntegerHexPadFormats[$Value.GetType()]))" | Write-Output;
                        }
                    } else {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString('x') | Write-Output;
                        } else {
                            "0x$($Value.ToString('x'))" | Write-Output;
                        }
                    }
                } else {
                    if ($ZeroPadded.IsPresent) {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString($Script:IntegerHexPadFormats[[byte]]))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString($Script:IntegerHexPadFormats[[sbyte]]))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString($Script:IntegerHexPadFormats[[short]]))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString($Script:IntegerHexPadFormats[[ushort]]))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString($Script:IntegerHexPadFormats[[uint]]))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString($Script:IntegerHexPadFormats[[long]]))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString($Script:IntegerHexPadFormats[[ulong]]))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('x'))N" | Write-Output; break; }
                            default { $Value.ToString($Script:IntegerHexPadFormats[[int]]) | Write-Output; break; }
                        }
                    } else {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString('x'))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString('x'))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString('x'))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString('x'))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString('x'))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString('x'))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString('x'))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('x'))N" | Write-Output; break; }
                            default { $Value.ToString('x') | Write-Output; break; }
                        }
                    }
                }
                break;
            }
            'Binary' {
                if ($OmitNumericSuffix.IsPresent) {
                    if ($ZeroPadded.IsPresent) {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString($Script:IntegerBinaryPadFormats[$Value.GetType()]) | Write-Output;
                        } else {
                            "0x$($Value.ToString($Script:IntegerBinaryPadFormats[$Value.GetType()]))" | Write-Output;
                        }
                    } else {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString('x') | Write-Output;
                        } else {
                            "0x$($Value.ToString('x'))" | Write-Output;
                        }
                    }
                } else {
                    if ($ZeroPadded.IsPresent) {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[byte]]))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[sbyte]]))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[short]]))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[ushort]]))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[uint]]))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[long]]))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[ulong]]))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('b'))N" | Write-Output; break; }
                            default { $Value.ToString($Script:IntegerBinaryPadFormats[[int]]) | Write-Output; break; }
                        }
                    } else {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString('b'))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString('b'))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString('b'))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString('b'))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString('b'))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString('b'))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString('b'))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('b'))N" | Write-Output; break; }
                            default { $Value.ToString('b') | Write-Output; break; }
                        }
                    }
                }
                break;
            }
            default {
                if ($OmitNumericSuffix.IsPresent) {
                    if ($ZeroPadded.IsPresent) {
                        $Value.ToString($Script:IntegerBase10PadFormats[$Value.GetType()]) | Write-Output;
                    } else {
                        $Value.ToString() | Write-Output;
                    }
                } else {
                    if ($ZeroPadded.IsPresent) {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[byte]]))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[sbyte]]))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[short]]))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[ushort]]))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[uint]]))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[long]]))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[ulong]]))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value)N" | Write-Output; break; }
                            default { $Value.ToString($Script:IntegerBase10PadFormats[[int]]) | Write-Output; break; }
                        }
                    } else {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value)UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value)Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value)S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value)US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value)U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value)L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value)UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value)N" | Write-Output; break; }
                            default { $Value.ToString() | Write-Output; break; }
                        }
                    }
                }
                break;
            }
        }
    }
}

Function ConvertTo-PsScriptLiteral {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [ValidateScript({ $_ -is [string] -or $_ -is [char] -or $_ -is [byte] -or $_ -is [sbyte] -or $_ -is [short]  -or $_ -is [ushort] -or $_ -is [int] -or $_ -is [uint] -or $_ -is [long] -or `
            $_ -is [ulong] -or $_ -is [bigint] -or $_ -is [float] -or  $_ -is [double] -or $_ -is [decimal] -or $_ -is [bool] -or $_ -is [DateTime] -or $_ -is [DateOnly] -or $_ -is [TimeOnly] -or `
            $_ -is [Guid] -or $_ -is [TimeSpan] -or $_ -is [Uri] -or $_ -is [System.Management.Automation.SemanticVersion] -or $_ -is [Version] -or $_ -is [ScriptBlock] -or $_ -is [enum] })]
        [object]$InputObject,

        [ValidateSet('PreferSingle', 'PreferDouble', 'AlwaysDouble')]
        [string]$StringQuotes = 'PreferSingle',

        [ValidateSet('Base10', 'Hexidecimal', 'Binary')]
        [string]$IntegerFormat = 'Base10',

        [ValidateSet('short', 'int', 'long')]
        [string]$MinIntegerBits,

        [switch]$OmitNumericSuffix,

        [switch]$ZeroPaddedIntegers,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$MinDecimalPlaces = 1,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$MaxDecimalPlaces,

        [ValidateSet('Microseconds', 'Milliseconds', 'Seconds', 'Minutes', 'Hours', 'Days')]
        [string]$TruncateDateTime,

        [ValidateSet('Local', 'Utc')]
        [string]$DateTimeKind,

        [ValidateSet('Local', 'Utc')]
        [string]$DateTimeUnspecifiedAs
    )

    Process {
        if ($null -eq $InputObject) {
            '$null' | Write-Output;
        } else {
            switch ($InputObject) {
                $null { '$null' | Write-Output; break; }
                { $_ -is [char] } {
                    switch ($StringQuotes) {
                        'PreferDouble' {
                            ($InputObject | ConvertTo-PsCharLiteral -PreferDoubleQuotes) | Write-Output;
                            break;
                        }
                        'AlwaysDouble' {
                            ($InputObject | ConvertTo-PsCharLiteral -AlwaysDoubleQuotes) | Write-Output;
                            break;
                        }
                        default {
                            ($InputObject | ConvertTo-PsCharLiteral -PreferSingleQuotes) | Write-Output;
                            break;
                        }
                    }
                    break;
                }
                { $_ -is [byte] -or $_ -is [sbyte] -or $_ -is [short]  -or $_ -is [ushort] -or $_ -is [int] -or $_ -is [uint] -or $_ -is [long] -or $_ -is [ulong] -or $_ -is [bigint] } {
                    switch ($IntegerFormat) {
                        'Binary' {
                            if ($OmitNumericSuffix.IsPresent) {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary -OmitNumericSuffix) | Write-Output;
                                    }
                                }
                            } else {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary -ZeroPadded) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary -ZeroPadded) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary) | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                        'Hexidecimal' {
                            if ($OmitNumericSuffix.IsPresent) {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal -OmitNumericSuffix) | Write-Output;
                                    }
                                }
                            } else {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal -ZeroPadded) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal -ZeroPadded) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal) | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                        default {
                            if ($OmitNumericSuffix.IsPresent) {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10 -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10 -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10 -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10 -OmitNumericSuffix) | Write-Output;
                                    }
                                }
                            } else {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10 -ZeroPadded) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10 -ZeroPadded) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10) | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
                { $_ -is [float] -or  $_ -is [double] } {
                    $Value = $InputObject;
                    if ($PSBoundParameters.ContainsKey('MaxDecimalPlaces')) { $Value = [Math]::Round($InputObject, $MaxDecimalPlaces) }
                    $Code = $Value.ToString();
                    if ($MinDecimalPlaces -eq 0 -or $Code.Contains('E')) {
                        $Code | Write-Output;
                    } else {
                        $i = $Code.IndexOf('.');
                        if ($i -lt 0) {
                            "$Code.$([string]::new(([char]'0'), $MinDecimalPlaces))" | Write-Output;
                        } else {
                            $i = $MinDecimalPlaces - ($Code.Length - $i - 1);
                            if ($i -gt 0) {
                                "$Code$([string]::new(([char]'0'), $i))" | Write-Output;
                            } else {
                                $Code | Write-Output;
                            }
                        }
                    }
                    break;
                }
                { $_ -is [decimal] } {
                    $Value = $InputObject;
                    if ($PSBoundParameters.ContainsKey('MaxDecimalPlaces')) { $Value = [Math]::Round($InputObject, $MaxDecimalPlaces) }
                    $Code = $Value.ToString();
                    if ($MinDecimalPlaces -eq 0 -or $Code.Contains('E')) {
                        if ($OmitNumericSuffix.IsPresent) {
                            $Code | Write-Output;
                        } else {
                            "$($Code)D" | Write-Output;
                        }
                    } else {
                        $i = $Code.IndexOf('.');
                        if ($i -lt 0) {
                            if ($OmitNumericSuffix.IsPresent) {
                                "$Code.$([string]::new(([char]'0'), $MinDecimalPlaces))" | Write-Output
                            } else {
                                "$Code.$([string]::new(([char]'0'), $MinDecimalPlaces))D" | Write-Output
                            };
                        } else {
                            $i = $MinDecimalPlaces - ($Code.Length - $i - 1);
                            if ($i -gt 0) {
                                if ($OmitNumericSuffix.IsPresent) {
                                    "$Code$([string]::new(([char]'0'), $i))" | Write-Output;
                                } else {
                                    "$Code$([string]::new(([char]'0'), $i))D" | Write-Output;
                                }
                            } else {
                                if ($OmitNumericSuffix.IsPresent) {
                                    $Code | Write-Output;
                                } else {
                                    "$($Code)D" | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                { $_ -is [bool] } {
                    if ($InputObject) { '$true' | Write-Output } else { '$false' | Write-Output }
                    break;
                }
                { $_ -is [DateTime] } {
                    [DateTime]$DateTime = $InputObject;
                    if ($PSBoundParameters.ContainsKey('DateTimeUnspecifiedAs') -and $DateTime.Kind -eq [System.DateTimeKind]::Unspecified) {
                        if ($DateTimeUnspecifiedAs -eq 'Utc') {
                            $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Utc);
                        } else {
                            $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Local);
                        }
                        if ($PSBoundParameters.Contains('DateTimeKind')) {
                            if ($DateTimeKind -eq 'Local') {
                                if ($DateTime.Kind -eq [DateTimeKind]::Utc) { $DateTime = $DateTime.ToLocalTime(); break; }
                            } else {
                                if ($DateTime.Kind -eq [DateTimeKind]::Local) { $DateTime = $DateTime.ToUniversalTime(); break; }
                            }
                        }
                    } else {
                        if ($PSBoundParameters.Contains('DateTimeKind')) {
                            if ($DateTimeKind -eq 'Local') {
                                switch ($DateTime.Kind) {
                                    Utc { $DateTime = $DateTime.ToLocalTime(); break; }
                                    Unspecified { $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Local); break; }
                                }
                            } else {
                                switch ($DateTime.Kind) {
                                    Local { $DateTime = $DateTime.ToUniversalTime(); break; }
                                    Unspecified { $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Utc); break; }
                                }
                            }
                        }
                    }
                    if ($PSBoundParameters.ContainsKey('TruncateDateTime')) {
                        switch ($TruncateDateTime) {
                            'Microseconds' {
                                if ($DateTime.Nanosecond -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, $DateTime.Second, $DateTime.Millisecond, $DateTime.Microsecond, $DateTime.Kind);
                                }
                                break;
                            }
                            'Milliseconds' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, $DateTime.Second, $DateTime.Millisecond, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Seconds' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, $DateTime.Second, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Minutes' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0 -or $DateTime.Seconds -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, 0, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Hours' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0 -or $DateTime.Seconds -ne 0 -or $DateTime.Minutes -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, 0, 0, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Days' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0 -or $DateTime.Seconds -ne 0 -or $DateTime.Minutes -ne 0 -or $DateTime.Hours -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, 0, 0, 0, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                        }
                    }
                    if ($DateTime.Nanosecond -ne 0) {
                        "[DateTime]::new($($DateTime.Ticks)L, [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                    } else {
                        if ($DateTime.Microsecond -ne 0) {
                            "[DateTime]::new($($DateTime.Year), $($DateTime.Month), $($DateTime.Day), $($DateTime.Hour), $($DateTime.Minute), $($DateTime.Second), $($DateTime.Millisecond), $($DateTime.Microsecond), [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                        } else {
                            if ($DateTime.Millisecond -ne 0) {
                                "[DateTime]::new($($DateTime.Year), $($DateTime.Month), $($DateTime.Day), $($DateTime.Hour), $($DateTime.Minute), $($DateTime.Second), $($DateTime.Millisecond), [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                            } else {
                                "[DateTime]::new($($DateTime.Year), $($DateTime.Month), $($DateTime.Day), $($DateTime.Hour), $($DateTime.Minute), $($DateTime.Second), [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                            }
                        }
                    }
                    break;
                }
                { $_ -is [DateOnly] } {
                    "[DateOnly]::new($($InputObject.Year), $($InputObject.Month), $($InputObject.Day))" | Write-Output;
                    break;
                }
                { $_ -is [TimeOnly] } {
                    [TimeOnly]$TimeOnly = $InputObject;
                    if ($PSBoundParameters.ContainsKey('TruncateDateTime')) {
                        switch ($TruncateDateTime) {
                            'Microseconds' {
                                if ($TimeOnly.Nanosecond -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, $TimeOnly.Second, $TimeOnly.Millisecond, $TimeOnly.Microsecond);
                                }
                                break;
                            }
                            'Milliseconds' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, $TimeOnly.Second, $TimeOnly.Millisecond, 0);
                                }
                                break;
                            }
                            'Seconds' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0 -or $TimeOnly.Millisecond -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, $TimeOnly.Second, 0, 0);
                                }
                                break;
                            }
                            'Minutes' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0 -or $TimeOnly.Millisecond -ne 0 -or $TimeOnly.Second -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, 0, 0, 0);
                                }
                                break;
                            }
                            'Hours' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0 -or $TimeOnly.Millisecond -ne 0 -or $TimeOnly.Second -ne 0 -or $TimeOnly.Minute -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, 0, 0, 0, 0);
                                }
                                break;
                            }
                        }
                    }
                    if ($TimeOnly.Nanosecond -ne 0) {
                        "[TimeOnly]::new($($TimeOnly.Ticks)L)" | Write-Output;
                    } else {
                        if ($TimeOnly.Microsecond -ne 0) {
                            "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute), $($TimeOnly.Second), $($TimeOnly.Millisecond), $($TimeOnly.Microsecond))" | Write-Output;
                        } else {
                            if ($TimeOnly.Millisecond -ne 0) {
                                "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute), $($TimeOnly.Second), $($TimeOnly.Millisecond))" | Write-Output;
                            } else {
                                if ($TimeOnly.Second -ne 0) {
                                    "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute), $($TimeOnly.Second))" | Write-Output;
                                } else {
                                    "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute))" | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                { $_ -is [Guid] } {
                    if ($InputObject -eq [Guid]::Empty) {
                        '[Guid]::Empty' | Write-Output;
                    } else {
                        "[Guid]::new('$($InputObject.ToString('d'))')" | Write-Output;
                    }
                    break;
                }
                { $_ -is [TimeSpan] } {
                    if ($InputObject -eq [TimeSpan]::Zero) {
                        '[TimeSpan]::Zero' | Write-Output;
                    } else {
                        [TimeSpan]$TimeSpan = $InputObject;
                        if ($PSBoundParameters.ContainsKey('TruncateDateTime')) {
                            switch ($TruncateDateTime) {
                                'Microseconds' {
                                    if ($TimeSpan.Nanosecond -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, $TimeSpan.Second, $TimeSpan.Millisecond, $TimeSpan.Microsecond);
                                    }
                                    break;
                                }
                                'Milliseconds' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, $TimeSpan.Second, $TimeSpan.Millisecond, 0);
                                    }
                                    break;
                                }
                                'Seconds' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0 -or $TimeSpan.Millisecond -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, $TimeSpan.Second, 0, 0);
                                    }
                                    break;
                                }
                                'Minutes' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0 -or $TimeSpan.Millisecond -ne 0 -or $TimeSpan.Second -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, 0, 0, 0);
                                    }
                                    break;
                                }
                                'Hours' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0 -or $TimeSpan.Millisecond -ne 0 -or $TimeSpan.Second -ne 0 -or $TimeSpan.Minute -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, 0, 0, 0, 0);
                                    }
                                    break;
                                }
                            }
                        }
                        if ($TimeSpan.Nanosecond -ne 0) {
                            "[TimeSpan]::new($($TimeSpan.Ticks)L)" | Write-Output;
                        } else {
                            if ($TimeSpan.Microsecond -ne 0) {
                                "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), $($TimeSpan.Second), $($TimeSpan.Millisecond), $($TimeSpan.Microsecond))" | Write-Output;
                            } else {
                                if ($TimeSpan.Millisecond -ne 0) {
                                    "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), $($TimeSpan.Second), $($TimeSpan.Millisecond))" | Write-Output;
                                } else {
                                    if ($TimeSpan.Second -ne 0) {
                                        "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), $($TimeSpan.Second))" | Write-Output;
                                    } else {
                                        "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), 0)" | Write-Output;
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
                { $_ -is [Uri] } {
                    if (([Uri]$InputObject).IsAbsoluteUri) {
                        "[Uri]::new('$($InputObject.AbsoluteUri)', [UriKind]::Absolute)" | Write-Output;
                    } else {
                        "[Uri]::new('$($InputObject.OriginalString | ConvertTo-PsStringLiteral)', [UriKind]::Relative)" | Write-Output;
                    }
                    break;
                }
                { $_ -is [Version] } {
                    if ($InputObject.Revision -lt 0) {
                        if ($InputObject.Build -lt 0) {
                            "[Version]::new($($InputObject.Major), $($InputObject.Minor))" | Write-Output;
                        } else {
                            "[Version]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Build))" | Write-Output;
                        }
                    } else {
                        "[Version]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Build), $($InputObject.Revision))" | Write-Output;
                    }
                    break;
                }
                { $_ -is [System.Management.Automation.SemanticVersion] } {
                    if ([string]::IsNullOrEmpty($InputObject.BuildLabel)) {
                        if ([string]::IsNullOrEmpty($InputObject.PreReleaseLabel)) {
                            if ($InputObject.Patch -lt 0) {
                                "[semver]::new($($InputObject.Major), $($InputObject.Minor))" | Write-Output;
                            } else {
                                "[semver]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Patch))" | Write-Output;
                            }
                        } else {
                            "[semver]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Patch), $($InputObject.PreReleaseLabel | ConvertTo-PsStringLiteral))" | Write-Output;
                        }
                    } else {
                        "[semver]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Patch), $($InputObject.PreReleaseLabel | ConvertTo-PsStringLiteral), $($InputObject.BuildLabel | ConvertTo-PsStringLiteral))" | Write-Output;
                    }
                    break;
                }
                { $_ -is [ScriptBlock] } {
                    "{$InputObject}" | Write-Output;
                    break;
                }
                { $_ -is [enum] } {
                    $Type = $InputObject.GetType();
                    $Tn = [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type);
                    if ($Type.GetCustomAttributes([System.FlagsAttribute], $false).Length -gt 0) {
                        $Ut = [enum]::GetUnderlyingType($Type);
                        if ([System.Management.Automation.LanguagePrimitives]::ConvertTo($InputObject, $Ut) -eq 0) {
                            "$($Tn)::$($InputObject.ToString('F'))" | Write-Output;
                        } else {
                            $FlagValues = @([enum]::GetValues($Type) | Where-Object { $InputObject.HasFlag($_) -and [System.Management.Automation.LanguagePrimitives]::ConvertTo($_, $Ut) -ne 0 } | ForEach-Object {
                                "$($Tn)::$($_.ToString('F'))" | Write-Output;
                            });
                            if ($FlagValues.Count -lt 2) {
                                "$($Tn)::$($InputObject.ToString('F'))" | Write-Output;
                            } else {
                                "($Tn($($FlagValues -join ' -bor ')))" | Write-Output;
                            }
                        }
                    } else {
                        "$($Tn)::$($InputObject.ToString('F'))" | Write-Output;
                    }
                    break;
                }
                default {
                    switch ($StringQuotes) {
                        'PreferDouble' {
                            ($InputObject | ConvertTo-PsStringLiteral -PreferDoubleQuotes) | Write-Output;
                            break;
                        }
                        'AlwaysDouble' {
                            ($InputObject | ConvertTo-PsStringLiteral -AlwaysDoubleQuotes) | Write-Output;
                            break;
                        }
                        default {
                            ($InputObject | ConvertTo-PsStringLiteral -PreferSingleQuotes) | Write-Output;
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}