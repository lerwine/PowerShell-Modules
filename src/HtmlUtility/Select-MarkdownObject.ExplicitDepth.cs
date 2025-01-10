using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    private int _depth;

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        if (inputObject is HtmlAttributes)
            WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/>, if present.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        var attributes = inputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
    }

    /// <summary>
    /// Returns all descendant <see cref="HtmlAttributes" /> at the specified <see cref="_depth"/> from <paramref name="inputObject"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Depth 2
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type HtmlAttributes -Depth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in inputObject.GetDescendantsAtDepth(_depth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is *NOT* <see cref="HtmlAttributes" /> (no recursion).
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        if (inputObject is not HtmlAttributes)
            WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> irregardless of its type (no recursion).
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the direct descendants, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        foreach (var item in inputObject.GetDirectDescendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the direct descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        foreach (var item in inputObject.GetDirectDescendantsAndAttributes())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <paramref name="inputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        foreach (var item in inputObject.GetDescendantsAtDepth(_depth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <paramref name="inputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
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
        foreach (var item in inputObject.GetDescendantsAtDepthIncludingAttributes(_depth))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/> (no recursion).
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 0
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        if (_multiTypes.Any(t => t.IsInstanceOfType(inputObject)))
            WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/> (no recursion).
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 0 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Depth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Depth 0 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        if (inputObject is HtmlAttributes || _multiTypes.Any(t => t.IsInstanceOfType(inputObject)))
            WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the direct descendants that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 1
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        foreach (var item in inputObject.GetDirectDescendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/>, if present, along with the direct descendants that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 1 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Depth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Depth 1 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        var attributes = inputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in inputObject.GetDirectDescendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 2
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in inputObject.GetDescendantsAtDepth(_depth).Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> at that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Depth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var parent in inputObject.GetDescendantsAtDepth(_depth - 1))
        {
            var attributes = parent.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in parent.GetDirectDescendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
                WriteObject(item, false);
        }
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/> (no recursion).
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 0
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 0
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 0
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 0
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        if (_singleType.IsInstanceOfType(inputObject))
            WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/> (no recursion).
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInputObj(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_singleType is not null);
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
        if (inputObject is HtmlAttributes || _singleType.IsInstanceOfType(inputObject))
            WriteObject(inputObject, false);
    }

    /// <summary>
    /// Returns the direct descendants that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 1
        // DepthRange: Select-MarkdownObject -Type Block
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 1
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 1
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 1 -RecurseUnmatchedOnly
        foreach (var item in inputObject.GetDirectDescendants().Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/>, if present, along with the direct descendants that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribDirectDesc(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_singleType is not null);
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
        var attributes = inputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in inputObject.GetDirectDescendants().Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var item in inputObject.GetDescendantsAtDepth(_depth).Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all descendants at the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> at that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(inputObject is not null);
        Debug.Assert(_singleType is not null);
        Debug.Assert(_depth > 1);
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block -Depth 2 -IncludeAttributes
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 2
        // ExplicitDepth: Select-MarkdownObject -Type Block, HtmlAttributes -Depth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 2 -RecurseUnmatchedOnly
        foreach (var parent in inputObject.GetDescendantsAtDepth(_depth - 1))
        {
            var attributes = parent.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in parent.GetDirectDescendants().Where(_singleType.IsInstanceOfType))
                WriteObject(item, false);
        }
    }

    private void BeginProcessing_ExplicitDepth(List<Type>? types, int depth, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            switch (Depth)
            {
                case 0:
                    _processInputObject = includeAttributes ? AnyTypePlusAttribInputObj : AnyTypeInputObj;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? AnyTypePlusAttribDirectDesc : AnyTypeDirectDesc;
                    break;
                default:
                    _depth = Depth;
                    _processInputObject = includeAttributes ? AnyTypePlusAttribAtDepth : AnyTypeAtDepth;
                    break;
            }
        else if (types.Count > 1)
        {
            _multiTypes = types;
            switch (depth)
            {
                case 0:
                    _processInputObject = includeAttributes ? MultiTypePlusAttribInputObj : MultiTypeInputObj;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? MultiTypePlusAttribDirectDesc : MultiTypeDirectDesc;
                    break;
                default:
                    _depth = depth;
                    _processInputObject = includeAttributes ? MultiTypePlusAttribAtDepth : MultiTypeAtDepth;
                    break;
            }
        }
        else
        {
            _singleType = types[0];
            switch (depth)
            {
                case 0:
                    _processInputObject = includeAttributes ? SingleTypePlusAttribInputObj : SingleTypeInputObj;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? SingleTypePlusAttribDirectDesc : SingleTypeDirectDesc;
                    break;
                default:
                    _depth = depth;
                    _processInputObject = includeAttributes ? SingleTypePlusAttribAtDepth : SingleTypeAtDepth;
                    break;
            }
        }
    }
}