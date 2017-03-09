using System;
using System.ComponentModel;

namespace PSTTS
{
    public class AsyncCompletedEventLogItem<TArgs> : SpeechEventLogItem<TArgs>
        where TArgs : AsyncCompletedEventArgs
    {
        public override bool Cancelled { get { return Args.Cancelled; } }

        public override Exception Error { get { return Args.Error; } }

        protected AsyncCompletedEventLogItem(TArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}