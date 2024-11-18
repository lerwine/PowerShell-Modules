using System.Management.Automation;

namespace SwPackage.VsCodeVsix;

// Was VsCodeExtensionFile
public class ExtensionFile : ExtensionBaseFileName
{
    private string _psPath;

    public string PSPath
    {
        get => _psPath; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _psPath = value;
        }
    }

    public ExtensionFile(string publisher, string name, SemanticVersion version, TargetPlatform targetPlatform, string psPath)
        : base(publisher, name, version, targetPlatform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(psPath);
        _psPath = psPath;
    }
}
