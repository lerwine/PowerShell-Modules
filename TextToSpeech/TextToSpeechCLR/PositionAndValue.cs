using System;

namespace TextToSpeechCLR
{
    public struct PositionAndValue<T> : IPositionalEventRecord
    {
        private TimeSpan _audioPosition;
        private long _eventOrder;
        private T _value;
        public TimeSpan AudioPosition { get { return _audioPosition; } }
        public long EventOrder { get { return _eventOrder; } }
        public T Value { get { return _value; } }
        public PositionAndValue(TimeSpan audioPosition, long eventOrder, T value)
        {
            _audioPosition = audioPosition;
            _eventOrder = eventOrder;
            _value = value;
        }

        public bool Equals(IEventRecord other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IEventRecord other)
        {
            throw new NotImplementedException();
        }
    }
}