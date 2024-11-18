using System.Text.Json.Nodes;
using SwPackage.VsCodeVsix;

namespace SwPackage.UnitTests;

public class RawGalleryExtensionPublisherUnitTest
{
    [SetUp]
    public void Setup()
    {
    }


    [Test]
    public void TryCreateTest()
    {
        var actual = RawGalleryExtensionPublisher.TryCreate(JsonNode.Parse("""
{
                        "publisherId": "eed56242-9699-4317-8bc7-e9f4b9bdd3ff",
                        "publisherName": "redhat",
                        "displayName": "Red Hat",
                        "flags": "verified",
                        "domain": "https://redhat.com",
                        "isDomainVerified": true
                    }
""")!.AsObject(), out RawGalleryExtensionPublisher? publisher);
        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.True);
            Assert.That(publisher, Is.Not.Null);
            Assert.That(publisher!.PublisherId, Is.EqualTo("eed56242-9699-4317-8bc7-e9f4b9bdd3ff"));
            Assert.That(publisher!.PublisherName, Is.EqualTo("redhat"));
            Assert.That(publisher!.DisplayName, Is.EqualTo("Red Hat"));
            Assert.That(publisher!.IsDomainVerified, Is.True);
        });
    }
}