using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text.Json.Nodes;
using Json.More;
using SwPackage.VsCodeVsix;

namespace SwPackage.UnitTests;

static class TestData
{
    internal static JsonObject GetFindRedhatXmlResultRawJson() =>
        JsonNode.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData/redhat.vscode-xml__find_result.json")))!.AsObject();

    internal static JsonObject GetFindRedhatXmlResultExtensionJson() =>
        GetFindRedhatXmlResultRawJson()["results"]!.AsArray()[0]!["extensions"]!.AsArray()[0]!.AsObject();

    private static RawGalleryExtensionVersion ToRawGalleryExtensionVersion(JsonObject obj)
    {
        var item = new RawGalleryExtensionVersion(SemanticVersion.Parse(obj["version"]!.AsValue().GetString()!),
            DateTime.Parse(obj["lastUpdated"]!.AsValue().GetString()!),
            new Uri(obj["assetUri"]!.AsValue().GetString()!, UriKind.Absolute),
            (obj.TryGetPropertyValue("targetPlatform", out JsonNode? jsonNode) && jsonNode is not null) ?
                jsonNode.AsValue().GetString()!.ToTargetPlatform() : TargetPlatform.UNIVERSAL);
        if (obj.TryGetPropertyValue("fallbackAssetUri", out jsonNode))
            item.FallbackAssetUri = new(jsonNode!.AsValue().GetString()!, UriKind.Absolute);
        return item;
    }

    internal static Collection<RawGalleryExtensionVersion> GetFindRedhatXmlVersions(out JsonArray json)
    {
        Collection<RawGalleryExtensionVersion> result = [];
        json = GetFindRedhatXmlResultExtensionJson()["versions"]!.AsArray();
        foreach (var obj in json.Select(n => n!.AsObject()))
            result.Add(ToRawGalleryExtensionVersion(obj));
        return result;
    }

    internal static RawGalleryExtensionPublisher GetFindRedhatXmlPublisher(out JsonObject json)
    {
        json = GetFindRedhatXmlResultExtensionJson()["publisher"]!.AsObject();
        return ToRawGalleryExtensionPublisher(json);
    }

    private static RawGalleryExtensionPublisher ToRawGalleryExtensionPublisher(JsonObject json)
    {
        RawGalleryExtensionPublisher result = new(json["publisherId"]!.AsValue().GetString()!, json["publisherName"]!.AsValue().GetString()!, json["displayName"]!.AsValue().GetString()!);
        if (json.TryGetPropertyValue("isDomainVerified", out JsonNode? jsonNode))
            result.IsDomainVerified = jsonNode!.AsValue().GetBool();
        return result;
    }

    internal static RawGalleryExtension GetFindRedhatXmlExtension(out JsonObject json)
    {
        json = GetFindRedhatXmlResultExtensionJson();
        return ToRawGalleryExtension(json);
    }

    private static RawGalleryExtension ToRawGalleryExtension(JsonObject json)
    {
        RawGalleryExtension result = new(json["extensionId"]!.AsValue().GetString()!, json["extensionName"]!.AsValue().GetString()!,
            ToRawGalleryExtensionPublisher(json["publisher"]!.AsObject()!), json["displayName"]!.AsValue().GetString()!,
            DateTime.Parse(json["releaseDate"]!.AsValue().GetString()!), DateTime.Parse(json["publishedDate"]!.AsValue().GetString()!),
            DateTime.Parse(json["lastUpdated"]!.AsValue().GetString()!));
        foreach (var obj in GetFindRedhatXmlResultExtensionJson()["versions"]!.AsArray().Select(n => n!.AsObject()))
            result.Versions.Add(ToRawGalleryExtensionVersion(obj));
        if (json.TryGetPropertyValue("shortDescription", out JsonNode? jsonNode))
            result.ShortDescription = jsonNode!.AsValue().GetString();
        if (json.TryGetPropertyValue("categories", out jsonNode))
        {
            Collection<string> categories = [];
            foreach (var obj in jsonNode!.AsArray())
                categories.Add(obj!.AsValue().GetString()!);
            result.Categories = categories;
        }
        if (json.TryGetPropertyValue("tags", out jsonNode))
        {
            Collection<string> tags = [];
            foreach (var obj in jsonNode!.AsArray())
                tags.Add(obj!.AsValue().GetString()!);
            result.Tags = tags;
        }
        return result;
    }
}