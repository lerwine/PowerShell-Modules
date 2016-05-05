using System;

namespace TextToSpeechCLR
{
    public struct PositionAndValue<T> : IPositionalEventRecord
    {
        private TimeSpan _audioPosition;
        private long _eventOrder;
        private string _message;
        private T _value;
        public TimeSpan AudioPosition { get { return _audioPosition; } }
        public long EventOrder { get { return _eventOrder; } }
        public T Value { get { return _value; } }
        public string Message { get { return _message; } }

        public PositionAndValue(TimeSpan audioPosition, long eventOrder, string message, T value)
        {
            _audioPosition = audioPosition;
            _eventOrder = eventOrder;
            _message = message;
            _value = value;
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
            return String.Format("{0} ({1}): {2}", _audioPosition, _eventOrder, _message);
        }

        public int CompareTo(IEventRecord other)
        {
            return (other == null) ? -1 : EventOrder.CompareTo(other.EventOrder);
        }
    }
}