using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Speech.Commands
{
    /// <summary>
    /// Waits for a text-to-speech job.
    /// </summary>
    /// <remarks>Waits for the text-to-speech job, returning any queued output from the background job.</remarks>
    [Cmdlet(VerbsLifecycle.Wait, "TextToSpeech", RemotingCapability = RemotingCapability.None, DefaultParameterSetName = ParameterSetName_Completed)]
    public class Wait_TextToSpeech : TextToSpeechCmdlet
    {
        /// <summary>
        /// Name of parameter set for waiting for speech generation to complete.
        /// <para></para>
        /// </summary>
        public const string ParameterSetName_Completed = "Completed";

        /// <summary>
        /// Name of parameter set for waiting for a speech generation progress change.
        /// </summary>
        public const string ParameterSetName_Progress = "Progress";

        /// <summary>
        /// Object representing a text-to-speech background job.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

        /// <summary>
        /// Wait for text-to-speech job ot complete.
        /// </summary>
        [Parameter()]
        public SwitchParameter Completed { get; set; }

        /// <summary>
        /// Wait for a progress update event from the text-to-speech job.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Progress)]
        public SwitchParameter Progress { get; set; }

        private int? _milliSecondsTimeout = null;

        /// <summary>
        /// Number of milliseconds to wait.
        /// </summary>
        public int MillisecondsTimeout { get { return (_milliSecondsTimeout.HasValue) ? _milliSecondsTimeout.Value : 0; } set { _milliSecondsTimeout = value; } }

        /// <summary>
        /// Waits for a <seealso cref="TextToSpeechJob"/>.
        /// </summary>
        /// <param name="job">Text-To-Speech job to wait on.</param>
        /// <param name="milliSecondsTimeout">Number of milliseconds to wait, or null to wait indefinitely.</param>
        /// <param name="progress">true to wait for a progress change; otherwise, false to wait for completion.</param>
        /// <param name="output">Objects representing output of the text-to-speech process.</param>
        /// <returns>true if job was completed; otherwise false. This also returns true if <paramref name="progress"/> was true, and there was a progress change event.</returns>
        public static bool TryWait(TextToSpeechJob job, int? milliSecondsTimeout, bool progress, out Collection<object> output)
        {
            bool hasEventData;
            if (progress)
            {
                if (milliSecondsTimeout.HasValue)
                    hasEventData = job.WaitSpeechProgress(milliSecondsTimeout.Value);
                else
                    hasEventData = job.WaitSpeechProgress();

            }
            else if (milliSecondsTimeout.HasValue)
                hasEventData = job.WaitSpeechCompleted(milliSecondsTimeout.Value);
            else
                hasEventData = job.WaitSpeechCompleted();

            output = (hasEventData) ? job.GetOutput() : null;

            return hasEventData;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            base.ProcessRecord();
            Collection<object> output;
            if (TryWait(Job, _milliSecondsTimeout, Progress.IsPresent, out output))
                WriteObject(output, true);
        }
    }
}
