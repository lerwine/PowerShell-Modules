using System.Diagnostics;
using System.Management.Automation;
using Markdig.Renderers.Html;
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
    private List<Type> _multiTypes = null!;
    private Type _singleType = null!;
    private int _depth;
    private Action<MarkdownObject> _processInputObject = null!;

    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select child objects from.")]
    [ValidateNotNull()]
    [Alias("MarkdownObject", "Markdown")]
    public MarkdownObject InputObject { get; set; } = null!;

    [Parameter(Position = 1, ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_Type)]
    [Parameter(Position = 1, ParameterSetName = ParameterSetName_ExplicitDepth, HelpMessage = HelpMessage_Type)]
    [Parameter(Position = 1, ParameterSetName = ParameterSetName_Recurse, HelpMessage = HelpMessage_Type)]
    [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = HelpMessage_Type)]
    public MarkdownTokenType[] Type { get; set; } = null!;

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitDepth, HelpMessage = "The explicit depth of objects to return. This implies nested child object recursion.")]
    [ValidateRange(0, int.MaxValue)]
    public int Depth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_MinDepth)]
    [Parameter(ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = HelpMessage_MinDepth)]
    [ValidateRange(0, int.MaxValue)]
    public int MinDepth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_MaxDepth)]
    [Parameter(ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = HelpMessage_MaxDepth)]
    [ValidateRange(0, int.MaxValue)]
    public int MaxDepth { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Recurse, HelpMessage = "Recurse into nested child objects.")]
    public SwitchParameter Recurse { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_RecurseUnmatched, HelpMessage = "Do not recurse into child objects of those matching the specified type(s).")]
    public SwitchParameter RecurseUnmatchedOnly { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange, HelpMessage = HelpMessage_IncludeAttributes)]
    [Parameter(ParameterSetName = ParameterSetName_ExplicitDepth, HelpMessage = HelpMessage_IncludeAttributes)]
    [Parameter(ParameterSetName = ParameterSetName_Recurse, HelpMessage = HelpMessage_IncludeAttributes)]
    public SwitchParameter IncludeAttributes { get; set; }
    
    protected override void BeginProcessing()
    {
        List<Type>? types;
        bool hasAnyType, hasAttributesType;

        if (ParameterSetName == ParameterSetName_RecurseUnmatched || MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            hasAnyType = Type.Contains(MarkdownTokenType.Any);
            hasAttributesType = Type.Contains(MarkdownTokenType.HtmlAttributes);
            if (hasAnyType)
                types = null;
            else if (hasAttributesType)
            {
                if (Type.Length == 1 || (types = Type.Where(t => t != MarkdownTokenType.HtmlAttributes).ToReflectionTypes()).Count == 0)
                    types = null;
            }
            else
                types = Type.ToReflectionTypes();
        }
        else
        {
            types = null;
            hasAttributesType = hasAnyType = false;
        }
        bool includeAttributes = hasAttributesType || IncludeAttributes.IsPresent;
        if (ParameterSetName == ParameterSetName_ExplicitDepth)
        {
            if (hasAttributesType && !hasAnyType)
                switch (Depth)
                {
                    case 0:
                        _processInputObject = AttributesInputObj;
                        break;
                    case 1:
                        _processInputObject = AttributesDirectDesc;
                        break;
                    default:
                        _depth = Depth;
                        _processInputObject = AttributesAtDepth;
                        break;
                }
            else
                BeginProcessing_ExplicitDepth(types, Depth, includeAttributes);
        }
        else if (ParameterSetName == ParameterSetName_Recurse)
        {
            if (hasAttributesType && !hasAnyType)
                _processInputObject = Attributes;
            else
                BeginProcessing_Recurse(types, includeAttributes);
        }
        else
        {
            int effectiveMinDepth;
            int? maxDepth;
            bool explicitMinDepth = MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth));
            if (explicitMinDepth)
            {
                effectiveMinDepth = MinDepth;
                if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
                {
                    if (MinDepth > MaxDepth)
                    {
                        WriteError(new(new ArgumentOutOfRangeException(nameof(MaxDepth), $"{nameof(MaxDepth)} cannot be less than {nameof(MinDepth)}"), "MaxDepthLessThanMinDepth", ErrorCategory.InvalidArgument, MaxDepth));
                        throw new PipelineStoppedException();
                    }
                    maxDepth = MaxDepth;
                }
                else
                    maxDepth = null;
            }
            else if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
            {
                maxDepth = MaxDepth;
                effectiveMinDepth = (MaxDepth > 0) ? 1 : 0;
            }
            else
            {
                effectiveMinDepth = 1;
                maxDepth = null;
            }
            
            if (hasAttributesType && !hasAnyType)
            {
                if (maxDepth.HasValue)
                {
                    if (effectiveMinDepth == MaxDepth)
                        switch (effectiveMinDepth)
                        {
                            case 0:
                                _processInputObject = AttributesInputObj;
                                break;
                            case 1:
                                _processInputObject = AttributesDirectDesc;
                                break;
                            default:
                                _depth = effectiveMinDepth;
                                _processInputObject = AttributesAtDepth;
                                break;
                        }
                    else
                        switch (effectiveMinDepth)
                        {
                            case 0:
                                _depth = MaxDepth;
                                _processInputObject = (MaxDepth > 1) ? AttributesToDepthInclInputObj : AttributesInputObjAndDirectDesc;
                                break;
                            case 1:
                                _depth = MaxDepth;
                                _processInputObject = AttributesToDepth;
                                break;
                            default:
                                _depth = effectiveMinDepth;
                                _processInputObject = AttributesInRange;
                                break;
                        }
                }
                else
                    switch (effectiveMinDepth)
                    {
                        case 0:
                            _processInputObject = AttributesInputObjAndAllDesc;
                            break;
                        case 1:
                            _processInputObject = Attributes;
                            break;
                        default:
                            _depth = effectiveMinDepth;
                            _processInputObject = AttributesFromDepth;
                            break;
                    }
            }
            else if (ParameterSetName == ParameterSetName_RecurseUnmatched)
                BeginProcessing_RecurseUnmatched(types!, effectiveMinDepth, maxDepth, includeAttributes);
            else if (maxDepth.HasValue)
                BeginProcessing_DepthRange(types, effectiveMinDepth, maxDepth.Value, includeAttributes);
            else if (explicitMinDepth)
                BeginProcessing_DepthRange(types, effectiveMinDepth, includeAttributes);
            else
                BeginProcessing_NoRecursion(types, includeAttributes);
        }
    }

    protected override void ProcessRecord()
    {
        Debug.Assert(_processInputObject is not null);
        if (!Stopping)
            _processInputObject(InputObject);
    }
}