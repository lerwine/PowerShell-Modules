using System;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.CertificateCryptography
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public short Year;
        public short Month;
        public short DayOfWeek;
        public short Day;
        public short Hour;
        public short Minute;
        public short Second;
        public short Milliseconds;

        internal static SystemTime Create(DateTime dateTime)
        {
            long fileTime = dateTime.ToFileTime();
            SystemTime systemTime;
            MarshalHelper.ErrorCheck(FileTimeToSystemTime(ref fileTime, out systemTime));
            return systemTime;
        }
    }
}