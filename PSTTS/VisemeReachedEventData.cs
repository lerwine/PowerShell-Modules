using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    internal class VisemeReachedEventData : AsyncCompletedEventLogItem<VisemeReachedEventArgs>
    {
        public override int Viseme { get { return Args.Viseme; } }
        public override TimeSpan AudioPosition { get { return Args.AudioPosition; } }
        public override TimeSpan Duration { get { return Args.Duration; } }
        public override SynthesizerEmphasis Emphasis { get { return Args.Emphasis; } }
        public VisemeReachedEventData(VisemeReachedEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}