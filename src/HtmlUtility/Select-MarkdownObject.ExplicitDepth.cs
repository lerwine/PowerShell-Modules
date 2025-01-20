using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    private int _depth;

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <i>NOT</i> <see cref="HtmlAttributes" /> (no recursion).
    /// </summary>
    private void WriteInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // ExplicitDepth: Select-MarkdownObject -Depth 0
        // DepthRange: Select-MarkdownObject -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 0
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 0
        // ExplicitDepth: Select-MarkdownObject -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, Any -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, Any -Depth 0
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        if (InputObject is not HtmlAttributes)
            WriteObject(InputObject, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> irregardless of its type (no recursion).
    /// </summary>
    private void WriteInputObjPlusAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 0
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Any -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, Any -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes, Any -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes, Any -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Depth 0 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        WriteObject(InputObject, false);
    }

    /// <summary>
    /// Returns the direct descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // ExplicitDepth: Select-MarkdownObject -Depth 1
        // DepthRange: Select-MarkdownObject -Type Any
        // DepthRange: Select-MarkdownObject -Type Block, Any
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 1
        // DepthRange: Select-MarkdownObject -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 1
        // ExplicitDepth: Select-MarkdownObject -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, Any -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, Any -Depth 1
        // RecurseUnmatched: Select-MarkdownObject -Type Any -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetDirectDescendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the direct descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteAttribAndDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 1
        // DepthRange: Select-MarkdownObject -Type Any -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Any -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, Any -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes, Any -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes, Any -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Depth 1 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetAttributesAndDirectDescendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <see cref="InputObject"/>, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteAtDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Depth 2
        // DepthRange: Select-MarkdownObject -MaxDepth 1
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 2
        // ExplicitDepth: Select-MarkdownObject -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, Any -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, Any -Depth 2
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <see cref="InputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteAtDepthInclAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Any -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Any -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, Any -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes, Any -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes, Any -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Depth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetAttributesAndDescendantsAtDepth(_depth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/> (no recursion).
    /// </summary>
    private void WriteTypePredicatedInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 0
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 0
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        if (_predicate(InputObject))
            WriteObject(InputObject, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/> (no recursion).
    /// </summary>
    private void WriteTypePredicatedOrAttribInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 0
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 0 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        if (InputObject is HtmlAttributes || _predicate(InputObject))
            WriteObject(InputObject, false);
    }

    /// <summary>
    /// Returns the direct descendants that match the specified <see cref="_si_predicatengleType"/>.
    /// </summary>
    private void WriteTypePredicatedDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 1
        // DepthRange: Select-MarkdownObject -Type Block
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 1
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetDirectDescendants(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/>, if present, along with the direct descendants that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void WriteAttribAndTypePredicatedDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 1
        // DepthRange: Select-MarkdownObject -Type Block -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 1 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetAttributesAndDirectDescendants(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void WriteTypePredicatedAtDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth, _predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> at that depth.
    /// </summary>
    private void WriteAttribAndTypePredicatedAtDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetAttributesAndDescendantsAtDepth(_depth, _predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteAttributesInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // ExplicitDepth: Select-MarkdownObject -Depth 0
        // DepthRange: Select-MarkdownObject -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 0 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
    }

    /// <summary>
    /// Returns the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/>, if present.
    /// </summary>
    private void WriteAttributesDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // ExplicitDepth: Select-MarkdownObject -Depth 1
        // DepthRange: Select-MarkdownObject -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 1 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
    }

    /// <summary>
    /// Returns all descendant <see cref="HtmlAttributes" /> at the specified <see cref="_depth"/> from <see cref="InputObject"/>.
    /// </summary>
    private void WriteAttributesAtDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Depth 2
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in InputObject.GetAttributesAtDepth(_depth))
            WriteObject(item, false);
    }

    private void BeginProcessing_ExplicitDepth(List<Type>? types, int depth, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            switch (Depth)
            {
                case 0:
                    _processInputObject = includeAttributes ? WriteInputObjPlusAttrib : WriteInputObj;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? WriteAttribAndDirectDesc : WriteDirectDesc;
                    break;
                default:
                    _depth = Depth;
                    _processInputObject = includeAttributes ? WriteAtDepthInclAttrib : WriteAtDepth;
                    break;
            }
        else
        {
            _types = types;
            if (types.Count > 1)
            {
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
                switch (depth)
                {
                    case 0:
                        _processInputObject = includeAttributes ? WriteTypePredicatedOrAttribInputObj : WriteTypePredicatedInputObj;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedDirectDesc : WriteTypePredicatedDirectDesc;
                        break;
                    default:
                        _depth = depth;
                        _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedAtDepth : WriteTypePredicatedAtDepth;
                        break;
                }
            }
            else
            {
                _predicate = types[0].IsInstanceOfType;
                switch (depth)
                {
                    case 0:
                        _processInputObject = includeAttributes ? WriteTypePredicatedOrAttribInputObj : WriteTypePredicatedInputObj;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedDirectDesc : WriteTypePredicatedDirectDesc;
                        break;
                    default:
                        _depth = depth;
                        _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedAtDepth : WriteTypePredicatedAtDepth;
                        break;
                }
            }
        }
    }
}