Add-Type -AssemblyName 'System.Web';
Add-Type -AssemblyName 'System.Web.Services';

$Script:Regex = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    EndingNewline = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '(\r\n?|\n)$', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
    UrlEncodedItem = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '(^|&)(?<key>[^&=]*)(=(?<value>[^&]*))?', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
};

Function Test-ContentType {
    [CmdletBinding(DefaultParameterSetName = 'Validate')]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Validate')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'String_String')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'String_ContentType')]
        [AllowNull()]
        [AllowEmptyString()]
        [string[]]$InputString,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ContentType_String')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ContentType_ContentType')]
        [AllowNull()]
        [System.Net.Mime.ContentType[]]$InputObject,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'String_String')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ContentType_String')]
        [ValidateScript({ Test-ContentType -InputString $_ })]
        [string]$Expected,

        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'String_ContentType')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ContentType_ContentType')]
        [System.Net.Mime.ContentType]$ContentType,
        
        [Parameter(ParameterSetName = 'String_String')]
        [Parameter(ParameterSetName = 'String_ContentType')]
        [Parameter(ParameterSetName = 'ContentType_String')]
        [Parameter(ParameterSetName = 'ContentType_ContentType')]
        [switch]$MediaTypeOnly,
        
        [Parameter(ParameterSetName = 'Validate')]
        [switch]$AllowEmpty
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Validate') {
            if ([System.String]::IsNullOrEmpty($InputString)) {
                $AllowEmpty.IsPresent | Write-Output;
            } else {
                try { $c = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $InputString } catch { $c = $null }
                ($c -ne $null) | Write-Output;
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Expected')) {
                $ContentType = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $Expected;
            }
            $success = $true;
            if ($PSBoundParameters.ContainsKey('InputString')) {
                if ($InputString -eq $null -or $InputString.Length -eq 0) {
                    $success = $false;
                } else {$ContentType = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $str
                    if ($MediaTypeOnly) {
                        foreach ($str in $InputString) {
                            if ([System.String]::IsNullOrEmpty($str)) { $success = $false; break; }
                            $c = $null;
                            try { $c = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $str } catch { }
                            if ($c -eq $null -or -not (Test-ContentType -InputObject $c -ContentType $ContentType -MediaTypeOnly)) { $success = $false; break; }
                        }
                    } else {
                        foreach ($str in $InputString) {
                            if ([System.String]::IsNullOrEmpty($str)) { $success = $false; break; }
                            $c = $null;
                            try { $c = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $str } catch { }
                            if ($c -eq $null -or -not (Test-ContentType -InputObject $c -ContentType $ContentType)) { $success = $false; break; }
                        }
                    }
                }
            } else {
                if ($InputObject -eq $null -or $InputObject.Length -eq 0) {
                    $success = $false;
                } else {
                    if ($MediaTypeOnly) {
                        foreach ($c in $InputObject) {
                            if ($c -eq $null -or $c.MediaType -ne $ContentType.MediaType) { $success = $false; break; }
                        }
                    } else {
                        foreach ($c in $InputObject) {
                            if ($c -eq $null -or $c.MediaType -ne $ContentType.MediaType) { $success = $false; break; }
                            if ([System.String]::IsNullOrEmpty($ContentType.CharSet)) {
                                if (-not [System.String]::IsNullOrEmpty($c.CharSet)) { $success = $false; break; }
                            } else {
                                if ([System.String]::IsNullOrEmpty($c.CharSet) -or $c.CharSet -ne $ContentType.CharSet) { $success = $false; break; }
                            }
                        }
                    }
                }
            }
        
            $success | Write-Output;
        }
    }
}

Function New-WebRequest {
    [CmdletBinding(DefaultParameterSetName = 'PSCredential')]
    [OutputType([System.Net.WebRequest])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Uri]$Uri,

        [System.Net.Cache.RequestCacheLevel]$CachePolicy,

        [string]$Method,

        [string]$ConnectionGroupName,

        [Hashtable]$Headers,

        [bool]$PreAuthenticate,

        [ValidateRange(0, 2147483647)]
        [int]$Timeout,

        [System.Net.Security.AuthenticationLevel]$AuthenticationLevel,

        [System.Security.Principal.TokenImpersonationLevel]$ImpersonationLevel,

        [Parameter(ParameterSetName = 'PSCredential')]
        [System.Management.Automation.PSCredential]$PSCredential,

        [Parameter(Mandatory = $true, ParameterSetName = 'ICredentials')]
        [System.Net.ICredentials]$Credentials,

        [Parameter(Mandatory = $true, ParameterSetName = 'UseDefaultCredentials')]
        [switch]$UseDefaultCredentials
    )

    Process {
        $WebRequest = [System.Net.WebRequest]::Create($Uri);
        if ($WebRequest -ne $null) {
            $WebRequest.UseDefaultCredentials = $UseDefaultCredentials;
            if ($PSBoundParameters.ContainsKey('CachePolicy')) { $WebRequest.CachePolicy = New-Object -TypeName 'System.Net.Cache.RequestCachePolicy' -ArgumentList $CachePolicy }
            if ($PSBoundParameters.ContainsKey('Method')) { $WebRequest.Method = $Method }
            if ($PSBoundParameters.ContainsKey('ConnectionGroupName')) { $WebRequest.ConnectionGroupName = $ConnectionGroupName }
            if ($PSBoundParameters.ContainsKey('PreAuthenticate')) { $WebRequest.PreAuthenticate = $PreAuthenticate }
            if ($PSBoundParameters.ContainsKey('Timeout')) { $WebRequest.Timeout = $Timeout }
            if ($PSBoundParameters.ContainsKey('AuthenticationLevel')) { $WebRequest.AuthenticationLevel = $AuthenticationLevel }
            if ($PSBoundParameters.ContainsKey('ImpersonationLevel')) { $WebRequest.ImpersonationLevel = $ImpersonationLevel }
            if ($PSBoundParameters.ContainsKey('PSCredential')) { $WebRequest.Credentials = $PSCredential.GetNetworkCredential() }
            if ($PSBoundParameters.ContainsKey('Credentials')) { $WebRequest.Credentials = $Credentials }

            if ($PSBoundParameters.ContainsKey('Headers') -and $Headers.Count -gt 0) {
                $Headers.Keys | ForEach-Object {
                    if ($_ -is [string]) { $Key = $_ } else { $Key = $Script:Regex.EndingNewline.Replace(($_ | Out-String), '') }
                    if ($Headers[$_] -eq $null) {
                        $WebRequest.Headers.Add($Key, '');
                    } else {
                        if ($Headers[$_] -is [string]) {
                            $WebRequest.Headers.Add($Key, $Headers[$_]);
                        } else {
                            $WebRequest.Headers.Add($Key, $Script:Regex.EndingNewline.Replace(($Headers[$_] | Out-String), ''));
                        }
                    }
                }
            }
        }
    }
}

Function Read-FormUrlEncoded {
    [CmdletBinding(DefaultParameterSetName = 'InputString')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'InputString')]
        [AllowEmptyString()]
        [string]$InputString,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'TextReader')]
        [System.IO.TextReader]$Reader,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Stream')]
        [System.IO.Stream]$Stream,

        [Parameter(Position = 1, ParameterSetName = 'Stream')]
        [System.Text.Encoding]$Encoding,
        
        [switch]$Hashtable
	)

	Process {
        if ($Hashtable) {
            $splat = @{};
            foreach ($k in $PSBoundParameters.Keys) {
                if ($k -ne 'Hashtable') { $splat.Add($k, $PSBoundParameters[$k]) }
            }
            $OutputHashtable = @{};
            Read-FormUrlEncoded @splat | ForEach-Object {
                if ($OutputHashtable.ContainsKey($_.Key)) {
                    $OutputHashtable[$_.Key] = @($OutputHashtable[$_.Key]) + @($_.Value);
                } else {
                    $OutputHashtable[$_.Key] = $_.Value;
                }
            }
            $OutputHashtable | Write-Output;
        } else {
            switch ($PSCmdlet.ParameterSetName) {
                'TextReader' {
                    Read-FormUrlEncoded -InputString $Reader.ReadToEnd();
                    break;
                }
                'Stream' {
                    if ($PSBoundParameters.ContainsKey('Encoding')) {
                        $Reader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Stream, $true;
                    } else {
                        $Reader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Stream, $Encoding;
                    }
                    try {
                        Read-FormUrlEncoded -Reader $Reader;
                    } catch {
                        throw;
                    } finally {
                        $Reader.Dispose();
                    }
                    break;
                }
                default {
                    if ($InputString.Length -gt 0) {
		                if ($InputString[0] -eq '?') {
                            [System.Text.RegularExpressions.MatchCollection]$MatchCollection = $Script:Regex.UrlEncodedItem.Matches($InputString.Substring(1));
                        } else {
                            [System.Text.RegularExpressions.MatchCollection]$MatchCollection = $Script:Regex.UrlEncodedItem.Matches($InputString);
			            }
                        for ($i = 0; $i -lt $MatchCollection.Count; $i++) {
                            $Propertes = @{
                                Index = $i;
                                $Key = [System.Uri]::UnescapeDataString($MatchCollection[$i].Groups['key'].Value);
                            }
                            if ($MatchCollection[$i].Groups['value'].Success) {
                                $Propertes['Value'] = [System.Uri]::UnescapeDataString($MatchCollection[$i].Groups['value'].Value);
                            } else {
                                $Propertes['Value'] = $null;
                            }
                            $Item = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Propertes;
                            $Item.TypeNames.Insert(0, 'Erwine.Leonard.T.NetworkUtility.FormValue');
                            $Item | Write-Output;
                        }
                    }
                    break;
                }
            }
        }
	}
}

Function New-TextWriter {
    [CmdletBinding(DefaultParameterSetName = 'StringWriter')]
    Param(
        [Parameter(Position = 0, ParameterSetName = 'StringWriter')]
        [System.Text.StringBuilder]$StringBuilder,
        
        [Parameter(Position = 1, ParameterSetName = 'StringWriter')]
        [System.IFormatProvider]$FormatProvider,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Stream')]
        [System.IO.Stream]$Stream,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Path')]
        [string]$Path,
        
        [Parameter(Position = 1, ParameterSetName = 'Stream')]
        [Parameter(Position = 1, ParameterSetName = 'Path')]
        [System.Text.Encoding]$Encoding = [System.Text.Encoding]::UTF8,
        
        [Parameter(Position = 2, ParameterSetName = 'Stream')]
        [Parameter(Position = 2, ParameterSetName = 'Path')]
        [int]$BufferSize = 32767,

        [Parameter(Position = 3, ParameterSetName = 'Stream')]
        [switch]$LeaveOpen,

        [Parameter(Position = 3, ParameterSetName = 'Path')]
        [switch]$Append
    )

    switch ($PSCmdlet.ParameterSetName) {
        'Path' {
            if ($PSBoundParameters.ContainsKey('BufferSize')) {
                New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Append.IsPresent, $Encoding, $BufferSize;
            } else {
                if ($PSBoundParameters.ContainsKey('Encoding')) {
                    New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Append.IsPresent, $Encoding;
                } else {
                    if ($PSBoundParameters.ContainsKey('Append')) {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Append.IsPresent;
                    } else {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream;
                    }
                }
            }
            break;
        }
        'Stream' {
            if ($PSBoundParameters.ContainsKey('LeaveOpen')) {
                New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding, $BufferSize, $LeaveOpen.IsPresent;
            } else {
                if ($PSBoundParameters.ContainsKey('BufferSize')) {
                    New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding, $BufferSize;
                } else {
                    if ($PSBoundParameters.ContainsKey('Encoding')) {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding;
                    } else {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream;
                    }
                }
            }
            break;
        }
        default {
            if ($PSBoundParameters.ContainsKey('StringBuilder')) {
                if ($PSBoundParameters.ContainsKey('FormatProvider')) {
                    New-Object -TypeName 'System.IO.StringWriter' -ArgumentList $StringBuilder, $FormatProvider;
                } else {
                    New-Object -TypeName 'System.IO.StringWriter' -ArgumentList $StringBuilder;
                }
            } else {
                if ($PSBoundParameters.ContainsKey('FormatProvider')) {
                    New-Object -TypeName 'System.IO.StringWriter' -ArgumentList $FormatProvider;
                } else {
                    New-Object -TypeName 'System.IO.StringWriter';
                }
            }
        }
    }
}

Function Write-FormUrlEncoded {
    [CmdletBinding(DefaultParameterSetName = 'KeyValue_OutString')]
    Param(
    )
}

Function Write-FormUrlEncoded2 {
    [CmdletBinding(DefaultParameterSetName = 'KeyValue_OutString')]
    [OutputType([string], ParameterSetName = 'KeyValue_OutString')]
    [OutputType([string], ParameterSetName = 'Hashtable_OutString')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_OutString')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_Stream')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_TextWriter')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_WebRequest')]
        [AllowEmptyCollection()]
        [Hashtable]$Data,
		
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_OutString')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_Stream')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_TextWriter')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_WebRequest')]
		[AllowEmptyString()]
        [object]$Key,
		
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_OutString')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_Stream')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_TextWriter')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_WebRequest')]
		[AllowNull()]
        [object]$Value,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_OutString')]
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_Stream')]
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_TextWriter')]
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_WebRequest')]
        [int]$Index = 0,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_Stream')]
        [Parameter(Mandatory = $true, ParameterSetName = 'KeyValue_Stream')]
        [System.IO.Stream]$Stream,
        
        [Parameter(ParameterSetName = 'Hashtable_Stream')]
        [Parameter(ParameterSetName = 'KeyValue_Stream')]
        [Parameter(ParameterSetName = 'Hashtable_WebRequest')]
        [Parameter(ParameterSetName = 'KeyValue_WebRequest')]
        [System.Text.Encoding]$Encoding,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_TextWriter')]
        [Parameter(Mandatory = $true, ParameterSetName = 'KeyValue_TextWriter')]
        [System.IO.TextWriter]$Writer,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_WebRequest')]
        [Parameter(Mandatory = $true, ParameterSetName = 'KeyValue_WebRequest')]
        [System.Net.WebRequest]$WebRequest
	)

    Begin {
        $AllItems =  @();
        $ParameterSetParts = $PSCmdlet.ParameterSetName.Split('_');
    }

	Process {
		if ($ParameterSetParts[0] -eq 'Hashtable') {
            foreach ($Key in $Data.Keys) {
                $Value = $Data[$Key];
                if ($Value -ne $null) {
                    $AllItems += New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                        Key = $Key;
                        Value = $null;
                    };
                } else {
                    $Value = @($Value);
                    if ($Value.Count -eq 0) {
                        $AllItems += New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                            Key = $Key;
                            Value = '';
                        };
                    } else {
                        $Value | ForEach-Object {
                            $AllItems += New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                                Key = $Key;
                                Value = $_;
                            };
                        }
                    }
                }
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Index')) {
                $AllItems += New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                    Key = $Key;
                    Index = $Index;
                    Value = $Value;
                };
            } else {
                $AllItems += New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                    Key = $Key;
                    Value = $Value;
                };
            }
		}
	}

    End {
        if ($AllItems.Count -gt 0) {
            switch ($ParameterSetParts[1]) {
                'OutString' {
                    $TextWriter = New-Object -TypeName 'System.IO.StringWriter';
                    break;
                }
                'Stream' {
                    if ($PSBoundParameters.ContainsKey('Encoding')) {
                        $TextWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding;
                    } else {
                        $TextWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, ([System.Text.Encoding]::UTF8);
                    }
                    break;
                }
                'TextWriter' {
                    $TextWriter = $Writer;
                    break;
                }
                default {
                    if ($ParameterSetParts[1] -eq 'WebRequest') {
                        $Stream = $WebRequest.GetRequestStream();
                        [long]$TotalLength = 0;
                    }
                    if (-not $PSBoundParameters.ContainsKey('Encoding')) { $Encoding = [System.Text.Encoding]::UTF8 }
                    $TextWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding;
                    break;
                }
            }
        
            $IndexedItems = $AllItems | Where-Object { $_.Index -ne $null }
            if ($IndexedItems -ne $null) {
                $AllItems = @($IndexedItems | Sort-Object -Property 'Index') + @($AllItems | Where-Object { $_.Index -eq $null });
            }
            
            try {
                if ($PSCmdlet.ParameterSetName -eq 'KeyValueIndexed') { $AllItems = $AllItems | Sort-Object -Property 'Index' }
                for ($i = 0; $i -lt $AllItems.Count; $i++) {
                    if ($i -gt 0) {
                        if ($ParameterSetParts[1] -eq 'WebRequest') { [long]$TotalLength = $TotalLength + ([long]($Encoding.GetByteCount('&'))) }
                        $TextWriter.Write('&');
                    }
                    if ($AllItems[$i].Key -ne $null) {
                        if ($AllItems[$i].Key -is [string]) {
                            $Key = $AllItems[$i].Key;
                        } else {
                            $Key = $Script:Regex.EndingNewline.Replace(($AllItems[$i].Key | Out-String), '');
                        }
                        if ($Key -ne '') {
                            $Key = [System.Uri]::EscapeDataString($Key);
                            if ($ParameterSetParts[1] -eq 'WebRequest') { [long]$TotalLength = $TotalLength + ([long]($Encoding.GetByteCount($Key))) }
                            $TextWriter.Write($Key) | Out-Null;
                        }
                    }
                    if ($AllItems[$i].Value -ne $null) {
                        $TextWriter.Write('=') | Out-Null;
                        if ($ParameterSetParts[1] -eq 'WebRequest') { [long]$TotalLength = $TotalLength + ([long]($Encoding.GetByteCount('='))) }
                        if ($AllItems[$i].Value -is [string]) {
                            $Value = $AllItems[$i].Value;
                        } else {
                            $Value = $Script:Regex.EndingNewline.Replace(($AllItems[$i].Value | Out-String), '');
                        }
                        if ($Value -ne '') {
                            $Value = [System.Uri]::EscapeDataString($Value);
                            if ($ParameterSetParts[1] -eq 'WebRequest') { [long]$TotalLength = $TotalLength + ([long]($Encoding.GetByteCount($Value))) }
                            $TextWriter.Write($Value) | Out-Null;
                        }
                    }
                }
                $TextWriter.Flush();
                switch ($ParameterSetParts[1]) {
                    'OutString' {
                        $TextWriter.ToString();
                        break;
                    }
                    'WebRequest' {
                        $ContentType = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList 'application/x-www-form-urlencoded';
                        $ContentType.CharSet = $Encoding.BodyName;
                        $WebRequest.ContentType = $ContentType.ToString();
                        $WebRequest.ContentLength = $TotalLength;
                    }
                }
            } catch {
                throw;
            } finally {
                if (-not $PSBoundParameters.ContainsKey('TextWriter')) { $TextWriter.Dispose() }
                if (-not $PSBoundParameters.ContainsKey('Stream')) { $Stream.Dispose() }
            }
        }
    }
}

Function Write-XmlData {
    [CmdletBinding(DefaultParameterSetName = 'WebRequestEncoding')]
    [OutputType([string], ParameterSetName = 'String')]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ ($_.NodeType -eq [System.Xml.XmlNodeType]::Document -and $_.DocumentElement -ne $null) -or $_.NodeType -eq [System.Xml.XmlNodeType]::Element })]
        [System.Xml.XmlNode]$XmlData,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Stream')]
        [System.IO.Stream]$Stream,
        
        [Parameter(ParameterSetName = 'Stream')]
        [Parameter(ParameterSetName = 'WebRequestEncoding')]
        [System.Text.Encoding]$Encoding,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'WebRequestContentType')]
        [System.Net.Mime.ContentType]$ContentType,

        [Parameter(Mandatory = $true, ParameterSetName = 'Writer')]
        [System.IO.TextWriter]$Writer,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'WebRequestEncoding')]
        [Parameter(Mandatory = $true, ParameterSetName = 'WebRequestContentType')]
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(ParameterSetName = 'String')]
        [Parameter(ParameterSetName = 'Stream')]
        [Parameter(ParameterSetName = 'WebRequestContentType')]
        [System.Xml.XmlWriterSettings]$Settings,

        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [switch]$AsString
    )
    
    switch ($PSCmdlet.ParameterSetName) {
        'Stream' {
            if ($PSBoundParameters.ContainsKey('Settings')) {
                $Writer = [System.Xml.XmlWriter]::Create($Stream, $Settings);
            } else {
                $XmlData.WriteTo($Stream);
            }
            break;
        }
        'String' {
            $Stream = New-Object -TypeName 'System.IO.MemoryStream';
            if ($PSBoundParameters.ContainsKey('Settings')) {
                $Settings = $Settings.Clone();
            } else {
                $Settings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
            }
            $Settings.CloseOutput = $true;
            $Writer = [System.Xml.XmlWriter]::Create($Stream, $Settings);
            break;
        }
        'WebRequestEncoding' {
            $Stream = $WebRequest.GetRequestStream();
            if ($PSBoundParameters.ContainsKey('Settings')) {
                $Settings = $Settings.Clone();
            } else {
                $Settings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
            }
            $Settings.Encoding = $Encoding;
            $Settings.CloseOutput = $true;
            $Writer = [System.Xml.XmlWriter]::Create($Stream, $Settings);
        }
        'WebRequestContentType' {
            $Stream = $WebRequest.GetRequestStream();
            if (-not [System.String]::IsNullOrEmpty($ContentType.CharSet)) {
                try { $Encoding = [System.Text.Encoding]::GetEncoding($ContentType.CharSet) } catch { }
            }
            if ($Encoding -eq $null) { $Encoding = [System.Text.Encoding]::UTF8 }
            $Settings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
                Indent = $Indent;
                CloseOutput = $true;
                Encoding = $Encoding;
            }
            $Writer = [System.Xml.XmlWriter]::Create($Stream, $Settings);
        }
    }
    
    if ($Writer -ne $null) {
        try {
            $XmlData.WriteTo($Writer);
            $Writer.Flush();
            switch ($PSCmdlet.ParameterSetName) {
                'String' {
                    $Settings.Encoding.GetString($Stream.ToArray());
                    break;
                }
                'WebRequestEncoding' {
                    
                }
                'WebRequestContentType' {
                }
            }
        } catch {
            throw;
        } finally {
            if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer.Dispose() }
            if ($Stream -ne $null -and -not $PSBoundParameters.ContainsKey('Stream')) { $Stream.Close() }
        }
    }
}

Function Initialize-WebRequestPostXml {
    [CmdletBinding(DefaultParameterSetName = 'GetRequest')]
    [OutputType([System.Net.WebRequest], ParameterSetName = 'GetRequest')]
    [OutputType([System.Management.Automation.PSObject], ParameterSetName = 'GetResponse')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlNode]$XmlData,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetRequest')]
        [switch]$PassThru,
        
        [Parameter(ParameterSetName = 'GetResponse')]
        [bool]$AllowRedirect,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetResponse')]
        [switch]$GetResponse
    )
    
    Process {
        $WebRequest.Method = 'POST';
        
        $WebRequest.ContentType = 'text/xml;charset=utf-8';
        $Stream = $WebRequest.GetRequestStream();
        try {
            $XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
                Indent = $true;
                CloseOutput = $true;
                Encoding = [System.Text.Encoding]::UTF8;
                OmitXmlDeclaration = $true;
            };
            $XmlWriter = [System.Xml.XmlWriter]::Create($Stream, $XmlWriterSettings);
            try {
                $XmlData.WriteTo($XmlWriter);
            } catch {
                throw;
            } finally {
                $XmlWriter.Flush();
                $Stream.Flush();
                $WebRequest.ContentLength = [int]$Stream.Position;
                $XmlWriter.Close();
                $XmlWriter = $null;
                $Stream = $null;
            }
        } catch {
            throw;
        } finally {
            if ($Stream -ne $null) { $Stream.Dispose() }
        }
        if ($GetResponse) {
            if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
            $WebRequest.GetResponse() | Write-Output;
        } else {
            if ($PassThru) { $WebRequest | Write-Output };
        }
    }
}

Function Get-WebResponse {
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.WebRequest]$WebRequest,
        
        [bool]$AllowRedirect
    )
    
    Process {
        if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
        $Response = @{
            Request = $WebRequest;
            ErrorStatus = [System.Net.WebExceptionStatus]::UnknownError;
            StatusCode = [System.Management.Automation.PSInvocationState]::NotStarted;
            Success = $false;
            StatusDescription = 'Request not sent.';
        };
        try {
            $Response['Response'] = $WebRequest.GetResponse();
            $Response['ErrorStatus'] = [System.Net.WebExceptionStatus]::Success;
            if ($Response['Response'].StatusCode -ne $null) {
                $Response['StatusCode'] = $Response['Response'].StatusCode;
                $Response['Success'] = $SuccessCodes -contains $Response['StatusCode'];
            } else {
                $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Completed;
                $Response['Success'] = $true;
            }
            if ($Response['Response'].StatusDescription -eq $null -or $Response['Response'].StatusDescription.Trim() -eq '') {
                $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
            } else {
                $Response['StatusDescription'] = $Response['Response'].StatusDescription;
            }
            
        } catch [System.Net.WebException] {
            $Response['ErrorStatus'] = $_.Exception.Status;
            $Response['Response'] = $_.Exception.Response;
            $Response['Error'] = $_;
            if ($Response['Response'] -ne $null -and $Response['Response'].StatusCode -ne $null) {
                $Response['StatusCode'] = $Response['Response'].StatusCode;
            } else {
                $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Failed;
            }
            if ($Response['Response'] -eq $null -or $Response['Response'].StatusDescription -eq $null -or $Response['Response'].StatusDescription.Trim() -eq '') {
                if ($_.ErrorDetails -eq $null -or $_.ErrorDetails.Message -eq $null -or $_.ErrorDetails.Message.Trim() -eq '') {
                    if ($_.Exception -eq $null -or $_.Exception.Message -eq $null -or $_.Exception.Message.Trim() -eq '') {
                        $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                    } else {
                        $Response['StatusDescription'] = $_.Exception.Message;
                    }
                } else {
                    $Response['StatusDescription'] = $_.ErrorDetails.Message;
                }
            } else {
                $Response['StatusDescription'] = $Response['Response'].StatusDescription;
            }
        } catch [System.Net.Sockets.SocketException] {
            $Response['StatusCode'] = $_.SocketErrorCode;
            $Response['Error'] = $_;
            if ($_.ErrorDetails -eq $null -or $_.ErrorDetails.Message -eq $null -or $_.ErrorDetails.Message.Trim() -eq '') {
                if ($_.Exception -eq $null -or $_.Exception.Message -eq $null -or $_.Exception.Message.Trim() -eq '') {
                    $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                } else {
                    $Response['StatusDescription'] = $_.Exception.Message;
                }
            } else {
                $Response['StatusDescription'] = $_.ErrorDetails.Message;
            }
        } catch {
            $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Failed;
            if ($_.Exception -ne $null -and $_.Exception -is [System.Management.Automation.MethodException]) {
                if ($_.Exception.ErrorRecord -ne $null) {
                    $Response['Error'] = $_.Exception.ErrorRecord;
                } else {
                    $Response['Error'] = $_;
                }
                if ($_.Exception.InnerException -ne $null) {
                    $e = $_.Exception.InnerException;
                } else {
                    $e = $null;
                }
            } else {
                $e = $null;
            }
            if ($e -ne $null -and $e.Message -ne $null -and $e.Message.Trim() -ne '') {
                $Response['StatusDescription'] = $e.Message;
            } else {
                if ($Response['Error'].ErrorDetails -eq $null -or $Response['Error'].ErrorDetails.Message -eq $null -or $Response['Error'].ErrorDetails.Message.Trim() -eq '') {
                    if ($Response['Error'].Exception -eq $null -or $Response['Error'].Exception.Message -eq $null -or $Response['Error'].Exception.Message.Trim() -eq '') {
                        $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                    } else {
                        $Response['StatusDescription'] = $Response['Error'].Exception.Message;
                    }
                } else {
                    $Response['StatusDescription'] = $Response['Error'].ErrorDetails.Message;
                }
            }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Parameter $Response;
        if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
        $WebRequest.GetResponse() | Write-Output;
    }
}

Function Test-SoapEnvelope {
    [CmdletBinding(DefaultParameterSetName = 'XmlNamespaceManager')]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [Parameter(Mandatory = $true, ParameterSetName = 'XmlNamespaceManager')]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'PSObject' {
                if ($SoapEnvelope.XmlDocument -eq $null) { $false } else { $SoapEnvelope | Test-SoapEnvelope }
                break;
            }
            'Properties' {
                if ($PSBoundParameters.ContainsKey('XmlNamespaceManager')) {
                    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
                    $XmlNamespaceManager.AddNamespace('xsd', 'http://www.w3.org/2001/XMLSchema');
                    $XmlNamespaceManager.AddNamespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance');
                    $XmlNamespaceManager.AddNamespace('soap12', 'http://www.w3.org/2003/05/soap-envelope');
                }
                if (Test-SoapEnvelope -XmlNamespaceManager $XmlNamespaceManager) {
                    $XPath = '/{0}:Envelope/{0}:Body' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
                    if ($XmlDocument.SelectNodes($XPath, $XmlNamespaceManager).Count -eq 1) { $true } else { $false }
                } else {
                    $false;
                }
            }
            default {
                if ($XmlNamespaceManager.HasNamespace('http://www.w3.org/2001/XMLSchema') -and $XmlNamespaceManager.HasNamespace('http://www.w3.org/2001/XMLSchema-instance') -and `
                        $XmlNamespaceManager.HasNamespace('http://www.w3.org/2003/05/soap-envelope')) {
                    $true
                } else {
                    $false;
                }
            }
        }
    }
}

Function Get-SoapXmlNamespacePrefix {
    [CmdletBinding(DefaultParameterSetname = 'PropertiesSoap')]
    [OutputType([System.Xml.XmlDocument])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectSoap')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectSchema')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectInstance')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesSoap')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesSchema')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesInstance')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager,
        
        [Parameter(ParameterSetname = 'PropertiesSoap')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectSoap')]
        [switch]$Soap,
        
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectSchema')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PropertiesSchema')]
        [switch]$Schema,
        
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectInstance')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PropertiesInstance')]
        [switch]$SchemaInstance
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'PSObjectSoap' { $SoapEnvelope | Get-SoapBodyElement -Soap; break; }
            'PSObjectSchema' { $SoapEnvelope | Get-SoapBodyElement -Schema; break; }
            'PSObjectInstance' { $SoapEnvelope | Get-SoapBodyElement -SchemaInstance; break; }
            'PropertiesSoap' { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope'); break; }
            'PropertiesSchema' { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2001/XMLSchema'); break; }
            default { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2001/XMLSchema-instance'); break; }
        }
    }
}

Function Add-SoapBodyElement {
    [CmdletBinding(DefaultParameterSetName = 'Properties')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetname = 'PSObject')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Properties')]
        [System.Xml.XmlElement[]]$Body
    )
    
    Begin { if ($PSCmdlet.ParameterSetName -eq 'PSObject') { $BodyElements = @() } }
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'PSObject') {
            $BodyElements = $BodyElements + $Body;
        } else {
            $XPath = '/{0}:Envelope/{0}:Body' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
            $BodyElement = $XmlDocument.SelectSingleNode($XPath, $XmlNamespaceManager);
            foreach ($XmlElement in $Body) { $BodyElement.AppendChild($XmlDocument.ImportNode($XmlElement)) }
        }
    }
    
    End { if ($PSCmdlet.ParameterSetName -eq 'PSObject') { $SoapEnvelope | Add-SoapBodyElement -Body $BodyElements } }
}

Function Get-SoapBodyElement {
    [CmdletBinding(DefaultParameterSetName = 'Properties')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager
    )
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'PSObject') {
            $SoapEnvelope | Get-SoapBodyElement
        } else {
            $XPath = '/{0}:Envelope/{0}:Body/*' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
            $XmlNodeList = $XmlDocument.SelectNodes($XPath, $XmlNamespaceManager);
            for ($n = 0; $n -lt $XmlNodeList.Count; $n++) { $XmlNodeList.Item($n) }
        }
    }
}

Function New-SoapEnvelope {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlDocument])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Xml.XmlElement[]]$Body,
    )
    
    [Xml]$XmlDocument = @'
<?xml version="1.0" encoding="utf-8"?>
<soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
  <soap12:Body />
</soap12:Envelope>
'@;
    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $SoapEnvelope.NameTable;
    $XmlNamespaceManager.AddNamespace('xsd', 'http://www.w3.org/2001/XMLSchema');
    $XmlNamespaceManager.AddNamespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance');
    $XmlNamespaceManager.AddNamespace('soap12', 'http://www.w3.org/2003/05/soap-envelope');
    
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ XmlDocument = $XmlDocument; XmlNamespaceManager = $XmlNamespaceManager; }
}