Add-Type -TypeDefinition @'
namespace TypeGeneratorHelper
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class TypeParser
    {
        public static readonly Regex TypeNameParseRegex = new Regex(@"(^|\G)(?<b>\[)?(?<fn>((?<ns><[\w-]+>|[\w-]+(\.[\w-]+(?=\.))*)\.)?((?<dt>[\w-]+)\+)?(?<name>(<[\w-]+>)+|(<>)?[\w?$@-]+(<(\w+::)+\w+(\s*(\^|\\\*))?>|\{\d+\})?))(`(?<gCount>\d+))?(\[(?<gArgs>(?=[^,\]])([^\[\],]*(?>(?<g>\[)[^\[\]]*)*(?>(?<-g>\])[^\[\]]*)*(?(g)(?!))(\[[,\s]*\]|\*|&)*(\+|,\s*)?)*)\])?(?<indexer>(\[[,\s]*\]|\*|&)+)?(,\s*[\w-]+(\.[\w-]+)*(,\s*\w+=[\w.]+)+)?(?<-b>\])?(?(b)(?!))((?<j>\+|,\s*)|$)", RegexOptions.Compiled);

        public static string ToCSharpTypeName(string typeName, string[] usings)
        {
            if (usings == null)
                usings = new string[0];
            else
                usings = usings.Where(s => s != null).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
            if (typeName == null || (typeName = typeName.Trim()).Length == 0)
                return "";
            
            return _ToCSharpTypeName(typeName, usings);
        }

        private static string _ToCSharpTypeName(string typeName, params string[] usings)
        {
            MatchCollection matches = TypeNameParseRegex.Matches(typeName);
            if (matches.Count == 0)
                return typeName;
            StringBuilder result = new StringBuilder();
            foreach (Match m in matches)
            {
                switch (m.Groups["fn"].Value)
                {
                    case "System.String":
                        result.Append("string");
                        break;
                    case "System.Boolean":
                        result.Append("bool");
                        break;
                    case "System.Decimal":
                        result.Append("decimal");
                        break;
                    case "System.Double":
                        result.Append("double");
                        break;
                    case "System.Single":
                        result.Append("float");
                        break;
                    case "System.Char":
                        result.Append("char");
                        break;
                    case "System.Byte":
                        result.Append("byte");
                        break;
                    case "System.SByte":
                        result.Append("sbyte");
                        break;
                    case "System.Int16":
                        result.Append("short");
                        break;
                    case "System.Int32":
                        result.Append("int");
                        break;
                    case "System.Int64":
                        result.Append("long");
                        break;
                    case "System.UInt16":
                        result.Append("ushort");
                        break;
                    case "System.UInt32":
                        result.Append("uint");
                        break;
                    case "System.UInt64":
                        result.Append("ulong");
                        break;
                    default:
                        if (m.Groups["ns"].Success)
                        {
                            string ns = m.Groups["ns"].Value;
                            if (!usings.Any(s => s == ns))
                                result.Append(ns).Append(".");
                        }
                        if (m.Groups["dt"].Success)
                            result.Append(m.Groups["dt"].Value).Append('.');
                        result.Append(m.Groups["name"].Value);
                        int c;
                        if (m.Groups["gArgs"].Success)
                            result.Append('<').Append(_ToCSharpTypeName(m.Groups["gArgs"].Value, usings)).Append('>');
                        else if (m.Groups["gCount"].Success && int.TryParse(m.Groups["gCount"].Value, out c))
                        {
                            result.Append('<');
                            if (c == 1)
                                result.Append(',');
                            else if (c > 1)
                                result.Append(',', c - 1);
                            result.Append('>');
                        }
                        break;
                }
                if (m.Groups["indexer"].Success)
                    result.Append(m.Groups["indexer"].Value);
                if (m.Groups["j"].Success)
                    result.Append(m.Groups["j"].Value);
            }
            Group grp = matches[matches.Count - 1];
            int idx = grp.Index + grp.Length;
            if (idx < typeName.Length)
                result.Append(typeName.Substring(idx));
            return result.ToString();
        }

        public static string ToPSTypeName(string typeName)
        {
            if (typeName == null || (typeName = typeName.Trim()).Length == 0)
                return "";
            
            string s = LanguagePrimitives.ConvertTypeNameToPSTypeName(_ToPSTypeName(typeName));
            return s.Substring(1, s.Length - 2);
        }

        private static string _ToPSTypeName(string typeName)
        {   
            MatchCollection matches = TypeNameParseRegex.Matches(typeName);
            if (matches.Count == 0)
                return typeName;
            StringBuilder result = new StringBuilder();
            foreach (Match m in matches)
            {
                if (m.Groups["ns"].Success)
                    result.Append(m.Groups["ns"].Value).Append(".");
                if (m.Groups["dt"].Success)
                    result.Append(m.Groups["dt"].Value).Append('+');
                result.Append(m.Groups["name"].Value);
                if (m.Groups["gArgs"].Success)
                    result.Append('[').Append(_ToPSTypeName(m.Groups["gArgs"].Value)).Append(']');
                else if (m.Groups["gCount"].Success)
                    result.Append('`').Append(m.Groups["gCount"].Value);
                if (m.Groups["indexer"].Success)
                    result.Append(m.Groups["indexer"].Value);
                if (m.Groups["j"].Success)
                    result.Append(m.Groups["j"].Value);
            }
            Group grp = matches[matches.Count - 1];
            int idx = grp.Index + grp.Length;
            if (idx < typeName.Length)
                result.Append(typeName.Substring(idx));
            return result.ToString();
        }
    }
}
'@

$ReadOnlyTemplate = @'

        #region {0} Property Members

        /// <summary>
        /// Defines the name for the <see cref="{0}"/> dependency property.
        /// </summary>
        public const string PropertyName_{0} = "{0}";

        private static readonly DependencyPropertyKey {0}PropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_{0},
            typeof({2}), typeof({1}), new PropertyMetadata());

        /// <summary>
        /// Identifies the <see cref="{0}"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;

        /// <summary>
        /// Value of the model property <seealso cref="{3}.{0}"/>.
        /// </summary>
        public {2} {0}
        {{
            get {{ return ({2})(GetValue({0}Property)); }}
            private set {{ SetValue({0}PropertyKey, value); }}
        }}

        #endregion
        // {0} = model.{0}
'@
$Using = (@'
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
'@.Replace(';', '') -split '\r\n?|\n') -replace '^using\s+', '';
$ModelType = [System.Type];
$SeeAlso = [TypeGeneratorHelper.TypeParser]::ToCSharpTypeName($ModelType.FullName, $Using);
[System.Windows.Clipboard]::SetText(($ModelType.GetProperties() | ForEach-Object {
    $ReadOnlyTemplate -f $_.Name, 'TypeInfoDataItem', [TypeGeneratorHelper.TypeParser]::ToCSharpTypeName($_.PropertyType.FullName, $Using), $SeeAlso;
} | Out-String).Trim());

