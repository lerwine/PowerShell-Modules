using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace SwPackage;

public partial class Pep440Version : IEquatable<Pep440Version>, IComparable<Pep440Version>, IComparable
{
    private int? _epoch = null;
    public int? Epoch
    {
        get => _epoch;
        set
        {
            if (value.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(value.Value, nameof(value));
            _epoch = value;
        }
    }

    public Collection<int> Release { get; init; }

    public Pep440PreRelease? PreRelease { get; set; }

    private int? _postRelease = null;
    public int? PostRelease
    {
        get => _postRelease;
        set
        {
            
            if (value.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(value.Value, nameof(value));
            _postRelease = value;
        }
    }

    private int? _development = null;
    public int? Development
    {
        get => _development;
        set
        {
            if (value.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(value.Value, nameof(value));
            _development = value;
        }
    }

    private string? _localLabel = null;
    public string? LocalLabel
    {
        get => _localLabel;
        set
        {
            if (string.IsNullOrEmpty(value) || LocalLabelPattern.IsMatch(value))
                _localLabel = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public static readonly Regex Pep440Pattern = new(@"^(?:(?<epoch>\d+)!)?(?<release>\d+(?:\.\d+)*)(?:[_.-]?(?:(?<a>a(?:lpha)?)|(?<b>b(?:eta)?)|(?<c>r?c)|(?<pre>pre(?:view)?))[_.-]?(?<pre_n>\d*))?(?:-(?<post_n1>\d+)|[_.-]?(?:(?:post|r(?:ev)?)[_.-]?)(?<post_n2>\d*))?(?:[_.-]?dev[_.-]?(?<dev>\d*))?(?:\+(?<local>[\w.-]*))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static readonly Regex LocalLabelPattern = new(@"^[\w.-]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Pep440Version(params int[] release)
    {
        if (release is null || release.Length == 0)
            Release = new ReleaseCollection();
        else
        {
            try
            {
                Release = new ReleaseCollection(release[0]);
                for (int i = 1; i < release.Length; i++)
                    Release.Add(release[i]);
            }
            catch { throw new ArgumentOutOfRangeException(nameof(release)); }
        }
    }

    public Pep440Version(int? epoch, Pep440PreRelease? preRelease, int? postRelease, int? development, int release, params int[] additionalRelease)
    {
        if (epoch.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(epoch.Value, nameof(epoch));
        if (postRelease.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(postRelease.Value, nameof(postRelease));
        if (development.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(development.Value, nameof(development));
        try { Release = new ReleaseCollection(release); }
        catch { throw new ArgumentOutOfRangeException(nameof(release)); }
        if (additionalRelease is not null)
            foreach (int r in additionalRelease)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(r, nameof(additionalRelease));
                Release.Add(r);
            }
        _epoch = epoch;
        PreRelease = preRelease;
        _postRelease = postRelease;
        _development = development;
    }

    public Pep440Version(int? epoch, int[] release, Pep440PreRelease? preRelease, int? postRelease = null, int? development = null)
    {
        if (epoch.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(epoch.Value, nameof(epoch));
        if (null == release)
            throw new ArgumentNullException(nameof(release));
        if (release.Length == 0)
            throw new ArgumentException("Must have at least one release value", nameof(release));
        if (postRelease.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(postRelease.Value, nameof(postRelease));
        if (development.HasValue) ArgumentOutOfRangeException.ThrowIfNegative(development.Value, nameof(development));
        try
        {
            Release = new ReleaseCollection(release[0]);
            for (int i = 1; i < release.Length; i++)
                Release.Add(release[i]);
        }
        catch { throw new ArgumentOutOfRangeException(nameof(release)); }
        _epoch = epoch;
        PreRelease = preRelease;
        _postRelease = postRelease;
        _development = development;
    }

    private Pep440Version(int initialValue) { Release = new ReleaseCollection(initialValue); }

    public int CompareTo(Pep440Version? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        int? x = _epoch;
        int? y = other._epoch;
        int i;
        if (x.HasValue)
        {
            if (!y.HasValue) return 1;
            if ((i = x.Value.CompareTo(y.Value)) != 0) return i;
        }
        else if (y.HasValue)
            return -1;

        if ((i = ((ReleaseCollection)Release).CompareTo(other.Release)) != 0) return i;
        Pep440PreRelease? pX = PreRelease;
        Pep440PreRelease? pY = other.PreRelease;
        if (pX is not null)
        {
            if (pY is null) return 1;
            if ((i = pX.CompareTo(pY)) != 0) return i;
        }
        else if (pY is not null)
            return -1;
        x = _postRelease;
        y = other._postRelease;
        if (x.HasValue)
        {
            if (!y.HasValue) return 1;
            if ((i = x.Value.CompareTo(y.Value)) != 0) return i;
        }
        else if (y.HasValue)
            return -1;
        x = _development;
        y = other._development;
        if (x.HasValue)
        {
            if (!y.HasValue) return 1;
            if ((i = x.Value.CompareTo(y.Value)) != 0) return i;
        }
        else if (y.HasValue)
            return -1;
        string? lX = _localLabel;
        string? lY = other._localLabel;
        if (lX is not null)
            return (lY is null) ? 1 : string.Compare(lX, lY, StringComparison.OrdinalIgnoreCase);
        return (lY is not null) ? -1 : 0;
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is Pep440PreRelease release) return CompareTo(release);
        string? s = (obj is string v) ? v : obj.ToString();
        if (string.IsNullOrWhiteSpace(s)) return 1;
        return TryParse(s, out Pep440Version? other) ? CompareTo(other) : string.Compare(ToString(), s, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(Pep440Version? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        int? x = _epoch;
        int? y = other._epoch;
        if (x.HasValue ? (!y.HasValue || x.Value != y.Value) : y.HasValue) return false;
        Pep440PreRelease? pX = PreRelease;
        Pep440PreRelease? pY = other.PreRelease;
        if ((pX is null) ? pY is not null : !pX.Equals(pY)) return false;
        x = _postRelease;
        y = other._postRelease;
        if (x.HasValue ? (!y.HasValue || x.Value != y.Value) : y.HasValue) return false;
        x = _development;
        y = other._development;
        if (x.HasValue ? (!y.HasValue || x.Value != y.Value) : y.HasValue) return false;
        string? lX = _localLabel;
        string? lY = other._localLabel;
        return ((lX is null) ? lY is null : lY is not null && string.Equals(lX, lY, StringComparison.OrdinalIgnoreCase)) && Release.Equals(other.Release);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is Pep440PreRelease release) return Equals(release);
        string? s = (obj is string v) ? v : obj.ToString();
        if (string.IsNullOrWhiteSpace(s)) return false;
        return TryParse(s, out Pep440Version? other) ? Equals(other) : string.Equals(ToString(), s, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => HashCode.Combine(_epoch, Release, PreRelease, _postRelease, _development, _localLabel);

    public bool IsFinal() { return PreRelease is null && !(_postRelease.HasValue || _development.HasValue) && _localLabel is null; }

    public override string ToString()
    {
        StringBuilder sb = new();
        int? i = _epoch;
        if (i.HasValue)
            sb.Append(i.Value).Append('!');
        int[] values = [.. Release];
        int e = values.Length - 1;
        if (e < 0)
            sb.Append('0');
        else
        {
            sb.Append(values[0]);
            if (e > 0)
            {
                if (values[e] == 0)
                    do { e--; } while (e > 0 && values[e] == 0);
                for (var n = 1; n <= e; n++)
                    sb.Append('.').Append(e);
            }
        }
        Pep440PreRelease? preRelease = PreRelease;
        if (preRelease is not null)
        {
            switch (preRelease.Cycle)
            {
                case Pep440PreReleaseCycle.Beta:
                    sb.Append('b');
                    break;
                case Pep440PreReleaseCycle.ReleaseCandidate:
                    sb.Append("rc");
                    break;
                case Pep440PreReleaseCycle.Preview:
                    sb.Append("pre");
                    break;
                default:
                    sb.Append('a');
                    break;
            }
            sb.Append(preRelease.Number);
        }
        if ((i = _postRelease).HasValue)
            sb.Append(".post").Append(i.Value);
        if ((i = _development).HasValue)
            sb.Append(".dev").Append(i.Value);
        string? localLabel = _localLabel;
        if (localLabel is not null)
        {
            sb.Append('+');
            if (localLabel.Length > 0)
                sb.Append(localLabel);
        }
        return sb.ToString();
    }

    public static bool TryParse(string versionString, [MaybeNullWhen(false)] out Pep440Version? result)
    {
        if (!string.IsNullOrEmpty(versionString))
        {
            Match match = Pep440Pattern.Match(versionString);
            if (match.Success)
            {
                string[] release = match.Groups["release"].Value.Split('.');
                if (int.TryParse(release[0], out int i))
                {
                    result = new Pep440Version(i);
                    for (int r = 1; r < release.Length; r++)
                    {
                        if (int.TryParse(release[r], out i))
                            result.Release.Add(i);
                        else
                            return false;
                    }
                    Group g = match.Groups["epoch"];
                    if (g.Success)
                    {
                        if (int.TryParse(g.Value, out i))
                            result._epoch = i;
                        else
                            return false;
                    }
                    if ((g = match.Groups["pre_n"]).Success)
                    {
                        if (g.Length == 0)
                            result.PreRelease = new Pep440PreRelease(match.Groups["b"].Success ? Pep440PreReleaseCycle.Beta : match.Groups["c"].Success ? Pep440PreReleaseCycle.ReleaseCandidate :
                                match.Groups["pre"].Success ? Pep440PreReleaseCycle.Preview : Pep440PreReleaseCycle.Alpha, 0);
                        else if (int.TryParse(g.Value, out i))
                            result.PreRelease = new Pep440PreRelease(match.Groups["b"].Success ? Pep440PreReleaseCycle.Beta : match.Groups["c"].Success ? Pep440PreReleaseCycle.ReleaseCandidate :
                                match.Groups["pre"].Success ? Pep440PreReleaseCycle.Preview : Pep440PreReleaseCycle.Alpha, i);
                        else
                            return false;
                    }
                    if ((g = match.Groups["post_n1"]).Success)
                    {
                        if (g.Length == 0)
                            result._postRelease = 0;
                        else if (int.TryParse(g.Value, out i))
                            result._postRelease = i;
                        else
                            return false;
                    }
                    else if ((g = match.Groups["post_n2"]).Success)
                    {
                        if (g.Length == 0)
                            result._postRelease = 0;
                        else if (int.TryParse(g.Value, out i))
                            result._postRelease = i;
                        else
                            return false;
                    }
                    if ((g = match.Groups["dev"]).Success)
                    {
                        if (g.Length == 0)
                            result._development = 0;
                        else if (int.TryParse(g.Value, out i))
                            result._development = i;
                        else
                            return false;
                    }
                    if ((g = match.Groups["local"]).Success)
                        result._localLabel = g.Value;
                    return true;
                }
            }
        }
        result = null;
        return false;
    }
}
