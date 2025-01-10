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
    /// Returns the <see cref="InputObject"/> if it is *NOT* <see cref="HtmlAttributes" />, along with all recursive descendants up to the specified <see cref="MaxDepth"/>,
    /// *NOT* including <see cref="HtmlAttributes" />.
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
    /// Returns the <see cref="InputObject"/> if it is *NOT* <see cref="HtmlAttributes" />, along with all direct descendants, *NOT* including <see cref="HtmlAttributes" />.
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
    /// Returns all recursive descendants up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
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
    /// Returns all recursive descendants from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
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
                WriteObject(item, false);
                MultiTypePlusAttribToDepth(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants())
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
        // TODO: Implement MultiTypeInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1
        throw new NotImplementedException();
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
        // TODO: Implement MultiTypePlusAttribInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        throw new NotImplementedException();
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
        // TODO: Implement MultiTypeToDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 2
        throw new NotImplementedException();
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
        // TODO: Implement MultiTypePlusAttribToDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        throw new NotImplementedException();
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
        // TODO: Implement MultiTypeInRange
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3
        throw new NotImplementedException();
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
        // TODO: Implement MultiTypePlusAttribInRange
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        throw new NotImplementedException();
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
        // TODO: Implement SingleTypeToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2
        throw new NotImplementedException();
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
        // TODO: Implement SingleTypePlusAttribToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_singleType"/>, along with all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void SingleTypeInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        // TODO: Implement SingleTypeInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1
        throw new NotImplementedException();
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
        // TODO: Implement SingleTypePlusAttribInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    private void SingleTypeToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(MaxDepth > 1);
        // TODO: Implement SingleTypeToDepth
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2
        throw new NotImplementedException();
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
        // TODO: Implement SingleTypePlusAttribToDepth
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        throw new NotImplementedException();
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
        // TODO: Implement SingleTypeInRange
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3
        throw new NotImplementedException();
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
        // TODO: Implement SingleTypePlusAttribInRange
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is *NOT* <see cref="HtmlAttributes" />, along with all recursive descendants, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // TODO: Implement AnyTypeInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> along with all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // TODO: Implement AnyTypePlusAttribInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement AnyTypeFromDepth
        // DepthRange: Select-MarkdownObject -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement AnyTypePlusAttribFromDepth
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants that match any of the specified types.
    /// </summary>
    private void MultiTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // TODO: Implement MultiTypeInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>,
    /// along with all recursive descendants that match any of the specified types, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void MultiTypePlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // TODO: Implement MultiTypePlusAttribInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    private void MultiTypeFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement MultiTypeFromDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match any of the specified <see cref="_multiTypes"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    private void MultiTypePlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement MultiTypePlusAttribFromDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants that match the specified type.
    /// </summary>
    private void SingleTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // TODO: Implement SingleTypeInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>,
    /// along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void SingleTypePlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        // TODO: Implement SingleTypePlusAttribInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    private void SingleTypeFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement SingleTypeFromDepth
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_singleType"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    private void SingleTypePlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement SingleTypePlusAttribFromDepth
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -IncludeAttributes
        throw new NotImplementedException();
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