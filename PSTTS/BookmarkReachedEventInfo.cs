using System.Speech.Synthesis;

namespace PSTTS
{
    public class BookmarkReachedEventInfo : SpeechWaitEvent<BookmarkReachedEventInfo>, ISpeechWaitEvent
    {
        public BookmarkReachedEventInfo(BookmarkReachedEventArgs args)
        {
        }
    }
}