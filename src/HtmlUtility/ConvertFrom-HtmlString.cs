using HtmlAgilityPack;
using System.Management.Automation;

namespace HtmlUtility;

[Cmdlet(VerbsData.ConvertFrom, "HtmlString")]
[OutputType(typeof(HtmlDocument))]
public class ConvertFrom_HtmlString : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    [ValidateNotNull()]
    [AllowEmptyString()]
    public string InputText { get; set; } = null!;

    /// <summary>
    /// What action to take during a parsing error.
    /// </summary>
    [Parameter()]
    public ParseErrorHandling ErrorHandling { get; set; }

    /// <summary>
    /// Adds Debugging attributes to node.
    /// </summary>
    [Parameter()]
    public SwitchParameter AddDebuggingAttributes { get; set; }

    /// <summary>
    /// Defines if closing for non closed nodes must be done at the end or directly in the document.
    /// </summary>
    [Parameter()]
    public SwitchParameter AutoCloseOnEnd { get; set; }

    /// <summary>
    /// Do not check non-closed nodes at the end of parsing.
    /// </summary>
    [Parameter()]
    public SwitchParameter DoNotCheckSyntax { get; set; }

    /// <summary>
    /// Compute checksum while parsing.
    /// </summary>
    [Parameter()]
    public SwitchParameter ComputeChecksum { get; set; }

    /// <summary>
    /// Treat whole <![CDATA[ block as a single comment.
    /// </summary>
    [Parameter()]
    public SwitchParameter TreatCDataBlockAsComment { get; set; }
    
    /// <summary>
    /// Extract source text while parsing errors.
    /// </summary>
    [Parameter()]
    public SwitchParameter ExtractErrorSourceText { get; set; }
    
    /// <summary>
    /// Defines the maximum length of source text or parse errors.
    /// </summary>
    [Parameter()]
    public int ExtractErrorSourceTextMaxLength { get; set; }
    
    /// <summary>
    /// LI, TR, TH, TD tags will be partially fixed when nesting errors are detected.
    /// </summary>
    [Parameter()]
    public SwitchParameter FixNestedTags { get; set; }
    
    /// <summary>
    /// Do not read declared encoding from document.
    /// </summary>
    [Parameter()]
    public SwitchParameter DoNotReadEncoding { get; set; }
    
    /// <summary>
    /// The max number of nested child nodes allowed.
    /// </summary>
    [Parameter()]
    public int MaxNestedChildNodes { get; set; }

    private StringWriter _writer = null!;

    protected override void BeginProcessing() { _writer = new(); }

    protected override void ProcessRecord()
    {
        if (InputText.EndsWith('\n'))
            _writer.Write(InputText);
        else
            _writer.WriteLine(InputText);
    }

    protected override void EndProcessing()
    {
        if (_writer == null) return;
        var text = _writer.ToString();
        HtmlDocument result = new()
        {
            OptionAddDebuggingAttributes = AddDebuggingAttributes.IsPresent,
            OptionAutoCloseOnEnd = AutoCloseOnEnd.IsPresent,
            OptionCheckSyntax = !DoNotCheckSyntax.IsPresent,
            OptionComputeChecksum = ComputeChecksum.IsPresent,
            OptionTreatCDataBlockAsComment = TreatCDataBlockAsComment.IsPresent,
            OptionOutputAsXml = FixNestedTags.IsPresent,
            OptionFixNestedTags = FixNestedTags.IsPresent,
            OptionReadEncoding = !DoNotReadEncoding.IsPresent
        };
        if (ExtractErrorSourceText.IsPresent)
        {
            if (MyInvocation.BoundParameters.ContainsKey(nameof(ExtractErrorSourceTextMaxLength)))
            {
                if (ExtractErrorSourceTextMaxLength > 0)
                {
                    result.OptionExtractErrorSourceText = true;
                    result.OptionExtractErrorSourceTextMaxLength = ExtractErrorSourceTextMaxLength;
                }
                else
                    result.OptionExtractErrorSourceText = false;
            }
            else
                result.OptionExtractErrorSourceText = true;
        }
        else
            result.OptionExtractErrorSourceText = false;
        if (MyInvocation.BoundParameters.ContainsKey(nameof(MaxNestedChildNodes)))
            result.OptionMaxNestedChildNodes = MaxNestedChildNodes;

        result.LoadHtml(text);
        if (ErrorHandling != ParseErrorHandling.Ignore)
        {
            using var enumerator = result.ParseErrors.GetEnumerator();
            if (enumerator.MoveNext())
            {
                switch (ErrorHandling)
                {
                    case ParseErrorHandling.AsWarning:
                        do
                        {
                            var pe = enumerator.Current;
                            WriteWarning($"Line {pe.Line}, Position {pe.LinePosition}: {pe.Reason}");
                        }
                        while (enumerator.MoveNext());
                        break;
                    case ParseErrorHandling.Information:
                        do
                        {
                            var pe = enumerator.Current;
                            WriteInformation(new($"Line {pe.Line}, Position {pe.LinePosition}: {pe.Reason}", "ConvertFrom-HtmlString"));
                        }
                        while (enumerator.MoveNext());
                        break;
                    case ParseErrorHandling.Verbose:
                        do
                        {
                            var pe = enumerator.Current;
                            WriteVerbose($"Line {pe.Line}, Position {pe.LinePosition}: {pe.Reason}");
                        }
                        while (enumerator.MoveNext());
                        break;
                    case ParseErrorHandling.Debug:
                        do
                        {
                            var pe = enumerator.Current;
                            WriteDebug($"Line {pe.Line}, Position {pe.LinePosition}: {pe.Reason}");
                        }
                        while (enumerator.MoveNext());
                        break;
                    default:
                        do
                        {
                            var pe = enumerator.Current;
                            WriteError(new(new PSArgumentException($"Line {pe.Line}, Position {pe.LinePosition}: {pe.Reason}", nameof(InputText)), pe.Code.ToString("F"), ErrorCategory.ParserError, pe));
                        }
                        while (enumerator.MoveNext());
                        break;
                }
            }
        }
        WriteObject(result);
    }
}
