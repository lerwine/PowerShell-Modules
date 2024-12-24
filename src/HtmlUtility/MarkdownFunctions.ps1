if ($null -eq $Script:MarkdownObjectExtensions_Descendants) {
    New-Variable -Name 'MarkdownObjectExtensions_Descendants' -Option ReadOnly -Scope 'Script' -Value ([Markdig.Syntax.MarkdownObjectExtensions].GetMethod('Descendants', 1, ([Type[]]@([Markdig.Syntax.MarkdownObject]))));
}

Function New-MarkdownPipeline {
    <#
    .SYNOPSIS
        Creates a new markdown pipeline object.
    .DESCRIPTION
        Creates a new Markdig.MarkdownPipeline object for parsing and rendering.
    .LINK
        Convert-TextToMarkdownDocument
    .LINK
        Open-MarkdownDocument
    .LINK
        https://github.com/xoofx/markdig/blob/fdaef77474dff931d25a443b8f9c038202d81942/src/Markdig/MarkdownPipeline.cs
    #>
    [CmdletBinding(DefaultParameterSetName = 'ExplicitExtensions')]
    [OutputType([Markdig.MarkdownPipeline])]
    Param(
        # Specifies the NewLine character to use for rendering output.
        [string]$NewLine,

        [ValidateScript({ $_.IsAbsolute })]
        # The base URL to use for JIRA links. This implies the inclusion of the JiraLinks extension.
        [Uri]$JiraLinkBaseUrl,

        # Options for the EmphasisExtras extension.
        # Using this parameter implies the inclusion of the EmphasisExtras extension, even if the -UseEmphasisExtras switch is not used.
        [Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions[]]$EmphasisExtraOptions,

        # The base URI path to use for JIRA links. This is ignored if -JiraLinkBaseUrl is not specified.
        [string]$JiraLinkBasePath,

        [Parameter(Mandatory = $true, ParameterSetName = 'UseAdvancedExtensions')]
        # Includes all extensions except the BootStrap, Emoji, SmartyPants and soft line as hard line breaks extensions.
        # This is equivalent to using switches: -UseAlertBlocks -UseAbbreviations -UseAutoIdentifiers -UseCitations -UseCustomContainers -UseDefinitionLists -UseEmphasisExtras -UseFigures -UseFooters -UseFootnotes -UseGridTables -UseMathematics -UseMediaLinks -UsePipeTables -UseListExtras -UseTaskLists -UseDiagrams -UseAutoLinks -UseGenericAttributes
        [switch]$UseAdvancedExtensions,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Includes the AlertExtension to enable parsing of alert blocks.
        [switch]$UseAlertBlocks,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Includes the AutoLinkExtension to parse autolinks from text.
        [switch]$UseAutoLinks,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the task list extension.
        [switch]$UseTaskLists,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the custom container extension.
        [switch]$UseCustomContainers,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the media extension
        [switch]$UseMediaLinks,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the auto-identifier extension.
        [switch]$UseAutoIdentifiers,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the math extension.
        [switch]$UseMathematics,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the figure extension.
        [switch]$UseFigures,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the custom abbreviation extension.
        [switch]$UseAbbreviations,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the definition lists extension.
        [switch]$UseDefinitionLists,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the the pipe table extension.
        [switch]$UsePipeTables,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the grid table extension.
        [switch]$UseGridTables,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the cite extension.
        [switch]$UseCitations,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the footer extension.
        [switch]$UseFooters,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the footnotes extension.
        [switch]$UseFootnotes,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the strikethrough superscript, subscript, inserted and marked text extensions.
        [switch]$UseEmphasisExtras,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the list extra extension to add support for `a.`, `A.`, `i.` and `I.` ordered list items.
        [switch]$UseListExtras,

        [Parameter(ParameterSetName = 'ExplicitExtensions')]
        # Include the generic attributes extension.
        [switch]$UseGenericAttributes,

        # Includes the NonAsciiNoEscapeExtension to disable URI escape with % characters for non-US-ASCII characters in order to workaround a bug under IE/Edge with local file links containing non US-ASCII chars. DO NOT USE OTHERWISE.
        [switch]$UseNonAsciiNoEscape,

        # Includes the YAML frontmatter extension that will parse a YAML frontmatter into the MarkdownDocument. Note that they are not rendered by any default HTML renderer.
        [switch]$UseYamlFrontMatter,

        # Use pragma lines to output span with an id containing the line number (pragma-line#line_number_zero_based`)
        [switch]$UsePragmaLines,

        # Use precise source code location (useful for syntax highlighting).
        [switch]$UsePreciseSourceLocation,

        # Include the SmartyPants extension.
        [switch]$UseSmartyPants,

        # Include the bootstrap extension.
        [switch]$UseBootstrap,

        # Indicates that pipe tables do not require header separators.
        # Using this switch implies the inclusion of the PipeTableExtension extension, even if the -UsePipeTables switch is not used.
        [switch]$PipeTablesDoNotRequireHeaderSeparator,

        # Specifies whether pipe tables should be normalized to the amount of columns as defined in the table header.
        # Using this switch implies the inclusion of the PipeTableExtension extension, even if the -UsePipeTables switch is not used.
        [switch]$UseHeaderForPipeTableColumnCount,

        # Include the softline break as hardline break extension
        [switch]$UseSoftlineBreakAsHardlineBreak,

        # Include the emojis extension.
        [switch]$UseEmoji,

        # Include the emojis extension with smileys enabled.
        [switch]$UseEmojiAndSmiley,

        # Adds "nofollow" to a "rel" attribute on all links.
        [switch]$UseNoFollowLinks,

        # Adds "noopener" to a "rel" attribute on all links.
        [switch]$UseNoOpenerLinks,

        # Adds "noreferrer" to a "rel" attribute on all links.
        [switch]$UseNoReferrerLinks,

        # Specifies that Jira links are not opened in a new window. This is ignored if -JiraLinkBaseUrl is not specified.
        [switch]$DoNotOpenJiraLinksInNewWindow,

        # Adds support for right-to-left content by adding appropriate html attribtues.
        [switch]$UseGlobalization,

        # Disable the HTML support in the markdown processor (for constraint/safe parsing).
        [switch]$DisableHtml,

        # Disables parsing of ATX and Setex headings
        [switch]$DisableHeadings,

        # Enables parsing and tracking of trivia characters
        [switch]$EnableTrackTrivia
    )

    $MarkdownPipelineBuilder = [Markdig.MarkdownPipelineBuilder]::new();
    
    if ($UseAdvancedExtensions.IsPresent) {
        if ($PSBoundParameters.ContainsKey('EmphasisExtraOptions')) {
            $Eo = $EmphasisExtraOptions[0];
            ($EmphasisExtraOptions | Select-Object -Skip 1) | ForEach-Object {
                [Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions]$Eo =  $Eo -bor $_;
            }
            [Markdig.MarkdownExtensions]::UseAlertBlocks([Markdig.MarkdownExtensions]::UseAbbreviations([Markdig.MarkdownExtensions]::UseAutoIdentifiers($MarkdownPipelineBuilder))) | Out-Null;
            [Markdig.MarkdownExtensions]::UseCitations([Markdig.MarkdownExtensions]::UseCustomContainers([Markdig.MarkdownExtensions]::UseDefinitionLists($MarkdownPipelineBuilder))) | Out-Null;
            [Markdig.MarkdownExtensions]::UseEmphasisExtras([Markdig.MarkdownExtensions]::UseFigures([Markdig.MarkdownExtensions]::UseFooters($MarkdownPipelineBuilder)), $Eo) | Out-Null;
            [Markdig.MarkdownExtensions]::UseFootnotes([Markdig.MarkdownExtensions]::UseGridTables([Markdig.MarkdownExtensions]::UseMathematics($MarkdownPipelineBuilder))) | Out-Null;
            if ($PipeTablesDoNotRequireHeaderSeparator.IsPresent) {
                [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder, ([Markdig.Extensions.Tables.PipeTableOptions]@{
                    RequireHeaderSeparator = $false;
                    UseHeaderForColumnCount  = $UseHeaderForPipeTableColumnCount.IsPresent;
                })) | Out-Null;
            } else {
                if ($UseHeaderForPipeTableColumnCount.IsPresent) {
                    [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder, ([Markdig.Extensions.Tables.PipeTableOptions]@{
                        RequireHeaderSeparator = $false;
                        UseHeaderForColumnCount  = $true;
                    })) | Out-Null;
                } else {
                    [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder) | Out-Null;
                }
            }
            [Markdig.MarkdownExtensions]::UseMediaLinks([Markdig.MarkdownExtensions]::UseListExtras([Markdig.MarkdownExtensions]::UseTaskLists($MarkdownPipelineBuilder))) | Out-Null;
            [Markdig.MarkdownExtensions]::UseDiagrams([Markdig.MarkdownExtensions]::UseAutoLinks([Markdig.MarkdownExtensions]::UseGenericAttributes($MarkdownPipelineBuilder))) | Out-Null;
        } else {
            if ($PipeTablesDoNotRequireHeaderSeparator.IsPresent) {
                [Markdig.MarkdownExtensions]::UseAlertBlocks([Markdig.MarkdownExtensions]::UseAbbreviations([Markdig.MarkdownExtensions]::UseAutoIdentifiers($MarkdownPipelineBuilder))) | Out-Null;
                [Markdig.MarkdownExtensions]::UseCitations([Markdig.MarkdownExtensions]::UseCustomContainers([Markdig.MarkdownExtensions]::UseDefinitionLists($MarkdownPipelineBuilder))) | Out-Null;
                [Markdig.MarkdownExtensions]::UseEmphasisExtras([Markdig.MarkdownExtensions]::UseFigures([Markdig.MarkdownExtensions]::UseFooters($MarkdownPipelineBuilder))) | Out-Null;
                [Markdig.MarkdownExtensions]::UseFootnotes([Markdig.MarkdownExtensions]::UseGridTables([Markdig.MarkdownExtensions]::UseMathematics($MarkdownPipelineBuilder))) | Out-Null;
                [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder, ([Markdig.Extensions.Tables.PipeTableOptions]@{
                    RequireHeaderSeparator = $true;
                    UseHeaderForColumnCount  = $UseHeaderForPipeTableColumnCount.IsPresent;
                })) | Out-Null;
                [Markdig.MarkdownExtensions]::UseMediaLinks([Markdig.MarkdownExtensions]::UseListExtras([Markdig.MarkdownExtensions]::UseTaskLists($MarkdownPipelineBuilder))) | Out-Null;
                [Markdig.MarkdownExtensions]::UseDiagrams([Markdig.MarkdownExtensions]::UseAutoLinks([Markdig.MarkdownExtensions]::UseGenericAttributes($MarkdownPipelineBuilder))) | Out-Null;
            } else {
                if ($UseHeaderForPipeTableColumnCount.IsPresent) {
                    [Markdig.MarkdownExtensions]::UseAlertBlocks([Markdig.MarkdownExtensions]::UseAbbreviations([Markdig.MarkdownExtensions]::UseAutoIdentifiers($MarkdownPipelineBuilder))) | Out-Null;
                    [Markdig.MarkdownExtensions]::UseCitations([Markdig.MarkdownExtensions]::UseCustomContainers([Markdig.MarkdownExtensions]::UseDefinitionLists($MarkdownPipelineBuilder))) | Out-Null;
                    [Markdig.MarkdownExtensions]::UseEmphasisExtras([Markdig.MarkdownExtensions]::UseFigures([Markdig.MarkdownExtensions]::UseFooters($MarkdownPipelineBuilder))) | Out-Null;
                    [Markdig.MarkdownExtensions]::UseFootnotes([Markdig.MarkdownExtensions]::UseGridTables([Markdig.MarkdownExtensions]::UseMathematics($MarkdownPipelineBuilder))) | Out-Null;
                    [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder, ([Markdig.Extensions.Tables.PipeTableOptions]@{
                        RequireHeaderSeparator = $true;
                        UseHeaderForColumnCount  = $true;
                    })) | Out-Null;
                    [Markdig.MarkdownExtensions]::UseMediaLinks([Markdig.MarkdownExtensions]::UseListExtras([Markdig.MarkdownExtensions]::UseTaskLists($MarkdownPipelineBuilder))) | Out-Null;
                    [Markdig.MarkdownExtensions]::UseDiagrams([Markdig.MarkdownExtensions]::UseAutoLinks([Markdig.MarkdownExtensions]::UseGenericAttributes($MarkdownPipelineBuilder))) | Out-Null;
                } else {
                    [Markdig.MarkdownExtensions]::UseAdvancedExtensions($MarkdownPipelineBuilder) | Out-Null;
                }
            }
        }
    } else {
        if ($UseAlertBlocks.IsPresent) { [Markdig.MarkdownExtensions]::UseAlertBlocks($MarkdownPipelineBuilder) | Out-Null }
        if ($UseAbbreviations.IsPresent) { [Markdig.MarkdownExtensions]::UseAbbreviations($MarkdownPipelineBuilder) | Out-Null }
        if ($UseAutoIdentifiers.IsPresent) { [Markdig.MarkdownExtensions]::UseAutoIdentifiers($MarkdownPipelineBuilder) | Out-Null }
        if ($UseCitations.IsPresent) { [Markdig.MarkdownExtensions]::UseCitations($MarkdownPipelineBuilder) | Out-Null }
        if ($UseCustomContainers.IsPresent) { [Markdig.MarkdownExtensions]::UseCustomContainers($MarkdownPipelineBuilder) | Out-Null }
        if ($UseDefinitionLists.IsPresent) { [Markdig.MarkdownExtensions]::UseDefinitionLists($MarkdownPipelineBuilder) | Out-Null }
        if ($PSBoundParameters.ContainsKey('EmphasisExtraOptions')) {
            $Eo = $EmphasisExtraOptions[0];
            ($EmphasisExtraOptions | Select-Object -Skip 1) | ForEach-Object {
                [Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions]$Eo =  $Eo -bor $_;
            }
            [Markdig.MarkdownExtensions]::UseEmphasisExtras($MarkdownPipelineBuilder, $Eo) | Out-Null
        } else {
            if ($UseEmphasisExtras.IsPresent) { [Markdig.MarkdownExtensions]::UseEmphasisExtras($MarkdownPipelineBuilder) | Out-Null }
        }
        if ($UseFigures.IsPresent) { [Markdig.MarkdownExtensions]::UseFigures($MarkdownPipelineBuilder) | Out-Null }
        if ($UseFooters.IsPresent) { [Markdig.MarkdownExtensions]::UseFooters($MarkdownPipelineBuilder) | Out-Null }
        if ($UseFootnotes.IsPresent) { [Markdig.MarkdownExtensions]::UseFootnotes($MarkdownPipelineBuilder) | Out-Null }
        if ($UseGridTables.IsPresent) { [Markdig.MarkdownExtensions]::UseGridTables($MarkdownPipelineBuilder) | Out-Null }
        if ($UseMathematics.IsPresent) { [Markdig.MarkdownExtensions]::UseMathematics($MarkdownPipelineBuilder) | Out-Null }
        if ($UseMediaLinks.IsPresent) { [Markdig.MarkdownExtensions]::UseMediaLinks($MarkdownPipelineBuilder) | Out-Null }
        if ($PipeTablesDoNotRequireHeaderSeparator.IsPresent) {
            [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder, ([Markdig.Extensions.Tables.PipeTableOptions]@{
                RequireHeaderSeparator = $UseHeaderForPipeTableColumnCount.IsPresent;
                UseHeaderForColumnCount  = $false;
            })) | Out-Null;
        } else {
            if ($UseHeaderForPipeTableColumnCount.IsPresent) {
                [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder, ([Markdig.Extensions.Tables.PipeTableOptions]@{
                    RequireHeaderSeparator = $true;
                    UseHeaderForColumnCount  = $true;
                })) | Out-Null;
            } else {
                if ($UsePipeTables.IsPresent) { [Markdig.MarkdownExtensions]::UsePipeTables($MarkdownPipelineBuilder) | Out-Null }
            }
        }
        if ($UseListExtras.IsPresent) { [Markdig.MarkdownExtensions]::UseListExtras($MarkdownPipelineBuilder) | Out-Null }
        if ($UseTaskLists.IsPresent) { [Markdig.MarkdownExtensions]::UseTaskLists($MarkdownPipelineBuilder) | Out-Null }
        if ($UseDiagrams.IsPresent) { [Markdig.MarkdownExtensions]::UseDiagrams($MarkdownPipelineBuilder) | Out-Null }
        if ($UseAutoLinks.IsPresent) { [Markdig.MarkdownExtensions]::UseAutoLinks($MarkdownPipelineBuilder) | Out-Null }
        if ($UseGenericAttributes.IsPresent) { [Markdig.MarkdownExtensions]::UseGenericAttributes($MarkdownPipelineBuilder) | Out-Null }
    }
    if ($UseNonAsciiNoEscape.IsPresent) { [Markdig.MarkdownExtensions]::UseNonAsciiNoEscape($MarkdownPipelineBuilder) | Out-Null }
    if ($UseYamlFrontMatter.IsPresent) { [Markdig.MarkdownExtensions]::UseYamlFrontMatter($MarkdownPipelineBuilder) | Out-Null }
    if ($UsePragmaLines.IsPresent) { [Markdig.MarkdownExtensions]::UsePragmaLines($MarkdownPipelineBuilder) | Out-Null }
    if ($UsePreciseSourceLocation.IsPresent) { [Markdig.MarkdownExtensions]::UsePreciseSourceLocation($MarkdownPipelineBuilder) | Out-Null }
    if ($UseSmartyPants.IsPresent) { [Markdig.MarkdownExtensions]::UseSmartyPants($MarkdownPipelineBuilder) | Out-Null }
    if ($UseBootstrap.IsPresent) { [Markdig.MarkdownExtensions]::UseBootstrap($MarkdownPipelineBuilder) | Out-Null }
    if ($UseSoftlineBreakAsHardlineBreak.IsPresent) { [Markdig.MarkdownExtensions]::UseSoftlineBreakAsHardlineBreak($MarkdownPipelineBuilder) | Out-Null }
    if ($UseEmojiAndSmiley.IsPresent) {
        [Markdig.MarkdownExtensions]::UseEmojiAndSmiley($MarkdownPipelineBuilder, $true) | Out-Null;
    } else {
        if ($UseEmoji.IsPresent) { [Markdig.MarkdownExtensions]::UseEmojiAndSmiley($MarkdownPipelineBuilder, $false) | Out-Null }
    }
    if ($UseGlobalization.IsPresent) { [Markdig.MarkdownExtensions]::UseGlobalization($MarkdownPipelineBuilder) | Out-Null }
    if ($DisableHtml.IsPresent) { [Markdig.MarkdownExtensions]::DisableHtml($MarkdownPipelineBuilder) | Out-Null }
    if ($DisableHeadings.IsPresent) { [Markdig.MarkdownExtensions]::DisableHeadings($MarkdownPipelineBuilder) | Out-Null }
    if ($EnableTrackTrivia.IsPresent) { [Markdig.MarkdownExtensions]::EnableTrackTrivia($MarkdownPipelineBuilder) | Out-Null }
    if ($PSBoundParameters.ContainsKey('NewLine')) { [Markdig.MarkdownExtensions]::ConfigureNewLine($NewLine) | Out-Null }
    if ($UseNoFollowLinks.IsPresent) {
        if ($UseNoOpenerLinks.IsPresent) {
            if ($UseNoReferrerLinks.IsPresent) {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'nofollow', 'noopener', 'noreferrer') | Out-Null;
            } else {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'nofollow', 'noopener') | Out-Null;
            }
        } else {
            if ($UseNoReferrerLinks.IsPresent) {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'nofollow', 'noreferrer') | Out-Null;
            } else {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'nofollow') | Out-Null;
            }
        }
    } else {
        if ($UseNoOpenerLinks.IsPresent) {
            if ($UseNoReferrerLinks.IsPresent) {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'noopener', 'noreferrer') | Out-Null;
            } else {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'noopener') | Out-Null;
            }
        } else {
            if ($UseNoReferrerLinks.IsPresent) {
                [Markdig.MarkdownExtensions]::UseReferralLinks($MarkdownPipelineBuilder, 'noreferrer') | Out-Null;
            }
        }
    }
    if ($PSBoundParameters.ContainsKey('JiraLinkBaseUrl')) {
        $JiraLinkOptions = [Markdig.Extensions.JiraLinks.JiraLinkOptions]::new($JiraLinkBaseUrl.AbsoluteUri);
        $JiraLinkOptions.OpenInNewWindow = -not $DoNotOpenJiraLinksInNewWindow.IsPresent;
        if ($PSBoundParameters.ContainsKey('JiraLinkBasePath')) { $JiraLinkOptions.BasePath = $JiraLinkBasePath }
        [Markdig.MarkdownExtensions]::UseJiraLinks($MarkdownPipelineBuilder, $JiraLinkOptions) | Out-Null
    } else {
        if ($PSBoundParameters.ContainsKey('JiraLinkBasePath')) {
            if ($DoNotOpenJiraLinksInNewWindow.IsPresent) {
                Write-Warning -Message '-JiraLinkBasePath and -DoNotOpenJiraLinksInNewWindow ignored because -JiraLinkBaseUrl was not provided.';
            } else {
                Write-Warning -Message '-JiraLinkBasePath ignored because -JiraLinkBaseUrl was not provided.';
            }
        } else {
            if ($DoNotOpenJiraLinksInNewWindow.IsPresent) {
                Write-Warning -Message '-DoNotOpenJiraLinksInNewWindow ignored because -JiraLinkBaseUrl was not provided.';
            }
        }
    }

    Write-Output -InputObject $MarkdownPipelineBuilder.Build() -NoEnumerate;
}

Function Convert-TextToMarkdownDocument {
    <#
    .SYNOPSIS
        Parses input text as a Markdig.Syntax.MarkdownDocument object.
    .DESCRIPTION
        Parses input text, creating a Markdig.Syntax.MarkdownDocument object.
    .LINK
        New-MarkdownPipeline
    .LINK
        Open-MarkdownDocument
    #>
    [CmdletBinding(DefaultParameterSetName = 'ExplicitExtensions')]
    [OutputType([Markdig.Syntax.MarkdownDocument])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The text containing the markdown to parse.
        [string]$InputText,

        # The pipeline to use for parsing the markdown.
        # The default is to use a pipeline created using New-MarkdownPipeline -UseAdvancedExtensions
        [Markdig.MarkdownPipeline]$MarkdownPipeline
    )

    Begin {
        $Local:MarkdownTextLines = @();
    }

    Process {
        $Local:MarkdownTextLines += $InputText;
    }

    End {
        if ($Local:MarkdownTextLines.Count -eq 1) {
            $Local:MarkdownText = $Local:MarkdownTextLines[0];
        } else {
            $Local:MarkdownText = $Local:MarkdownTextLines -join "`n";
        }

        if ([string]::IsNullOrWhiteSpace($Local:MarkdownText)) {
            Write-Warning -Message "Input text contains no markdown";
        } else {
            try {
                if ($PSBoundParameters.ContainsKey('MarkdownPipeline')) {
                    $Local:MarkdownDocument = [Markdig.Markdown]::Parse($Local:MarkdownText, $MarkdownPipeline);
                } else {
                    $Local:MarkdownDocument = [Markdig.Markdown]::Parse($Local:MarkdownText, (New-MarkdownPipeline -UseAdvancedExtensions));
                }
                Write-Output -InputObject $Local:MarkdownDocument -NoEnumerate;
            } catch {
                Write-Error -Exception $_.Exception -Category $_.CategoryInfo.Category -Message "Failed to parse contents of $Path`: $_" -ErrorId 'FileParseError' -TargetObject $Path -CategoryTargetName 'Path';
            }
        }
        
    }
}

Function Open-MarkdownDocument {
    <#
    .SYNOPSIS
        Opens a Markdown document.
    .DESCRIPTION
        Parses a file into a MarkdownDocument object.
    .LINK
        New-MarkdownPipeline
    .LINK
        Convert-TextToMarkdownDocument
    #>
    [CmdletBinding(DefaultParameterSetName = 'ExplicitExtensions')]
    Param(
        # Specifies a path to one or more markdown files. Wildcards are permitted.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "WcPath", ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, HelpMessage = "Path to one or more locations.")]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        [string[]]$Path,

        # Specifies a path to one or more markdown files. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = $true, HelpMessage = "Literal path to one or more locations.")]
        [Alias("PSPath")]
        [ValidateNotNullOrEmpty()]
        [string[]]$LiteralPath,

        # The pipeline to use for parsing the markdown.
        # The default is to use a pipeline created using New-MarkdownPipeline -UseAdvancedExtensions
        [Markdig.MarkdownPipeline]$MarkdownPipeline
    )

    Begin {
        $Local:MdPipline = $MarkdownPipeline;
        if (-not $PSBoundParameters.ContainsKey('MarkdownPipeline')) {
            $Local:MdPipline = New-MarkdownPipeline -UseAdvancedExtensions;
        }
    }

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            foreach ($p in $LiteralPath) {
                try { $Local:Content = Get-Content -LiteralPath $p -ErrorAction Stop }
                catch {
                    $ex = $_.Exception;
                    if ($null -ne $ex.InnerException -and $ex.InnerException -is [System.Management.Automation.IContainsErrorRecord]) { $ex = $ex.InnerException }
                    Write-Error -Exception $ex -Message "Failed to read from $p`: $_" -Category $ex.ErrorRecord.CategoryInfo.Category -ErrorId 'FileReadFailure' -TargetObject $p -CategoryTargetName 'LiteralPath';
                    continue;
                }
                if ([string]::IsNullOrWhiteSpace($Local:Content)) {
                    Write-Error -Message "File $p is empty" -Category InvalidData -ErrorId 'EmptyFile' -TargetObject $p -CategoryTargetName 'LiteralPath';
                } else {
                    try { $Local:Content | Convert-TextToMarkdownDocument -MarkdownPipeline $Local:MdPipline -ErrorAction Stop }
                    catch {
                        $ex = $_.Exception;
                        if ($null -ne $ex.InnerException -and $ex.InnerException -is [System.Management.Automation.IContainsErrorRecord]) { $ex = $ex.InnerException }
                        Write-Error -Exception $ex -Message "Failed to parse content of $p`: $_" -Category ParserError -ErrorId 'FileParseError' -TargetObject $p -CategoryTargetName 'LiteralPath';
                    }
                }
            }
        } else {
            $provider = $null;
            foreach ($WcPath in $Path) {
                if ($WcPath | Test-Path -PathType Leaf) {
                    foreach ($p in $psCmdlet.SessionState.Path.GetResolvedProviderPathFromPSPath($p, [ref]$provider)) {
                        if ($p | Test-Path -PathType Leaf) {
                            try { $Local:Content | Convert-TextToMarkdownDocument -MarkdownPipeline $Local:MdPipline -ErrorAction Stop }
                            catch {
                                $ex = $_.Exception;
                                if ($null -ne $ex.InnerException -and $ex.InnerException -is [System.Management.Automation.IContainsErrorRecord]) { $ex = $ex.InnerException }
                                Write-Error -Exception $ex -Message "Failed to read from $p`: $_" -Category $ex.ErrorRecord.CategoryInfo.Category -ErrorId 'FileReadFailure' -TargetObject $p -CategoryTargetName 'Path';
                                continue;
                            }
                            if ([string]::IsNullOrWhiteSpace($Local:Content)) {
                                Write-Error -Message "File $p is empty" -Category InvalidData -ErrorId 'EmptyFile' -TargetObject $p -CategoryTargetName 'Path';
                            } else {
                                try { Convert-TextToMarkdownDocument -InputText $Local:Content -MarkdownPipeline $Local:MdPipline -ErrorAction Stop }
                                catch {
                                    $ex = $_.Exception;
                                    if ($null -ne $ex.InnerException -and $ex.InnerException -is [System.Management.Automation.IContainsErrorRecord]) { $ex = $ex.InnerException }
                                    Write-Error -Exception $ex -Message "Failed to parse content of $p`: $_" -Category ParserError -ErrorId 'FileParseError' -TargetObject $p -CategoryTargetName 'Path';
                                }
                            }
                        }
                    }
                } else {
                    $ex = [System.Management.Automation.ItemNotFoundException]::new("Cannot find file '$WcPath' because it does not exist.");
                    Write-Error -Exception $ex -Message $ex.Message -Category ObjectNotFound -ErrorId 'FileNotFound' -TargetObject $WcPath;
                }
            }
        }
    }
}

Function Get-MarkdownDescendants {
    <#
    .SYNOPSIS
        Iterates over the descendant elements for the specified markdown element and filters by a specified type.
    .DESCRIPTION
        Iterates over the descendant elements for the specified markdown element, filtered by the specified type. The descendant elements are returned in DFS-like order.
    .LINK
        https://github.com/xoofx/markdig/blob/fdaef77474dff931d25a443b8f9c038202d81942/src/Markdig/Syntax/MarkdownObjectExtensions.cs#L22
    #>
    [CmdletBinding(DefaultParameterSetName = 'AllDescendants')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $true)]
        [Alias('Tokens', 'Markdown')]
        # The parent markdown objects.
        [Markdig.Syntax.MarkdownObject]$MarkdownObject,

        [Parameter(Mandatory = $true, ParameterSetName = 'FilteredDescendants')]
        [ValidateScript({ [Markdig.Syntax.MarkdownObject].IsAssignableFrom($_) })]
        # The type to use for filtering descendants.
        [Type]$Type,

        [Parameter(ParameterSetName = 'AllDescendants')]
        # Indicates whether to include HtmlAttributes.
        [switch]$IncludeAttributes
    )

    Process {
        if ($PSBoundParameters.ContainsKey('Type') -and $Type -ne [Markdig.Syntax.MarkdownObject]) {
            if ($Type -eq [Markdig.Renderers.Html.HtmlAttributes]) {
                $Local:HtmlAttributes = [Markdig.Renderers.Html.HtmlAttributesExtensions]::TryGetAttributes($MarkdownObject);
                if ($null -ne $HtmlAttributes) {
                    Write-Output -InputObject $HtmlAttributes -NoEnumerate;
                }
                [Markdig.Syntax.MarkdownObjectExtensions]::Descendants($MarkdownObject) | ForEach-Object {
                    $HtmlAttributes = [Markdig.Renderers.Html.HtmlAttributesExtensions]::TryGetAttributes($_);
                    if ($null -ne $HtmlAttributes) {
                        Write-Output -InputObject $HtmlAttributes -NoEnumerate;
                    }
                }
            } else {
                $Script:MarkdownObjectExtensions_Descendants.MakeGenericMethod($Type).Invoke($null, ([object[]]@(,$MarkdownObject))) | Write-Output;
            }
        } else {
            if ($IncludeAttributes.IsPresent) {
                $Local:HtmlAttributes = [Markdig.Renderers.Html.HtmlAttributesExtensions]::TryGetAttributes($MarkdownObject);
                if ($null -ne $HtmlAttributes) {
                    Write-Output -InputObject $HtmlAttributes -NoEnumerate;
                }
                [Markdig.Syntax.MarkdownObjectExtensions]::Descendants($MarkdownObject) | ForEach-Object {
                    Write-Output -InputObject $_ -NoEnumerate;
                    $HtmlAttributes = [Markdig.Renderers.Html.HtmlAttributesExtensions]::TryGetAttributes($_);
                    if ($null -ne $HtmlAttributes) {
                        Write-Output -InputObject $HtmlAttributes -NoEnumerate;
                    }
                }
            } else {
                [Markdig.Syntax.MarkdownObjectExtensions]::Descendants($MarkdownObject) | Write-Output;
            }
        }
    }
}

$Path = $PSScriptRoot | Join-Path -ChildPath '../HtmlUtility.UnitTests/Resources/Example1.md';
[Markdig.Syntax.MarkdownDocument]$Document = (ConvertFrom-Markdown -LiteralPath $Path).Tokens;

$Descendants = @(Get-MarkdownDescendants -MarkdownObject $Document -Type ([Markdig.Renderers.Html.HtmlAttributes]));
$Descendants | % { $_.GetType().Name }
$Descendants.Count;