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
                if ($InputType.BaseType.BaseType -ne $null) {
                    $Script:IndentText + 'Inherits From:';
                    for ($t = $InputType.BaseType; $t.BaseType -ne $null; $t = $t.BaseType) { '{0}{0}$obj -is [{1}]' -f $Script:IndentText, ($t | Get-ClrSimpleName) };
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
        if ($Script:ProjectTypesDocument -eq $null) {
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
					if ($a -eq $null) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
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
					if ($a -eq $null) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
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
					if ($a -eq $null) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
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
			if ($a -eq $null) { $Properties['Language'] = '' } else { $Properties['Language'] = $a.Value }
			$a = $_.SelectSingleNode('@Package');
			if ($a -eq $null) { $Properties['Package'] = $null } else { $Properties['Package'] = [Guid]::Parse($a.Value) }
			$Properties['Guid'] = [Guid]::Parse($Properties['Guid']);
			$Properties['AltGuids'] = @($Properties['AltGuids'] | ForEach-Object { [Guid]::Parse($_) });
			New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
		}
    }
}

$Script:MamlSchema = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    msh = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        ns = 'http://msh';
        rootElement = 'helpItems';
    };
    maml = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        ns = 'http://schemas.microsoft.com/maml/2004/10';
    };
    command = 'http://schemas.microsoft.com/maml/dev/command/2004/10';
    dev = 'http://schemas.microsoft.com/maml/dev/2004/10';
};

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
                if ($x -eq $null -or $PSMaml.DocumentElement -eq $null -or $PSMaml.DocumentElement.LocalName -ne $Script:MamlSchema.msh.rootElement -or $PSMaml.DocumentElement.NamespaceURI -ne $Script:MamlSchema.msh.ns) {
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
            if ($VerbText -eq $null) {
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
        if ($Script:__Test_CommandVerb -eq $null) {
            $Script:__Test_CommandVerb = @(Get-Verb | ForEach-Object { $_.Verb })
        }
        $Success = $true;
    }
    Process {
        if ($GetMatching) {
            foreach ($v in $Verb) {
                if ($v -ne $null) {
                    $s = $v.Trim();
                    $Script:__Test_CommandVerb | Where-Object { $_ -ieq $s }
                }
            }
        } else {
            if ($Success) {
                foreach ($v in $Verb) {
                    if ($v -eq $null -or $Script:__Test_CommandVerb -inotcontains $v) {
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