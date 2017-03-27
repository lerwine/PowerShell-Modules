using System.Management.Automation;

namespace Speech.Commands
{
    [Cmdlet(VerbsCommon.Get, "TextToSpeechStatus", RemotingCapability = RemotingCapability.None)]
    public class Get_TextToSpeechStatus : TextToSpeechCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            WriteObject(Job.State);
        }
    }
}
