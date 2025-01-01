using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Reflection.Metadata;
using Markdig.Syntax;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "MarkdownObject", DefaultParameterSetName = ParameterSetName_DepthRange)]
public partial class Select_MarkdownObject : PSCmdlet
{
    public const string ParameterSetName_DepthRange = "DepthRange";
    public const string ParameterSetName_ExplicitDepth = "ExplicitDepth";
    public const string ParameterSetName_Recurse = "Recurse";
    public const string ParameterSetName_RecurseUnmatched = "RecurseUnmatched";
    private const string HelpMessage_Type = "The type(s) of child Markdown objects to return.";
    private const string HelpMessage_MinDepth = "The inclusive minimum depth of child Markdown objects to return. This implies nested child object recursion.";
    private const string HelpMessage_MaxDepth = "The inclusive maximum depth of child Markdown objects to return. This implies nested child object recursion.";
    private const string HelpMessage_IncludeAttributes = $"This is ignored when {nameof(Type)} is specified.";

    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select child objects from.")]
    [ValidateNotNull()]
    [Alias("MarkdownObject", "Markdown")]
    public MarkdownObject[] InputObject { get; set; } = null!;

    [Parameter(Position = 1, ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_Type)]
    [Parameter(Position = 1, ParameterSetName = ParameterSetName_ExplicitDepth, HelpMessage = HelpMessage_Type)]
    [Parameter(Position = 1, ParameterSetName = ParameterSetName_Recurse, HelpMessage = HelpMessage_Type)]
    [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = HelpMessage_Type)]
    public MarkdownTokenType[] Type { get; set; } = null!;

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitDepth, HelpMessage = "The explicit depth of objects to return. This implies nested child object recursion.")]
    [ValidateRange(1, int.MaxValue)]
    public int Depth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_MinDepth)]
    [Parameter(ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = HelpMessage_MinDepth)]
    [ValidateRange(1, int.MaxValue)]
    public int MinDepth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_MaxDepth)]
    [Parameter(ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = HelpMessage_MaxDepth)]
    [ValidateRange(1, int.MaxValue)]
    public int MaxDepth { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Recurse, HelpMessage = "Recurse into nested child objects.")]
    public SwitchParameter Recurse { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = "Do not recurse into child objects of those matching the specified type(s).")]
    public SwitchParameter RecurseUnmatchedOnly { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_IncludeAttributes)]
    [Parameter(ParameterSetName = ParameterSetName_ExplicitDepth, HelpMessage = HelpMessage_IncludeAttributes)]
    [Parameter(ParameterSetName = ParameterSetName_Recurse, HelpMessage = HelpMessage_IncludeAttributes)]
    public SwitchParameter IncludeAttributes { get; set; }

    private List<Type> _multiType = null!;
    private Type _singleType = null!;
    private int _depth;
    private Action<MarkdownObject> _processInputObject = null!;
    private Func<MarkdownObject, IEnumerable<MarkdownObject>> _getDescendants = null!;

    private bool TryGetNonAttributeTypes([NotNullWhen(true)] out List<Type>? nonAttributeTypes, out bool includeAttributes)
    {
        includeAttributes = Type.Contains(MarkdownTokenType.HtmlAttributes);
        if (includeAttributes)
        {
            if (Type.Length == 1)
            {
                nonAttributeTypes = null;
                return false;
            }
            if ((nonAttributeTypes = Type.Where(t => t != MarkdownTokenType.HtmlAttributes).ToReflectionTypes()).Count == 0)
                return false;
        }
        else
        {
            nonAttributeTypes = Type.ToReflectionTypes();
            includeAttributes = IncludeAttributes.IsPresent;
        }
        return true;
    }

    protected void BeginProcessingExplicitDepth()
    {
        // MarkdownTokenType[]? Type, int Depth, IncludeAttributes?
        if (Depth == 1)
        {
            if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
            {
                if (Type.Contains(MarkdownTokenType.Any))
                {
                    if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                        _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
                    else
                        _processInputObject = WriteDirectChildrenForCurrent;
                }
                else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
                {
                    if (types.Count == 1)
                    {
                        _singleType = types[0];
                        _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesForCurrent : WriteMatchedSingleTypeForCurrent;
                    }
                    else
                    {
                        _multiType = types;
                        _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesForCurrent : WriteMatchedTypesForCurrent;
                    }
                }
                else
                    _processInputObject = WriteAttributesAtDepth;
            }
            else
                _processInputObject = IncludeAttributes.IsPresent ? WriteDirectChildrenIncludingAttributesForCurrent : WriteDirectChildrenForCurrent;
        }
        else
        {
            _depth = Depth;
            if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
            {
                if (Type.Contains(MarkdownTokenType.Any))
                {
                    if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                        _processInputObject = WriteDirectChildrenIncludingAttributesAtDepth;
                    else
                        _processInputObject = WriteDirectChildrenAtDepth;
                }
                else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
                {
                    if (types.Count == 1)
                    {
                        _singleType = types[0];
                        _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesAtDepth : WriteMatchedSingleTypeAtDepth;
                    }
                    else
                    {
                        _multiType = types;
                        _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesAtDepth : WriteMatchedTypesAtDepth;
                    }
                }
                else
                    _processInputObject = WriteAttributesAtDepth;
            }
            else
                _processInputObject = IncludeAttributes.IsPresent ? WriteDirectChildrenIncludingAttributesAtDepth : WriteDirectChildrenAtDepth;
        }
    }

    private void BeginProcessingRecurse()
    {
        // MarkdownTokenType[]? Type
        if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            if (Type.Contains(MarkdownTokenType.Any))
            {
                if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                    _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
                else
                    _processInputObject = WriteDirectChildrenForCurrent;
            }
            else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
            {
                if (types.Count == 1)
                {
                    _singleType = types[0];
                    _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesRecursive : WriteMatchedSingleTypeRecursive;
                }
                else
                {
                    _multiType = types;
                    _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesRecursive : WriteMatchedTypesRecursive;
                }
            }
            else
                _processInputObject = WriteRecursiveAttributes;
        }
        else
            _processInputObject = IncludeAttributes.IsPresent ? WriteRecursiveDescendantsIncludingAttributes : WriteRecursiveDescendants;
    }

    private void BeginProcessingRecurseUnmatched()
    {
        // MarkdownTokenType[] Type, Recurse, IncludeAttributes?
        if (Type.Contains(MarkdownTokenType.Any))
        {
            if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
            else
                _processInputObject = WriteDirectChildrenForCurrent;
        }
        else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
        {
            if (types.Count == 1)
            {
                _singleType = types[0];
                _processInputObject = includeAttributes ? WriteMatchedSingleTypeBranchesAndAttributes : WriteMatchedSingleTypeBranches;
            }
            else
            {
                _multiType = types;
                _processInputObject = includeAttributes ? WriteMatchedTypeBranchesAndAttributes : WriteMatchedTypeBranches;
            }
        }
        else
            _processInputObject = WriteRecursiveAttributes;
    }

    private void BeginProcessingDepthRange()
    {
        if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            if (Type.Contains(MarkdownTokenType.Any))
            {
                if (MinDepth > 1)
                {
                    _depth = MinDepth;
                    if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                        _processInputObject = WriteDirectChildrenIncludingAttributesAtDepth;
                    else
                        _processInputObject = WriteDirectChildrenAtDepth;
                }
                else if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                    _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
                else
                    _processInputObject = WriteDirectChildrenForCurrent;
            }
            else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
            {
                if (MinDepth > 1)
                {
                    if (MaxDepth > MinDepth)
                    {
                        if (types.Count == 1)
                        {
                            _singleType = types[0];
                            _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesInRange : WriteMatchedSingleTypeInRange;
                        }
                        else
                        {
                            _multiType = types;
                            _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesInRange : WriteMatchedTypesInRange;
                        }
                    }
                    else
                    {
                        _depth = MinDepth;
                        if (types.Count == 1)
                        {
                            _singleType = types[0];
                            _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesAtDepth : WriteMatchedSingleTypeAtDepth;
                        }
                        else
                        {
                            _multiType = types;
                            _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesAtDepth : WriteMatchedTypesAtDepth;
                        }
                    }
                }
                else if (MaxDepth > 1)
                {
                    _depth = MinDepth;
                    if (types.Count == 1)
                    {
                        _singleType = types[0];
                        _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesToDepth : WriteMatchedSingleTypeToDepth;
                    }
                    else
                    {
                        _multiType = types;
                        _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesToDepth : WriteMatchedTypesToDepth;
                    }
                }
                else if (types.Count == 1)
                {
                    _singleType = types[0];
                    _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesForCurrent : WriteMatchedSingleTypeForCurrent;
                }
                else
                {
                    _multiType = types;
                    _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesForCurrent : WriteMatchedTypesForCurrent;
                }
            }
            else if (MinDepth < MaxDepth)
            {
                if (MinDepth > 1)
                    _processInputObject = WriteAttributesInRange;
                else
                {
                    _depth = MaxDepth;
                    _processInputObject = WriteAttributesToDepth;
                }
            }
            else if (MinDepth > 1)
            {
                _depth = MinDepth;
                _processInputObject = WriteAttributesAtDepth;
            }
            else
                _processInputObject = WriteAttributesForCurrent;
        }
        else if (MinDepth < MaxDepth)
        {
            if (MinDepth > 1)
                _processInputObject = IncludeAttributes.IsPresent ? WriteDescendantsAndAttributesInRange : WriteDescendantsInRange;
            else
            {
                _depth = MaxDepth;
                _processInputObject = IncludeAttributes.IsPresent ? WriteDescendantsAndAttributesToDepth : WriteDescendantsToDepth;
            }
        }
        else if (MaxDepth > 1)
        {
            _depth = MaxDepth;
            _processInputObject = IncludeAttributes.IsPresent ? WriteDirectChildrenIncludingAttributesAtDepth : WriteDirectChildrenAtDepth;
        }
        else
            _processInputObject = IncludeAttributes.IsPresent ? WriteDirectChildrenIncludingAttributesForCurrent : WriteDirectChildrenForCurrent;
    }

    private void BeginProcessingMaxDepth()
    {
        if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            if (Type.Contains(MarkdownTokenType.Any))
            {
                if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                    _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
                else
                    _processInputObject = WriteDirectChildrenForCurrent;
            }
            else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
            {
                if (MaxDepth > 1)
                {
                    _depth = MaxDepth;
                    if (types.Count > 0)
                    {
                        _multiType = types;
                        _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesToDepth : WriteMatchedTypesToDepth;
                    }
                    else
                    {
                        _singleType = types[0];
                        _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesToDepth : WriteMatchedSingleTypeToDepth;
                    }
                }
                else if (types.Count > 0)
                {
                    _multiType = types;
                    _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesForCurrent : WriteMatchedTypesForCurrent;
                }
                else
                {
                    _singleType = types[0];
                    _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesForCurrent : WriteMatchedSingleTypeForCurrent;
                }
            }
            else if (MaxDepth > 1)
            {
                _depth = MaxDepth;
                _processInputObject = WriteAttributesAtDepth;
            }
            else
                _processInputObject = WriteAttributesForCurrent;
        }
        else if (MaxDepth > 1)
        {
            _depth = MaxDepth;
            _processInputObject = IncludeAttributes.IsPresent ? WriteDescendantsAndAttributesToDepth : WriteDescendantsToDepth;
        }
        else
            _processInputObject = IncludeAttributes.IsPresent ? WriteDirectChildrenIncludingAttributesForCurrent : WriteDirectChildrenForCurrent;
    }

    private void BeginProcessingMinRange()
    {
        if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            if (Type.Contains(MarkdownTokenType.Any))
            {
                if (MinDepth > 1)
                {
                    _depth = MinDepth;
                    if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                        _processInputObject = WriteDirectChildrenIncludingAttributesAtDepth;
                    else
                        _processInputObject = WriteDirectChildrenAtDepth;
                }
                else if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                    _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
                else
                    _processInputObject = WriteDirectChildrenForCurrent;
            }
            else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
            {
                if (MinDepth > 1)
                {
                    _depth = MinDepth;
                    if (types.Count > 1)
                    {
                        _multiType = types;
                        _processInputObject = includeAttributes ? WriteMatchedTypesAndAttributesFromDepth : WriteMatchedTypesFromDepth;
                    }
                    else
                    {
                        _singleType = types[0];
                        _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesFromDepth : WriteMatchedSingleTypeFromDepth;
                    }
                }
                else
                    _processInputObject = IncludeAttributes.IsPresent ? WriteRecursiveDescendantsIncludingAttributes : WriteRecursiveDescendants;
            }
            else if (MinDepth > 1)
            {
                _depth = MinDepth;
                _processInputObject =  WriteAttributesFromDepth;
            }
            else
                _processInputObject =  WriteRecursiveAttributes;
        }
        else if (MinDepth > 1)
        {
            _depth = MinDepth;
            _processInputObject = IncludeAttributes.IsPresent ? WriteDescendantsAndAttributesFromDepth : WriteDescendantsFromDepth;
        }
        else
            _processInputObject = IncludeAttributes.IsPresent ? WriteRecursiveDescendantsIncludingAttributes : WriteRecursiveDescendants;
    }

    private void BeginProcessingNoRecurse()
    {
        if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            if (Type.Contains(MarkdownTokenType.Any))
            {
                if (IncludeAttributes.IsPresent || (Type.Length > 1 && Type.Contains(MarkdownTokenType.HtmlAttributes)))
                    _processInputObject = WriteDirectChildrenIncludingAttributesForCurrent;
                else
                    _processInputObject = WriteDirectChildrenForCurrent;
            }
            else if (TryGetNonAttributeTypes(out List<Type>? types, out bool includeAttributes))
            {
                if (types.Count > 0)
                {
                    _multiType = types;
                    _processInputObject = includeAttributes ? WriteDirectChildrenMatchedTypesAndAttributesForCurrent : WriteDirectChildrenMatchedTypesForCurrent;
                }
                else
                {
                    _singleType = types[0];
                    _processInputObject = includeAttributes ? WriteMatchedSingleTypeAndAttributesForCurrent : WriteMatchedSingleTypeForCurrent;
                }
            }
        }
        else
            _processInputObject = IncludeAttributes.IsPresent ? WriteDirectChildrenIncludingAttributesForCurrent : WriteDirectChildrenForCurrent;
    }

    protected override void BeginProcessing()
    {   
        switch (ParameterSetName)
        {
            case ParameterSetName_ExplicitDepth:
                BeginProcessingExplicitDepth();
                break;
            case ParameterSetName_Recurse:
                BeginProcessingRecurse();
                break;
            case ParameterSetName_RecurseUnmatched:
                BeginProcessingRecurseUnmatched();
                break;
            default: // ParameterSetName_DepthRange
            // MarkdownTokenType[]? Type, int? MinDepth, int? MaxDepth, IncludeAttributes?
            if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
            {
                if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
                {
                    if (MinDepth > MaxDepth)
                    {
                        WriteError(new(new ArgumentOutOfRangeException(nameof(MaxDepth), $"{nameof(MaxDepth)} cannot be less than {nameof(MinDepth)}"), "MaxDepthLessThanMinDepth", ErrorCategory.InvalidArgument, MaxDepth));
                        throw new PipelineStoppedException();
                    }
                    BeginProcessingDepthRange();
                }
                else
                    BeginProcessingMaxDepth();
            }
            else if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
                BeginProcessingMinRange();
            else
                BeginProcessingNoRecurse();
            break;
        }
    }

    private void WriteDirectChildrenIncludingAttributesForCurrent(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
                        
    private void WriteDirectChildrenForCurrent(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
                        
    private void WriteMatchedSingleTypeAndAttributesForCurrent(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
                        
    private void WriteMatchedSingleTypeForCurrent(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
                        
    private void WriteMatchedTypesAndAttributesForCurrent(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
                        
    private void WriteMatchedTypesForCurrent(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth
    private void WriteDirectChildrenIncludingAttributesAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth
    private void WriteDirectChildrenAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth, _singleType
    private void WriteMatchedSingleTypeAndAttributesAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth, _singleType
    private void WriteMatchedSingleTypeAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth, _multiType
    private void WriteMatchedTypesAndAttributesAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth, _multiType
    private void WriteMatchedTypesAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _depth
    private void WriteAttributesAtDepth(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _singleType
    private void WriteMatchedSingleTypeAndAttributesRecursive(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _singleType
    private void WriteMatchedSingleTypeRecursive(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _multiType
    private void WriteMatchedTypesAndAttributesRecursive(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _multiType
    private void WriteMatchedTypesRecursive(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    private void WriteRecursiveAttributes(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    private void WriteRecursiveDescendantsIncludingAttributes(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    private void WriteRecursiveDescendants(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }

    // _singleType
    private void WriteMatchedSingleTypeBranchesAndAttributes(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
    
    // _singleType
    private void WriteMatchedSingleTypeBranches(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
    
    // _multiType
    private void WriteMatchedTypeBranchesAndAttributes(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
    
    // _multiType
    private void WriteMatchedTypeBranches(MarkdownObject markdownObject)
    {
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth
    private void WriteAttributesInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth, _singleType
    private void WriteMatchedSingleTypeAndAttributesInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth, _singleType
    private void WriteMatchedSingleTypeInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth, _multiType
    private void WriteMatchedTypesAndAttributesInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth, _multiType
    private void WriteMatchedTypesInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _singleType
    private void WriteMatchedSingleTypeAndAttributesToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _singleType
    private void WriteMatchedSingleTypeToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _multiType
    private void WriteMatchedTypesAndAttributesToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _multiType
    private void WriteMatchedTypesToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth
    private void WriteAttributesToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    private void WriteAttributesForCurrent(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth
    private void WriteDescendantsAndAttributesInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // MinDepth, MaxDepth
    private void WriteDescendantsInRange(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth
    private void WriteDescendantsAndAttributesToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth
    private void WriteDescendantsToDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _multiType
    private void WriteMatchedTypesAndAttributesFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _multiType
    private void WriteMatchedTypesFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _singleType
    private void WriteMatchedSingleTypeAndAttributesFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth, _singleType
    private void WriteMatchedSingleTypeFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth
    private void WriteAttributesFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth
    private void WriteDescendantsAndAttributesFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _depth
    private void WriteDescendantsFromDepth(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _multiType
    private void WriteDirectChildrenMatchedTypesAndAttributesForCurrent(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
    }
    
    // _singleType
    private void WriteDirectChildrenMatchedTypesForCurrent(MarkdownObject markdownObject)
    {
        
        throw new NotImplementedException();
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
