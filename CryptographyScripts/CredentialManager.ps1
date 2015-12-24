#Requires –Version 2
('Erwine.Leonard.T.IOUtility', 'Erwine.Leonard.T.WindowsForms', 'Erwine.Leonard.T.CertificateCryptography') | ForEach-Object {
	if ((Get-Module -Name $_) -eq $null) { Import-Module -Name $_ -ErrorAction Stop }
}

$Script:ProductInfo = New-ProductInfo -Company 'ErwineLeonardT' -Product 'CredentialManager' -Version (New-Object -TypeName 'System.Version' -ArgumentList 0, 1, 0, 0) `
	-SettingsFileName 'Settings.xml' -DefaultCredentialsFileName 'Credentials.xml', 'two';

Function Get-ApplicationSettings {
    if ($Script:ApplicationSettings -ne $null) { return $Script:ApplicationSettings }
    
    $AppDataPath = $Script:ProductInfo | Get-AppDataPath -Create;
    $SettingsPath = Get-AppDataPath | Join-Path -ChildPath $Script:ProductInfo.SettingsFileName;
    
    if ($SettingsPath | Test-Path -PathType Leaf) {
        $Script:ApplicationSettings = Import-Clixml -Path $SettingsPath;
        $Script:ApplicationSettings.SettingsPath = $SettingsPath;
    } else {
        $Script:ApplicationSettings = @{ SettingsPath = $SettingsPath };
    }
    
    return $Script:ApplicationSettings;
}

Function Get-CredentialsFilePath {
    $ApplicationSettings = Get-ApplicationSettings;
    
    if ($ApplicationSettings.CredentialsFilePath -eq $null) {
		$ApplicationSettings.CredentialsFilePath = ($Script:ProductInfo | Get-AppDataPath -Create) | Join-Path -ChildPath $Script:ProductInfo.DefaultCredentialsFileName;
	}

    $ApplicationSettings.CredentialsFilePath;
}

Function Set-CredentialsFilePath {
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Path
    )
    (Get-ApplicationSettings).CredentialsFilePath = $Path;
}

Function Get-CredentialData {
    if ($Script:CredentialData -ne $null) { return $Script:CredentialData }
    
    $CredentialsFilePath = Get-CredentialsFilePath;
    
    if ($CredentialsFilePath | Test-Path -PathType Leaf) {
        $Script:CredentialData = Import-Clixml -Path $CredentialsFilePath;
    } else {
        $Script:CredentialData = @()
    }
    
    return $Script:CredentialData;
}

Function Show-CredentialBrowser {
    Param(
        [Parameter(Mandatory = $true)]
        [HashTable]$Current,

		[Parameter(Mandatory = $false)]
		[string[]]$Parent
    )

}