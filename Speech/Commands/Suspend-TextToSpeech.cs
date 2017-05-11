using System.Management.Automation;
using System.Speech.Synthesis;

namespace Speech.Commands
{
    /// <summary>
    /// Pauses text-to-speech job.
    /// </summary>
    /// <remarks>If the text-to-speech job was in processes, the speech generation will be paused.</remarks>
    [Cmdlet(VerbsLifecycle.Suspend, "TextToSpeech", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PromptBuilder))]
    public class Suspend_TextToSpeech : TextToSpeechCmdlet
    {
        /// <summary>
        /// Object representing the text-to-speech background job to pause.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            base.ProcessRecord();
            Job.Pause();
        }
    }
}
