using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwPackage.VsCodeVsix;

public static class VsixExtensions
{
    public static bool TryConvertToTargetPlatform(this string? value, out TargetPlatform result)
    {
        if (string.IsNullOrWhiteSpace(value))
            result = TargetPlatform.UNIVERSAL;
        else
            switch (value.ToLower())
            {
                case "win32-x64":
                    result = TargetPlatform.WIN32_X64;
                    break;
                case "win32-arm64":
                    result = TargetPlatform.WIN32_ARM64;
                    break;
                case "win32-ia32":
                    result = TargetPlatform.WIN32_IA32;
                    break;
                case "win32-x32":
                    result = TargetPlatform.WIN32_IA32;
                    break;
                case "linux-x64":
                    result = TargetPlatform.LINUX_X64;
                    break;
                case "linux-arm64":
                    result = TargetPlatform.LINUX_ARM64;
                    break;
                case "linux-armhf":
                    result = TargetPlatform.LINUX_ARMHF;
                    break;
                case "linux-armf":
                    result = TargetPlatform.LINUX_ARMHF;
                    break;
                case "linux-arm":
                    result = TargetPlatform.LINUX_ARMHF;
                    break;
                case "alpine-x64":
                    result = TargetPlatform.ALPINE_X64;
                    break;
                case "alpine-arm64":
                    result = TargetPlatform.ALPINE_ARM64;
                    break;
                case "darwin-x64":
                    result = TargetPlatform.DARWIN_X64;
                    break;
                case "darwin-arm64":
                    result = TargetPlatform.DARWIN_ARM64;
                    break;
                case "web":
                    result = TargetPlatform.WEB;
                    break;
                case "universal":
                    result = TargetPlatform.UNIVERSAL;
                    break;
                default:
                    result = TargetPlatform.UNIVERSAL;
                    return false;
            }
        return true;
    }
    public static TargetPlatform ToTargetPlatform(this string? value, bool unknownAsUniversal = false)
    {
        if (TryConvertToTargetPlatform(value, out TargetPlatform result) || unknownAsUniversal)
            return result;
        throw new ArgumentOutOfRangeException(nameof(value), $"\"{value!.Replace("\\", "\\\"").Replace("\"", "\\\"")}\" is an unknown target platform");
    }

    public static bool TryGetIdentifierString(this TargetPlatform value, out string result)
    {
        switch (value)
        {
            case TargetPlatform.WIN32_X64:
                result = "win32-x64";
                break;
            case TargetPlatform.WIN32_ARM64:
                result = "win32-arm64";
                break;
            case TargetPlatform.WIN32_IA32:
                result = "win32-ia32";
                break;
            case TargetPlatform.LINUX_X64:
                result = "linux-x64";
                break;
            case TargetPlatform.LINUX_ARM64:
                result = "linux-arm64";
                break;
            case TargetPlatform.LINUX_ARMHF:
                result = "linux-armhf";
                break;
            case TargetPlatform.ALPINE_X64:
                result = "alpine-x64";
                break;
            case TargetPlatform.ALPINE_ARM64:
                result = "alpine-arm64";
                break;
            case TargetPlatform.DARWIN_X64:
                result = "darwin-x64";
                break;
            case TargetPlatform.DARWIN_ARM64:
                result = "darwin-arm64";
                break;
            case TargetPlatform.WEB:
                result = "web";
                break;
            case TargetPlatform.UNIVERSAL:
                result = string.Empty;
                break;
            default:
                result = string.Empty;
                return false;
        }
        return true;
    }

    public static string ToIdentifierString(this TargetPlatform value, bool unknownAsEmpty = false)
    {
        if (TryGetIdentifierString(value, out string result) || unknownAsEmpty)
            return result;
        throw new ArgumentOutOfRangeException(nameof(value), "Unknown target platform cannot be converted to an identifier string");
    }

    public static string GetDisplayName(this TargetPlatform value) => value switch
    {
        TargetPlatform.WIN32_X64 => "Windows 64 bit",
        TargetPlatform.WIN32_ARM64 => "Windows ARM",
        TargetPlatform.WIN32_IA32 => "Windows 32 bit",
        TargetPlatform.LINUX_X64 => "Linux 64 bit",
        TargetPlatform.LINUX_ARM64 => "Linux ARM 64",
        TargetPlatform.LINUX_ARMHF => "Linux ARM",
        TargetPlatform.ALPINE_X64 => "Alpine Linux 64 bit",
        TargetPlatform.ALPINE_ARM64 => "Alpine ARM 64",
        TargetPlatform.DARWIN_X64 => "Mac",
        TargetPlatform.DARWIN_ARM64 => "Mac Silicon",
        TargetPlatform.WEB => "Web",
        TargetPlatform.UNIVERSAL => "Universal",
        _ => "unknown",
    };
}