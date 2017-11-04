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
        public static class parameter
        {
            internal static void UpdateParameters(CmdletInfo cmdletInfo, PSObject cmdHelpObj, XmlElement parametersElement)
            {
                //public static void WriteHelp(this ParameterMetadata parameter, PSObject helpInfo, CmdletInfo cmdlet, XmlElement element, XmlWriter writer)
                //{
                //    /*
                //        <command:parameter globbing="false" pipelineInput="false" required="true" position="named" variableLength="false">
                //        <maml:name>PromptBuilder</maml:name>
                //        <maml:description>
                //            <maml:para>PromptBuilder to append to. If not specified, a new PromptBuilder will be created.</maml:para>
                //        </maml:description>
                //        <dev:type>
                //            <maml:name>System.Speech.Synthesis.PromptBuilder</maml:name>
                //            <maml:uri>https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.aspx</maml:uri>
                //        </dev:type>
                //        </command:parameter>
                //     */
                //}
            }
        }
    }
}