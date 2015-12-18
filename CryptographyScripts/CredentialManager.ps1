Import-Module IOUtility;
Import-Module WindowsForms;
Import-Module CertificateCryptography;

$Script:ApplicationInfo = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    Company = 'ErwineLeonardT';
    Application = 'CredentialManager';
    Version = New-Object -TypeName 'System.Version' -ArgumentList 0, 1, 0, 0;
    SettingsFileName = 'Settings.xml';
    DefaultCredentialsFileName = 'Credentials.xml';
};

Function Get-AppDataPath {
    if ($Script:AppDataPath -ne $null) { return $Script:AppDataPath }
    
    $AppDataPath = Get-SpecialFolder -Folder ApplicationData;
    if ([System.String]::IsNullOrEmpty($AppDataPath)) {
        throw 'Unable to get application data path.';
        return;
    }
    
    if (-not ($AppDataPath | Test-Path -PathType Container)) {
        throw ('Application data path "{0}" does not exist. Cannot continue.' -f $AppDataPath);
        return;
    }
    
    $AppDataPath = $AppDataPath | Join-Path -ChildPath $Script:ApplicationInfo.Company;
    
    if (-not ($AppDataPath | Test-Path -PathType Container)) {
        New-Item -Path $AppDataPath -Type 'Directory' | Out-Null;
        if (-not ($AppDataPath | Test-Path -PathType Container)) {
            throw ('Unable to create Application Data path "{0}".' -f $AppDataPath);
            return;
        }
    }
    
    $AppDataPath = $AppDataPath | Join-Path -ChildPath ('{0}{1}' -f $Script:ApplicationInfo.Application, $Script:ApplicationInfo.Version.Major);
    
    if (-not ($AppDataPath | Test-Path -PathType Container)) {
        New-Item -Path $AppDataPath -Type 'Directory' | Out-Null;
        if (-not ($AppDataPath | Test-Path -PathType Container)) {
            throw ('Unable to create Application Data path "{0}".' -f $AppDataPath);
            return;
        }
    }
    
    $Script:AppDataPath = $AppDataPath;
    
    return $AppDataPath;
}

Function Get-ApplicationSettings {
    if ($Script:ApplicationSettings -ne $null) { return $Script:ApplicationSettings }
    
    $ApplicationSettings = @{
        Company = $Script:ApplicationInfo.Company;
        Application = $Script:ApplicationInfo.Application;
        Version = $Script:ApplicationInfo.Version;
    };
    
    $AppDataPath = Get-SpecialFolder -Folder ApplicationData;
    if ([System.String]::IsNullOrEmpty($AppDataPath)) {
        throw 'Unable to get application data path.';
        return;
    }
    
    if (-not ($AppDataPath | Test-Path -PathType Container)) {
        throw ('Application data path "{0}" does not exist. Cannot continue.' -f $AppDataPath);
        return;
    }
    
    $SettingsPath = Get-AppDataPath | Join-Path -ChildPath $Script:ApplicationInfo.SettingsFileName;
    
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
    
    if ($ApplicationSettings.CredentialsFilePath -eq $null) { $ApplicationSettings.CredentialsFilePath = Get-AppDataPath | Join-Path -ChildPath $Script:ApplicationInfo.DefaultCredentialsFileName }
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
