using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeechCLR
{
    public struct ErrorEvent : IEventRecord
    {
        private long _eventOrder;
        private string _message;
        Exception _error;

        public long EventOrder { get { return _eventOrder; } }

        public string Message { get { return _message; } }

        public Exception Error { get { return _error; } }

        public ErrorEvent(long eventOrder, string message)
        {
            _eventOrder = eventOrder;
            _message = message ?? "";
            _error = null;
        }

        public ErrorEvent(long eventOrder, Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");
            _eventOrder = eventOrder;
            _message = error.Message ?? "";
            _error = error;
        }

        public int CompareTo(IEventRecord other)
        {
            return (other == null) ? -1 : _eventOrder.CompareTo(other.EventOrder);
        }

        public bool Equals(IEventRecord other)
        {
            return other != null && _eventOrder == other.EventOrder;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IEventRecord);
        }

        public override int GetHashCode()
        {
            return _eventOrder.GetHashCode();
        }

        public override string ToString()
        {
            if (_error == null)
                return String.Format("Error ({0}): {1}", _eventOrder, _message);

            return String.Format("{0} ({1}): {2}", _error.GetType().FullName, _eventOrder, _message);
        }
    }
}
