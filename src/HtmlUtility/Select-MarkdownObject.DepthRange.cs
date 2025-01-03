using System.Diagnostics;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    private void WriteAttributesPlusInputObjectAndDescendantsToDepth(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        // TODO: Write input object and HtmlAttributes plus descendants up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndDescendantsToDepth(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        // TODO: Write input object and descendants up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectAndDirectDescendants(MarkdownObject inputObject)
    {
        // TODO: Write input object and HtmlAttributes plus direct descendants
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndDirectDescendants(MarkdownObject inputObject)
    {
        // TODO: Write input object and direct descendants
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsToDepth(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        // TODO: Write HtmlAttributes plus descendants up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsToDepth(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        // TODO: Write descendants up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsInRange(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        // TODO: Write HtmlAttributes plus descendants from _depth to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsInRange(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        // TODO: Write descendants from _depth to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectAndDescendantsToDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object and HtmlAttributes plus descendants matching any of _multiTypes up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndDescendantsToDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object and descendants matching any of _multiTypes up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectAndDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object and HtmlAttributes plus direct descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object and direct descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsToDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write HtmlAttributes plus descendants matching any of _multiTypes up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsToDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants matching any of _multiTypes up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsInRangeMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write HtmlAttributes plus descendants matching any of _multiTypes from _depth to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsInRangeMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants matching any of _multiTypes from _depth to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectAndDescendantsToDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_singleType is not null);
        // TODO: Write input object and HtmlAttributes plus descendants matching _singleType up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndDescendantsToDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_singleType is not null);
        // TODO: Write input object and descendants matching _singleType up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectAndDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write input object and HtmlAttributes plus direct descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write input object and direct descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsToDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write HtmlAttributes plus descendants matching _singleType up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsToDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants matching _singleType up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsInRangeMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write HtmlAttributes plus descendants matching _singleType from _depth to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsInRangeMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants matching _singleType from _depth to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndAllDescendantsIncludingAttributes(MarkdownObject inputObject)
    {
        // TODO: Write input object and HtmlAttributes plus descendants
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndAllDescendants(MarkdownObject inputObject)
    {
        // TODO: Write input object and descendants
        throw new NotImplementedException();
    }

    private void WriteDescendants(MarkdownObject inputObject)
    {
        // TODO: Write descendantsh
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusAllDescendantsFromDepth(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        // TODO: Write HtmlAttributes plus descendants from _depth
        throw new NotImplementedException();
    }

    private void WriteAllDescendantsFromDepth(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        // TODO: Write descendants from _depth
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndAllDescendantsIncludingAttributesMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object and HtmlAttributes plus descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndAllDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object and descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusAllDescendantsFromDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write HtmlAttributes plus descendants matching any of _multiTypes from _depth
        throw new NotImplementedException();
    }

    private void WriteAllDescendantsFromDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants matching any of _multiTypes from _depth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectAndAllDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write input object and HtmlAttributes plus descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteInputObjectAndAllDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write input object and descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusAllDescendantsFromDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write HtmlAttributes plus descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteAllDescendantsFromDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteAttributesAndDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write direct descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteAttributesAndDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write direct descendants matching _singleType
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
                            _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndDescendantsToDepth : WriteInputObjectAndDescendantsToDepth;
                        else
                            _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndDirectDescendants : WriteInputObjectAndDirectDescendants;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsToDepth : WriteDescendantsToDepth;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsInRange : WriteDescendantsInRange;
                        break;
                }
            else if (types.Count > 0)
            {
                _multiTypes = types;
                switch (minDepth)
                {
                    case 0:
                        if (maxDepth > 1)
                            _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndDescendantsToDepthMatchingAnyOf : WriteInputObjectAndDescendantsToDepthMatchingAnyOf;
                        else
                            _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndDirectDescendantsMatchingAnyOf : WriteInputObjectAndDirectDescendantsMatchingAnyOf;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsToDepthMatchingAnyOf : WriteDescendantsToDepthMatchingAnyOf;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsInRangeMatchingAnyOf : WriteDescendantsInRangeMatchingAnyOf;
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
                            _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndDescendantsToDepthMatchingType : WriteInputObjectAndDescendantsToDepthMatchingType;
                        else
                            _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndDirectDescendantsMatchingType : WriteInputObjectAndDirectDescendantsMatchingType;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsToDepthMatchingType : WriteDescendantsToDepthMatchingType;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsInRangeMatchingType : WriteDescendantsInRangeMatchingType;
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
                    _processInputObject = includeAttributes ? WriteInputObjectAndAllDescendantsIncludingAttributes : WriteInputObjectAndAllDescendants;
                    break;
                case 1:

                    _processInputObject = includeAttributes ? WriteAllDescendantsIncludingAttributes : WriteAllDescendants;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = includeAttributes ? WriteAttributesPlusAllDescendantsFromDepth : WriteAllDescendantsFromDepth;
                    break;
            }
        else if (types.Count > 0)
        {
            _multiTypes = types;
            switch (minDepth)
            {
                case 0:
                    _processInputObject = includeAttributes ? WriteInputObjectAndAllDescendantsIncludingAttributesMatchingAnyOf : WriteInputObjectAndAllDescendantsMatchingAnyOf;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? WriteAllAttributesPlusDescendantsMatchingAnyOf : WriteAllDescendantsMatchingAnyOf;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = includeAttributes ? WriteAttributesPlusAllDescendantsFromDepthMatchingAnyOf : WriteAllDescendantsFromDepthMatchingAnyOf;
                    break;
            }
        }
        else
        {
            _singleType = types[0];
            switch (minDepth)
            {
                case 0:
                    _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectAndAllDescendantsMatchingType : WriteInputObjectAndAllDescendantsMatchingType;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? WriteAllAttributesPlusAllDescendantsMatchingType : WriteAllDescendantsMatchingType;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = includeAttributes ? WriteAttributesPlusAllDescendantsFromDepthMatchingType : WriteAllDescendantsFromDepthMatchingType;
                    break;
            }
        }
    }

    private void BeginProcessing_NoRecursion(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? WriteAttributesAndDirectDescendants : WriteDirectDescendants;
        else if (types.Count > 1)
        {
            _multiTypes = types;
            _processInputObject = includeAttributes ? WriteAttributesAndDirectDescendantsMatchingAnyOf : WriteDirectDescendantsMatchingAnyOf;
        }
        else
        {
            _singleType = types[0];
            _processInputObject = includeAttributes ? WriteAttributesAndDirectDescendantsMatchingType : WriteDirectDescendantsMatchingType;
        }
    }
}