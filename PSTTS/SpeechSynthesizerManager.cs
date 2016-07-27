using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PSTTS
{
    public class SpeechSynthesizerManager : IDisposable
    {
        private SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
        
        public SpeechSynthesizerManager()
        {
            _lastKnownState = _synthesizer.State;
            _synthesizer.BookmarkReached += _synthesizer_BookmarkReached;
            _synthesizer.PhonemeReached += _synthesizer_PhonemeReached;
            _synthesizer.SpeakCompleted += _synthesizer_SpeakCompleted;
            _synthesizer.SpeakProgress += _synthesizer_SpeakProgress;
            _synthesizer.SpeakStarted += _synthesizer_SpeakStarted;
            _synthesizer.StateChanged += _synthesizer_StateChanged;
            _synthesizer.VisemeReached += _synthesizer_VisemeReached;
            _synthesizer.VoiceChange += _synthesizer_VoiceChange;
        }

        private void _synthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private void _synthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private void _synthesizer_StateChanged(object sender, StateChangedEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _lastKnownState = e.State;
                _mre.Set();
            }
        }

        private void _synthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private void _synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private void _synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private void _synthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private void _synthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            lock (_syncRoot)
            {
                _events.AddLast(SpeechWaitEvent.Create(e));
                _mre.Set();
            }
        }

        private object _syncRoot = new object();
        private SynthesizerState _lastKnownState;
        ManualResetEvent _mre = new ManualResetEvent(false);
        private LinkedList<SpeechWaitEvent> _events = new LinkedList<SpeechWaitEvent>();
        public SpeechWaitEvent Wait()
        {
            Task task;
            lock (_syncRoot)
            {
                if (_events.First != null && _lastKnownState == SynthesizerState.Speaking)
                    task = Task.Factory.StartNew(() =>_mre.WaitOne());
                else
                    task = null;
            }

            if (task != null)
                task.Wait();

            lock (_syncRoot)
            {
                if (_events.First == null)
                    return SpeechWaitEvent.None;
                SpeechWaitEvent result = _events.First.Value;
                _events.RemoveFirst();
                return result;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SpeechSynthesizerManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
