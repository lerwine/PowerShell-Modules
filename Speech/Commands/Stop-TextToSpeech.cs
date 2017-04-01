using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Speech.Commands
{
    [Cmdlet(VerbsLifecycle.Stop, "TextToSpeech", RemotingCapability = RemotingCapability.None)]
    public class Stop_TextToSpeech : TextToSpeechCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Job.AsyncCancel();
            Collection<object> output;
            if (Wait_TextToSpeech.TryWait(Job, null, false, out output))
                WriteObject(output, true);
        }
    }
}
