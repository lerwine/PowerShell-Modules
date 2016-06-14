using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;

namespace Erwine.Leonard.T.TextToSpeech
{
    public abstract class SpeechSynthesisResult
    {
        public TimeSpan AudioPosition { get; internal set; }
        public bool Cancelled { get; internal set; }
        public int CharacterCount { get; internal set; }
        public int CharacterPosition { get; internal set; }
        public Exception Error { get; internal set; }
        public int Index { get; internal set; }
        public string LastBookmark { get; internal set; }
        public string Phoneme { get; internal set; }
        public object Prompt { get; internal set; }
        public SynthesizerState State { get; internal set; }
        public string Text { get; internal set; }
        public int Viseme { get; internal set; }
        public VoiceInfo Voice { get; internal set; }
    }

    public class SpeechSynthesisResult<TPrompt> : SpeechSynthesisResult
    {
        public new TPrompt Prompt { get; internal set; }
    }

    public class SpeechSynthesisResult<TPrompt, TNext> : SpeechSynthesisResult<TPrompt>
    {
        public TimeSpan Duration { get; internal set; }
        public SynthesizerEmphasis Emphasis { get; internal set; }
        public TNext Next { get; internal set; }
    }
}