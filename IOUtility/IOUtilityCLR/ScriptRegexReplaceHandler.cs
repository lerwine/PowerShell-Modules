using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace IOUtilityCLR
{
    public class ScriptRegexReplaceHandler : RegexReplaceHandler
    {
        private ScriptBlock _scriptBlock;

        public ScriptRegexReplaceHandler(Regex regex, ScriptBlock scriptBlock) : base(regex)
        {
            if (scriptBlock == null)
                throw new ArgumentNullException("scriptBlock");

            this._scriptBlock = scriptBlock;
        }

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
