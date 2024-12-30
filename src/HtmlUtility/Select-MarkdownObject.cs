using System.Management.Automation;
using Markdig.Syntax;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "MarkdownObject", DefaultParameterSetName = ParameterSetName_DepthRange)]
public partial class Select_MarkdownObject : PSCmdlet
{
    public const string ParameterSetName_DepthRange = "DepthRange";
    public const string ParameterSetName_ExplicitDepth = "ExplicitDepth";
    public const string ParameterSetName_Recurse = "Recurse";
    public const string ParameterSetName_RecurseUnmatched = "RecurseUnmatched";
    private const string HtmlMessage_Type = "The type of the Markdown object(s) to select.";

    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select from.")]
    [ValidateNotNull()]
    [Alias("MarkdownObject", "Markdown")]
    public MarkdownObject[] InputObject { get; set; } = null!;

    [Parameter(Position = 1, HelpMessage = HtmlMessage_Type, ParameterSetName = ParameterSetName_DepthRange)]
    [Parameter(Position = 1, HelpMessage = HtmlMessage_Type, ParameterSetName = ParameterSetName_ExplicitDepth)]
    [Parameter(Position = 1, HelpMessage = HtmlMessage_Type, ParameterSetName = ParameterSetName_Recurse)]
    [Parameter(Mandatory = true, Position = 1, HelpMessage = HtmlMessage_Type, ParameterSetName = ParameterSetName_RecurseUnmatched)]
    public MarkdownTokenType[] Type { get; set; } = null!;

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitDepth)]
    [ValidateRange(1, int.MaxValue)]
    public int Depth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    [Parameter(ParameterSetName = ParameterSetName_RecurseUnmatched)]
    [ValidateRange(1, int.MaxValue)]
    public int MinDepth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    [Parameter(ParameterSetName = ParameterSetName_RecurseUnmatched)]
    [ValidateRange(1, int.MaxValue)]
    public int MaxDepth { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Recurse)]
    public SwitchParameter Recurse { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_RecurseUnmatched)]
    public SwitchParameter RecurseUnmatchedOnly { get; set; }

    // TODO: Fix logic - IncludeAttributes is only applicable when type is not specified
    public SwitchParameter IncludeAttributes { get; set; }

    private List<Type> _multiType = null!;
    private Type _singleType = null!;
    private int _depth;
    private Action<MarkdownObject> _processInputObject = null!;

    protected override void BeginProcessing()
    {
        List<Type>? typesToMatch = Type.ToReflectionTypes();
        if (typesToMatch is not null)
        {
            if (typesToMatch.Count == 1)
                _singleType = typesToMatch[0];
            else
                _multiType = typesToMatch;
        }
        switch (ParameterSetName)
        {
            case ParameterSetName_ExplicitDepth:
                _depth = Depth;
                if (typesToMatch is null)
                    _processInputObject = GetDescendantsAtDepth;
                else if (typesToMatch.Count == 1)
                    _processInputObject = GetDescendantsMatchingSingleTypeAtDepth;
                else
                    _processInputObject = GetDescendantsMatchingTypeAtDepth;
                break;
            case ParameterSetName_Recurse:
                if (typesToMatch is null)
                    _processInputObject = GetCurrentAndDescendants;
                else if (typesToMatch.Count == 1)
                    _processInputObject = GetCurrentAndDescendantsMatchingSingleTypeRecurseAll;
                else
                    _processInputObject = GetCurrentAndDescendantsMatchingTypeRecurseAll;
                break;
            default:
                if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
                {
                    if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
                    {
                        _depth = MaxDepth = MinDepth;
                        if (_depth < 0)
                        {
                            WriteError(new(new ArgumentOutOfRangeException(nameof(MaxDepth), $"{nameof(MaxDepth)} cannot be less than {nameof(MinDepth)}"), "", ErrorCategory.InvalidArgument, MaxDepth));
                            throw new PipelineStoppedException();
                        }
                        if (_depth == 0)
                        {
                            _depth = MinDepth;
                            if (typesToMatch is null)
                                _processInputObject = GetDescendantsAtDepth;
                            else if (typesToMatch.Count == 1)
                                _processInputObject = GetDescendantsMatchingSingleTypeAtDepth;
                            else
                                _processInputObject = GetDescendantsMatchingTypeAtDepth;
                        }
                        else if (RecurseUnmatchedOnly.IsPresent)
                        {
                            if (typesToMatch!.Count == 1)
                                _processInputObject = GetDescendantsMatchingSingleTypeInDepthRange;
                            else
                                _processInputObject = GetDescendantsMatchingTypeInDepthRange;
                        }
                        else if (typesToMatch is null)
                            _processInputObject = GetDescendantsInDepthRange;
                        else if (typesToMatch.Count == 1)
                            _processInputObject = GetDescendantsMatchingSingleTypeInDepthRangeRecurseAll;
                        else
                            _processInputObject = GetDescendantsMatchingTypeInDepthRangeRecurseAll;
                    }
                    else if (RecurseUnmatchedOnly.IsPresent)
                    {
                        if (typesToMatch!.Count == 1)
                            _processInputObject = GetCurrentAndDescendantsMatchingSingleTypeUpToDepth;
                        else
                            _processInputObject = GetCurrentAndDescendantsMatchingTypeUpToDepth;
                    }
                    else
                    {
                        if (typesToMatch is null)
                            _processInputObject = GetCurrentAndDescendantsUpToDepth;
                        else if (typesToMatch.Count == 1)
                            _processInputObject = GetCurrentAndDescendantsMatchingSingleTypeUpToDepthRecurseAll;
                        else
                            _processInputObject = GetCurrentAndDescendantsMatchingTypeUpToDepthRecurseAll;
                    }
                }
                else if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
                {
                    if (RecurseUnmatchedOnly.IsPresent)
                    {
                        if (typesToMatch!.Count == 1)
                            _processInputObject = GetDescendantsMatchingSingleTypeFromDepth;
                        else
                            _processInputObject = GetDescendantsMatchingTypeFromDepth;
                    }
                    else
                    {
                        if (typesToMatch is null)
                            _processInputObject = GetDescendantsFromDepth;
                        else if (typesToMatch.Count == 1)
                            _processInputObject = GetDescendantsMatchingSingleTypeFromDepthRecurseAll;
                        else
                            _processInputObject = GetDescendantsMatchingTypeFromDepthRecurseAll;
                    }
                }
                else if (RecurseUnmatchedOnly.IsPresent)
                {
                    if (typesToMatch!.Count == 1)
                        _processInputObject = GetCurrentAndDescendantsMatchingSingleType;
                    else
                        _processInputObject = GetCurrentAndDescendantsMatchingType;
                }
                else
                {
                    if (typesToMatch is null)
                        _processInputObject = obj => WriteObject(obj, false);
                    else if (typesToMatch.Count == 1)
                        _processInputObject = GetCurrentMatchingSingleType;
                    else
                        _processInputObject = GetCurrentMatchingType;
                }
                break;
        }
    }

    private void GetCurrentAndDescendants(MarkdownObject currentObject)
    {
        WriteObject(currentObject, false);
        foreach (var c in IncludeAttributes.IsPresent ? currentObject.GetDescendantsAndAttributes() : currentObject.Descendants())
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingSingleTypeRecurseAll(MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        // TODO: Logic could probably be better when _singleType is HtmlAttributes
        foreach (var c in (IncludeAttributes.IsPresent ? currentObject.GetDescendantsAndAttributes() : currentObject.Descendants()).Where(_singleType.IsInstanceOfType))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingSingleType(MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.DescendantBranchesMatchingType(_singleType))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentMatchingSingleType(MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
    }

    private void GetCurrentAndDescendantsMatchingTypeRecurseAll(MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        // TODO: Logic could probably be better when _singleType is HtmlAttributes
        foreach (var c in (IncludeAttributes.IsPresent ? currentObject.GetDescendantsAndAttributes() : currentObject.Descendants()).Where(obj => _multiType.Any(t => t.IsInstanceOfType(obj))))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingType(MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.DescendantBranchesMatchingType(_multiType))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentMatchingType(MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
    }

    private void GetDescendantsAtDepth(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(_depth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingSingleTypeAtDepth(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(_depth, IncludeAttributes.IsPresent).Where(_singleType.IsInstanceOfType))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingTypeAtDepth(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(_depth, IncludeAttributes.IsPresent).Where(obj => _multiType.Any(t => t.IsInstanceOfType(obj))))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsFromDepth(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsFromDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingSingleTypeFromDepth(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            else
                foreach (var c in item.DescendantBranchesMatchingType(_singleType))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingSingleTypeFromDepthRecurseAll(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsFromDepth(MinDepth, IncludeAttributes.IsPresent).Where(_singleType.IsInstanceOfType))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingTypeFromDepth(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_multiType.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            else
                foreach (var c in item.DescendantBranchesMatchingType(_multiType))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingTypeFromDepthRecurseAll(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsFromDepth(_depth, IncludeAttributes.IsPresent).Where(obj => _multiType.Any(t => t.IsInstanceOfType(obj))))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetCurrentAndDescendantsUpToDepth(MarkdownObject currentObject)
    {
        WriteObject(currentObject, false);
        foreach (var item in currentObject.DescendantsUpToDepth(MaxDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingSingleTypeUpToDepth(MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        else
            foreach (var c in currentObject.DescendantBranchesMatchingType(_singleType))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
    }

    private void GetCurrentAndDescendantsMatchingSingleTypeUpToDepthRecurseAll(MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.DescendantBranchesMatchingType(_singleType, MaxDepth))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingTypeUpToDepth(MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        else
            foreach (var c in currentObject.DescendantBranchesMatchingType(_multiType))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
    }

    private void GetCurrentAndDescendantsMatchingTypeUpToDepthRecurseAll(MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.DescendantBranchesMatchingType(_multiType, MaxDepth))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetDescendantsInDepthRange(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
            foreach (var c in item.DescendantsUpToDepth(_depth, IncludeAttributes.IsPresent))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
        }
    }

    private void GetDescendantsMatchingSingleTypeInDepthRange(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            else
                foreach (var c in item.DescendantBranchesMatchingType(_singleType, _depth))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingSingleTypeInDepthRangeRecurseAll(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            foreach (var c in item.DescendantBranchesMatchingType(_singleType, _depth))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
        }
    }

    private void GetDescendantsMatchingTypeInDepthRange(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_multiType.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            else
                foreach (var c in item.DescendantBranchesMatchingType(_multiType, _depth))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingTypeInDepthRangeRecurseAll(MarkdownObject currentObject)
    {
        foreach (var item in currentObject.DescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (Stopping)
                break;
            if (_multiType.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            foreach (var c in item.DescendantBranchesMatchingType(_multiType, _depth))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
        }
    }

    protected override void ProcessRecord()
    {
        foreach (var item in InputObject)
        {
            if (Stopping) break;
            _processInputObject(item);
        }
    }
}
