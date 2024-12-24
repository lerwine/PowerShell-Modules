Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.HtmlUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Testing New-MarkdownPipeline' {
    Context 'New-MarkdownPipeline with no extensions' {
        It '"New-MarkdownPipeline" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 0 -Because 'Extensions.Count';
        }
        
        It '"New-MarkdownPipeline -DoNotOpenJiraLinksInNewWindow" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 0 -Because 'Extensions.Count';
        }
        
        It '"New-MarkdownPipeline -JiraLinkBasePath [string]" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBasePath "/mybase";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 0 -Because 'Extensions.Count';
        }
        
        It '"New-MarkdownPipeline -JiraLinkBasePath [string] -DoNotOpenJiraLinksInNewWindow" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBasePath "/mybase" -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 0 -Because 'Extensions.Count';
        }
    }

    Context 'New-MarkdownPipeline -JiraLinkBaseUrl [string]' {
        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path" -DoNotOpenJiraLinksInNewWindow) returns "<p><a href=`"http://www.example.com/mybase/path/SND-100`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path" -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/mybase/path/SND-100`">SND-100</a></p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path") returns "<p><a href=`"http://www.example.com/mybase/path/SND-100`" target=`"_blank`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/mybase/path/SND-100`" target=`"_blank`">SND-100</a></p>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -DoNotOpenJiraLinksInNewWindow) returns "<p><a href=`"http://www.example.com/browse/SND-100`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/browse/SND-100`">SND-100</a></p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com") returns "<p><a href=`"http://www.example.com/browse/SND-100`" target=`"_blank`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/browse/SND-100`" target=`"_blank`">SND-100</a></p>`n" -Because 'Render';
        }
    }

    Context 'New-MarkdownPipeline -NewLine [string]' {
        It '[Markdig.Markdown]::ToHtml("First`nSecond`n`nThird", (New-MarkdownPipeline -NewLine "`r`n") returns "<p>First`r`nSecond</p>`r`n<p>Third</p>`r`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -NewLine "`r`n";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("First`nSecond`n`nThird", $MarkdownPipeline);
            $Html | Should -Be "<p>First`r`nSecond</p>`r`n<p>Third</p>`r`n" -Because 'Render';
        }
    }

    Context 'New-MarkdownPipeline -UseAdvancedExtensions' {
        It '[Markdig.Markdown]::ToHtml("- [X] H~2~O is ~~not~~ a **liquid**.`n- [ ] 2^10^ is `1024`!", (New-MarkdownPipeline -UseAdvancedExtensions)) returns "<ul class=`"contains-task-list`">`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" checked=`"checked`" /> H<sub>2</sub>O is <del>not</del> a <strong>liquid</strong>.</li>`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" /> 2<sup>10</sup> is 1024!</li>`n</ul>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("- [X] H~2~O is ~~not~~ a **liquid**.`n- [ ] 2^10^ is `1024`!", $MarkdownPipeline);
            $Html | Should -Be "<ul class=`"contains-task-list`">`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" checked=`"checked`" /> H<sub>2</sub>O is <del>not</del> a <strong>liquid</strong>.</li>`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" /> 2<sup>10</sup> is 1024!</li>`n</ul>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("- [X] H~2~O is ~~not~~ a **liquid**.`n- [ ] 2^10^ is `1024`!", (New-MarkdownPipeline -EmphasisExtraOptions Superscript -UseAdvancedExtensions)) returns "<ul class=`"contains-task-list`">`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" checked=`"checked`" /> H~2~O is ~~not~~ a <strong>liquid</strong>.</li>`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" /> 2<sup>10</sup> is 1024!</li>`n</ul>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -EmphasisExtraOptions Superscript -UseAdvancedExtensions;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("- [X] H~2~O is ~~not~~ a **liquid**.`n- [ ] 2^10^ is `1024`!", $MarkdownPipeline);
            $Html | Should -Be"<ul class=`"contains-task-list`">`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" checked=`"checked`" /> H~2~O is ~~not~~ a <strong>liquid</strong>.</li>`n<li class=`"task-list-item`"><input disabled=`"disabled`" type=`"checkbox`" /> 2<sup>10</sup> is 1024!</li>`n</ul>`n" -Because 'Render';
        }

        <#
        foreach ($InputText in (
            [PSCustomObject]@{
                Name = '3 Headers, 3 Last Row, HasHeaderSeparator'
                Text = "| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** | ~~not~~ |"
            },
            [PSCustomObject]@{
                Name = '3 Headers, 2 Last Row, HasHeaderSeparator'
                Text = "| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** |"
            },
            [PSCustomObject]@{
                Name = '2 Headers, 3 Last Row, HasHeaderSeparator'
                Text = "| Symbol | Value |`n| ------ | ----- |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** | ~~not~~ |"
            },
            [PSCustomObject]@{
                Name = '3 Headers, 3 Last Row, NoHeaderSeparator'
                Text = "| Symbol | Value | Notes |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** | ~~not~~ |"
            },
            [PSCustomObject]@{
                Name = '3 Headers, 2 Last Row, NoHeaderSeparator'
                Text = "| Symbol | Value | Notes |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** |"
            },
            [PSCustomObject]@{
                Name = '2 Headers, 3 Last Row, NoHeaderSeparator'
                Text = "| Symbol | Value |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** | ~~not~~ |"
            }
        )) {
            $InputEncoded = "`"$($InputText.Text.Replace("`n", '`n'))`"";
            '';
            "        # $($InputText.Name)";
            foreach ($RequireHeaderSeparator in ($false, $true)) {
                foreach ($UseHeaderForColumnCount in ($true, $false)) {
                    $Cmd = 'New-MarkdownPipeline -UseAdvancedExtensions';
                    if ($RequireHeaderSeparator) {
                        if ($UseHeaderForColumnCount) {
                            $Cmd =  "$Cmd -UseHeaderForPipeTableColumnCount";
                        }
                    } else {
                        if ($UseHeaderForColumnCount) {
                            $Cmd =  "$Cmd -PipeTablesDoNotRequireHeaderSeparator -UseHeaderForPipeTableColumnCount";
                        } else {
                            $Cmd =  "$Cmd -PipeTablesDoNotRequireHeaderSeparator";
                        }
                    }
                    $PipeTableOptions = [Markdig.Extensions.Tables.PipeTableOptions]@{ RequireHeaderSeparator = $RequireHeaderSeparator; UseHeaderForColumnCount = $UseHeaderForColumnCount };
                    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseAdvancedExtensions([Markdig.MarkdownExtensions]::UsePipeTables([Markdig.MarkdownPipelineBuilder]::new(), $PipeTableOptions)).Build()
                    $Html = ((([Markdig.Markdown]::ToHtml($InputText.Text, $MarkdownPipeline)) | ConvertTo-Json -EscapeHandling EscapeNonAscii) -replace '\\n', '`n') -replace '\\"', '`"';
                    "        It '[Markdig.Markdown]::ToHtml($InputEncoded, ($Cmd) return $Html' {";
                    "            `$Html = [Markdig.Markdown]::ToHtml($InputEncoded, `$MarkdownPipeline);";
                    "            `$Html | Should -Be $Html -Because 'Render';";
                    "        }"
                }
            }
        }

        (([Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10^  | `1024`! |`n| H~2~O  | **liquid** | ~~not~~ |", $MarkdownPipeline) | ConvertTo-Json -EscapeHandling EscapeNonAscii) -replace '\\n', '`n') -replace '\\"', '`"';
        #>
        
        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** |", (New-MarkdownPipeline -UseAdvancedExtensions) return "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n<th>Notes</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n<th>Notes</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", (New-MarkdownPipeline -UseAdvancedExtensions) return "<p>| Symbol | Value | Notes |`n| 2<sup>10</sup>  | 1024! |`n| H<sub>2</sub>O  | <strong>liquid</strong> | <del>not</del> |</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", $MarkdownPipeline);
            $Html | Should -Be "<p>| Symbol | Value | Notes |`n| 2<sup>10</sup>  | 1024! |`n| H<sub>2</sub>O  | <strong>liquid</strong> | <del>not</del> |</p>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value |`n| ------ | ----- |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", (New-MarkdownPipeline -UseAdvancedExtensions -UseHeaderForPipeTableColumnCount) return "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions -UseHeaderForPipeTableColumnCount;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value |`n| ------ | ----- |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** |", (New-MarkdownPipeline -UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", (New-MarkdownPipeline -UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td></td>`n</tr>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td><del>not</del></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td></td>`n</tr>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td><del>not</del></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", (New-MarkdownPipeline -UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td><del>not</del></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 18 -Because 'Extensions.Count';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Abbreviations.AbbreviationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AbbreviationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoIdentifierExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Citations.CitationExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CitationExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.CustomContainers.CustomContainerExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'CustomContainerExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.DefinitionLists.DefinitionListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DefinitionListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'EmphasisExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Figures.FigureExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FigureExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footers.FooterExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FooterExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Footnotes.FootnoteExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'FootnoteExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.GridTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GridTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Mathematics.MathExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MathExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.MediaLinks.MediaLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'MediaLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Tables.PipeTableExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'PipeTableExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.ListExtras.ListExtraExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'ListExtraExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.TaskLists.TaskListExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'TaskListExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.AutoLinks.AutoLinkExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'AutoLinkExtension';
            (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.GenericAttributes.GenericAttributesExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'GenericAttributesExtension';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10^  | 1024! |`n| H~2~O  | **liquid** | ~~not~~ |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2<sup>10</sup></td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H<sub>2</sub>O</td>`n<td><strong>liquid</strong></td>`n<td><del>not</del></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseAlertBlocks([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseAlertBlocks' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseAutoLinks([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseAutoLinks' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseTaskLists([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseTaskLists' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseCustomContainers([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseCustomContainers' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseMediaLinks([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseMediaLinks' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseAutoIdentifiers([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseAutoIdentifiers' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseMathematics([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseMathematics' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseFigures([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseFigures' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseAbbreviations([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseAbbreviations' {
    }
    
    <#
    $MarkdownPipeline = [Markdig.MarkdownExtensions]::UseDefinitionLists([Markdig.MarkdownPipelineBuilder]::new()).Build();
    ([Markdig.Markdown]::ToHtml("", $MarkdownPipeline) | ConvertTo-Json).Replace("\n", "`n").Replace('\"', '`"');
    #>
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
    Context 'New-MarkdownPipeline -UseDefinitionLists' {
    }
    
    Context 'New-MarkdownPipeline -UsePipeTables' {
        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10  | 1024! |`n| H20  | **liquid** |", (New-MarkdownPipeline -UsePipeTables) return "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n<th>Notes</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePipeTables;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| ------ | ----- | ----- |`n| 2^10  | 1024! |`n| H20  | **liquid** |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n<th>Notes</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -UsePipeTables) return "<p>| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | <strong>liquid</strong> | <em>not</em> |</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePipeTables;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<p>| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | <strong>liquid</strong> | <em>not</em> |</p>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value |`n| ------ | ----- |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -UsePipeTables -UseHeaderForPipeTableColumnCount) return "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePipeTables -UseHeaderForPipeTableColumnCount;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value |`n| ------ | ----- |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value |`n| ------ | ----- |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -UseHeaderForPipeTableColumnCount) return "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseHeaderForPipeTableColumnCount;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value |`n| ------ | ----- |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<thead>`n<tr>`n<th>Symbol</th>`n<th>Value</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** |", (New-MarkdownPipeline -UsePipeTables -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePipeTables -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** |", (New-MarkdownPipeline  -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -UsePipeTables -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td></td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePipeTables -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td></td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td></td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td></td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -UsePipeTables -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePipeTables -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", (New-MarkdownPipeline -PipeTablesDoNotRequireHeaderSeparator) return "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -PipeTablesDoNotRequireHeaderSeparator;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("| Symbol | Value | Notes |`n| 2^10  | 1024! |`n| H20  | **liquid** | *not* |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<tbody>`n<tr>`n<td>Symbol</td>`n<td>Value</td>`n<td>Notes</td>`n</tr>`n<tr>`n<td>2^10</td>`n<td>1024!</td>`n<td></td>`n</tr>`n<tr>`n<td>H20</td>`n<td><strong>liquid</strong></td>`n<td><em>not</em></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UseGridTables' {
        It '[Markdig.Markdown]::ToHtml("+--------+--------+`n| Header | Header |`n| Col 1  | Col 2  |`n+========+========+`n| Cell   | Cell`n| Number | Two`n| One    |`n+--------+--------+`n| Second row spanning`n| on two columns`n+--------+--------+`n| Back   |        |`n| to One |        |", (New-MarkdownPipeline -UseGridTables)  returns "<table>`n<col style=`"width:50%`" />`n<col style=`"width:50%`" />`n<thead>`n<tr>`n<th>Header`nCol 1</th>`n<th>Header`nCol 2</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>Cell`nNumber`nOne</td>`n<td>Cell`nTwo</td>`n</tr>`n<tr>`n<td colspan=`"2`">Second row spanning`non two columns</td>`n</tr>`n<tr>`n<td>Back`nto One</td>`n<td></td>`n</tr>`n</tbody>`n</table>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseGridTables;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.GridTableExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("+--------+--------+`n| Header | Header |`n| Col 1  | Col 2  |`n+========+========+`n| Cell   | Cell`n| Number | Two`n| One    |`n+--------+--------+`n| Second row spanning`n| on two columns`n+--------+--------+`n| Back   |        |`n| to One |        |", $MarkdownPipeline);
            $Html | Should -Be "<table>`n<col style=`"width:50%`" />`n<col style=`"width:50%`" />`n<thead>`n<tr>`n<th>Header`nCol 1</th>`n<th>Header`nCol 2</th>`n</tr>`n</thead>`n<tbody>`n<tr>`n<td>Cell`nNumber`nOne</td>`n<td>Cell`nTwo</td>`n</tr>`n<tr>`n<td colspan=`"2`">Second row spanning`non two columns</td>`n</tr>`n<tr>`n<td>Back`nto One</td>`n<td></td>`n</tr>`n</tbody>`n</table>`n" -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UseCitations' {
    }
    
    Context 'New-MarkdownPipeline -UseFooters' {
    }
    
    Context 'New-MarkdownPipeline -UseFootnotes' {
    }
    
    Context 'New-MarkdownPipeline -UseEmphasisExtras' {
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -UseEmphasisExtras)) returns "<p>H<sub>2</sub>O is <del>not</del> a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseEmphasisExtras;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H<sub>2</sub>O is <del>not</del> a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript, Marked)) returns "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -EmphasisExtraOptions Superscript, Marked)) returns "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript)) returns "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>"`n' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -EmphasisExtraOptions Superscript)) returns "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions.Count | Should -Be 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UseListExtras' {
    }
    
    Context 'New-MarkdownPipeline -UseGenericAttributes' {
    }
    
    Context 'New-MarkdownPipeline -UseNonAsciiNoEscape' {
    }
    
    Context 'New-MarkdownPipeline -UseYamlFrontMatter' {
    }
    
    Context 'New-MarkdownPipeline -UsePragmaLines' {
    }
    
    Context 'New-MarkdownPipeline -UsePreciseSourceLocation' {
    }
    
    Context 'New-MarkdownPipeline -UseSmartyPants' {
    }
    
    Context 'New-MarkdownPipeline -UseBootstrap' {
    }
    
    Context 'New-MarkdownPipeline -UseSoftlineBreakAsHardlineBreak' {
    }
    
    Context 'New-MarkdownPipeline -UseEmoji -UseEmojiAndSmiley' {
    }
    
    Context 'New-MarkdownPipeline -UseEmoji' {
    }
    
    Context 'New-MarkdownPipeline -UseEmojiAndSmiley' {
    }
    
    Context 'New-MarkdownPipeline -UseNoFollowLinks -UseNoOpenerLinks -UseNoReferrerLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseNoFollowLinks -UseNoOpenerLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseNoFollowLinks -UseNoReferrerLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseNoOpenerLinks -UseNoReferrerLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseNoFollowLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseNoOpenerLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseNoReferrerLinks' {
    }
    
    Context 'New-MarkdownPipeline -UseGlobalization' {
    }
    
    Context 'New-MarkdownPipeline -DisableHtml' {
    }
    
    Context 'New-MarkdownPipeline -DisableHeadings' {
    }
    
    Context 'New-MarkdownPipeline -EnableTrackTrivia' {
    }
}