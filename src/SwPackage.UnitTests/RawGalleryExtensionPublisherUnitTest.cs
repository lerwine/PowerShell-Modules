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
        RawGalleryExtensionPublisher expected = TestData.GetFindRedhatXmlPublisher(out JsonObject jsonObject);
        bool actual = RawGalleryExtensionPublisher.TryCreate(jsonObject, out RawGalleryExtensionPublisher? publisher);
        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.True);
            Assert.That(publisher, Is.Not.Null);
            Assert.That(publisher!.PublisherId, Is.EqualTo(expected.PublisherId));
            Assert.That(publisher!.PublisherName, Is.EqualTo(expected.PublisherName));
            Assert.That(publisher!.DisplayName, Is.EqualTo(expected.DisplayName));
            Assert.That(publisher!.IsDomainVerified, Is.EqualTo(expected.IsDomainVerified));
        });
    }
}