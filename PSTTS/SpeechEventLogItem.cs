using System;
using System.Collections;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading;

namespace PSTTS
{
    public abstract class SpeechEventLogItem<TArgs> : SpeechEventLogItem
        where TArgs : EventArgs
    {
        TArgs _args;

        protected TArgs Args { get { return _args; } }

        protected SpeechEventLogItem(TArgs e, SpeechEventHandler.SpeechContext context)
        {
            _args = e;
        }
    }
    public abstract class SpeechEventLogItem
    {
        private SpeechEventLogItem _previous = null;
        private SpeechEventLogItem _next = null;
        private SpeechEventLog _root = null;

        public SpeechEventLogItem Previous { get { return _previous; } }

        public SpeechEventLogItem Next { get { return _next; } }

        public SpeechEventLog Root { get { return _root; } }

        public virtual TimeSpan AudioPosition { get { return (_previous == null) ? TimeSpan.Zero : _previous.AudioPosition; } }

        public virtual string Bookmark { get { return (_previous == null) ? null : _previous.Bookmark; } }

        public virtual bool Cancelled { get { return (_previous == null) ? false : _previous.Cancelled; } }

        public virtual int CharacterCount { get { return (_previous == null) ? 0 : _previous.CharacterCount; } }

        public virtual int CharacterPosition { get { return (_previous == null) ? 0 : _previous.CharacterPosition; } }

        public virtual TimeSpan Duration { get { return (_previous == null) ? TimeSpan.Zero : _previous.Duration; } }
        
        public virtual SynthesizerEmphasis Emphasis { get { return (_previous == null) ? SynthesizerEmphasis.Emphasized : _previous.Emphasis; } }

        public virtual Exception Error { get { return (_previous == null) ? null : _previous.Error; } }

        public virtual object Key { get { return (_previous == null) ? null : _previous.Key; } }

        public virtual string Phoneme { get { return (_previous == null) ? null : _previous.Phoneme; } }

        public virtual SynthesizerState State { get { return (_previous == null) ? SynthesizerState.Ready : _previous.State; } }

        public virtual string Text { get { return (_previous == null) ? null : _previous.Text; } }

        public virtual object UserState { get { return (_previous == null) ? null : _previous.UserState; } }

        public virtual int Viseme { get { return (_previous == null) ? 0 : _previous.Viseme; } }

        public virtual VoiceInfo Voice { get { return (_previous == null) ? null : _previous.Voice; } }
        
        public IEnumerable<SpeechEventLogItem> GetFollowing(bool includeCurrent)
        {
            if (includeCurrent)
                yield return this;
            for (SpeechEventLogItem i = _next; i != null; i = i._next)
                yield return i;
        }

        public SpeechEventLogItem GetFollowing(int index) { return GetFollowing(index, false); }

        public SpeechEventLogItem GetFollowing(int index, bool doNotThrowException)
        {
            SpeechEventLogItem item = this;
            do
            {
                if (index == 0)
                    return item;
                index--;
            } while ((item = item.Next) != null && index > -1);
                

            if (!doNotThrowException)
                throw new ArgumentOutOfRangeException("index");
            return null;
        }

        public SpeechEventLogItem GetPreceding(int index)
        {
            SpeechEventLogItem item = this;
            do
            {
                if (index == 0)
                    return item;
                index--;
            } while ((item = item.Previous) != null && index > -1);

            throw new ArgumentOutOfRangeException("index");
        }

        public void ReplaceWith(SpeechEventLogItem value)
        {
            if (value == null)
                throw new ArgumentNullException();

            if (ReferenceEquals(value, this))
                return;

            if (_root == null)
                throw new InvalidOperationException("Item does not belong to a speech event log collection.");

            object syncRoot = _root.SyncRoot;
            Monitor.Enter(syncRoot);
            try
            {
                if (value._root != null)
                {
                    if (ReferenceEquals(value._root, _root))
                        throw new InvalidOperationException("Item already exists in this collection.");
                    throw new InvalidOperationException("Item belongs to another collection.");
                }
                value._root = _root;
                value._previous = _previous;
                value._next = _next;
                if (_previous != null)
                    _previous._next = value;
                if (_next != null)
                    _next._previous = value;
                _next = null;
                _previous = null;
                _root = null;
                if (value._next == null)
                    _root.ResetLast(value);
                if (value._previous == null)
                    _root.ResetFirst(value);
            }
            finally { Monitor.Exit(syncRoot); }
        }

        // public List<SpeechEventLogItem> ToList() { return new List<SpeechEventLogItem>(GetFollowing(true)); }

        protected int IndexOfFollowing(SpeechEventLogItem item)
        {
            if (item == null)
                return -1;

            int index = 0;
            for (SpeechEventLogItem i = this; i != null; i = i._next)
            {
                if (ReferenceEquals(item, i))
                    return index;
                index++;
            }

            return -1;
        }

        internal void InsertBefore(SpeechEventLogItem refItem)
        {
            if (refItem == null)
                throw new ArgumentNullException("refItem");

            SpeechEventLog log = refItem._root;
            if (log == null)
                throw new InvalidOperationException("Item does not belong to an event log collection.");
            Monitor.Enter(log.SyncRoot);
            try
            {
                if (_root != null)
                    throw new InvalidOperationException("Item belongs to another event log collection.");
                _root = log;
                _previous = refItem._previous;
                _next = refItem;
                refItem._previous = this;
                if (_previous == null)
                    _root.ResetFirst(this);
                else
                    _previous._next = this;
            }
            finally { Monitor.Exit(log.SyncRoot); }
        }

        internal void Remove()
        {
            SpeechEventLog log = _root;
            if (log == null)
                throw new InvalidOperationException("Item does not belong to an event log collection.");
            Monitor.Enter(log.SyncRoot);
            try
            {
                SpeechEventLogItem previous = _previous;
                SpeechEventLogItem next = _next;
                _root = null;
                _previous = null;
                _next = null;
                if (previous != null)
                {
                    previous._next = next;
                    if (next != null)
                        next._previous = previous;
                    else
                        log.ResetLast(previous);
                }
                else
                {
                    if (next != null)
                        next._previous = null;
                    log.ResetFirst(next);
                }
            }
            finally { Monitor.Exit(log.SyncRoot); }
        }

        internal void InsertAfter(SpeechEventLogItem refItem)
        {
            if (refItem == null)
                throw new ArgumentNullException("refItem");

            SpeechEventLog log = refItem._root;
            if (log == null)
                throw new InvalidOperationException("Item does not belong to an event log collection.");
            Monitor.Enter(log.SyncRoot);
            try
            {
                if (_root != null)
                    throw new InvalidOperationException("Item belongs to another event log collection.");
                _root = log;
                _previous = refItem;
                _next = refItem._next;
                refItem._next = this;
                if (_next == null)
                    _root.ResetLast(this);
                else
                    _next._previous = this;
            }
            finally { Monitor.Exit(log.SyncRoot); }
        }

        internal void SetFirst(SpeechEventLog speechEventLog)
        {
            if (speechEventLog == null)
                throw new ArgumentNullException("speechEventLog");

            Monitor.Enter(speechEventLog.SyncRoot);
            try
            {
                if (_root != null)
                    throw new InvalidOperationException("This item belongs to another collection.");
                if (speechEventLog.IsEmpty)
                    speechEventLog.Add(this);
                else
                {
                    if (!(ReferenceEquals(speechEventLog.FirstItem, this) && ReferenceEquals(speechEventLog.LastItem, this)))
                        throw new InvalidOperationException("Speech event log is not empty.");
                    _root = speechEventLog;
                }
            }
            finally { Monitor.Exit(speechEventLog.SyncRoot); }
        }

        internal void RemoveAll()
        {
            SpeechEventLog root = _root;
            if (root == null)
                return;

            Monitor.Enter(root.SyncRoot);
            try
            {
                SpeechEventLogItem first = this;
                for (SpeechEventLogItem i = first.Previous; i != null; i = i.Previous)
                    first = i;

                first._root = null;
                for (SpeechEventLogItem i = first.Next; i != null; i = i.Next)
                {
                    i._root = null;
                    i._previous._next = null;
                    i._previous = null;
                }
                root.ResetFirst(null);
            } finally { Monitor.Exit(root.SyncRoot); }
        }

        internal void RemoveFollowing()
        {
            SpeechEventLog root = _root;
            if (root == null)
                return;

            Monitor.Enter(root.SyncRoot);
            try
            {
                for (SpeechEventLogItem i = Next; i != null; i = i.Next)
                {
                    i._root = null;
                    i._previous._next = null;
                    i._previous = null;
                }
                root.ResetLast(this);
            }
            finally { Monitor.Exit(root.SyncRoot); }
        }
    }
}