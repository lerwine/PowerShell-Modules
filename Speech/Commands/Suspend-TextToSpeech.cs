using System.Management.Automation;
using System.Speech.Synthesis;

namespace Speech.Commands
{
    [Cmdlet(VerbsLifecycle.Suspend, "TextToSpeech", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PromptBuilder))]
    public class Suspend_TextToSpeech : TextToSpeechCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Job.Pause();
        }
    }
}
