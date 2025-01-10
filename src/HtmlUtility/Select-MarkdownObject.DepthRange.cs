using System.Diagnostics;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" />; otherwise the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/> is returned, if present.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement AttributesInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> up to the specified <see cref="_depth"/> from <paramref name="inputObject"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesToDepth(MarkdownObject inputObject)
    {
        // TODO: Implement AttributesToDepth
        // DepthRange: Select-MarkdownObject -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" />; otherwise all recursive descendant <see cref="HtmlAttributes" /> up to the specified <see cref="_depth"/> will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement AttributesToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> within the specified <see cref="_depth"/> range from <paramref name="inputObject"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesInRange(MarkdownObject inputObject)
    {
        // TODO: Implement AttributesInRange
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" />; otherwise all recursive descendant <see cref="HtmlAttributes" /> will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesInputObjAndAllDesc(MarkdownObject inputObject)
    {
        // TODO: Implement AttributesInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendant <see cref="HtmlAttributes" /> starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AttributesFromDepth(MarkdownObject inputObject)
    {
        // TODO: Implement AttributesFromDepth
        // DepthRange: Select-MarkdownObject -MinDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is *NOT* <see cref="HtmlAttributes" />, along with all recursive descendants up to the specified <see cref="_depth"/>, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypeToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> along with all recursive descendants up to the specified <see cref="_depth"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypePlusAttribToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes, Any -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is *NOT* <see cref="HtmlAttributes" />, along with all direct descendants, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypeInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -MinDepth 0 -MaxDepth 1
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/>, along with all direct descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypePlusAttribInputObjAndDirectDesc
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants up to the specified <see cref="_depth"/> from <paramref name="inputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeToDepth(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypeToDepth
        // DepthRange: Select-MarkdownObject -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MaxDepth 2
        // DepthRange: Select-MarkdownObject -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 1 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 1 -MaxDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribToDepth(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypePlusAttribToDepth
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeInRange(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypeInRange
        // DepthRange: Select-MarkdownObject -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2 -MaxDepth 3
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribInRange(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypePlusAttribInRange
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match any of the specified types.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match any of the specified types, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all direct descendants that match any of the specified types, irregardless of whether the <paramref name="inputObject"/> was a match.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/> and all direct descendants that match any of the specified types, irregardless of whether the <paramref name="inputObject"/> was a match.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeToDepth(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeToDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribToDepth(MarkdownObject inputObject)
    {
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
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInRange(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeInRange
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> within that range.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInRange(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribInRange
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match the specified type.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribToDepthInclInputObj(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribToDepthInclInputObj
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/>, along with all direct descendants that match the specified type, irregardless of whether the <paramref name="inputObject"/> was a match.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/> and all direct descendants that match the specified type, irregardless of whether the <paramref name="inputObject"/> was a match.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInputObjAndDirectDesc(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribInputObjAndDirectDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeToDepth(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeToDepth
        // DepthRange: Select-MarkdownObject -Type Block -MaxDepth 2
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> to that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribToDepth(MarkdownObject inputObject)
    {
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
    /// Returns all recursive descendants, within the specified <see cref="_depth"/> range from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInRange(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeInRange
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, within the specified <see cref="_depth"/> range from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> within that range.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInRange(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribInRange
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -IncludeAttributes
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is *NOT* <see cref="HtmlAttributes" />, along with all recursive descendants, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeInputObjAndAllDesc(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypeInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 0
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> along with all recursive descendants, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribInputObjAndAllDesc(MarkdownObject inputObject)
    {
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
    /// Returns all recursive descendants starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/>, *NOT* including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypeFromDepth(MarkdownObject inputObject)
    {
        // TODO: Implement AnyTypeFromDepth
        // DepthRange: Select-MarkdownObject -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Any -MinDepth 2
        // DepthRange: Select-MarkdownObject -Type Block, Inline, Any -MinDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/>, including <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void AnyTypePlusAttribFromDepth(MarkdownObject inputObject)
    {
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
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants that match any of the specified types.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInputObjAndAllDesc(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants that match any of the specified types, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInputObjAndAllDesc(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeFromDepth(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeFromDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribFromDepth(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribFromDepth
        // DepthRange: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants that match the specified type.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInputObjAndAllDesc(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInputObjAndAllDesc(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribInputObjAndAllDesc
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 0 -IncludeAttributes
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0
        // DepthRange: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -IncludeAttributes
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeFromDepth(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeFromDepth
        // DepthRange: Select-MarkdownObject -Type Block -MinDepth 2
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> from that depth.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribFromDepth(MarkdownObject inputObject)
    {
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
                switch (minDepth)
                {
                    case 0:
                        if (maxDepth > 1)
                            _processInputObject = includeAttributes ? AnyTypePlusAttribToDepthInclInputObj : AnyTypeToDepthInclInputObj;
                        else
                            _processInputObject = includeAttributes ? AnyTypePlusAttribInputObjAndDirectDesc : AnyTypeInputObjAndDirectDesc;
                        break;
                    case 1:
                        _depth = maxDepth;
                        _processInputObject = includeAttributes ? AnyTypePlusAttribToDepth : AnyTypeToDepth;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? AnyTypePlusAttribInRange : AnyTypeInRange;
                        break;
                }
            else if (types.Count > 0)
            {
                _multiTypes = types;
                switch (minDepth)
                {
                    case 0:
                        if (maxDepth > 1)
                            _processInputObject = includeAttributes ? MultiTypePlusAttribToDepthInclInputObj : MultiTypeToDepthInclInputObj;
                        else
                            _processInputObject = includeAttributes ? MultiTypePlusAttribInputObjAndDirectDesc : MultiTypeInputObjAndDirectDesc;
                        break;
                    case 1:
                        _depth = maxDepth;
                        _processInputObject = includeAttributes ? MultiTypePlusAttribToDepth : MultiTypeToDepth;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? MultiTypePlusAttribInRange : MultiTypeInRange;
                        break;
                }
            }
            else
            {
                _singleType = types[0];
                switch (minDepth)
                {
                    case 0:
                        if (maxDepth > 1)
                            _processInputObject = includeAttributes ? SingleTypePlusAttribToDepthInclInputObj : SingleTypeToDepthInclInputObj;
                        else
                            _processInputObject = includeAttributes ? SingleTypePlusAttribInputObjAndDirectDesc : SingleTypeInputObjAndDirectDesc;
                        break;
                    case 1:
                        _depth = maxDepth;
                        _processInputObject = includeAttributes ? SingleTypePlusAttribToDepth : SingleTypeToDepth;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? SingleTypePlusAttribInRange : SingleTypeInRange;
                        break;
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
            switch (minDepth)
            {
                case 0:
                    _processInputObject = includeAttributes ? AnyTypePlusAttribInputObjAndAllDesc : AnyTypeInputObjAndAllDesc;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? AnyTypePlusAttrib : AnyType;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = includeAttributes ? AnyTypePlusAttribFromDepth : AnyTypeFromDepth;
                    break;
            }
        else if (types.Count > 0)
        {
            _multiTypes = types;
            switch (minDepth)
            {
                case 0:
                    _processInputObject = includeAttributes ? MultiTypePlusAttribInputObjAndAllDesc : MultiTypeInputObjAndAllDesc;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? MultiTypePlusAttrib : MultiType;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = includeAttributes ? MultiTypePlusAttribFromDepth : MultiTypeFromDepth;
                    break;
            }
        }
        else
        {
            _singleType = types[0];
            switch (minDepth)
            {
                case 0:
                    _processInputObject = includeAttributes ? SingleTypePlusAttribInputObjAndAllDesc : SingleTypeInputObjAndAllDesc;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? SingleTypePlusAttrib : SingleType;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = includeAttributes ? SingleTypePlusAttribFromDepth : SingleTypeFromDepth;
                    break;
            }
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