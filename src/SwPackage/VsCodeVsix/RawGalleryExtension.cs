using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Json.More;

namespace SwPackage.VsCodeVsix;

// Was RawVsCodeGalleryExtension
public partial class RawGalleryExtension
{
    private string _extensionId;

    private string _extensionName;
    private string _displayName;
    private RawGalleryExtensionPublisher _publisher;
    private VersionCollection _versions;

    public string ExtensionId
    {
        get => _extensionId; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _extensionId = value;
        }
    }

    public string ExtensionName
    {
        get => _extensionName; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _extensionName = value;
        }
    }

    public string DisplayName
    {
        get => _displayName; set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _displayName = value;
        }
    }

    public string? ShortDescription { get; set; }

    public RawGalleryExtensionPublisher Publisher
    {
        get => _publisher; set
        {
            ArgumentNullException.ThrowIfNull(value);
            _publisher = value;
        }
    }

    public VersionCollection Versions
    {
        get => _versions; set
        {
            ArgumentNullException.ThrowIfNull(value);
            _versions = value;
        }
    }

    public Collection<string>? Tags { get; set; }

    public DateTime ReleaseDate { get; set; }

    public DateTime PublishedDate { get; set; }

    public DateTime LastUpdated { get; set; }

    public Collection<string>? Categories { get; set; }

    public RawGalleryExtension(string extensionId, string extensionName, RawGalleryExtensionPublisher publisher, string displayName, DateTime releaseDate,
        DateTime publishedDate, DateTime lastUpdated, params RawGalleryExtensionVersion[] versions)
    {
        _extensionId = extensionId;
        _extensionName = extensionName;
        _publisher = publisher;
        _displayName = displayName;
        ReleaseDate = releaseDate;
        PublishedDate = publishedDate;
        LastUpdated = lastUpdated;
        _versions = [];
        if (versions is not null)
            foreach (RawGalleryExtensionVersion v in versions)
            {
                if (v is null) throw new ArgumentNullException(nameof(versions));
                _versions.Add(v);
            }
    }

    public static IEnumerable<RawGalleryExtension> FromWebResponseObject(Microsoft.PowerShell.Commands.WebResponseObject response)
    {
        bool isValidResponse = false;
        if (!string.IsNullOrWhiteSpace(response.RawContent) && JsonNode.Parse(response.RawContent) is JsonObject jsonObj &&
            jsonObj.TryGetJsonArrayProperty("results", out JsonArray? results))
        {
            if (results.Count == 0)
                isValidResponse = true;
            else
                foreach (JsonNode? ri in results)
                {
                    if (ri is JsonObject obj && obj.TryGetJsonArrayProperty("extensions", out JsonArray? extensions))
                    {
                        if (extensions.Count == 0)
                            isValidResponse = true;
                        else
                            foreach (JsonNode? ei in extensions)
                            {
                                if (ei is JsonObject extItem && TryCreate(extItem, out RawGalleryExtension? galleryExt))
                                {
                                    isValidResponse = true;
                                    yield return galleryExt;
                                }
                            }
                    }
                }
        }
        if (!isValidResponse)
            throw new InvalidOperationException("Invalid response data");
    }

    private static bool TryCreate(JsonObject extensionJson, [NotNullWhen(true)] out RawGalleryExtension? result)
    {
        if (extensionJson.TryGetJsonStringProperty("extensionId", out string? extensionId) &&
            extensionJson.TryGetJsonStringProperty("extensionName", out string? extensionName) &&
            extensionJson.TryGetJsonStringProperty("displayName", out string? displayName) &&
            extensionJson.TryGetJsonDateTimeProperty("lastUpdated", out DateTime lastUpdated) &&
            extensionJson.TryGetJsonDateTimeProperty("publishedDate", out DateTime publishedDate) &&
            extensionJson.TryGetJsonDateTimeProperty("releaseDate", out DateTime releaseDate) &&
            extensionJson.TryGetJsonObjectProperty("publisher", out JsonObject? publisherJson) &&
            RawGalleryExtensionPublisher.TryCreate(publisherJson, out RawGalleryExtensionPublisher? publisher) &&
            extensionJson.TryGetJsonArrayProperty("versions", out JsonArray? jsonArray))
        {
            result = new(extensionId, extensionName, publisher, displayName, lastUpdated, publishedDate, releaseDate);
            if (extensionJson.TryGetJsonStringProperty("shortDescription", out string? shortDescription))
                result.ShortDescription = shortDescription;
            if (RawGalleryExtensionVersion.TryAddVersions(jsonArray, result.Versions))
            {
                if (extensionJson.TryGetJsonArrayProperty("categories", out jsonArray) && jsonArray.Count > 0)
                {
                    result.Categories = [];
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JsonNode? node = jsonArray[i];
                        if (node is null) continue;
                        if (node is not JsonValue jv)
                        {
                            result = null;
                            return false;
                        }
                        string? s = jv.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            result.Categories.Add(s);
                    }
                    if (result.Categories.Count == 0)
                        result.Categories = null;
                }
                if (extensionJson.TryGetJsonArrayProperty("tags", out jsonArray) && jsonArray.Count > 0)
                {
                    result.Tags = [];
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JsonNode? node = jsonArray[i];
                        if (node is null) continue;
                        if (node is not JsonValue jv)
                        {
                            result = null;
                            return false;
                        }
                        string? s = jv.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            result.Tags.Add(s);
                    }
                    if (result.Tags.Count == 0)
                        result.Tags = null;
                }
                return true;
            }

        }
        result = null;
        return false;
    }
}