using System;

namespace PSModuleInstallUtil.Module
{
    public interface IManifestParseError : IEquatable<IManifestParseError>, IComparable<IManifestParseError>
    {
        int EndColumn { get; }

        int EndLine { get; }

        int Length { get; }

        int StartIndex { get; }

        int StartColumn { get; }

        int StartLine { get; }
    }
}