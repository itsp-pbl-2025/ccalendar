using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Domain.Entity
{
    [JsonConverter(typeof(CCDateTimeConverter))]
    public readonly struct CCDateTime : IComparable<CCDateTime>, IEquatable<CCDateTime>
    {
        public Year Year { get; }
        public Month Month { get; }
        public Day Day { get; }
        public Hour Hour { get; }
        public Minute Minute { get; }
        public Second Second { get; }

        public CCDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            Year = new Year(year);
            Month = new Month(month);
            Day = new Day(day);
            Hour = new Hour(hour);
            Minute = new Minute(minute);
            Second = new Second(second);
        }
        
        public CCDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 0)
        {
        }


        public CCDateTime(DateTime datetime)
        {
            datetime = datetime.ToLocalTime();
            Year = new Year(datetime.Year);
            Month = new Month(datetime.Month);
            Day = new Day(datetime.Day);
            Hour = new Hour(datetime.Hour);
            Minute = new Minute(datetime.Minute);
            Second = new Second(datetime.Second);
        }
        
        public CCDateTime(CCDateOnly date, CCTimeOnly time)
            : this(date.Year.Value, date.Month.Value, date.Day.Value,
                time.Hour.Value, time.Minute.Value, time.Second.Value)
        {
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year.Value, Month.Value, Day.Value, Hour.Value, Minute.Value, Second.Value, DateTimeKind.Local);
        }

        public override string ToString()
        {
            return $"{Year.Value}-{Month.Value:d2}-{Day.Value:d2}T{Hour.Value:d2}:{Minute.Value:d2}:{Second.Value:d2}";
        }

        public string ToString(string format)
        {
            return ToDateTime().ToString(format);
        }

        public CCDateTime SetDate(CCDateOnly date)
        {
            return new CCDateTime(date.Year.Value, date.Month.Value, date.Day.Value, Hour.Value, Minute.Value, Second.Value);
        }

        public CCDateOnly ToDateOnly()
        {
            return new CCDateOnly(Year.Value, Month.Value, Day.Value);
        }

        public CCDateTime SetTime(CCTimeOnly time)
        {
            return new CCDateTime(Year.Value, Month.Value, Day.Value, time.Hour.Value, time.Minute.Value, time.Second.Value);
        }

        public CCTimeOnly ToTimeOnly()
        {
            return new CCTimeOnly(Hour.Value, Minute.Value, Second.Value);
        }

        public int CompareTo(CCDateTime other)
        {
            int yearComparison = Year.CompareTo(other.Year);
            if (yearComparison != 0) return yearComparison;

            int monthComparison = Month.CompareTo(other.Month);
            if (monthComparison != 0) return monthComparison;

            int dayComparison = Day.CompareTo(other.Day);
            if (dayComparison != 0) return dayComparison;

            int hourComparison = Hour.CompareTo(other.Hour);
            if (hourComparison != 0) return hourComparison;

            int minuteComparison = Minute.CompareTo(other.Minute);
            if (minuteComparison != 0) return minuteComparison;

            return Second.CompareTo(other.Second);
        }
        
        public CCDateTime Add(CCTimeSpan timeSpan) => new(ToDateTime().AddSeconds(timeSpan.TotalSeconds));
        
        public CCDateTime AddSeconds(double seconds) => new(ToDateTime().AddSeconds(seconds));
        public CCDateTime AddMinutes(double minutes) => new(ToDateTime().AddMinutes(minutes));
        public CCDateTime AddHours(double hours) => new(ToDateTime().AddHours(hours));
        public CCDateTime AddDays(int days) => new(ToDateTime().AddDays(days));
        public CCDateTime AddMonths(int months) => new(ToDateTime().AddMonths(months));

        public CCDateTime AddYears(int years)
        {
            try
            {
                return new CCDateTime(Year.Value + years, Month.Value, Day.Value, Hour.Value, Minute.Value, Second.Value);
            }
            catch (ArgumentOutOfRangeException) // 「うるう年の2/29」.AddYear(1)が落ちるのを回避
            {
                return new CCDateTime(ToDateTime().AddYears(years));
            }
        }

        public static CCTimeSpan operator -(CCDateTime left, CCDateTime right) =>
            CCTimeSpan.FromSeconds((left.ToDateTime() - right.ToDateTime()).TotalSeconds);
        public static CCDateTime operator +(CCDateTime dateTime, CCTimeSpan timeSpan) =>
            dateTime.Add(timeSpan);
        public static CCDateTime operator -(CCDateTime dateTime, CCTimeSpan timeSpan) =>
            dateTime.Add(timeSpan.Negate());
        
        public bool IsBetween(CCDateTime start, CCDateTime end)
        {
            if (end < start)
            {
                throw new ArgumentException("End date must be greater than or equal to start date.");
            }

            return this >= start && this <= end;
        }
        
        public static bool operator ==(CCDateTime left, CCDateTime right) => left.CompareTo(right) == 0;
        public static bool operator !=(CCDateTime left, CCDateTime right) => left.CompareTo(right) != 0;
        public static bool operator <(CCDateTime left, CCDateTime right) => left.CompareTo(right) < 0;
        public static bool operator >(CCDateTime left, CCDateTime right) => left.CompareTo(right) > 0;
        public static bool operator <=(CCDateTime left, CCDateTime right) => left.CompareTo(right) <= 0;
        public static bool operator >=(CCDateTime left, CCDateTime right) => left.CompareTo(right) >= 0;
        
        public static CCDateTime Today => new(DateTime.Today);
        public static CCDateTime Now => new(DateTime.Now);
        public static CCDateTime MinValue => new(DateTime.MinValue);
        public static CCDateTime MaxValue => new(DateTime.MaxValue);
        public DayOfWeek DayOfWeek => ToDateTime().DayOfWeek;

        public int YearValue => Year.Value;
        public int MonthValue => Month.Value;
        public int DayValue => Day.Value;
        public int HourValue => Hour.Value;
        public int MinuteValue => Minute.Value;
        public int SecondValue => Second.Value;

        public bool Equals(CCDateTime other)
        {
            return Year.Equals(other.Year) && Month.Equals(other.Month) && Day.Equals(other.Day) &&
                   Hour.Equals(other.Hour) && Minute.Equals(other.Minute) && Second.Equals(other.Second);
        }

        public override bool Equals(object obj)
        {
            return obj is CCDateTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month, Day, Hour, Minute, Second);
        }
    }

    internal class CCDateTimeConverter : JsonConverter<CCDateTime>
    {
        private static readonly Regex CCDateTimeRegex = new(@"^(\d+)-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d)$");
        
        public override void WriteJson(JsonWriter writer, CCDateTime value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Year.Value}-{value.Month.Value:d2}-{value.Day.Value:d2}T{value.Hour.Value:d2}:{value.Minute.Value:d2}:{value.Second.Value:d2}");
        }

        public override CCDateTime ReadJson(JsonReader reader, Type objectType, CCDateTime existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value is null) throw new JsonSerializationException("Cannot convert null value to a non-nullable type CCDateTime.");
            
            var str = Convert.ToString(reader.Value);
            var match = CCDateTimeRegex.Match(str);
            if (!match.Success) throw new FormatException($"Invalid CCDateTime format: {str}");
            
            var mg = match.Groups;
            return new CCDateTime(
                int.Parse(mg[1].Value), int.Parse(mg[2].Value), int.Parse(mg[3].Value),
                int.Parse(mg[4].Value), int.Parse(mg[5].Value),  int.Parse(mg[6].Value));
        }
    }
}