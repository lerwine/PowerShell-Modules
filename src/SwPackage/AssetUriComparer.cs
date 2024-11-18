using System.Diagnostics.CodeAnalysis;

namespace SwPackage;

public class AssetUriComparer : IEqualityComparer<Uri>, IComparer<Uri>
{
    public static readonly Uri EmptyURI = new("", UriKind.Relative);

    public static readonly AssetUriComparer Default = new();

    private static string RemoveTrailingSlash(string path)
    {
        while (path.EndsWith('/'))
        {
            path = path[..^1];
            if (path.Length == 1) break;
        }
        return path;
    }

    private static string SplitAtIndexOf(string source, char value, out string leaf)
    {
        int i = source.IndexOf(value);
        if (i < 0)
        {
            leaf = string.Empty;
            return source;
        }
        leaf = source[(i + 1)..];
        return source[..i];
    }

    public int Compare(Uri? x, Uri? y)
    {
        if (x is null) return (y is null) ? 0 : -1;
        if (y is null) return 1;
        if (ReferenceEquals(x, y)) return 0;
        int diff;
        string pathX, pathY;
        if (x.IsAbsoluteUri)
        {
            if (!y.IsAbsoluteUri) return -1;
            if ((diff = string.Compare(x.GetComponents(UriComponents.Scheme, UriFormat.Unescaped), y.GetComponents(UriComponents.Scheme, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase)) == 0 &&
                (diff = string.Compare(x.GetComponents(UriComponents.Host, UriFormat.Unescaped), y.GetComponents(UriComponents.Host, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase)) == 0 &&
                (diff = string.Compare(x.GetComponents(UriComponents.Port, UriFormat.Unescaped), y.GetComponents(UriComponents.Port, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase)) == 0)
            {
                pathX = x.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                pathY = y.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                if (pathX.Length > 1 && pathY.Length > 1)
                {
                    pathX = RemoveTrailingSlash(pathX);
                    pathY = RemoveTrailingSlash(pathY);
                }
                if ((diff = string.Compare(pathX, pathY, StringComparison.OrdinalIgnoreCase)) == 0 &&
                    (diff = string.Compare(x.GetComponents(UriComponents.Query, UriFormat.Unescaped), y.GetComponents(UriComponents.Query, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase)) == 0)
                    return string.Compare(x.GetComponents(UriComponents.Fragment, UriFormat.Unescaped), y.GetComponents(UriComponents.Fragment, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase);
            }
            return diff;
        }
        if (y.IsAbsoluteUri) return 1;
        pathX = x.OriginalString;
        pathY = y.OriginalString;
        if (pathX.Length == 0)
            return (pathY.Length > 0) ? -1 : 0;
        if (pathY.Length == 0) return 1;
        pathX = SplitAtIndexOf(pathX, '?', out string queryX);
        pathY = SplitAtIndexOf(pathY, '?', out string queryY);
        if (pathX.Length > 1 && pathY.Length > 1)
        {
            pathX = RemoveTrailingSlash(pathX);
            pathY = RemoveTrailingSlash(pathY);
        }
        if ((diff = string.Compare(pathX, pathY, StringComparison.OrdinalIgnoreCase)) != 0) return diff;
        if (queryX.Length == 0) return (queryY.Length > 0) ? -1 : 0;
        if (queryY.Length == 0) return 1;
        queryX = SplitAtIndexOf(queryX, '#', out pathX);
        queryY = SplitAtIndexOf(queryY, '#', out pathY);
        return ((diff = string.Compare(queryX, queryY, StringComparison.OrdinalIgnoreCase)) != 0) ? diff : string.Compare(pathX, pathY, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(Uri? x, Uri? y)
    {
        if (x is null) return y is null;
        if (y is null) return false;
        if (ReferenceEquals(x, y)) return true;
        string pathX, pathY;
        if (x.IsAbsoluteUri)
        {
            if (!y.IsAbsoluteUri) return false;
            if (string.Equals(x.GetComponents(UriComponents.Scheme, UriFormat.Unescaped), y.GetComponents(UriComponents.Scheme, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.GetComponents(UriComponents.Host, UriFormat.Unescaped), y.GetComponents(UriComponents.Host, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.GetComponents(UriComponents.Port, UriFormat.Unescaped), y.GetComponents(UriComponents.Port, UriFormat.Unescaped), StringComparison.OrdinalIgnoreCase))
            {
                pathX = x.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                pathY = y.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                if (pathX.Length > 1 && pathY.Length > 1)
                {
                    pathX = RemoveTrailingSlash(pathX);
                    pathY = RemoveTrailingSlash(pathY);
                }
                if (string.Equals(pathX, pathY, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.GetComponents(UriComponents.Query, UriFormat.Unescaped), y.GetComponents(UriComponents.Query, UriFormat.Unescaped)))
                    return string.Equals(x.GetComponents(UriComponents.Fragment, UriFormat.Unescaped), y.GetComponents(UriComponents.Fragment, UriFormat.Unescaped));
            }
            return false;
        }
        if (y.IsAbsoluteUri) return false;
        pathX = x.OriginalString;
        pathY = y.OriginalString;
        if (pathX.Length == 0)
            return pathY.Length == 0;
        if (pathY.Length == 0) return false;
        pathX = SplitAtIndexOf(pathX, '?', out string queryX);
        pathY = SplitAtIndexOf(pathY, '?', out string queryY);
        if (pathX.Length > 1 && pathY.Length > 1)
        {
            pathX = RemoveTrailingSlash(pathX);
            pathY = RemoveTrailingSlash(pathY);
        }
        if (!string.Equals(pathX, pathY, StringComparison.OrdinalIgnoreCase)) return false;
        if (queryX.Length == 0) return (queryY.Length == 0);
        if (queryY.Length == 0) return false;
        queryX = SplitAtIndexOf(queryX, '#', out pathX);
        queryY = SplitAtIndexOf(queryY, '#', out pathY);
        return string.Equals(queryX, queryY, StringComparison.OrdinalIgnoreCase) && string.Equals(pathX, pathY, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode([DisallowNull] Uri obj)
    {
        if (obj is null) return 0;
        string path;
        HashCode hashCode = new();
        if (obj.IsAbsoluteUri)
        {
            hashCode.Add(obj.GetComponents(UriComponents.Scheme, UriFormat.Unescaped), StringComparer.OrdinalIgnoreCase);
            hashCode.Add(obj.GetComponents(UriComponents.Host, UriFormat.Unescaped), StringComparer.OrdinalIgnoreCase);
            hashCode.Add(obj.GetComponents(UriComponents.Port, UriFormat.Unescaped), StringComparer.OrdinalIgnoreCase);
            path = obj.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
            hashCode.Add((path.Length > 1) ? RemoveTrailingSlash(path) : path, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(obj.GetComponents(UriComponents.Query, UriFormat.UriEscaped), StringComparer.OrdinalIgnoreCase);
            hashCode.Add(obj.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped), StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            hashCode.Add(string.Empty, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(string.Empty, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(string.Empty, StringComparer.OrdinalIgnoreCase);
            path = SplitAtIndexOf(obj.OriginalString, '?', out string query);
            hashCode.Add((path.Length > 1) ? RemoveTrailingSlash(path) : path, StringComparer.OrdinalIgnoreCase);
            query = SplitAtIndexOf(query, '?', out string frag);
            hashCode.Add(query, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(frag, StringComparer.OrdinalIgnoreCase);
        }
        return hashCode.ToHashCode();
    }
}
