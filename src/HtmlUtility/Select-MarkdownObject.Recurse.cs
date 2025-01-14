using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" />.
    /// </summary>
    private void WriteAllAttributes()
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
        foreach (var item in InputObject.Descendants())
        {
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants, <i>NOT</i> including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyType()
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

    private void AnyTypePlusAttrib(MarkdownObject parent)
    {
        var attributes = parent.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.GetDirectDescendants())
        {
            WriteObject(item, false);
            AnyTypePlusAttrib(item);
        }
    }

    /// <summary>
    /// Returns all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttrib()
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
        AnyTypePlusAttrib(InputObject);
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>.
    /// </summary>
    private void TypePredicated()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // Recurse: Select-MarkdownObject -Type Block -Recurse
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1
        foreach (var item in InputObject.Descendants().Where(_predicate))
            WriteObject(item, false);
    }

    private void TypePredicatedPlusAttrib(MarkdownObject parent)
    {
        var attributes = parent.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.GetDirectDescendants())
        {
            if (_predicate(item))
                WriteObject(item, false);
            TypePredicatedPlusAttrib(item);
        }
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>, along with all <see cref="HtmlAttributes" />.
    /// </summary>
    private void TypePredicatedPlusAttrib()
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
        TypePredicatedPlusAttrib(InputObject);
    }

    private void BeginProcessing_Recurse(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? AnyTypePlusAttrib : AnyType;
        else
        {
            _types = types;
            if (types.Count > 0)
                _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
            else
                _predicate = types[0].IsInstanceOfType;
            _processInputObject = includeAttributes ? TypePredicatedPlusAttrib : TypePredicated;
        }
    }
}