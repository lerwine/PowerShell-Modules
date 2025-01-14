using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all direct descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteInputObjAndDirectDesc()
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
    private void WriteInputObjAttribAndDirectDesc()
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
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// </summary>
    private void WriteTypePredicatedInputObjAndDirectDesc()
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
    private void WriteAttribAndTypePredicatedInputObjAndDirectDesc()
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
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" />; otherwise the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> is returned, if present.
    /// </summary>
    private void WriteAttributesInputObjOrDirectDesc()
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
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all recursive descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteRecurseInclInputObj()
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
    private void WriteRecurseInclAttribAndInputObj()
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
            WriteRecursePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants that match the specified type.
    /// </summary>
    private void WriteRecurseTypePredicatedInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0
        if (InputObject is HtmlAttributes)
            return;
        if (_predicate(InputObject))
            WriteObject(InputObject);
        WriteRecurseTypePredicatedPlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteRecurseTypePredicatedInclInputObjAndAttrib()
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
        WriteRecurseTypePredicatedPlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" />; otherwise all recursive descendant <see cref="HtmlAttributes" /> will be returned.
    /// </summary>
    private void WriteRecurseAttribInclInputObj()
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
    /// Returns all recursive descendants starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteFromDepth()
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
    private void WriteFromDepthInclAttrib()
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
            WriteRecursePlusAttrib(item);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void WriteTypePredicatedFromDepth()
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
    private void WriteAttribAndTypePredicatedFromDepth()
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
            WriteRecurseTypePredicatedPlusAttrib(item);
        }
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/>.
    /// </summary>
    private void WriteAttributesFromDepth()
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
    /// Returns all recursive descendants up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteToDepth()
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

    private void WriteToDepthInclAttrib(MarkdownObject parent, int maxDepth)
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
                WriteToDepthInclAttrib(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants())
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>, along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void WriteToDepthInclAttrib()
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
        WriteToDepthInclAttrib(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" />, along with all recursive descendants up to the specified <see cref="MaxDepth"/>,
    /// <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteToDepthInclInputObj()
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

    /// <summary>
    /// Returns the <see cref="InputObject"/> along with all recursive descendants up to the specified <see cref="MaxDepth"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteToDepthInclAttribAndInputObj()
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
            WriteToDepthInclAttrib(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void WriteTypePredicatedToDepth()
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
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants, up to the specified <see cref="MaxDepth"/>,
    /// that match the specified type.
    /// </summary>
    private void WriteTypePredicatedToDepthInclInputObj()
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

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>, along with all recursive descendants,
    /// up to the specified <see cref="MaxDepth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// </summary>
    private void WriteAttribAndTypePredicatedToDepthInclInputObj()
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
            WriteAttribAndTypePredicatedToDepth(InputObject, MaxDepth);
        }
    }

    private void WriteAttribAndTypePredicatedToDepth(MarkdownObject parent, int maxDepth)
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
                WriteAttribAndTypePredicatedToDepth(item, maxDepth);
            }
        }
        else
            foreach (var item in parent.GetDirectDescendants().Where(_predicate))
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    private void WriteAttribAndTypePredicatedToDepth()
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
        WriteAttribAndTypePredicatedToDepth(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/>.
    /// </summary>
    private void WriteAttributesToDepth()
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
    private void WriteAttributesToDepthInclInputObj()
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
    /// Returns all recursive descendants from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteInRange()
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
    private void WriteInRangeInclAttrib()
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
            WriteToDepthInclAttrib(parent, MaxDepth - MinDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>,
    /// that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void WriteTypePredicatedInRange()
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
    private void WriteAttribAndTypePredicatedInRange()
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
            WriteAttribAndTypePredicatedToDepth(parent, depth);
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> within the specified range from <see cref="MinDepth"/> to <see cref="MaxDepth"/>, relative to <see cref="InputObject"/>.
    /// </summary>
    private void WriteAttributesInRange()
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

    private void BeginProcessing_DepthRange(List<Type>? types, int minDepth, int maxDepth, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        Debug.Assert(maxDepth >= minDepth);
        if (maxDepth > minDepth)
        {
            if (types is null)
                _processInputObject = minDepth switch
                {
                    0 => (maxDepth > 1) ? (includeAttributes ? WriteToDepthInclAttribAndInputObj : WriteToDepthInclInputObj) :
                                                includeAttributes ? WriteInputObjAttribAndDirectDesc : WriteInputObjAndDirectDesc,
                    1 => includeAttributes ? WriteToDepthInclAttrib : WriteToDepth,
                    _ => includeAttributes ? WriteInRangeInclAttrib : WriteInRange,
                };
            else
            {
                _types = types;
                if (types.Count > 1)
                {
                    _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                    _processInputObject = minDepth switch
                    {
                        0 => (maxDepth > 1) ? (includeAttributes ? WriteAttribAndTypePredicatedToDepthInclInputObj : WriteTypePredicatedToDepthInclInputObj) :
                                                    includeAttributes ? WriteAttribAndTypePredicatedInputObjAndDirectDesc : WriteTypePredicatedInputObjAndDirectDesc,
                        1 => includeAttributes ? WriteAttribAndTypePredicatedToDepth : WriteTypePredicatedToDepth,
                        _ => includeAttributes ? WriteAttribAndTypePredicatedInRange : WriteTypePredicatedInRange,
                    };
                }
                else
                {
                    _predicate = types[0].IsInstanceOfType;
                    _processInputObject = minDepth switch
                    {
                        0 => (maxDepth > 1) ? (includeAttributes ? WriteAttribAndTypePredicatedToDepthInclInputObj : WriteTypePredicatedToDepthInclInputObj) :
                                                    includeAttributes ? WriteAttribAndTypePredicatedInputObjAndDirectDesc : WriteTypePredicatedInputObjAndDirectDesc,
                        1 => includeAttributes ? WriteAttribAndTypePredicatedToDepth : WriteTypePredicatedToDepth,
                        _ => includeAttributes ? WriteAttribAndTypePredicatedInRange : WriteTypePredicatedInRange,
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
                0 => includeAttributes ? WriteRecurseInclAttribAndInputObj : WriteRecurseInclInputObj,
                1 => includeAttributes ? WriteRecursePlusAttrib : WriteRecurse,
                _ => includeAttributes ? WriteFromDepthInclAttrib : WriteFromDepth,
            };
        else
        {
            _types = types;
            if (types.Count > 1)
            {
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? WriteRecurseTypePredicatedInclInputObjAndAttrib : WriteRecurseTypePredicatedInclInputObj,
                    1 => includeAttributes ? WriteRecurseTypePredicatedPlusAttrib : WriteRecurseTypePredicated,
                    _ => includeAttributes ? WriteAttribAndTypePredicatedFromDepth : WriteTypePredicatedFromDepth,
                };
            }
            else
            {
                _predicate = types[0].IsInstanceOfType;
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? WriteRecurseTypePredicatedInclInputObjAndAttrib : WriteRecurseTypePredicatedInclInputObj,
                    1 => includeAttributes ? WriteRecurseTypePredicatedPlusAttrib : WriteRecurseTypePredicated,
                    _ => includeAttributes ? WriteAttribAndTypePredicatedFromDepth : WriteTypePredicatedFromDepth,
                };
            }
        }
    }

    private void BeginProcessing_NoRecursion(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? WriteAttribAndDirectDesc : WriteDirectDesc;
        else
        {
            _types = types;
            if (types.Count > 1)
            {
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedDirectDesc : WriteTypePredicatedDirectDesc;
            }
            else
            {
                _predicate = types[0].IsInstanceOfType;
                _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedDirectDesc : WriteTypePredicatedDirectDesc;
            }
        }
    }
}