using System.Management.Automation;
using System.Text.Json.Nodes;

namespace SwPackage.VsCodeVsix;

// Was RawVsCodeGalleryExtensionVersion
public class RawGalleryExtensionVersion : IPlatformAndVersion, IEquatable<RawGalleryExtensionVersion>, IComparable<RawGalleryExtensionVersion>, IComparable
{
    private SemanticVersion _version;
    private Uri _assetUri;
    private TargetPlatform _platform;

    public SemanticVersion Version
    {
        get => _version; set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.PreReleaseLabel is null || value.BuildLabel is not null) throw new ArgumentOutOfRangeException(nameof(value));
            _version = value;
        }
    }

    public DateTime LastUpdated { get; set; }

    public Uri AssetUri { get => _assetUri; set => _assetUri = value ?? throw new ArgumentNullException(nameof(value)); }

    public Uri? FallbackAssetUri { get; set; }

    public TargetPlatform Platform
    {
        get => _platform; set
        {
            if (value == TargetPlatform.UNKNOWN) throw new ArgumentOutOfRangeException(nameof(value));
            _platform = value;
        }
    }

    public RawGalleryExtensionVersion(SemanticVersion version, DateTime lastUpdated, Uri assetUri, TargetPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(version);
        if (version.PreReleaseLabel is null || version.BuildLabel is not null) throw new ArgumentOutOfRangeException(nameof(version));
        if (platform == TargetPlatform.UNKNOWN) throw new ArgumentOutOfRangeException(nameof(platform));
        _assetUri = assetUri ?? throw new ArgumentNullException(nameof(assetUri));
        _version = version;
        LastUpdated = lastUpdated;
        _platform = platform;
    }

    public int CompareTo(RawGalleryExtensionVersion? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        int diff = _version.CompareTo(other._version);
        return (diff == 0 && (diff = _platform.CompareTo(other._platform)) == 0 && (diff = AssetUriComparer.Default.Compare(_assetUri, other._assetUri)) == 0) ? LastUpdated.CompareTo(other.LastUpdated) : diff;
    }

    public int CompareTo(IPlatformAndVersion? other)
    {
        if (other is null) return 1;
        if (other is RawGalleryExtension obj) return CompareTo(obj);
        if (ReferenceEquals(this, other)) return 0;
        int diff = _version.CompareTo(other.Version);
        return (diff != 0) ? diff : _platform.CompareTo(other.Platform);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is RawGalleryExtension other) return CompareTo(other);
        if (obj is IPlatformAndVersion pv)
        {
            int diff = _version.CompareTo(pv.Version);
            return (diff != 0) ? diff : _platform.CompareTo(pv.Platform);
        }
        if (obj is not string s)
        {
            s = obj.ToString()!;
            if (s is null) return 1;
        }
        return string.Compare(s, ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(RawGalleryExtensionVersion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _version.Equals(other._version) && _platform.Equals(other._platform) && AssetUriComparer.Default.Equals(_assetUri, other._assetUri) && LastUpdated.Equals(other.LastUpdated);
    }

    public bool Equals(IPlatformAndVersion? other)
    {
        if (other is null) return false;
        if (other is RawGalleryExtension obj) return Equals(obj);
        if (ReferenceEquals(this, other)) return true;
        return _version.Equals(other.Version) && _platform.Equals(other.Platform);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is RawGalleryExtension other) return Equals(other);
        if (obj is IPlatformAndVersion pv)
            return _version.Equals(pv.Version) && _platform.Equals(pv.Platform);
        if (obj is not string s)
        {
            s = obj.ToString()!;
            if (s is null) return false;
        }
        return string.Equals(s, ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(_version);
        hashCode.Add(_platform);
        hashCode.Add(_assetUri, AssetUriComparer.Default);
        hashCode.Add(LastUpdated);
        return hashCode.ToHashCode();
    }

    public override string ToString() => (_platform == TargetPlatform.UNIVERSAL) ? $"{_version}: {_assetUri}" :
        $"{_version}@{_platform.ToIdentifierString()}: {_assetUri}";

    public static bool TryAddVersions(JsonArray jsonArray, RawGalleryExtension.VersionCollection versions)
    {
        // "version": "0.27.2024111208",
        // "targetPlatform": "linux-x64",
        // "flags": "validated",
        // "lastUpdated": "2024-11-12T08:18:45.123Z",
        // "assetUri": "https://redhat.gallerycdn.vsassets.io/extensions/redhat/vscode-xml/0.27.2024111208/1731399103281",
        // "fallbackAssetUri": "https://redhat.gallery.vsassets.io/_apis/public/gallery/publisher/redhat/extension/vscode-xml/0.27.2024111208/assetbyname"
        throw new NotImplementedException();
    }
}
