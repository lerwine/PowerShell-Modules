using System.Diagnostics;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;
public partial class Select_MarkdownObject
{
    private void WriteRecurseUnmatchedInclInputObj(MarkdownObject inputObject)
    {
        if (_predicate(inputObject))
            WriteObject(inputObject, false);
        else
            foreach (var item in inputObject.GetDirectDescendants())
                WriteRecurseUnmatchedInclInputObj(item);
    }

    private void WriteRecurseUnmatchedInclInputObjOrAttrib(MarkdownObject inputObject)
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        if (_predicate(inputObject))
            WriteObject(inputObject, false);
        else
        {
            var attributes = inputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in inputObject.GetDirectDescendants())
                WriteRecurseUnmatchedInclInputObj(item);
        }
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatched()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        foreach (var item in InputObject.GetDirectDescendants())
            WriteRecurseUnmatchedInclInputObj(item);
    }

    /// <summary>
    /// Returns all recursive descendants that match the specified <see cref="_predicate"/>, along with all <see cref="HtmlAttributes" />.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedPlusAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        foreach (var item in InputObject.GetDirectDescendants())
            WriteRecurseUnmatchedInclInputObjOrAttrib(item);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants that match the specified type.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        WriteRecurseUnmatchedInclInputObj(InputObject);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with all recursive descendants that match the specified type, along with all descendant <see cref="HtmlAttributes" />.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedInclInputObjOrAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
            WriteRecurseUnmatchedInclInputObjOrAttrib(InputObject);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedFromDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        foreach (var item in InputObject.GetDescendantsAtDepth(MinDepth))
            WriteRecurseUnmatchedInclInputObj(item);
    }

    /// <summary>
    /// Returns all recursive descendants, starting from the specified <see cref="MinDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> from that depth.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedFromDepthOrAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        foreach (var parent in InputObject.GetDescendantsAtDepth(MinDepth - 1))
        {
            var attributes = parent.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in parent.GetDirectDescendants())
                WriteRecurseUnmatchedInclInputObj(item);
        }
    }

    private void WriteRecurseUnmatchedToDepthInclInputObj(MarkdownObject inputObject, int maxDepth)
    {
        if (_predicate(inputObject))
            WriteObject(inputObject, false);
        else if (maxDepth > 1)
        {
            maxDepth--;
            foreach (var item in inputObject.GetDirectDescendants())
                WriteRecurseUnmatchedToDepthInclInputObj(item, maxDepth);
        }
        else
            foreach (var item in inputObject.GetDirectDescendants().Where(_predicate))
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedToDepth()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        var maxDepth = MaxDepth - 1;
        foreach (var item in InputObject.GetDirectDescendants())
            WriteRecurseUnmatchedToDepthInclInputObj(item, maxDepth);
    }

    private void WriteRecurseUnmatchedPlusAttribToDepthInclInputObj(MarkdownObject inputObject, int maxDepth)
    {
        if (_predicate(inputObject))
            WriteObject(inputObject, false);
        else
        {
            var attributes = inputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            if (maxDepth > 1)
            {
                maxDepth--;
                foreach (var item in inputObject.GetDirectDescendants())
                    WriteRecurseUnmatchedToDepthInclInputObj(item, maxDepth);
            }
            else
                foreach (var item in inputObject.GetDirectDescendants().Where(_predicate))
                    WriteObject(item, false);
        }
    }

    /// <summary>
    /// Returns all recursive descendants, up to the specified <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> to that depth.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedToDepthOrAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(MaxDepth > 1);
        var maxDepth = MaxDepth - 1;
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.GetDirectDescendants())
            WriteRecurseUnmatchedPlusAttribToDepthInclInputObj(item, maxDepth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all recursive descendants, up to the specified <see cref="MaxDepth"/>,
    /// that match the specified type.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        WriteRecurseUnmatchedToDepthInclInputObj(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches the specified <see cref="_predicate"/>,
    /// along with all recursive descendants, up to the specified <see cref="MaxDepth"/>, that match the specified type, along with all descendant <see cref="HtmlAttributes" /> up to that depth.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedPlusAttribToDepthInclInputObj()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MaxDepth > 1);
        if (InputObject is HtmlAttributes)
            WriteObject(InputObject, false);
        else
            WriteRecurseUnmatchedPlusAttribToDepthInclInputObj(InputObject, MaxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        var maxDepth = MaxDepth - MinDepth;
        foreach (var item in InputObject.GetDescendantsAtDepth(MinDepth))
            WriteRecurseUnmatchedToDepthInclInputObj(item, maxDepth);
    }

    /// <summary>
    /// Returns all recursive descendants, from the specified <see cref="MinDepth"/> to the <see cref="MaxDepth"/> from <see cref="InputObject"/> that match the specified <see cref="_predicate"/>,
    /// along with all <see cref="HtmlAttributes" /> within that range.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteRecurseUnmatchedOrAttribInRange()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        Debug.Assert(MinDepth > 1);
        Debug.Assert(MaxDepth > MinDepth);
        var maxDepth = MaxDepth - MinDepth - 1;
        if (maxDepth == 0)
        {
            foreach (var parent in InputObject.GetDescendantsAtDepth(MinDepth - 1))
            {
                var attributes = parent.TryGetAttributes();
                if (attributes is not null)
                    WriteObject(attributes, false);
                foreach (var item in parent.GetDirectDescendants().Where(_predicate))
                    WriteObject(item, false);
            }
        }
        else
        {
            foreach (var parent in InputObject.GetDescendantsAtDepth(MinDepth - 1))
            {
                var attributes = parent.TryGetAttributes();
                if (attributes is not null)
                    WriteObject(attributes, false);
                foreach (var item in parent.GetDirectDescendants())
                    WriteRecurseUnmatchedToDepthInclInputObj(item, maxDepth);
            }
        }
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it matches the specified <see cref="_predicate"/>, along with all direct descendants that match the specified type,
    /// irregardless of whether the <see cref="InputObject"/> was a match.
    /// No descendants of matches will be returned.
    /// </summary>
    private void WriteTypePredicatedInputObjOrDirectDesc()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        if (_predicate(InputObject))
            WriteObject(InputObject, false);
        else
            foreach (var item in InputObject.GetDirectDescendants().Where(_predicate))
                WriteObject(item, false);
    }

    /// <summary>
    /// Returns the <see cref="InputObject"/> if it is <see cref="HtmlAttributes" /> or it matches any of the specified type(s);
    /// otherwise, the <see cref="HtmlAttributes" /> of the <see cref="InputObject"/> if it exists, along with all direct descendants that match the specified type(s).
    /// </summary>
    private void WriteTypePredicatedInputObjDirectDescOrAttrib()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(_types is not null && _types.Count > 0);
        Debug.Assert(_predicate is not null);
        if (InputObject is HtmlAttributes || _predicate(InputObject))
            WriteObject(InputObject, false);
        else
        {
            var attributes = InputObject.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            foreach (var item in InputObject.GetDirectDescendants().Where(_predicate))
                WriteObject(item, false);
        }
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
                        _processInputObject = includeAttributes ? WriteTypePredicatedOrAttribInputObj : WriteTypePredicatedInputObj;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? WriteAttribAndTypePredicatedDirectDesc : WriteTypePredicatedDirectDesc) :
                            includeAttributes ? WriteTypePredicatedInputObjDirectDescOrAttrib : WriteTypePredicatedInputObjOrDirectDesc;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            _processInputObject = minDepth switch
                            {
                                0 => includeAttributes ? WriteRecurseUnmatchedPlusAttribToDepthInclInputObj : WriteRecurseUnmatchedToDepthInclInputObj,
                                1 => includeAttributes ? WriteRecurseUnmatchedToDepthOrAttrib : WriteRecurseUnmatchedToDepth,
                                _ => includeAttributes ? WriteRecurseUnmatchedOrAttribInRange : WriteRecurseUnmatchedInRange,
                            };
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedAtDepth : WriteTypePredicatedAtDepth;
                        }
                        break;
                }
            else
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? WriteRecurseUnmatchedInclInputObjOrAttrib : WriteRecurseUnmatchedInclInputObj,
                    1 => includeAttributes ? WriteRecurseUnmatchedPlusAttrib : WriteRecurseUnmatched,
                    _ => includeAttributes ? WriteRecurseUnmatchedFromDepthOrAttrib : WriteRecurseUnmatchedFromDepth,
                };
        }
        else
        {
            _predicate = types[0].IsInstanceOfType;
            if (maxDepth.HasValue)
                switch (maxDepth.Value)
                {
                    case 0:
                        _processInputObject = includeAttributes ? WriteTypePredicatedOrAttribInputObj : WriteTypePredicatedInputObj;
                        break;
                    case 1:
                        _processInputObject = (minDepth == 1) ? (includeAttributes ? WriteAttribAndTypePredicatedDirectDesc : WriteTypePredicatedDirectDesc) :
                            includeAttributes ? WriteTypePredicatedInputObjDirectDescOrAttrib : WriteTypePredicatedInputObjOrDirectDesc;
                        break;
                    default:
                        if (maxDepth.Value > minDepth)
                            _processInputObject = minDepth switch
                            {
                                0 => includeAttributes ? WriteRecurseUnmatchedPlusAttribToDepthInclInputObj : WriteRecurseUnmatchedToDepthInclInputObj,
                                1 => includeAttributes ? WriteRecurseUnmatchedToDepthOrAttrib : WriteRecurseUnmatchedToDepth,
                                _ => includeAttributes ? WriteRecurseUnmatchedOrAttribInRange : WriteRecurseUnmatchedInRange,
                            };
                        else
                        {
                            _depth = minDepth;
                            _processInputObject = includeAttributes ? WriteAttribAndTypePredicatedAtDepth : WriteTypePredicatedAtDepth;
                        }
                        break;
                }
            else
                _processInputObject = minDepth switch
                {
                    0 => includeAttributes ? WriteRecurseUnmatchedInclInputObjOrAttrib : WriteRecurseUnmatchedInclInputObj,
                    1 => includeAttributes ? WriteRecurseUnmatched : WriteRecurseUnmatchedPlusAttrib,
                    _ => includeAttributes ? WriteRecurseUnmatchedFromDepthOrAttrib : WriteRecurseUnmatchedFromDepth,
                };
        }
    }
}