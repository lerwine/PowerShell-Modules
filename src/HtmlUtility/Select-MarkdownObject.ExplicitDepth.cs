using System.Diagnostics;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    private void WriteInputObject(MarkdownObject inputObject)
    {
        // TODO: Just write to output
        throw new NotImplementedException();
    }

    private void WriteAttributesAndDirectDescendants(MarkdownObject inputObject)
    {
        // TODO: Write HtmlAttributes and all direct descendants
        throw new NotImplementedException();
    }

    private void WriteDirectDescendants(MarkdownObject inputObject)
    {
        // TODO: Write all direct descendants
        throw new NotImplementedException();
    }

    private void WriteAttributesAndDescendantsAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        // TODO: Write all descendants at _depth
        throw new NotImplementedException();
    }

    private void WriteDescendantsAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        // TODO: Write all descendants at _depth
        throw new NotImplementedException();
    }

    private void WriteInputObjectMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write to output if matches any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write HtmlAttributes plus direct descendants that match any of _multiTypes to output
        throw new NotImplementedException();
    }

    private void WriteDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write direct descendants that match any of _multiTypes to output
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsAtDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write HtmlAttributes plus direct descendants at _depth that match any of _multiTypes to output
        throw new NotImplementedException();
    }

    private void WriteDescendantsAtDepthMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write direct descendants at _depth that match any of _multiTypes to output
        throw new NotImplementedException();
    }

    private void WriteInputObjectMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write to output if matches _singleType
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write HtmlAttributes plus direct descendants that match _singleType to output
        throw new NotImplementedException();
    }

    private void WriteDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write direct descendants that match _singleType to output
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsAtDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_singleType is not null);
        // TODO: Write HtmlAttributes plus direct descendants at _depth that match _singleType to output
        throw new NotImplementedException();
    }

    private void WriteDescendantsAtDepthMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_singleType is not null);
        // TODO: Write direct descendants at _depth that match _singleType to output
        throw new NotImplementedException();
    }

    private void BeginProcessing_ExplicitDepth(List<Type>? types, int depth, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            switch (depth)
            {
                case 0:
                    _processInputObject = WriteInputObject;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? WriteAttributesAndDirectDescendants : WriteDirectDescendants;
                    break;
                default:
                    _depth = depth;
                    _processInputObject = includeAttributes ? WriteAttributesAndDescendantsAtDepth : WriteDescendantsAtDepth;
                    break;
            }
        else if (types.Count > 1)
        {
            _multiTypes = types;
            switch (depth)
            {
                case 0:
                    _processInputObject = WriteInputObjectMatchingAnyOf;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? WriteAttributesPlusDirectDescendantsMatchingAnyOf : WriteDirectDescendantsMatchingAnyOf;
                    break;
                default:
                    _depth = depth;
                    _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsAtDepthMatchingAnyOf : WriteDescendantsAtDepthMatchingAnyOf;
                    break;
            }
        }
        else
        {
            _singleType = types[0];
            switch (depth)
            {
                case 0:
                    _processInputObject = WriteInputObjectMatchingType;
                    break;
                case 1:
                    _processInputObject = includeAttributes ? WriteAttributesPlusDirectDescendantsMatchingType : WriteDirectDescendantsMatchingType;
                    break;
                default:
                    _depth = depth;
                    _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsAtDepthMatchingType : WriteDescendantsAtDepthMatchingType;
                    break;
            }
        }
    }
}