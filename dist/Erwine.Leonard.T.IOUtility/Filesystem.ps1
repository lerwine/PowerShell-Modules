Function ConvertTo-SafeFileName {
	<#
		.SYNOPSIS
			Converts a string to a usable file name / path.

		.DESCRIPTION
			Encodes a string in a format which is compatible with a file name / path, and can be converted back to the original text.

		.OUTPUTS
			System.String. Text encoded as a valid file name / path.

		.EXAMPLE
			ConvertTo-SafeFileName -InputText 'My *unsafe* file';

		.EXAMPLE
			'c:\my*path\User.string' | ConvertTo-SafeFileName -FullPath;

		.LINK
			ConvertFrom-SafeFileName
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# String to convert to file name
		[string[]]$InputText,

		[Parameter(ParameterSetName = 'FileName')]
		# Only allow file names. This is the default behavior.
		[switch]$FileName,

		[Parameter(Mandatory = $true, ParameterSetName = 'RelativePath')]
		# Only allow relative paths
		[switch]$RelativePath,

		[Parameter(Mandatory = $true, ParameterSetName = 'FullPath')]
		# Allow full path specification
		[switch]$FullPath
	)

	Process {
        foreach ($Text in $InputText) {
            $P = $Text | Split-Path -Parent;
            if ([string]::IsNullOrEmpty($P)) {
                $P = $Text -replace '[\s_:]+', '_';
                if (Test-Path -LiteralPath $P -IsValid) {
                    $P | Write-Output;
                } else {
                    if ($P.Length -eq 1) {
                        "_0x$(([int]$P[0]).ToString('x4'))_" | Write-Output;
                    } else {
                        $Sb = [System.Text.StringBuilder]::new();
                        foreach ($c in $P.ToCharArray()) {
                            if (Test-Path -LiteralPath $c -IsValid) {
                                $Sb.Append($c) | Out-Null;
                            } else {
                                $Sb.Append("_0x$(([int]$c).ToString('x4'))_") | Out-Null;
                            }
                        }
                        $Sb.ToString() | Write-Output;
                    }
                }
            } else {
                "$($P | ConvertTo-SafeFileName)_$(($Text | Split-Path -Leaf) | ConvertTo-SafeFileName)" | Write-Output
            }
        }
    }
}

Function Use-Location {
    <#
    .SYNOPSIS
        Runs ScriptBlock(s) using a specified location.
    .DESCRIPTION
        Changes to a specified location, executes the ScriptBlock(s), and restores the previous location when complete.
        If more than one path is specified, each of the ScriptBlock will be executed for each path.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    Param(
        # ScriptBlock(s) will be executed for each path.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, HelpMessage = 'ScriptBlock to run.')]
        [ScriptBlock[]]$Process,

        # Specifies a path to one or more locations to temporarily change to. If more than one path is specified, the Process ScriptBlock will be executed for each path.
        [Parameter(Mandatory = $true, ParameterSetName = 'Path', HelpMessage = 'Path to one or more locations.')]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        [string[]]$Path,

        # Specifies the locations(s) to temporarily change to. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences. If more than one path is specified, the Process ScriptBlock will be executed for each path.
        [Parameter(Mandatory = $true, ParameterSetName = 'LiteralPath', HelpMessage = 'Path to one or more locations.')]
        [Alias('PSPath', 'LP', 'FullName')]
        [ValidateNotNullOrEmpty()]
        [string[]]$LiteralPath
    )

    Process {
        $IsValidLocation = $true;
        $StackName = "Use-Location$([Guid]::NewGuid())";
        try { Push-Location -StackName $StackName -ErrorAction Stop }
        catch {
            Write-Error -ErrorRecord $_;
            $IsValidLocation = $false;
        }
        if ($IsValidLocation) {
            try {
                if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
                    foreach ($LP in $LiteralPath) {
                        $IsValidLocation = $true;
                        try { Set-Location -LiteralPath $LP -ErrorAction Stop }
                        catch {
                            Write-Error -ErrorRecord $_;
                            $IsValidLocation = $false;
                        }
                        if ($IsValidLocation) {
                            foreach ($ScriptBlock in $Process) { &$Process }
                        }
                    }
                } else {
                    foreach ($P in $Path) {
                        $IsValidLocation = $true;
                        try { Set-Location -Path $P -ErrorAction Stop }
                        catch {
                            Write-Error -ErrorRecord $_;
                            $IsValidLocation = $false;
                        }
                        if ($IsValidLocation) {
                            foreach ($ScriptBlock in $Process) { &$Process }
                        }
                    }
                }
            } finally {
                Pop-Location -StackName $StackName;
            }
        }
    }
}

Function Get-AppDataPath {
	<#
		.SYNOPSIS
			Get path for application data storage.

		.DESCRIPTION
			Constructs a path for application-specific data.

		.OUTPUTS
			System.String. Path to application data storage folder.
	#>
	[CmdletBinding(DefaultParameterSetName = 'Roaming')]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Name of company
		[string]$Company,

		[Parameter(Mandatory = $true, Position = 1)]
		# Name of application
		[string]$ProductName,

		[Parameter(Position = 2)]
		# Version of application
		[System.Version]$Version,

		[Parameter(Position = 3)]
		# Name of component
		[string]$ComponentName,

		# Create folder structure if it does not exist
		[switch]$Create,

		[Parameter(ParameterSetName = 'Roaming')]
		# Create folder structure under roaming profile.
		[switch]$Roaming,

		[Parameter(ParameterSetName = 'Local')]
		# Create folder structure under local profile.
		[switch]$Local,

		[Parameter(ParameterSetName = 'Common')]
		# Create folder structure under common location.
		[switch]$Common
	)

	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Common' { $AppDataPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::CommonApplicationData); break; }
			'Local' { $AppDataPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::LocalApplicationData); break; }
			default { $AppDataPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::ApplicationData); break; }
		}

		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			throw ('Unable to find {0} path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
		}

		$AppDataPath = $AppDataPath | Join-Path -ChildPath ($Company | ConvertTo-SafeFileName);

		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
			if (-not ($AppDataPath | Test-Path -PathType Container)) {
				throw ('Unable to create {0} company path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
			}
		}

		$N = $ProductName;
		if ($PSBoundParameters.ContainsKey('Version')) { $N = '{0}_{1}_{2}' -f $N, $Version.Major, $Version.Minor }
		$AppDataPath = $AppDataPath | Join-Path -ChildPath ($N | ConvertTo-SafeFileName);

		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
			if (-not ($AppDataPath | Test-Path -PathType Container)) {
				throw ('Unable to create {0} product path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
			}
		}

		if ($PSBoundParameters.ContainsKey('ComponentName')) {
			$AppDataPath = $AppDataPath | Join-Path -ChildPath ($ComponentName | ConvertTo-SafeFileName -AllowExtension);

			if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
				New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
				if (-not ($AppDataPath | Test-Path -PathType Container)) {
					throw ('Unable to create {0} component path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
				}
			}
		}

		$AppDataPath | Write-Output;
	}
}

Function Use-TempFolder {
    <#
    .SYNOPSIS
        Creates a temporary folder that is automatically deleted.
    .DESCRIPTION
        Creates a temporary folder and runs scripts, deleting the temp folder when complete.
    #>
    [CmdletBinding(DefaultParameterSetName = 'TempPathVar')]
    Param (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        # The script block(s) to run.
        [ScriptBlock[]]$Process,

        # The name of the variable containing the temp folder path (without the leading '$'). The default is _.
        [Parameter(ParameterSetName = 'TempPathVar')]
        [ValidatePattern('^[_a-z]\w*$')]
        [string]$TempPathVar = '_',

        [Parameter(ParameterSetName = 'TempPathVar')]
        [PSVariable[]]$ContextVariables,

        # The temp folder path variable will contain the DirectoryInfo object. If this is not specified, it will contain the full path string.
        [switch]$TempPathItem,

        [Parameter(Mandatory = $true, ParameterSetName = 'PassAsArg')]
        # Passes the folder path as the first argument.
        [switch]$PassAsArg
    )

    Begin {
        $TempPath = [System.IO.Path]::GetTempPath();
        $FolderName = [Guid]::NewGuid().ToString('n');
        $FullPath = $TempPath | Join-Path -ChildPath $FolderName;
        while ($FullPath | Test-Path) {
            $FolderName = [Guid]::NewGuid().ToString('n');
            $FullPath = $TempPath | Join-Path -ChildPath $FolderName;
        }
        $DirectoryInfo = New-Item -Path $TempPath -Name $FolderName -ItemType Directory -Force;
        $TempValue = $null;
        if ($TempPathItem.IsPresent) {
            $TempValue = $DirectoryInfo;
        } else {
            $TempValue = $DirectoryInfo.FullName;
        }
        [System.Collections.Generic.List[PSVariable]]$Variables = $null;
        if (-not $PassAsArg.IsPresent) {
            $Variables = [System.Collections.Generic.List[PSVariable]]::new();
            $Variables.Add([PSVariable]::new($TempPathVar, $TempValue, [System.Management.Automation.ScopedItemOptions]::ReadOnly));
            if ($PSBoundParameters.ContainsKey('ContextVariables')) {
                foreach ($v in $ContextVariables) {
                    if ($v.Name -ine $TempPathVar) { $Variables.Add($v) }
                }
            }
        }
    }

    Process {
        if ($PassAsArg.IsPresent) {
            foreach ($sb in $Process) {
                $sb.Invoke($TempValue);
            }
        } else {
            foreach ($sb in $Process) {
                $sb.InvokeWithContext($null, $Variables);
            }
        }
    }

    End {
        Remove-Item -LiteralPath $DirectoryInfo.FullName -Recurse -Force;
    }
}
