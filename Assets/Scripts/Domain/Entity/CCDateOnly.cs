using System;

namespace Domain.Entity
{
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

        public CCDateOnly AddYears(int years) =>
            new CCDateTime(Year.Value, Month.Value, Day.Value, 0, 0, 0)
                .AddYears(years)
                .ToDateOnly();
        
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
    }
}