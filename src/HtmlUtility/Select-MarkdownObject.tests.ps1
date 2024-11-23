Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.HtmlUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Testing Select-MarkdownObject (DepthRange Parameter Set)' {
    Context 'Select-MarkdownObject -Type [Type]' {
    }

    Context 'Select-MarkdownObject -TType [Type[]]' {
    }

    Context 'Select-MarkdownObject -TMinDepth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth 1' {
    }

    Context 'Select-MarkdownObject -TMinDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TMaxDepth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MaxDepth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MaxDepth 1' {
    }

    Context 'Select-MarkdownObject -TMaxDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MaxDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MaxDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TMinDepth 1 -MaxDepth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth 1 -MaxDepth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth 1 -MaxDepth 1' {
    }

    Context 'Select-MarkdownObject -TMinDepth 1 -MaxDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth 1 -MaxDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth 1 -MaxDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TMinDepth (>1) -MaxDepth (=MinDepth)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -MaxDepth (=MinDepth)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -MaxDepth (=MinDepth)' {
    }

    Context 'Select-MarkdownObject -TMinDepth (>1) -MaxDepth (=MinDepth+1)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -MaxDepth (=MinDepth+1)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -MaxDepth (=MinDepth+1)' {
    }

    Context 'Select-MarkdownObject -TMinDepth (>1) -MaxDepth (>MinDepth+1)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -MaxDepth (>MinDepth+1)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -MaxDepth (>MinDepth+1)' {
    }
}

Describe 'Testing Select-MarkdownObject (ExplicitDepth Parameter Set)' {
    Context 'Select-MarkdownObject -TDepth 1' {
    }

    Context 'Select-MarkdownObject -TDepth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type] -Depth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type] -Depth (>1)' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -Depth 1' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -Depth (>1)' {
    }
}

Describe 'Testing Select-MarkdownObject (Recurse Parameter Set)' {
    Context 'Select-MarkdownObject -TRecurse' {
    }

    Context 'Select-MarkdownObject -TType [Type] -Recurse' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -Recurse' {
    }
}

Describe 'Testing Select-MarkdownObject (RecurseUnmatched Parameter Set)' {
    Context 'Select-MarkdownObject -TType [Type] -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth 1 -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth 1 -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MaxDepth 1 -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MaxDepth 1 -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MaxDepth (>1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MaxDepth (>1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth 1 -MaxDepth 1 -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth 1 -MaxDepth 1 -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth 1 -MaxDepth (>1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth 1 -MaxDepth (>1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -MaxDepth (=MinDepth) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -MaxDepth (=MinDepth) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -MaxDepth (=MinDepth+1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -MaxDepth (=MinDepth+1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type] -MinDepth (>1) -MaxDepth (>MinDepth+1) -RecurseUnmatched' {
    }

    Context 'Select-MarkdownObject -TType [Type[]] -MinDepth (>1) -MaxDepth (>MinDepth+1) -RecurseUnmatched' {
    }
}