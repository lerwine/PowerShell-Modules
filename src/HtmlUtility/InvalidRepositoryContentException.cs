using System.Management.Automation;

namespace HtmlUtility;

[Serializable]
public class InvalidRepositoryContentException : Exception, IContainsErrorRecord
{
    public InvalidRepositoryContentException()
    {
        ErrorRecord = new(this, "InvalidRepositoryContentException", ErrorCategory.NotSpecified, null);
    }
    public InvalidRepositoryContentException(string message, string errorId, ErrorCategory category, DirectoryInfo rootDirectory) : base(message)
    {
        ErrorRecord = new(this, errorId, category, rootDirectory);
    }
    public InvalidRepositoryContentException(string message, string errorId, ErrorCategory category, DirectoryInfo rootDirectory, System.Exception inner) : base(message, inner)
    {
        ErrorRecord = new(this, errorId, category, rootDirectory);
    }

    public ErrorRecord ErrorRecord { get; }
}