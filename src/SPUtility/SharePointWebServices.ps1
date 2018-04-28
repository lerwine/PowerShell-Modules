Add-Type -AssemblyName 'System.Web';
Add-Type -AssemblyName 'System.Web.Services';

$Script:AllMimeMediaTypes = @{};

Function Read-WebResponse {
    [CmdletBinding(DefaultParameterSetName = 'GET')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Alias('WebResponse')]
        [System.Net.WebResponse]$Response,

        [switch]$Wsdl
    )

    Process {
        $Properties = @{};
        if (-not [System.String]::IsNullOrEmpty($Response.ContentType)) {
            try {
                $Properties['ContentType'] = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $Response.ContentType;
            } catch { }
        }
        if ($Response.ContentLength -gt 0) {
            if ($Properties['ContentType'] -eq $null) {
                $Properties['ContentType'] = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList ([System.Net.Mime.MediaTypeNames+Application]::Octet);
            }
            if (-not [System.String]::IsNullOrEmpty($Response.CharacterSet)) {
                try {
                    $Properties['CharSet'] = [System.Text.Encoding]::GetEncoding($Response.CharacterSet);
                } catch { }
            }
            if ($Properties['CharSet'] -eq $null -and -not [System.String]::IsNullOrEmpty($Properties['ContentType'].CharSet)) {
                try {
                    $Properties['CharSet'] = [System.Text.Encoding]::GetEncoding($Properties['ContentType'].CharacterSet);
                } catch { }
            }
            if ($Properties['CharSet'] -eq $null) { $Properties['CharSet'] = [System.Text.Encoding]::UTF8 }

            switch ($Properties['ContentType'].MediaType) {
                { $_ -eq [System.Net.Mime.MediaTypeNames+Application]::Soap } {
                    $IsText = $true;
                    $ParseAsXml = $true;
                    break;
                }
                { $_ -eq [System.Net.Mime.MediaTypeNames+Text]::Plain } {
                    $IsText = $true;
                    $ParseAsXml = $false;
                    break;
                }
                { $_ -eq [System.Net.Mime.MediaTypeNames+Text]::Html } {
                    $IsText = $true;
                    $ParseAsXml = $false;
                    break;
                }
                { $_ -eq [System.Net.Mime.MediaTypeNames+Text]::Xml } {
                    $IsText = $true;
                    $ParseAsXml = $true;
                    break;
                }
                { $_ -eq [System.Net.Mime.MediaTypeNames+Text]::RichText } {
                    $IsText = $true;
                    $ParseAsXml = $false;
                    break;
                }
                default {
                    $IsText = $Properties['ContentType'].MediaType.StartsWith('text/');
                    $ParseAsXml = $false;
                    break;
                }
            }
            
            if ($Response.ContentLength -gt 0) {
                $Stream = $Response.GetResponseStream();
                try {
                    if ($IsText) {
                        $StreamReader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Stream, $Properties['CharSet'];
                        try {
                            if ($ParseAsXml) {
                                if ($Wsdl) {
                                    $Properties['Content'] = [System.Web.Services.Description.ServiceDescription]::Read($StreamReader);
                                } else {
                                    $Properties['Content'] = New-Object -TypeName 'System.Xml.XmlDocument';
                                    try {
                                        $Properties['Content'].Load($StreamReader);
                                    } catch {
                                        $Properties.Remove('Content');
                                        throw;
                                    }
                                }
                            } else {
                                $Properties['Content'] = $StreamReader.ReadToEnd();
                            }
                        } catch {
                            throw;
                        } finally {
                            $StreamReader.Dispose();
                        }
                    } else {
                        if ($Response.ContentLength -lt [int]::MaxValue) {
                            $Properties['Content'] = New-Object -TypeName 'System.Byte[]' -ArgumentList $Response.ContentLength;
                            $Stream.Read($Properties['Content'], 0, $Response.ContentLength) | Out-Null;
                        } else {
                            $MemoryStream = New-Object -TypeName 'System.IO.MemoryStream';
                            $Buffer = New-Object -TypeName 'System.Byte[]' -ArgumentList 4096;
                            $Length = $Stream.Read($Buffer, 0, $Buffer.Length);
                            while ($Length -eq $Buffer.Length) {
                                $MemoryStream.Write($Buffer, 0, $Buffer.Length);
                                $Length = $Stream.Read($Buffer, 0, $Buffer.Length);
                            }
                            if ($Length -gt 0) { $MemoryStream.Write($Buffer, 0, $Length) }
                            $Properties['Content'] = $MemoryStream.ToArray();
                        }
                    }
                } catch {
                    throw;
                } finally {
                    $Stream.Dispose();
                }
            }
        }

        $ResponseContent = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
        $ResponseContent.TypeNames.Insert(0, 'Erwine.Leonard.T.WebClient.ResponseContent');
        $ResponseContent | Write-Output;
    }
}

Function Get-MimeMediaTypes {
    [CmdletBinding(DefaultParameterSetName = 'AsString')]
    Param(
        [ValidateSet('Application', 'Image', 'Text', 'Text')]
        [string[]]$Names,
        
        [Parameter(ParameterSetName = 'AsHashtable')]
        [switch]$AsHashtable,
        
        [Parameter(ParameterSetName = 'AsContentType')]
        [switch]$AsContentType,
        
        [Parameter(ParameterSetName = 'AsString')]
        [switch]$AsString
        
    )

    $ContentTypeStrings = @{};
    if ($Names.Count -eq 0 -or $Names -icontains 'Application') {
        if ($Script:AllMimeMediaTypes.ContainsKey('Application')) {
            $ContentTypeStrings['Application'] = $Script:AllMimeMediaTypes['Application'];
        } else {
            $ContentTypeStrings['Application'] = @{};
            [System.Net.Mime.MediaTypeNames+Application].GetFields() | ForEach-Object {
                $ContentTypeStrings['Application'][$_.Name] = [System.Net.Mime.MediaTypeNames+Application]::($_.Name);
            }
            $Additional = @{
                FormUlrEncoded = 'application/x-www-form-urlencoded'
            };
            $Additional.Keys | ForEach-Object {
                if ($ContentTypeStrings.Values -inotcontains $Additional[$_]) {
                    $ContentTypeStrings['Application'][$_] = $Additional[$_];
                }
            }
            $Script:AllMimeMediaTypes['Application'] = $ContentTypeStrings['Application'];
        }
    }
    if ($Names.Count -eq 0 -or $Names -icontains 'Image') {
        if ($Script:AllMimeMediaTypes.ContainsKey('Application')) {
            $ContentTypeStrings['Image'] = $Script:AllMimeMediaTypes['Image'];
        } else {
            $ContentTypeStrings['Image'] = @{};
            [System.Net.Mime.MediaTypeNames+Application].GetFields() | ForEach-Object {
                $ContentTypeStrings['Image'][$_.Name] = [System.Net.Mime.MediaTypeNames+Application]::($_.Name);
            }
            $Additional = @{
                FormUlrEncoded = 'application/x-www-form-urlencoded'
            };
            $Additional.Keys | ForEach-Object {
                if ($ContentTypeStrings.Values -inotcontains $Additional[$_]) {
                    $ContentTypeStrings['Application'][$_] = $Additional[$_];
                }
            }
            $Script:AllMimeMediaTypes['Application'] = $ContentTypeStrings['Application'];
        }
        [System.Net.Mime.MediaTypeNames+Image].GetFields() | ForEach-Object { [System.Net.Mime.MediaTypeNames+Image]::($_.Name) }
    }
    if ($Names.Count -eq 0 -or $Names -icontains 'Text') {
        [System.Net.Mime.MediaTypeNames+Text].GetFields() | ForEach-Object { [System.Net.Mime.MediaTypeNames+Text]::($_.Name) }
    }
}

Function Set-FormPostData {
    [CmdletBinding(DefaultParameterSetName = 'GET')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Alias('Request')]
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Post_FormData')]
        [Hashtable]$FormData
    )

    $WebRequest.ContentType = 'application/x-www-form-urlencoded';
    
}

Function Get-HttpWebResponse {
    [CmdletBinding(DefaultParameterSetName = 'GET')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ $_.IsAbsoluteUri -and ($_.Scheme -eq 'http' -or $_.Scheme -eq 'https') })]
        [Alias('Url')]
        [System.Uri]$Uri,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Post_FormData')]
        [Hashtable]$FormData,

        [Parameter(Mandatory = $true, ParameterSetName = 'Post_XmlData')]
        [System.Xml.XmlDocument]$Xml,

        [bool]$UseDefaultCredentials,
        
        [string]$Accept,

        [Alias('Certificates', 'X509Certificates', 'Certificate', 'X509Certificate', 'X509Certificate2')]
        [System.Security.Cryptography.X509Certificates.X509Certificate[]]$ClientCertificates,

        [Alias('PSCredential')]
        [System.Management.Automation.PSCredential]$Credentials,
        
        [Parameter(ParameterSetName = 'GET')]
        [switch]$GET,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Post_FormData')]
        [Parameter(Mandatory = $true, ParameterSetName = 'Post_XmlData')]
        [switch]$POST
    )

    Process {
        $Properties = @{
            ExceptionStatus = [System.Net.WebExceptionStatus]::UnknownError;
            StatusCode = [System.Net.HttpStatusCode]::Unused;
            StatusDescription = 'Request Not Sent';
        };
        try {
            $Properties['Uri'] = $Uri;
            $Properties['Request'] = [System.Net.WebRequest]::CreateHttp($Uri.OriginalString);
            if ($PSBoundParameters.ContainsKey('Accept')) { $Properties['Request'].Accept = $Accept }
            if ($PSBoundParameters.ContainsKey('UseDefaultCredentials')) { $Properties['Request'].UseDefaultCredentials = $UseDefaultCredentials }
            if ($PSBoundParameters.ContainsKey('ClientCertificates')) {
                $Properties['Request'].ClientCertificates.AddRange($ClientCertificates);
            }
            if ($PSBoundParameters.ContainsKey('Credentials')) { $Properties['Request'].Credentials = $Credentials.GetNetworkCredential() }
            $Properties['Response'] = $Properties['Request'].GetResponse();
            $Properties['StatusCode'] = $Properties['Response'].StatusCode;
            $Properties['StatusDescription'] = $Properties['Response'].StatusDescription;
            $Properties['ExceptionStatus'] = [System.Net.WebExceptionStatus]::Success;
        } catch [System.Net.WebException] {
            $Properties['Error'] = $_;
            if ($_ -is [System.Net.WebException]) { [System.Net.WebException]$WebException = $_ } else { [System.Net.WebException]$WebException = $_.Exception }
            $Properties['Response'] = $WebException.Response;
            if ($Properties['Response'] -eq $null) {
                $Properties['StatusDescription'] = 'Request not received.';
            } else {
                $Properties['StatusCode'] = $Properties['Response'].StatusCode;
                $Properties['StatusDescription'] = $Properties['Response'].StatusDescription;
            }
            $Properties['ExceptionStatus'] = $WebException.Status;
        } catch {
            $Properties['Error'] = $_;
            if ($_ -is [System.Exception]) { [System.Exception]$Exception = $_ } else { [System.Exception]$Exception = $_.Exception }
            if ($Exception -ne $null -and $Exception -is [System.Management.Automation.MethodException] -and $Exception.InnerException -ne $null) {
                $Exception = $Exception.InnerException;
                if ($Exception.ErrorRecord -eq $null) {
                    $Properties['Error'] = $Exception.ErrorRecord;
                } else {
                    $Properties['Error'] = $Exception;
                }
            }
            if ($Properties['Error'] -is [System.Exception]) {
                if ([System.String]::IsNullOrEmpty($Properties['Error'].Message)) {
                    if ($_.ErrorDetails -ne $null -and -not [System.String]::IsNullOrEmpty($_.ErrorDetails.Message)) {
                        $Properties['StatusDescription'] = $_.ErrorDetails.Message;
                    } else {
                        if ($_.CategoryInfo -ne $null) {
                            $Properties['StatusDescription'] = $_.CategoryInfo.GetMessage();
                        } else {
                            $Properties['StatusDescription'] = '';
                        }
                        if ([System.String]::IsNullOrEmpty($Properties['StatusDescription'])) {
                            $Properties['StatusDescription'] = $_ | Out-String;
                        }
                    }
                }
                $Properties['StatusDescription'] = $Properties['Error'].Message;
            } else {
                if ($Properties['Error'].ErrorDetails -ne $null -and -not [System.String]::IsNullOrEmpty($Properties['Error'].ErrorDetails.Message)) {
                    $Properties['StatusDescription'] = $Properties['Error'].ErrorDetails.Message;
                } else {
                    if ($Properties['Error'].CategoryInfo -ne $null) {
                        $Properties['StatusDescription'] = $Properties['Error'].CategoryInfo.GetMessage();
                    } else {
                        $Properties['StatusDescription'] = '';
                    }
                    if ([System.String]::IsNullOrEmpty($Properties['StatusDescription'])) {
                        if ($Properties['Error'].Exception -ne $null -and -not [System.String]::IsNullOrEmpty($Properties['Error'].Exception.Message)) {
                            $Properties['StatusDescription'] = $Properties['Error'].Exception.Message;
                        } else {
                            $Properties['StatusDescription'] = $Properties['Error'] | Out-String;
                        }
                    }
                }
            }
        }
        if ([System.String]::IsNullOrEmpty($Properties['StatusDescription'])) {
            $Properties['StatusDescription'] = $Properties['StatusCode'].ToString('F');
        }
            
        $HttpWebResponse = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
        $HttpWebResponse.TypeNames.Insert(0, 'Erwine.Leonard.T.WebClient.WebResponse');
        $HttpWebResponse.TypeNames.Insert(0, 'Erwine.Leonard.T.WebClient.HttpWebResponse');
        $HttpWebResponse | Write-Output;
    }
}

Function Read-SharePointWebServiceWsdl {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ $_.IsAbsoluteUri -and ($_.Scheme -eq 'http' -or $_.Scheme -eq 'https') })]
        [Alias('WebServiceUrl', 'WebServiceUri', 'Url')]
        [System.Uri]$Uri,

        [bool]$UseDefaultCredentials,
        
        [Alias('Certificates', 'X509Certificates', 'Certificate', 'X509Certificate', 'X509Certificate2')]
        [System.Security.Cryptography.X509Certificates.X509Certificate[]]$ClientCertificates,

        [Alias('PSCredential')]
        [System.Management.Automation.PSCredential]$Credentials
    )

    Process {
        $splat = @{};
        foreach ($key in $PSBoundParameters.Keys) { $splat[$key] = $PSBoundParameters[$key] }
        $splat['GET'] = [System.Management.Automation.SwitchParameter]::Present;
        $splat['Accept'] = 'text/xml';
        $UriBuilder = New-Object -TypeName 'System.UriBuilder' -ArgumentList $Uri;
        $UriBuilder.Query = 'WSDL';
        $splat['Uri'] = $UriBuilder.Uri;
        $HttpWebResponse = Get-HttpWebResponse @splat;

        if ($HttpWebResponse.Error -eq $null -and $HttpWebResponse.Response -ne $null) {
            Read-WebResponse -Response $HttpWebResponse.Response -Wsdl;
        } else {
            $HttpWebResponse | Write-Output;
        }
    }
}

$ServiceDescription = 
#$WsdlImport = New-Object -TypeName 'System.Web.Services.Description.Import';

if ($PSCredential -eq $null) { $PSCredential = Get-Credential }
$WsdlResponse = Read-SharePointWebServiceWsdl -Uri 'http://win-52jfnf2doeh/_vti_bin/SiteData.asmx' -Credentials $PSCredential;
$ServiceDescriptionImporter = New-Object -TypeName 'System.Web.Services.Description.ServiceDescriptionImporter';
$ServiceDescriptionImporter.ProtocolName = 'Soap12';
AddServiceDescription(System.Web.Services.Description.ServiceDescription serviceDescription, string appSettingUrlKey, string appSettingBaseUrl)
$ServiceDescriptionImporter.AddServiceDescription(