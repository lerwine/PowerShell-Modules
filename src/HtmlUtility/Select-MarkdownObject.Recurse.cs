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
    /// Returns all recursive descendants, *NOT* including <see cref="HtmlAttributes" />.
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

    /// <summary>
    /// Returns all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    private void AnyTypePlusAttrib()
    {
        Debug.Assert(InputObject is not null);
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
    /// Returns all recursive descendants that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    private void MultiType()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // Recurse: Select-MarkdownObject -Type Block, Inline -Recurse
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1
        foreach (var item in InputObject.Descendants().Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" />.
    /// </summary>
    private void MultiTypePlusAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // Recurse: Select-MarkdownObject -Type Block, Inline -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Recurse -IncludeAttributes
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            if (_multiTypes.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_singleType"/>.
    /// </summary>
    private void SingleType()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        // Recurse: Select-MarkdownObject -Type Block -Recurse
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1
        foreach (var item in InputObject.Descendants().Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_singleType"/>s, along with all <see cref="HtmlAttributes" />.
    /// </summary>
    private void SingleTypePlusAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_singleType is not null);
        // Recurse: Select-MarkdownObject -Type Block -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes -Recurse -IncludeAttributes
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    private void BeginProcessing_Recurse(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? AnyTypePlusAttrib : AnyType;
        else if (types.Count > 0)
        {
            _multiTypes = types;
            _processInputObject = includeAttributes ? MultiTypePlusAttrib : MultiType;
        }
        else
        {
            _singleType = types[0];
            _processInputObject = includeAttributes ? SingleTypePlusAttrib : SingleType;
        }
    }
}