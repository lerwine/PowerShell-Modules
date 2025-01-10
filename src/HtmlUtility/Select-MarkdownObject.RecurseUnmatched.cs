using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;
public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all direct descendants that match any of the specified types, irregardless of whether the <paramref name="inputObject"/> was a match. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInputObjAndDirectDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeInputObjAndDirectDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/> and all direct descendants that match any of the specified types, irregardless of whether the <paramref name="inputObject"/> was a match. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInputObjAndDirectDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribInputObjAndDirectDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match any of the specified types. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeToDepthInclInputObjRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeToDepthInclInputObjRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match any of the specified types, along with all descendant <see cref="HtmlAttributes" /> up to that depth. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribToDepthInclInputObjRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribToDepthInclInputObjRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeToDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeToDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> to that depth. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribToDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribToDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInRangeRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeInRangeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> within that range. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInRangeRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribInRangeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants that match any of the specified types. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeInputObjAndAllDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeInputObjAndAllDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified <see cref="_multiTypes"/>, along with all recursive descendants that match any of the specified types, along with all descendant <see cref="HtmlAttributes" />. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribInputObjAndAllDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribInputObjAndAllDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match any of the specified <see cref="_multiTypes"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" />. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypeFromDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypeFromDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match any of the specified <see cref="_multiTypes"/>, along with all <see cref="HtmlAttributes" /> from that depth. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void MultiTypePlusAttribFromDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement MultiTypePlusAttribFromDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, Inline, HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/>, along with all direct descendants that match the specified type, irregardless of whether the <paramref name="inputObject"/> was a match. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInputObjAndDirectDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeInputObjAndDirectDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with the <see cref="HtmlAttributes" /> of the <paramref name="inputObject"/> and all direct descendants that match the specified type, irregardless of whether the <paramref name="inputObject"/> was a match. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInputObjAndDirectDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribInputObjAndDirectDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match the specified type. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeToDepthInclInputObjRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeToDepthInclInputObjRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with all recursive descendants, up to the specified <see cref="_depth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribToDepthInclInputObjRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribToDepthInclInputObjRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeToDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeToDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> to that depth. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribToDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribToDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInRangeRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeInRangeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> within that range. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInRangeRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribInRangeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it matches the specified <see cref="_singleType"/>, along with all recursive descendants that match the specified type. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeInputObjAndAllDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeInputObjAndAllDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <paramref name="inputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_singleType"/>, along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribInputObjAndAllDescRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribInputObjAndAllDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_singleType"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_singleType"/>s, along with all <see cref="HtmlAttributes" />. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypeFromDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypeFromDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="_depth"/> from <paramref name="inputObject"/> that match the specified <see cref="_singleType"/>, along with all <see cref="HtmlAttributes" /> from that depth. No descendants of matches will be returned.
    /// </summary>
    /// <param name="inputObject">The <see cref="MarkdownObject"/> to process.</param>
    private void SingleTypePlusAttribFromDepthRnm(MarkdownObject inputObject)
    {
        // TODO: Implement SingleTypePlusAttribFromDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void BeginProcessing_RecurseUnmatched(List<Type> types, int minDepth, int? maxDepth, bool includeAttributes)
    {
        Debug.Assert(types is not null && types.Count > 0);
        Debug.Assert(!maxDepth.HasValue || maxDepth.Value >= minDepth);
        if (types.Count > 1)
        {
            _multiTypes = types;
            if (maxDepth.HasValue)
                switch (maxDepth.Value)
                {
                    case 0:
                        _processInputObject = includeAttributes ? MultiTypePlusAttribInputObj : MultiTypeInputObj;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? MultiTypePlusAttribDirectDesc : MultiTypeDirectDesc) :
                            includeAttributes ? MultiTypePlusAttribInputObjAndDirectDescRnm : MultiTypeInputObjAndDirectDescRnm;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            switch (minDepth)
                            {
                                case 0:
                                    _processInputObject = includeAttributes ? MultiTypePlusAttribToDepthInclInputObjRnm : MultiTypeToDepthInclInputObjRnm;
                                    break;
                                case 1:
                                    _depth = maxDepth.Value;
                                    _processInputObject = includeAttributes ? MultiTypePlusAttribToDepthRnm : MultiTypeToDepthRnm;
                                    break;
                                default:
                                    _depth = minDepth;
                                    _processInputObject = includeAttributes ? MultiTypePlusAttribInRangeRnm : MultiTypeInRangeRnm;
                                    break;
                            }
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? MultiTypePlusAttribAtDepth : MultiTypeAtDepth;
                        }
                        break;
                }
            else
                switch (minDepth)
                {
                    case 0:
                        _processInputObject = includeAttributes ? MultiTypePlusAttribInputObjAndAllDescRnm : MultiTypeInputObjAndAllDescRnm;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? MultiTypePlusAttribRnm : MultiTypeRnm;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? MultiTypePlusAttribFromDepthRnm : MultiTypeFromDepthRnm;
                        break;
                }
        }
        else
        {
            _singleType = types[0];
            if (maxDepth.HasValue)
                switch (maxDepth.Value)
                {
                    case 0:
                        _processInputObject = includeAttributes ? SingleTypePlusAttribInputObj : SingleTypeInputObj;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? SingleTypePlusAttribDirectDesc : SingleTypeDirectDesc) :
                            includeAttributes ? SingleTypePlusAttribInputObjAndDirectDescRnm : SingleTypeInputObjAndDirectDescRnm;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            switch (minDepth)
                            {
                                case 0:
                                    _processInputObject = includeAttributes ? SingleTypePlusAttribToDepthInclInputObjRnm : SingleTypeToDepthInclInputObjRnm;
                                    break;
                                case 1:
                                    _depth = maxDepth.Value;
                                    _processInputObject = includeAttributes ? SingleTypePlusAttribToDepthRnm : SingleTypeToDepthRnm;
                                    break;
                                default:
                                    _depth = minDepth;
                                    _processInputObject = includeAttributes ? SingleTypePlusAttribInRangeRnm : SingleTypeInRangeRnm;
                                    break;
                            }
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? SingleTypePlusAttribAtDepth : SingleTypeAtDepth;
                        }
                        break;
                }
            else
                switch (minDepth)
                {
                    case 0:
                        _processInputObject = includeAttributes ? SingleTypePlusAttribInputObjAndAllDescRnm : SingleTypeInputObjAndAllDescRnm;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? SingleTypeRnm : SingleTypePlusAttribRnm;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? SingleTypePlusAttribFromDepthRnm : SingleTypeFromDepthRnm;
                        break;
                }
        }
    }
}