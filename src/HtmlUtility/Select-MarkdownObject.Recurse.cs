using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns all recursive descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteRecurse()
    {
        Debug.Assert(InputObject is not null);
        // Recurse: Select-MarkdownObject -Recurse
        // DepthRange: Select-MarkdownObject (no parameters)
        // DepthRange: Select-MarkdownObject -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1
        // Recurse: Select-MarkdownObject -Type Any -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Any -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Inline, Any -Recurse
        foreach (var item in InputObject.Descendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteRecursePlusAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is null);
        Debug.Assert(_predicate is null);
        // Recurse: Select-MarkdownObject -Type Any -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, Any -Recurse -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, Inline, Any -Recurse -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type HtmlAttributes, Any -Recurse
        // Recurse: Select-MarkdownObject -Type HtmlAttributes, Any -Recurse -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Recurse
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes, Any -Recurse -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -Recurse -IncludeAttributes
        foreach (var item in InputObject.GetAttributesAndDirectDescendants())
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void WriteRecurseTypePredicated()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // Recurse: Select-MarkdownObject -Type Block -Recurse
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1
        foreach (var item in InputObject.GetDescendants(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>, along with all <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteRecurseTypePredicatedPlusAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // Recurse: Select-MarkdownObject -Type Block -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes -Recurse -IncludeAttributes
        foreach (var item in InputObject.GetAttributesAndDirectDescendants(_predicate))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteRecurseAttributes()
    {
        Debug.Assert(InputObject is not null);
        // Recurse: Select-MarkdownObject -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type HtmlAttributes -Recurse -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.GetDescendantAttributes())
            WriteObject(item, false);
    }

    private void BeginProcessing_Recurse(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? WriteRecursePlusAttrib : WriteRecurse;
        else
        {
            _types = types;
            if (types.Count > 0)
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
            else
                _predicate = types[0].IsInstanceOfType;
            _processInputObject = includeAttributes ? WriteRecurseTypePredicatedPlusAttrib : WriteRecurseTypePredicated;
        }
    }
}