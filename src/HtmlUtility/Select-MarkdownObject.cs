using System.Management.Automation;

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
    public Markdig.Syntax.MarkdownObject[] InputObject { get; set; } = null!;

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
    private Action<Markdig.Syntax.MarkdownObject> _processInputObject = null!;

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

    private void GetCurrentAndDescendants(Markdig.Syntax.MarkdownObject currentObject)
    {
        WriteObject(currentObject, false);
        foreach (var c in currentObject.GetAllDescendants(IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingSingleTypeRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.GetAllDescendants(IncludeAttributes.IsPresent).Where(_singleType.IsInstanceOfType))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingSingleType(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.GetDescendantBranchesMatchingType(_singleType))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentMatchingSingleType(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
    }

    private void GetCurrentAndDescendantsMatchingTypeRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.GetAllDescendants(IncludeAttributes.IsPresent).Where(obj => _multiType.Any(t => t.IsInstanceOfType(obj))))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingType(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.GetDescendantBranchesMatchingType(_multiType))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentMatchingType(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
    }

    private void GetDescendantsAtDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(_depth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingSingleTypeAtDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(_depth, IncludeAttributes.IsPresent).Where(_singleType.IsInstanceOfType))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingTypeAtDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(_depth, IncludeAttributes.IsPresent).Where(obj => _multiType.Any(t => t.IsInstanceOfType(obj))))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsFromDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsFromDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingSingleTypeFromDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            else
                foreach (var c in item.GetDescendantBranchesMatchingType(_singleType))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingSingleTypeFromDepthRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsFromDepth(MinDepth, IncludeAttributes.IsPresent).Where(_singleType.IsInstanceOfType))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetDescendantsMatchingTypeFromDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_multiType.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            else
                foreach (var c in item.GetDescendantBranchesMatchingType(_multiType))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingTypeFromDepthRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsFromDepth(_depth, IncludeAttributes.IsPresent).Where(obj => _multiType.Any(t => t.IsInstanceOfType(obj))))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetCurrentAndDescendantsUpToDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        WriteObject(currentObject, false);
        foreach (var item in currentObject.GetDescendantsUpToDepth(MaxDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingSingleTypeUpToDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        else
            foreach (var c in currentObject.GetDescendantBranchesMatchingType(_singleType))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
    }

    private void GetCurrentAndDescendantsMatchingSingleTypeUpToDepthRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_singleType.IsInstanceOfType(currentObject))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.GetDescendantBranchesMatchingType(_singleType, MaxDepth))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetCurrentAndDescendantsMatchingTypeUpToDepth(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        else
            foreach (var c in currentObject.GetDescendantBranchesMatchingType(_multiType))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
    }

    private void GetCurrentAndDescendantsMatchingTypeUpToDepthRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        if (_multiType.Any(t => t.IsInstanceOfType(currentObject)))
            WriteObject(currentObject, false);
        foreach (var c in currentObject.GetDescendantBranchesMatchingType(_multiType, MaxDepth))
        {
            if (Stopping) return;
            WriteObject(c, false);
        }
    }

    private void GetDescendantsInDepthRange(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            WriteObject(item, false);
            foreach (var c in item.GetDescendantsUpToDepth(_depth, IncludeAttributes.IsPresent))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
        }
    }

    private void GetDescendantsMatchingSingleTypeInDepthRange(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            else
                foreach (var c in item.GetDescendantBranchesMatchingType(_singleType, _depth))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingSingleTypeInDepthRangeRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            foreach (var c in item.GetDescendantBranchesMatchingType(_singleType, _depth))
            {
                if (Stopping) return;
                WriteObject(c, false);
            }
        }
    }

    private void GetDescendantsMatchingTypeInDepthRange(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (_multiType.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            else
                foreach (var c in item.GetDescendantBranchesMatchingType(_multiType, _depth))
                {
                    if (Stopping) return;
                    WriteObject(c, false);
                }
        }
    }

    private void GetDescendantsMatchingTypeInDepthRangeRecurseAll(Markdig.Syntax.MarkdownObject currentObject)
    {
        foreach (var item in currentObject.GetDescendantsAtDepth(MinDepth, IncludeAttributes.IsPresent))
        {
            if (Stopping) return;
            if (Stopping)
                break;
            if (_multiType.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            foreach (var c in item.GetDescendantBranchesMatchingType(_multiType, _depth))
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
