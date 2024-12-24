Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.HtmlUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Testing New-MarkdownPipeline' {
    Context 'New-MarkdownPipeline with no extensions' {
        It '"New-MarkdownPipeline" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
        }
        
        It '"New-MarkdownPipeline -DoNotOpenJiraLinksInNewWindow" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
        }
        
        It '"New-MarkdownPipeline -JiraLinkBasePath [string]" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBasePath "/mybase";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
        }
        
        It '"New-MarkdownPipeline -JiraLinkBasePath [string] -DoNotOpenJiraLinksInNewWindow" returns no extensions' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBasePath "/mybase" -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
        }
    }

    Context 'New-MarkdownPipeline -JiraLinkBaseUrl [string]' {
        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path" -DoNotOpenJiraLinksInNewWindow) returns "<p><a href=`"http://www.example.com/mybase/path/SND-100`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path" -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/mybase/path/SND-100`">SND-100</a></p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path") returns "<p><a href=`"http://www.example.com/mybase/path/SND-100`" target=`"_blank`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -JiraLinkBasePath "/mybase/path";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/mybase/path/SND-100`" target=`"_blank`">SND-100</a></p>`n" -Because 'Render';
        }

        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -DoNotOpenJiraLinksInNewWindow) returns "<p><a href=`"http://www.example.com/browse/SND-100`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com" -DoNotOpenJiraLinksInNewWindow;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/browse/SND-100`">SND-100</a></p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("SND-100", (New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com") returns "<p><a href=`"http://www.example.com/browse/SND-100`" target=`"_blank`">SND-100</a></p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -JiraLinkBaseUrl "http://www.example.com";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('SND-100', $MarkdownPipeline);
            $Html | Should -Be "<p><a href=`"http://www.example.com/browse/SND-100`" target=`"_blank`">SND-100</a></p>`n" -Because 'Render';
        }
    }

    Context 'New-MarkdownPipeline -NewLine [string]' {
        It '[Markdig.Markdown]::ToHtml("First`r`nSecond`r`n`r`nThird", (New-MarkdownPipeline -NewLine "`n") returns "<p>First`nSecond</p>`n<p>Third</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -NewLine "`n";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("First`r`nSecond`r`n`r`nThird", $MarkdownPipeline);
            $Html | Should -Be "<p>First`nSecond</p>`n<p>Third</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("First`nSecond`n`nThird", (New-MarkdownPipeline -NewLine "`r`n") returns "<p>First`r`nSecond</p>`r`n<p>Third</p>`r`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -NewLine "`r`n";
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.JiraLinks.JiraLinkExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml("First`nSecond`n`nThird", $MarkdownPipeline);
            $Html | Should -Be "<p>First`r`nSecond</p>`r`n<p>Third</p>`r`n" -Because 'Render';
        }
    }

    Context 'New-MarkdownPipeline -UseAdvancedExtensions' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline <Options>)) returns <Expected>' -ForEach @(
            @{
                Splat = @{ UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
- [X] H~2~O is ~~not~~ a **liquid**.
- [ ] 2^10^ is `1024`!
"@;
                Options = 'EmphasisExtraOptions Superscript, Inserted -UseAdvancedExtensions';
                Expected = @"
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> H<sub>2</sub>O is <del>not</del> a <strong>liquid</strong>.</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> 2<sup>10</sup> is 1024!</li>
</ul>

"@;
            }, @{
                Splat = @{
                    UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present;
                    EmphasisExtraOptions = [Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions[]]@('Superscript', 'Strikethrough')
                };
                Text = @"
- [X] H~2~O is ~~not~~ a **liquid**.
- [ ] 2^10^ is `1024`!
"@;
                Options = 'EmphasisExtraOptions Superscript, Inserted -UseAdvancedExtensions';
                Expected = @"
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> H~2~O is <del>not</del> a <strong>liquid</strong>.</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> 2<sup>10</sup> is 1024!</li>
</ul>

"@;
            }, @{
                Splat = @{
                    UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present;
                    EmphasisExtraOptions = [Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions]::Superscript
                };
                Text = @"
"@;
                Options = 'EmphasisExtraOptions Superscript, Inserted -UseAdvancedExtensions';
                Expected = @"
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> H~2~O is ~~not~~ a <strong>liquid</strong>.</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> 2<sup>10</sup> is 1024!</li>
</ul>

"@;
            }, @{
                Splat = @{ UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| ------ | ----- | ----- |
| 2^10^  | 1024! |
| H~2~O  | **liquid** |
"@;
                Options = '-UseAdvancedExtensions';
                Expected = @"
<table>
<thead>
<tr>
<th>Symbol</th>
<th>Value</th>
<th>Notes</th>
</tr>
</thead>
<tbody>
<tr>
<td>2<sup>10</sup></td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H<sub>2</sub>O</td>
<td><strong>liquid</strong></td>
<td></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10^  | 1024! |
| H~2~O  | **liquid** | ~~not~~ |
"@;
                Options = '-UseAdvancedExtensions';
                Expected = @"
<p>| Symbol | Value | Notes |
| 2<sup>10</sup>  | 1024! |
| H<sub>2</sub>O  | <strong>liquid</strong> | <del>not</del> |</p>
"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present; UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value |
| ------ | ----- |
| 2^10^  | 1024! |
| H~2~O  | **liquid** | ~~not~~ |
"@;
                Options = '-UseAdvancedExtensions -UseHeaderForPipeTableColumnCount';
                Expected = @"
<table>
<thead>
<tr>
<th>Symbol</th>
<th>Value</th>
</tr>
</thead>
<tbody>
<tr>
<td>2<sup>10</sup></td>
<td>1024!</td>
</tr>
<tr>
<td>H<sub>2</sub>O</td>
<td><strong>liquid</strong></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ PipeTablesDoNotRequireHeaderSeparator = [System.Management.Automation.SwitchParameter]::Present; UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10^  | 1024! |
| H~2~O  | **liquid** |
"@;
                Options = '-UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td>Notes</td>
</tr>
<tr>
<td>2<sup>10</sup></td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H<sub>2</sub>O</td>
<td><strong>liquid</strong></td>
<td></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ PipeTablesDoNotRequireHeaderSeparator = [System.Management.Automation.SwitchParameter]::Present; UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value |
| 2^10^  | 1024! |
| H~2~O  | **liquid** | ~~not~~ |
"@;
                Options = '-UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td></td>
</tr>
<tr>
<td>2<sup>10</sup></td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H<sub>2</sub>O</td>
<td><strong>liquid</strong></td>
<td><del>not</del></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ PipeTablesDoNotRequireHeaderSeparator = [System.Management.Automation.SwitchParameter]::Present; UseAdvancedExtensions = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10^  | 1024! |
| H~2~O  | **liquid** | ~~not~~ |
"@;
                Options = '-UseAdvancedExtensions -PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td>Notes</td>
</tr>
<tr>
<td>2<sup>10</sup></td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H<sub>2</sub>O</td>
<td><strong>liquid</strong></td>
<td><del>not</del></td>
</tr>
</tbody>
</table>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 18 -Because 'Extensions.Count';
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
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Text -Because 'Render';
        }
    }
    
#     # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AlertBlockSpecs.md
#     Context 'New-MarkdownPipeline -UseAlertBlocks' {
#         It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseAlertBlocks)) returns <Expected>' -ForEach @(
#             @{
#                 Text = @"
# "@;
#                 Expected = @"
# "@;
#             }, @{
#                 Text = @"
# "@;
#                 Expected = @"
# "@;
#             }
#         ) {
#             [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAlertBlocks;
#             $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
#             $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
#             $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Alerts.AlertExtension]) -Because 'Extensions[0].GetType()';
#         }
#     }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoLinks.md
    Context 'New-MarkdownPipeline -UseAutoLinks' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseAutoLinks)) returns <Expected>' -ForEach @(
            @{
                Text = "http://www.google.com";
                Expected = "<p><a href=`"http://www.google.com`">http://www.google.com</a></p>`n";
            }, @{
                Text = "ftp://test.com";
                Expected = "<p><a href=`"ftp://test.com`">ftp://test.com</a></p>`n";
            }, @{
                Text = "mailto:email@toto.com";
                Expected = "<p><a href=`"mailto:email@toto.com`">email@toto.com</a></p>`n";
            }, @{
                Text = "tel:+1555123456";
                Expected = "<p><a href=`"tel:+1555123456`">+1555123456</a></p>`n";
            }, @{
                Text = "www.google.com";
                Expected = "<p><a href=`"http://www.google.com`">www.google.com</a></p>`n";
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAutoLinks;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.AutoLinks.AutoLinkExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/TaskListSpecs.md
    Context 'New-MarkdownPipeline -UseTaskLists' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseTaskLists)) returns <Expected>' -ForEach @(
            @{
                Text = @"
- [ ] Item1
- [x] Item2
- [ ] Item3
- Item4
"@;
                Expected = @"
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item1</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Item2</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item3</li>
<li>Item4</li>
</ul>

"@;
            }, @{
                Text = @"
1. [x] Item1
    - [ ] Item2
"@;
                Expected = @"
<ol class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Item1
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item2</li>
</ul>
</li>
</ol>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseTaskLists;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.TaskLists.TaskListExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';  
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/CustomContainerSpecs.md
    Context 'New-MarkdownPipeline -UseCustomContainers' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline <Options>)) returns <Expected>' -ForEach @(
            @{
                Text = @"
:::
This is a regular div
:::
"@;
                Options = '-UseCustomContainers';
                Splat = @{ UseCustomContainers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<div><p>This is a regular div</p>
</div>

"@;
            }, @{
                Text = @"
:::spoiler {#myspoiler myprop=yes}
With class ID and custom attrib
:::
"@;
                Options = '-UseCustomContainers -UseGenericAttributes';
                Splat = @{ UseGenericAttributes = [System.Management.Automation.SwitchParameter]::Present;  UseCustomContainers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<div id="myspoiler" class="spoiler" myprop="yes"><p>With class ID and custom attrib</p>
</div>

"@;
            }, @{
                Text = @"
:::mycontainer
Just with class
:::
"@;
                Options = '-UseCustomContainers';
                Splat = @{ UseCustomContainers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<div class="mycontainer"><p>Just with class</p>
</div>

"@;
            }, @{
                Text = @"
Text ::with special emphasis::
"@;
                Expected = @"
<p>Text <span>with special emphasis</span></p>

"@;
            }, @{
                Text = @"
Text ::with special emphasis::{#myId .myemphasis}
"@;
                Options = '-UseCustomContainers';
                Splat = @{ UseCustomContainers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<p>Text <span>with special emphasis</span>{#myId .myemphasis}</p>

"@;
            }, @{
                Text = @"
Text ::with special emphasis::{#myId .myemphasis}
"@;
                Options = '-UseCustomContainers -UseGenericAttributes';
                Splat = @{ UseGenericAttributes = [System.Management.Automation.SwitchParameter]::Present;  UseCustomContainers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<p>Text <span id="myId" class="myemphasis">with special emphasis</span></p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.CustomContainers.CustomContainerExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/MediaSpecs.md
    Context 'New-MarkdownPipeline -UseMediaLinks' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseMediaLinks)) returns <Expected>' -ForEach @(
            @{
                Text = "![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)";
                Expected = @"
<p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![youtube.com with t](https://www.youtube.com/watch?v=mswPy5bt3TQ&t=100)";
                Expected = @"
<p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ?start=100" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![youtu.be](https://youtu.be/mswPy5bt3TQ)";
                Expected = @"
<p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![youtu.be with t](https://youtu.be/mswPy5bt3TQ?t=100)";
                Expected = @"
<p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ?start=100" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![youtube.com/embed 1](https://www.youtube.com/embed/mswPy5bt3TQ?start=100&rel=0)";
                Expected = @"
<p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ?start=100&amp;rel=0" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![youtube.com/embed 2](https://www.youtube.com/embed?listType=playlist&list=PLC77007E23FF423C6)";
                Expected = @"
<p><iframe src="https://www.youtube.com/embed?listType=playlist&amp;list=PLC77007E23FF423C6" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![vimeo](https://vimeo.com/8607834)";
                Expected = @"
<p><iframe src="https://player.vimeo.com/video/8607834" class="vimeo" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }, @{
                Text = "![static mp4](https://sample.com/video.mp4)";
                Expected = @"
<p><video width="500" height="281" controls=""><source type="video/mp4" src="https://sample.com/video.mp4"></source></video></p>

"@;
            }, @{
                Text = "![yandex.ru](https://music.yandex.ru/album/411845/track/4402274)";
                Expected = @"
<p><iframe src="https://music.yandex.ru/iframe/#track/4402274/411845/" class="yandex" width="500" height="281" frameborder="0"></iframe></p>

"@;
            }, @{
                Text = "![ok.ru](https://ok.ru/video/26870090463)";
                Expected = @"
<p><iframe src="https://ok.ru/videoembed/26870090463" class="odnoklassniki" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseMediaLinks;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.MediaLinks.MediaLinkExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';   
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoIdentifierSpecs.md
    Context 'New-MarkdownPipeline -UseAutoIdentifiers' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline <Options>)) returns <Expected>' -ForEach @(
            @{
                Text = "# This is a heading";
                Options = '-UseAutoIdentifiers';
                Splat = @{ UseAutoIdentifiers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<h1 id="this-is-a-heading">This is a heading</h1>

"@;
            }, @{
                Text = "## This is a heading {#other-id}";
                Options = '-UseAutoIdentifiers';
                Splat = @{ UseGenericAttributes = [System.Management.Automation.SwitchParameter]::Present; UseAutoIdentifiers = [System.Management.Automation.SwitchParameter]::Present };
                Expected = @"
<h2 id="other-id">This is a heading</h2>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render'; 
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/MathSpecs.md
    Context 'New-MarkdownPipeline -UseMathematics' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseMathematics)) returns <Expected>' -ForEach @(
            @{
                Text = '$\sqrt{3x-1}+(1+x)^2$';
                Expected = @"
<p><span class="math">\(\sqrt{3x-1}+(1+x)^2\)</span></p>

"@;
            }, @{
                Text = @'
$$
\sqrt{3x-1}
+
(1+x)^2
$$
'@
                Expected = @"
<div class="math">
\[
\sqrt{3x-1}
+
(1+x)^2
\]</div>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseMathematics;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Mathematics.MathExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md
    Context 'New-MarkdownPipeline -UseFigures' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseFigures)) returns <Expected>' -ForEach @(
            @{
                Text = @"
^^^
This is a figure
^^^ This is a caption
"@;
                Expected = @"
<figure>
<p>This is a figure</p>
<figcaption>This is a caption</figcaption>
</figure>

"@;
            }, @{
                Text = @"
^^^
![alt attribute goes here](./sn-logo.jpg "This is a Title")
^^^ This is a caption
"@;
                Expected = @"
<figure>
<p><img src="./sn-logo.jpg" alt="alt attribute goes here" title="This is a Title" /></p>
<figcaption>This is a caption</figcaption>
</figure>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseFigures;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Figures.FigureExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AbbreviationSpecs.md
    Context 'New-MarkdownPipeline -UseAbbreviations' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseAbbreviations)) returns <Expected>' -ForEach @(
            @{
                Text = "
*[HTML]: Hypertext Markup Language

Example tag HTML
";
                Expected = @"
<p>Example tag <abbr title="Hypertext Markup Language">HTML</abbr></p>

"@;
            }, @{
                Text = @"
*[API]: Application Programming Interface
*[JSON]: JavaScript Object Notation

Example tag API and JSON.
"@;
                Expected = @"
<p>Example tag <abbr title="Application Programming Interface">API</abbr> and <abbr title="JavaScript Object Notation">JSON</abbr>.</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseAbbreviations;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Abbreviations.AbbreviationExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/DefinitionListSpecs.md
    Context 'New-MarkdownPipeline -UseDefinitionLists' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseDefinitionLists)) returns <Expected>' -ForEach @(
            @{
                Text = "Term 1`n   : This is a definition item`n     `n     With a paragraph`n   : This is another definition item`n`nTerm2`nTerm3 *with some inline*`n:   This is another definition for term2";
                Expected = @"
<dl>
<dt>Term 1</dt>
<dd><p>This is a definition item</p>
<p>With a paragraph</p>
</dd>
<dd>This is another definition item</dd>
<dt>Term2</dt>
<dt>Term3 <em>with some inline</em></dt>
<dd>This is another definition for term2</dd>
</dl>

"@;
            }, @{
                Text = "1. First`n2. Second`n    `n    Term 1`n       : Definition`n    `n    Term 2`n       : Second Definition";
                Expected = @"
<ol>
<li>First</li>
<li>Second
<dl>
<dt>Term 1</dt>
<dd>Definition</dd>
<dt>Term 2</dt>
<dd>Second Definition</dd>
</dl>
</li>
</ol>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseDefinitionLists;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.DefinitionLists.DefinitionListExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UsePipeTables' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline <Options>)) returns <Expected>' -ForEach @(
            @{
                Splat = @{ UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
 a     | b       | c
:------|:-------:| ----:
 0     | 1       | 2
 3     | 4       | 5
"@;
                Options = '-UsePipeTables';
                Expected = @"
<table>
<thead>
<tr>
<th style="text-align: left;">a</th>
<th style="text-align: center;">b</th>
<th style="text-align: right;">c</th>
</tr>
</thead>
<tbody>
<tr>
<td style="text-align: left;">0</td>
<td style="text-align: center;">1</td>
<td style="text-align: right;">2</td>
</tr>
<tr>
<td style="text-align: left;">3</td>
<td style="text-align: center;">4</td>
<td style="text-align: right;">5</td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| ------ | ----- | ----- |
| 2^10  | 1024! |
| H20  | **liquid** |
"@;
                Options = '-UsePipeTables';
                Expected = @"
<table>
<thead>
<tr>
<th>Symbol</th>
<th>Value</th>
<th>Notes</th>
</tr>
</thead>
<tbody>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-UsePipeTables';
                Expected = @"
<p>| Symbol | Value | Notes |
| 2^10  | 1024! |
| H20  | <strong>liquid</strong> | <em>not</em> |</p>

"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present; UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value |
| ------ | ----- |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-UsePipeTables -UseHeaderForPipeTableColumnCount';
                Expected = @"
<table>
<thead>
<tr>
<th>Symbol</th>
<th>Value</th>
</tr>
</thead>
<tbody>
<tr>
<td>2^10</td>
<td>1024!</td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value |
| ------ | ----- |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-UseHeaderForPipeTableColumnCount';
                Expected = @"
<table>
<thead>
<tr>
<th>Symbol</th>
<th>Value</th>
</tr>
</thead>
<tbody>
<tr>
<td>2^10</td>
<td>1024!</td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ PipeTablesDoNotRequireHeaderSeparator = [System.Management.Automation.SwitchParameter]::Present; UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10  | 1024! |
| H20  | **liquid** |
"@;
                Options = '-UsePipeTables -PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td>Notes</td>
</tr>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ PipeTablesDoNotRequireHeaderSeparator = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10  | 1024! |
| H20  | **liquid** |
"@;
                Options = '-PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td>Notes</td>
</tr>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present; UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-UsePipeTables -PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td></td>
</tr>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td><em>not</em></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td></td>
</tr>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td><em>not</em></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present; UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-UsePipeTables -PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td>Notes</td>
</tr>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td><em>not</em></td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UseHeaderForPipeTableColumnCount = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
| Symbol | Value | Notes |
| 2^10  | 1024! |
| H20  | **liquid** | *not* |
"@;
                Options = '-PipeTablesDoNotRequireHeaderSeparator';
                Expected = @"
<table>
<tbody>
<tr>
<td>Symbol</td>
<td>Value</td>
<td>Notes</td>
</tr>
<tr>
<td>2^10</td>
<td>1024!</td>
<td></td>
</tr>
<tr>
<td>H20</td>
<td><strong>liquid</strong></td>
<td><em>not</em></td>
</tr>
</tbody>
</table>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.PipeTableExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UseGridTables' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UsePipeTables)) returns <Expected>' -ForEach @(
            @{
                Splat = @{ UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
+---------+---------+
| Header  | Header  |
| Column1 | Column2 |
+=========+=========+
| Cell    | Cell 2
| One     |
+---------+---------+
| Second row spanning
| on two columns
+---------+---------+
| Spans   | NoSpan A
+ 2 Cols  +---------+
|         | NoSpan B
+---------+---------+
"@;
                Expected = @"
<table>
<col style="width:50%" />
<col style="width:50%" />
<thead>
<tr>
<th>Header
Column1</th>
<th>Header
Column2</th>
</tr>
</thead>
<tbody>
<tr>
<td>Cell
One</td>
<td>Cell 2</td>
</tr>
<tr>
<td colspan="2">Second row spanning
on two columns</td>
</tr>
<tr>
<td rowspan="2">Spans
2 Cols</td>
<td>NoSpan A</td>
</tr>
<tr>
<td>NoSpan B</td>
</tr>
</tbody>
</table>

"@;
            }, @{
                Splat = @{ UsePipeTables = [System.Management.Automation.SwitchParameter]::Present };
                Text = @"
+-----+:---:+-----+
|  A  |  B  |  C  |
+-----+-----+-----+
"@;
                Expected = @"
<table>
<col style="width:33.33%" />
<col style="width:33.33%" />
<col style="width:33.33%" />
<tbody>
<tr>
<td>A</td>
<td style="text-align: center;">B</td>
<td>C</td>
</tr>
</tbody>
</table>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Tables.GridTableExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md
    Context 'New-MarkdownPipeline -UseCitations' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseCitations)) returns <Expected>' -ForEach @(
            @{
                Text = 'This is a ""citation of someone""';
                Expected = @"
<p>This is a <cite>citation of someone</cite></p>

"@;
            }, @{
                Text = '""citation with *inline*""';
                Expected = @"
<p><cite>citation with <em>inline</em></cite></p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseCitations;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Citations.CitationExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md
    Context 'New-MarkdownPipeline -UseFooters' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseFooters)) returns <Expected>' -ForEach @(
            @{
                Text = "^^ This is a footer";
                Expected = @"
<footer>This is a footer</footer>

"@;
            }, @{
                Text = @"
^^ This is a footer
^^ multi-line
"@;
                Expected = @"
<footer>This is a footer
multi-line</footer>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseFooters;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Footers.FooterExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/FootnotesSpecs.md
    Context 'New-MarkdownPipeline -UseFootnotes' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseFootnotes)) returns <Expected>' -ForEach @(
            @{
                Text = @"
Here is a footnote[^1]. And another one[^2].

[^1]: Footnote 1 text
[^2]: Footnote 2 text
"@;
                Expected = @"
<p>Here is a footnote<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a>. And another one<a id="fnref:2" href="#fn:2" class="footnote-ref"><sup>2</sup></a>.</p>
<div class="footnotes">
<hr />
<ol>
<li id="fn:1">
<p>Footnote 1 text<a href="#fnref:1" class="footnote-back-ref">&#8617;</a></p>
</li>
<li id="fn:2">
<p>Footnote 2 text<a href="#fnref:2" class="footnote-back-ref">&#8617;</a></p>
</li>
</ol>
</div>

"@;
            }, @{
                Text = @"
Here is a footnote reference,[^1] and another.[^longnote]

This is another reference to [^1]

[^1]: Here is the footnote.

And another reference to [^longnote]

[^longnote]: Here's one with multiple blocks.

    Subsequent paragraphs are indented to show that they
belong to the previous footnote.
"@;
                Expected = @"
<p>Here is a footnote reference,<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a> and another.<a id="fnref:3" href="#fn:2" class="footnote-ref"><sup>2</sup></a></p>
<p>This is another reference to <a id="fnref:2" href="#fn:1" class="footnote-ref"><sup>1</sup></a></p>
<p>And another reference to <a id="fnref:4" href="#fn:2" class="footnote-ref"><sup>2</sup></a></p>
<div class="footnotes">
<hr />
<ol>
<li id="fn:1">
<p>Here is the footnote.<a href="#fnref:1" class="footnote-back-ref">&#8617;</a><a href="#fnref:2" class="footnote-back-ref">&#8617;</a></p>
</li>
<li id="fn:2">
<p>Here's one with multiple blocks.</p>
<p>Subsequent paragraphs are indented to show that they
belong to the previous footnote.<a href="#fnref:3" class="footnote-back-ref">&#8617;</a><a href="#fnref:4" class="footnote-back-ref">&#8617;</a></p>
</li>
</ol>
</div>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseFootnotes;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Footnotes.FootnoteExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UseEmphasisExtras' {
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -UseEmphasisExtras)) returns "<p>H<sub>2</sub>O is <del>not</del> a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseEmphasisExtras;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H<sub>2</sub>O is <del>not</del> a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript, Marked)) returns "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -EmphasisExtraOptions Superscript, Marked)) returns "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a <mark>liquid</mark>. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript)) returns "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>"`n' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseEmphasisExtras -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
        
        It '[Markdig.Markdown]::ToHtml("H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024", (New-MarkdownPipeline -EmphasisExtraOptions Superscript)) returns "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>`n"' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -EmphasisExtraOptions Superscript, Marked;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.EmphasisExtras.EmphasisExtraExtension]) -Because 'Extensions[0].GetType()';
            $Html = [Markdig.Markdown]::ToHtml('H~2~O is ~~not~~ a ==liquid==. 2^10^ is 1024', $MarkdownPipeline);
            $Html | Should -Be "<p>H~2~O is ~~not~~ a ==liquid==. 2<sup>10</sup> is 1024</p>`n" -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/DiagramsSpecs.md
    Context 'New-MarkdownPipeline -UseDiagrams' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseDiagrams)) returns <Expected>' -ForEach @(
            @{
                Text = @'
```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```
'@;
                Expected = @"
<div class="mermaid">graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
</div>

"@;
            }, @{
                Text = @'
```nomnoml
[example|
  propertyA: Int
  propertyB: string
|
  methodA()
  methodB()
|
  [subA]--[subB]
  [subA]-:>[sub C]
]
```
'@
                Expected = @"
<div class="nomnoml">[example|
  propertyA: Int
  propertyB: string
|
  methodA()
  methodB()
|
  [subA]--[subB]
  [subA]-:>[sub C]
]
</div>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseDiagrams;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Diagrams.DiagramExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    (($MarkdownPipeline.Extensions | Where-Object { [Markdig.Extensions.Diagrams.DiagramExtension].IsInstanceOfType($_) }).Count -eq 1) | Should -BeTrue -Because 'DiagramExtension';

    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/ListExtraSpecs.md
    Context 'New-MarkdownPipeline -UseListExtras' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseListExtras)) returns <Expected>' -ForEach @(
            @{
                Text = @"
i. First item
ii. Second item
iii. Third item
"@;
                Expected = @"
<ol type="i">
<li>First item</li>
<li>Second item</li>
<li>Third item</li>
</ol>

"@;
            }, @{
                Text = @"
1.   First item

Some text

2.   Second item
"@;
                Expected = @"
<ol>
<li>First item</li>
</ol>
<p>Some text</p>
<ol start="2">
<li>Second item</li>
</ol>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseListExtras;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.ListExtras.ListExtraExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md
    Context 'New-MarkdownPipeline -UseGenericAttributes' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseGenericAttributes)) returns <Expected>' -ForEach @(
            @{
                Text = '[This is a link](http://google.com){#a-link .myclass data-lang=fr data-value="This is a value"}';
                Expected = @"
<p><a href="http://google.com" id="a-link" class="myclass" data-lang="fr" data-value="This is a value">This is a link</a></p>

"@;
            }, @{
                Text = @"
{#fenced-id .fenced-class}
```json
var t = "This is a fenced with attached attributes";
```
"@;
                Expected = @"
<pre><code id="fenced-id" class="language-json fenced-class">var t = &quot;This is a fenced with attached attributes&quot;;
</code></pre>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseGenericAttributes;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.GenericAttributes.GenericAttributesExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/
    Context 'New-MarkdownPipeline -UseNonAsciiNoEscape' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseNonAsciiNoEscape)) returns <Expected>' -ForEach @(
            @{
                Text = "`u{00a9}";
                Expected = @"
<p>`u{00a9}</p>

"@;
            }, @{
                Text = "`u{2192}";
                Expected = @"
<p>`u{2192}</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseNonAsciiNoEscape;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.NonAsciiNoEscape.NonAsciiNoEscapeExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/YamlSpecs.md
    Context 'New-MarkdownPipeline -UseYamlFrontMatter' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseYamlFrontMatter)) returns <Expected>' -ForEach @(
            @{
                Text = @"
---
this: is a frontmatter
---
This is a text
"@;
                Expected = @"
<p>This is a text</p>

"@;
            }, @{
                Text = "---   `nthis: is a frontmatter`n...`nThis is a text";
                Expected = @"
<p>This is a text</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseYamlFrontMatter;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Yaml.YamlFrontMatterExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
            $MarkdownDocument = [Markdig.Markdown]::Parse($Text, $MarkdownPipeline);
            $YamlFrontMatterBlock = [Markdig.Syntax.MarkdownObjectExtensions]::Descendants($MarkdownDocument) | Select-Object -First 1;
            $YamlFrontMatterBlock | Should -BeOfType ([Markdig.Extensions.Yaml.YamlFrontMatterBlock]) -Because 'MarkdownDocument[0].GetType()';
        }
    }
  
    
    Context 'New-MarkdownPipeline -UsePragmaLines' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UsePragmaLines)) returns <Expected>' -ForEach @(
            @{
                Text = @"
# Heading

- Item 1
- Item 2
"@;
                Pragmas = @('pragma-line-0', $null, 'pragma-line-2', 'pragma-line-2', 'pragma-line-2', $null, 'pragma-line-3', 'pragma-line-3', $null);
                Expected = @"
<h1 id="pragma-line-0">Heading</h1>
<ul id="pragma-line-2">
<li id="pragma-line-2">Item 1</li>
<li id="pragma-line-3">Item 2</li>
</ul>

"@;
            }, @{
                Text = @'
Text

```json
[
    1,
    2,
    3
]
```

More Text
'@;
                Pragmas = @('pragma-line-0', $null, 'pragma-line-2', 'pragma-line-10', $null)
                Expected = @"
<p id="pragma-line-0">Text</p>
<pre><code id="pragma-line-2" class="language-json">[
    1,
    2,
    3
]
</code></pre>
<p id="pragma-line-10">More Text</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePragmaLines;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.PragmaLines.PragmaLineExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
  
    # https://github.com/xoofx/markdig/blob/master/src/
    Context 'New-MarkdownPipeline -UsePreciseSourceLocation' {
        It '[Markdig.Markdown]::ToHtml("#Heading`n`nText *emphasized*", (New-MarkdownPipeline -UsePreciseSourceLocation)) returns [''$0, 0, 0-7'', ''$0, 0, 0-7'', ''$2, 0, 10-26'', ''$2, 0, 10-14'', ''$2, 5, 15-26'', ''$2, 6, 16-25'']' {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UsePreciseSourceLocation;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
            $MarkdownDocument = [Markdig.Markdown]::Parse("#Heading`n`nText *emphasized*");
            ([Markdig.Syntax.MarkdownObjectExtensions]::Descendants($MarkdownDocument) | ForEach-Object { $_.ToPositionText() }) | Should -Be '$0, 0, 0-7', '$0, 0, 0-7', '$2, 0, 10-26', '$2, 0, 10-14', '$2, 5, 15-26', '$2, 6, 16-25' -Because 'Position';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/SmartyPantsSpecs.md
    Context 'New-MarkdownPipeline -UseSmartyPants' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseSmartyPants)) returns <Expected>' -ForEach @(
            @{
                Text = "This is a `"text`"";
                Expected = @"
<p>This is a &ldquo;text&rdquo;</p>

"@;
            }, @{
                Text = "This is a <<text>>";
                Expected = @"
<p>This is a &laquo;text&raquo;</p>

"@;
            }, @{
                Text = "This is a --- text";
                Expected = @"
<p>This is a &mdash; text</p>

"@;
            }, @{
                Text = "This is a en ellipsis...";
                Expected = @"
<p>This is a en ellipsis&hellip;</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseSmartyPants;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.SmartyPants.SmartyPantsExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/BootstrapSpecs.md
    Context 'New-MarkdownPipeline -UseBootstrap' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseBootstrap)) returns <Expected>' -ForEach @(
            @{
                Text = "> This is a blockquote";
                Expected = @"
<blockquote class="blockquote">
<p>This is a blockquote</p>
</blockquote>

"@;
            }, @{
                Text = "![Image Link](/url)";
                Expected = @"
<p><img src="/url" class="img-fluid" alt="Image Link" /></p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseBootstrap;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Bootstrap.BootstrapExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/HardlineBreakSpecs.md
    Context 'New-MarkdownPipeline -UseSoftlineBreakAsHardlineBreak' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseSoftlineBreakAsHardlineBreak)) returns <Expected>' -ForEach @(
            @{
                Text = @"
This is a paragraph
with a break inside
"@;
                Expected = @"
<p>This is a paragraph<br />
with a break inside</p>

"@;
            }, @{
                Text = @"
This is a paragraph
with a break inside

And Paragraph
"@;
                Expected = @"
<p>This is a paragraph<br />
with a break inside</p>
<p>And Paragraph</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseSoftlineBreakAsHardlineBreak;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Hardlines.SoftlineBreakAsHardlineExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/EmojiSpecs.md
    Context 'New-MarkdownPipeline -UseEmoji -UseEmojiAndSmiley' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline <Options>)) returns <Expected>' -ForEach @(
            @{
                Text = "This is a test with a :) and a :angry: smiley";
                Splat = @{ UseEmoji = [System.Management.Automation.SwitchParameter]::Present; UseEmojiAndSmiley = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseEmoji -UseEmojiAndSmiley';
                Mapping = [Markdig.Extensions.Emoji.EmojiMapping]::DefaultEmojisAndSmileysMapping;
                Expected = @"
<p>This is a test with a `u{d83d}`u{de03} and a `u{d83d}`u{de20} smiley</p>

"@;
            }, @{
                Text = "This is a test with a :) and a :angry: smiley";
                Splat = @{ UseEmojiAndSmiley = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseEmojiAndSmiley';
                Mapping = [Markdig.Extensions.Emoji.EmojiMapping]::DefaultEmojisAndSmileysMapping;
                Expected = @"
<p>This is a test with a `u{d83d}`u{de03} and a `u{d83d}`u{de20} smiley</p>

"@;
            }, @{
                Text = "This is a test with a :) and a :angry: smiley";
                Splat = @{ UseEmoji = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseEmoji';
                Mapping = [Markdig.Extensions.Emoji.EmojiMapping]::DefaultEmojisOnlyMapping;
                Expected = @"
<p>This is a test with a :) and a `u{d83d}`u{de20} smiley</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Emoji.EmojiExtension]) -Because 'Extensions[0].GetType()';
            $MarkdownPipeline.Extensions[0].EmojiMapping | Should -BeExactly $Mapping -Because 'EmojiMapping';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -UseNoFollowLinks -UseNoOpenerLinks -UseNoReferrerLinks' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline <Options>)) returns <Expected>' -ForEach @(
            @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoFollowLinks = [System.Management.Automation.SwitchParameter]::Present; UseNoOpenerLinks = [System.Management.Automation.SwitchParameter]::Present; UseNoReferrerLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoFollowLinks -UseNoOpenerLinks -UseNoReferrerLinks';
                Rels = @('nofollow', 'noopener', 'noreferrer');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="nofollow noopener noreferrer">Markdig Documentation</a></p>

"@;
            }, @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoFollowLinks = [System.Management.Automation.SwitchParameter]::Present; UseNoOpenerLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoFollowLinks -UseNoOpenerLinks';
                Rels = @('nofollow', 'noopener');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="nofollow noopener">Markdig Documentation</a></p>

"@;
            }, @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoFollowLinks = [System.Management.Automation.SwitchParameter]::Present; UseNoReferrerLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoFollowLinks -UseNoReferrerLinks';
                Rels = @('nofollow', 'noreferrer');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="nofollow noreferrer">Markdig Documentation</a></p>

"@;
            }, @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoOpenerLinks = [System.Management.Automation.SwitchParameter]::Present; UseNoReferrerLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoOpenerLinks -UseNoReferrerLinks';
                Rels = @('noopener', 'noreferrer');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="noopener noreferrer">Markdig Documentation</a></p>

"@;
            }, @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoFollowLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoFollowLinks';
                Rels = @('nofollow');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="nofollow">Markdig Documentation</a></p>

"@;
            }, @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoOpenerLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoOpenerLinks';
                Rels = @('noopener');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="noopener">Markdig Documentation</a></p>

"@;
            }, @{
                Text = "[Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)";
                Splat = @{ UseNoReferrerLinks = [System.Management.Automation.SwitchParameter]::Present };
                Options = '-UseNoReferrerLinks';
                Rels = @('noreferrer');
                Expected = @"
<p><a href="https://github.com/xoofx/markdig/blob/master/readme.md" rel="noreferrer">Markdig Documentation</a></p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline @Splat;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.ReferralLinks.ReferralLinksExtension]) -Because 'Extensions[0].GetType()';
            $MarkdownPipeline.Extensions[0].Rels | Should -Be $Rels -Because 'Rels';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GlobalizationSpecs.md
    Context 'New-MarkdownPipeline -UseGlobalization' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -UseGlobalization)) returns <Expected>' -ForEach @(
            @{
                Text = @"
# میوە
[میوە](https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95) یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو

> میوە بۆ تەندروستی باشە
-- نەزانراو
"@;
                Expected = @"
<h1 dir="rtl">میوە</h1>
<p dir="rtl"><a href="https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95" dir="rtl">میوە</a> یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو</p>
<blockquote dir="rtl">
<p dir="rtl">میوە بۆ تەندروستی باشە
-- نەزانراو</p>
</blockquote>

"@;
            }, @{
                Text = @"
## جۆرەکانی میوە
- توو
  - فڕاولە
  - کیوی
- مزرەمەنی
  - پڕتەقاڵ
  - لیمۆ
"@;
                Expected = @"
<h2 dir="rtl">جۆرەکانی میوە</h2>
<ul dir="rtl">
<li>توو
<ul dir="rtl">
<li>فڕاولە</li>
<li>کیوی</li>
</ul>
</li>
<li>مزرەمەنی
<ul dir="rtl">
<li>پڕتەقاڵ</li>
<li>لیمۆ</li>
</ul>
</li>
</ul>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -UseGlobalization;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 1 -Because 'Extensions.Count';
            $MarkdownPipeline.Extensions[0] | Should -BeOfType ([Markdig.Extensions.Globalization.GlobalizationExtension]) -Because 'Extensions[0].GetType()';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/NoHtmlSpecs.md
    Context 'New-MarkdownPipeline -DisableHtml' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -DisableHtml)) returns <Expected>' -ForEach @(
            @{
                Text = "This has <em>HTML</em>";
                Expected = @"
<p>This has &lt;em&gt;HTML&lt;/em&gt;</p>

"@;
            }, @{
                Text = @"
<div>
this is some text
</div>
"@;
                Expected = @"
<p>&lt;div&gt;
this is some text
&lt;/div&gt;</p>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -DisableHtml;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    # https://github.com/xoofx/markdig/blob/master/src/
    Context 'New-MarkdownPipeline -DisableHeadings' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -DisableHeadings)) returns <Expected>' -ForEach @(
            @{
                Text = @"
# Heading 1

My Text

## Heading 2
"@;
                Expected = @"
<p># Heading 1</p>
<p>My Text</p>
<p>## Heading 2</p>

"@;
            }, @{
                Text = @"
<h1>Heading 1</h1>

My Text

<h2>Heading 2</h2>
"@;
                Expected = @"
<h1>Heading 1</h1>
<p>My Text</p>
<h2>Heading 2</h2>

"@;
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -DisableHeadings;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
            $Actual = [Markdig.Markdown]::ToHtml($Text, $MarkdownPipeline);
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
    
    Context 'New-MarkdownPipeline -EnableTrackTrivia' {
        It '[Markdig.Markdown]::ToHtml(<Text>, (New-MarkdownPipeline -EnableTrackTrivia)) returns <Expected>' -ForEach @(
            @{
                Text = @"
# Heading 1

Content

- *A*: B
- *C*: D
"@;
                Expected = '[{"Before":0,"After":0},{"Before":1,"After":1},{"Before":1,"After":1},{"Before":0,"After":1},{"Before":1,"After":1},{"Before":0,"After":1},{"Before":1,"After":1}]'
            }, @{
                Text = "# Heading 1";
                Expected = '{"Before":0,"After":0}';
            }
        ) {
            [Markdig.MarkdownPipeline]$MarkdownPipeline = New-MarkdownPipeline -EnableTrackTrivia;
            $MarkdownPipeline | Should -BeOfType ([Markdig.MarkdownPipeline]) -Because 'Type';
            $MarkdownPipeline.Extensions | Should -HaveCount 0 -Because 'Extensions.Count';
            $MarkdownDocument = [Markdig.Markdown]::Parse($Text, $MarkdownPipeline);
            $Actual = [Markdig.Syntax.MarkdownObjectExtensions]::Descendants[Markdig.Syntax.Block]($MarkdownDocument) | ForEach-Object {
                if ($null -eq $_.TriviaBefore) {
                    if ($null -eq $_.TriviaAfter) {
                        [PSCustomObject]@{
                            Before = 0;
                            After = 0;
                        }
                    } else {
                        [PSCustomObject]@{
                            Before = 0;
                            After = $_.TriviaAfter.Length;
                        }
                    }
                } else {
                    if ($null -eq $_.TriviaAfter) {
                        [PSCustomObject]@{
                            Before = $_.TriviaBefore.Length;
                            After = 0;
                        }
                    } else {
                        [PSCustomObject]@{
                            Before = $_.TriviaBefore.Length;
                            After = $_.TriviaAfter.Length;
                        }
                    }
                }
            } | ConvertTo-Json -Depth 3 -Compress;
            $Actual | Should -Be $Expected -Because 'Render';
        }
    }
}