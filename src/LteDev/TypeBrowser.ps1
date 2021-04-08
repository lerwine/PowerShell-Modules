Function Get-CSTypeName {
    [CmdletBinding(DefaultParameterSetName = 'FullName')]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Type[]]$InputType,

        [ValidateScript({
            $AliasRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^([_a-z][a-z\d]*)?$', ([System.Text.RegularExpressions.RegexOptions]::IgnoreCase);
            $FullNameRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^[_a-z][_a-z\d]*(\.[_a-z][_a-z\d]*)*$', ([System.Text.RegularExpressions.RegexOptions]::IgnoreCase);
            return (@(($_) | Where-Object {
                if ($null -eq $_) {
                    $true;
                } else {
                    if ($_ -is [string]) {
                        $_.Length -eq 0 -or -not $FullNameRegex.IsMatch($_);
                    } else {
                        $null -eq $_.Namespace -or $_.Namespace -isnot [string] -or -not $FullNameRegex.IsMatch($_.Namespace) -or ($null -ne $_.Alias -and ($_.Alias -isnot [string] -or -not $AliasRegex.IsMatch($_.Alias)))
                    }
                }
            }).Count -eq 0);
        })]
        [object[]]$Using,

        [Parameter(ParameterSetName = 'Parts')]
        [switch]$Name,

        [Parameter(ParameterSetName = 'Parts')]
        [switch]$Namespace,

        [Parameter(ParameterSetName = 'Parts')]
        [switch]$DeclaringType,

        [Parameter(ParameterSetName = 'Parts')]
        [switch]$ElementType,

        [Parameter(ParameterSetName = 'Parts')]
        [switch]$Indices,

        [Parameter(ParameterSetName = 'Parts')]
        [switch]$GenericArguments,

        [Parameter(ParameterSetName = 'FullName')]
        [switch]$FullName
    )

    Begin {
        $StringType = [System.String];
        $VoidType = [System.Void];
        $DecimalType = [System.Decimal];
        if ($null -eq $__GetCSTypeNameGenericNameRegex) {
            $__GetCSTypeNameGenericNameRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '`\d+$', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
        }
        $NsAliases = @();
        if ($PSBoundParameters.ContainsKey('Using')) {
            $NsAliases = @($Using | ForEach-Object {
                if ($_ -is [string]) {
                    @{ Namespace = $_; Alias = '' };
                } else {
                    if ($null -eq $_.Alias) {
                        @{ Namespace = $_.Namespace; Alias = '' };
                    } else {
                        $_;
                    }
                }
            });
        }
    }
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'FullName') {
            foreach ($Type in $InputType) {
                if ($Type.IsArray) {
                    $ArrayRank = $Type.GetArrayRank();
                    $s = '';
                    if ($ArrayRank -eq 1) { $s = '[]' } else { if ($ArrayRank -eq 1) { $s = '[,]' } else { $s = "[$(New-Object -TypeName 'System.String' -ArgumentList ([char]','), ($ArrayRank - 1))]" } }
                    if ($NsAliases.Count -gt 0) {
                        ($Type.GetElementType() | Get-CSTypeName -Using $NsAliases) + $s;
                    } else {
                        ($Type.GetElementType() | Get-CSTypeName) + $s;
                    }
                } else {
                    if ($Type.IsByRef) {
                        if ($NsAliases.Count -gt 0) {
                            ($Type.GetElementType() | Get-CSTypeName -Using $NsAliases) + '&';
                        } else {
                            ($Type.GetElementType() | Get-CSTypeName) + '&';
                        }
                    } else {
                        if ($Type.IsPointer) {
                            if ($NsAliases.Count -gt 0) {
                                ($Type.GetElementType() | Get-CSTypeName -Using $NsAliases) + '*';
                            } else {
                                ($Type.GetElementType() | Get-CSTypeName) + '*';
                            }
                        } else {
                            $NS = '';
                            if ($Type.IsNested) {
                                if ($NsAliases.Count -gt 0) {
                                    $NS = $Type.DeclaringType | Get-CSTypeName -Using @NsAliases;
                                } else {
                                    $NS = $Type.DeclaringType | Get-CSTypeName
                                }
                            } else {
                                $NS = $Type.Namespace;
                            }

                            $TN = $Type.Name;
                            if ($Type.IsPrimitive) {
                                switch ($Type.FullName) {
                                    'System.Boolean' { $NS = ''; $TN = "bool$ai"; break; }
                                    'System.Byte' { $NS = ''; $TN = "byte$ai"; break; }
                                    'System.Char' { $NS = ''; $TN = "char$ai"; break; }
                                    'System.Double' { $NS = ''; $TN = "double$ai"; break; }
                                    'System.Int16' { $NS = ''; $TN = "short$ai"; break; }
                                    'System.Int32' { $NS = ''; $TN = "int$ai"; break; }
                                    'System.Int64' { $NS = ''; $TN = "long$ai"; break; }
                                    'System.SByte' { $NS = ''; $TN = "sbyte$ai"; break; }
                                    'System.Single' { $NS = ''; $TN = "float$ai"; break; }
                                    'System.UInt16' { $NS = ''; $TN = "ushort$ai"; break; }
                                    'System.UInt32' { $NS = ''; $TN = "uint$ai"; break; }
                                    'System.UInt64' { $NS = ''; $TN = "ulong$ai"; break; }
                                }
                            } else {
                                if ($StringType.IsAssignableFrom($Type)) {
                                    $TN = "string$ai";
                                    $NS = '';
                                } else {
                                    if ($DecimalType.IsAssignableFrom($Type)) {
                                        $TN = "decimal$ai";
                                        $NS = '';
                                    } else {
                                        if ($VoidType.IsAssignableFrom($Type)) {
                                            $TN = 'void';
                                            $NS = '';
                                        }
                                    }
                                }
                            }
                            if ($Type.IsGenericType) {
                                $TN = $__GetCSTypeNameGenericNameRegex.Replace($TN, '');
                                $GA = $Type.GetGenericArguments();
                                if ($Type.IsGenericTypeDefinition) {
                                    $TN = "$TN<$(($GA | ForEach-Object { $_.Name }) -join ',')>";
                                } else {
                                    if ($NsAliases.Count -gt 0) {
                                        $TN = "$TN<$(($GA | ForEach-Object { $_ | Get-CSTypeName -Using @NsAliases }) -join ',')>";
                                    } else {
                                        $TN = "$TN<$(($GA | ForEach-Object { $_ | Get-CSTypeName }) -join ',')>";
                                    }
                                }
                            }
                        
                            if ([string]::IsNullOrEmpty($NS)) {
                                $TN;
                            } else {
                                if (-not ($Type.IsNested -or $NsAliases.Count -eq 0)) {
                                    $mapping = $NsAliases | Where-Object { $_.Namespace -ceq $NS } | Select-Object -Last 1;
                                    if ($null -ne $mapping) { $NS = $mapping.Alias };
                                }
                                if ($NS.Length -eq 0) { $TN } else { "$NS.$TN" }
                            }
                        }
                    }
                }
            }
        } else {
            if ($NsAliases.Count -gt 0) { $Splat['Using'] = $NsAliases }
            foreach ($Type in $InputType) {
                $ai = '';
                $CurrentType = $Type;
                $hasElement = $CurrentType.IsArray -or $CurrentType.IsByRef -or $CurrentType.IsPointer;
                if ($hasElement) {
                    if ($Indices) {
                        do {
                            if ($CurrentType.IsArray) {
                                $ArrayRank = $CurrentType.GetArrayRank();
                                if ($ArrayRank -eq 1) { $ai = "$ai[]" } else { if ($ArrayRank -eq 1) { $ai = "$ai[,]" } else { $ai = "$ai[$(New-Object -TypeName 'System.String' -ArgumentList ([char]','), ($ArrayRank - 1))]" } }
                                $CurrentType = $CurrentType.GetElementType();
                            } else {
                                if ($CurrentType.IsByRef) {
                                    $ai = "$ai&";
                                    $CurrentType = $CurrentType.GetElementType();
                                } else {
                                    if ($CurrentType.IsPointer) {
                                        $ai = "$ai*";
                                        $CurrentType = $CurrentType.GetElementType();
                                    } else {
                                        $hasElement = $false;
                                    }
                                }
                            }
                        } while ($hasElement);
                        if ($ElementType) {
                            if (-not ($Name -or $Namespace -or $DeclaringType -or $GenericArguments)) {
                                if ($NsAliases.Count -gt 0) {
                                    "$($CurrentType | Get-CSTypeName -Using @NsAliases)$ai";
                                } else {
                                    "$($CurrentType | Get-CSTypeName)$ai";
                                }
                                continue;
                            }
                        } else {
                            if (-not ($Name -or $Namespace -or $DeclaringType -or $GenericArguments)) {
                                $ai | Write-Output;
                                continue;
                            }
                        }
                    } else {
                        do { $CurrentType = $CurrentType.GetElementType() } while ($CurrentType.IsArray -or $CurrentType.IsByRef -or $CurrentType.IsPointer);
                        if ($ElementType) {
                            if (-not ($Name -or $Namespace -or $DeclaringType -or $GenericArguments)) {
                                if ($NsAliases.Count -gt 0) {
                                    "$($CurrentType | Get-CSTypeName -Using @NsAliases)$ai";
                                } else {
                                    "$($CurrentType | Get-CSTypeName)$ai";
                                }
                                continue;
                            }
                        } else {
                            if (-not ($Name -or $Namespace -or $DeclaringType -or $GenericArguments)) {
                                '' | Write-Output;
                                continue;
                            }
                        }
                    }
                }
                $TN = '';
                $NS = '';
                if ($Namespace) { $NS = $CurrentType.Namespace }
                if ($Name) { $TN = $CurrentType.Name }
                if ($CurrentType.IsGenericType) {
                    if ($Name) { $TN = $__GetCSTypeNameGenericNameRegex.Replace($TN, '') }
                    if ($GenericArguments) {
                        if ($CurrentType.IsGenericTypeDefinition) {
                            $TN = "$TN<$(($CurrentType.GetGenericArguments() | ForEach-Object { $_.Name }) -join ',')>$ai";
                        } else {
                            if ($NsAliases.Count -gt 0) {
                                $TN = "$TN<$(($CurrentType.GetGenericArguments() | ForEach-Object { $_ | Get-CSTypeName -Using @NsAliases }) -join ',')>$ai";
                            } else {
                                $TN = "$TN<$(($CurrentType.GetGenericArguments() | ForEach-Object { $_ | Get-CSTypeName }) -join ',')>$ai";
                            }
                        }
                    } else {
                        $TN += $ai;
                    }
                } else {
                    $TN += $ai;
                }
                if ($CurrentType.IsNested) {
                    if ($DeclaringType) {
                        if ($Namespace) {
                            if ($NsAliases.Count -gt 0) {
                                "$($CurrentType.DeclaringType | Get-CSTypeName -Using @NsAliases).$TN";
                            } else {
                                "$($CurrentType.DeclaringType | Get-CSTypeName -Namespace -Name -DeclaringType -GenericArguments).$TN";
                            }
                        } else {
                            if ($NsAliases.Count -gt 0) {
                                "$($CurrentType.DeclaringType | Get-CSTypeName -Name -DeclaringType -GenericArguments -Using @NsAliases).$TN";
                            } else {
                                "$($CurrentType.DeclaringType | Get-CSTypeName -Name -DeclaringType -GenericArguments).$TN";
                            }
                        }
                    } else {
                        $TN | Write-Output;
                    }
                } else {
                    if ($Name) {
                        if ($CurrentType.IsPrimitive) {
                            switch ($CurrentType.FullName) {
                                'System.Boolean' { $NS = ''; $TN = "bool$ai"; break; }
                                'System.Byte' { $NS = ''; $TN = "byte$ai"; break; }
                                'System.Char' { $NS = ''; $TN = "char$ai"; break; }
                                'System.Double' { $NS = ''; $TN = "double$ai"; break; }
                                'System.Int16' { $NS = ''; $TN = "short$ai"; break; }
                                'System.Int32' { $NS = ''; $TN = "int$ai"; break; }
                                'System.Int64' { $NS = ''; $TN = "long$ai"; break; }
                                'System.SByte' { $NS = ''; $TN = "sbyte$ai"; break; }
                                'System.Single' { $NS = ''; $TN = "float$ai"; break; }
                                'System.UInt16' { $NS = ''; $TN = "ushort$ai"; break; }
                                'System.UInt32' { $NS = ''; $TN = "uint$ai"; break; }
                                'System.UInt64' { $NS = ''; $TN = "ulong$ai"; break; }
                            }
                        } else {
                            if ($StringType.IsAssignableFrom($CurrentType)) {
                                $TN = "string$ai";
                                $NS = '';
                            } else {
                                if ($DecimalType.IsAssignableFrom($CurrentType)) {
                                    $TN = "decimal$ai";
                                    $NS = '';
                                } else {
                                    if ($VoidType.IsAssignableFrom($CurrentType)) {
                                        $TN = 'void';
                                        $NS = '';
                                    }
                                }
                            }
                        }
                    } else {
                        if ($Namespace) {
                            if ($CurrentType.IsPrimitive) {
                                switch ($CurrentType.FullName) {
                                    'System.Boolean' { $NS = ''; break; }
                                    'System.Byte' { $NS = ''; break; }
                                    'System.Char' { $NS = ''; break; }
                                    'System.Double' { $NS = ''; break; }
                                    'System.Int16' { $NS = ''; break; }
                                    'System.Int32' { $NS = ''; break; }
                                    'System.Int64' { $NS = ''; break; }
                                    'System.SByte' { $NS = ''; break; }
                                    'System.Single' { $NS = ''; break; }
                                    'System.UInt16' { $NS = ''; break; }
                                    'System.UInt32' { $NS = ''; break; }
                                    'System.UInt64' { $NS = ''; break; }
                                }
                            } else {
                                if ($StringType.IsAssignableFrom($CurrentType) -or $DecimalType.IsAssignableFrom($CurrentType) -or $VoidType.IsAssignableFrom($CurrentType)) { $TN = '' }
                            }
                        } else {
                            $NS = '';
                        }
                    }
                    if ([string]::IsNullOrEmpty($NS)) {
                        $TN | Write-Output;
                    } else {
                        if (-not ($CurrentType.IsNested -or $NsAliases.Count -gt 0)) {
                            $mapping = $NsAliases | Where-Object { $_.Namespace -ceq $NS } | Select-Object -Last 1;
                            if ($null -ne $mapping) { $NS = $mapping.Alias };
                        }
                        if ($NS.Length -gt 0) { "$NS.$TN" } else { $TN }
                    }
                }
            }
        }
    }
}
$Script:AssemblySelectables = @(([System.AppDomain]::CurrentDomain.GetAssemblies() | Sort-Object -Property 'FullName') | ForEach-Object {
    [System.Reflection.AssemblyName]$AssemblyName = $_.GetName();
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        GridViewItem = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            CodeBase = $_.CodeBase;
            GAC = $_.GlobalAssemblyCache;
            ImageRuntimeVersion = $_.ImageRuntimeVersion;
            IsFullyTrusted = $_.IsFullyTrusted;
            Location = $_.Location;
            Culture = $AssemblyName.CultureName;
            Name = $AssemblyName.Name;
            ProcessorArchitecture = $AssemblyName.ProcessorArchitecture;
            Version = $AssemblyName.Version;
            PublicKeyToken = -join ($AssemblyName.GetPublicKeyToken() | ForEach-Object { ([int]$_).ToString('x2') });
        };
        Assembly = $_;
    };
});

$Script:NamespaceSelectables = New-Object -TypeName 'System.Collections.Generic.Dictionary[System.String,System.Collections.ObjectModel.Collection[System.Reflection.Assembly]]' -ArgumentList ([System.StringComparer]::InvariantCulture);
$Script:AssemblySelectables | ForEach-Object {
    $ns = @();
    $_.Assembly.GetTypes() | ForEach-Object {
        if ([String]::IsNullOrEmpty($_.Namespace)) {
            if ($ns -cnotcontains '') {
                $ns += @('');
                $Collection = $null;
                if ($Script:NamespaceSelectables.ContainsKey('')) {
                    $Script:NamespaceSelectables[''].Add($_.Assembly);
                } else {
                    $Collection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Reflection.Assembly]';
                    $Script:NamespaceSelectables.Add('', $Collection);
                    $Collection.Add($_.Assembly);
                }
            }
        } else {
            if ($ns -cnotcontains $_.Namespace) {
                $ns += @($_.Namespace);
                $Collection = $null;
                if ($Script:NamespaceSelectables.ContainsKey($_.Namespace)) {
                    $Script:NamespaceSelectables[$_.Namespace].Add($_.Assembly);
                } else {
                    $Collection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Reflection.Assembly]';
                    $Script:NamespaceSelectables.Add($_.Namespace, $Collection);
                    $Collection.Add($_.Assembly);
                }
            }
        }
    }
}

Function Select-AssemblyFromGridView {
    Param()

    $SelectedItem = $Script:AssemblySelectables | ForEach-Object { $_.GridViewItem } | Out-GridView -Title 'Select Assembly' -OutputMode Single;
    if ($null -ne $SelectedItem) {
        for ($i = 0; $i -lt $AssemblySelectables.Length; $i++) {
            if ([System.Object]::ReferenceEquals($AssemblySelectables[$i].GridViewItem, $SelectedItem)) {
                return $AssemblySelectables[$i].Assembly;
            }
        }
    }
}


Function Select-NamespaceFromGridView {
    Param()

    $Items = @($Script:NamespaceSelectables.Keys | Sort-Object | ForEach-Object {
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            GridViewItem = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                Namespace = $_;
                Assemblies = ($Script:NamespaceSelectables[$_] | ForEach-Object { $_.FullName } | Sort-Object | Out-String).Trim();
            }
            Assemblies = $Script:NamespaceSelectables[$_];
        }
    });
    $SelectedItem = $Items | ForEach-Object { $_.GridViewItem } | Out-GridView -Title 'Select Assembly' -OutputMode Single;
    if ($null -ne $SelectedItem) {
        for ($i = 0; $i -lt $Items.Length; $i++) {
            if ([System.Object]::ReferenceEquals($Items[$i].GridViewItem, $SelectedItem)) {
                return New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ Namespace = $Items[$i].GridViewItem.Namespace; Assemblies = $Items[$i].Assemblies; };
            }
        }
    }
}

Function Select-AssemblyTypeFromGridView {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [System.Reflection.Assembly[]]$Assemblies,
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$Namespace
    )
    
    Begin {
        $Items = @();
    }

    Process {
        $Items = @($Assemblies | ForEach-Object {
            foreach ($Type in $_.GetTypes()) {
                $Ns = $Type | Get-CSTypeName -Namespace;
                if ($null -eq $NS) { $NS = '' }
                $TN = '';
                if ($Type.IsInterface) {
                    $TN = 'interface';
                } else {
                    if ($Type.IsClass) {
                        $TN = 'class';
                    } else {
                        if ($Type.IsEnum) {
                            $TN = 'enum';
                        } else {
                            if ($Type.IsValueType) {
                                $TN = 'struct';
                            }
                        }
                    }
                }
                $TV = '';
                if ($Type.IsAbstract) {
                    $TV = 'abstract';
                } else {
                    if ($Type.IsSealed) {
                        $TV = 'sealed';
                    } else {
                        if ($Type.IsEnum) {
                            $TV = 'enum';
                        } else {
                            if ($Type.IsAutoLayout) {
                                $TV = 'auto-layout';
                            } else {
                                if ($Type.IsExplicitLayout) {
                                    $TV = 'explicit layout';
                                } else {
                                    if ($Type.IsLayoutSequential) {
                                        $TV = 'sequential layout';
                                    }
                                }
                            }
                        }
                    }
                }

                $BaseType = $Type.BaseType;;
                
                if ($Type.IsGenericType -and -not $Type.IsGenericTypeDefinition) {
                    $BaseType = $Type.GetGenericTypeDefinition();
                }
                $BT = '';
                if ($null -ne $BaseType) { $BT = $BaseType | Get-CSTypeName }
                New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                    GridViewItem = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                        Namespace = $NS;
                        Name = $Type | Get-CSTypeName -Name;
                        GenericArguments = $Type | Get-CSTypeName -GenericArguments;
                        Assembly = $Type.Assembly.FullName;
                        ObjectType = $TN;
                        TypeVariant = $TV;
                        BaseType = $BT;
                        Interfaces = (($Type.GetInterfaces() | Get-CSTypeName) | Out-String).Trim();
                    }
                    Type = $Type;
                }
            }
        });
        $SelectedItem = $Items | ForEach-Object { $_.GridViewItem } | Out-GridView -Title 'Select Type' -OutputMode Single;
        if ($null -ne $SelectedItem) {
            for ($i = 0; $i -lt $Items.Length; $i++) {
                if ([System.Object]::ReferenceEquals($Items[$i].GridViewItem, $SelectedItem)) {
                    return $Items[$i].Type;
                }
            }
        }
    }
}

$Assembly = Select-AssemblyFromGridView;
if ($null -ne $Assembly) { $Assembly | Select-AssemblyTypeFromGridView }
