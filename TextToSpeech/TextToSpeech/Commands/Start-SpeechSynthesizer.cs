using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    [Cmdlet(VerbsLifecycle.Start, "SpeechSynthesizer", DefaultParameterSetName = PARAMETERSETNAME_TEXT)]
    [OutputType(typeof(SpeechSynthesisAsyncInvocation))]
    public class Start_SpeechSynthesizer : SpeechSynthesizerCommandBase<SpeechSynthesisAsyncInvocation>
    {
        [Parameter()]
        public object UserState { get; set; }

        [Parameter()]
        public SwitchParameter DoNotStart { get; set; }

        protected override SpeechSynthesisAsyncInvocation CreateResult(Collection<Prompt> prompts, out bool enumerateCollection)
        {
            SpeechSynthesisAsyncInvocation invocation = new SpeechSynthesisAsyncInvocation(prompts, OnSpeakStarted, OnPromptStarted, OnStateChanged, 
                OnSpeakProgress, OnBookmarkReached, OnVoiceChange, OnVisemeReached, OnPhonemeReached, OnPromptCompleted, OnSpeakCompleted, UserState);
            if (DoNotStart == null || !DoNotStart.IsPresent)
                invocation.Start();
            enumerateCollection = false;
            return invocation;
        }
    }
}
