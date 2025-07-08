using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Domain.Entity
{
    [JsonConverter(typeof(CCDateOnlyConverter))]
    public readonly struct CCDateOnly : IComparable<CCDateOnly>, IEquatable<CCDateOnly>
    {
        public Year Year { get; }
        public Month Month { get; }
        public Day Day { get; }

        public CCDateOnly(int year, int month, int day)
        {
            Year = new Year(year);
            Month = new Month(month);
            Day = new Day(day);
        }
        
        public static CCDateOnly Today => CCDateTime.Today.ToDateOnly();

        public int CompareTo(CCDateOnly other)
        {
            int yearComparison = Year.CompareTo(other.Year);
            if (yearComparison != 0) return yearComparison;

            int monthComparison = Month.CompareTo(other.Month);
            if (monthComparison != 0) return monthComparison;

            return Day.CompareTo(other.Day);
        }
        
        public CCDateOnly AddDays(int days) => 
            new CCDateTime(Year.Value, Month.Value, Day.Value, 0, 0, 0)
                .AddDays(days)
                .ToDateOnly();
        
        public CCDateOnly AddMonths(int months) =>
            new CCDateTime(Year.Value, Month.Value, Day.Value, 0, 0, 0)
                .AddMonths(months)
                .ToDateOnly();

        public CCDateOnly AddYears(int years) =>
            new CCDateTime(Year.Value, Month.Value, Day.Value, 0, 0, 0)
                .AddYears(years)
                .ToDateOnly();
        
        public static CCDateOnly Default => new(1, 1, 1);
        public static CCDateOnly MinValue => new(1, 1, 1);
        public static CCDateOnly MaxValue => new(9999, 12, 31);
        
        public DateTime ToDateTime()
        {
            return new DateTime(Year.Value, Month.Value, Day.Value).ToLocalTime().Date;
        }

        public bool Equals(CCDateOnly other)
        {
            return Year.Equals(other.Year) && Month.Equals(other.Month) && Day.Equals(other.Day);
        }

        public override bool Equals(object obj)
        {
            return obj is CCDateOnly other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month, Day);
        }

        public bool IsDefault()
        {
            return Year.Value == 1 && Month.Value == 1 && Day.Value == 1;
        }
    }
    
    internal class CCDateOnlyConverter : JsonConverter<CCDateOnly>
    {
        private static readonly Regex CCDateOnlyRegex = new(@"^(\d+)-(\d\d)-(\d\d)$");
        
        public override void WriteJson(JsonWriter writer, CCDateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.Year.Value}-{value.Month.Value:d2}-{value.Day.Value:d2}");
        }

        public override CCDateOnly ReadJson(JsonReader reader, Type objectType, CCDateOnly existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value is null) throw new JsonSerializationException("Cannot convert null value to a non-nullable type CCDateOnly.");
            
            var str = Convert.ToString(reader.Value);
            var match = CCDateOnlyRegex.Match(str);
            if (!match.Success) throw new FormatException($"Invalid CCDateOnly format: {str}");
            
            var mg = match.Groups;
            return new CCDateOnly(int.Parse(mg[1].Value), int.Parse(mg[2].Value), int.Parse(mg[3].Value));
        }
    }
}