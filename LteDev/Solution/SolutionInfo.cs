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
#pragma warning disable  1591 // Missing XML comment for publicly visible type or member    
    public class SolutionInfo
    {
        #region Constants

        public static readonly Regex NodesRegex = new Regex(@"(^[\r\n]*|\G[\r\n]+)");

        public static readonly Regex ProjectPattern = new Regex(@"^[\r\n]*Project\(\""\{(?<typeGuid>[\da-f]{8}(-[\da-f]{4}){4}[\da-f]{8})\}\""\)[ \t]*=[ \t]*\""(?<projectName>[^\""]*)\""[ \t]*,[ \t]*\""(?<projectPath>[^\""]*)\""[ \t]*,[ \t]*\""\{(?<projectGuid>[\da-f]{8}(-[\da-f]{4}){4}[\da-f]{8})\}\""(?<otherProperties>[^\r\n]+)?(?<content>([\r\n]+(?!EndProject)[^\r\n]*)*)(\r\n?|\n)EndProject");

        public static readonly Regex GlobalPattern = new Regex(@"^[\r\n]*Global(?<properties>[^\r\n]+)?(?<globalContent>([\r\n]+(?!EndGlobal)[^\r\n]*)*)(\r\n?|\n)EndGlobal");

        public static readonly Regex KeyValuePairPattern = new Regex(@"^[\r\n]*(?<key>[^=\s]+)[ \t]*=[ \t]*(\""(?<value>[^""]*)\""|(?<value>[^\s\r\n]*([ \t]+[^\s\r\n]+)*))");

        public static readonly Regex OtherPattern = new Regex(@"^[\r\n]*(?<other>[^\r\n]+)");

        #endregion

        public static SolutionInfo Parse(string contents)
        {
            SolutionInfo result = new SolutionInfo();
            if (String.IsNullOrEmpty(contents))
                return result;
            int start = 0;
            string other = "";
            while (start < contents.Length)
            {
                Match m = ProjectPattern.Match(contents, start);
                if (m.Success)
                {
                    result._nodes.Add(new ProjectNode(Guid.Parse(m.Groups["projectGuid"].Value), Guid.Parse(m.Groups["typeGuid"].Value), m.Groups["projectName"].Value,
                        m.Groups["projectPath"].Value, m.Groups["otherProperties"].Value, m.Groups["content"].Value))
                }
            }
        }
        #region Fields
 
        private Collection<SolutionNode> _nodes = new Collection<SolutionNode>();

        #endregion

        #region Properties

        public Collection<SolutionNode> Nodes { get { return _nodes; } }

        #endregion

        #region Constructors


        #endregion

        #region Operators


        #endregion

        #region Methods

        public static SolutionInfo Load(string path)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        #endregion
    }
}
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

