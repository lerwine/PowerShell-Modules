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
	
	Begin { $LastName = '' }
	
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

Function New-HelpItemsDocument {
	<#
		.SYNOPSIS
			Create new XML Help document
 
		.DESCRIPTION
			Generates empty MAML help XML document.
	#>
	[CmdletBinding()]
	[OutputType([System.Xml.XmlDocument])]
	Param(
		[Parameter(Mandatatory = $true, ValueFromPipeline = $true)]
		[object]$HelpSourceItem
	)
	
	Process {
		[HelpXml.HelpSourceItem]::CreateHelp($HelpSourceItem)
	}
}

Function New-HelpSourceItem {
	<#
		.SYNOPSIS
			Create new XML Help document
 
		.DESCRIPTION
			Generates empty MAML help XML document.
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatatory = $true)]
		[System.Reflection.Assembly]$ModuleAssembly,

		[Parameter(Mandatatory = $true)]
		[string]$Author,

		[string]$DocPath
	)
	
	$XmlDocument = $null;
	if ($PSBounParameters.ContainsKey('DocPath')) {
		$XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
		$XmlDocument.Load($DocPath);
	}
	New-Object -TypeName 'HelpXml.HelpSourceItem' -ArgumentList $ModuleAssembly, $XmlDocument, $Author;
}

Function Find-ProjectTypeInfo {
	[CmdletBinding(DefaultParameterSetName = 'Guid')]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Name')]
		[string[]]$Name,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'FileExtension')]
		[string[]]$FileExtension,
		
		[Parameter(ParameterSetName = 'Guid')]
		[Parameter(Mandatory = $true, ParameterSetName = 'GuidPrimary')]
		[System.Guid[]]$Guid,

		[Parameter(ParameterSetName = 'FileExtension')]
		[Parameter(Mandatory = $true, ParameterSetName = 'GuidPrimary')]
		[switch]$Primary
	)

	Begin {
		if ($Script:ProjectTypesByName -eq $null) {
			$Script:ProjectTypesByName = @{};
			$Script:ProjectTypesByGuid = [System.Collections.Generic.Dictionary[System.Guid, System.Collections.ObjectModel.Collection[System.String]]]::new();
			if ($MyInvocation.MyCommand.Module.PrivateData -ne $null -and ($ProjectTypes = $MyInvocation.MyCommand.Module.PrivateData.ProjectTypes) -ne $null -and $ProjectTypes -is [Hashtable] -and $ProjectTypes.Count -gt 0) {
				foreach ($Name in @($ProjectTypes.Keys)) {
					$SourceItem = @($ProjectTypes[$Name])[0];
					$TargetItem = @{ Name = $Name };
					$SourceItem.Keys | ForEach-Object { $TargetItem[$_] = $SourceItem[$_]; }
					$TargetItem['Guid'] = [System.Guid]::Parse($TargetItem['Guid']);
					if (-not $TargetItem.ContainsKey('IsPrimary')) { $TargetItem['IsPrimary'] = $false }
					if ($TargetItem['Description'] -eq $null -or ($TargetItem['Description'] = $TargetItem['Description'].Trim()).Length -eq 0) {
						$TargetItem['Description'] = $TargetItem['Name'];
					}
					if ($TargetItem['Extension'] -ne $null) {
						if (($TargetItem['Extension'] = $TargetItem['Extension'].Trim()).Length -eq 0) {
							$TargetItem.Remove('Extension');
						} else {
							if ($TargetItem['Extension'].Substring(0, 1) -ne '.') { $TargetItem['Extension'] = ".$($TargetItem['Extension'])" }
						}
					}
					$Script:ProjectTypesByName[$Name] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $TargetItem;
					if ($Script:ProjectTypesByGuid.ContainsKey($TargetItem['Guid'])) {
						$Script:ProjectTypesByGuid[$TargetItem['Guid']].Add($Name);
					} else {
						$c = [System.Collections.ObjectModel.Collection[System.String]]::new();
						$c.Add($Name);
						$Script:ProjectTypesByGuid.Add($TargetItem['Guid'], $c);
					}
				}
				foreach ($Guid in @($Script:ProjectTypesByGuid.Keys)) {
					if ($Script:ProjectTypesByGuid[$Guid].Count -gt 1) {
						$Items = @($Script:ProjectTypesByGuid[$Guid] | ForEach-Object { $Script:ProjectTypesByName[$_] });
						$PrimaryArr = @($Items | Where-Object { $_.IsPrimary });
						if ($PrimaryArr.Count -eq 0) {
							Write-Warning -Message "No Primary: $(($Items | ForEach-Object { "$($_.Name) ($($_.Description))" }) -join ', ')";
						} else {
							if ($PrimaryArr.Count -gt 1) {
								Write-Warning -Message "Multiple Primary: $(($PrimaryArr | ForEach-Object { "$($_.Name) ($($_.Description))" }) -join ', ')";
							}
						}
						$Extensions = @($Items | Where-Object { $_.Extension -ne $null });
						if ($Extensions.Count -gt 0 -and $Extensions.Count -lt $Items.Count) {
							if ($Extensions.Count -gt 1) {
								$e = @($Extensions | Where-Object { $_.IsPrimary });
								if ($e.Count -gt 0) { $Extensions = $e }
							}
							$Items | Where-Object { $_.Extension -eq $null } | ForEach-Object { $_ | Add-Member -MemberType NoteProperty -Name 'Extension' -Value $Extensions[0].Extension }
						}
					}
				}
			}
		}
	}

	Process {
		$ProjectObjArr = @();
		if ($PSBoundParameters.ContainsKey('Name')) {
			$ProjectObjArr = @(($Name | Select-Object -Unique) | ForEach-Object {
				if ($Script:ProjectTypesByName.ContainsKey($_)) {
					$Script:ProjectTypesByName[$_] | Write-Output;
				}
			});
		} else {
			if ($PSBoundParameters.ContainsKey('FileExtension')) {
				$Matching = @($Script:ProjectTypesByGuid.Keys | ForEach-Object { $Script:ProjectTypesByName[$_]  } | Where-Object { $_.Extension -ne $null -and $_.Extension -ieq $FileExtension });
				if ($Matching.Count -eq 1) {
					$Matching[0] | Write-Output;
				} else {
					if ($Matching.Count -gt 1) {
						if ($Primary) {
							$m = @($Matching | Where-Object { $_.IsPrimary });
							if ($m.Count -gt 0) { $m[0] | Write-Output } else { $Matching[0] | Write-Output }
						} else {
							$Matching | Write-Output;
						}
					}
				}
			} else {
				if ($PSBoundParameters.ContainsKey('Guid')) {
					if ($Primary) {
						(($Guid | Select-Object -Unique) | ForEach-Object {
							if ($Script:ProjectTypesByGuid.ContainsKey($_)) {
								$Items = @($Script:ProjectTypesByGuid[$_] | ForEach-Object { $Script:ProjectTypesByName[$_] | Write-Output });
								if ($Items.Count -gt 1) {
									$a = @($Items | Where-Object { $_.IsPrimary });
									if ($a.Count -gt 0) { $Items = $a }
								}
								$Items[0] | Write-Output;
							}
						}) | Select-Object -Unique;
					} else {
						(($Guid | Select-Object -Unique) | ForEach-Object {
							if ($Script:ProjectTypesByGuid.ContainsKey($_)) {
								$Script:ProjectTypesByGuid[$_] | ForEach-Object { $Script:ProjectTypesByName[$_] | Write-Output }
							}
						}) | Select-Object -Unique;
					}
				} else {
					$Script:ProjectTypesByName.Keys | ForEach-Object { $Script:ProjectTypesByName[$_] | Write-Output }
				}
			}
		}
	}
}

$Script:SolutionContentRegex_old = [System.Text.RegularExpressions.Regex]::new('(\G(\r\n?|\n)|^)(?<i>\t+)?((End(?<e>\S+))|(#(?<c>.*))|(((?<s>[^\r\n\s()=]+)|(?<n>[^\r\n()=]+))(\((?<a>[^()]*)\))?|(?<k>[^\r\n=]*))([ \t]*=[ \t]*(?<v>[^\r\n]*))?)(?=[\r\n]|$)', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase);
$Script:SolutionContentRegex_old = [System.Text.RegularExpressions.Regex]::new(@'

^
(?<s>[a-z])+
[a-z]?
(?:\k<s>(?<-s>))+
(?(s)(?!))$

'@, ([System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor [System.Text.RegularExpressions.RegexOptions]::IgnorePatternWhitespace));

Function Open-VSSolution_old {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Path')]
		[sttring[]]$Path,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Literal')]
		[sttring]$LiteralPath
	)

	Process {
		$ResolvedPaths = @();
		if ($PSBoundParameters.ContainsKey('Path')) {
			$ResolvedPaths = @($Path | Resolve-Path);
		} else {
			$ResolvedPaths = @(Resolve-Path -LiteralPath $LiteralPath);
		}
		$ResolvedPaths | ForEach-Object {
			if ($_ | Test-Path -PathType Leaf) {
				$Text = Get-Content -LiteralPath $_;
				if ($Text.Length -eq 0) {
					Write-Error -Message 'File is empty' -Category InvalidData -ErrorId 'EmptyFile' -TargetObject $_;
				} else {
					$MatchCollection = $SolutionContentRegex.Matches($Text);
					$TokenCollection = [System.Collections.Generic.LinkedList[System.Management.Automation.PSObject]]::new();
					$ParentStack = [System.Collections.Generic.Stack[System.Management.Automation.PSObject]]::new();
					@($MatchCollection) | ForEach-Object {
						$Properties = @{ Kind = ''; Key = ''; Level = 0; Arguments = $null; Value = $null; PreviousLineEnd = $_.Index; Length = $_.Length; Index = $_.Index };
						if ($_.Groups['t'].Success) { $Properties['Level'] = $_.Groups['t'].Length }
						if ($_.Groups['c'].Success) {
							$Properties['Kind'] = 'Comment';
							$Properties['Index'] = $_.Groups['c'].Index;
							$Properties['Value'] = $_.Groups['c'].Value;
						} else {
							if ($_.Groups['c'].Success) {
								$Properties['Kind'] = 'End';
								$Properties['Index'] = $_.Groups['e'].Index - 3;
								$Properties['Key'] = $_.Groups['e'].Value;
							} else {
								if ($_.Groups['k'].Success) {
									$Properties['Kind'] = 'Key';
									$Properties['Index'] = $_.Groups['k'].Index;
									$Properties['Key'] = $_.Groups['k'].Value;
								} else {
									if ($_.Groups['s'].Success) {
										$Properties['Kind'] = 'Start';
										$Properties['Index'] = $_.Groups['s'].Index;
										$Properties['Key'] = $_.Groups['s'].Value;
									} else {
										$Properties['Kind'] = 'Named';
										$Properties['Index'] = $_.Groups['n'].Index;
										$Properties['Key'] = $_.Groups['n'].Value;
									}
									if ($_.Groups['a'].Success) {
										$Properties['Arguments'] = $_.Groups['a'].Value;
									}
								}
								if ($_.Groups['v'].Success) {
									$Properties['Value'] = $_.Groups['v'].Value;
								}
							}
						}
						$TokenCollection.AddLast((New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties));
					}
					$StartIndex = 0;
					while ($TokenCollection.Count -gt 0) {
						$t = $TokenCollection.First;
						if ($t.Kind -eq 'Comment') {
							if ($t.PreviousLineEnd -gt $StartIndex) {
								$SolutionItems.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
									Kind = 'Header'
									Value = $Text.Substring($StartIndex, $t.PreviousLineEnd - $StartIndex);
									Index = $StartIndex;
								}));
							}
							$StartIndex = $t.PreviousLineEnd + $t.Length;
							$SolutionItems.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
								Kind = 'Comment'
								Value = $t.Value;
								Index = $T.Index;
							}));
						}
						if ($t.Level -eq 0 -and $t.Kind -ne 'Comment' -and ($t.Kind -ne 'Key' -or $t.Value -ne $null)) {
							break;
						}
						$TokenCollection.RemoveFirst();
					}

					$ParentStack.Push((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
						Path = $_;
						Items = [System.Collections.Generic.LinkedList[System.Management.Automation.PSObject]]::new();
					}));
					$ChildLevel = 0;
					for ($CurrentNode = $TokenCollection.First; $CurrentNode -ne $null; $CurrentNode = $CurrentNode.Next) {
						$CurrentParent = $ParentStack.Peek();
						$CurrentToken = $CurrentNode.Value;
						$Properties = @{
							Key = $CurrentToken.Key;
							Kind = $CurrentToken.Kind;
							Value = $CurrentToken.Value;
							Arguments = $CurrentToken.Arguments;
							Index = $CurrentToken.Index;
							Length = $CurrentToken.Length - ($CurrentToken.Index - $CurrentToken.PreviousLineEnd);
						};
						if ($Properties['Kind'] -ne 'Comment') {
							$HasChildren = $CurrentNode.Next -ne $null -and $CurrentNode.Next.Value.Level -gt $CurrentToken.Level;
							$IsSection = $HasChildren -or (($CurrentNode.Kind -eq 'Start' -or $CurrentNode.Kind -eq 'Named') -and $CurrentNode.Next -ne $null -and $CurrentNode.Next.Value.Level -eq $CurrentToken.Level -and $CurrentNode.Next.Value.Kind -eq 'End' -and $CurrentNode.Next.Value.Key -eq $CurrentToken.Key);
							if ($IsSection) {
								$Properties['Kind'] = 'Section';
							} else {
								if ($Properties['Kind'] -ne 'Comment') { $Properties['Kind']= 'Setting' }
							}
							if ($CurrentToken.Level -eq $ChildLevel) {
								if ($CurrentToken.Kind -eq 'End' -and $CurrentParent.Items.Last -ne $null -and $CurrentParent.Items.Last.Value.Kind -eq 'Section' -and $CurrentParent.Items.Last.Value.Key -eq $CurrentToken.Key) { continue }

							} else {
								if ($current.Level -lt $ChildLevel) {
									# Need to check to see if it's the end item

								} else {

								}
							}
						}
					}
					if ($t.PreviousLineEnd -gt $StartIndex) {
						$SolutionItems.Add((New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
							Kind = 'Header'
							Value = $Text.Substring($StartIndex, $t.PreviousLineEnd - $StartIndex);
							Index = $StartIndex;
						}));
					}
					New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
						Path = $_;
						Items = $SolutionItems;
					}
				}
			} else {
				Write-Error -Message 'Path not found' -Category ObjectNotFound -ErrorId 'PathNotFound' -TargetObject $_;
			}
		}
	}
}

if (@(Find-ProjectTypeInfo).Count -eq 0) {
	Write-Warning -Message 'No project types defined in module manifest';
}