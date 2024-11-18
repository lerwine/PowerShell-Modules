namespace SwPackage.VsCodeVsix;

/// <summary>
/// 
/// </summary>
/// <remarks>Was TargetVsixPlatform</remarks>
public enum TargetPlatform
{
    UNIVERSAL,
    WIN32_X64,
    WIN32_ARM64,
    WIN32_IA32,
    LINUX_X64,
    LINUX_ARM64,
    LINUX_ARMHF,
    ALPINE_X64,
    ALPINE_ARM64,
    DARWIN_X64,
    DARWIN_ARM64,
    WEB,
    UNKNOWN
}
