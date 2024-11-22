using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace HtmlUtility;

[Cmdlet(VerbsCommon.Open, "HtmlRepository")]
[OutputType(typeof(HtmlRepository))]
public class Open_HtmlRepository : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
    [Alias("LiteralPath", "LP", "FullName", "PSPath")]
    [ValidateNotNullOrWhiteSpace()]
    public string Path { get; set; } = null!;

    protected override void ProcessRecord()
    {
        if (InvokeProvider.Item.Exists(Path, true, true))
        {
            string fullPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(Path);
            if (InvokeProvider.Item.IsContainer(fullPath))
            {
                Collection<PSObject> items = InvokeProvider.Item.Get([fullPath], true, true);
                if (items[0].BaseObject is DirectoryInfo rootDirectory)
                    try
                    {
                        WriteObject(new HtmlRepository(rootDirectory));
                    }
                    catch (Exception exception)
                    {
                        if (exception is IContainsErrorRecord containsErrorRecord)
                            WriteError(containsErrorRecord.ErrorRecord);
                        else
                            WriteError(new(new InvalidOperationException(string.IsNullOrWhiteSpace(exception.Message) ?
                                $"An unexpected exception occurred while opening HTML repository subdirectory {fullPath}" :
                                $"An unexpected exception occurred while opening HTML repository subdirectory {fullPath}: {exception.Message}"), "UnexpectedError", ErrorCategory.InvalidOperation, Path));
                    }
                else
                    WriteError(new(new NotSupportedException("Only FileSystem directories are supported"), "", ErrorCategory.NotImplemented, Path));
            }
            else
                WriteError(new(new DirectoryNotFoundException($"Path is not a container: {fullPath}"), "PathNotAContainer", ErrorCategory.ObjectNotFound, Path));
        }
        else
            WriteError(new(new DirectoryNotFoundException($"Path not found: {Path}"), "PathNotFound", ErrorCategory.ObjectNotFound, Path));
    }
}