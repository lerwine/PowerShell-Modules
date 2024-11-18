using System.Management.Automation;

namespace SwPackage.VsCodeVsix;

// Was ScannedVsCodeExtensionFile
public class ScannedExtensionFile : ExtensionFile
{
    private string _displayName;

    public string DisplayName
    {
        get => _displayName; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _displayName = value;
        }
    }

    public string? Description { get; set; }

    public string? IconUri { get; set; }

    public object[]? IconData { get; set; }

    public ScannedExtensionFile(string publisher, string name, SemanticVersion version, TargetPlatform targetPlatform, string psPath, string displayName)
        : base(publisher, name, version, targetPlatform, psPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        _displayName = displayName;
    }
}
