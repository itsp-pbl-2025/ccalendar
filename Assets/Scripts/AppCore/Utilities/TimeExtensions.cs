using System;
using System.Runtime.InteropServices;

namespace AppCore.Utilities
{
    public static class TimeExtensions
    {
        /// <summary>
        /// UTC を東京時間に変換します。
        /// Windows では "Tokyo Standard Time"、それ以外では "Asia/Tokyo" を使います。
        /// </summary>
        public static DateTime ToTimeZoneTokyo(this DateTime utcDateTime)
        {
            var tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Tokyo Standard Time" : "Asia/Tokyo";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
        }
    }
}