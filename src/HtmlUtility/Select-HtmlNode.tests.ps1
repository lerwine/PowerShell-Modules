Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.HtmlUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Testing Select-HtmlNode' {
    Context 'Select-HtmlNode -InputNode [HtmlNode] -XPath [string]' {
    }

    Context 'Select-HtmlNode -InputNode [HtmlNode] -XPath [string] -First' {
    }

    Context 'Select-HtmlNode -InputNode [HtmlNode] -XPath [string] -AsNodeCollection' {
    }
}