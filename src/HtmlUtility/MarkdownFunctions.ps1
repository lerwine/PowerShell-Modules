if ($null -eq $Script:MarkdownObjectExtensions_Descendants) {
    New-Variable -Name 'MarkdownObjectExtensions_Descendants' -Option ReadOnly -Scope 'Script' -Value ([Markdig.Syntax.MarkdownObjectExtensions].GetMethod('Descendants', 1, ([Type[]]@([Markdig.Syntax.MarkdownObject]))));
}

Function Get-MarkdownDescendants {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $true)]
        [Alias('Tokens', 'Markdown')]
        [Markdig.Syntax.MarkdownObject]$MarkdownObject,

        [ValidateScript({ [Markdig.Syntax.MarkdownObject].IsAssignableFrom($_) })]
        [Type]$Type,

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