using System;

namespace Erwine.Leonard.T.GDIPlus.Collections
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
