using HtmlAgilityPack;
using System.Management.Automation;
using System.Xml;

namespace HtmlUtility;

[Cmdlet(VerbsData.ConvertFrom, "HtmlDocument", DefaultParameterSetName = ParameterSetName_Html)]
[OutputType(typeof(string), ParameterSetName = [ParameterSetName_Html, ParameterSetName_XmlString])]
[OutputType(typeof(XmlDocument), ParameterSetName = [ParameterSetName_XmlDocument])]
public class ConvertFrom_HtmlDocument : PSCmdlet
{
    public const string ParameterSetName_Html = "Html";
    
    public const string ParameterSetName_XmlString = "XmlString";
    
    public const string ParameterSetName_XmlDocument = "XmlDocument";
    
    /// <summary>
    /// The document to convert.
    /// </summary>
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    [ValidateNotNull()]
    public HtmlDocument[] InputDocument { get; set; } = null!;

    /// <summary>
    /// Output must conform to XML, instead of HTML.
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_XmlString)]
    public SwitchParameter AsXmlString { get; set; }

    /// <summary>
    /// Output as XML document object.
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_XmlDocument)]
    public SwitchParameter AsXmlDocument { get; set; }

    /// <summary>
    /// Preserve XML namespaces in element names.
    /// </summary>
    [Parameter(ParameterSetName = ParameterSetName_XmlString)]
    [Parameter(ParameterSetName = ParameterSetName_XmlDocument)]
    public SwitchParameter PreserveXmlNamespaces { get; set; }

    /// <summary>
    /// Attribute value output must be optimized (not bound with double quotes if it is possible).
    /// </summary>
    [Parameter()]
    public SwitchParameter OptimizeAttributeValues { get; set; }

    /// <summary>
    /// the global attribute value quote.
    /// </summary>
    [Parameter()]
    public AttributeValueQuote GlobalAttributeValueQuote { get; set; }

    /// <summary>
    /// Tag names must be output with their original case.
    /// </summary>
    public SwitchParameter OriginalTagCase { get; set; }

    /// <summary>
    /// Attributes should use original names by default, rather than lower case.
    /// </summary>
    public SwitchParameter OriginalAttributeCase { get; set; }

    /// <summary>
    /// Determines if empty nodes are written.
    /// </summary>
    public EmptyHtmlNodeOption EmptyNodes { get; set; } = EmptyHtmlNodeOption.DoNotWrite;

    protected override void ProcessRecord()
    {
        foreach (var html in InputDocument)
        {
            if (AsXmlString.IsPresent || AsXmlDocument.IsPresent)
            {
                html.OptionOutputAsXml = true;
                html.OptionPreserveXmlNamespaces = PreserveXmlNamespaces.IsPresent;
            }
            else
                html.OptionOutputAsXml = false;
            html.OptionOutputOptimizeAttributeValues = OptimizeAttributeValues.IsPresent;
            if (MyInvocation.BoundParameters.ContainsKey(nameof(GlobalAttributeValueQuote)))
                html.GlobalAttributeValueQuote = GlobalAttributeValueQuote;
            else
                html.GlobalAttributeValueQuote = null;
            html.OptionOutputOriginalCase = OriginalTagCase.IsPresent;
            html.OptionDefaultUseOriginalName = OriginalAttributeCase.IsPresent;
            switch (EmptyNodes)
            {
                case EmptyHtmlNodeOption.WriteNoWhitespace:
                    html.OptionWriteEmptyNodes = true;
                    html.OptionWriteEmptyNodesWithoutSpace = false;
                    break;
                case EmptyHtmlNodeOption.IncludeWhitespace:
                    html.OptionWriteEmptyNodes = true;
                    html.OptionWriteEmptyNodesWithoutSpace = true;
                    break;
                default:
                    html.OptionWriteEmptyNodes = false;
                    html.OptionWriteEmptyNodesWithoutSpace = false;
                    break;
            }
            using StringWriter writer = new();
            html.Save(writer);
            string text = writer.ToString();
            
            if (AsXmlDocument.IsPresent)
            {
                XmlDocument? xml = new();
                try { xml.LoadXml(text); }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, "XmlParseError", ErrorCategory.InvalidData, text));
                    xml = null;
                }
                if (xml != null)
                    WriteObject(xml);
            } else
                WriteObject(text);
        }
    }
}