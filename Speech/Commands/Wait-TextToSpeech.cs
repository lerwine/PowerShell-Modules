using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Speech.Commands
{
    [Cmdlet(VerbsLifecycle.Wait, "TextToSpeech", RemotingCapability = RemotingCapability.None, DefaultParameterSetName = ParameterSetName_Completed)]
    public class Wait_TextToSpeech : TextToSpeechCmdlet
    {
        public const string ParameterSetName_Completed = "Completed";
        public const string ParameterSetName_Progress = "Progress";

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNull()]
        public TextToSpeechJob Job { get; set; }

        [Parameter]
        public SwitchParameter Completed { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Progress)]
        public SwitchParameter Progress { get; set; }

        private int? _milliSecondsTimeout = null;

        public int MillisecondsTimeout { get { return (_milliSecondsTimeout.HasValue) ? _milliSecondsTimeout.Value : 0; } set { _milliSecondsTimeout = value; } }

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

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Collection<object> output;
            if (TryWait(Job, _milliSecondsTimeout, Progress.IsPresent, out output))
                WriteObject(output, true);
        }
    }
}
