using System;
using System.Speech.Synthesis;

namespace TextToSpeechCLR
{
    public struct ChangeEvent<T> : IEventRecord
    {
        private long _eventOrder;
        private string _message;
        private T _value;

        public ChangeEvent(long eventOrder, T value, string message)
        {
            _eventOrder = eventOrder;
            _value = value;
            _message = message ?? "";
        }

        public long EventOrder { get { return _eventOrder; } }

        public string Message { get { return _message; } }

        public T Value { get { return _value; } }

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