using System;
using System.Threading;
using System.Windows.Forms;

namespace PersonalEventTracking
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class EventDefinitionBase
    {
        private object _syncRoot = new object();
        private int _urgency = 2;
        private int _impact = 2;
        private int _priority = 2;
        private string _title = "";
        private string _details = "";
        private DateTime? _start;
        private DateTime? _end;
        private DateTime? _snoozeEnd;
        private TimeSpan? _duration;
        private TimeSpan? _dueIn;
        private TimeSpan? _overdueIn;
        private bool _isDueToday = true;
        private bool _isDueNow = true;
        private bool _isOverdue = true;
        private bool _isActive = true;

        private RecurranceDefinition _recurrance = null;

        protected internal object SyncRoot { get { return _syncRoot; } }

        public int Urgency
        {
            get { return _urgency; }
            set
            {
                if (value < 1)
                    Urgency = 1;
                else if (value > 3)
                    Urgency = 3;
                else
                {
                    Monitor.Enter(_syncRoot);
                    try
                    {
                        if (value == _urgency)
                            return;
                        _urgency = value;
                        UpdatePriority();
                    }
                    finally { Monitor.Exit(_syncRoot); }
                }
            }
        }
        
        public int Impact
        {
            get { return _impact; }
            set
            {
                if (value < 1)
                    Urgency = 1;
                else if (value > 3)
                    Urgency = 3;
                else
                {
                    Monitor.Enter(_syncRoot);
                    try
                    {
                        if (value == _impact)
                            return;
                        _impact = value;
                        UpdatePriority();
                    }
                    finally { Monitor.Exit(_syncRoot); }
                }
            }
        }
        
        public string Title
        {
            get { return _title; }
            set { _title = (value == null) ? "" : value; }
        }
        
        public string Details
        {
            get { return _details; }
            set { _details = (value == null) ? "" : value; }
        }
        
        public DateTime Start
        {
            get
            {
                DateTime dateTime = default(DateTime);
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_start.HasValue)
                    {
                        dateTime = _start.Value;
                        if (_end.HasValue && dateTime > _end.Value)
                        {
                            dateTime = _end.Value;
                            _end = _start;
                            _start = dateTime;
                            _snoozeEnd = null;
                            UpdateExpiration();
                        }
                    }
                    else
                    {
                        dateTime = (_end.HasValue) ? (_duration.HasValue && !_duration.Equals(TimeSpan.Zero)) ? _end.Value.Add(TimeSpan.Zero.Subtract(_duration.Value)) : _end.Value : DateTime.Now;
                        _start = dateTime;
                        _snoozeEnd = null;
                        UpdateExpiration();
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return dateTime;
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_start.HasValue && _start.Value.Equals(value))
                        return;
                    if (_end.HasValue)
                        _duration = null;
                    _start = value;
                    _snoozeEnd = null;
                    UpdateExpiration();
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        
        public DateTime End
        {
            get
            {
                DateTime dateTime = default(DateTime);
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_end.HasValue)
                    {
                        dateTime = _end.Value;
                        if (_start.HasValue && dateTime < _start.Value)
                        {
                            dateTime = _start.Value;
                            _start = _end;
                            _end = dateTime;
                            _snoozeEnd = null;
                            UpdateExpiration();
                        }
                    }
                    else
                    {
                        dateTime = (_start.HasValue) ? _start.Value : DateTime.Now;
                        if (_duration.HasValue)
                            dateTime = dateTime.Add(_duration.Value);
                        _end = dateTime;
                        _snoozeEnd = null;
                        UpdateExpiration();
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return dateTime;
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_end.HasValue && _end.Value.Equals(value))
                        return;
                    if (_start.HasValue)
                        _duration = null;
                    _end = value;
                    _snoozeEnd = null;
                    UpdateExpiration();
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        
        public TimeSpan Duration
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_duration.HasValue)
                        timeSpan = _duration.Value;
                    else
                    {
                        if (_start.HasValue && _end.HasValue)
                            timeSpan = End - Start;
                        _duration = timeSpan;
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return timeSpan;
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_duration.HasValue && _duration.Value.Equals(value))
                        return;
                    if (value < TimeSpan.Zero)
                    {
                        _start = Start.Add(value);
                        _duration = TimeSpan.Zero.Subtract(value);
                        _end = null;
                    } 
                    else
                    {
                        _duration = value;
                        if (_start.HasValue && _end.HasValue)
                            _end = Start.Add(value);
                    }
                    _snoozeEnd = null;
                    UpdateExpiration();
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        
        public TimeSpan DueIn
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                Monitor.Enter(_syncRoot);
                try
                {
                    if (!_dueIn.HasValue)
                        UpdateExpiration();
                    timeSpan = _dueIn.Value;
                }
                finally { Monitor.Exit(_syncRoot); }
                return timeSpan;
            }
        }

        public TimeSpan OverdueIn
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Zero;
                Monitor.Enter(_syncRoot);
                try
                {
                    if (!_overdueIn.HasValue)
                        UpdateExpiration();
                    timeSpan = _overdueIn.Value;
                }
                finally { Monitor.Exit(_syncRoot); }
                return timeSpan;
            }
        }

        public DateTime? SnoozeEnd
        {
            get { return _snoozeEnd; }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (value.HasValue)
                    {
                        if (_snoozeEnd.HasValue && value.Value.Equals(_snoozeEnd.Value))
                            return;
                    }
                    else if (!_snoozeEnd.HasValue)
                        return;
                    _snoozeEnd = value;
                    UpdateExpiration();
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        
        public virtual bool IsDueToday { get { return _isDueToday; } }
        
        public virtual bool IsDueNow { get { return _isDueNow; } }
        
        public virtual bool IsOverdue { get { return _isOverdue; } }
        
        public virtual bool IsActive { get { return _isActive; } }
        
        public abstract bool IsClosed { get; }

        public abstract bool CanClose(out string reason);
        
        public abstract bool TryClose(out string reason);

        public bool TryClose()
        {
            string reason;
            return TryClose(out reason);
        }
        
        public void Close()
        {
            string reason;
            if (!TryClose(out reason))
                throw new InvalidOperationException(reason);
        }
        
        public void UpdateExpiration()
        {
            DateTime now = DateTime.Now;
            _dueIn = Start - now;
            _overdueIn = End - now;
            if (IsClosed)
            {
                _isOverdue = false;
                _isDueNow = false;
                _isDueToday = false;
                _isActive = false;
                _snoozeEnd = null;
                return;
            }
            if (_snoozeEnd.HasValue && _snoozeEnd <= now)
                _snoozeEnd = null;
            _isOverdue = _overdueIn < TimeSpan.Zero;
            if (_isOverdue)
            {
                _isDueNow = true;
                _isDueToday = true;
            }
            else
            {
                _isDueNow = _dueIn < TimeSpan.Zero;
                if (_isDueNow)
                    _isDueToday = true;
                else
                    _isDueToday = (now.Date == Start.Date);
            }
            _isActive = (_isDueNow && !_snoozeEnd.HasValue);
        }

        protected EventDefinitionBase() { UpdatePriority(); }

        private void UpdatePriority() { _priority = Convert.ToInt32(Math.Floor(Convert.ToDouble((_urgency * _impact) + _urgency + _impact + 1) / 4.0)); }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}