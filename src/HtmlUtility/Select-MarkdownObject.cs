using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Markdig.Syntax;
using Microsoft.PowerShell.Commands;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "MarkdownObject", DefaultParameterSetName = ParameterSetName_ByType)]
public class Select_MarkdownObject : PSCmdlet
{
    private const string ParameterSetName_ByType = "ByType";
    private const string ParameterSetName_ExplicitDepth = "ExplicitDepth";
    private const string ParameterSetName_DepthRange = "DepthRange";

    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select from.")]
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select from.")]
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select from.")]
    [ValidateNotNull()]
    [Alias("MarkdownObject", "Markdown")]
    public Markdig.Syntax.MarkdownObject[] InputObject { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1, HelpMessage = "The type of the Markdown object(s) to select.", ParameterSetName = ParameterSetName_ByType)]
    [Parameter(Position = 1, HelpMessage = "The type of the Markdown object(s) to select.")]
    public MarkdownTokenType[] Type { get; set; } = null!;

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitDepth)]
    [ValidateRange(0, int.MaxValue)]
    public int Depth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    [ValidateRange(0, int.MaxValue)]
    public int MinDepth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    [ValidateRange(0, int.MaxValue)]
    public int MaxDepth { get; set; }

    private IMarkdownObjectFilter _filter = null!;

    interface IMarkdownObjectFilter
    {
        IEnumerable<Markdig.Syntax.MarkdownObject> GetMatches(Markdig.Syntax.MarkdownObject source);
    }

    class SingleTypeFilter : IMarkdownObjectFilter
    {
        public IEnumerable<MarkdownObject> GetMatches(MarkdownObject source)
        {
            throw new NotImplementedException();
        }
    }

    class MultiTypeFilter : IMarkdownObjectFilter
    {
        public IEnumerable<MarkdownObject> GetMatches(MarkdownObject source)
        {
            throw new NotImplementedException();
        }
    }

    protected override void BeginProcessing()
    {
        if (ParameterSetName == ParameterSetName_ByType)
        {
            // List<Type> byType = Type.ToUniqueReflectionTypes()!;
            // if (byType.Count == 1)
            //     _filter = new SingleTypeFilter(byType[0]);
            // else
            //     _filter = new MultiTypeFilter(byType);
            
        }
        else
        {
            List<Type>? byType = Type.ToUniqueReflectionTypes();
            if (ParameterSetName == ParameterSetName_ExplicitDepth)
            {
                // TODO: Depth
            }
            else if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
            {
                if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
                {
                    // TODO: MinDepth, MaxDepth
                }
                else
                {
                    // TODO: MinDepth
                }
            }
            else
            {
                    // TODO: MaxDepth
            }
        }
    }

    protected override void ProcessRecord()
    {
    }
}
