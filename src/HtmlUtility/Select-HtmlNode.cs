using HtmlAgilityPack;
using System.Diagnostics;
using System.Management.Automation;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "HtmlNode", DefaultParameterSetName = ParameterSetName_Nodes)]
[OutputType(typeof(HtmlNode), ParameterSetName = [ParameterSetName_Single, ParameterSetName_Collection])]
[OutputType(typeof(HtmlNodeCollection), ParameterSetName = [ParameterSetName_Collection])]
public class Select_HtmlNode : PSCmdlet
{
    public const string ParameterSetName_Single = "Single";
    
    public const string ParameterSetName_Collection = "Collection";
    
    public const string ParameterSetName_Nodes = "Nodes";
    
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "HTML node to select from.")]
    [ValidateNotNull()]
    [Alias(nameof(HtmlDocument.DocumentNode))]
    public HtmlNode[] InputNode { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1, HelpMessage = "The XPath of the HTML node(s) to select.")]
    [ValidateNotNullOrWhiteSpace()]
    public string XPath { get; set; } = null!;

    [Parameter(ParameterSetName = ParameterSetName_Nodes, HelpMessage = "Returns all matching nodes as individual HtmlNode objects. This is the default behavior.")]
    [Alias("SelectNodes")]
    public SwitchParameter All { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Single, HelpMessage = "Returns the first matching HtmlNode object.")]
    [Alias("Single", "SelectSingleNode")]
    public SwitchParameter First { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Collection, HelpMessage = "Return all matching nodes as a single HtmlNodeCollection object.")]
    public SwitchParameter AsNodeCollection { get; set; }

    protected override void ProcessRecord()
    {
        if (First.IsPresent)
            foreach (var html in InputNode)
            {
                HtmlNode? node;
                try { node = html.SelectSingleNode(XPath); }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(new PSArgumentException(string.IsNullOrWhiteSpace(exception.Message) ? $"Error evaluating XPath '{XPath}'" : $"Error evaluating XPath '{XPath}': {exception.Message.Trim()}", nameof(XPath)), "XPathError", ErrorCategory.InvalidArgument, XPath));
                    node = null;
                }
                if (node is not null)
                    WriteObject(node);
            }
        else if (AsNodeCollection.IsPresent)
            foreach (var html in InputNode)
            {
                HtmlNodeCollection? nodes;
                try { nodes = html.SelectNodes(XPath); }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(new PSArgumentException(string.IsNullOrWhiteSpace(exception.Message) ? $"Error evaluating XPath '{XPath}'" : $"Error evaluating XPath '{XPath}': {exception.Message.Trim()}", nameof(XPath)), "XPathError", ErrorCategory.InvalidArgument, XPath));
                    nodes = null;
                }
                if (nodes is not null)
                    WriteObject(nodes, false);
            }
        else
            foreach (var html in InputNode)
            {
                HtmlNodeCollection? nodes;
                try { nodes = html.SelectNodes(XPath); }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(new PSArgumentException(string.IsNullOrWhiteSpace(exception.Message) ? $"Error evaluating XPath '{XPath}'" : $"Error evaluating XPath '{XPath}': {exception.Message.Trim()}", nameof(XPath)), "XPathError", ErrorCategory.InvalidArgument, XPath));
                    nodes = null;
                }
                if (nodes is not null && nodes.Count > 0)
                    WriteObject(nodes);
            }
    }
}