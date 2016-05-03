using System;

namespace TextToSpeechCLR
{
    public interface IPositionalEventRecord : IEventRecord
    {
        TimeSpan AudioPosition { get; }
    }
}