using System.Collections.ObjectModel;

namespace SwPackage.VsCodeVsix;

public partial class RawGalleryExtension
{
    public class VersionCollection : Collection<RawGalleryExtensionVersion>
    {
        public VersionCollection() { }

        public VersionCollection(IList<RawGalleryExtensionVersion> list) : base(list ?? throw new ArgumentNullException(nameof(list)))
        {
            foreach (RawGalleryExtensionVersion item in Items)
                if (item is null) throw new ArgumentNullException(nameof(list));
        }

        protected override void InsertItem(int index, RawGalleryExtensionVersion item)
        {
            ArgumentNullException.ThrowIfNull(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, RawGalleryExtensionVersion item)
        {
            ArgumentNullException.ThrowIfNull(item);
            base.SetItem(index, item);
        }
    }
}