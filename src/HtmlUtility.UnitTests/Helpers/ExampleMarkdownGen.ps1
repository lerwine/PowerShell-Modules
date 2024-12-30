Import-Module ($PSScriptRoot | Join-Path -ChildPath '../bin/Debug/net8.0/Erwine.Leonard.T.HtmlUtility.psd1') -ErrorAction Stop;

Function Convert-ToJsonObject {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [Markdig.Syntax.MarkdownObject]$MarkdownObject,

        [Parameter(Mandatory = $true)]
        [int]$Level,

        [Parameter(Mandatory = $true)]
        [int]$Index
    )

    $InnerLevel = $Level + 1;
    $ChildObjects = [System.Text.Json.Nodes.JsonArray]::new();
    [Markdig.Renderers.Html.HtmlAttributes]$HtmlAttributes = [Markdig.Renderers.Html.HtmlAttributesExtensions]::TryGetAttributes($MarkdownObject);
    if ($null -ne $HtmlAttributes) {
        [System.Text.Json.Nodes.JsonObject]$Position = [System.Text.Json.Nodes.JsonObject]::new();
        $Position.Add('Level', [System.Text.Json.Nodes.JsonValue]::Create($InnerLevel));
        
        if (-not $HtmlAttributes.Span.IsEmpty) {
            $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($HtmlAttributes.Line));
            $Position.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($HtmlAttributes.Column));
            $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($HtmlAttributes.Span.Start));
            $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($HtmlAttributes.Span.End));
        }
        $ElementJson = [System.Text.Json.Nodes.JsonObject]::new();
        $ElementJson.Add('Name', [System.Text.Json.Nodes.JsonValue]::Create($HtmlAttributes.GetType().Name));
        $ElementJson.Add('Position', $Position);
        if (-not [string]::IsNullOrEmpty($HtmlAttributes.Id)) {
            $ElementJson.Add('Id', [System.Text.Json.Nodes.JsonValue]::Create($HtmlAttributes.Column));
        }
        if ($null -ne $HtmlAttributes.Classes -and $HtmlAttributes.Classes.Count -gt 0) {
            $JsonArray = [System.Text.Json.Nodes.JsonArray]::new();
            $HtmlAttributes.Classes | ForEach-Object { $JsonArray.Add([System.Text.Json.Nodes.JsonValue]::Create($_)) }
            $ElementJson.Add('Classes', $JsonArray);
        }
        if ($null -ne $HtmlAttributes.Properties -and $HtmlAttributes.Properties.Count -gt 0) {
            $JsonArray = [System.Text.Json.Nodes.JsonArray]::new();
            $HtmlAttributes.Properties | ForEach-Object {
                $JsonObject = [System.Text.Json.Nodes.JsonObject]::new();
                $JsonObject.Add('Name', [System.Text.Json.Nodes.JsonValue]::Create($_.Key));
                $JsonObject.Add('Value', [System.Text.Json.Nodes.JsonValue]::Create($_.Value));
                $JsonArray.Add($JsonObject);
            }
            $ElementJson.Add('Properties', $JsonArray);
        }
        $ChildObjects.Add($ElementJson);
    }
    [System.Text.Json.Nodes.JsonObject]$Position = [System.Text.Json.Nodes.JsonObject]::new();
    $Position.Add('Level', [System.Text.Json.Nodes.JsonValue]::Create($Level));
    $Position.Add('Index', [System.Text.Json.Nodes.JsonValue]::Create($Index));
    $ElementJson = [System.Text.Json.Nodes.JsonObject]::new();
    $ElementJson.Add('Name', [System.Text.Json.Nodes.JsonValue]::Create($MarkdownObject.GetType().Name));
    $ElementJson.Add('Position', $Position);
    if ($MarkdownObject -is [Markdig.Syntax.Block]) {
        [Markdig.Syntax.Block]$Block = $MarkdownObject;
        if (-not $Block.TriviaBefore.IsEmpty) {
            $ElementJson.Add('TriviaBefore', [System.Text.Json.Nodes.JsonValue]::Create($Block.TriviaBefore.ToString()));
        }
            $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($Block.Line));
            if (-not $Block.Span.IsEmpty) {
                $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($Block.Span.Start));
                $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($Block.Span.End));
            }
        if ($Block -is [Markdig.Syntax.ContainerBlock]) {
            [Markdig.Syntax.ContainerBlock]$ContainerBlock = $Block;
            if ($ContainerBlock -is [Markdig.Syntax.ListBlock]) {
                [Markdig.Syntax.ListBlock]$ListBlock = $ContainerBlock;
                $ElementJson.Add('IsOrdered', [System.Text.Json.Nodes.JsonValue]::Create($ListBlock.IsOrdered));
                $ElementJson.Add('BulletType', [System.Text.Json.Nodes.JsonValue]::Create($ListBlock.BulletType));
                if ($ListBlock.IsOrdered) {
                    if ($null -ne $ListBlock.OrderedStart) {
                        $ElementJson.Add('OrderedStart', [System.Text.Json.Nodes.JsonValue]::Create($ListBlock.OrderedStart));
                    }
                    $ElementJson.Add('OrderedDelimiter', [System.Text.Json.Nodes.JsonValue]::Create($ListBlock.OrderedDelimiter));
                }
            } else {
                if ($ContainerBlock -is [Markdig.Extensions.Tables.Table]) {
                    [Markdig.Extensions.Tables.Table]$Table = $ContainerBlock;
                    if ($Table.ColumnDefinitions.Count -gt 0) {
                        $JsonArray = [System.Text.Json.Nodes.JsonArray]::new();
                        $Table.ColumnDefinitions | ForEach-Object {
                            $JsonObject = [System.Text.Json.Nodes.JsonObject]::new();
                            $JsonObject.Add('Width', [System.Text.Json.Nodes.JsonValue]::Create($_.Width));
                            if ($null -ne $_.Alignment) {
                                $JsonObject.Add('Alignment', [System.Text.Json.Nodes.JsonValue]::Create($_.Alignment.ToString('F')));
                            }
                            $JsonArray.Add($JsonObject);
                        }
                        $ElementJson.Add('ColumnDefinitions', $JsonArray);
                    }
                } else {
                    if ($ContainerBlock -is [Markdig.Extensions.Tables.TableCell]) {
                        [Markdig.Extensions.Tables.TableCell]$TableCell = $ContainerBlock;
                        if ($TableCell.ColumnIndex -ge 0) {
                            $ElementJson.Add('ColumnIndex', [System.Text.Json.Nodes.JsonValue]::Create($TableCell.ColumnIndex));
                        }
                        if ($TableCell.RowSpan -gt 1) { $ElementJson.Add('RowSpan', [System.Text.Json.Nodes.JsonValue]::Create($TableCell.RowSpan)) }
                        if ($TableCell.ColumnSpan -gt 1) { $ElementJson.Add('ColumnSpan', [System.Text.Json.Nodes.JsonValue]::Create($TableCell.ColumnSpan)) }
                    } else {
                        if ($ContainerBlock -is [Markdig.Extensions.Tables.TableRow]) {
                            [Markdig.Extensions.Tables.TableRow]$TableRow = $ContainerBlock;
                            $ElementJson.Add('IsHeader', [System.Text.Json.Nodes.JsonValue]::Create($TableRow.IsHeader));
                        } else {
                            if ($ContainerBlock -is [Markdig.Extensions.Footnotes.Footnote]) {
                                [Markdig.Extensions.Footnotes.Footnote]$Footnote = $ContainerBlock;
                                if ($Footnote.Order -ge 0) { $ElementJson.Add('Order', [System.Text.Json.Nodes.JsonValue]::Create($Footnote.Order)) }
                                if ($null -ne $Footnote.Label) { $ElementJson.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($Footnote.Label)) }
                                $ElementJson.Add('IsLastLineEmpty', [System.Text.Json.Nodes.JsonValue]::Create($Footnote.IsLastLineEmpty))
                                if ($Footnote.Links.Count -gt 0) {
                                    $JsonArray = [System.Text.Json.Nodes.JsonArray]::new();
                                    $Footnote.Links | ForEach-Object {
                                        [System.Text.Json.Nodes.JsonObject]$Link = [System.Text.Json.Nodes.JsonObject]::new();
                                        $Link.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($_.Line));
                                        $Link.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($_.Column));
                                        $JsonArray.Add($Link);
                                    }
                                    $ElementJson.Add('Links', $JsonArray);
                                }
                            } else {
                                if ($ContainerBlock -is [Markdig.Extensions.CustomContainers.CustomContainer]) {
                                    [Markdig.Extensions.CustomContainers.CustomContainer]$CustomContainer = $ContainerBlock;
                                    $ElementJson.Add('FencedChar', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.FencedChar));
                                    $ElementJson.Add('OpeningFencedCharCount', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.OpeningFencedCharCount));
                                    if (-not $CustomContainer.TriviaAfterFencedChar.IsEmpty) {
                                        $ElementJson.Add('TriviaAfterFencedChar', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.TriviaAfterFencedChar.ToString()));
                                    }
                                    if (-not [string]::IsNullOrEmpty($CustomContainer.Arguments)) {
                                        $ElementJson.Add('Arguments', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.Arguments));
                                    }
                                    if (-not $CustomContainer.TriviaAfterArguments.IsEmpty) {
                                        $ElementJson.Add('TriviaAfterArguments', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.TriviaAfterArguments.ToString()));
                                    }
                                    if (-not [string]::IsNullOrEmpty($CustomContainer.Info)) {
                                        $ElementJson.Add('Info', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.Info));
                                    }
                                    if (-not $CustomContainer.TriviaAfterInfo.IsEmpty) {
                                        $ElementJson.Add('TriviaAfterInfo', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.TriviaAfterInfo.ToString()));
                                    }
                                    if (-not $CustomContainer.TriviaBeforeClosingFence.IsEmpty) {
                                        $ElementJson.Add('TriviaBeforeClosingFence', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.TriviaBeforeClosingFence.ToString()));
                                    }
                                    if ($CustomContainer.ClosingFencedCharCount -ne $CustomContainer.OpeningFencedCharCount) {
                                        $ElementJson.Add('ClosingFencedCharCount', [System.Text.Json.Nodes.JsonValue]::Create($CustomContainer.ClosingFencedCharCount));
                                    }
                                } else {
                                    if ($ContainerBlock -is [Markdig.Extensions.Figures.Figure]) {
                                        [Markdig.Extensions.Figures.Figure]$Figure = $ContainerBlock;
                                        $ElementJson.Add('OpeningCharacter', [System.Text.Json.Nodes.JsonValue]::Create($Figure.OpeningCharacter));
                                        $ElementJson.Add('OpeningCharacterCount', [System.Text.Json.Nodes.JsonValue]::Create($Figure.OpeningCharacterCount));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for ($i = 0; $i -lt $ContainerBlock.Count; $i++) {
                $JsonObject = Convert-ToJsonObject -MarkdownObject $ContainerBlock[$i] -Level $InnerLevel -Index $i;
                $ChildObjects.Add($JsonObject);
            }
        } else {
            if ($Block -is [Markdig.Syntax.LeafBlock]) {
                [Markdig.Syntax.LeafBlock]$LeafBlock = $Block;
                if ($LeafBlock -is [Markdig.Syntax.CodeBlock]) {
                    [Markdig.Syntax.CodeBlock]$CodeBlock = $LeafBlock;
                    if ($CodeBlock -is [Markdig.Syntax.FencedCodeBlock]) {
                        [Markdig.Syntax.FencedCodeBlock]$FencedCodeBlock = $CodeBlock;
                        $ElementJson.Add('FencedChar', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.FencedChar));
                        $ElementJson.Add('IndentCount', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.IndentCount));
                        $ElementJson.Add('OpeningFencedCharCount', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.OpeningFencedCharCount));
                        if (-not $FencedCodeBlock.TriviaAfterFencedChar.IsEmpty) {
                            $ElementJson.Add('TriviaAfterFencedChar', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.TriviaAfterFencedChar.ToString()));
                        }
                        if (-not [string]::IsNullOrEmpty($FencedCodeBlock.Info)) {
                            $ElementJson.Add('Info', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.Info));
                        }
                        if (-not $FencedCodeBlock.TriviaAfterInfo.IsEmpty) {
                            $ElementJson.Add('TriviaAfterInfo', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.TriviaAfterInfo.ToString()));
                        }
                        if (-not [string]::IsNullOrEmpty($FencedCodeBlock.Arguments)) {
                            $ElementJson.Add('Arguments', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.Arguments));
                        }
                        if (-not $FencedCodeBlock.TriviaAfterArguments.IsEmpty) {
                            $ElementJson.Add('TriviaAfterArguments', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.TriviaAfterArguments.ToString()));
                        }
                        if (-not $FencedCodeBlock.TriviaBeforeClosingFence.IsEmpty) {
                            $ElementJson.Add('TriviaBeforeClosingFence', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.TriviaBeforeClosingFence.ToString()));
                        }
                        if ($FencedCodeBlock.ClosingFencedCharCount -ne $FencedCodeBlock.OpeningFencedCharCount) {
                            $ElementJson.Add('ClosingFencedCharCount', [System.Text.Json.Nodes.JsonValue]::Create($FencedCodeBlock.ClosingFencedCharCount));
                        }
                    }
                    if ($CodeBlock.CodeBlockLines.Count -gt 0) {
                        $JsonArray = [System.Text.Json.Nodes.JsonArray]::new();
                        $CodeBlock.CodeBlockLines | ForEach-Object {
                            $JsonArray.Add([System.Text.Json.Nodes.JsonValue]::Create($_.TriviaBefore.ToString()));
                        }
                        $ElementJson.Add('CodeBlockLines', $JsonArray);
                    }
                } else {
                    if ($LeafBlock -is [Markdig.Syntax.ThematicBreakBlock]) {
                        [Markdig.Syntax.ThematicBreakBlock]$ThematicBreakBlock = $LeafBlock;
                        $ElementJson.Add('ThematicChar', [System.Text.Json.Nodes.JsonValue]::Create($ThematicBreakBlock.ThematicChar));
                        $ElementJson.Add('ThematicCharCount', [System.Text.Json.Nodes.JsonValue]::Create($ThematicBreakBlock.ThematicCharCount));
                    } else {
                        if ($LeafBlock -is [Markdig.Syntax.HeadingBlock]) {
                            [Markdig.Syntax.HeadingBlock]$HeadingBlock = $LeafBlock;
                            $ElementJson.Add('HeaderChar', [System.Text.Json.Nodes.JsonValue]::Create($HeadingBlock.HeaderChar));
                            $ElementJson.Add('HeaderCharCount', [System.Text.Json.Nodes.JsonValue]::Create($HeadingBlock.HeaderCharCount));
                            $ElementJson.Add('Level', [System.Text.Json.Nodes.JsonValue]::Create($HeadingBlock.Level));
                            $ElementJson.Add('IsSetext', [System.Text.Json.Nodes.JsonValue]::Create($HeadingBlock.IsSetext));
                        } else {
                            if ($LeafBlock -is [Markdig.Extensions.Abbreviations.Abbreviation]) {
                                [Markdig.Extensions.Abbreviations.Abbreviation]$Abbreviation = $LeafBlock;
                                if ($null -ne $Abbreviation.Label) {
                                    $ElementJson.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($Abbreviation.Label));
                                }
                                if (-not $Abbreviation.Text.IsEmpty) {
                                    $ElementJson.Add('Text', [System.Text.Json.Nodes.JsonValue]::Create($Abbreviation.Text.ToString()));
                                }
                            } else {
                                if ($LeafBlock -is [Markdig.Syntax.LinkReferenceDefinition]) {
                                    [Markdig.Syntax.LinkReferenceDefinition]$LinkReferenceDefinition = $LeafBlock;
                                    if ($null -ne $LinkReferenceDefinition.Label) {
                                        $ElementJson.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($LinkReferenceDefinition.Label));
                                    }
                                    if (-not $LinkReferenceDefinition.TriviaBeforeUrl.IsEmpty) {
                                        $ElementJson.Add('TriviaBeforeUrl', [System.Text.Json.Nodes.JsonValue]::Create($LinkReferenceDefinition.TriviaBeforeUrl.ToString()));
                                    }
                                    if ($null -ne $LinkReferenceDefinition.Url) {
                                        $ElementJson.Add('Url', [System.Text.Json.Nodes.JsonValue]::Create($LinkReferenceDefinition.Url));
                                        $ElementJson.Add('UrlHasPointyBrackets', [System.Text.Json.Nodes.JsonValue]::Create($LinkReferenceDefinition.UrlHasPointyBrackets));
                                    }
                                    if (-not $LinkReferenceDefinition.TriviaBeforeTitle.IsEmpty) {
                                        $ElementJson.Add('TriviaBeforeTitle', [System.Text.Json.Nodes.JsonValue]::Create($LinkReferenceDefinition.TriviaBeforeTitle.ToString()));
                                    }
                                    if (-not [string]::IsNullOrEmpty($LinkReferenceDefinition.Title)) {
                                        $ElementJson.Add('Title', [System.Text.Json.Nodes.JsonValue]::Create($LinkReferenceDefinition.Title));
                                    }
                                    if ($LinkReferenceDefinition -is [Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition]) {
                                        [Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition]$FootnoteLinkReferenceDefinition = $LinkReferenceDefinition;
                                        $JsonObject = [System.Text.Json.Nodes.JsonObject]::new();
                                        $JsonObject.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLinkReferenceDefinition.Footnote.Line));
                                        $JsonObject.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLinkReferenceDefinition.Footnote.Column));
                                        if ($null -ne $FootnoteLinkReferenceDefinition.Footnote.Label) {
                                            $JsonObject.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLinkReferenceDefinition.Footnote.Label));
                                        }
                                        if ($FootnoteLinkReferenceDefinition.Footnote.Order -ge 0)
                                        {
                                            $JsonObject.Add('Order', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLinkReferenceDefinition.Footnote.Order));
                                        }
                                        $ElementJson.Add('Footnote', $JsonObject);
                                    } else {
                                        if ($LinkReferenceDefinition -is [Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition]) {
                                            [Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition]$HeadingLinkReferenceDefinition = $LinkReferenceDefinition;
                                            $JsonObject = [System.Text.Json.Nodes.JsonObject]::new();
                                            $JsonObject.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($HeadingLinkReferenceDefinition.Heading.Line));
                                            $JsonObject.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($HeadingLinkReferenceDefinition.Heading.Column));
                                            $Attr = [Markdig.Renderers.Html.HtmlAttributesExtensions]::TryGetAttributes($HeadingLinkReferenceDefinition.Heading);
                                            if ($null -ne $Attr -and -not [string]::IsNullOrEmpty($Attr.Id)) {
                                                $JsonObject.Add('Id', [System.Text.Json.Nodes.JsonValue]::Create($Attr.Id));
                                            }
                                            $ElementJson.Add('Heading', $JsonObject);
                                        }
                                    }
                                } else {
                                    if ($LeafBlock -is [Markdig.Syntax.HtmlBlock]) {
                                        [Markdig.Syntax.HtmlBlock]$HtmlBlock = $LeafBlock;
                                        $ElementJson.Add('Type', [System.Text.Json.Nodes.JsonValue]::Create($HtmlBlock.Type.ToString('F')));
                                    }
                                }
                            }
                        }
                    }
                }
                if ($null -ne $LeafBlock.Inline) {
                    $i = -1;
                    for ($c = $LeafBlock.Inline.FirstChild; $null -ne $c; $c = $c.NextSibling) {
                        $i++;
                        $JsonObject = Convert-ToJsonObject -MarkdownObject $c -Level $InnerLevel -Index $i;
                        $ChildObjects.Add($JsonObject);
                    }
                }
            }
        }
        if (-not $Block.TriviaAfter.IsEmpty) {
            $ElementJson.Add('TriviaAfter', [System.Text.Json.Nodes.JsonValue]::Create($Block.TriviaAfter.ToString()));
        }
    } else {
        if ($MarkdownObject -is [Markdig.Syntax.Inlines.Inline]) {
            [Markdig.Syntax.Inlines.Inline]$Inline =  $MarkdownObject;
            if ($Inline -is [Markdig.Syntax.Inlines.ContainerInline]) {
                [Markdig.Syntax.Inlines.ContainerInline]$ContainerInline =  $Inline;
                $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($ContainerInline.Line));
                $Position.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($ContainerInline.Column));
                $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($ContainerInline.Span.Start));
                $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($ContainerInline.Span.End));
                if ($ContainerInline -is [Markdig.Syntax.Inlines.DelimiterInline]) {
                    [Markdig.Syntax.Inlines.DelimiterInline]$DelimiterInline = $ContainerInline;
                    $ElementJson.Add('Type', [System.Text.Json.Nodes.JsonValue]::Create($DelimiterInline.Type.ToString('F')));
                    if ($DelimiterInline -is [Markdig.Syntax.Inlines.EmphasisDelimiterInline]) {
                        [Markdig.Syntax.Inlines.EmphasisDelimiterInline]$EmphasisDelimiterInline = $DelimiterInline;
                        $ElementJson.Add('DelimiterChar', [System.Text.Json.Nodes.JsonValue]::Create($EmphasisDelimiterInline.DelimiterChar));
                        $ElementJson.Add('DelimiterCount', [System.Text.Json.Nodes.JsonValue]::Create($EmphasisDelimiterInline.DelimiterCount));
                    } else {
                        if ($DelimiterInline -is [Markdig.Syntax.Inlines.LinkDelimiterInline]) {
                            [Markdig.Syntax.Inlines.LinkDelimiterInline]$LinkDelimiterInline = $DelimiterInline;
                            $ElementJson.Add('IsImage', [System.Text.Json.Nodes.JsonValue]::Create($LinkDelimiterInline.IsImage));
                            if ($null -ne $LinkDelimiterInline.Label) {
                                $ElementJson.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($LinkDelimiterInline.Label));
                            }
                        } else {
                            if ($DelimiterInline -is [Markdig.Extensions.Tables.PipeTableDelimiterInline]) {
                                [Markdig.Extensions.Tables.PipeTableDelimiterInline]$PipeTableDelimiterInline = $DelimiterInline;
                                $ElementJson.Add('LocalLineIndex', [System.Text.Json.Nodes.JsonValue]::Create($PipeTableDelimiterInline.LocalLineIndex));
                            }
                        }
                    }
                } else {
                    if ($ContainerInline -is [Markdig.Syntax.Inlines.EmphasisInline]) {
                        [Markdig.Syntax.Inlines.EmphasisInline]$EmphasisInline = $ContainerInline;
                        $ElementJson.Add('DelimiterChar', [System.Text.Json.Nodes.JsonValue]::Create($EmphasisInline.DelimiterChar));
                        $ElementJson.Add('DelimiterCount', [System.Text.Json.Nodes.JsonValue]::Create($EmphasisInline.DelimiterCount));
                    } else {
                        if ($ContainerInline -is [Markdig.Syntax.Inlines.LinkInline]) {
                            [Markdig.Syntax.Inlines.LinkInline]$LinkInline = $ContainerInline;
                            if ($null -ne $LinkInline.Label) {
                                $ElementJson.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.Label));
                            }
                            if ($null -ne $LinkInline.LinkRefDefLabel) {
                                $ElementJson.Add('LinkRefDefLabel', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.LinkRefDefLabel));
                            }
                            if (-not $LinkInline.TriviaBeforeUrl.IsEmpty) {
                                $ElementJson.Add('TriviaBeforeUrl', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.TriviaBeforeUrl.ToString()));
                            }
                            if ($null -ne $LinkInline.Url) {
                                $ElementJson.Add('Url', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.Url));
                                $ElementJson.Add('UrlHasPointyBrackets', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.UrlHasPointyBrackets));
                            }
                            if (-not $LinkInline.TriviaAfterUrl.IsEmpty) {
                                $ElementJson.Add('TriviaAfterUrl', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.TriviaAfterUrl.ToString()));
                            }
                            if (-not [string]::IsNullOrEmpty($LinkInline.Title)) {
                                $ElementJson.Add('Title', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.Title));
                            }
                            if (-not $LinkInline.TriviaAfterTitle.IsEmpty) {
                                $ElementJson.Add('TriviaAfterTitle', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.TriviaAfterTitle.ToString()));
                            }
                            $ElementJson.Add('IsShortcut', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.IsShortcut));
                            $ElementJson.Add('IsAutoLink', [System.Text.Json.Nodes.JsonValue]::Create($LinkInline.IsAutoLink));
                            if ($LinkInline -is [Markdig.Extensions.JiraLinks.JiraLink]) {
                                [Markdig.Extensions.JiraLinks.JiraLink]$JiraLink = $LinkInline;
                                $ElementJson.Add('ProjectKey', [System.Text.Json.Nodes.JsonValue]::Create($JiraLink.ProjectKey.ToString()));
                                $ElementJson.Add('Issue', [System.Text.Json.Nodes.JsonValue]::Create($JiraLink.Issue.ToString()));
                            }
                        }
                    }
                }
                $i = -1;
                for ($c = $ContainerInline.FirstChild; $null -ne $c; $c = $c.NextSibling) {
                    $i++;
                    $JsonObject = Convert-ToJsonObject -MarkdownObject $c -Level $InnerLevel -Index $i;
                    $ChildObjects.Add($JsonObject);
                }
            } else { # $Inline -isnot [Markdig.Syntax.Inlines.ContainerInline]
                if ($Inline -is [Markdig.Syntax.Inlines.LeafInline]) {
                    [Markdig.Syntax.Inlines.LeafInline]$LeafInline =  $Inline;
                    $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($LeafInline.Line));
                    $Position.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($LeafInline.Column));
                    $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($LeafInline.Span.Start));
                    $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($LeafInline.Span.End));
                    if ($LeafInline -is [Markdig.Syntax.Inlines.AutolinkInline]) {
                        [Markdig.Syntax.Inlines.AutolinkInline]$AutolinkInline =  $LeafInline;
                        $ElementJson.Add('Url', [System.Text.Json.Nodes.JsonValue]::Create($AutolinkInline.Url));
                        $ElementJson.Add('IsEmail', [System.Text.Json.Nodes.JsonValue]::Create($AutolinkInline.IsEmail));
                    } else {
                        if ($LeafInline -is [Markdig.Syntax.Inlines.CodeInline]) {
                            [Markdig.Syntax.Inlines.CodeInline]$CodeInline =  $LeafInline;
                            $ElementJson.Add('Delimiter', [System.Text.Json.Nodes.JsonValue]::Create($CodeInline.Delimiter));
                            $ElementJson.Add('DelimiterCount', [System.Text.Json.Nodes.JsonValue]::Create($CodeInline.DelimiterCount));
                            $ElementJson.Add('Content', [System.Text.Json.Nodes.JsonValue]::Create($CodeInline.Content));
                        } else {
                            if ($LeafInline -is [Markdig.Syntax.Inlines.HtmlEntityInline]) {
                                [Markdig.Syntax.Inlines.HtmlEntityInline]$HtmlEntityInline =  $LeafInline;
                                $ElementJson.Add('Content', [System.Text.Json.Nodes.JsonValue]::Create($HtmlEntityInline.Original.ToString()));
                            } else {
                                if ($LeafInline -is [Markdig.Syntax.Inlines.LineBreakInline]) {
                                    [Markdig.Syntax.Inlines.LineBreakInline]$LineBreakInline =  $LeafInline;
                                    $ElementJson.Add('IsHard', [System.Text.Json.Nodes.JsonValue]::Create($LineBreakInline.IsHard));
                                    $ElementJson.Add('IsBackslash', [System.Text.Json.Nodes.JsonValue]::Create($LineBreakInline.IsBackslash));
                                } else {
                                    if ($LeafInline -is [Markdig.Syntax.Inlines.LiteralInline]) {
                                        [Markdig.Syntax.Inlines.LiteralInline]$LiteralInline =  $LeafInline;
                                        $ElementJson.Add('Content', [System.Text.Json.Nodes.JsonValue]::Create($LiteralInline.Content.ToString()));
                                        $ElementJson.Add('IsFirstCharacterEscaped', [System.Text.Json.Nodes.JsonValue]::Create($LiteralInline.IsFirstCharacterEscaped));
                                        if ($LiteralInline -is [Markdig.Extensions.Emoji.EmojiInline]) {
                                            [Markdig.Extensions.Emoji.EmojiInline]$EmojiInline =  $LiteralInline;
                                            if ($null -ne $EmojiInline.Match) {
                                                $ElementJson.Add('Match', [System.Text.Json.Nodes.JsonValue]::Create($EmojiInline.Match));
                                            }
                                        }
                                    } else {
                                        if ($LeafInline -is [Markdig.Extensions.TaskLists.TaskList]) {
                                            [Markdig.Extensions.TaskLists.TaskList]$TaskList =  $LeafInline;
                                            $ElementJson.Add('Checked', [System.Text.Json.Nodes.JsonValue]::Create($TaskList.Checked));
                                        } else {
                                            if ($LeafInline -is [Markdig.Extensions.Mathematics.MathInline]) {
                                                [Markdig.Extensions.Mathematics.MathInline]$MathInline =  $LeafInline;
                                                $ElementJson.Add('Delimiter', [System.Text.Json.Nodes.JsonValue]::Create($MathInline.Delimiter));
                                                $ElementJson.Add('DelimiterCount', [System.Text.Json.Nodes.JsonValue]::Create($MathInline.DelimiterCount));
                                                $ElementJson.Add('Content', [System.Text.Json.Nodes.JsonValue]::Create($MathInline.Content.ToString()));
                                            } else {
                                                if ($LeafInline -is [Markdig.Extensions.Abbreviations.AbbreviationInline]) {
                                                    [Markdig.Extensions.Abbreviations.AbbreviationInline]$AbbreviationInline =  $LeafInline;
                                                    $JsonObject = [System.Text.Json.Nodes.JsonObject]::new();
                                                    $JsonObject.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($AbbreviationInline.Abbreviation.Line));
                                                    $JsonObject.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($AbbreviationInline.Abbreviation.Column));
                                                    if ($null -ne $AbbreviationInline.Abbreviation.Label) {
                                                        $JsonObject.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($AbbreviationInline.Abbreviation.Label));
                                                    }
                                                    if (-not $AbbreviationInline.Abbreviation.Text.IsEmpty) {
                                                        $JsonObject.Add('Text', [System.Text.Json.Nodes.JsonValue]::Create($AbbreviationInline.Abbreviation.Text.ToString()));
                                                    }
                                                    $ElementJson.Add('Abbreviation', $JsonObject);
                                                } else {
                                                    if ($LeafInline -is [Markdig.Extensions.SmartyPants.SmartyPant]) {
                                                        [Markdig.Extensions.SmartyPants.SmartyPant]$SmartyPant =  $LeafInline;
                                                        $ElementJson.Add('OpeningCharacter', [System.Text.Json.Nodes.JsonValue]::Create($SmartyPant.OpeningCharacter));
                                                        $ElementJson.Add('Type', [System.Text.Json.Nodes.JsonValue]::Create($SmartyPant.Type.ToString('F')));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if ($Inline -is [Markdig.Extensions.Footnotes.FootnoteLink]) {
                        [Markdig.Extensions.Footnotes.FootnoteLink]$FootnoteLink =  $Inline;
                        if ($FootnoteLink.Line -gt 0 -or $FootnoteLink.Column -gt 0) {
                            $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.Line));
                            $Position.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.Column));
                        }
                        if (-not $Inline.Span.IsEmpty) {
                            $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($Inline.Span.Start));
                            $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($Inline.Span.End));
                        }
                        $ElementJson.Add('IsBackLink', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.IsBackLink));
                        if ($FootnoteLink.Index -ge 0) {
                            $ElementJson.Add('Index', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.Index));
                        }
                        $JsonObject = [System.Text.Json.Nodes.JsonObject]::new();
                        $JsonObject.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.Footnote.Line));
                        $JsonObject.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.Footnote.Column));
                        if ($null -ne $FootnoteLink.Footnote.Label) {
                            $JsonObject.Add('Label', [System.Text.Json.Nodes.JsonValue]::Create($FootnoteLink.Footnote.Label));
                        }
                        $ElementJson.Add('Footnote', $JsonObject);
                    } else {
                        if (-not $Inline.Span.IsEmpty) {
                            $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($Inline.Line));
                            $Position.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($Inline.Column));
                            $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($Inline.Span.Start));
                            $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($Inline.Span.End));
                        }
                    }
                }
            }
        } else { # $MarkdownObject -isnot [Markdig.Syntax.Inlines.Inline]
            if (-not $MarkdownObject.Span.IsEmpty) {
                $Position.Add('Line', [System.Text.Json.Nodes.JsonValue]::Create($MarkdownObject.Line));
                $Position.Add('Column', [System.Text.Json.Nodes.JsonValue]::Create($MarkdownObject.Column));
                $Position.Add('Start', [System.Text.Json.Nodes.JsonValue]::Create($MarkdownObject.Span.Start));
                $Position.Add('End', [System.Text.Json.Nodes.JsonValue]::Create($MarkdownObject.Span.End));
            }
        }
    }
    if ($ChildObjects.Count -gt 0) {
        $ElementJson.Add('Tokens', $ChildObjects);
    }
    Write-Output -InputObject $ElementJson -NoEnumerate;
}

Function Write-ExampleMarkdownInfo {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [string]$BaseName
    )

    Process {
        $MdPath = $PSScriptRoot | Join-Path -ChildPath "../Resources/$BaseName.md";
        $MarkdownDocument = Open-MarkdownDocument -LiteralPath $MdPath -MarkdownPipeline (New-MarkdownPipeline -UseAdvancedExtensions -UseEmojiAndSmiley -UsePreciseSourceLocation);
        if ($null -ne $MarkdownDocument) {
            $RootJsonObject = [System.Text.Json.Nodes.JsonObject]::new();
            $RootJsonObject.Add("Name", [System.Text.Json.Nodes.JsonValue]::Create($MarkdownDocument.GetType().Name));
            $RootJsonObject.Add("LineCount", [System.Text.Json.Nodes.JsonValue]::Create($MarkdownDocument.LineCount));
            $RootJsonObject.Add("End", [System.Text.Json.Nodes.JsonValue]::Create($MarkdownDocument.Span.End));
            $Tokens = [System.Text.Json.Nodes.JsonArray]::new();
            for ($i = 0; $i -lt $MarkdownDocument.Count; $i++) {
                $JsonObject = Convert-ToJsonObject -MarkdownObject $MarkdownDocument[$i] -Level 1 -Index $i;
                $Tokens.Add($JsonObject);
            }
            $RootJsonObject.Add("Tokens", $Tokens);
            $JsonPath = $PSScriptRoot | Join-Path -ChildPath "$BaseName.json";
            $FileStream = [System.IO.FileStream]::new($JsonPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write);
            try {
                $Writer = [System.Text.Json.Utf8JsonWriter]::new($FileStream, ([System.Text.Json.JsonWriterOptions]@{
                    Encoder = [System.Text.Encodings.Web.JavaScriptEncoder]::UnsafeRelaxedJsonEscaping;
                    Indented = $true;
                }));
                $RootJsonObject.WriteTo($Writer);
                $Writer.Flush();
                $FileStream.Flush();
            } finally { $FileStream.Close() }
            
            $Encoding = [System.Text.UTF8Encoding]::new($false, $false);
            $JsonText = [System.IO.File]::ReadAllText($JsonPath, $Encoding) -replace '\\u0026', '&';
            $JsonText = $JsonText -replace '\\u0027', "'";
            $JsonText = $JsonText -replace '\\u002B', "+";
            $JsonText = $JsonText -replace '\\u0060', '`';
            $JsonText = $JsonText -replace '\\u003C', '<';
            $JsonText = $JsonText -replace '\\u003E', '>';
            # $JsonText = $JsonText -replace '\\u0022', '\\"';
            $JsonText = $JsonText -replace '\{[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null)[\r\n\s]+\}', '{ "$1": $2, "$3": $4, "$5": $6, "$7": $8, "$9": $10, "$11": $12 }';
            $JsonText = $JsonText -replace '\{[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null),[\r\n\s]+"(\w+)":\s*(\d+(?:\.\d+)?|true|false|null)[\r\n\s]+\}', '{ "$1": $2, "$3": $4, "$5": $6, "$7": $8, "$9": $10 }';
            $JsonText = $JsonText -replace '\{[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null),[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null),[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null),[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null)[\r\n\s]*\}', '{ "$1": $2, "$3": $4, "$5": $6, "$7": $8 }';
            $JsonText = $JsonText -replace '\{[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null),[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null),[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null)[\r\n\s]*\}', '{ "$1": $2, "$3": $4, "$5": $6 }';
            $JsonText = $JsonText -replace '\{[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null),[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null)[\r\n\s]*\}', '{ "$1": $2, "$3": $4 }';
            $JsonText = $JsonText -replace '\{[\r\n\s]*"(\w+)":\s*("[^\\"]*"|\d+(?:\.\d+)?|true|false|null)[\r\n\s]*\}', '{ "$1": $2 }';
            [System.IO.File]::WriteAllText($JsonPath, $JsonText, $Encoding);
        }
    }
}

('Example1', 'Example2') | Write-ExampleMarkdownInfo -ErrorAction Stop;

<#
([Markdig.Syntax.Inlines.LiteralInline].Assembly.GetTypes() | ? { [Markdig.Syntax.ParagraphBlock].IsAssignableFrom($_) } | % { $_.Name }) -join '|'
([Markdig.Syntax.Inlines.LiteralInline].Assembly.GetTypes() | ? { [Markdig.Syntax.ParagraphBlock].IsAssignableFrom($_) -or [Markdig.Syntax.Inlines.LeafInline].IsAssignableFrom($_) } | % { $_.Name }) -join '|'
([Markdig.Syntax.Inlines.LiteralInline].Assembly.GetTypes() | ? { [Markdig.Syntax.ContainerBlock].IsAssignableFrom($_) -or [Markdig.Syntax.Inlines.LinkInline].IsAssignableFrom($_) -or [Markdig.Syntax.Inlines.LiteralInline].IsAssignableFrom($_) } | % { $_.Name }) -join '|'
#>
