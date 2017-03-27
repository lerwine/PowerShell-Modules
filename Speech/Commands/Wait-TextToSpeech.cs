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

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            bool hasEventData;
            if (Progress.IsPresent)
            {
                if (_milliSecondsTimeout.HasValue)
                    hasEventData = Job.WaitSpeechProgress(_milliSecondsTimeout.Value);
                else
                    hasEventData = Job.WaitSpeechProgress();
                
            } else if (_milliSecondsTimeout.HasValue)
                hasEventData = Job.WaitSpeechCompleted(_milliSecondsTimeout.Value);
            else
                hasEventData = Job.WaitSpeechCompleted();

            if (hasEventData)
                WriteObject(Job.GetOutput(), true);
        }
    }
}
