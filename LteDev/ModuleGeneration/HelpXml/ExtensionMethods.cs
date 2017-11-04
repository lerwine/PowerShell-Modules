using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class ExtensionMethods
    {
        public static readonly string[] InlineGroupParentElements = new string[] { "para", "alertSet", "quote", "parameterizedBlock" };

        public static string HelpDocPath(this Assembly assembly)
        {
            if (assembly == null || String.IsNullOrEmpty(assembly.Location))
                return null;

            return Path.Combine(Path.GetDirectoryName(assembly.Location), Path.GetFileNameWithoutExtension(assembly.Location) + ".XML");
        }

        public static string FullRootModulePath(this PSModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                return null;

            if (Path.IsPathRooted(moduleInfo.RootModule))
                return moduleInfo.RootModule;

            if (String.IsNullOrEmpty(moduleInfo.ModuleBase))
            {
                if (String.IsNullOrEmpty(moduleInfo.Path))
                    return null;

                return Path.Combine(Path.GetDirectoryName(moduleInfo.Path), moduleInfo.RootModule);
            }
            return Path.Combine(moduleInfo.ModuleBase, moduleInfo.RootModule);
        }

        public static string PSHelpXmlPath(this PSModuleInfo moduleInfo)
        {
            string path = moduleInfo.FullRootModulePath();
            if (path == null)
                return null;

            throw new NotImplementedException();
            //return path + PSHelpFactory.PSMaml.HelpXml_FileName_Append;
        }

        public static XElement FirstChildElement(this XElement element)
        {
            if (element == null || !element.HasElements)
                return null;

            return element.Elements().First();
        }

        public static XElement NextSiblingElement(this XElement element)
        {
            if (element == null)
                return null;

            for (XNode n = element.NextNode; n != null; n = n.NextNode)
            {
                if (n is XElement)
                    return n as XElement;
            }

            return null;
        }

        public static IEnumerable<string> GetTextNodeValues(this XElement element)
        {
            if (element == null || element.IsEmpty)
                yield break;

            foreach (XNode node in element.Nodes())
            {
                if (node is XText)
                    yield return (node as XText).Value;
                else if (node is XCData)
                    yield return (node as XCData).Value;
            }
        }

        public static IEnumerable<string> GetCommentNodeValues(this XElement element)
        {
            if (element == null || element.IsEmpty)
                return new string[0];

            return element.Nodes().OfType<XComment>().Select(c => c.Value);
        }

        public static XElement CreateClone(this XElement element)
        {
            if (element == null)
                return null;

            using (XmlReader reader = element.CreateReader())
                return XDocument.Load(reader).Root;
        }

        public static IEnumerable<XNode> CloneContents(this XElement element)
        {
            if (element == null || element.IsEmpty)
                return new XNode[0];

            return element.CreateClone().Nodes();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
