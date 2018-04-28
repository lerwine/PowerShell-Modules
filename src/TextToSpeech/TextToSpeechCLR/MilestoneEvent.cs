using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeechCLR
{
    public struct MilestoneEvent : IEventRecord
    {
        private long _eventOrder;
        private string _message;

        public long EventOrder { get { return _eventOrder; } }

        public string Message { get { return _message; } }

        public MilestoneEvent(long eventOrder, string message)
        {
            _eventOrder = eventOrder;
            _message = message ?? "";
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
            return String.Format("({0}): {1}", _eventOrder, _message);
        }
    }
}
