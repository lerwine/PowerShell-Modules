# . $PSScriptRoot/Offline-Html.ps1

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
        $ResourcesPath = $PSScriptRoot | Join-Path -ChildPath 'Resources';
        ('from-md.css', 'highlight.css', 'katex-copytex.min.css', 'katex-copytex.min.js', 'katex.min.css', 'markdown.css', 'vscode-github.css') | ForEach-Object {
            $CssPath = ($LibFolderPath | Join-Path -ChildPath $_);
            if (-not ($CssPath | Test-Path -PathType Leaf)) {
                if ($CssPath | Test-Path) { Remove-Item -LiteralPath $CssPath -Recurse -Force }
                Copy-Item -LiteralPath ($ResourcesPath | Join-Path -ChildPath $_) -Destination $CssPath -Force;
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