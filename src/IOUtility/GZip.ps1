Function Expand-GZip {
    <#
    .SYNOPSIS
        Decompresses GZip files.
    .DESCRIPTION
        Decompresses the specified GZip-compressed file(s).
    #>
    [CmdletBinding(DefaultParameterSetName = 'WcPath', SupportsShouldProcess = $true, ConfirmImpact = 'High')]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'WcPath')]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        [SupportsWildcards()]
        # Path to one or more locations of files to decrypt. The default behavior is to expand all .gz, and .tgz files in the current directory.
        [string[]]$Path,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'LiteralPath')]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
        [Alias("PSPath", "FullName")]
        # Literal path to one or more locations of files to decrypt.
        [string[]]$LiteralPath,

        # Literal path of output directory. If not specified, it will use the current subdirectory as the output path.
        [ValidateScript({ $_ | Test-Path -PathType Container })]
        [string]$OutputDirectory,

        [ValidateScript({ "/$_" | Test-Path -IsValid -and [string]::IsNullOrEmpty(($_ | Split-Path -Parent)) })]
        # Default output file extension if the input file doesn't have a compound extension.
        # If this is not specified, files without a compound extension will be given the '.data' extension.
        # Files with the extension '.tgz' are treated as though they have a '.tar.gz' compound extension.
        [string]$DefaultExt = '.data',

        # Force overwrite of existing files. Attempting to overwrite the current input file will result in an exception.
        [switch]$Force
    )

    Begin {
        $FileSystemProviderName = [Microsoft.PowerShell.Commands.FileSystemProvider]::ProviderName;
        $Ext = $DefaultExt;
        if (-not $Ext.StartsWith('.')) { $Ext = ".$Ext" }
        $OutputPathInfo = $null;
        if ($PSBoundParameters.ContainsKey('OutputDirectory')) {
            $OutputPathInfo = Resolve-Path -LiteralPath $OutputDirectory;
        } else {
            $OutputPathInfo = Get-Location;
        }
        if ($null -eq $OutputPathInfo) { break }

        function ExpandItemTo([string]$InputPath, [string]$OutputPath) {
            $InputStream = [System.IO.FileStream]::new($InputPath, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::Read);
            try {
                $OutputStream = $null;
                if ($null -ne ($OutputStream = [System.IO.FileStream]::new($OutputPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None))) {
                    try {
                        $GzipStream = [System.IO.Compression.GzipStream]::new($InputStream, [System.IO.Compression.CompressionMode]::Decompress);
                        try {
                            $Buffer = New-Object 'System.Byte[]' -ArgumentList 32768;
                            $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                            while ($Count -gt 0) {
                                $OutputStream.Write($Buffer, 0, $Count);
                                $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                            }
                        }
                        finally { $GzipStream.Close() }
                        $OutputStream.Flush();
                    }
                    finally { $OutputStream.Close() }
                }
            } finally {
                $InputStream.Close();
            }
        }

        if ($OutputPathInfo.Provider.Name -eq $FileSystemProviderName) {
            function ExpandItem([System.Management.Automation.PathInfo]$SourcePathInfo) {
                $FileName = $SourcePathInfo.Path | Split-Path -Leaf;
                $Extension = $FileName | Split-Path -Extension;
                $FileName = $FileName | Split-Path -LeafBase;
                if ([string]::IsNullOrEmpty($Extension)) {
                    if (-not $FileName.EndsWith('.')) { $FileName += $Ext }
                } else {
                    if ($Extension -ieq '.tgz') { $FileName += '.tar' }
                }
                $OutputPath = $OutputPathInfo.Path | Join-Path -ChildPath $FileName;
                $OkayToProcess = $false;
                if ($OutputPath | Test-Path -PathType Leaf) {
                    if ($Force.IsPresent) {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Overwrite');
                    } else {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) already exists" -Category ResourceExists -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    }
                } else {
                    if ($OutputPath | Test-Path) {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) is not a normal file" -Category InvalidOperation -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    } else {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Create');
                    }
                }
                if ($OkayToProcess) {
                    if ($SourcePathInfo.Provider.Name -eq $FileSystemProviderName) {
                        ExpandItemTo($SourcePathInfo.Path, $OutputPathInfo.Path | Join-Path $FileName);
                    } else {
                        $IntermediateSourcePath = [System.IO.Path]::GetTempFileName();
                        [System.IO.File.Delete]::($IntermediateSourcePath);
                        Copy-Item -LiteralPath $SourcePathInfo.Path -Destination $IntermediateSourcePath -Force;
                        if ($IntermediateSourcePath | Test-Path) {
                            try { ExpandItemTo($IntermediateSourcePath, $OutputPathInfo.Path | Join-Path $FileName) }
                            finally { [System.IO.File.Delete]::($IntermediateSourcePath) }
                        }
                    }
                }
            }
        } else {
            function ExpandItem([System.Management.Automation.PathInfo]$SourcePathInfo) {
                $FileName = $SourcePathInfo.Path | Split-Path -Leaf;
                $Extension = $FileName | Split-Path -Extension;
                $FileName = $FileName | Split-Path -LeafBase;
                if ([string]::IsNullOrEmpty($Extension)) {
                    if (-not $FileName.EndsWith('.')) { $FileName += $Ext }
                } else {
                    if ($Extension -ieq '.tgz') { $FileName += '.tar' }
                }
                $OutputPath = $OutputPathInfo.Path | Join-Path -ChildPath $FileName;
                $OkayToProcess = $false;
                if ($OutputPath | Test-Path -PathType Leaf) {
                    if ($Force.IsPresent) {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Overwrite');
                    } else {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) already exists" -Category ResourceExists -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    }
                } else {
                    if ($OutputPath | Test-Path) {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) is not a normal file" -Category InvalidOperation -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    } else {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Create');
                    }
                }
                if ($OkayToProcess) {
                    $IntermediateTargetPath = [System.IO.Path]::GetTempFileName();
                    [System.IO.File.Delete]::($IntermediateTargetPath);
                    if ($SourcePathInfo.Provider.Name -eq $FileSystemProviderName) {
                        ExpandItemTo($SourcePathInfo.Path, $OutputPathInfo.Path | Join-Path $FileName);
                    } else {
                        $IntermediateSourcePath = [System.IO.Path]::GetTempFileName();
                        [System.IO.File.Delete]::($IntermediateSourcePath);
                        Copy-Item -LiteralPath $SourcePathInfo.Path -Destination $IntermediateSourcePath -Force;
                        if ($IntermediateSourcePath | Test-Path) {
                            try { ExpandItemTo($IntermediateSourcePath, $OutputPathInfo.Path | Join-Path $FileName) }
                            finally { [System.IO.File.Delete]::($IntermediateSourcePath) }
                        }
                    }
                    if (Test-Path -LiteralPath $IntermediateTargetPath) {
                        try {
                            if ($Force.IsPresent) {
                                Copy-Item -LiteralPath $IntermediateTargetPath -Destination $OutputPath -Force;
                            } else {
                                Copy-Item -LiteralPath $IntermediateTargetPath -Destination $OutputPath;
                            }
                        }
                        finally { [System.IO.File.Delete]::($IntermediateTargetPath) }
                    }
                }
            }
        }
    }

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            foreach ($SourcePathInfo in (Resolve-Path -LiteralPath $LiteralPath)) {
                ExpandItem($SourcePathInfo);
            }
        } else {
            foreach ($SourcePathInfo in ((Resolve-Path -LiteralPath $LiteralPath) | Where-Object { $_.Path | Test-Path -PathType Leaf })) {
                ExpandItem($SourcePathInfo);
            }
        }
    }
}

Function Compress-GZip {
    <#
    .SYNOPSIS
        Compresses GZip files.
    .DESCRIPTION
        Compresses the specified file(s) as GZIP.
    #>
    [CmdletBinding(DefaultParameterSetName = 'WcPath', SupportsShouldProcess = $true, ConfirmImpact = 'High')]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'WcPath')]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        [SupportsWildcards()]
        # Path to one or more locations of files to decrypt. The default behavior is to expand all .gz, and .tgz files in the current directory.
        [string[]]$Path,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'LiteralPath')]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
        [Alias("PSPath", "FullName")]
        # Literal path to one or more locations of files to decrypt.
        [string[]]$LiteralPath,

        # Literal path of output directory. If not specified, it will use the current subdirectory as the output path.
        [ValidateScript({ $_ | Test-Path -PathType Container })]
        [string]$OutputDirectory,

        # Compress .tar files to .tar.gz instead of .tgz
        [switch]$NoTgzExt,

        # Force overwrite of existing files.
        [switch]$Force
    )

    Begin {
        $FileSystemProviderName = [Microsoft.PowerShell.Commands.FileSystemProvider]::ProviderName;
        $OutputPathInfo = $null;
        if ($PSBoundParameters.ContainsKey('OutputDirectory')) {
            $OutputPathInfo = Resolve-Path -LiteralPath $OutputDirectory;
        } else {
            $OutputPathInfo = Get-Location;
        }
        if ($null -eq $OutputPathInfo) { break }
        
        function CompressItemTo([string]$InputPath, [string]$OutputPath) {
            $InputStream = [System.IO.FileStream]::new($InputPath, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::Read);
            try {
                $OutputStream = $null;
                if ($null -ne ($OutputStream = [System.IO.FileStream]::new($OutputPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None))) {
                    try {
                        $GzipStream = [System.IO.Compression.GzipStream]::new($InputStream, [System.IO.Compression.CompressionMode]::Compress);
                        try {
                            $Buffer = New-Object 'System.Byte[]' -ArgumentList 32768;
                            $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                            while ($Count -gt 0) {
                                $OutputStream.Write($Buffer, 0, $Count);
                                $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                            }
                        }
                        finally { $GzipStream.Close() }
                        $OutputStream.Flush();
                    }
                    finally { $OutputStream.Close() }
                }
            } finally {
                $InputStream.Close();
            }
        }
        
        if ($NoTgzExt.IsPresent) {
            Function GetOutputFileName([string]$FileName) {
                if ($FileName.EndsWith('.')) { return "$($FileName).gz" }
                return "$FileName.gz";
            }
        } else {
            Function GetOutputFileName([string]$FileName) {
                if (($Leaf | Split-Path -Extension) -ieq '.tar') { return "$($FileName | Split-Path -LeafBase).gz" }
                if ($FileName.EndsWith('.')) { return "$($FileName).gz" }
                return "$FileName.gz";
            }
        }

        if ($OutputPathInfo.Provider.Name -eq $FileSystemProviderName) {
            function CompressItem([System.Management.Automation.PathInfo]$SourcePathInfo) {
                $OutputPath = $OutputPathInfo.Path | Join-Path -ChildPath (GetOutputFileName(($SourcePathInfo.Path | Split-Path -Leaf)));
                $OkayToProcess = $false;
                if ($OutputPath | Test-Path -PathType Leaf) {
                    if ($Force.IsPresent) {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Overwrite');
                    } else {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) already exists" -Category ResourceExists -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    }
                } else {
                    if ($OutputPath | Test-Path) {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) is not a normal file" -Category InvalidOperation -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    } else {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Create');
                    }
                }
                if ($OkayToProcess) {
                    if ($SourcePathInfo.Provider.Name -eq $FileSystemProviderName) {
                        CompressItemTo($SourcePathInfo.Path, $OutputPathInfo.Path | Join-Path $FileName);
                    } else {
                        $IntermediateSourcePath = [System.IO.Path]::GetTempFileName();
                        [System.IO.File.Delete]::($IntermediateSourcePath);
                        Copy-Item -LiteralPath $SourcePathInfo.Path -Destination $IntermediateSourcePath -Force;
                        if ($IntermediateSourcePath | Test-Path) {
                            try { CompressItemTo($IntermediateSourcePath, $OutputPathInfo.Path | Join-Path $FileName) }
                            finally { [System.IO.File.Delete]::($IntermediateSourcePath) }
                        }
                    }
                }
            }
        } else {
            function CompressItem([System.Management.Automation.PathInfo]$SourcePathInfo) {
                $OutputPath = $OutputPathInfo.Path | Join-Path -ChildPath (GetOutputFileName(($SourcePathInfo.Path | Split-Path -Leaf)));
                $OkayToProcess = $false;
                if ($OutputPath | Test-Path -PathType Leaf) {
                    if ($Force.IsPresent) {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Overwrite');
                    } else {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) already exists" -Category ResourceExists -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    }
                } else {
                    if ($OutputPath | Test-Path) {
                        Write-Error -Message "$($OutputPath | Resolve-Path -Relative) is not a normal file" -Category InvalidOperation -ErrorId 'OutputFileExists' -TargetObject $OutputPath;
                    } else {
                        $OkayToProcess = $PSCmdlet.ShouldProcess(($OutputPath | Resolve-Path -Relative), 'Create');
                    }
                }
                if ($OkayToProcess) {
                    $IntermediateTargetPath = [System.IO.Path]::GetTempFileName();
                    [System.IO.File.Delete]::($IntermediateTargetPath);
                    if ($SourcePathInfo.Provider.Name -eq $FileSystemProviderName) {
                        CompressItemTo($SourcePathInfo.Path, $OutputPathInfo.Path | Join-Path $FileName);
                    } else {
                        $IntermediateSourcePath = [System.IO.Path]::GetTempFileName();
                        [System.IO.File.Delete]::($IntermediateSourcePath);
                        Copy-Item -LiteralPath $SourcePathInfo.Path -Destination $IntermediateSourcePath -Force;
                        if ($IntermediateSourcePath | Test-Path) {
                            try { CompressItemTo($IntermediateSourcePath, $OutputPathInfo.Path | Join-Path $FileName) }
                            finally { [System.IO.File.Delete]::($IntermediateSourcePath) }
                        }
                    }
                    if (Test-Path -LiteralPath $IntermediateTargetPath) {
                        try {
                            if ($Force.IsPresent) {
                                Copy-Item -LiteralPath $IntermediateTargetPath -Destination $OutputPath -Force;
                            } else {
                                Copy-Item -LiteralPath $IntermediateTargetPath -Destination $OutputPath;
                            }
                        }
                        finally { [System.IO.File.Delete]::($IntermediateTargetPath) }
                    }
                }
            }
        }
    }
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            foreach ($SourcePathInfo in (Resolve-Path -LiteralPath $LiteralPath)) {
                CompressItem($SourcePathInfo);
            }
        } else {
            foreach ($SourcePathInfo in ((Resolve-Path -LiteralPath $LiteralPath) | Where-Object { $_.Path | Test-Path -PathType Leaf })) {
                CompressItem($SourcePathInfo);
            }
        }
    }
}
