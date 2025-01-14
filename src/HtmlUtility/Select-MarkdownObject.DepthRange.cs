using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" />; otherwise the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> is returned, if present.
    /// </summary>
    private void AttributesInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            var attributes = InputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>.
    /// </summary>
    private void AttributesToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var parent in InputObject.GetDescendantsToDepth(MaxDepth - 1))
        {
            if ((attributes = parent.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" />; otherwise all recursive descendant <see cref="HtmlAttributes" /> up to the specified <see cref="MaxDepth"/>
    /// will be returned.
    /// </summary>
    private void AttributesToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            var attributes = InputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var parent in InputObject.GetDescendantsToDepth(MaxDepth - 1))
            {
                if ((attributes = parent.TryGetAttributes()) is not null)
                    WriteObject(attributes, false);
            }
        }
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>.
    /// </summary>
    private void AttributesInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        foreach (var parent in InputObject.GetDescendantsInDepthRange(MinDepth - 1, MaxDepth - 1))
        {
            var attributes = parent.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" />; otherwise all recursive descendant <see cref="HtmlAttributes" /> will be returned.
    /// </summary>
    private void AttributesInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            var attributes = InputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var parent in InputObject.Descendants())
            {
                if ((attributes = parent.TryGetAttributes()) is not null)
                    WriteObject(attributes, false);
            }
        }
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>.
    /// </summary>
    private void AttributesFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        foreach (var parent in InputObject.GetDescendantsFromDepth(MinDepth - 1))
        {
            var attributes = parent.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in parent.Descendants())
            {
                if ((attributes = item.TryGetAttributes()) is not null)
                    WriteObject(attributes, false);
            }
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all recursive descendants up to the specified <see cref="MaxDepth"/>,
    /// <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MaxDepth > 1);
        if (InputObject is HtmlAttributes)
            return;

        WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth))
            WriteObject(item, false);
    }

    private void AnyTypePlusAttribToDepth(MarkdownObject parent, int maxDepth)
    {
        var attributes = parent.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        if (maxDepth > 1)
        {
            maxDepth--;
            foreach (var item in parent.GetDirectDescendants())
            {
                WriteObject(item, false);
                AnyTypePlusAttribToDepth(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants())
                WriteObject(item, false);
    }
    
    /// <summary>
    /// Returns the <see cref="InputObject"/> along with all recursive descendants up to the specified <see cref="MaxDepth"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        WriteObject(InputObject, false);
        if (InputObject is not HtmlAttributes)
            AnyTypePlusAttribToDepth(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all direct descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 1
        if (InputObject is HtmlAttributes)
            return;
        WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDirectDescendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/>, along with all direct descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 2
        WriteObject(InputObject, false);
        if (InputObject is HtmlAttributes)
            return;
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            WriteObject(item, false);
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 2
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void AnyTypePlusAttribToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        AnyTypePlusAttribToDepth(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 3
        foreach (var item in InputObject.GetDescendantsInDepthRange(MinDepth, MaxDepth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        foreach (var parent in InputObject.GetDescendantsFromDepth(MinDepth - 1))
            AnyTypePlusAttribToDepth(parent, MaxDepth - MinDepth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants, up to the specified <see cref="MaxDepth"/>,
    /// that match any of the specified types.
    /// </summary>
    private void MultiTypeToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2
        if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
            WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth).Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    private void MultiTypePlusAttribToDepth(MarkdownObject parent, int maxDepth)
    {
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        var attributes = parent.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        if (maxDepth > 1)
        {
            maxDepth--;
            foreach (var item in parent.GetDirectDescendants())
            {
                if (_multiTypes.Any(t => t.IsInstanceOfType(item)))
                    WriteObject(item, false);
                MultiTypePlusAttribToDepth(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants,
    /// up to the specified <see cref="MaxDepth"/>, that match any of the specified types, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// </summary>
    private void MultiTypePlusAttribToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
                WriteObject(InputObject, false);
            MultiTypePlusAttribToDepth(InputObject, MaxDepth);
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all direct descendants that match any of the specified types,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void MultiTypeInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1
        if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
            WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDirectDescendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>,
    /// along with the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> and all direct descendants that match any of the specified types,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void MultiTypePlusAttribInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
                WriteObject(InputObject, false);
            var attributes = InputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in InputObject.GetDirectDescendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
                WriteObject(item, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    private void MultiTypeToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 2
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth).Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match any of the specified <see cref="_multiTypes"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void MultiTypePlusAttribToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        MultiTypePlusAttribToDepth(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/> that match any of the
    /// specified <see cref="_multiTypes"/>.
    /// </summary>
    private void MultiTypeInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3
        foreach (var item in InputObject.GetDescendantsInDepthRange(MinDepth, MaxDepth).Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/> that match any of the
    /// specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> within that range.
    /// </summary>
    private void MultiTypePlusAttribInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        int depth = MaxDepth - MinDepth;
        foreach (var parent in InputObject.GetDescendantsAtDepth(MinDepth - 1))
            MultiTypePlusAttribToDepth(parent, depth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants, up to the specified <see cref="MaxDepth"/>,
    /// that match the specified type.
    /// </summary>
    private void SingleTypeToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2
        if (_singleType.IsInstanceOfType(InputObject))
            WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth).Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    private void SingleTypePlusAttribToDepth(MarkdownObject parent, int maxDepth)
    {
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        var attributes = parent.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        if (maxDepth > 1)
        {
            maxDepth--;
            foreach (var item in parent.GetDirectDescendants())
            {
                if (_singleType.IsInstanceOfType(item))
                    WriteObject(item, false);
                SingleTypePlusAttribToDepth(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants().Where(_singleType.IsInstanceOfType))
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with all recursive descendants,
    /// up to the specified <see cref="MaxDepth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// </summary>
    private void SingleTypePlusAttribToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            if (_singleType.IsInstanceOfType(InputObject))
                WriteObject(InputObject, false);
            SingleTypePlusAttribToDepth(InputObject, MaxDepth);
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_singleType"/>, along with all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void SingleTypeInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1
        if (_singleType.IsInstanceOfType(InputObject))
            WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDirectDescendants().Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>,
    /// along with the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> and all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void SingleTypePlusAttribInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            if (_singleType.IsInstanceOfType(InputObject))
                WriteObject(InputObject, false);
            foreach (var item in InputObject.GetDirectDescendants().Where(_singleType.IsInstanceOfType))
                WriteObject(item, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    private void SingleTypeToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth).Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void SingleTypePlusAttribToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        SingleTypePlusAttribToDepth(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>,
    /// that match the specified <see cref="_singleType"/>.
    /// </summary>
    private void SingleTypeInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3
        foreach (var item in InputObject.GetDescendantsInDepthRange(MinDepth, MaxDepth).Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>,
    /// that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> within that range.
    /// </summary>
    private void SingleTypePlusAttribInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        int depth = MaxDepth - MinDepth;
        foreach (var parent in InputObject.GetDescendantsAtDepth(MinDepth - 1))
            SingleTypePlusAttribToDepth(parent, depth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all recursive descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0
        if (InputObject is HtmlAttributes)
            return;
        WriteObject(InputObject, false);
        foreach (var item in InputObject.Descendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> along with all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -IncludeAttributes
        WriteObject(InputObject, false);
        if (InputObject is not HtmlAttributes)
            AnyTypePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns all recursive descendants starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -IncludeAttributes
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth - 1))
            AnyTypePlusAttrib(item);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants that match any of the specified types.
    /// </summary>
    private void MultiTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0
        if (InputObject is HtmlAttributes)
            return;
        if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
            WriteObject(InputObject);
        MultiTypePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>,
    /// along with all recursive descendants that match any of the specified types, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void MultiTypePlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -IncludeAttributes
        if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
            WriteObject(InputObject);
        else if (InputObject is HtmlAttributes)
        {
            WriteObject(InputObject);
            return;
        }
        MultiTypePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    private void MultiTypeFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth).Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match any of the specified <see cref="_multiTypes"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    private void MultiTypePlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -IncludeAttributes
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth - 1))
        {
            if (_multiTypes.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            MultiTypePlusAttrib(item);
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants that match the specified type.
    /// </summary>
    private void SingleTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0
        if (InputObject is HtmlAttributes)
            return;
        if (_singleType.IsInstanceOfType(InputObject))
            WriteObject(InputObject);
        SingleTypePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>,
    /// along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void SingleTypePlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -IncludeAttributes
        if (_singleType.IsInstanceOfType(InputObject))
            WriteObject(InputObject);
        else if (InputObject is HtmlAttributes)
        {
            WriteObject(InputObject);
            return;
        }
        SingleTypePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    private void SingleTypeFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth).Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    private void SingleTypePlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -IncludeAttributes
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth - 1))
        {
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            SingleTypePlusAttrib(item);
        }
    }

    private void BeginProcessing_DepthRange(List<Type>? types, int minDepth, int maxDepth, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        Debug.Assert(maxDepth >= minDepth);
        if (maxDepth > minDepth)
        {
            if (types is null)
                _processInputObject = minDepth switch
                {
                    0 => (maxDepth > 1) ? (includeAttributes ? AnyTypePlusAttribToDepthInclInputObj : AnyTypeToDepthInclInputObj) :
                                                includeAttributes ? AnyTypePlusAttribInputObjAndDirectDesc : AnyTypeInputObjAndDirectDesc,
                    1 => includeAttributes ? AnyTypePlusAttribToDepth : AnyTypeToDepth,
                    _ => includeAttributes ? AnyTypePlusAttribInRange : AnyTypeInRange,
                };
            else if (types.Count > 0)
            {
                _multiTypes = types;
                _processInputObject = minDepth switch
                {
                    0 => (maxDepth > 1) ? (includeAttributes ? MultiTypePlusAttribToDepthInclInputObj : MultiTypeToDepthInclInputObj) :
                                                includeAttributes ? MultiTypePlusAttribInputObjAndDirectDesc : MultiTypeInputObjAndDirectDesc,
                    1 => includeAttributes ? MultiTypePlusAttribToDepth : MultiTypeToDepth,
                    _ => includeAttributes ? MultiTypePlusAttribInRange : MultiTypeInRange,
                };
            }
            else
            {
                _singleType = types[0];
                _processInputObject = minDepth switch
                {
                    0 => (maxDepth > 1) ? (includeAttributes ? SingleTypePlusAttribToDepthInclInputObj : SingleTypeToDepthInclInputObj) :
                                                includeAttributes ? SingleTypePlusAttribInputObjAndDirectDesc : SingleTypeInputObjAndDirectDesc,
                    1 => includeAttributes ? SingleTypePlusAttribToDepth : SingleTypeToDepth,
                    _ => includeAttributes ? SingleTypePlusAttribInRange : SingleTypeInRange,
                };
            }
        }
        else
            BeginProcessing_ExplicitDepth(types, minDepth, includeAttributes);
    }

    private void BeginProcessing_DepthRange(List<Type>? types, int minDepth, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = minDepth switch
            {
                0 => includeAttributes ? AnyTypePlusAttribInputObjAndAllDesc : AnyTypeInputObjAndAllDesc,
                1 => includeAttributes ? AnyTypePlusAttrib : AnyType,
                _ => includeAttributes ? AnyTypePlusAttribFromDepth : AnyTypeFromDepth,
            };
        else if (types.Count > 0)
        {
            _multiTypes = types;
            _processInputObject = minDepth switch
            {
                0 => includeAttributes ? MultiTypePlusAttribInputObjAndAllDesc : MultiTypeInputObjAndAllDesc,
                1 => includeAttributes ? MultiTypePlusAttrib : MultiType,
                _ => includeAttributes ? MultiTypePlusAttribFromDepth : MultiTypeFromDepth,
            };
        }
        else
        {
            _singleType = types[0];
            _processInputObject = minDepth switch
            {
                0 => includeAttributes ? SingleTypePlusAttribInputObjAndAllDesc : SingleTypeInputObjAndAllDesc,
                1 => includeAttributes ? SingleTypePlusAttrib : SingleType,
                _ => includeAttributes ? SingleTypePlusAttribFromDepth : SingleTypeFromDepth,
            };
        }
    }

    private void BeginProcessing_NoRecursion(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? AnyTypePlusAttribDirectDesc : AnyTypeDirectDesc;
        else if (types.Count > 1)
        {
            _multiTypes = types;
            _processInputObject = includeAttributes ? MultiTypePlusAttribDirectDesc : MultiTypeDirectDesc;
        }
        else
        {
            _singleType = types[0];
            _processInputObject = includeAttributes ? SingleTypePlusAttribDirectDesc : SingleTypeDirectDesc;
        }
    }
}