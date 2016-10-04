using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Speech.Synthesis;
using System.Text;

namespace Erwine.Leonard.T.TextToSpeech
{
    public class SpeechSynthesisResult
    {
        public bool Cancelled { get; private set; }
        public bool Failed { get; private set; }
        public Hashtable SynchronizedData { get; private set; }
        public Collection<DebugRecord> DebugMessages { get; private set; }
        public Collection<ErrorRecord> Errors { get; private set; }
        public Collection<InformationRecord> InformationMessages { get; private set; }
        public Collection<PSObject> Output { get; private set; }
        public Collection<VerboseRecord> VerboseMessages { get; private set; }
        public Collection<WarningRecord> Warnings { get; private set; }

        public SpeechSynthesisResult(Collection<PSObject> output, Collection<ErrorRecord> errors, Collection<WarningRecord> warnings,
            Collection<InformationRecord> informationMessages, Collection<VerboseRecord> verboseMessages, Collection<DebugRecord> debugMessages, bool cancelled,
            bool failed, Hashtable synchronizedData)
        {
            Output = output;
            Errors = errors;
            Warnings = warnings;
            InformationMessages = informationMessages;
            VerboseMessages = verboseMessages;
            DebugMessages = debugMessages;
            Cancelled = cancelled;
            Failed = failed;
            SynchronizedData = synchronizedData;
        }
    }
}