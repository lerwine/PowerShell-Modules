using System;
using System.Speech.Synthesis;

namespace PSTTS
{
    public class BookmarkReachedEventData : AsyncCompletedEventLogItem<BookmarkReachedEventArgs>
    {
        public override TimeSpan AudioPosition { get { return Args.AudioPosition; } }
        public override string Bookmark { get { return Args.Bookmark; } }
        public BookmarkReachedEventData(BookmarkReachedEventArgs e, SpeechEventHandler.SpeechContext context) : base(e, context) { }
    }
}