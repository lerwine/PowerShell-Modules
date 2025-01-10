using System.Diagnostics;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void Attributes(MarkdownObject inputObject)
    {
        // TODO: Implement Attributes
        // Recurse: Select-MarkdownObject -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type HtmlAttributes -Recurse -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyType(MarkdownObject inputObject)
    {
        // TODO: Implement AnyType
        // Recurse: Select-MarkdownObject -Recurse
        // DepthRange: Select-MarkdownObject (no parameters)
        // DepthRange: Select-MarkdownObject -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1
        // Recurse: Select-MarkdownObject -Type Any -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Any -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Inline, Any -Recurse
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttrib(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypePlusAttrib
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiType(MarkdownObject inputObject)
    {
        // TODO: Implement MultiType
        // Recurse: Select-MarkdownObject -Type Block, Inline -Recurse
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttrib(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttrib
        // Recurse: Select-MarkdownObject -Type Block, Inline -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -Recurse -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleType(MarkdownObject inputObject)
    {
        // TODO: Implement SingleType
        // Recurse: Select-MarkdownObject -Type Block -Recurse
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_singleType"/>s, along with all <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttrib(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttrib
        // Recurse: Select-MarkdownObject -Type Block -Recurse -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -IncludeAttributes
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes -Recurse
        // Recurse: Select-MarkdownObject -Type Block, HtmlAttributes -Recurse -IncludeAttributes
        throw new NotImplementedException();
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