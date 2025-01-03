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

    private void WriteAttributesOnly(MarkdownObject inputObject)
    {
        var attributes = inputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
    }

    private void WriteAttributesInRange(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        foreach (var item in inputObject.DescendantsInDepthRange(_depth - 1, MaxDepth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    private void WriteAttributesAtDepth(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        foreach (var item in inputObject.DescendantsAtDepth(_depth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    private void WriteAttributesFromDepth(MarkdownObject inputObject)
    {
        Debug.Assert(_depth > 1);
        foreach (var item in inputObject.DescendantsFromDepth(_depth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    private void WriteAllAttributesRecursive(MarkdownObject inputObject)
    {
        var attributes = inputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in inputObject.Descendants())
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
    }

    private void WriteNothing(MarkdownObject inputObject) { }

    private void BeginProcessing_AttributesInRange(int minDepth, int? maxDepth)
    {
        if (maxDepth.HasValue)
            switch (maxDepth.Value)
            {
                case 0:
                    _processInputObject = WriteNothing;
                    break;
                case 1:
                    _processInputObject = WriteAttributesOnly;
                    break;
                default:
                    _depth = minDepth;
                    _processInputObject = (maxDepth.Value > minDepth) ? WriteAttributesInRange : (minDepth > 1) ? WriteAttributesAtDepth : WriteAttributesOnly;
                    break;
            }
        else if (minDepth > 1)
        {
            _depth = minDepth;
            _processInputObject = WriteAttributesFromDepth;
        }
        else
            _processInputObject = WriteAllAttributesRecursive;
    }

    protected override void BeginProcessing()
    {
        List<Type>? types;
        bool includeAttributes, hasAnyType, hasAttributesType;

        includeAttributes = IncludeAttributes.IsPresent;
        if (ParameterSetName == ParameterSetName_RecurseUnmatched || MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            hasAnyType = Type.Contains(MarkdownTokenType.Any);
            hasAttributesType = Type.Contains(MarkdownTokenType.HtmlAttributes);
            if (hasAnyType)
            {
                if (!includeAttributes)
                    includeAttributes = hasAttributesType;
                types = null;
            }
            else
            {
                if (hasAttributesType)
                {
                    includeAttributes = true;
                    if (Type.Length == 1 || (types = Type.Where(t => t != MarkdownTokenType.HtmlAttributes).ToReflectionTypes()).Count == 0)
                        types = null;
                }
                else
                    types = Type.ToReflectionTypes();
            }
        }
        else
        {
            types = null;
            hasAttributesType = hasAnyType = false;
        }
        if (ParameterSetName == ParameterSetName_ExplicitDepth)
        {
            if (types is null && hasAttributesType && !hasAnyType)
            {
                switch (Depth)
                {
                    case 0:
                        _processInputObject = WriteNothing;
                        break;
                    case 1:
                        _processInputObject = WriteAttributesOnly;
                        break;
                    default:
                        _depth = Depth;
                        _processInputObject = WriteAttributesAtDepth;
                        break;
                }
            }
            else
                BeginProcessing_ExplicitDepth(types, Depth, includeAttributes);
        }
        else if (ParameterSetName == ParameterSetName_Recurse)
        {
            if (types is null && hasAttributesType && !hasAnyType)
                _processInputObject = WriteAllAttributesRecursive;
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

            if (ParameterSetName == ParameterSetName_RecurseUnmatched)
            {
                if (hasAnyType)
                    BeginProcessing_ExplicitDepth(null, effectiveMinDepth, includeAttributes);
                else if (types is null)
                    BeginProcessing_AttributesInRange(effectiveMinDepth, maxDepth);
                else
                    BeginProcessing_RecurseUnmatched(types!, effectiveMinDepth, maxDepth, includeAttributes);
            }
            else if (types is null && hasAttributesType && !hasAnyType)
            {
                if (explicitMinDepth || maxDepth.HasValue)
                    BeginProcessing_AttributesInRange(effectiveMinDepth, maxDepth);
                else
                    _processInputObject = WriteAttributesOnly;
            }
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