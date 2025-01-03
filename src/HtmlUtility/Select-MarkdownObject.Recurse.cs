using System.Diagnostics;
using Markdig.Syntax;

namespace HtmlUtility;

public partial class Select_MarkdownObject
{
    private void WriteAllDescendantsIncludingAttributes(MarkdownObject inputObject)
    {
        // TODO: Write all descendants, including HtmlAttributes
        throw new NotImplementedException();
    }

    private void WriteAllDescendants(MarkdownObject inputObject)
    {
        // TODO: Write all descendants
        throw new NotImplementedException();
    }

    private void WriteAllAttributesPlusDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write all HtmlAttributes, plus descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteAllDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write all descendants matching any of _multiTypes
        throw new NotImplementedException();
    }

    private void WriteAllAttributesPlusAllDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write all HtmlAttributes, plus descendants matching _singleType
        throw new NotImplementedException();
    }

    private void WriteAllDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write all descendants matching _singleType
        throw new NotImplementedException();
    }

    private void BeginProcessing_Recurse(List<Type>? types, bool includeAttributes)
    {
        Debug.Assert(types is null || types.Count > 0);
        if (types is null)
            _processInputObject = includeAttributes ? WriteAllDescendantsIncludingAttributes : WriteAllDescendants;
        else if (types.Count > 0)
        {
            _multiTypes = types;
            _processInputObject = includeAttributes ? WriteAllAttributesPlusDescendantsMatchingAnyOf : WriteAllDescendantsMatchingAnyOf;
        }
        else
        {
            _singleType = types[0];
            _processInputObject = includeAttributes ? WriteAllAttributesPlusAllDescendantsMatchingType : WriteAllDescendantsMatchingType;
        }
    }
}