if ($null -eq $Script:MamlSchema) {
    New-Variable -Name 'MamlSchema' -Option ReadOnly -Scope 'Script' -Value ([PSCustomObject]@{
        msh = [PSCustomObject]@{
            ns = 'http://msh';
            rootElement = 'helpItems';
        }
        maml = [PSCustomObject]@{
            ns = 'http://schemas.microsoft.com/maml/2004/10';
        }
        command = 'http://schemas.microsoft.com/maml/dev/command/2004/10';
        dev = 'http://schemas.microsoft.com/maml/dev/2004/10';
    });
}

Function New-PSMaml {
    <#
        .SYNOPSIS
            Get PowerShell help MAML.
 
        .DESCRIPTION
            Creates a new Xml document for PowerShell MAML help.

        .EXAMPLE
            $MamDocument = New-PSMaml;
            $CommandElement = Add-CommandMaml -Maml $MamDocument -Verb 'Invoke' -Noun 'MyCommand';

        .LINK
            Add-CommandMaml

    #>
    [CmdletBinding()]
    [OutputType([xml])]
    Param()

    $PSMaml = New-Object -TypeName 'System.Xml.XmlDocument';
    $PSMaml.AppendChild($PSMaml.CreateElement($Script:MamlSchema.msh.rootElement, $Script:MamlSchema.msh.ns)) | Out-Null;
    $PSMaml | Write-Output;
}

Function Import-PSMaml {
    <#
        .SYNOPSIS
            Imports PowerShell help MAML.
 
        .DESCRIPTION
            Loads PowerShell help MAM into a new Xml document.

        .EXAMPLE
            $MamDocument = (Get-Content -Path 'MyModule.dll-Help.xml') | Import-PSMaml;

        .LINK
            New-CommandMaml
    #>
    [CmdletBinding()]
    [OutputType([xml])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Content')]
        [AllowEmptyString()]
        [string[]]$Content,

        [Parameter(Mandatory = $true, ParameterSetName = 'Stream')]
        [System.IO.Stream]$Stream,

        [Parameter(Mandatory = $true, ParameterSetName = 'TextReader')]
        [System.IO.TextReader]$TextReader,

        [Parameter(Mandatory = $true, ParameterSetName = 'XmlReader')]
        [System.Xml.XmlReader]$XmlReader
    )

    Begin {
        $PSMaml = New-Object -TypeName 'System.Xml.XmlDocument';
        $AllContent = @();
    }

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Stream' {
                $PSMaml.Load($Stream);
                break;
            }
            'TextReader' {
                $PSMaml.Load($TextReader);
                break;
            }
            'XmlReader' {
                $PSMaml.Load($XmlReader);
                break;
            }
            default {
                $AllContent = $AllContent + @($Content);
                break;
            }
        }
    }
    End {
        if ($PSCmdlet.ParameterSetName -eq 'Content') {
            $PSMaml.LoadXml(($AllContent | Out-String).Trim());
        }
        $PSMaml | Write-Output;
    }
}

Function Test-PSMaml {
    <#
        .SYNOPSIS
            Checks whether XML represents PowerShell help MAML.
 
        .DESCRIPTION
            Checks whether the XML document element name is 'helpItems' and the namespace of that element is 'http://msh'.
    #>
    [CmdletBinding()]
    [OutputType([bool], [string])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [Alias('maml')]
        [xml]$PSMaml
    )

    Begin {
        $Success = $true;
    }
    Process {
        if ($Success) {
            foreach ($x in $PSMaml) {
                if ($null -eq $x -or $null -eq $PSMaml.DocumentElement -or $PSMaml.DocumentElement.LocalName -ne $Script:MamlSchema.msh.rootElement -or $PSMaml.DocumentElement.NamespaceURI -ne $Script:MamlSchema.msh.ns) {
                    $Success = $false;
                    break;
                }
            }
        }
    }
    End {
        $Success | Write-Output;
    }
}

Function Test-CommandVerb {
    <#
        .SYNOPSIS
            Test command verb text.
 
        .DESCRIPTION
            Checks validity of command verb text or gets proper letter-case commnd verb string.

        .EXAMPLE
            $VerbText = Read-Host -Prompt 'Enter verb';
            $IsVerbValid = $VerbText | Test-CommandVerb;

        .EXAMPLE
            $VerbText = Read-Host -Prompt 'Enter verb';
            $VerbText = $VerbText | Test-CommandVerb -GetMatching;
            if ($null -eq $VerbText) {
                Write-Warning -Message "'$VerbText' is not a valid verb.";
            }
    #>
    [CmdletBinding()]
    [OutputType([bool], [string])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [string[]]$Verb,

        [switch]$GetMatching
    )

    Begin {
        if ($null -eq $Script:__Test_CommandVerb) {
            $Script:__Test_CommandVerb = @(Get-Verb | ForEach-Object { $_.Verb })
        }
        $Success = $true;
    }
    Process {
        if ($GetMatching) {
            foreach ($v in $Verb) {
                if ($null -ne $v) {
                    $s = $v.Trim();
                    $Script:__Test_CommandVerb | Where-Object { $_ -ieq $s }
                }
            }
        } else {
            if ($Success) {
                foreach ($v in $Verb) {
                    if ($null -eq $v -or $Script:__Test_CommandVerb -inotcontains $v) {
                        $Success = $false;
                        break;
                    }
                }
            }
        }
    }
    End {
        if (-not $GetMatching) { $Success | Write-Output }
    }
}

Function Get-CommandMaml {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlElement])]
    Param(
        [Parameter(Mandatory = $true)]
        [Alias('maml')]
        [ValidateScript({ $_ | Test-PSMaml })]
        [xml]$PSMaml,
        
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'InputNames')]
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'InputVerbs')]
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'InputNouns')]
        [string[]]$InputString,
        
        [Parameter(ParameterSetName = 'VerbNoun')]
        [string[]]$Verb,
        
        [Parameter(ParameterSetName = 'VerbNoun')]
        [string]$Noun,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Name')]
        [string[]]$Name,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'InputName')]
        [switch]$Names,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'InputVerbs')]
        [switch]$Verbs,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'InputNouns')]
        [switch]$Nouns
    )



    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'InputNames' {
                
            }
        }
    }
}

Function Add-CommandMaml {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlElement])]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({})]
        [string]$Verb
    )

}
