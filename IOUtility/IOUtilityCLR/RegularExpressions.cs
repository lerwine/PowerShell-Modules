using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace IOUtilityCLR
{
    public static class RegularExpressions
    {
        public static readonly Regex Whitespace = new Regex(@"\s+", RegexOptions.Compiled);
        public static readonly Regex WhitespaceEncode = new Regex(@"(_(?=0x[\dA-F]{4}_)|\s)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex EncodedName = new Regex(@"_0x(?<hex>[\dA-F]{4})_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex ControlChar = new Regex(@"\p{Cc}", RegexOptions.Compiled);
        public static readonly Regex InvalidFileNameChars;
        public static readonly Regex InvalidPathChars;
        public static readonly Regex InvalidRelativePathChars;
        static RegularExpressions()
        {
            RegularExpressions.InvalidFileNameChars = new Regex(String.Format(@"(_(?=0x[\dA-F]{4}_)|{0})",
                TextHelper.GetRegexPattern(null, Path.GetInvalidFileNameChars(), false, false), RegexOptions.Compiled | RegexOptions.IgnoreCase));
            List<char> invalidChars = new List<char>(Path.GetInvalidPathChars());
            invalidChars.Add(Path.PathSeparator);
            RegularExpressions.InvalidPathChars = new Regex(String.Format(@"(_(?=0x[\dA-F]{4}_)|{0})",
                TextHelper.GetRegexPattern(null, invalidChars, false, false)), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            invalidChars.Add(Path.VolumeSeparatorChar);
            RegularExpressions.InvalidRelativePathChars = new Regex(String.Format(@"(^[{0}{1}]|_(?=0x[\dA-F]{4}_)|{2})", Regex.Escape(Path.DirectorySeparatorChar.ToString()),
                Regex.Escape(Path.AltDirectorySeparatorChar.ToString()), TextHelper.GetRegexPattern(null, Path.GetInvalidPathChars(), false, false)),
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
