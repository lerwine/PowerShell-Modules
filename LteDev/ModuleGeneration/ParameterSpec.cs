using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.ModuleGeneration
{
    [Serializable]
    public class ParameterSpec
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string TypeName { get; set; }

        [XmlElement("ParameterSet")]
        public List<ParameterSetSpec> ParameterSets { get; set; }

        public IEnumerable<string> GetParameterSetNames()
        {
            if (ParameterSets == null)
                return new string[] { "" };

            return ParameterSets.Where(p => p != null).Select(p => (p.Name == null) ? "" : p.Name.Trim()).DefaultIfEmpty("");
        }

        public void WriteSyntaxItem(XmlWriter writer, string parameterSetName)
        {
            ParameterSetSpec parameterSet;

            if (ParameterSets == null)
                parameterSet = null;
            else if (String.IsNullOrWhiteSpace(parameterSetName))
                parameterSet = ParameterSets.FirstOrDefault(p => String.IsNullOrWhiteSpace(p.Name));
            else
            {
                parameterSetName = parameterSetName.Trim();
                parameterSet = ParameterSets.FirstOrDefault(p => p.Name != null && String.Equals(p.Name.Trim(), parameterSetName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (parameterSet == null)
            {
                if (ParameterSets != null && !ParameterSets.Any(p => p != null && !String.IsNullOrWhiteSpace(p.Name)))
                    return;
                parameterSet = new ParameterSetSpec();
            }

            throw new NotImplementedException();
        }

        public void WriteParameter(XmlWriter writer)
        {
            ParameterSetSpec parameterSet = (ParameterSets == null) ? null : ParameterSets.FirstOrDefault(p => p != null && !String.IsNullOrWhiteSpace(p.Name));
            if (parameterSet == null)
                parameterSet = new ParameterSetSpec();

            throw new NotImplementedException();
        }

        public bool HasInputType()
        {
            return ParameterSets != null && ParameterSets.Any(p => p != null && !String.IsNullOrWhiteSpace(p.Name) && (p.ValueFromPipeline || p.ValueFromPipelineByPropertyName));
        }

        public void WriteInputTypes(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}