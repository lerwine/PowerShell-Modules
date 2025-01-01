using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "MarkdownObject", DefaultParameterSetName = ParameterSetName_DepthRange)]
public class Select_MarkdownObject : PSCmdlet
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
    private Action _processRecord = null!;

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

    private void Process_ExplicitDepth0()
    {
        // ExplicitDepth: -Depth 0
        // ExplicitDepth: -Depth 0 -IncludeAttributes
        // ExplicitDepth: -Type HtmlAttributes -Depth 0
        // ExplicitDepth: -Type Any, HtmlAttributes -Depth 0
        // ExplicitDepth: -Type Any -Depth 0 -IncludeAttributes
        // ExplicitDepth: -Type Any -Depth 0
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Any, HtmlAttributes
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Any
        // RecurseUnmatched: -Type Any, HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any, HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any, HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any -MinDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any, HtmlAttributes -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any -MinDepth 0 -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any, HtmlAttributes -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any -MinDepth 0 -MaxDepth 1 -RecurseUnmatchedOnly
        WriteObject(InputObject, false);
    }

    private void Process_ExplicitDepthWithAttributes1()
    {
        // ExplicitDepth: -Depth 1 -IncludeAttributes
        // ExplicitDepth: -Depth 1 -Type HtmlAttributes
        // ExplicitDepth: -Depth 1 -Type Any, HtmlAttributes
        // ExplicitDepth: -Depth 1 -IncludeAttributes -Type Any
        // RecurseUnmatched: -Type Any, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Any, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly -MaxDepth 1
        // RecurseUnmatched: -Type Any, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly -MaxDepth 2
        if (InputObject.HasDirectDescendantIncludingAttributes(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var obj in directDescendants)
                WriteObject(obj, false);
    }

    private void Process_ExplicitDepthNoAttributes1()
    {
        // ExplicitDepth: -Depth 1
        // ExplicitDepth: -Type Any -Depth 1
        // RecurseUnmatched: -MinDepth 1 -Type Any -RecurseUnmatchedOnly
        // RecurseUnmatched: -MinDepth 1 -MaxDepth 1 -Type Any -RecurseUnmatchedOnly
        // RecurseUnmatched: -MinDepth 1 -MaxDepth 2 -Type Any -RecurseUnmatchedOnly
        if (InputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var obj in directDescendants)
                WriteObject(obj, false);
    }

    private void Process_ExplicitDepthWithAttributesN()
    {
        Debug.Assert(_depth > 1);
        // ExplicitDepth: -Depth 2 -IncludeAttributes
        // ExplicitDepth: -Depth 2 -Type HtmlAttributes
        // ExplicitDepth: -Depth 2 -Type Any, HtmlAttributes
        // ExplicitDepth: -Type Any -Depth 2 -IncludeAttributes
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Any, HtmlAttributes -MinDepth 2
        // RecurseUnmatched: -MaxDepth 2 -RecurseUnmatchedOnly -Type Any, HtmlAttributes -MinDepth 2
        // RecurseUnmatched: -MaxDepth 3 -RecurseUnmatchedOnly -Type Any, HtmlAttributes -MinDepth 2
        foreach (var obj in InputObject.GetDescendantsAtDepthIncludingAttributes(_depth))
            WriteObject(obj, false);
    }

    private void Process_ExplicitDepthNoAttributesN()
    {
        Debug.Assert(_depth > 1);
        // ExplicitDepth: -Depth 2
        // ExplicitDepth: -Depth 2 -Type Any
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Any -MinDepth 2
        // RecurseUnmatched: -RecurseUnmatchedOnly -MaxDepth 2 -Type Any -MinDepth 2
        // RecurseUnmatched: -RecurseUnmatchedOnly -MaxDepth 3 -Type Any -MinDepth 2
        foreach (var obj in InputObject.GetDescendantsAtDepth(_depth))
            WriteObject(obj, false);
    }

    private void Process_ExplicitDepthMatchedTypes0()
    {
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: -Type Block, Inline, HtmlAttributes -Depth 0
        // ExplicitDepth: -IncludeAttributes -Type Block, Inline -Depth 0
        // ExplicitDepth: -Type Block, Inline -Depth 0
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly -MinDepth 0
        if (_multiTypes.Any(t => t.IsInstanceOfType(InputObject)))
            WriteObject(InputObject, false);
    }

    private void Process_ExplicitDepthMatchedTypesWithAttributes1()
    {
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: -Depth 1 -Type Block, Inline, HtmlAttributes
        // ExplicitDepth: -Depth 1 -Type Block, Inline -IncludeAttributes
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly -MinDepth 1
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        if (InputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var item in directDescendants.Where(obj => _multiTypes.Any(t => t.IsInstanceOfType(obj))))
                WriteObject(item, false);
    }

    private void Process_ExplicitDepthMatchedTypesNoAttributes1()
    {
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: -Depth 1 -Type Block, Inline
        if (InputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var item in directDescendants.Where(obj => _multiTypes.Any(t => t.IsInstanceOfType(obj))))
                WriteObject(item, false);
    }

    private void Process_ExplicitDepthMatchedTypesWithAttributesN()
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: -Depth 2 -Type Block, Inline, HtmlAttributes
        // ExplicitDepth: -Type Block, Inline -IncludeAttributes -Depth 2
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly -MinDepth 2
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            if (item.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
                foreach (var obj in directDescendants.Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
                    WriteObject(obj, false);
        }
    }

    private void Process_ExplicitDepthMatchedTypesNoAttributesN()
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // ExplicitDepth: -Type Block, Inline -Depth 2
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth).Where(a => _multiTypes.Any(t => t.IsInstanceOfType(a))))
            WriteObject(item, false);
    }

    private void Process_ExplicitDepthMatchedSingleType0()
    {
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: -Type Block, HtmlAttributes -Depth 0
        // ExplicitDepth: -IncludeAttributes -Depth 0 -Type Block
        // ExplicitDepth: -Type Block -Depth 0
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block, HtmlAttributes -MaxDepth 0
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 0 -Type Block, HtmlAttributes -MaxDepth 0
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block -MaxDepth 0
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 0 -Type Block -MaxDepth 0
        if (_singleType.IsInstanceOfType(InputObject))
            WriteObject(InputObject, false);
    }

    private void Process_ExplicitDepthMatchedSingleTypeWithAttributes1()
    {
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: -Type Block, HtmlAttributes -Depth 1
        // ExplicitDepth: -Depth 1 -Type Block -IncludeAttributes
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block, HtmlAttributes -MaxDepth 1
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 1 -Type Block, HtmlAttributes -MaxDepth 1
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block -MaxDepth 1
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 1 -Type Block -MaxDepth 1
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        if (InputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var item in directDescendants.Where(_singleType.IsInstanceOfType))
                WriteObject(item, false);
    }

    private void Process_ExplicitDepthMatchedSingleTypeNoAttributes1()
    {
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: -Type Block -Depth 1
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 2 -Type Block -MaxDepth 2
        if (InputObject.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
            foreach (var item in directDescendants.Where(_singleType.IsInstanceOfType))
                WriteObject(item, false);
    }

    private void Process_ExplicitDepthMatchedSingleTypeWithAttributesN()
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: -Type Block, HtmlAttributes -Depth 2
        // ExplicitDepth: -IncludeAttributes -Type Block -Depth 2
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 2 -Type Block, HtmlAttributes -MaxDepth 2
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
            if (item.HasDirectDescendant(out IEnumerable<MarkdownObject>? directDescendants))
                foreach (var obj in directDescendants.Where(_singleType.IsInstanceOfType))
                    WriteObject(obj, false);
        }
    }

    private void Process_ExplicitDepthMatchedSingleTypeNoAttributesN()
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(_singleType is not null);
        // ExplicitDepth: -Depth 2 -Type Block, Inline
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth).Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    private void Process_RecurseWithAttributes1()
    {
        // Recurse: -Recurse -IncludeAttributes
        // Recurse: -Recurse -Type HtmlAttributes
        // Recurse: -Recurse -Type Any, HtmlAttributes
        // Recurse: -Type Any -Recurse -IncludeAttributes
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            WriteObject(item, false);
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_RecurseNoAttributes1()
    {
        // Recurse: -Recurse
        // Recurse: -Recurse -Type Any
        foreach (var item in InputObject.Descendants())
            WriteObject(item, false);
    }

    private void Process_RecurseMatchedTypesWithAttributes1()
    {
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // Recurse: -Type Block, Inline, HtmlAttributes -Recurse
        // Recurse: -IncludeAttributes -Recurse -Type Block, Inline
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            if (_multiTypes.Any(t => t.IsInstanceOfType(item)))
                WriteObject(item, false);
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_RecurseMatchedTypesNoAttributes1()
    {
        Debug.Assert(_multiTypes is not null);
        Debug.Assert(_multiTypes.Count > 1);
        // Recurse: -Type Block, Inline -Recurse
        foreach (var item in InputObject.Descendants().Where(obj => _multiTypes.Any(t => t.IsInstanceOfType(obj))))
            WriteObject(item, false);
    }

    private void Process_RecurseMatchedSingleTypeWithAttributes1()
    {
        Debug.Assert(_singleType is not null);
        // Recurse: -Recurse -Type Block, HtmlAttributes
        // Recurse: -IncludeAttributes -Type Block -Recurse
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            if (_singleType.IsInstanceOfType(item))
                WriteObject(item, false);
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_RecurseMatchedSingleTypeNoAttributes1()
    {
        Debug.Assert(_singleType is not null);
        // Recurse: -Recurse -Type Block
        foreach (var item in InputObject.Descendants().Where(_singleType.IsInstanceOfType))
            WriteObject(item, false);
    }

    private static void Process_SkipAll()
    {
        // RecurseUnmatched: -Type HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type HtmlAttributes -MaxDepth 0 -RecurseUnmatchedOnly -MinDepth 0
    }

    private void Process_Attributes1()
    {
        // RecurseUnmatched: -Type HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type HtmlAttributes -MaxDepth 1 -RecurseUnmatchedOnly -MinDepth 1
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 0 -MaxDepth 1 -Type HtmlAttributes
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
    }

    private void Process_AttributesToDepth()
    {
        Debug.Assert(MaxDepth > 1);
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 1 -MaxDepth 2 -Type HtmlAttributes
        // RecurseUnmatched: -RecurseUnmatchedOnly -MaxDepth 2 -Type HtmlAttributes
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.GetDescendantsToDepth(MaxDepth - 1))
        {
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_Attributes_Recurse()
    {
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type HtmlAttributes
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 0 -Type HtmlAttributes
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 1 -Type HtmlAttributes
        var attributes = InputObject.TryGetAttributes();
        if (attributes is not null)
            WriteObject(attributes, false);
        foreach (var item in InputObject.Descendants())
        {
            if ((attributes = item.TryGetAttributes()) is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_AttributesInRange()
    {
        Debug.Assert(_depth > 1);
        Debug.Assert(MaxDepth > _depth);
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 2 -MaxDepth 3 -Type HtmlAttributes
        foreach (var item in InputObject.GetDescendantsInDepthRange(_depth - 1, MaxDepth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_AttributesAtDepth()
    {
        Debug.Assert(_depth > 1);
        // RecurseUnmatched: -Type HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly -MinDepth 2
        // RecurseUnmatched: -RecurseUnmatchedOnly -MinDepth 2 -Type HtmlAttributes
        foreach (var item in InputObject.GetDescendantsAtDepth(_depth - 1))
        {
            var attributes = item.TryGetAttributes();
            if (attributes is not null)
                WriteObject(attributes, false);
        }
    }

    private void Process_RecurseUnmatchedMatchedTypesWithAttributes0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypes0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesWithAttributes1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block, Inline, HtmlAttributes
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypes1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block, Inline
        // RecurseUnmatched: -Type Block, Inline -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesWithAttributesN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesWithAttributesMaxDepth0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 1 -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesMaxDepth0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline -MaxDepth 1 -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesWithAttributesMaxDepth1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 2 -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesMaxDepth1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Block, Inline -MaxDepth 2 -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesWithAttributesMaxDepthN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline, HtmlAttributes -MaxDepth 3 -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedTypesMaxDepthN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, Inline -MaxDepth 3 -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeWithAttributes0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, HtmlAttributes -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleType0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeWithAttributes1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block, HtmlAttributes
        // RecurseUnmatched: -Type Block, HtmlAttributes -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleType1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -RecurseUnmatchedOnly -Type Block
        // RecurseUnmatched: -Type Block -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeWithAttributesN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, HtmlAttributes -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block -MinDepth 2 -RecurseUnmatchedOnly
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeWithAttributesMaxDepth0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, HtmlAttributes -MaxDepth 1 -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeMaxDepth0()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block -MaxDepth 1 -MinDepth 0 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeWithAttributesMaxDepth1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, HtmlAttributes -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Block, HtmlAttributes -MaxDepth 2 -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeMaxDepth1()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block -MaxDepth 2 -RecurseUnmatchedOnly
        // RecurseUnmatched: -Type Block -MaxDepth 2 -MinDepth 1 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeWithAttributesMaxDepthN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block, HtmlAttributes -MaxDepth 3 -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void Process_RecurseUnmatchedMatchedSingleTypeMaxDepthN()
    {
        // TODO: Implement input processor
        // RecurseUnmatched: -Type Block -MaxDepth 3 -MinDepth 2 -RecurseUnmatchedOnly
        throw new NotImplementedException();
    }

    private void BeginProcessing_ExplicitDepth(List<Type>? types, bool includeAttributes, int depth)
    {
        if (types is null)
        {
            switch (depth)
            {
                case 0:
                    _processRecord = Process_ExplicitDepth0;
                    break;
                case 1:
                    _processRecord = includeAttributes ? Process_ExplicitDepthWithAttributes1 : Process_ExplicitDepthNoAttributes1;
                    break;
                default:
                    _depth = depth;
                    _processRecord = includeAttributes ? Process_ExplicitDepthWithAttributesN : Process_ExplicitDepthNoAttributesN;
                    break;
            }
        }
        else if (types.Count > 1)
        {
            _multiTypes = types;
            switch (depth)
            {
                case 0:
                    _processRecord = Process_ExplicitDepthMatchedTypes0;
                    break;
                case 1:
                    _processRecord = includeAttributes ? Process_ExplicitDepthMatchedTypesWithAttributes1 : Process_ExplicitDepthMatchedTypesNoAttributes1;
                    break;
                default:
                    _depth = depth;
                    _processRecord = includeAttributes ? Process_ExplicitDepthMatchedTypesWithAttributesN : Process_ExplicitDepthMatchedTypesNoAttributesN;
                    break;
            }
        }
        else
        {
            _singleType = types[0];
            switch (depth)
            {
                case 0:
                    _processRecord = Process_ExplicitDepthMatchedSingleType0;
                    break;
                case 1:
                    _processRecord = includeAttributes ? Process_ExplicitDepthMatchedSingleTypeWithAttributes1 : Process_ExplicitDepthMatchedSingleTypeNoAttributes1;
                    break;
                default:
                    _depth = depth;
                    _processRecord = includeAttributes ? Process_ExplicitDepthMatchedSingleTypeWithAttributesN : Process_ExplicitDepthMatchedSingleTypeNoAttributesN;
                    break;
            }
        }
    }

    private void BeginProcessing_Recurse(List<Type>? types, bool includeAttributes)
    {
        if (types is null)
            _processRecord = includeAttributes ? Process_RecurseWithAttributes1 : Process_RecurseNoAttributes1;
        else if (types.Count > 1)
        {
            _multiTypes = types;
            _processRecord = includeAttributes ? Process_RecurseMatchedTypesWithAttributes1 : Process_RecurseMatchedTypesNoAttributes1;
        }
        else
        {
            _singleType = types[0];
            _processRecord = includeAttributes ? Process_RecurseMatchedSingleTypeWithAttributes1 : Process_RecurseMatchedSingleTypeNoAttributes1;
        }
    }

    private void BeginProcessing_AttributesInRange(int minDepth, int? maxDepth)
    {
        if (minDepth < 2)
        {
            if (maxDepth.HasValue)
                _processRecord = (maxDepth.Value < 1) ? Process_SkipAll : (maxDepth.Value > 1) ? Process_AttributesToDepth : Process_Attributes1;
            else
                _processRecord = Process_Attributes_Recurse;
        }
        else
        {
            _depth = minDepth;
            _processRecord = (maxDepth.HasValue && maxDepth.Value > minDepth) ? Process_AttributesInRange : Process_AttributesAtDepth;
        }
    }

    private void BeginProcessing_RecurseUnmatched(List<Type>? types, bool includeAttributes, int effectiveMinDepth, int? effectiveMaxDepth)
    {
        if (types is null)
        {
            Debug.Assert(includeAttributes);
            BeginProcessing_AttributesInRange(effectiveMinDepth, effectiveMaxDepth);
        }
        else if (types.Count > 1)
        {
            _multiTypes = types;
            if (effectiveMaxDepth.HasValue)
                switch (effectiveMinDepth)
                {
                    case 0:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedTypesWithAttributesMaxDepth0 : Process_RecurseUnmatchedMatchedTypesMaxDepth0;
                        break;
                    case 1:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedTypesWithAttributesMaxDepth1 : Process_RecurseUnmatchedMatchedTypesMaxDepth1;
                        break;
                    default:
                        _depth = effectiveMinDepth;
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedTypesWithAttributesMaxDepthN : Process_RecurseUnmatchedMatchedTypesMaxDepthN;
                        break;
                }
            else
                switch (effectiveMinDepth)
                {
                    case 0:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedTypesWithAttributes0 : Process_RecurseUnmatchedMatchedTypes0;
                        break;
                    case 1:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedTypesWithAttributes1 : Process_RecurseUnmatchedMatchedTypes1;
                        break;
                    default:
                        _depth = effectiveMinDepth;
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedTypesWithAttributesN : Process_RecurseUnmatchedMatchedTypesN;
                        break;
                }
        }
        else
        {
            _singleType = types[0];
            if (effectiveMaxDepth.HasValue)
                switch (effectiveMinDepth)
                {
                    case 0:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedSingleTypeWithAttributesMaxDepth0 : Process_RecurseUnmatchedMatchedSingleTypeMaxDepth0;
                        break;
                    case 1:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedSingleTypeWithAttributesMaxDepth1 : Process_RecurseUnmatchedMatchedSingleTypeMaxDepth1;
                        break;
                    default:
                        _depth = effectiveMinDepth;
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedSingleTypeWithAttributesMaxDepthN : Process_RecurseUnmatchedMatchedSingleTypeMaxDepthN;
                        break;
                }
            else
                switch (effectiveMinDepth)
                {
                    case 0:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedSingleTypeWithAttributes0 : Process_RecurseUnmatchedMatchedSingleType0;
                        break;
                    case 1:
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedSingleTypeWithAttributes1 : Process_RecurseUnmatchedMatchedSingleType1;
                        break;
                    default:
                        _depth = effectiveMinDepth;
                        _processRecord = includeAttributes ? Process_RecurseUnmatchedMatchedSingleTypeWithAttributesN : Process_RecurseUnmatchedMatchedSingleTypeN;
                        break;
                }
        }
    }

    private void BeginProcessing_DepthRange(List<Type>? types, bool includeAttributes, int effectiveMinDepth, int? effectiveMaxDepth)
    {
        // TODO: Implement process initializer
        throw new NotImplementedException();
    }

    private void BeginProcessing_NonRecursive(List<Type>? types, bool includeAttributes, int depth)
    {
        // TODO: Implement process initializer
        throw new NotImplementedException();
    }

    private bool GetEffectiveDepthRange(out int effectiveMinDepth, [NotNullWhen(true)] out int? effectiveMaxDepth)
    {
        if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
        {
            effectiveMinDepth = MinDepth;
            if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
            {
                if (MinDepth > MaxDepth)
                {
                    WriteError(new(new ArgumentOutOfRangeException(nameof(MaxDepth), $"{nameof(MaxDepth)} cannot be less than {nameof(MinDepth)}"), "MaxDepthLessThanMinDepth", ErrorCategory.InvalidArgument, MaxDepth));
                    throw new PipelineStoppedException();
                }
                effectiveMaxDepth = MaxDepth;
                return MaxDepth == effectiveMinDepth;
            }
            effectiveMaxDepth = null;
            return false;
        }
        if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
        {
            effectiveMaxDepth = MaxDepth;
            if (MaxDepth == 0)
            {
                effectiveMinDepth = 0;
                return true;
            }
            effectiveMinDepth = 1;
            return MaxDepth == 1;
        }
        effectiveMinDepth = 1;
        effectiveMaxDepth = null;
        return false;
    }

    protected override void BeginProcessing()
    {
        List<Type>? types;
        bool includeAttributes, hasAnyType;

        includeAttributes = IncludeAttributes.IsPresent;
        if (MyInvocation.BoundParameters.ContainsKey(nameof(Type)))
        {
            hasAnyType = Type.Contains(MarkdownTokenType.Any);
            if (hasAnyType)
            {
                if (!includeAttributes && Type.Length > 1)
                    includeAttributes = Type.Contains(MarkdownTokenType.HtmlAttributes);
                types = null;
            }
            else
            {
                if (Type.Contains(MarkdownTokenType.HtmlAttributes))
                {
                    includeAttributes = true;
                    types = (Type.Length == 1 || (types = Type.Where(t => t != MarkdownTokenType.HtmlAttributes).ToReflectionTypes()).Count == 0) ? null : types;
                }
                else
                    types = Type.ToReflectionTypes();
            }
        }
        else
        {
            types = null;
            hasAnyType = false;
        }
        int effectiveMinDepth;
        int? effectiveMaxDepth;
        switch (ParameterSetName)
        {
            case ParameterSetName_ExplicitDepth:
                BeginProcessing_ExplicitDepth(types, includeAttributes, Depth);
                break;
            case ParameterSetName_Recurse:
                BeginProcessing_Recurse(types, includeAttributes);
                break;
            case ParameterSetName_RecurseUnmatched:
                if (hasAnyType)
                {
                    _ = GetEffectiveDepthRange(out effectiveMinDepth, out _);
                    switch (effectiveMinDepth)
                    {
                        case 0:
                            _processRecord = Process_ExplicitDepth0;
                            break;
                        case 1:
                            _processRecord = includeAttributes ? Process_ExplicitDepthWithAttributes1 : Process_ExplicitDepthNoAttributes1;
                            break;
                        default:
                            _depth = effectiveMinDepth;
                            _processRecord = includeAttributes ? Process_ExplicitDepthWithAttributesN : Process_ExplicitDepthNoAttributesN;
                            break;
                    }
                }
                else if (GetEffectiveDepthRange(out effectiveMinDepth, out effectiveMaxDepth))
                {
                    if (types is null)
                        BeginProcessing_AttributesInRange(effectiveMinDepth, effectiveMaxDepth);
                    else
                        BeginProcessing_ExplicitDepth(types, includeAttributes, effectiveMinDepth);
                }
                else
                    BeginProcessing_RecurseUnmatched(types, includeAttributes, effectiveMinDepth, effectiveMaxDepth);
                break;
            default:
                if (GetEffectiveDepthRange(out effectiveMinDepth, out effectiveMaxDepth))
                    BeginProcessing_NonRecursive(types, includeAttributes, effectiveMinDepth);
                else
                    BeginProcessing_DepthRange(types, includeAttributes, effectiveMinDepth, effectiveMaxDepth);
                break;
        }
    }

    protected override void ProcessRecord()
    {
        Debug.Assert(_processRecord is not null);
        if (!Stopping)
            _processRecord();
    }
}