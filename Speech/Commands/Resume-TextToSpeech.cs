using System.Management.Automation;

namespace Speech.Commands
{
    /// <summary>
    /// Resumes speech synthesis.
    /// </summary>
    /// <description>If speech was paused, this resume the speech synthesis process.</description>
    [Cmdlet(VerbsLifecycle.Resume, "TextToSpeech", RemotingCapability = RemotingCapability.None)]
    public class Resume_TextToSpeech : TextToSpeechCmdlet
    {
        /// <summary>
        /// Object representing a text-to-speech job.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            base.ProcessRecord();
            Job.Resume();
        }
    }
}
