using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace SwPackage.VsCodeVsix;

// Was VsCodeExtensionBaseFileName
public partial class ExtensionBaseFileName : IPlatformAndVersion, IEquatable<ExtensionBaseFileName>
{
    [GeneratedRegex(@"^(?<pub>[a-z][a-z\d]*(?:-[a-z][a-z\d]*)*).(?<pkg>[a-z][a-z\d]*(?:-[a-z][a-z\d]*)*)-(?<v>\d+(?:\.\d+)*)(?:@(?<t>.+)?)?$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex GetBaseNameParseRegex();
    public static readonly Regex BaseNameParseRegex = GetBaseNameParseRegex();

    [GeneratedRegex(@"^[a-z\d]+(?:-[a-z\d]+)*$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex GetIdentifierRegex();

    public static readonly Regex IdentifierRegex = GetIdentifierRegex();

    private string _publisher;
    private string _name;
    private SemanticVersion _version;
    private TargetPlatform _targetPlatform;

    public string Publisher
    {
        get => _publisher; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if (!IdentifierRegex.IsMatch(value)) throw new ArgumentOutOfRangeException(nameof(value));
            _publisher = value;
        }
    }

    public string Name
    {
        get => _name; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            if (!IdentifierRegex.IsMatch(value)) throw new ArgumentOutOfRangeException(nameof(value));
            _name = value;
        }
    }

    public SemanticVersion Version
    {
        get => _version; set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.PreReleaseLabel is null || value.BuildLabel is not null) throw new ArgumentOutOfRangeException(nameof(value));
            _version = value;
        }
    }

    public TargetPlatform TargetPlatform
    {
        get => _targetPlatform; set
        {
            if (value == TargetPlatform.UNKNOWN) throw new ArgumentOutOfRangeException(nameof(value));
            _targetPlatform = value;
        }
    }

    TargetPlatform IPlatformAndVersion.Platform => TargetPlatform;

    public ExtensionBaseFileName(string publisher, string name, SemanticVersion version, TargetPlatform targetPlatform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publisher);
        if (!IdentifierRegex.IsMatch(publisher)) throw new ArgumentOutOfRangeException(nameof(publisher));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!IdentifierRegex.IsMatch(name)) throw new ArgumentOutOfRangeException(nameof(name));
        ArgumentNullException.ThrowIfNull(version);
        if (version.PreReleaseLabel is null || version.BuildLabel is not null) throw new ArgumentOutOfRangeException(nameof(version));
        if (targetPlatform == TargetPlatform.UNKNOWN) throw new ArgumentOutOfRangeException(nameof(targetPlatform));
        _publisher = publisher;
        _name = name;
        _version = version;
        _targetPlatform = targetPlatform;
    }

    int IComparable<IPlatformAndVersion>.CompareTo(IPlatformAndVersion? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        int diff = _version.CompareTo(other.Version);
        return (diff != 0) ? diff : _targetPlatform.CompareTo(other.Platform);
    }

    public bool Equals(ExtensionBaseFileName? other) => other is not null && (ReferenceEquals(this, other) ||
        (string.Equals(_publisher, other._publisher, StringComparison.OrdinalIgnoreCase) && string.Equals(_name, other._name, StringComparison.OrdinalIgnoreCase) &&
            _version.Equals(other._version) && _targetPlatform == other._targetPlatform));

    public bool Equals(IPlatformAndVersion? other)
    {
        if (other == null) return false;
        if (other is ExtensionBaseFileName obj) return Equals(obj);
        return _version.Equals(other.Version) && _targetPlatform == other.Platform;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj is ExtensionBaseFileName other) return Equals(other);
        if (obj is IPlatformAndVersion pV)
            return _version.Equals(pV.Version) && _targetPlatform == pV.Platform;
        if (obj is string s)
        {
            if (TryParse(s, out other!)) return Equals(other);
        }
        else
        {
            s = obj.ToString()!;
            if (s is null) return false;
        }
        return string.Equals(s, ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(_publisher, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(_name, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(_version);
        hashCode.Add(_targetPlatform);
        return hashCode.ToHashCode();
    }

    public override string ToString() => (_targetPlatform == TargetPlatform.UNIVERSAL) ? $"{_publisher}.{_name}-{_version}" :
        $"{_publisher}.{_name}-{_version}@{_targetPlatform.ToIdentifierString()}";

    public static ExtensionBaseFileName Parse(string baseName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseName);
        Match match = BaseNameParseRegex.Match(baseName);
        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(baseName), "Base name does not conform to VS Code VSIX package name format");

        if (SemanticVersion.TryParse(match.Groups["v"].Value, out SemanticVersion version))
        {
            Group g = match.Groups["t"];
            try
            {
                return new ExtensionBaseFileName(match.Groups["p"].Value, match.Groups["e"].Value, version,
                    g.Success ? g.Value.ToTargetPlatform() : TargetPlatform.UNIVERSAL);
            }
            catch (ArgumentException exc)
            {
                throw new ArgumentOutOfRangeException(nameof(baseName), exc.Message);
            }
        }
        throw new ArgumentOutOfRangeException(nameof(baseName), "Base name contains an invalid version component");
    }

    public static bool TryParse(string baseName, [MaybeNullWhen(false)] out ExtensionBaseFileName? result)
    {
        if (!string.IsNullOrEmpty(baseName))
        {
            Match match = BaseNameParseRegex.Match(baseName);
            if (match.Success)
            {
                if (SemanticVersion.TryParse(match.Groups["v"].Value, out SemanticVersion version))
                {
                    Group g = match.Groups["t"];
                    if (g.Success)
                    {
                        if (g.Value.TryConvertToTargetPlatform(out TargetPlatform targetPlatform))
                        {
                            result = new ExtensionBaseFileName(match.Groups["p"].Value, match.Groups["e"].Value, version, targetPlatform);
                            return true;
                        }
                    }
                    else
                    {
                        result = new ExtensionBaseFileName(match.Groups["p"].Value, match.Groups["e"].Value, version, TargetPlatform.UNIVERSAL);
                        return true;
                    }
                }
            }
        }
        result = null;
        return false;
    }
}
