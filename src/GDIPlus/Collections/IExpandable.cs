#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Collections
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IExpandable
    {
        event EventHandler IsExpandedChanged;
        event EventHandler Expanded;
        event EventHandler Collapsed;
        bool IsExpanded { get; set; }
        void ToggleExpand();
        void Expand();
        void Collapse();
    }

    public interface IExpandable<T> : IExpandable
    {
        T Value { get; set; }
    }
}
