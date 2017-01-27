Import-Module -Name 'Erwine.Leonard.T.NetworkUtility';

Function Get-NugetApiUrl {
    <#
        .SYNOPSIS
			Get URL for NuGet API request.
         
        .DESCRIPTION
			returns URI for API request.
        
        .OUTPUTS
			System.Uri. A Uri for the NuGet API request.
    #>
    [CmdletBinding()]
    [OutputType([System.Uri])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # Path of request.
        [string]$Path,
        
        [string]$Query
    )
    
    $UriBuilder = New-Object -TypeName 'System.UriBuilder' -ArgumentList 'http://nuget.org/api/v2';
    if ($Path.StartsWith('/')) {
        $UriBuilder.Path = $UriBuilder.Path + $Path;
    } else {
        $UriBuilder.Path = $UriBuilder.Path + '/' + $Path;
    }
    
    if ($PSBoundParameters.ContainsKey('Query')) { $UriBuilder.Query = $Query }
    $UriBuilder.Uri | Write-Output;
}

Function Get-NugetPackageVersions {
    <#
        .SYNOPSIS
			Get versions for NuGet package.
         
        .DESCRIPTION
			returns a list of NuGet package versions
        
        .OUTPUTS
			System.String[]. An array of package versions
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # NuGet package identifier.
        [string]$Identifier
    )
    $WebRequest = New-WebRequest -Uri (Get-NugetApiUrl -Path ('package-versions/' + $Identifier));
    $WebResponse = Get-WebResponse -WebRequest $WebRequest -AllowRedirect $true;
    $Encoding = [System.Text.Encoding]::UTF8;
    if ($WebResponse.ContentType -ne $null -and $WebResponse.ContentType.CharSet -ne $null -and $WebResponse.ContentType.CharSet.Trim().Length -gt 0) {
        $e = $Encoding;
        try { $e = [System.Text.Encoding]::GetEncoding($WebResponse.ContentType.CharSet) } catch { }
        if ($e -ne $null) { $Encoding = $e }
    }
    $Stream = $WebResponse.Response.GetResponseStream();
    $Json = $null;
    try {
        $Reader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Stream, $Encoding;
        try {
            $Json = $Reader.ReadToEnd();
        } finally { $Stream.Dispose() }
    } finally { $Stream.Dispose() }

    $JavaScriptSerializer = New-Object -TypeName 'System.Web.Script.Serialization.JavaScriptSerializer';
    #[System.Web.Script.Serialization.JavaScriptConverter[]]$Converters = @((New-Object -TypeName 'NugetSerialization2.JsonConverter'));
    #$JavaScriptSerializer.RegisterConverters($Converters);
    $JavaScriptSerializer.DeserializeObject($Json) | Write-Output;
}

Function Get-NugetPackageInfo {
    <#
        .SYNOPSIS
			Get meta data for NuGet package.
         
        .DESCRIPTION
			returns a list of NuGet package versions
        
        .OUTPUTS
			System.String[]. An array of package versions
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # NuGet package identifier.
        [string]$Identifier,
        
        [Parameter(Position = 1, ParameterSetName = 'ByVersion')]
        # NuGet package version.
        [string]$Version,
        
        [Parameter(ParameterSetName = 'LatestVersion')]
        # Returns latest stable version.
        [switch]$LatestVersion
        
    )
    $Url = Get-NugetApiUrl -Path 'FindPackagesById()' -Query ('id=''' + $Identifier + '''');
    $WebRequest = New-WebRequest -Uri $Url;
    $WebResponse = Get-WebResponse -WebRequest $WebRequest -AllowRedirect $true;
    $Encoding = [System.Text.Encoding]::UTF8;
    if ($WebResponse.ContentType -ne $null -and $WebResponse.ContentType.CharSet -ne $null -and $WebResponse.ContentType.CharSet.Trim().Length -gt 0) {
        $e = $Encoding;
        try { $e = [System.Text.Encoding]::GetEncoding($WebResponse.ContentType.CharSet) } catch { }
        if ($e -ne $null) { $Encoding = $e }
    }
    $Stream = $WebResponse.Response.GetResponseStream();
    $XmlDocment = New-Object -TypeName 'System.Xml.XmlDocument';
    try {
        $XmlDocment.Load($Stream);
    } finally { $Stream.Dispose() }

    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
    $XmlNamespaceManager.AddNamespace('a', 'http://www.w3.org/2005/Atom');
    $XmlNamespaceManager.AddNamespace('d', 'http://schemas.microsoft.com/ado/2007/08/dataservices');
    $XmlNamespaceManager.AddNamespace('m', 'http://schemas.microsoft.com/ado/2007/08/dataservices/metadata');
    $XmlNamespaceManager.AddNamespace('georss', 'http://www.georss.org/georss');
    $XmlNamespaceManager.AddNamespace('gml', 'http://www.opengis.net/gml');
    $Results = @($XmlDocument.SelectNodes('/a:feed/a:entry', $XmlNamespaceManager) | ForEach-Object {
        $EntryElement = $_;
        $Properties = @{};
        $XmlElement = $_.SelectSingleNode('a:id', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) { $Properties['id'] = $XmlElement.InnerText }
        $XmlElement = $_.SelectSingleNode('a:title', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) { $Properties['title'] = $XmlElement.InnerText }
        $XmlElement = $_.SelectSingleNode('a:summary', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) { $Properties['summary'] = $XmlElement.InnerText }
        $XmlElement = $_.SelectSingleNode('a:updated', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) {
            $s = $XmlElement.InnerText.Trim();
            if ($s.Length -gt 0) { try { $Properties['updated'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
        }
        $XmlElement = $_.SelectSingleNode('a:author', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) {
            $p = @{};
            foreach ($e in $XmlElement.SelectNodes('a:*', $XmlNamespaceManager)) {
                if (-not $e.IsEmpty) { $p[$e.LocalName] = $e.InnerText }
            }
            if ($p.Count -gt 0) { $Properties['author'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p }
        }
        
        $XmlElement = $_.SelectSingleNode('a:content', $XmlNamespaceManager);
        if ($XmlElement -ne $null) {
            $p = @{};
            foreach ($a in $XmlElement.SelectNodes('@*')) {
                $p[([System.Xml.XmlConvert]::EncodeLocalName($a.Name))] = $a.Value;
            }
            if ($p.Count -gt 0) { $Properties['content'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p }
        }
        $XmlElement = $_.SelectSingleNode('m:properties', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) {
            $p = @{};
            foreach ($e in $XmlElement.SelectNodes('d:*', $XmlNamespaceManager)) {
                if (-not $e.IsEmpty) {
                    switch ($e.LocalName) {
                        'Created' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['Created'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'LastUpdated' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['LastUpdated'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'Published' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['Published'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'LastEdited' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['LastEdited'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'IsLatestVersion' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['IsLatestVersion'] = [System.Xml.XmlConvert]::ToBoolean($s) } catch { } }
                            break;
                        }
                        'IsAbsoluteLatestVersion' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['IsAbsoluteLatestVersion'] = [System.Xml.XmlConvert]::ToBoolean($s) } catch { } }
                            break;
                        }
                        'IsPrerelease' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['IsPrerelease'] = [System.Xml.XmlConvert]::ToBoolean($s) } catch { } }
                            break;
                        }
                        'PackageSize' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['PackageSize'] = [System.Xml.XmlConvert]::ToInt64($s) } catch { } }
                            break;
                        }
                        default {
                            $p[$e.LocalName] = $e.InnerText;
                            break;
                        }
                    }
                }
            }
            if ($p.Count -gt 0) { $Properties['properties'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties
    });
    if ($LatestVersion) {
        $Results | Where-Object { $_.properties.IsLatestVersion  }
    } else {
        if ($PSBoundParameters.ContainsKey('Version')) {
            $Results | Where-Object { $_.properties.Version -eq $Version  }
        } else {
            $Results | Write-Output;
        }
    }
}