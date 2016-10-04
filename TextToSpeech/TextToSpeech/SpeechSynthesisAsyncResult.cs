using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.TextToSpeech
{
    public class SpeechSynthesisAsyncResult : SpeechSynthesisResult
    {
        public object UserState { get; private set; }

        public SpeechSynthesisAsyncResult(Collection<PSObject> output, Collection<ErrorRecord> errors, Collection<WarningRecord> warnings, 
            Collection<InformationRecord> informationMessages, Collection<VerboseRecord> verboseMessages, Collection<DebugRecord> debugMessages, bool cancelled, 
            bool failed, Hashtable synchronizedData, object userState) 
            : base(output, errors, warnings, informationMessages, verboseMessages, debugMessages, cancelled, failed, synchronizedData)
        {
            UserState = userState;
        }
    }
}
