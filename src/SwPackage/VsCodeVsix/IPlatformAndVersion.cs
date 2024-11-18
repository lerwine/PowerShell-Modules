using System.Management.Automation;

namespace SwPackage.VsCodeVsix;

public interface IPlatformAndVersion : IEquatable<IPlatformAndVersion>, IComparable<IPlatformAndVersion>
{
    SemanticVersion Version { get; }
    TargetPlatform Platform { get; }
}