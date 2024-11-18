using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;
using SwPackage.VsCodeVsix;

namespace SwPackage.UnitTests
{
    public class RawGalleryExtensionVersionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TryAddVersionsTest()
        {
            RawGalleryExtension.VersionCollection versions = [];

            Collection<RawGalleryExtensionVersion> expectedValues = TestData.GetFindRedhatXmlVersions(out JsonArray versionsJson);
            var actual = RawGalleryExtensionVersion.TryAddVersions(versionsJson, versions);
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.True);
                Assert.That(versions, Has.Count.EqualTo(expectedValues.Count));
            });
            for (var i = 0; i < expectedValues.Count; i++)
            {
                var item = versions[i];
                var expected = expectedValues[i];
                Assert.Multiple(() =>
                {
                    Assert.That(item, Is.Not.Null, "Item {0}", i);
                    Assert.That(item.Version, Is.EqualTo(expected.Version), "Item {0}", i);
                    Assert.That(item.Platform, Is.EqualTo(expected.Platform), "Item {0}", i);
                    Assert.That(item.LastUpdated, Is.EqualTo(expected.LastUpdated), "Item {0}", i);
                    Assert.That(item.AssetUri, Is.EqualTo(expected.AssetUri), "Item {0}", i);
                    Assert.That(item.FallbackAssetUri, Is.EqualTo(expected.FallbackAssetUri), "Item {0}", i);
                });
            }
        }
    }
}