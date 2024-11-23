using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text.Json.Nodes;

namespace HtmlUtility;

public class HtmlRepository
{
    public const string ImagesFolderName = "img";
    public const string StylesFolderName = "style";
    public const string ScriptsFolderName = "script";
    public const string ContentFolderName = "content";
    public const string ImagesPropertyName = "images";
    public const string StylesPropertyName = "styles";
    public const string ScriptsPropertyName = "scripts";
    public const string ContentPropertyName = "content";
    public const string ContentDbName = "content-db.json";
    public const string IndexHtmlName = "index";
    public const int ResourcesFolderMaxDepth = 16;
    public const int ContentFolderMaxDepth = 32;

    public DirectoryInfo RootDirectory { get; }

    public TrackedResourceDirectory ImagesFolder { get; }

    public TrackedResourceDirectory StylesFolder { get; }

    public TrackedResourceDirectory ScriptsFolder { get; }

    public TrackedHtmlDirectory ContentFolder { get; }

    public HtmlRepository(DirectoryInfo rootDirectory)
    {
        RootDirectory = rootDirectory;
        string dbFilePath = Path.Combine(rootDirectory.FullName, IndexHtmlName);
        if (Path.Exists(dbFilePath))
        {
            if (!File.Exists(dbFilePath))
                throw new InvalidRepositoryContentException($"{IndexHtmlName} in {rootDirectory.FullName} is not a file", "ExpectedFile", ErrorCategory.InvalidData, rootDirectory);
        }
        ImagesFolder = TrackedResourceDirectory.Load(this, ImagesFolderName, ResourcesFolderMaxDepth);
        StylesFolder = TrackedResourceDirectory.Load(this, StylesFolderName, ResourcesFolderMaxDepth);
        ScriptsFolder = TrackedResourceDirectory.Load(this, ScriptsFolderName, ResourcesFolderMaxDepth);
        ContentFolder = TrackedHtmlDirectory.Load(this, ContentFolderName, ContentFolderMaxDepth);
        dbFilePath = Path.Combine(rootDirectory.FullName, ContentDbName);
        if (Path.Exists(dbFilePath))
        {
            if (!File.Exists(dbFilePath))
                throw new InvalidRepositoryContentException($"{ContentDbName} in {rootDirectory.FullName} is not a file", "ExpectedFile", ErrorCategory.InvalidData, rootDirectory);
            using FileStream stream = File.Open(dbFilePath, FileMode.Open, FileAccess.Read);
            var jsonNode = JsonNode.Parse(stream);
            if (jsonNode is not JsonObject jsonObject)
                throw new InvalidRepositoryContentException($"{ContentDbName} in {rootDirectory.FullName} does not contain valid data", "InvalidJsonContent", ErrorCategory.InvalidData, rootDirectory);
            if (jsonObject.TryGetPropertyValue(ImagesPropertyName, out jsonNode) && jsonNode is JsonObject imagesJson)
                ImagesFolder.Load(imagesJson);
            else
                throw new InvalidRepositoryContentException($"{ContentDbName} in {rootDirectory.FullName} does not contain valid data (invalid {ImagesPropertyName} property)", "InvalidJsonContent", ErrorCategory.InvalidData, rootDirectory);
            if (jsonObject.TryGetPropertyValue(StylesPropertyName, out jsonNode) && jsonNode is JsonObject stylesJson)
                StylesFolder.Load(stylesJson);
            else
                throw new InvalidRepositoryContentException($"{ContentDbName} in {rootDirectory.FullName} does not contain valid data (invalid {StylesPropertyName} property)", "InvalidJsonContent", ErrorCategory.InvalidData, rootDirectory);
            if (jsonObject.TryGetPropertyValue(ScriptsPropertyName, out jsonNode) && jsonNode is JsonObject scriptsJson)
                ScriptsFolder.Load(scriptsJson);
            else
                throw new InvalidRepositoryContentException($"{ContentDbName} in {rootDirectory.FullName} does not contain valid data (invalid {ScriptsPropertyName} property)", "InvalidJsonContent", ErrorCategory.InvalidData, rootDirectory);
            if (jsonObject.TryGetPropertyValue(ContentPropertyName, out jsonNode) &&jsonNode is JsonObject source)
                ContentFolder.Load(source);
            else
                throw new InvalidRepositoryContentException($"{ContentDbName} in {rootDirectory.FullName} does not contain valid data (invalid {ContentPropertyName} property)", "InvalidJsonContent", ErrorCategory.InvalidData, rootDirectory);
        }
    }
}

public interface ITrackedItem
{
    FileSystemInfo FileSystemInfo { get; }
    ITrackedFolder? Parent { get; }
    HtmlRepository Repository { get; }
}

public interface ITrackedFolder : ITrackedItem, IReadOnlyList<ITrackedItem>
{
    new DirectoryInfo FileSystemInfo { get; }
}

public interface ITrackedFile : ITrackedItem
{
    new FileInfo FileSystemInfo { get; }
}

public abstract class TrackedResourceNode : ITrackedItem
{
    public TrackedResourceDirectory? Parent { get; }

    ITrackedFolder? ITrackedItem.Parent => Parent;

    public HtmlRepository Repository { get; }

    public FileSystemInfo FileSystemInfo { get; }

    protected TrackedResourceNode(FileSystemInfo fileSystemInfo, TrackedResourceDirectory parent)
    {
        ArgumentNullException.ThrowIfNull(fileSystemInfo);
        ArgumentNullException.ThrowIfNull(parent);
        FileSystemInfo = fileSystemInfo;
        Parent = parent;
        Repository = parent.Repository;
    }

    protected TrackedResourceNode(FileSystemInfo fileSystemInfo, HtmlRepository repository)
    {
        ArgumentNullException.ThrowIfNull(fileSystemInfo);
        ArgumentNullException.ThrowIfNull(repository);
        Repository = repository;
        FileSystemInfo = fileSystemInfo;
    }
}

public class TrackedResourceDirectory : TrackedResourceNode, IReadOnlyList<TrackedResourceNode>, ITrackedFolder
{
    public const string PropertyName_Files = "files";
    
    public const string PropertyName_Folders = "folders";
    
    private readonly Collection<TrackedResourceNode> _innerList = [];

    public TrackedResourceNode this[int index] => _innerList[index];

    ITrackedItem IReadOnlyList<ITrackedItem>.this[int index] => _innerList[index];

    public int Count => _innerList.Count;

    public new DirectoryInfo FileSystemInfo => (DirectoryInfo)base.FileSystemInfo;

    public IEnumerator<TrackedResourceNode> GetEnumerator() => _innerList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_innerList).GetEnumerator();

    IEnumerator<ITrackedItem> IEnumerable<ITrackedItem>.GetEnumerator() => _innerList.Cast<ITrackedItem>().GetEnumerator();

    internal static TrackedResourceDirectory Load(HtmlRepository htmlRepository, string folderName, int maxDepth)
    {
        DirectoryInfo directoryInfo = new(Path.Combine(htmlRepository.RootDirectory.FullName, folderName));
        if (!directoryInfo.Exists)
        {
            if (Path.Exists(directoryInfo.FullName))
                throw new InvalidRepositoryContentException($"{folderName} in {htmlRepository.RootDirectory.FullName} was expected to be a subdirectory", "ExpectedFolder", ErrorCategory.InvalidData, htmlRepository.RootDirectory);
            directoryInfo.Create();
        }
        return new(directoryInfo, htmlRepository, maxDepth);
    }

    public TrackedResourceDirectory(JsonObject source, string name, TrackedResourceDirectory parent)
        : base(new DirectoryInfo(Path.Combine(parent.FileSystemInfo.FullName, name)), parent)
    {
    }

    public TrackedResourceDirectory(DirectoryInfo fileSystemInfo, TrackedResourceDirectory parent, int maxDepth)
        : base(fileSystemInfo, parent)
    {
        if (fileSystemInfo.Parent is null || !string.Equals(fileSystemInfo.Parent.FullName, parent.FileSystemInfo.FullName))
            throw new InvalidOperationException();
    }

    public TrackedResourceDirectory(DirectoryInfo fileSystemInfo, HtmlRepository repository, int maxDepth)
        : base(fileSystemInfo, repository)
    {
        if (fileSystemInfo.Parent is null || !string.Equals(fileSystemInfo.Parent.FullName, repository.RootDirectory.FullName))
            throw new InvalidOperationException();
        foreach (FileInfo fileInfo in fileSystemInfo.GetFiles())
            _innerList.Add(new TrackedResourceFile(fileInfo, this));
        if (maxDepth < 1) return;
        maxDepth--;
        foreach (DirectoryInfo directoryInfo in fileSystemInfo.GetDirectories())
            _innerList.Add(new TrackedResourceDirectory(directoryInfo, this, maxDepth));
    }

    internal void Load(JsonObject source)
    {
        if (source.TryGetPropertyValue(PropertyName_Files, out JsonNode? filesNode))
        {
            if (filesNode is not JsonObject jsonObject)
                throw new InvalidRepositoryContentException();
            foreach (var missingItem in jsonObject.Where(kvp =>
            {
                var matching = _innerList.OfType<TrackedResourceFile>().FirstOrDefault(f => string.Equals(f.FileSystemInfo.Name, kvp.Key, StringComparison.CurrentCultureIgnoreCase));
                if (matching is null) return true;
                if (kvp.Value is not JsonObject itemObj)
                    throw new InvalidOperationException();
                matching.Load(itemObj);
                return false;
            }).ToArray())
            {
                if (missingItem.Value is not JsonObject itemObj)
                    throw new InvalidOperationException();
                _innerList.Add(new TrackedResourceFile(itemObj, missingItem.Key, this));
            }
        }
        if (source.TryGetPropertyValue(PropertyName_Folders, out JsonNode? foldersNode))
        {
            if (foldersNode is not JsonObject jsonObject)
                throw new InvalidRepositoryContentException();
            foreach (var missingItem in jsonObject.Where(kvp =>
            {
                var matching = _innerList.OfType<TrackedResourceDirectory>().FirstOrDefault(f => string.Equals(f.FileSystemInfo.Name, kvp.Key, StringComparison.CurrentCultureIgnoreCase));
                if (matching is null) return true;
                if (kvp.Value is not JsonObject itemObj)
                    throw new InvalidOperationException();
                matching.Load(itemObj);
                return false;
            }).ToArray())
            {
                if (missingItem.Value is not JsonObject itemObj)
                    throw new InvalidOperationException();
                _innerList.Add(new TrackedResourceDirectory(itemObj, missingItem.Key, this));
            }
        }
    }
}

public class TrackedResourceFile : TrackedResourceNode, ITrackedFile
{
    public new FileInfo FileSystemInfo => (FileInfo)base.FileSystemInfo;

    public TrackedResourceFile(FileInfo fileSystemInfo, TrackedResourceDirectory parent)
        : base(fileSystemInfo, parent)
    {
        if (fileSystemInfo.DirectoryName is null || !string.Equals(fileSystemInfo.DirectoryName, parent.FileSystemInfo.FullName))
            throw new InvalidOperationException();
    }

    public TrackedResourceFile(JsonObject source, string name, TrackedResourceDirectory parent)
        : base(new FileInfo(Path.Combine(parent.FileSystemInfo.FullName, name)), parent)
    {
    }

    internal void Load(JsonObject source)
    {
        throw new NotImplementedException();
    }
}

public abstract class TrackedHtmlItem : ITrackedItem
{
    public TrackedHtmlDirectory? Parent { get; }

    ITrackedFolder? ITrackedItem.Parent => Parent;

    public HtmlRepository Repository { get; }

    public FileSystemInfo FileSystemInfo { get; }

    protected TrackedHtmlItem(FileSystemInfo fileSystemInfo, TrackedHtmlDirectory parent)
    {
        ArgumentNullException.ThrowIfNull(fileSystemInfo);
        ArgumentNullException.ThrowIfNull(parent);
        FileSystemInfo = fileSystemInfo;
        Parent = parent;
        Repository = parent.Repository;
    }

    protected TrackedHtmlItem(FileSystemInfo fileSystemInfo, HtmlRepository repository)
    {
        ArgumentNullException.ThrowIfNull(fileSystemInfo);
        ArgumentNullException.ThrowIfNull(repository);
        Repository = repository;
        FileSystemInfo = fileSystemInfo;
    }

    protected internal virtual void Load(JsonObject source)
    {

        throw new NotImplementedException();
    }
}

public class TrackedHtmlDirectory : TrackedHtmlItem, IReadOnlyList<TrackedHtmlItem>, ITrackedFolder
{
    private readonly Collection<TrackedHtmlItem> _innerList = [];

    public TrackedHtmlItem this[int index] => _innerList[index];

    ITrackedItem IReadOnlyList<ITrackedItem>.this[int index] => _innerList[index];

    public int Count => _innerList.Count;

    public new DirectoryInfo FileSystemInfo => (DirectoryInfo)base.FileSystemInfo;

    public IEnumerator<TrackedHtmlItem> GetEnumerator() => _innerList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_innerList).GetEnumerator();

    IEnumerator<ITrackedItem> IEnumerable<ITrackedItem>.GetEnumerator() => _innerList.Cast<ITrackedItem>().GetEnumerator();

    internal static TrackedHtmlDirectory Load(HtmlRepository htmlRepository, string folderName, int maxDepth)
    {
        throw new NotImplementedException();
    }

    protected internal override void Load(JsonObject source)
    {
        base.Load(source);
    }

    public TrackedHtmlDirectory(DirectoryInfo fileSystemInfo, TrackedHtmlDirectory parent, int maxDepth)
        : base(fileSystemInfo, parent)
    {
        if (fileSystemInfo.Parent is null || !string.Equals(fileSystemInfo.Parent.FullName, parent.FileSystemInfo.FullName))
            throw new InvalidOperationException();
    }

    public TrackedHtmlDirectory(DirectoryInfo fileSystemInfo, HtmlRepository repository, int maxDepth)
        : base(fileSystemInfo, repository)
    {
        if (fileSystemInfo.Parent is null || !string.Equals(fileSystemInfo.Parent.FullName, repository.RootDirectory.FullName))
            throw new InvalidOperationException();
    }
}

public class TrackedHtmlFile : TrackedHtmlItem, ITrackedFile
{
    public new FileInfo FileSystemInfo => (FileInfo)base.FileSystemInfo;

    public TrackedHtmlFile(FileInfo fileSystemInfo, TrackedHtmlDirectory parent)
        : base(fileSystemInfo, parent)
    {
        if (fileSystemInfo.DirectoryName is null || !string.Equals(fileSystemInfo.DirectoryName, parent.FileSystemInfo.FullName))
            throw new InvalidOperationException();
    }
}