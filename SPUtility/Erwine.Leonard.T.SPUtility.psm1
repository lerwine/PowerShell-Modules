if ((Get-PSSnapin 'Microsoft.SharePoint.PowerShell') -eq $null) {
    if ((Get-PSSnapin 'Microsoft.SharePoint.PowerShell' -Registered) -eq $null) {
        throw ('The "Microsoft.SharePoint.PowerShell" Snapin was not found.');
    }
    if ((Add-PSSnapin -Name 'Microsoft.SharePoint.PowerShell' -PassThru) -eq $null) {
        throw ('The "Microsoft.SharePoint.PowerShell" Snapin could not be loaded.');
    }
}

$Script:MySitesUrl = $MyInvocation.MyCommand.Module.PrivateData.MySitesUrl;

function Get-Function {
	
}