using System;

namespace NLoad.App.Features.RunLoadTest
{
    public static class TimeSpanExtensions
    {
        public static string ToTimeString(this TimeSpan elapsed)
        {
            return string.Format("{0}:{1}:{2}", elapsed.ToString("hh"), elapsed.ToString("mm"), elapsed.ToString("ss"));
        }
    }
}