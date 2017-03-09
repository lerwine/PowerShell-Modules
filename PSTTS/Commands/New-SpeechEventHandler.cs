using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSTTS.Commands
{
    [Cmdlet(VerbsCommon.New, "SpeechEventHandler")]
    public class New_SpeechEventHandler : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            WriteObject(new SpeechEventHandler());
        }
    }
}
