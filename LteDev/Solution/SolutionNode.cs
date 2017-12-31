using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LteDev.Solution
{
    public abstract class SolutionNode
    {
        #region Constants


        #endregion

        #region Fields
        
        public static readonly Regex SolutionArgsRegex = new Regex(@"(\G\s*,|^)\s*(""([^""]+|\\"")*""|[^"",\s]+)(\s+(""([^""]+|\\"")*""|[^"",\s]+))*", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        public static readonly Regex QuotedTextRegex = new Regex(@"(\G|^)((?<t>[^""]+)|""(?<t>(\\""|[^""]+)*)"")", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        
        public static readonly Regex EscapededTextRegex = new Regex(@"(\G|^)(\\(?<t>.)|[^\\]+|(?<t>\\)$) ", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        
        // i c n p a e v
        public static readonly Regex SolutionLineRegex = new Regex(@"^(?<i>\s+)?
(
    #\s*(?<c>\S+(\s+\S+)*)
    |
    (?<n>[^\(\)=\s]+(\s+[^\(\)=\s]+)*)
    \s*
    (?<p>
        \(\s*
        (
            (?<a>
                (""[^""]*(\\""[^""]*)*""|[^\s\(\)""]+)+
                (\s+(""[^""]*(\\""[^""]*)*""|[^\s\(\)""]+)+)*
            )
            \s*
        )?
        \)\s*
    )?
    \s*
    (?<e>=)?
    \s*
    (?<v>\S+(\s+\S+)*)?
    |
    (?<v>\S+(\s+\S+)*)?
)$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Properties


        #endregion

        #region Constructors


        #endregion

        #region Operators


        #endregion

        #region Methods


        #endregion
    }
}