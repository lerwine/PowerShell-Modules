using System.Speech.Synthesis;

namespace PSTTS
{
    internal class StateChangedEventData : SpeechEventLogItem<StateChangedEventArgs>
    {
        public override SynthesizerState State { get { return Args.State; } }
        public StateChangedEventData(StateChangedEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}