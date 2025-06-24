using System;
using System.Collections.Generic;

namespace Domain.Entity
{
    public readonly struct CCTimeSpan : IComparable<CCTimeSpan>
    {
        private const int SecondsPerMinute = 60;
        private const int MinutesPerHour = 60;
        private const int HoursPerDay = 24;
        private const int DaysPerYear = 365;

        public double TotalSeconds { get; }
        
        private CCTimeSpan(double totalSeconds)
        {
            TotalSeconds = totalSeconds;
        }

        public static CCTimeSpan FromSeconds(double seconds) => new(seconds);
        public static CCTimeSpan FromMinutes(double minutes) => new(minutes * SecondsPerMinute);
        public static CCTimeSpan FromHours(double hours) => new(hours * SecondsPerMinute * MinutesPerHour);
        public static CCTimeSpan FromDays(double days) => new(days * SecondsPerMinute * MinutesPerHour * HoursPerDay);

        public static CCTimeSpan FromTimeSpan(TimeSpan timeSpan) => new(timeSpan.TotalSeconds);
        public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(TotalSeconds);

        public int Hours => (int)((TotalSeconds / (SecondsPerMinute * MinutesPerHour)) % HoursPerDay);
        public int Minutes => (int)((TotalSeconds / SecondsPerMinute) % MinutesPerHour);
        public int Seconds => (int)(TotalSeconds % SecondsPerMinute);
        public double TotalMinutes => TotalSeconds / SecondsPerMinute;
        public double TotalHours => TotalSeconds / (SecondsPerMinute * MinutesPerHour);
        public double TotalDays => TotalSeconds / (SecondsPerMinute * MinutesPerHour * HoursPerDay);
        public double TotalYears => TotalSeconds / (SecondsPerMinute * MinutesPerHour * HoursPerDay * DaysPerYear);

        public static CCTimeSpan operator +(CCTimeSpan left, CCTimeSpan right) =>
            new(left.TotalSeconds + right.TotalSeconds);

        public static CCTimeSpan operator -(CCTimeSpan left, CCTimeSpan right) =>
            new(left.TotalSeconds - right.TotalSeconds);

        public static CCTimeSpan operator *(CCTimeSpan timeSpan, double multiplier) =>
            new(timeSpan.TotalSeconds * multiplier);

        public static CCTimeSpan operator /(CCTimeSpan timeSpan, double divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();
            return new(timeSpan.TotalSeconds / divisor);
        }

        public static bool operator <(CCTimeSpan left, CCTimeSpan right) => left.CompareTo(right) < 0;
        public static bool operator <=(CCTimeSpan left, CCTimeSpan right) => left.CompareTo(right) <= 0;
        public static bool operator >(CCTimeSpan left, CCTimeSpan right) => left.CompareTo(right) > 0;
        public static bool operator >=(CCTimeSpan left, CCTimeSpan right) => left.CompareTo(right) >= 0;

        public int CompareTo(CCTimeSpan other) => TotalSeconds.CompareTo(other.TotalSeconds);

        public CCTimeSpan Abs() => TotalSeconds < 0 ? new(-TotalSeconds) : this;
        public CCTimeSpan Negate() => new(-TotalSeconds);

        public CCDateTime AddTo(CCDateTime dateTime) => 
            new(dateTime.ToDateTime().AddSeconds(TotalSeconds));

        public override string ToString()
        {
            if (TotalSeconds == 0) return "0 seconds";
            return $"{Hours}h {Minutes}m {Seconds}s";
        }

        public static CCTimeSpan Zero => new(0);
        public static CCTimeSpan MinValue => new(double.MinValue);
        public static CCTimeSpan MaxValue => new(double.MaxValue);

        public static bool TryParse(string s, out CCTimeSpan result)
        {
            result = Zero;
            if (string.IsNullOrWhiteSpace(s)) return false;

            try
            {
                if (double.TryParse(s, out double seconds))
                {
                    result = FromSeconds(seconds);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static CCTimeSpan Parse(string s)
        {
            if (TryParse(s, out CCTimeSpan result))
                return result;
            throw new FormatException("Input string was not in a correct format.");
        }

        public CCTimeSpan AddSeconds(double seconds) => new(TotalSeconds + seconds);
        public CCTimeSpan AddMinutes(double minutes) => AddSeconds(minutes * SecondsPerMinute);
        public CCTimeSpan AddHours(double hours) => AddSeconds(hours * SecondsPerMinute * MinutesPerHour);
        public CCTimeSpan AddDays(double days) => AddSeconds(days * SecondsPerMinute * MinutesPerHour * HoursPerDay);
        public CCTimeSpan AddYears(double years) => AddSeconds(years * SecondsPerMinute * MinutesPerHour * HoursPerDay * DaysPerYear);

        public static CCTimeSpan Second => FromSeconds(1);
        public static CCTimeSpan Minute => FromMinutes(1);
        public static CCTimeSpan Hour => FromHours(1);
        public static CCTimeSpan Day => FromDays(1);
    }
}
