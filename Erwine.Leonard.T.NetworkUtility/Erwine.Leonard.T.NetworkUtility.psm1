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
      if ($PSBoundParameters.ContainsKey('CachePolicy')) { $WebRequest.CachePolicy = New-Object -TypeName 'System.Net.Cache.RequestCachePolicy' -ArgumentList $CachePolicy }
      if ($PSBoundParameters.ContainsKey('Method')) { $WebRequest.Method = $Method }
    }
  }
}
