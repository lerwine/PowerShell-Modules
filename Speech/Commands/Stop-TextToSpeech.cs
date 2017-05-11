using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Speech.Commands
{
    /// <summary>
    /// Stops text-to-speech job.
    /// </summary>
    /// <remarks>If a text-to-speech job was in progress, it is stopped, and any queued output from the text-to-speech process is returned.</remarks>
    [Cmdlet(VerbsLifecycle.Stop, "TextToSpeech", RemotingCapability = RemotingCapability.None)]
    public class Stop_TextToSpeech : TextToSpeechCmdlet
    {
        /// <summary>
        /// Object representing the text-to-speech background job to stop.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            base.ProcessRecord();
            Job.AsyncCancel();
            Collection<object> output;
            if (Wait_TextToSpeech.TryWait(Job, null, false, out output))
                WriteObject(output, true);
        }
    }
}
