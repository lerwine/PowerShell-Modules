using System;
using System.Management.Automation;
using System.Reflection;

namespace WpfCLR
{
    public class PSAttachedEventInstance
    {
        private Delegate _handler;
        private EventInfo _eventInfo;
        private Type _sourceType;
        private object _eventSource;
        public EventInfo EventInfo { get { return _eventInfo; } }
        public Type SourceType { get { return _sourceType; } }
        public PSAttachedEventInstance(Delegate handler, string eventName, Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            Initialize(handler, null, sourceType, eventName);
        }
        public PSAttachedEventInstance(Delegate handler, object eventSource, string eventName)
        {
            if (eventSource == null)
                throw new ArgumentNullException("eventSource");

            Initialize(handler, eventSource, eventSource.GetType(), eventName);
        }
        private void Initialize(Delegate handler, object eventSource, Type sourceType, string eventName)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (eventName == null)
                throw new ArgumentNullException("eventName");

            if (eventName.Trim().Length == 0)
                throw new ArgumentException("Invalid event name", "eventName");

            _handler = handler;
            _sourceType = sourceType;

            try { _eventInfo = sourceType.GetEvent(eventName); }
            catch (Exception exception) { throw new ArgumentException(exception.Message, "eventName", exception); }

            if (_eventInfo == null)
                throw new ArgumentOutOfRangeException("eventName", eventName, String.Format("Event named \"{0}\" not found.", eventName));

            try { _eventInfo.AddEventHandler(eventSource, handler); }
            catch (Exception exception) { throw new ArgumentException(exception.Message, "handler", exception); }
        }

        public bool SourceEquals(object eventSource) { return eventSource != null && _eventSource != null && ReferenceEquals(_eventSource, eventSource); }

        public bool IsInstance { get { return _eventSource != null; } }
        public bool IsAttached() { return _handler != null; }
        public object Detach()
        {
            if (_handler == null)
                return null;

            _eventInfo.RemoveEventHandler(_eventSource, _handler);
            _handler = null;
            object obj = (_eventSource == null) ? _sourceType : _eventSource;
            _eventSource = null;
            EventHandler handler = OnDetached;
            if (handler != null)
                handler(this, EventArgs.Empty);
            return obj;
        }
        public event EventHandler OnDetached;
    }
}