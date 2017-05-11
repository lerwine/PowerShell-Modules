using System.Management.Automation;
using System.Speech.Synthesis;

namespace Speech.Commands
{
    /// <summary>
    /// Gets current status of speech synthesis.
    /// </summary>
    /// <remarks>Gets the current <seealso cref="SynthesizerState"/> from the referenced text-to-speech job.</remarks>
    [Cmdlet(VerbsCommon.Get, "TextToSpeechStatus", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(SynthesizerState))]
    public class Get_TextToSpeechStatus : TextToSpeechCmdlet
    {
        /// <summary>
        /// Object representing a Text-To-Speech job.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            WriteObject(Job.State);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
