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
    public class ProjectNode : SolutionNode
    {
        #region Constants

        #endregion
    
        #region Fields
 
        private Collection<SolutionNode> _nodes = new Collection<SolutionNode>();

        #endregion

        #region Properties

        public Collection<SolutionNode> Nodes { get { return _nodes; } }

        #endregion

        #region Constructors

        public ProjectNode(Guid projectGuid, Guid typeGuid, string projectName, string projectPath, string otherProperties,
            string content)
        {

        }
        
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

