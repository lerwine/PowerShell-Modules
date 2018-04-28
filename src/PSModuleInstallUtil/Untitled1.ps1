Param(
    [Parameter(Position = 0)]
    $TargetDir = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\Speech\bin\Debug\',
    [Parameter(Position = 1)]
    $ProjectDir = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\Speech\',
    [Parameter(Position = 2)]
    $TargetName = 'Erwine.Leonard.T.Speech'
)

Function ConvertTo-CmdletParameter {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Management.Automation.ParameterAttribute]$Attribute
    )

    Process {
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            DontShow = $Attribute.DontShow;
            Mandatory = $Attribute.Mandatory;
            ParameterSetName = $Attribute.ParameterSetName;
            HelpMessage = $Attribute.HelpMessage;
            Position = $Attribute.Position;
            ValueFromPipeline = $Attribute.ValueFromPipeline;
            ValueFromPipelineByPropertyName = $Attribute.ValueFromPipelineByPropertyName;
            ValueFromRemainingArguments = $Attribute.ValueFromRemainingArguments;
        };
    }
}

Function Get-CmdletProperty {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Reflection.PropertyInfo]$PropertyInfo
    )

    Process {
        $a = $PropertyInfo.GetCustomAttributes([System.Management.Automation.ParameterAttribute], $true);
        if ($a.Length -gt 0) {
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                Name = $PropertyInfo.Name;
                DocElement = $XmlDocument.SelectSingleNode("/doc/members/member[@name='P:$($PropertyInfo.ReflectedType.FullName).$($PropertyInfo.Name)']");
                Parameter = @($a | ConvertTo-CmdletParameter);
                PropertyType = $PropertyInfo.PropertyType;
            };
        }
    }
}

Function Get-ModuleCmdletInfo {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Type]$Type
    )

    Process {
        if ($Type.IsPublic -and [System.Management.Automation.PSCmdlet].IsAssignableFrom($Type) -and -not $Type.IsAbstract) {
            $a = $Type.GetCustomAttributes([System.Management.Automation.CmdletAttribute]);
            if ($a.Length -gt 0) {
                [System.Management.Automation.CmdletAttribute]$CmdletAttribute = $a[0];
                if (-not ([System.String]::IsNullOrWhiteSpace($CmdletAttribute.NounName) -or [System.String]::IsNullOrWhiteSpace($CmdletAttribute.VerbName))) {
                    $OutputType = @($Type.GetCustomAttributes([System.Management.Automation.OutputTypeAttribute], $true) | ForEach-Object {
                        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                            ParameterSetNames = $_.ParameterSetName;
                            Types = $_.Type;
                        }
                    });
                    $Properties = @($Type.GetProperties() | Get-CmdletProperty);
                    $AllParameterSets = @();
                    if ($a[0].DefaultParameterSetName -ne $null) {
                        $AllParameterSets = $AllParameterSets + $a[0].DefaultParameterSetName;
                    }
                    $OutputType | ForEach-Object { if ($_.ParameterSetName.Count -gt 0) { $AllParameterSets = $AllParameterSets + $_.ParameterSetName } }
                    $Properties | ForEach-Object { $_.Parameter | ForEach-Object { if ($_.ParameterSetName -ne $null) { $AllParameterSets = $AllParameterSets + $_.ParameterSetName } } }
                    $AllParameterSets = @($AllParameterSets | Select-Object -Unique);
                    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                        ConfirmImpact = $a[0].ConfirmImpact;
                        DefaultParameterSetName = $a[0].DefaultParameterSetName;
                        HelpUri = $a[0].HelpUri;
                        Noun = $a[0].NounName;
                        RemotingCapability = $a[0].RemotingCapability;
                        SupportsPaging = $a[0].SupportsPaging;
                        SupportsShouldProcess = $a[0].SupportsShouldProcess;
                        SupportsTransactions = $a[0].SupportsTransactions;
                        Verb = $a[0].VerbName;
                        DocElement = $Script:XmlDocument.SelectSingleNode("/doc/members/member[@name='T:$($_.FullName)']");
                        OutputType = $OutputType;
                        Properties = $Properties;
                        AllParameterSets = $AllParameterSets;
                    }
                }
            }
        }
    }
}

Function Test-IsParagraphHost {
    [CmdletBinding(DefaultParameterSetName = 'Element')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Element')]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$LocalName,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$NamespaceURI
    )

    if ($PSBoundParameters.ContainsKey('XmlElement')) {
        $XmlElement.NamespaceURI -eq $Script:Ns.maml -and @('description', 'copyright') -contains $XmlElement.LocalName;
    } else {
        $NamespaceURI -eq $Script:Ns.maml -and @('description', 'copyright') -contains $LocalName;
    }
}

Function Test-IsParagraph {
    [CmdletBinding(DefaultParameterSetName = 'Element')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Element')]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$LocalName,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$NamespaceURI
    )
    
    if ($PSBoundParameters.ContainsKey('XmlElement')) {
        $XmlElement.NamespaceURI -eq $Script:Ns.maml -and $XmlElement.LocalName -eq 'para';
    } else {
        $NamespaceURI -eq $Script:Ns.maml -and $LocalName -eq 'para';
    }
}

Function Test-IsParagraphContent {
    [CmdletBinding(DefaultParameterSetName = 'Element')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Element')]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$LocalName,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$NamespaceURI
    )
    
    if ($PSBoundParameters.ContainsKey('XmlElement')) {
        -not ((Test-IsParagraph -XmlElement $XmlElement) -or (Test-IsParagraphHost -XmlElement $XmlElement))
    } else {
        -not ((Test-IsParagraph -LocalName $LocalName -NamespaceURI $NamespaceURI) -or (Test-IsParagraphHost -LocalName $LocalName -NamespaceURI $NamespaceURI))
    }
}

Function Get-ParentElement {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true)]
        [string]$ChildName,
        
        [Parameter(Mandatory = $true)]
        [string]$ChildNamespaceURI
    )

    if (Test-IsParagraph -LocalName $ChildName -NamespaceURI $ChildNamespaceURI) {
        if (Test-IsParagraph -XmlElement $XmlElement) {
            $XmlElement.ParentNode | Write-Output;
        } else {
            $XmlElement | Write-Output;
        }
    } else {
        if (Test-IsParagraphHost -XmlElement $XmlElement) {
            $XmlElement.AppendChild($XmlElement.OwnerDocument.CreateElement('para', $Script:Ns.maml)) | Write-Output;
        } else {
            $XmlElement | Write-Output;
        }
    }
}

Function Add-CRefToPara {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$SourceElement,
        
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$TargetElement
    )

    $cref = $SourceElement.GetAttribute('cref');
    $ParentElement = $null;
    if (-not [System.String]::IsNullOrEmpty($cref)) {
        if ($cref.StartsWith('P:')) {
            $ParentElement = Get-ParentElement -XmlElement $TargetElement -ChildName 'parameterNameInline' -ChildNamespaceURI $Script:Ns.maml;
            $ParentElement.AppendChild($TargetElement.OwnerDocument.CreateElement('parameterNameInline', $Script:Ns.maml)).InnerText = $cref.Substring(2);
        } else {
            $ParentElement = Get-ParentElement -XmlElement $TargetElement -ChildName 'token' -ChildNamespaceURI $Script:Ns.maml;
            $index = $cref.IndexOf(':');
            if ($index -gt 0) {
                $ParentElement.AppendChild($TargetElement.OwnerDocument.CreateElement('token', $Script:Ns.maml)).InnerText = $cref.Substring($index + 1);
            } else {
                $ParentElement.AppendChild($TargetElement.OwnerDocument.CreateElement('token', $Script:Ns.maml)).InnerText = $cref;
            }
        }
    }
    if ($SourceElement.HasChildNodes) {
        if ($ParentElement -eq $null) {
            $ParentElement = Convert-XmlDocToPara -SourceElement $SourceElement -TargetElement $TargetElement;
        } else {
            Convert-XmlDocToPara -SourceElement $SourceElement -TargetElement $ParentElement | Out-Null;
        }
    }

    if (Test-IsParagraph -XmlElement $TargetElement) {
        $TargetElement | Write-Output;
    } else {
        if ($ParentElement -ne $null -and (Test-IsParagraph -XmlElement $ParentElement)) {
            $ParentElement | Write-Output;
        } else {
            if (Test-IsParagraph -XmlElement $TargetElement.ParentNode) {
                $TargetElement.ParentNode | Write-Output;
            }
        }
    }
}

Function Add-CodeToPara {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$SourceElement,
        
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$TargetElement
    )

    if ($SourceElement.HasChildNodes) {
        $ParentElement = Get-ParentElement -XmlElement $TargetElement -ChildName 'codeInline' -ChildNamespaceURI $Script:Ns.maml;
        Convert-XmlDocToPara -SourceElement $SourceElement -TargetElement $ParentElement.AppendChild($TargetElement.OwnerDocument.CreateElement('codeInline', $Script:Ns.maml)) | Out-Null;
    }
    if (Test-IsParagraph -XmlElement $TargetElement) {
        $TargetElement | Write-Output;
    } else {
        if ($ParentElement -ne $null -and (Test-IsParagraph -XmlElement $ParentElement)) {
            $ParentElement | Write-Output;
        } else {
            if (Test-IsParagraph -XmlElement $TargetElement.ParentNode) {
                $TargetElement.ParentNode | Write-Output;
            }
        }
    }
}

Function Convert-XmlDocToPara {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$SourceElement,
        
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$TargetElement
    )

    $CurrentParagraph = $null;
    if (Test-IsParagraph -XmlElement $TargetElement) {
        $CurrentParagraph = $TargetElement;
    }
    if ($SourceElement.HasChildNodes) {
        foreach ($XmlNode in $SourceElement.ChildNodes) {
            if ($XmlNode -is [System.Xml.XmlCharacterData]) {
                $s = $XmlNode.InnerText.Trim();
                if ($s.Length -gt 0) {
                    if ($CurrentParagraph -eq $null) {
                        $ParentElement = Get-ParentElement -XmlElement $TargetElement -ChildName 'para' -ChildNamespaceURI $Script:Ns.maml;
                        $CurrentParagraph = $ParentElement.AppendChild($ParentElement.OwnerDocument.CreateElement('para', $Script:Ns.maml));
                    }
                    $CurrentParagraph.AppendChild($TargetElement.OwnerDocument.CreateTextNode($s)) | Out-Null;
                }
            } else {
                if ($XmlNode -is [System.Xml.XmlElement]) {
                    switch ($XmlNode.LocalName) {
                        'seealso' {
                            $CurrentParagraph = Add-CRefToPara -SourceElement $XmlNode -TargetElement $TargetElement;
                            break;
                        }
                        'see' {
                            $CurrentParagraph = Add-CRefToPara -SourceElement $XmlNode -TargetElement $TargetElement;
                            break;
                        }
                        'typeparamref' {
                            $CurrentParagraph = Add-CRefToPara -SourceElement $XmlNode -TargetElement $TargetElement;
                            break;
                        }
                        'para' {
                            if ($XmlNode.HasChildNodes) {
                                Convert-XmlDocToPara -DocElement $XmlNode -ParentElement ($ParentElement.AppendChild($ParentElement.OwnerDocument.CreateElement('para', $Script:Ns.maml))) | Out-Null;
                                $CurrentParagraph = $null;
                            }
                            break;
                        }
                        'code' {
                            $CurrentParagraph = Add-CodeToPara -SourceElement $XmlNode -TargetElement $TargetElement;
                            break;
                        }
                        'c' {
                            $CurrentParagraph = Add-CodeToPara -SourceElement $XmlNode -TargetElement $TargetElement;
                            break;
                        }
                        default {
                            if ($CurrentParagraph -eq $null) {
                                $CurrentParagraph = Convert-XmlDocToPara -SourceElement $XmlNode -TargetElement $TargetElement;
                            } else {
                                $CurrentParagraph = Convert-XmlDocToPara -SourceElement $XmlNode -TargetElement $CurrentParagraph;
                            }
                            break;
                        }
                    }
                }
            }
        }
        if ($CurrentParagraph -ne $null) {
            $CurrentParagraph | Write-Output;
        } else {
            if (Test-IsParagraph -XmlElement $TargetElement) {
                $TargetElement | Write-Output;
            } else {
                if (Test-IsParagraph -XmlElement $TargetElement.ParentNode) {
                    $TargetElement.ParentNode | Write-Output;
                }
            }
        }
    } else {
        if (Test-IsParagraph -XmlElement $TargetElement) {
            $TargetElement | Write-Output;
        } else {
            if (Test-IsParagraph -XmlElement $TargetElement.ParentNode) {
                $TargetElement.ParentNode | Write-Output;
            }
        }
    }
}

Function Add-CommandHelp {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$HelpElement,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyCollection()]
        [string[]]$AllParameterSets,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [System.Management.Automation.ConfirmImpact]$ConfirmImpact,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$DefaultParameterSetName,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowNull()]
        [System.Xml.XmlElement]$DocElement,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyString()]
        [string]$HelpUri,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$Noun,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyCollection()]
        [System.Management.Automation.PSObject[]]$OutputType,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowEmptyCollection()]
        [System.Management.Automation.PSObject[]]$Properties,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [bool]$RemotingCapability,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [bool]$SupportsPaging,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [bool]$SupportsShouldProcess,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [bool]$SupportsTransactions,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$Verb
    )

    Process {
        $commandElement = $HelpElement.AppendChild($HelpElement.OwnerDocument.CreateElement('command', $Script:Ns.command));
        $detailsElement = $commandElement.AppendChild($HelpElement.OwnerDocument.CreateElement('details', $Script:Ns.command));
        $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('name', $Script:Ns.command)).InnerText = "$Verb-$Noun";
        if ($DocElement -ne $null) {
            $summaryElement = $DocElement.SelectSingleNode('summary');
            if ($summaryElement -ne $null) {
                Convert-XmlDocToPara -SourceElement $summaryElement -TargetElement $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('description', $Script:Ns.maml)) | Out-Null;
            }
            $copyrightElement = $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('copyright', $Script:Ns.maml));
            $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('details', $Script:Ns.maml)).InnerText = $Script:Copyright;
            $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('verb', $Script:Ns.command)).InnerText = $Verb;
            $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('noun', $Script:Ns.command)).InnerText = $Noun;
            $detailsElement.AppendChild($HelpElement.OwnerDocument.CreateElement('version', $Script:Ns.dev)).InnerText = $Script:Version;
            $remarksElement = $DocElement.SelectSingleNode('remarks');
            if ($remarksElement -ne $null) {
                Convert-XmlDocToPara -SourceElement $remarksElement -TargetElement $commandElement.AppendChild($HelpElement.OwnerDocument.CreateElement('description', $Script:Ns.maml)) | Out-Null;
            }
        }
    }
}

$Script:XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
$Script:XmlDocument.Load(($TargetDir | Join-Path -ChildPath "$TargetName.XML"));
$Script:ModuleAssembly = [System.Reflection.Assembly]::LoadFrom(($TargetDir | Join-Path -ChildPath "$TargetName.dll"));

$a = $Script:ModuleAssembly.GetCustomAttributes([System.Reflection.AssemblyCompanyAttribute], $true);
$Script:Company = 'Unknown'
if ($a.Length -gt 0 -and -not [System.String]::IsNullOrWhiteSpace($a[0].Company)) {
    $Script:Company = $a[0].Company.Trim();
}
$Script:Copyright = "(c) 2017 $Script:Company. All rights reserved.";
$a = $Script:ModuleAssembly.GetCustomAttributes([System.Reflection.AssemblyCopyrightAttribute], $true);
if ($a.Length -gt 0 -and -not [System.String]::IsNullOrWhiteSpace($a[0].Copyright)) {
    $Script:Copyright = $a[0].Copyright.Trim();
}
$Cmdlets = @(@($Script:ModuleAssembly.DefinedTypes) | Get-ModuleCmdletInfo);
$v = $Script:ModuleAssembly.GetName().Version;
if ($v.Revision -ne 0) {
    $Script:Version = $v.ToString();
} else {
    if ($v.Build -eq 0) {
        $Script:Version = $v.ToString(2);
    } else {
        $Script:Version = $v.ToString(3);
    }
}
$Script:Ns = @{
    maml = 'http://schemas.microsoft.com/maml/2004/10';
    command = 'http://schemas.microsoft.com/maml/dev/command/2004/10';
    dev = 'http://schemas.microsoft.com/maml/dev/2004/10';
    MSHelp = 'http://msdn.microsoft.com/mshelp';
}
$Script:HelpDocument = New-Object -TypeName 'System.Xml.XmlDocument';
$Script:HelpDocument.AppendChild($Script:HelpDocument.CreateElement('helpItems', 'http://msh')).Attributes.Append($Script:HelpDocument.CreateAttribute('schema')).Value = 'maml';
$Script:Ns.Keys | ForEach-Object {
    $Script:HelpDocument.DocumentElement.Attributes.Append($Script:HelpDocument.CreateAttribute($_, 'http://www.w3.org/XML/1998/namespace')).Value = $Script:Ns[$_];
}
$Cmdlets | Add-CommandHelp -HelpElement $Script:HelpDocument.DocumentElement;
$MemoryStream = New-Object -TypeName 'System.IO.MemoryStream';
$XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
$XmlWriterSettings.Encoding = New-Object -TypeName 'System.Text.UTF8Encoding' -ArgumentList $false;
$XmlWriterSettings.Indent = $true;
$XmlWriter = [System.Xml.XmlWriter]::Create($MemoryStream, $XmlWriterSettings);
$Script:HelpDocument.WriteTo($XmlWriter);
$XmlWriter.Flush();
$XmlWriterSettings.Encoding.GetString($MemoryStream.ToArray());
$XmlWriter.Close();
$MemoryStream.Dispose();