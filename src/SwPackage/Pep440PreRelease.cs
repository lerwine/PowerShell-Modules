using System.Text.RegularExpressions;

namespace SwPackage;

public partial class Pep440PreRelease : IEquatable<Pep440PreRelease>, IComparable<Pep440PreRelease>, IComparable
{
    public Pep440PreReleaseCycle Cycle { get; set; }
    private int _number;
    public int Number
    {
        get => _number;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _number = value;
        }
    }

    public static readonly Regex Pep440PrePattern = new(@"^\s*[_.-]?(?:(?<a>a(?:lpha)?)|(?<b>b(?:eta)?)|(?<c>r?c)|(?<pre>pre(?:view)?))[_.-]?(?<pre_n>\d+)?\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Pep440PreRelease() : this(Pep440PreReleaseCycle.Alpha, 0) { }

    public Pep440PreRelease(Pep440PreReleaseCycle cycle, int number)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        Cycle = cycle;
        _number = number;
    }

    public int CompareTo(Pep440PreRelease? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        int i = Cycle.CompareTo(other.Cycle);
        return (i == 0) ? _number.CompareTo(other._number) : i;
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is Pep440PreRelease release) return CompareTo(release);
        string? s = (obj is string v) ? v : obj.ToString();
        if (string.IsNullOrWhiteSpace(s)) return 1;
        Match match = Pep440PrePattern.Match(s);
        if (match.Success)
        {
            switch (Cycle)
            {
                case Pep440PreReleaseCycle.Beta:
                    if (!match.Groups["b"].Success) return match.Groups["a"].Success ? 1 : -1;
                    break;
                case Pep440PreReleaseCycle.ReleaseCandidate:
                    if (!match.Groups["c"].Success) return match.Groups["pre"].Success ? -1 : 1;
                    break;
                case Pep440PreReleaseCycle.Preview:
                    if (!match.Groups["pre"].Success) return 1;
                    break;
                default:
                    if (!match.Groups["a"].Success) return -1;
                    break;
            }
            Group g = match.Groups["pre_n"];
            if (g.Success)
            {
                if (int.TryParse(g.Value, out int i))
                    return _number.CompareTo(i);
            }
            else
                return _number.CompareTo(0);
        }
        return string.Compare(ToString(), s, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(Pep440PreRelease? other) { return other is not null && (ReferenceEquals(this, other) || Cycle == other.Cycle && _number == other._number); }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is Pep440PreRelease release) return Equals(release);
        string? s = (obj is string v) ? v : obj.ToString();
        if (string.IsNullOrWhiteSpace(s)) return false;
        Match match = Pep440PrePattern.Match(s);
        if (match.Success)
        {
            switch (Cycle)
            {
                case Pep440PreReleaseCycle.Beta:
                    if (!match.Groups["b"].Success) return false;
                    break;
                case Pep440PreReleaseCycle.ReleaseCandidate:
                    if (!match.Groups["c"].Success) return false;
                    break;
                case Pep440PreReleaseCycle.Preview:
                    if (!match.Groups["pre"].Success) return false;
                    break;
                default:
                    if (!match.Groups["a"].Success) return false;
                    break;
            }
            Group g = match.Groups["pre_n"];
            if (g.Success)
            {
                if (int.TryParse(g.Value, out int i))
                    return i == _number;
            }
            else
                return _number == 0;
        }
        return string.Equals(ToString(), s, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => HashCode.Combine(Cycle, _number);

    public override string ToString() => Cycle switch
    {
        Pep440PreReleaseCycle.Beta => $"b{_number}",
        Pep440PreReleaseCycle.ReleaseCandidate => $"rc{_number}",
        Pep440PreReleaseCycle.Preview => $"pre{_number}",
        _ => $"a{_number}"
    };
}
