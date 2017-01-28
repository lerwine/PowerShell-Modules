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
			Get NuGet package information.
         
        .DESCRIPTION
			Returns a list of NuGet package information objects.
        
        .OUTPUTS
			System.PSObject[]. An array of package information objects.
    #>
    [CmdletBinding(DefaultParameterSetName = 'ByVersion')]
    [OutputType([PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'File')]
        # Path to NuGet package file.
        [string]$Path,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'ByVersion')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'LatestVersion')]
        # NuGet package identifier.
        [string]$Identifier,
        
        [Parameter(Position = 1, ParameterSetName = 'ByVersion')]
        # NuGet package version.
        [string]$Version,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'LatestVersion')]
        # Returns latest stable version.
        [switch]$LatestVersion
    )
    
    Process {
        if ($PSBoundParameters.ContainsKey('Path')) {
            $OuterProperties = @{ Path = $Path };
            $Properties = @{ };
            $ZipPackage = [System.IO.Packaging.ZipPackage]::Open($Path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read);
            $Stream = $null;
            $XmlDocument = $null;
            try {
                $Parts = @($ZipPackage.GetParts());
                $OuterProperties['BinaryExcutables'] = @($Parts | ForEach-Object { $_.Uri.ToString() } | Where-Object { $_ -imatch '\.(exe|dll|com)$' });
                $pkg = @($Parts | Where-Object { $_.Uri.ToString() -imatch '/([^.]*\.)+nuspec' });
                if ($pkg.Count -gt 0) {
                    $PackagePart = $pkg[0];
                    $Stream = $PackagePart.GetStream([System.IO.FileMode]::Open, [System.IO.FileAccess]::Read);
                    $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
                    $XmlDocument.Load($Stream);
                    $OuterProperties['nuspec'] = $XmlDocument;
                    
                    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
                    $XmlNamespaceManager.AddNamespace('n', $XmlDocument.DocumentElement.NamespaceURI);
                    $XmlNodeList = $XmlDocument.SelectNodes('/n:package/n:metadata/n:*', $XmlNamespaceManager);
                    @($XmlNodeList) | ForEach-Object {
                        $XmlElement = $_;
                        if (-not $XmlElement.IsEmpty) {
                            if (@('id', 'title', 'description') -contains $XmlElement.LocalName) {
                                $Properties[$XmlElement.LocalName] = $XmlElement.InnerText;
                                $OuterProperties[$XmlElement.LocalName] = $XmlElement.InnerText;
                            } else {
                                if ($XmlElement.LocalName -eq 'authors') {
                                    $Properties['authors'] = $XmlElement.InnerText;
                                    $OuterProperties['author'] = $XmlElement.InnerText;
                                } else {
                                    if ($XmlElement.LocalName -eq 'frameworkAssemblies') {
                                        $Properties['frameworkAssemblies'] = @(@($XmlElement.SelectNodes('n:frameworkAssembly', $XmlNamespaceManager)) | ForEach-Object {
                                            $p = @{};
                                            @($_.SelectNodes('@*')) | ForEach-Object { $p[$_.LocalName] = $_.Value }
                                            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p;
                                        });
                                    } else {
                                        $Properties[$XmlElement.LocalName] = $XmlElement.InnerText;
                                    }
                                }
                            }
                        }
                    }
                    if ($Properties.Count -gt 0) {
                        $OuterProperties['properties'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
                    } else {
                        <#
                            $MemoryStream = New-Object -TypeName 'System.IO.MemoryStream';
                            $XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{ Indent = $true; Encoding = [System.Text.Encoding]::UTF8 };
                            $XmlWriter = [System.Xml.XmlWriter]::Create($MemoryStream, $XmlWriterSettings);
                            $XmlDocument.WriteTo($XmlWriter);
                            $XmlWriter.Flush();
                            $XmlWriterSettings.Encoding.GetString($MemoryStream.ToArray());
                            $XmlWriter.Close();
                            $MemoryStream.Close();
                        #>
                        $OuterProperties['allparts'] = @($Parts | ForEach-Object { $_.Uri.ToString() });
                    }
                }
            }
            finally {
                try { if ($Stream -ne $null) { $Stream.Close() } }
                finally { $ZipPackage.Close() }
            }
            New-Object -TypeName 'System.Management.Automation.PSObject' -Property $OuterProperties;
        } else {
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
            $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
            try {
                $XmlDocument.Load($Stream);
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
    }
}

Function Test-PackageContainsExecutable {
    <#
        .SYNOPSIS
			Checks whether a NuGet package contains any executable binary files.
         
        .DESCRIPTION
			Returns true if NuGet package contains any binary executable files, including DLL files.
        
        .OUTPUTS
			System.Boolean. true if package contains binary executable; otherwise false.
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # Path to NuGet package.
        [string]$Path
    )
    
    Process {
        $ZipPackage = [System.IO.Packaging.ZipPackage]::Open($Path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read);
        try { @(@($ZipPackage.GetParts()) | Where-Object { $_.Uri.ToString() -imatch '\.(exe|dll|com)$' }).Count -gt 0 }
        finally { $ZipPackage.Close() }
    }
}
