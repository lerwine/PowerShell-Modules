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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>, along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void AnyTypePlusAttribToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants, up to the specified <see cref="MaxDepth"/>,
    /// that match the specified type.
    /// </summary>
    private void TypePredicatedToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2
        if (_predicate(InputObject))
            WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth).Where(_predicate))
            WriteObject(item, false);
    }

    private void TypePredicatedPlusAttribToDepth(MarkdownObject parent, int maxDepth)
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
                if (_predicate(item))
                    WriteObject(item, false);
                TypePredicatedPlusAttribToDepth(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants().Where(_predicate))
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>, along with all recursive descendants,
    /// up to the specified <see cref="MaxDepth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// </summary>
    private void TypePredicatedPlusAttribToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            if (_predicate(InputObject))
                WriteObject(InputObject, false);
            TypePredicatedPlusAttribToDepth(InputObject, MaxDepth);
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void TypePredicatedInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1
        if (_predicate(InputObject))
            WriteObject(InputObject, false);
        foreach (var item in InputObject.GetDirectDescendants().Where(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> and all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void TypePredicatedPlusAttribInputObjAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
        {
            if (_predicate(InputObject))
                WriteObject(InputObject, false);
            foreach (var item in InputObject.GetDirectDescendants().Where(_predicate))
                WriteObject(item, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void TypePredicatedToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth).Where(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void TypePredicatedPlusAttribToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        TypePredicatedPlusAttribToDepth(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>,
    /// that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void TypePredicatedInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3
        foreach (var item in InputObject.GetDescendantsInDepthRange(MinDepth, MaxDepth).Where(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>,
    /// that match the specified <see cref="_predicate"/>, along with all <see cref="HtmlAttributes" /> within that range.
    /// </summary>
    private void TypePredicatedPlusAttribInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        int depth = MaxDepth - MinDepth;
        foreach (var parent in InputObject.GetDescendantsAtDepth(MinDepth - 1))
            TypePredicatedPlusAttribToDepth(parent, depth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all recursive descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypeInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
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
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants that match the specified type.
    /// </summary>
    private void TypePredicatedInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0
        if (InputObject is HtmlAttributes)
            return;
        if (_predicate(InputObject))
            WriteObject(InputObject);
        TypePredicatedPlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void TypePredicatedPlusAttribInputObjAndAllDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -IncludeAttributes
        if (_predicate(InputObject))
            WriteObject(InputObject);
        else if (InputObject is HtmlAttributes)
        {
            WriteObject(InputObject);
            return;
        }
        TypePredicatedPlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void TypePredicatedFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth).Where(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    private void TypePredicatedPlusAttribFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -IncludeAttributes
        foreach (var item in InputObject.GetDescendantsFromDepth(MinDepth - 1))
        {
            if (_predicate(item))
                WriteObject(item, false);
            TypePredicatedPlusAttrib(item);
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
            else
            {
                _types = types;
                if (types.Count > 1)
                {
                    _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                    _processInputObject = minDepth switch
                    {
                        0 => (maxDepth > 1) ? (includeAttributes ? TypePredicatedPlusAttribToDepthInclInputObj : TypePredicatedToDepthInclInputObj) :
                                                    includeAttributes ? TypePredicatedPlusAttribInputObjAndDirectDesc : TypePredicatedInputObjAndDirectDesc,
                        1 => includeAttributes ? TypePredicatedPlusAttribToDepth : TypePredicatedToDepth,
                        _ => includeAttributes ? TypePredicatedPlusAttribInRange : TypePredicatedInRange,
                    };
                }
                else
                {
                    _predicate = types[0].IsInstanceOfType;
                    _processInputObject = minDepth switch
                    {
                        0 => (maxDepth > 1) ? (includeAttributes ? TypePredicatedPlusAttribToDepthInclInputObj : TypePredicatedToDepthInclInputObj) :
                                                    includeAttributes ? TypePredicatedPlusAttribInputObjAndDirectDesc : TypePredicatedInputObjAndDirectDesc,
                        1 => includeAttributes ? TypePredicatedPlusAttribToDepth : TypePredicatedToDepth,
                        _ => includeAttributes ? TypePredicatedPlusAttribInRange : TypePredicatedInRange,
                    };
                }
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
        else
        {
            _types = types;
            if (types.Count > 1)
            {
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? TypePredicatedPlusAttribInputObjAndAllDesc : TypePredicatedInputObjAndAllDesc,
                    1 => includeAttributes ? TypePredicatedPlusAttrib : TypePredicated,
                    _ => includeAttributes ? TypePredicatedPlusAttribFromDepth : TypePredicatedFromDepth,
                };
            }
            else
            {
                _predicate = types[0].IsInstanceOfType;
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? TypePredicatedPlusAttribInputObjAndAllDesc : TypePredicatedInputObjAndAllDesc,
                    1 => includeAttributes ? TypePredicatedPlusAttrib : TypePredicated,
                    _ => includeAttributes ? TypePredicatedPlusAttribFromDepth : TypePredicatedFromDepth,
                };
            }
        }
    }

    private void BeginProcessing_NoRecursion(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? AnyTypePlusAttribDirectDesc : AnyTypeDirectDesc;
        else
        {
            _types = types;
            if (types.Count > 1)
            {
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                _processInputObject = includeAttributes ? TypePredicatedPlusAttribDirectDesc : TypePredicatedDirectDesc;
            }
            else
            {
                _predicate = types[0].IsInstanceOfType;
                _processInputObject = includeAttributes ? TypePredicatedPlusAttribDirectDesc : TypePredicatedDirectDesc;
            }
        }
    }
}