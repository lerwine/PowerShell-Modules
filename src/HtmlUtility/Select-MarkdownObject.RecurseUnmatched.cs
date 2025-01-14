using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;
public partial class Select_MarkdownObject
{
    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedInputObjAndDirectDescRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // TODO: Implement SingleTypeInputObjAndDirectDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> and all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribInputObjAndDirectDescRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // TODO: Implement SingleTypePlusAttribInputObjAndDirectDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants, up to the specified <see cref="MaxDepth"/>,
    /// that match the specified type.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedToDepthInclInputObjRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // TODO: Implement SingleTypeToDepthInclInputObjRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with all recursive descendants, up to the specified <see cref="MaxDepth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribToDepthInclInputObjRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // TODO: Implement SingleTypePlusAttribToDepthInclInputObjRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedToDepthRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        // TODO: Implement SingleTypeToDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribToDepthRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(MaxDepth > 1);
        // TODO: Implement SingleTypePlusAttribToDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 1 -MaxDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedInRangeRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // TODO: Implement SingleTypeInRangeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> within that range.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribInRangeRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        // TODO: Implement SingleTypePlusAttribInRangeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -MaxDepth 3 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants that match the specified type.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedInputObjAndAllDescRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // TODO: Implement SingleTypeInputObjAndAllDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribInputObjAndAllDescRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // TODO: Implement SingleTypePlusAttribInputObjAndAllDescRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // TODO: Implement SingleTypeRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>, along with all <see cref="HtmlAttributes" />.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        // TODO: Implement SingleTypePlusAttribRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedFromDepthRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement SingleTypeFromDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// No descendants of matches will be returned.
    /// </summary>
    private void TypePredicatedPlusAttribFromDepthRnm()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        // TODO: Implement SingleTypePlusAttribFromDepthRnm
        // RecurseUnmatched: Select-MarkdownObject -Type Block, HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void BeginProcessing_RecurseUnmatched(List<Type> types, int minDepth, int? maxDepth, bool includeAttributes)
    {
        Debug.Assert(types is not null && types.Count > 0);
        Debug.Assert(!maxDepth.HasValue || maxDepth.Value >= minDepth);
        _types = types;
        if (types.Count > 1)
        {
            _predicate = (MarkdownObject a) => _types.Any(t => t.IsInstanceOfType(a));
            if (maxDepth.HasValue)
                switch (maxDepth.Value)
                {
                    case 0:
                        _processInputObject = includeAttributes ? TypePredicatedPlusAttribInputObj : TypePredicatedInputObj;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? TypePredicatedPlusAttribDirectDesc : TypePredicatedDirectDesc) :
                            includeAttributes ? TypePredicatedPlusAttribInputObjAndDirectDescRnm : TypePredicatedInputObjAndDirectDescRnm;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            _processInputObject = minDepth switch
                            {
                                0 => includeAttributes ? TypePredicatedPlusAttribToDepthInclInputObjRnm : TypePredicatedToDepthInclInputObjRnm,
                                1 => includeAttributes ? TypePredicatedPlusAttribToDepthRnm : TypePredicatedToDepthRnm,
                                _ => includeAttributes ? TypePredicatedPlusAttribInRangeRnm : TypePredicatedInRangeRnm,
                            };
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? TypePredicatedPlusAttribAtDepth : TypePredicatedAtDepth;
                        }
                        break;
                }
            else
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? TypePredicatedPlusAttribInputObjAndAllDescRnm : TypePredicatedInputObjAndAllDescRnm,
                    1 => includeAttributes ? TypePredicatedPlusAttribRnm : TypePredicatedRnm,
                    _ => includeAttributes ? TypePredicatedPlusAttribFromDepthRnm : TypePredicatedFromDepthRnm,
                };
        }
        else
        {
            _predicate = types[0].IsInstanceOfType;
            if (maxDepth.HasValue)
                switch (maxDepth.Value)
                {
                    case 0:
                        _processInputObject = includeAttributes ? TypePredicatedPlusAttribInputObj : TypePredicatedInputObj;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? TypePredicatedPlusAttribDirectDesc : TypePredicatedDirectDesc) :
                            includeAttributes ? TypePredicatedPlusAttribInputObjAndDirectDescRnm : TypePredicatedInputObjAndDirectDescRnm;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            _processInputObject = minDepth switch
                            {
                                0 => includeAttributes ? TypePredicatedPlusAttribToDepthInclInputObjRnm : TypePredicatedToDepthInclInputObjRnm,
                                1 => includeAttributes ? TypePredicatedPlusAttribToDepthRnm : TypePredicatedToDepthRnm,
                                _ => includeAttributes ? TypePredicatedPlusAttribInRangeRnm : TypePredicatedInRangeRnm,
                            };
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? TypePredicatedPlusAttribAtDepth : TypePredicatedAtDepth;
                        }
                        break;
                }
            else
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? TypePredicatedPlusAttribInputObjAndAllDescRnm : TypePredicatedInputObjAndAllDescRnm,
                    1 => includeAttributes ? TypePredicatedRnm : TypePredicatedPlusAttribRnm,
                    _ => includeAttributes ? TypePredicatedPlusAttribFromDepthRnm : TypePredicatedFromDepthRnm,
                };
        }
    }
}