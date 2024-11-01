using HtmlAgilityPack;
using System.Diagnostics;
using System.Management.Automation;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Select, "HtmlNode", DefaultParameterSetName = ParameterSetName_Nodes)]
[OutputType(typeof(HtmlNode), ParameterSetName = [ParameterSetName_Nodes])]
[OutputType(typeof(HtmlNodeCollection), ParameterSetName = [ParameterSetName_Collection])]
public class Select_HtmlNode : PSCmdlet
{
    public const string ParameterSetName_Nodes = "Nodes";
    
    public const string ParameterSetName_Collection = "Collection";
    
    /// <summary>
    /// The document to convert.
    /// </summary>
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    [ValidateNotNull()]
    [Alias(nameof(HtmlDocument.DocumentNode))]
    public HtmlNode[] InputNode { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    [ValidateNotNullOrWhiteSpace()]
    public string XPath { get; set; } = null!;

    [Parameter(ParameterSetName = ParameterSetName_Nodes)]
    public SwitchParameter First { get; set; }


    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Collection)]
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