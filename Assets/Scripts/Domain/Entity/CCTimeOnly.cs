using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Domain.Entity
{
    [JsonConverter(typeof(CCTimeOnlyConverter))]
    public readonly struct CCTimeOnly : IComparable<CCTimeOnly>, IEquatable<CCTimeOnly>
    {
        public Hour Hour { get; }
        public Minute Minute { get; }
        public Second Second { get; }

        public CCTimeOnly(int hour, int minute, int second)
        {
            Hour = new Hour(hour);
            Minute = new Minute(minute);
            Second = new Second(second);
        }
        
        public static CCTimeOnly Now => CCDateTime.Now.ToTimeOnly();

        public int CompareTo(CCTimeOnly other)
        {
            int hourComparison = Hour.CompareTo(other.Hour);
            if (hourComparison != 0) return hourComparison;

            int minuteComparison = Minute.CompareTo(other.Minute);
            if (minuteComparison != 0) return minuteComparison;

            return Second.CompareTo(other.Second);
        }
        
        public CCTimeOnly AddHours(double hours)
        {
            var dateTime = new CCDateTime(1, 1, 1, Hour.Value, Minute.Value, Second.Value)
                .AddHours(hours);
            return new CCTimeOnly(dateTime.Hour.Value, dateTime.Minute.Value, dateTime.Second.Value);
        }

        public CCTimeOnly AddMinutes(double minutes)
        {
            var dateTime = new CCDateTime(1, 1, 1, Hour.Value, Minute.Value, Second.Value)
                .AddMinutes(minutes);
            return new CCTimeOnly(dateTime.Hour.Value, dateTime.Minute.Value, dateTime.Second.Value);
        }

        public CCTimeOnly AddSeconds(double seconds)
        {
            var dateTime = new CCDateTime(1, 1, 1, Hour.Value, Minute.Value, Second.Value)
                .AddSeconds(seconds);
            return new CCTimeOnly(dateTime.Hour.Value, dateTime.Minute.Value, dateTime.Second.Value);
        }

        public bool Equals(CCTimeOnly other)
        {
            return Hour.Equals(other.Hour) && Minute.Equals(other.Minute) && Second.Equals(other.Second);
        }

        public override bool Equals(object obj)
        {
            return obj is CCTimeOnly other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hour, Minute, Second);
        }
    }
    
    internal class CCTimeOnlyConverter : JsonConverter<CCTimeOnly>
    {
        private static readonly Regex CCTimeOnlyRegex = new(@"^(\d\d):(\d\d):(\d\d)$");
        
        public override void WriteJson(JsonWriter writer, CCTimeOnly value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Hour.Value:d2}:{value.Minute.Value:d2}:{value.Second.Value:d2}");
        }

        public override CCTimeOnly ReadJson(JsonReader reader, Type objectType, CCTimeOnly existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value is null) return new CCTimeOnly();
            
            var str = Convert.ToString(reader.Value);
            var match = CCTimeOnlyRegex.Match(str);
            if (!match.Success) throw new FormatException($"Invalid CCTimeOnly format: {str}");
            
            var mg = match.Groups;
            return new CCTimeOnly(int.Parse(mg[1].Value), int.Parse(mg[2].Value), int.Parse(mg[3].Value));
        }
    }
}