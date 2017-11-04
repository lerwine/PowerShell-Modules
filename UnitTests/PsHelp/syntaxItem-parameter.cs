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
            public static class parameter
            {
                //        <command:parameter globbing="false" pipelineInput="true (ByValue)" required="true" position="named" variableLength="false">
                //          <maml:name>InputText</maml:name>
                //          <maml:description>
                //            <maml:para>Plain text to add to PromptBuilder object.</maml:para>
                //          </maml:description>
                //          <command:parameterValue required="true" variableLength="false">string</command:parameterValue>
                //        </command:parameter>
                //            writer.WriteElement(commandPrefix, "parameter", ModuleConformance.ModuleValidator.XmlnsURI_command, () =>
                //            {
                //                string globbing;
                //                if (element == null || (globbing = element.GetAttribute("globbing")) == null || (globbing = globbing.Trim()).Length == 0)
                //                    globbing = "false";
                //                writer.WriteAttributeString("globbing", globbing);
                //                if (parameter.ValueFromPipelineByPropertyName)
                //                {
                //                    if (parameter.ValueFromPipeline)
                //                        writer.WriteAttributeString("pipelineInput", "true (ByValue)");
                //                    else
                //                        writer.WriteAttributeString("pipelineInput", "true (ByValue)");
                //                }
                //                else
                //                    writer.WriteAttributeString("pipelineInput", (parameter.ValueFromPipeline) ? "true" : "false");
                //                writer.WriteAttributeString("required", (parameter.IsMandatory) ? "true" : "false");
                //                writer.WriteAttributeString("position", (parameter.Position < 0) ? "named" : parameter.Position.ToString());
                //                writer.WriteAttributeString("variableLength", (parameter.ValueFromRemainingArguments) ? "true" : "false");
                //                writer.WriteElementString(mamlPrefix, "name", ModuleConformance.ModuleValidator.XmlnsURI_maml, parameter.Name);
                //                XmlElement d = element.GetElements("description", ModuleConformance.ModuleValidator.XmlnsURI_maml).FirstOrDefault(e => !e.GetElements().Any());
                //                if (d != null)
                //                    writer.WriteFromElement(d);
                //                else
                //                    writer.WriteElement(mamlPrefix, "description", ModuleConformance.ModuleValidator.XmlnsURI_maml, () =>
                //                    {
                //                        writer.WriteElement(mamlPrefix, "para", ModuleConformance.ModuleValidator.XmlnsURI_maml, () =>
                //                        {
                //                            writer.WriteComment("Parameter description goes here.");
                //                        });
                //                    });
                //            });
            }
        }
    }
}
