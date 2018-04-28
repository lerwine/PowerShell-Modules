using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public class PhonemeReachedEventData : AsyncCompletedEventLogItem<PhonemeReachedEventArgs>
    {
        public override string Phoneme { get { return Args.Phoneme; } }
        public override TimeSpan AudioPosition { get { return Args.AudioPosition; } }
        public override TimeSpan Duration { get { return Args.Duration; } }
        public override SynthesizerEmphasis Emphasis { get { return Args.Emphasis; } }
        public PhonemeReachedEventData(PhonemeReachedEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}