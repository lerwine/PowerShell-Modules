using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.PsHelp
{
    public static partial class command
    {
        public static partial class syntaxItem
        {
            public static void UpdateSyntax(CmdletInfo cmdletInfo, PSObject cmdHelpObj, XmlElement syntaxElement)
            {
                PSPropertyInfo syntaxProperty = cmdHelpObj.GetMemberProperty("Syntax");
                IEnumerable<PSObject> syntaxItemObjArray = syntaxProperty.AsPsObjectEnumerable();
                IEnumerable<XmlElement> elements = syntaxElement.GetCommandElements();
                foreach (CommandParameterSetInfo parameterSet in cmdletInfo.ParameterSets)
                {
                    //public static IEnumerable<XmlElement> WriteHelp(this CommandParameterSetInfo parameterSet, PSObject helpInfo, CmdletInfo cmdlet, XmlElement syntaxItem, XmlWriter writer)
                    //{
                    //    string commandPrefix = writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_command);
                    //    string mamlPrefix = writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_maml);
                    //    writer.WriteElement(commandPrefix, "syntaxItem", ModuleConformance.ModuleValidator.XmlnsURI_command, () =>
                    //    {
                    //        writer.WriteElementString(mamlPrefix, "name", ModuleConformance.ModuleValidator.XmlnsURI_maml, cmdlet.Name);
                    //        foreach (CommandParameterInfo parameter in parameterSet.Parameters)
                    //        {
                    //            XmlElement element = syntaxItem.GetElements("parameter", ModuleConformance.ModuleValidator.XmlnsURI_command).GetElements("name", ModuleConformance.ModuleValidator.XmlnsURI_maml)
                    //                .Where(e => !e.IsEmpty && String.Equals(e.InnerText.Trim(), parameter.Name, StringComparison.InvariantCultureIgnoreCase)).Select(e => e.ParentNode as XmlElement).FirstOrDefault();
                    //Parameter
                    //        }
                    //    });
                    //    /*
                    //      <command:syntaxItem>
                    //        <maml:name>ConvertTo-PromptBuilder</maml:name>
                    //      </command:syntaxItem>
                    //     */
                    //    throw new NotImplementedException();
                    //}
                }
            }
        }
    }
}