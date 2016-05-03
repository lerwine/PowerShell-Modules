using System;

namespace TextToSpeechCLR
{
    public interface IEventRecord : IEquatable<IEventRecord>, IComparable<IEventRecord>
    {
        long EventOrder { get; }
    }
}