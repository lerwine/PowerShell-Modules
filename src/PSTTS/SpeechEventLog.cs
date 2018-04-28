using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;

namespace PSTTS
{
    public class SpeechEventLog : IList<SpeechEventLogItem>, IList
    {
        public SpeechEventLog() { }
        public SpeechEventLog(SpeechEventHandler.SpeechContext context)
        {
            UserState = context.State;
            State = context.SpeechSynthesizer.State;
            //Rate = context.SpeechSynthesizer.Rate;
            Voice = context.SpeechSynthesizer.Voice;
            //Volume = context.SpeechSynthesizer.Volume;
        }

        private object _syncRoot = new object();
        private SpeechEventLogItem _lastItem = null;
        private SpeechEventLogItem _firstItem = null;
        private long? _longCount = 0L;
        private object _userState = null;
        private SynthesizerState _state = SynthesizerState.Ready;
        //private int _rate = 0;
        VoiceInfo _voice = null;
        //int _volumen = 0;
        public virtual TimeSpan AudioPosition { get { return (_lastItem == null) ? TimeSpan.Zero : _lastItem.AudioPosition; } }

        public virtual string Bookmark { get { return (_lastItem == null) ? null : _lastItem.Bookmark; } }

        public virtual bool Cancelled { get { return (_lastItem == null) ? false : _lastItem.Cancelled; } }

        public virtual int CharacterCount { get { return (_lastItem == null) ? 0 : _lastItem.CharacterCount; } }

        public virtual int CharacterPosition { get { return (_lastItem == null) ? 0 : _lastItem.CharacterPosition; } }

        public virtual TimeSpan Duration { get { return (_lastItem == null) ? TimeSpan.Zero : _lastItem.Duration; } }

        public virtual SynthesizerEmphasis Emphasis { get { return (_lastItem == null) ? SynthesizerEmphasis.Emphasized : _lastItem.Emphasis; } }

        public virtual Exception Error { get { return (_lastItem == null) ? null : _lastItem.Error; } }

        public virtual object Key { get { return (_lastItem == null) ? null : _lastItem.Key; } }

        public virtual string Phoneme { get { return (_lastItem == null) ? null : _lastItem.Phoneme; } }

        public virtual SynthesizerState State { get { return (_lastItem == null) ? _state : _lastItem.State; } set { _state = value; } }

        public virtual string Text { get { return (_lastItem == null) ? null : _lastItem.Text; } }

        public virtual object UserState { get { return (_lastItem == null) ? null : _lastItem.UserState; } set { _userState = value; } }

        public virtual int Viseme { get { return (_lastItem == null) ? 0 : _lastItem.Viseme; } }

        public virtual VoiceInfo Voice { get { return (_lastItem == null) ? _voice : _lastItem.Voice; } set { _voice = value; } }

        public SpeechEventLogItem FirstItem { get { return _firstItem; } }

        public SpeechEventLogItem LastItem { get { return _lastItem; } }

        public bool IsEmpty { get { return _firstItem == null; } }

        int ICollection<SpeechEventLogItem>.Count { get { return Count; } }

        bool ICollection<SpeechEventLogItem>.IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return false; } }

        bool IList.IsFixedSize { get { return false; } }

        protected int Count
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (!_longCount.HasValue)
                    {
                        long c = 0L;
                        for (SpeechEventLogItem i = _firstItem; i != null; i = i.Next)
                            c++;
                        _longCount = c;
                    }
                    return (_longCount.Value > (long)(int.MaxValue)) ? int.MaxValue : (int)(_longCount.Value);
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        int ICollection.Count { get { return Count; } }

        public object SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (SpeechEventLogItem)value; }
        }

        internal void ResetLast(SpeechEventLogItem value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (value == null)
                {
                    if (_lastItem == null)
                        return;

                    if ((_firstItem == null || _firstItem.Root == null || !ReferenceEquals(_firstItem.Root, this)) && (_lastItem.Root == null || !ReferenceEquals(_lastItem.Root, this)))
                    {
                        _lastItem = null;
                        _firstItem = null;
                        _longCount = 0L;
                        return;
                    }

                    return;
                }
                if (value.Root == null || !ReferenceEquals(value.Root, this))
                    return;
                for (SpeechEventLogItem i = value.Next; i != null; i = i.Next)
                    value = i;
                _lastItem = value;
                if (value.Previous == null)
                {
                    _firstItem = value;
                    _longCount = 1L;
                }
                else if (_firstItem == null)
                {
                    long c = 1L;
                    for (SpeechEventLogItem i = value.Previous; i != null; i = i.Previous)
                    {
                        value = i;
                        c++;
                    }
                    _longCount = c;
                    _firstItem = value;
                }
                else
                    _longCount = null;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        internal void ResetFirst(SpeechEventLogItem value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (value == null)
                {
                    if (_firstItem == null)
                        return;

                    if ((_lastItem == null || _lastItem.Root == null || !ReferenceEquals(_lastItem.Root, this)) && (_firstItem.Root == null || !ReferenceEquals(_firstItem.Root, this)))
                    {
                        _lastItem = null;
                        _firstItem = null;
                        _longCount = 0L;
                        return;
                    }

                    return;
                }
                if (value.Root == null || !ReferenceEquals(value.Root, this))
                    return;
                for (SpeechEventLogItem i = value.Previous; i != null; i = i.Next)
                    value = i;
                _firstItem = value;
                if (value.Next == null)
                {
                    _lastItem = value;
                    _longCount = 1L;
                }
                else if (_firstItem == null)
                {
                    long c = 1L;
                    for (SpeechEventLogItem i = value.Next; i != null; i = i.Next)
                    {
                        value = i;
                        c++;
                    }
                    _longCount = c;
                    _lastItem = value;
                }
                else
                    _longCount = null;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private SpeechEventLogItem _GetItemAt(int index)
        {
            if (index < 0 || (_longCount.HasValue && (long)index >= _longCount.Value))
                throw new ArgumentOutOfRangeException("index");

            if (index == 0)
                return _firstItem;

            if (!_longCount.HasValue)
                return _firstItem.GetFollowing(index);
            if ((long)index == _longCount - 1)
                return _lastItem;
            if (index <= _longCount.Value << 1)
                return _firstItem.GetFollowing(index);

            return _lastItem.GetPreceding((int)_longCount.Value - index);
        }

        private int _GetIndexOf(SpeechEventLogItem item)
        {
            if (item == null || item.Root == null || !ReferenceEquals(item.Root, this))
                return -1;

            int index = 0;
            for (item = item.Previous; item != null; item = item.Previous)
            {
                index++;
                if (index == int.MaxValue)
                    return (item.Previous == null) ? index : -1;
            }
            return index;
        }

        public SpeechEventLogItem this[int index]
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _GetItemAt(index); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    SpeechEventLogItem refItem = _GetItemAt(index);
                    refItem.ReplaceWith(value);
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        int IList<SpeechEventLogItem>.IndexOf(SpeechEventLogItem item)
        {
            Monitor.Enter(_syncRoot);
            try { return _GetIndexOf(item); }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList<SpeechEventLogItem>.Insert(int index, SpeechEventLogItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                long? c = _longCount;
                SpeechEventLogItem refItem = _GetItemAt(index);
                item.InsertBefore(refItem);
                if (c.HasValue)
                    _longCount = c.Value + 1;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList<SpeechEventLogItem>.RemoveAt(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                long? c = _longCount;
                SpeechEventLogItem refItem = _GetItemAt(index);
                refItem.Remove();
                if (c.HasValue)
                    _longCount = c.Value - 1;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Add(SpeechEventLogItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                SpeechEventLogItem refItem = _lastItem;
                if (refItem != null)
                {
                    long? c = _longCount;
                    item.InsertAfter(refItem);
                    if (c.HasValue)
                        _longCount = c.Value + 1;
                    return;
                }

                _firstItem = item;
                _lastItem = item;
                item.SetFirst(this);
                _longCount = 1L;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Clear()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_firstItem == null)
                    return;

                _firstItem.RemoveAll();
                _firstItem = null;
                _lastItem = null;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Contains(SpeechEventLogItem item)
        {
            Monitor.Enter(_syncRoot);
            try { return item != null && item.Root != null && ReferenceEquals(item.Root, this); }
            finally { Monitor.Exit(_syncRoot); }
        }

        void ICollection<SpeechEventLogItem>.CopyTo(SpeechEventLogItem[] array, int arrayIndex)
        {
            List<SpeechEventLogItem> list;
            Monitor.Enter(_syncRoot);
            try { list = (_firstItem == null) ? new List<SpeechEventLogItem>() : new List<PSTTS.SpeechEventLogItem>(_firstItem.GetFollowing(true)); }
            finally { Monitor.Exit(_syncRoot); }
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(SpeechEventLogItem item)
        {
            if (item == null)
                return false;
            Monitor.Enter(_syncRoot);
            try
            {
                if (item.Root == null || !ReferenceEquals(item.Root, this))
                    return false;

                long? c = _longCount;
                item.Remove();
                if (c.HasValue)
                    _longCount = c.Value - 1;
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        public IEnumerator<SpeechEventLogItem> GetEnumerator()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_firstItem == null)
                    return (new List<SpeechEventLogItem>()).GetEnumerator();
                return _firstItem.GetFollowing(true).GetEnumerator();
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_firstItem == null)
                    return (new SpeechEventLogItem[0]).GetEnumerator();
                return _firstItem.GetFollowing(true).ToArray().GetEnumerator();
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        int IList.Add(object value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                int index = Count;
                Add((SpeechEventLogItem)value);
                return index;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        bool IList.Contains(object value) { return Contains((SpeechEventLogItem)value); }

        int IList.IndexOf(object value)
        {
            Monitor.Enter(_syncRoot);
            try { return _GetIndexOf((SpeechEventLogItem)value); }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList.Insert(int index, object value)
        {
            if (value == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                long? c = _longCount;
                SpeechEventLogItem refItem = _GetItemAt(index);
                ((SpeechEventLogItem)value).InsertBefore(refItem);
                if (c.HasValue)
                    _longCount = c.Value + 1;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList.Remove(object value)
        {
            if (value == null)
                return;
            Monitor.Enter(_syncRoot);
            try
            {
                if (((SpeechEventLogItem)value).Root == null || !ReferenceEquals(((SpeechEventLogItem)value).Root, this))
                    return;

                long? c = _longCount;
                ((SpeechEventLogItem)value).Remove();
                if (c.HasValue)
                    _longCount = c.Value - 1;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList.RemoveAt(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                long? c = _longCount;
                SpeechEventLogItem refItem = _GetItemAt(index);
                refItem.Remove();
                if (c.HasValue)
                    _longCount = c.Value - 1;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            SpeechEventLogItem[] a;
            Monitor.Enter(_syncRoot);
            try { a = (_firstItem == null) ? new SpeechEventLogItem[0] : new List<PSTTS.SpeechEventLogItem>(_firstItem.GetFollowing(true)).ToArray(); }
            finally { Monitor.Exit(_syncRoot); }
            a.CopyTo(array, index);
        }

        public void Truncate(int maxItems)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_firstItem == null)
                    return;
                if (maxItems < 1)
                    Clear();
                else
                {
                    SpeechEventLogItem item;
                    if (_longCount.HasValue)
                    {
                        if (Count <= maxItems)
                            return;
                        if (maxItems > Count >> 1)
                            item = _lastItem.GetPreceding(Count - maxItems);
                        else
                            item = _firstItem.GetFollowing(maxItems - 1);
                    }
                    else if ((item = _firstItem.GetFollowing(maxItems - 1)) == null)
                        return;
                    item.RemoveFollowing();
                    _lastItem = item;
                    if (item.Previous == null)
                        _firstItem = item;
                    _longCount = maxItems;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
    }
}