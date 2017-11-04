using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlUtility.Commands
{
    [Cmdlet(VerbsCommon.Open, "XmlSchema")]
    public class Open_XmlSchema : Cmdlet
    {
        public const string ParameterSetName_Glob = "Glob";
        public const string ParameterSetName_FromPipeline = "FromPipeline";
        public const string ParameterSetName_Literal = "Literal";
        private XmlReaderSettings _readerSettings;
        private SchemaValidationRelay _validationRelay;

        [Parameter(HelpMessage = "Path of XML schema file to open", Mandatory = true, ParameterSetName = ParameterSetName_FromPipeline, ValueFromPipeline = true)]
        public string InputPath { get; set; }

        [Parameter(HelpMessage = "Path of XML schema file to open", Mandatory = true, ParameterSetName = ParameterSetName_Glob)]
        public string Path { get; set; }

        [Parameter(HelpMessage = "Literal Path of XML schema file to open", Mandatory = true, ParameterSetName = ParameterSetName_Literal)]
        public string LiteralPath { get; set; }

        [Parameter(HelpMessage = "Settings to use when reading from XML file.")]
        public XmlReaderSettings ReaderSettings { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _readerSettings = (ReaderSettings == null) ? new XmlReaderSettings() : ReaderSettings.Clone();
            _validationRelay = new SchemaValidationRelay(this);
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Stopping)
                return;

        }
    }
}
