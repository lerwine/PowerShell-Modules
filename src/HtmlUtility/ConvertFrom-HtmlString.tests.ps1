Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.HtmlUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Testing ConvertFrom-HtmlString)' {
    Context 'ConvertFrom-HtmlString -InputText [string]' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -ErrorHandling [ParseErrorHandling]' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -ExtractErrorSourceTextMaxLength [int]' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -MaxNestedChildNodes [int]' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -AddDebuggingAttributes' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -AutoCloseOnEnd' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -DoNotCheckSyntax' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -ComputeChecksum' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -TreatCDataBlockAsComment' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -ExtractErrorSourceText' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -FixNestedTags' {
    }
    
    Context 'ConvertFrom-HtmlString -InputText [string] -DoNotReadEncoding' {
    }
}
