using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Markdig.Renderers.Html;
using Microsoft.PowerShell.Commands;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "MarkdownObject", DefaultParameterSetName = ParameterSetName_DepthRange)]
public partial class Select_MarkdownObject : PSCmdlet
{
    private const string ParameterSetName_ExplicitDepth = "ExplicitDepth";
    private const string ParameterSetName_DepthRange = "DepthRange";

    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Markdown object to select from.")]
    [ValidateNotNull()]
    [Alias("MarkdownObject", "Markdown")]
    public Markdig.Syntax.MarkdownObject[] InputObject { get; set; } = null!;

    [Parameter(Position = 1, HelpMessage = "The type of the Markdown object(s) to select.")]
    public MarkdownTokenType[] Type { get; set; } = null!;

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitDepth)]
    [ValidateRange(1, int.MaxValue)]
    public int Depth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    [ValidateRange(1, int.MaxValue)]
    public int MinDepth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    [ValidateRange(1, int.MaxValue)]
    public int MaxDepth { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    public SwitchParameter Recurse { get; set; }

    [Parameter(ParameterSetName = ParameterSetName_DepthRange)]
    public SwitchParameter DoNotRecurseMatches { get; set; }

    protected override void BeginProcessing()
    {

        List<Type>? typesToMatch = Type.ToReflectionTypes();
        if (ParameterSetName == ParameterSetName_ExplicitDepth)
        {
            // Get descendant MarkdownObject exactly at Depth levels deep
        }
        else if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxDepth)))
        {
            if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
            {
                if (MinDepth > MaxDepth)
                {
                    WriteError(new(new ArgumentOutOfRangeException(nameof(MaxDepth), $"{nameof(MaxDepth)} cannot be less than {nameof(MinDepth)}"), "", ErrorCategory.InvalidArgument, MaxDepth));
                    throw new PipelineStoppedException();
                }
                if (MinDepth == MaxDepth)
                {
                    // Get descendant MarkdownObject exactly at MinDepth levels deep
                }
                else if (DoNotRecurseMatches.IsPresent)
                {
                    // MinDepth = 1; MaxDepth = 2; Recurse = any;   DoNotRecurseMatches = true;
                }
                else
                {
                    // MinDepth = 1; MaxDepth = 2; Recurse = any;   DoNotRecurseMatches = false
                }
            }
            else if (DoNotRecurseMatches.IsPresent)
            {
                // MinDepth = ?; MaxDepth = 1; Recurse = any;   DoNotRecurseMatches = true;  Depth = ?;
            }
            else
            {
                // MinDepth = ?; MaxDepth = 1; Recurse = any;   DoNotRecurseMatches = false; Depth = ?;
            }
        }
        else if (MyInvocation.BoundParameters.ContainsKey(nameof(MinDepth)))
        {
            if (DoNotRecurseMatches.IsPresent)
            {
                // MinDepth = 1; MaxDepth = ?; Recurse = any;   DoNotRecurseMatches = true;  Depth = ?;
            }
            else
            {
                // MinDepth = 1; MaxDepth = ?; Recurse = any;   DoNotRecurseMatches = false; Depth = ?;
            }
        }
        else if (Recurse.IsPresent)
        {
            if (DoNotRecurseMatches.IsPresent)
            {
                // MinDepth = ?; MaxDepth = ?; Recurse = true;  DoNotRecurseMatches = true;  Depth = ?;
                // MinDepth = ?; MaxDepth = ?; Recurse = true;  DoNotRecurseMatches = true;  Depth = ?;
            }
            else
            {
                // MinDepth = ?; MaxDepth = ?; Recurse = false; DoNotRecurseMatches = any;   Depth = ?;
                // MinDepth = ?; MaxDepth = ?; Recurse = true;  DoNotRecurseMatches = false; Depth = ?;
                // MinDepth = ?; MaxDepth = ?; Recurse = true;  DoNotRecurseMatches = false; Depth = ?;
            }
        }
        else
        {
                // MinDepth = ?; MaxDepth = ?; Recurse = ?;     DoNotRecurseMatches = false; Depth = ?;

        }
    }

    protected override void ProcessRecord()
    {
        throw new NotImplementedException();
    }
}
