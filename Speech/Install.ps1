Function Get-SpecialFolderPaths
{
  Param(
    [Parameter(Mandatory = $True, ValueFromPipeline = $True)]
    [System.Environment+SpecialFolder]$SpecialFolder
  )
  Process {
    $Path = [System.Environment+SpecialFolder]::GetFolderPath($_);
    if (-not [System.String]::IsNullOrEmpty($Path)) { [System.IO.Path]::GetFullPath($Path) }
  }
}

$AllUsersFolders = @([System.Environment+SpecialFolder]::CommonAdminTools, [System.Environment+SpecialFolder]::CommonApplicationData,
  [System.Environment+SpecialFolder]::CommonProgramFiles, [System.Environment+SpecialFolder]::CommonProgramFilesX86,
  [System.Environment+SpecialFolder]::CommonPrograms, [System.Environment+SpecialFolder]::ProgramFiles,
  [System.Environment+SpecialFolder]::ProgramFilesX86, [System.Environment+SpecialFolder]::Programs) | Get-SpecialFolderPaths);
$CurrentUserFolders = @([System.Environment+SpecialFolder]::MyDocuments, [System.Environment+SpecialFolder]::Personal,
  [System.Environment+SpecialFolder]::UserProfile, [System.Environment+SpecialFolder]::LocalApplicationData,
  [System.Environment+SpecialFolder]::ApplicationData) | Get-SpecialFolderPaths);
  
$InstallLocations = $PSModulePath.Split([System.IO.Path]::DirectorySeparatorChar) | ForEach-Object {
  $Path =  [System.IO.Path]::GetFullPath($_);
  if (($CurrentUserFolders | Where-Object {
    $Path.Length -ge $_.Length -and [System.String]::Equals($_, $Path.Substring(0, $_.Length), [System.StringCompare]::InvariantCultureIgnoreCase)
  }) -ne $null) {
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
      Path = $Path;
      Type = 'Current User';
    };
  } else {
    if (($AllUsersFolders | Where-Object {
      $Path.Length -ge $_.Length -and [System.String]::Equals($_, $Path.Substring(0, $_.Length), [System.StringCompare]::InvariantCultureIgnoreCase)
    }) -ne $null) {
      New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        Path = $Path;
        Type = 'All Users';
      };
    } else {
      New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        Path = $Path;
        Type = 'Admin Users';
      };
    }
  }
};
if (@($InstallLocations | Where-Object { $_.Type -eq 'Admin Users' }).Count -eq 0)) {
  # TODO: Add Admin Users
  $PSHOME
}
if (@($InstallLocations | Where-Object { $_.Type -eq 'Current User' }).Count -eq 0)) {
  # TODO: Add Current User
  $HOME
}
if (@($InstallLocations | Where-Object { $_.Type -eq 'All Users' }).Count -eq 0)) {
  # TODO: Add All Users
  $AllUsersFolders[0]
}
$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]' -ArgumentList $Label, $Message;
