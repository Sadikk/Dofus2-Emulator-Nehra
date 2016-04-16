using System;

namespace Stump.Core.Extensions
{
    public static class TimeExtensions
    {
        private static readonly DateTime _baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long GetUnixTimeStampLong(this DateTime date)
        {
            return (long)(date - _baseDateTime.ToLocalTime()).TotalMilliseconds;
        }

        public static int GetUnixTimeStamp(this DateTime date)
        {
            return (int)(date - _baseDateTime.ToLocalTime()).TotalSeconds;
        }
    }
}