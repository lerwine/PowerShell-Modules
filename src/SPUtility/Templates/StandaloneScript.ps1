$Script:SiteUrl = 'http://{0}' -f $env:COMPUTERNAME;

if ((Get-PSSnapin -Name 'Microsoft.SharePoint.PowerShell' -ErrorAction Continue) -eq $null) { Add-PSSnapin -Name 'Microsoft.SharePoint.PowerShell' -ErrorAction Stop }

Function Invoke-CrawlWebs {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Guid]$WebId
    )

    Process {
        [Microsoft.SharePoint.SPWeb]$SPWeb = Get-SPWeb -Identity $WebId -ErrorAction Stop;
        $SPWeb.ServerRelativeUrl | Write-Output;
        $IDs = @(if ($SPWeb.Webs.Count -gt 0) { for ($w = 0; $w -lt $SPWeb.Webs.Count; $w++) { $SPWeb.Webs[$w].ID | Write-Output } });
        $SPWeb = $null;
        if ($IDs.Count -gt 0) { $IDs | Invoke-CrawlWebs }
    }
}

$SPSite = Get-SPSite -Identity $Script:SiteUrl -ErrorAction Stop;
$SPWeb = $SPSite.OpenWeb();
$ID = $SPWeb.ID;
$SPWeb = $null;
$SPSite = $null;
$ID | Invoke-CrawlWebs;
