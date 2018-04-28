using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace IOUtility
{
    /// <summary>
    /// Uses a <see cref="ScriptBlock" /> to replace matching text.
    /// </summary>
    public class ScriptRegexReplaceHandler : RegexReplaceHandler
    {
        private ScriptBlock _scriptBlock;

        /// <summary>
        /// <see cref="ScriptBlock" /> which gets invoked when a match occurs.
        /// </summary>
        public ScriptBlock ScriptBlock { get { return this._scriptBlock; } }

        /// <summary>
        /// Initialize new <see cref="ScriptRegexReplaceHandler" />.
        /// </summary>
        /// <param name="pattern">Regular Expression pattern of text to encode.</param>
        /// <param name="options">Regular expression pattern options.</param>
        /// <param name="scriptBlock"><see cref="ScriptBlock" /> which gets invoked when a match occurs.</param>
        public ScriptRegexReplaceHandler(string pattern, RegexOptions options, ScriptBlock scriptBlock) : base(pattern, options)
        {
            if (scriptBlock == null)
                throw new ArgumentNullException("scriptBlock");

            this._scriptBlock = scriptBlock;
        }

        /// <summary>
        /// Initialize new <see cref="ScriptRegexReplaceHandler" />.
        /// </summary>
        /// <param name="pattern">Regular Expression pattern of text to encode.</param>
        /// <param name="scriptBlock"><see cref="ScriptBlock" /> which gets invoked when a match occurs.</param>
        public ScriptRegexReplaceHandler(string pattern, ScriptBlock scriptBlock) : base(pattern)
        {
            if (scriptBlock == null)
                throw new ArgumentNullException("scriptBlock");

            this._scriptBlock = scriptBlock;
        }

        /// <summary>
        /// Initialize new <see cref="ScriptRegexReplaceHandler" />.
        /// </summary>
        /// <param name="regex">Regular Expression object which matches text to encode.</param>
        /// <param name="scriptBlock"><see cref="ScriptBlock" /> which gets invoked when a match occurs.</param>
        public ScriptRegexReplaceHandler(Regex regex, ScriptBlock scriptBlock) : base(regex)
        {
            if (scriptBlock == null)
                throw new ArgumentNullException("scriptBlock");

            this._scriptBlock = scriptBlock;
        }

        /// <summary>
        /// Returns replaced text.
        /// </summary>
        /// <param name="match">Current regular expression match to be replaced.</param>
        /// <returns>Text which has been replaced by <see cref="ScriptBlock" />.</returns>
        protected override string Evaluator(Match match)
        {
            Collection<PSObject> output = this._scriptBlock.Invoke(match, this.Regex);
            if (output.Count == 1)
            {
                if (output[0] == null || output[0].BaseObject == null)
                    return "";

                if (output[0].BaseObject is string)
                    return output[0].BaseObject as string;

                return output[0].BaseObject.ToString();
            }
            StringBuilder sb = new StringBuilder();
            foreach (PSObject obj in output)
            {
                if (obj == null || obj.BaseObject == null)
                    continue;
                if (sb.Length > 0)
                    sb.AppendLine();
                if (obj.BaseObject is string)
                    sb.Append(obj.BaseObject as string);
                else
                    sb.Append(obj.BaseObject.ToString());
            }
            return sb.ToString();
        }
    }
}
