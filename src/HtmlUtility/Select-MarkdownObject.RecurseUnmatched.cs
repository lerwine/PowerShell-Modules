using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;
public partial class Select_MarkdownObject
{
    private void WriteAttributesPlusInputObjectOrDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        if (_multiTypes.Any(t => t.IsInstanceOfType(inputObject)))
            WriteObject(inputObject, false);
        else
        {
            var attributes = inputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            if (inputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
                foreach (var item in directDescendants.Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
                    WriteObject(item, false);
        }
    }

    private void WriteInputObjectOrDirectDescendantsMatchingAnyOf(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        if (_multiTypes.Any(t => t.IsInstanceOfType(inputObject)))
            WriteObject(inputObject, false);
        else if (inputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var item in directDescendants.Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
                WriteObject(item, false);
    }

    private void WriteAttributesPlusDescendantsInRangeMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write HtmlAttributes plus descendants at _depth that match any of _multiTypes or recurse up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsInRangeMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants at _depth that match any of _multiTypes or recurse up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectOrDescendantsToDepthMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object if it matches any of _multiTypes; otherwise, attributes plus descendants that match any of _multiTypes, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteInputObjectOrDescendantsToDepthMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object if it matches any of _multiTypes; otherwise, descendants that match any of _multiTypes, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsToDepthMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write attributes plus descendants that match any of _multiTypes, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsToDepthMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants that match any of _multiTypes, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectOrDescendantsMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object if it matches any of _multiTypes; otherwise, attributes plus descendants that match any of _multiTypes, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteInputObjectOrDescendantsMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write input object if it matches any of _multiTypes; otherwise, descendants that match any of _multiTypes, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write attributes plus descendants that match any of _multiTypes, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteDescendantsMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants that match any of _multiTypes, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsFromDepthMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write attributes plus descendants from _depth that match any of _multiTypes, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteDescendantsFromDepthMatchingAnyOfOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        // TODO: Write descendants from _depth that match any of _multiTypes, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectOrDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        if (_singleType.IsInstanceOfType(inputObject))
            WriteObject(inputObject, false);
        else
        {
            var attributes = inputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            if (inputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
                foreach (var item in directDescendants.Where(_singleType.IsInstanceOfType))
                    WriteObject(item, false);
        }
    }

    private void WriteInputObjectOrDirectDescendantsMatchingType(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        if (_singleType.IsInstanceOfType(inputObject))
            WriteObject(inputObject, false);
        else if (inputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var item in directDescendants.Where(_singleType.IsInstanceOfType))
                WriteObject(item, false);
    }

    private void WriteAttributesPlusDescendantsInRangeMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write HtmlAttributes plus descendants at _depth that match _singleType or recurse up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsInRangeMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants at _depth that match _singleType or recurse up to MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectOrDescendantsToDepthMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write input object if it matches _singleType; otherwise, attributes plus descendants that match _singleType, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteInputObjectOrDescendantsToDepthMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write input object if it matches _singleType; otherwise, descendants that match _singleType, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsToDepthMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write attributes plus descendants that match _singleType, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteDescendantsToDepthMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(MaxDepth > _depth);
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants that match _singleType, otherwise recurse until MaxDepth
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusInputObjectOrDescendantsMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write input object if it matches _singleType; otherwise, attributes plus descendants that match _singleType, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteInputObjectOrDescendantsMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write input object if it matches _singleType; otherwise, descendants that match _singleType, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write attributes plus descendants that match _singleType, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteDescendantsMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants that match _singleType, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteAttributesPlusDescendantsFromDepthMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_singleType is not null);
        // TODO: Write attributes plus descendants from _depth that match _singleType, otherwise recurse
        throw new NotImplementedException();
    }

    private void WriteDescendantsFromDepthMatchingTypeOrRecurse(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_singleType is not null);
        // TODO: Write descendants from _depth that match _singleType, otherwise recurse
        throw new NotImplementedException();
    }

    private void BeginProcessing_RecurseUnmatched(List<Type> types, int minDepth, int? maxDepth, bool includeAttributes)
    {
        Debug.Assert(types is not null && types.Count > 0);
        Debug.Assert(!maxDepth.HasValue || maxDepth.Value >= minDepth);
        // TODO: Implement parameter set RecurseUnmatched
        if (types.Count > 0)
        {
            _multiTypes = types;
            if (maxDepth.HasValue)
                switch (maxDepth.Value)
                {
                    case 0:
                        _processInputObject = WriteInputObjectMatchingAnyOf;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? WriteAttributesPlusDirectDescendantsMatchingAnyOf : WriteDirectDescendantsMatchingAnyOf) :
                            includeAttributes ? WriteAttributesPlusInputObjectOrDirectDescendantsMatchingAnyOf : WriteInputObjectOrDirectDescendantsMatchingAnyOf;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            switch (minDepth)
                            {
                                case 0:
                                    _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectOrDescendantsToDepthMatchingAnyOfOrRecurse : WriteInputObjectOrDescendantsToDepthMatchingAnyOfOrRecurse;
                                    break;
                                case 1:
                                    _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsToDepthMatchingAnyOfOrRecurse : WriteDescendantsToDepthMatchingAnyOfOrRecurse;
                                    break;
                                default:
                                    _depth = minDepth;
                                    _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsInRangeMatchingAnyOfOrRecurse : WriteDescendantsInRangeMatchingAnyOfOrRecurse;
                                    break;
                            }
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsAtDepthMatchingAnyOf : WriteDescendantsAtDepthMatchingAnyOf;
                        }
                        break;
                }
            else
                switch (minDepth)
                {
                    case 0:
                        _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectOrDescendantsMatchingAnyOfOrRecurse : WriteInputObjectOrDescendantsMatchingAnyOfOrRecurse;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsMatchingAnyOfOrRecurse : WriteDescendantsMatchingAnyOfOrRecurse;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsFromDepthMatchingAnyOfOrRecurse : WriteDescendantsFromDepthMatchingAnyOfOrRecurse;
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
                        _processInputObject = WriteInputObjectMatchingType;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? WriteAttributesPlusDirectDescendantsMatchingType : WriteDirectDescendantsMatchingType) :
                            includeAttributes ? WriteAttributesPlusInputObjectOrDirectDescendantsMatchingType : WriteInputObjectOrDirectDescendantsMatchingType;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            switch (minDepth)
                            {
                                case 0:
                                    _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectOrDescendantsToDepthMatchingTypeOrRecurse : WriteInputObjectOrDescendantsToDepthMatchingTypeOrRecurse;
                                    break;
                                case 1:
                                    _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsToDepthMatchingTypeOrRecurse : WriteDescendantsToDepthMatchingTypeOrRecurse;
                                    break;
                                default:
                                    _depth = minDepth;
                                    _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsInRangeMatchingTypeOrRecurse : WriteDescendantsInRangeMatchingTypeOrRecurse;
                                    break;
                            }
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsAtDepthMatchingType : WriteDescendantsAtDepthMatchingType;
                        }
                        break;
                }
            else
                switch (minDepth)
                {
                    case 0:
                        _processInputObject = includeAttributes ? WriteAttributesPlusInputObjectOrDescendantsMatchingTypeOrRecurse : WriteInputObjectOrDescendantsMatchingTypeOrRecurse;
                        break;
                    case 1:
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsMatchingTypeOrRecurse : WriteDescendantsMatchingTypeOrRecurse;
                        break;
                    default:
                        _depth = minDepth;
                        _processInputObject = includeAttributes ? WriteAttributesPlusDescendantsFromDepthMatchingTypeOrRecurse : WriteDescendantsFromDepthMatchingTypeOrRecurse;
                        break;
                }
        }
    }
}