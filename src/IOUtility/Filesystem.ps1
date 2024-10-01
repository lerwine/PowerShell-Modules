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

class OptionalPathInfo {
    [System.Management.Automation.PathInfo]$Resolved;
    [string]$Unresolved;
    [string]$Path;
}

Function Resolve-OptionalPath {
    [CmdletBinding(DefaultParameterSetName = 'WcPath')]
    Param(
        # Specifies a path to one or more locations.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "WcPath", ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [ValidateNotNullOrEmpty()]
        [string[]]$Path,

        # Specifies a path to one or more locations. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = $true)]
        [Alias("PSPath", "FullName")]
        [ValidateNotNullOrEmpty()]
        [string[]]$LiteralPath,

        [string]$RelativeBasePath,

        [switch]$Relative
    )

    Begin {
        $BasePathInfo = $null;
        if (-not $Relative.IsPresent) {
            if ($PSBoundParameters.ContainsKey('RelativeBasePath')) {
                $BasePathInfo = Resolve-Path -LiteralPath $RelativeBasePath;
                if ($null -eq $RelativeBasePath) { break }
            } else {
                $BasePathInfo = Get-Location;
            }
        }
    }
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            if ($Relative.IsPresent) {
                if ($PSBoundParameters.ContainsKey('RelativeBasePath')) {
                    foreach ($PathString in $LiteralPath) {
                        $Resolved = $null;
                        $Resolved = Resolve-Path -LiteralPath $PathString -RelativeBasePath $RelativeBasePath -Relative -ErrorAction Ignore;
                        if ($null -ne $Resolved) {
                            $Resolved | Write-Output;
                        } else {
                            $Unresolved = $PathString | Split-Path -Leaf;
                            $Parent = $PathString | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Resolved = Resolve-Path -LiteralPath $Parent -RelativeBasePath $RelativeBasePath -Relative -ErrorAction Ignore;
                                if ($null -ne $Resolved) { break }
                                $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            if ($null -ne $Resolved) {
                                ($Resolved | Join-Path -ChildPath $Unresolved) | Write-Output;
                            } else {
                                if ($Unresolved[0] -eq '.') {
                                    $Unresolved | Write-Output;
                                } else {
                                    ('.' | Join-Path -ChildPath $Unresolved) | Write-Output;
                                }
                            }
                        }
                    }
                } else {
                    foreach ($PathString in $LiteralPath) {
                        $Resolved = $null;
                        $Resolved = Resolve-Path -LiteralPath $PathString -Relative -ErrorAction Ignore;
                        if ($null -ne $Resolved) {
                            $Resolved | Write-Output;
                        } else {
                            $Unresolved = $PathString | Split-Path -Leaf;
                            $Parent = $PathString | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Resolved = Resolve-Path -LiteralPath $Parent -Relative -ErrorAction Ignore;
                                if ($null -ne $Resolved) { break }
                                $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            if ($null -ne $Resolved) {
                                ($Resolved | Join-Path -ChildPath $Unresolved) | Write-Output;
                            } else {
                                if ($Unresolved[0] -eq '.') {
                                    $Unresolved | Write-Output;
                                } else {
                                    ('.' | Join-Path -ChildPath $Unresolved) | Write-Output;
                                }
                            }
                        }
                    }
                }
            } else {
                if ($PSBoundParameters.ContainsKey('RelativeBasePath')) {
                    foreach ($PathString in $LiteralPath) {
                        $Resolved = $null;
                        $Resolved = Resolve-Path -LiteralPath $PathString -RelativeBasePath $RelativeBasePath -ErrorAction Ignore;
                        if ($null -ne $Resolved) {
                            [OptionalPathInfo]@{
                                Resolved = $Resolved;
                                Unresolved = '';
                                Path = $Resolved.Path;
                            } | Write-Output;
                        } else {
                            $Unresolved = $PathString | Split-Path -Leaf;
                            $Parent = $PathString | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Resolved = Resolve-Path -LiteralPath $Parent -RelativeBasePath $RelativeBasePath -ErrorAction Ignore;
                                if ($null -ne $Resolved) { break }
                                $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            if ($null -eq $Resolved) { $Resolved = $BasePathInfo }
                            [OptionalPathInfo]@{
                                Resolved = $Resolved;
                                Unresolved = $Unresolved;
                                Path = $Resolved.Path | Join-Path -ChildPath $Unresolved;
                            } | Write-Output;
                        }
                    }
                } else {
                    foreach ($PathString in $LiteralPath) {
                        $Resolved = $null;
                        $Resolved = Resolve-Path -LiteralPath $PathString -ErrorAction Ignore;
                        if ($null -ne $Resolved) {
                            [OptionalPathInfo]@{
                                Resolved = $Resolved;
                                Unresolved = '';
                                Path = $Resolved.Path;
                            } | Write-Output;
                        } else {
                            $Unresolved = $PathString | Split-Path -Leaf;
                            $Parent = $PathString | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Resolved = Resolve-Path -LiteralPath $Parent -ErrorAction Ignore;
                                if ($null -ne $Resolved) { break }
                                $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            if ($null -eq $Resolved) { $Resolved = $BasePathInfo }
                            [OptionalPathInfo]@{
                                Resolved = $Resolved;
                                Unresolved = $Unresolved;
                                Path = $Resolved.Path | Join-Path -ChildPath $Unresolved;
                            } | Write-Output;
                        }
                    }
                }
            }
        } else {
            if ($Relative.IsPresent) {
                if ($PSBoundParameters.ContainsKey('RelativeBasePath')) {
                    foreach ($P in $Path) {
                        $Resolved = @(Resolve-Path -LiteralPath $PathString -RelativeBasePath $RelativeBasePath -Relative -ErrorAction Ignore);
                        if ($Resolved.Count) {
                            $Resolved | Write-Output;
                        } else {
                            $Unresolved = $PathString | Split-Path -Leaf;
                            $Parent = $PathString | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Resolved = @(Resolve-Path -LiteralPath $Parent -RelativeBasePath $RelativeBasePath -Relative -ErrorAction Ignore);
                                if ($Resolved.Count -gt 0) { break }
                                $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            if ($Resolved.Count -gt 0) {
                                ($Resolved | Join-Path -ChildPath $Unresolved) | Write-Output;
                            } else {
                                if ($Unresolved[0] -eq '.') {
                                    $Unresolved | Write-Output;
                                } else {
                                    ('.' | Join-Path -ChildPath $Unresolved) | Write-Output;
                                }
                            }
                        }
                    }
                } else {
                    foreach ($P in $Path) {
                        $Resolved = @(Resolve-Path -LiteralPath $PathString -Relative -ErrorAction Ignore);
                        if ($Resolved.Count) {
                            $Resolved | Write-Output;
                        } else {
                            $Unresolved = $PathString | Split-Path -Leaf;
                            $Parent = $PathString | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Resolved = @(Resolve-Path -LiteralPath $Parent -Relative -ErrorAction Ignore);
                                if ($Resolved.Count -gt 0) { break }
                                $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            if ($Resolved.Count -gt 0) {
                                ($Resolved | Join-Path -ChildPath $Unresolved) | Write-Output;
                            } else {
                                if ($Unresolved[0] -eq '.') {
                                    $Unresolved | Write-Output;
                                } else {
                                    ('.' | Join-Path -ChildPath $Unresolved) | Write-Output;
                                }
                            }
                        }
                    }
                }
            } else {
                if ($PSBoundParameters.ContainsKey('RelativeBasePath')) {
                    foreach ($P in $Path) {
                        foreach ($PathString in $LiteralPath) {
                            $Resolved = @(Resolve-Path -LiteralPath $PathString -RelativeBasePath $RelativeBasePath -ErrorAction Ignore);
                            if ($Resolved.Count -gt 0) {
                                $Resolved | ForEach-Object {
                                    [OptionalPathInfo]@{
                                        Resolved = $_;
                                        Unresolved = '';
                                        Path = $_.Path;
                                    } | Write-Output;
                                }
                            } else {
                                $Unresolved = $PathString | Split-Path -Leaf;
                                $Parent = $PathString | Split-Path -Parent;
                                while (-not [string]::IsNullOrEmpty($Parent)) {
                                    $Resolved = @(Resolve-Path -LiteralPath $Parent -RelativeBasePath $RelativeBasePath -ErrorAction Ignore);
                                    if ($Resolved.Count -gt 0) { break }
                                    $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                    $Parent = $Parent | Split-Path -Parent;
                                }
                                if ($Resolved.Count -eq 0) {
                                    [OptionalPathInfo]@{
                                        Resolved = $BasePathInfo;
                                        Unresolved = $Unresolved;
                                        Path = $BasePathInfo.Path | Join-Path -ChildPath $Unresolved;
                                    } | Write-Output;
                                } else {
                                    $Resolved | ForEach-Object {
                                        [OptionalPathInfo]@{
                                            Resolved = $_;
                                            Unresolved = $Unresolved;
                                            Path = $_.Path | Join-Path -ChildPath $Unresolved;
                                        } | Write-Output;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    foreach ($P in $Path) {
                        foreach ($PathString in $LiteralPath) {
                            $Resolved = @(Resolve-Path -LiteralPath $PathString -ErrorAction Ignore);
                            if ($Resolved.Count -gt 0) {
                                $Resolved | ForEach-Object {
                                    [OptionalPathInfo]@{
                                        Resolved = $_;
                                        Unresolved = '';
                                        Path = $_.Path;
                                    } | Write-Output;
                                }
                            } else {
                                $Unresolved = $PathString | Split-Path -Leaf;
                                $Parent = $PathString | Split-Path -Parent;
                                while (-not [string]::IsNullOrEmpty($Parent)) {
                                    $Resolved = @(Resolve-Path -LiteralPath $Parent -ErrorAction Ignore);
                                    if ($Resolved.Count -gt 0) { break }
                                    $Unresolved = ($Parent | Split-Path -Leaf) | Join-Path -ChildPath $Unresolved;
                                    $Parent = $Parent | Split-Path -Parent;
                                }
                                if ($Resolved.Count -eq 0) {
                                    [OptionalPathInfo]@{
                                        Resolved = $BasePathInfo;
                                        Unresolved = $Unresolved;
                                        Path = $BasePathInfo.Path | Join-Path -ChildPath $Unresolved;
                                    } | Write-Output;
                                } else {
                                    $Resolved | ForEach-Object {
                                        [OptionalPathInfo]@{
                                            Resolved = $_;
                                            Unresolved = $Unresolved;
                                            Path = $_.Path | Join-Path -ChildPath $Unresolved;
                                        } | Write-Output;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

Function Get-PathStringSegments {
    <#
    .SYNOPSIS
        Gets path segments.
    .DESCRIPTION
        Gets names of path segments, normalizing '.' and '..' names where possible.
    .OUTPUTS
        System.String[]. Names of each path segment.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    Param(
        # Specifies a path to one or more locations. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias("PSPath", 'FullName')]
        [ValidateNotNullOrEmpty()]
        [string[]]$Path,

        # Do not attempt to resolve path or parent path.
        [switch]$NoResolve
    )

    Process {
        $Segments = [System.Collections.Generic.LinkedList[string]]::new();
        if ($NoResolve.IsPresent) {
            foreach ($PathString in $Path) {
                $Parent = $PathString | Split-Path -Parent;
                $Segments.AddFirst(($PathString | Split-Path -Leaf));
                while (-not [string]::IsNullOrEmpty($Parent)) {
                    $Segments.AddFirst(($Parent | Split-Path -Leaf));
                    $Parent = $Parent | Split-Path -Parent;
                }
            }
        } else {
            foreach ($PathString in $Path) {
                if ($PathString | Test-Path) {
                    $Parent = (Resolve-Path -LiteralPath $PathString).Path;
                    $Segments.AddFirst(($Parent | Split-Path -Leaf));
                    $Parent = $Parent | Split-Path -Parent;
                    while (-not [string]::IsNullOrEmpty($Parent)) {
                        $Segments.AddFirst(($Parent | Split-Path -Leaf));
                        $Parent = $Parent | Split-Path -Parent;
                    }
                    break;
                } else {
                    $Parent = $PathString | Split-Path -Parent;
                    $Segments.AddFirst(($PathString | Split-Path -Leaf));
                    while (-not [string]::IsNullOrEmpty($Parent)) {
                        if (Test-Path -LiteralPath $Parent) {
                            $Parent = (Resolve-Path -LiteralPath $Parent).Path;
                            $Segments.AddFirst(($Parent | Split-Path -Leaf));
                            $Parent = $Parent | Split-Path -Parent;
                            while (-not [string]::IsNullOrEmpty($Parent)) {
                                $Segments.AddFirst(($Parent | Split-Path -Leaf));
                                $Parent = $Parent | Split-Path -Parent;
                            }
                            break;
                        }
                        $Segments.AddFirst(($Parent | Split-Path -Leaf));
                        $Parent = $Parent | Split-Path -Parent;
                    }
                }
            }
        }
        [System.Collections.Generic.LinkedListNode[string]]$Node = $Segments.First;
        do {
            if ($Node.Value -eq '.') {
                $Next = $Node.Next;
                $Segments.Remove($Node) | Out-Null;
                $Node = $Next;
            } else {
                if ($Node.Value -eq '..') {
                    $Next = $Node.Next;
                    if ($null -ne $Node.Previous -and $Node.Previous -ne '..') {
                        $Segments.Remove($Node.Previous) | Out-Null;
                        $Segments.Remove($Node) | Out-Null;
                    }
                } else {
                    $Node = $Node.Next;
                }
            }
        } while ($null -ne $Node);
        $Segments | Write-Output;
    }
}

Function Optimize-PathString {
    <#
    .SYNOPSIS
        Gets normalized path strings.
    .DESCRIPTION
        Gets path string with normalized path separator characters, normalizing '.' and '..' names where possible.
    .OUTPUTS
        System.String. Normalized path string.
    #>
    [CmdletBinding()]
    Param(
        # Specifies a path to one or more locations. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias("PSPath", 'FullName', 'Path')]
        [ValidateNotNullOrEmpty()]
        [string[]]$Path,

        # Do not attempt to resolve path or parent path.
        [switch]$NoResolve
    )

    Process {
        if ($NoResolve.IsPresent) {
            foreach ($PathString in $Path) {
                ([string]$Result, [string[]]$RemainingSegments) = @(Get-PathStringSegments -Path $CompareTo -NoResolve);
                if ($null -ne $RemainingSegments) {
                    $RemainingSegments | ForEach-Object { $Result = $Result | Join-Path -ChildPath $_ }
                }
                $Result | Write-Output;
            }
        } else {
            foreach ($PathString in $Path) {
                ([string]$Result, [string[]]$RemainingSegments) = @(Get-PathStringSegments -Path $CompareTo);
                if ($null -ne $RemainingSegments) {
                    $RemainingSegments | ForEach-Object { $Result = $Result | Join-Path -ChildPath $_ }
                }
                $Result | Write-Output;
            }
        }
    }
}

Function Compare-PathStrings {
    <#
    .SYNOPSIS
        Compares 2 path strings.
    .DESCRIPTION
        Compares each segment of 2 path strings, normalizing '.' and '..' names where possible.
    .OUTPUTS
        System.Int32. < 0 if Path is less than CompareTo; > 0 if Path is greater than CompareTo; Otherwise, 0 if both are equal.
    #>
    [CmdletBinding()]
    Param(
        # Specifies a path to one or more locations. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias("PSPath", 'FullName', 'Path')]
        [ValidateNotNullOrEmpty()]
        [string[]]$Path,

        # Specifies a path to compare to. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$CompareTo,

        [Parameter(Position = 2)]
        # The comparison type to use for each path segment
        [System.StringComparison]$Type = [System.StringComparison]::CurrentCulture,

        # Do not attempt to resolve path or parent path.
        [switch]$NoResolve
    )

    Begin {
        [string[]]$CompareSegments = $null;
        if ($NoResolve.IsPresent) {
            [string[]]$CompareSegments = @(Get-PathStringSegments -Path $CompareTo -NoResolve);
        } else {
            [string[]]$CompareSegments = @(Get-PathStringSegments -Path $CompareTo);
        }
        $CL = $CompareSegments.Length;
    }

    Process {
        if ($NoResolve.IsPresent) {
            if ($CaseInsensitive.IsPresent) {}
            foreach ($PathString in $Path) {
                [string[]]$Segments = @(Get-PathStringSegments -Path $PathString -NoResolve);
                $Result = 0;
                for ($i = 0; $i -lt $CL; $i++) {
                    if ($i -eq $Segments.Length) {
                        $Result = -1;
                        break;
                    }
                    $Result = $Segments[$i].CompareTo($CompareSegments[$i], $Type);
                    if ($Result -ne 0) { break }
                }
                if ($Result -ne 0 -or $Segments.Length -eq $CL) {
                    $Result | Write-Output;
                } else {
                    1 | Write-Output;
                }
            }
        } else {
            foreach ($PathString in $Path) {
                [string[]]$Segments = @(Get-PathStringSegments -Path $PathString);
                $Result = 0;
                for ($i = 0; $i -lt $CL; $i++) {
                    if ($i -eq $Segments.Length) {
                        $Result = -1;
                        break;
                    }
                    $Result = $Segments[$i].CompareTo($CompareSegments[$i], $Type);
                    if ($Result -ne 0) { break }
                }
                if ($Result -ne 0 -or $Segments.Length -eq $CL) {
                    $Result | Write-Output;
                } else {
                    1 | Write-Output;
                }
            }
        }
    }
}
