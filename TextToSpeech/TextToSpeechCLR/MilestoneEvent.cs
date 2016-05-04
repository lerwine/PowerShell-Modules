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

        public long EventOrder { get { return this._eventOrder; } }

        public string Message { get { return this._message; } }

        public MilestoneEvent(long eventOrder, string message)
        {
            this._eventOrder = eventOrder;
            this._message = message ?? "";
        }

        public int CompareTo(IEventRecord other)
        {
            return (other == null) ? -1 : this.EventOrder.CompareTo(other.EventOrder);
        }

        public bool Equals(IEventRecord other)
        {
            return other != null && this.EventOrder == other.EventOrder;
        }
    }
}
