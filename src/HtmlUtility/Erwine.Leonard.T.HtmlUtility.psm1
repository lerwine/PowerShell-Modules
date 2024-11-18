# . $PSScriptRoot/Offline-Html.ps1

class ResourceRef {
    [string]$RelativePath;
    [Uri[]]$OriginalUri;

    static [ResourceRef] FromJson([object]$Obj) {
        if ($null -eq $Obj) { return $null }
        if ($Obj.RelativePath -isnot [string] -or $Obj.RelativePath.Trim().Length -eq 0) {
            throw 'Invalid RelativePath';
        }
        [Uri]$Uri = $null;
        return [ResourceRef]@{
            RelativePath = $Obj.RelativePath;
            OriginalUri = ([Uri[]]@(@($Obj.OriginalUri) | ForEach-Object {
                if ($_ -is [string] -and [Uri]::TryCreate($_, [System.UriKind]::Absolute, [ref]$Uri)) {
                    $Uri | Write-Output;
                } else {
                    throw "$_ is not an absolute URI";
                }
            }))
        };
    }
}
class HtmlImportRepository {
    [string]$RootPath;
    [System.Collections.ObjectModel.Collection[ResourceRef]]$Lib = [System.Collections.ObjectModel.Collection[ResourceRef]]::new();
    [System.Collections.ObjectModel.Collection[ResourceRef]]$Images = [System.Collections.ObjectModel.Collection[ResourceRef]]::new();
}

Function Open-HtmlImportRepository {
    [CmdletBinding()]
    [OutputType([HtmlImportRepository])]
    Param(
        # Specifies a path to one or more locations. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $true)]
        [Alias('LiteralPath', 'LP', 'FullName', 'PSPath')]
        [string]$Path,

        [switch]$Force
    )

    Process {
        if (Test-Path -LiteralPath $Path -PathType Container) {
            try {
                if ($Force.IsPresent) {
                    $Local:Item = Get-Item -LiteralPath $Path -ErrorAction Stop -Force;
                } else {
                    $Local:Item = Get-Item -LiteralPath $Path -ErrorAction Stop;
                }
            } catch {
                if ([string]::IsNullOrWhiteSpace($_.Exception.Message)) {
                    Write-Error -Exception $_.Exception -Message "Error accessing $($Path | ConvertTo-Json)" -Category $_.CategoryInfo.Category -ErrorId 'RepoDirAccessError' -TargetObject $Path;
                } else {
                    Write-Error -Exception $_.Exception -Message "Error accessing $($Path | ConvertTo-Json): $($_.Exception.Message)" -Category $_.CategoryInfo.Category -ErrorId 'RepoDirAccessError' -TargetObject $Path;
                }
                $Local:Item = $null;
            }
            if ($null -ne $Local:Item) {
                $Local:P = $Local:Item | Join-Path -ChildPath 'index.json';
                if (Test-Path -LiteralPath $P -PathType Leaf) {
                    try {
                        if ($Force.IsPresent) {
                            $Local:Content = Get-Content -LiteralPath $P -ErrorAction Stop -Force;
                        } else {
                            $Local:Content = Get-Content -LiteralPath $P -ErrorAction Stop;
                        }
                    } catch {
                        if ([string]::IsNullOrWhiteSpace($_.Exception.Message)) {
                            Write-Error -Exception $_.Exception -Message "Error accessing $($P | ConvertTo-Json)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexAccessError' -TargetObject $P;
                        } else {
                            Write-Error -Exception $_.Exception -Message "Error accessing $($P | ConvertTo-Json): $($_.Exception.Message)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexAccessError' -TargetObject $P;
                        }
                        $Local:Content = $null;
                    }
                    if ($null -ne $Local:Content) {
                        try { $Local:JsonData = $Local:Content | ConvertFrom-Json }
                        catch {
                            if ([string]::IsNullOrWhiteSpace($_.Exception.Message)) {
                                Write-Error -Exception $_.Exception -Message "Error parsing JSON from $($P | ConvertTo-Json)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexParseError' -TargetObject $P;
                            } else {
                                Write-Error -Exception $_.Exception -Message "Error parsing JSON from $($P | ConvertTo-Json): $($_.Exception.Message)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexParseError' -TargetObject $P;
                            }
                            $Local:JsonData = $null;
                        }
                        if ($null -ne $Local:JsonData) {
                            if ($Local:JsonData.Lib -isnot [Array]) {
                                Write-Error -Message "Lib property is not an array in $($P | ConvertTo-Json)" -Category InvalidData -ErrorId 'InvalidIndexLibProperty' -TargetObject $P;
                                $Local:JsonData = $null;
                            } else {
                                if ($Local:JsonData.Images -isnot [Array]) {
                                    Write-Error -Message "Images property is not an array in $($P | ConvertTo-Json)" -Category InvalidData -ErrorId 'InvalidIndexImagesProperty' -TargetObject $P;
                                    $Local:JsonData = $null;
                                } else {
                                    if ($Local:JsonData.HTML -isnot [Array]) {
                                        Write-Error -Message "HTML property is not an array in $($P | ConvertTo-Json)" -Category InvalidData -ErrorId 'InvalidIndexHTMLProperty' -TargetObject $P;
                                        $Local:JsonData = $null;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if (Test-Path -LiteralPath $P) {
                        Write-Error -Message "Path of index file is a subdirectory: $($P | ConvertTo-Json)" -Category InvalidOperation -ErrorId 'RepoIndexNotLeaf' -TargetObject $P;
                        $Local:Content = $null;
                    } else {
                        $Local:JsonData = [PSCustomObject]@{
                            Lib = ([object[]]@());
                            Images = ([object[]]@());
                            HTML = ([object[]]@());
                        };
                    }
                }
                if ($null -ne $Local:JsonData) {
                    if ($Local:Item.FullName -is [string] -and $Local:Item.FullName.Length -gt 0) {
                        $Local:HtmlImportRepository = [HtmlImportRepository]@{ RootPath = $Local:Item.FullName }
                    } else {
                        $Local:HtmlImportRepository = [HtmlImportRepository]@{ RootPath = $Local:Item.PSPath }
                    }
                    foreach ($obj in $Local:JsonData.Lib) {
                        if ($null -eq $obj) {
                            Write-Error -Message "Lib property contains a null value in $($P | ConvertTo-Json)" -Category InvalidData -ErrorId 'RepoIndexLibNull' -TargetObject $P;
                            $Local:HtmlImportRepository = $null;
                            break;
                        }
                        try {  $Local:HtmlImportRepository.Lib.Add([ResourceRef]::FromJson($obj)) }
                        catch {
                            if ([string]::IsNullOrWhiteSpace($_.Exception.Message)) {
                                Write-Error -Exception $_.Exception -Message "Error parsing Lib element $($obj | ConvertTo-Json) from $($P | ConvertTo-Json)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexParseError' -TargetObject $P;
                            } else {
                                Write-Error -Exception $_.Exception -Message "Error parsing Lib element $($obj | ConvertTo-Json) from $($P | ConvertTo-Json): $($_.Exception.Message)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexParseError' -TargetObject $P;
                            }
                            $Local:HtmlImportRepository = $null;
                        }
                        if ($null -eq $Local:HtmlImportRepository) { break }
                    }
                    if ($null -ne $Local:HtmlImportRepository) {
                        foreach ($obj in $Local:JsonData.Images) {
                            if ($null -eq $obj) {
                                Write-Error -Message "Images property contains a null value in $($P | ConvertTo-Json)" -Category InvalidData -ErrorId 'RepoIndexImageNull' -TargetObject $P;
                                $Local:HtmlImportRepository = $null;
                                break;
                            }
                            try {  $Local:HtmlImportRepository.Lib.Add([ResourceRef]::FromJson($obj)) }
                            catch {
                                if ([string]::IsNullOrWhiteSpace($_.Exception.Message)) {
                                    Write-Error -Exception $_.Exception -Message "Error parsing Images element $($obj | ConvertTo-Json) from $($P | ConvertTo-Json)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexParseError' -TargetObject $P;
                                } else {
                                    Write-Error -Exception $_.Exception -Message "Error parsing Images element $($obj | ConvertTo-Json) from $($P | ConvertTo-Json): $($_.Exception.Message)" -Category $_.CategoryInfo.Category -ErrorId 'RepoIndexParseError' -TargetObject $P;
                                }
                                $Local:HtmlImportRepository = $null;
                            }
                            if ($null -eq $Local:HtmlImportRepository) { break }
                        }
                    }
                    if ($null -ne $Local:HtmlImportRepository) {
                        # TODO: Parse in HTML property
                    }
                }
            }
        } else {
            if (Test-Path -LiteralPath $Path) {
                Write-Error -Message "Path does not refer to a subdirectory: $($Path | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'RepoPathNotContainer' -TargetObject $Path;
            } else {
                if (Test-Path -LiteralPath $Path -IsValid) {
                    Write-Error -Message "Path not found: $($Path | ConvertTo-Json)" -Category ObjectNotFound -ErrorId 'RepoPathNotFound' -TargetObject $Path;
                } else {
                    Write-Error -Message "Path is not valid: $($Path | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'RepoPathInvalid' -TargetObject $Path;
                }
            }
        }
    }
}

Function Publish-OfflineHtml {
    [CmdletBinding(DefaultParameterSetName = "Wcpath")]
    Param(
        # Specifies a path to one or more locations. Wildcards are permitted.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "Wcpath", ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, HelpMessage = "Path to one or more locations.")]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        [string[]]$Path,
        
        # Specifies a path to one or more locations. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true,  ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = $true, HelpMessage = "Literal path to one or more locations.")]
        [Alias("PSPath", "FullName")]
        [ValidateNotNullOrEmpty()]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
        [string[]]$LiteralPath,

        [Parameter(Mandatory = $true, HelpMessage = "Target directory to publish to.")]
        [ValidateScript({
            if (Test-Path -LiteralPath $_ -PathType Container) {
                if (Test-Path -LiteralPath ($_ | Join-Path -ChildPath 'lib') -PathType Leaf) { throw 'lib must be a sub-folder if it exists' }
                if (Test-Path -LiteralPath ($_ | Join-Path -ChildPath 'img') -PathType Leaf) { throw 'img must be a sub-folder if it exists' }
            }
        })]
        [string]$TargetDirectory,

        [Parameter(HelpMessage = "Relative output path")]
        [ValidateScript({ $_ | Test-Path -IsValid })]
        [string]$RelativePath
    )

    Begin {
        $RootFolder = $null;
        $TargetItem = $null;
        try { $RootFolder = Get-Item -LiteralPath $TargetDirectory -ErrorAction Stop -Force }
        catch {
            Write-Error -ErrorRecord $_;
            break;
        }
        if ($PSBoundParameters.ContainsKey($RelativePath)) {
            $TargetItem = Get-Item -LiteralPath ($RootFolder.FullName | Join-Path -ChildPath $RelativePath) -Force;
            if (-not $TargetItem.PSIsContainer) {
                Write-Error -Message "Relative path does not refer to a subdirectory" -Category ObjectNotFound -ErrorId 'InvalidRelativePath' -TargetObject $RelativePath;
                break;
            }
        } else {
            $TargetItem = $RootFolder;
        }
        $ImgFolderPath = $RootFolder.FullName | Join-Path -ChildPath 'lib';
        if (-not ($ImgFolderPath | Test-Path -PathType Container)) {
            if ($ImgFolderPath | Test-Path) { Remove-Item -LiteralPath $ImgFolderPath -Force }
            (New-Item -ItemType Directory -Path $RootFolder.FullName -Name 'lib') | Out-Null;
        }
        $ImgMapPath = $RootFolder.FullName | Join-Path -ChildPath 'libmap.json';
        $ImgMapHash = @{};
        if ($ImgMapPath | Test-Path -PathType Leaf) {
            $ImgMapHash = Import-Clixml -LiteralPath $ImgMapPath;
        } else {
            if ($ImgMapPath | Test-Path) { Remove-Item -LiteralPath $ImgMapPath -Recurse -Force }
        }
        $LibFolderPath = $RootFolder.FullName | Join-Path -ChildPath 'lib';
        if (-not ($LibFolderPath | Test-Path -PathType Container)) {
            if ($LibFolderPath | Test-Path) { Remove-Item -LiteralPath $LibFolderPath -Force }
            (New-Item -ItemType Directory -Path $RootFolder.FullName -Name 'lib') | Out-Null;
        }
        ('from-md.css', 'highlight.css', 'katex-copytex.min.css', 'katex-copytex.min.js', 'katex.min.css', 'markdown.css', 'vscode-github.css') | ForEach-Object {
            $CssPath = $LibFolderPath | Join-Path -ChildPath $_;
            if (-not ($CssPath | Test-Path -PathType Leaf)) {
                if ($CssPath | Test-Path) { Remove-Item -LiteralPath $CssPath -Recurse -Force }
                Copy-Item -LiteralPath ($PSScriptRoot | Join-Path -ChildPath $_) -Destination $CssPath -Force;
            }
        }
    }

    Process {
        [System.IO.FileInfo[]]$SourceItems = $null;
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            $SourceItems = @($LiteralPath | ForEach-Object { Get-Item -LiteralPath $_ -Force });
        } else {
            $SourceItems = @(($Path | Get-Item -Force) | Where-Object { -not $_.PSIsContainer });
        }
        foreach ($FileInfo in $SourceItems) {
            $DestinationPath = $TargetItem.FullName | Join-Path -ChildPath $FileInfo.Name;
            if ($DestinationPath | Test-Path -PathType Container) { Remove-Item -LiteralPath $DestinationPath -Recurse -Force }
            $HtmlDocument = $null;
            if ($FileInfo.Extension -ieq '.md') {
               $HtmlDocument = @"
<!DOCTYPE html>
<html lang="en">
    <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Create VM</title>
    <link rel="stylesheet" href="lib/katex.min.css" />
    <link href="lib/katex-copytex.min.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="lib/markdown.css" />
    <link rel="stylesheet" href="lib/highlight.css" />
    <link rel="stylesheet" href="lib/from-md.css" />
    </head>
    <body class="vscode-body vscode-light">
    $((ConvertFrom-Markdown -LiteralPath $FileInfo.FullName).Html)
    <script async src="lib/katex-copytex.min.js"></script>
    </body>
</html>
"@ | ConvertFrom-HtmlString;
            } else {
                if ($FileInfo.Extension -ieq '.html' -or $FileInfo.Extension -ieq '.htm') {
                    $HtmlDocument = (Get-Content -LiteralPath $FileInfo.FullName -Force) | ConvertFrom-HtmlString;
                }
            }
            if ($null -eq $HtmlDocument) {
                Copy-Item -LiteralPath $FileInfo.FullName -Destination $DestinationPath -Force;
            } else {
            }
        }
    }
}