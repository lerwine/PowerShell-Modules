Add-Type -Path @(
    'C:\Users\lerwi\Documents\Visual Studio 2015\Projects\PowerShell-Modules\PSModuleInstallUtil\InstallationLocationInfo.cs',
    'C:\Users\lerwi\Documents\Visual Studio 2015\Projects\PowerShell-Modules\PSModuleInstallUtil\InstallLocationSelectForm.cs',
    'C:\Users\lerwi\Documents\Visual Studio 2015\Projects\PowerShell-Modules\PSModuleInstallUtil\InstallLocationSelectForm.Designer.cs'
) -ReferencedAssemblies 'System', 'System.Drawing', 'System.Windows.Forms';


Add-Type -AssemblyName 'System.Web.Services'
$Form = New-Object -TypeName 'PSModuleInstallUtil.InstallLocationSelectForm';
try {
    $InstallationLocationInfo = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property @{
        Name = 'MyModule';
        ParentDirectory = 'C:\Users\lerwi\Documents\WindowsPowerShell\Modules';
        RelativePath = '\Users\lerwi\Documents\WindowsPowerShell\Modules';
        Reason = '';
        Exists = $false;
        IsInstallable = $true;
        IsAllUsers = $false;
        ExpectDirectory = $true;
    };
    $Form.AddInstallationLocationInfo($InstallationLocationInfo);
    $InstallationLocationInfo = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property @{
        Name = 'MyModule';
        ParentDirectory = 'C:\Program Files\WindowsPowerShell\Modules';
        RelativePath = '\Program Files\WindowsPowerShell\Modules';
        Reason = 'File found where directory expected.';
        Exists = $false;
        IsInstallable = $false;
        IsAllUsers = $true;
        ExpectDirectory = $true;
    };
    $Form.AddInstallationLocationInfo($InstallationLocationInfo);
    $InstallationLocationInfo = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property @{
        Name = 'MyModule';
        ParentDirectory = 'C:\Windows\system32\WindowsPowerShell\v1.0\Modules';
        RelativePath = '\Windows\system32\WindowsPowerShell\v1.0\Modules';
        Reason = '';
        Exists = $true;
        IsInstallable = $true;
        IsAllUsers = $true;
        ExpectDirectory = $true;
    };
    $Form.AddInstallationLocationInfo($InstallationLocationInfo);
    $DialogResult = $Form.ShowDialog();
} catch {
    throw;
} finally {
    $Form.Dispose();
}