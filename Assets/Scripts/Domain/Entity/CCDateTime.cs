using System;
using System.Diagnostics;

namespace Domain.Entity
{
    public readonly record struct CCDateTime : IComparable<CCDateTime>
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

        public CCDateTime(DateTime datetime)
        {
            Year = new Year(datetime.Year);
            Month = new Month(datetime.Month);
            Day = new Day(datetime.Day);
            Hour = new Hour(datetime.Hour);
            Minute = new Minute(datetime.Minute);
            Second = new Second(datetime.Second);
        }

        public DateTime ToDateTime()
        {
            return new DateTime(this.Year.Value, Month.Value, Day.Value, Hour.Value, Minute.Value, Second.Value);
        }

        public CCDateOnly ToDateOnly()
        {
            return new CCDateOnly(Year.Value, Month.Value, Day.Value);
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

        public static bool operator <(CCDateTime left, CCDateTime right) => left.CompareTo(right) < 0;
        public static bool operator >(CCDateTime left, CCDateTime right) => left.CompareTo(right) > 0;
        public static bool operator <=(CCDateTime left, CCDateTime right) => left.CompareTo(right) <= 0;
        public static bool operator >=(CCDateTime left, CCDateTime right) => left.CompareTo(right) >= 0;

        public int YearValue => Year.Value;
        public int MonthValue => Month.Value;
        public int DayValue => Day.Value;
        public int HourValue => Hour.Value;
        public int MinuteValue => Minute.Value;
        public int SecondValue => Second.Value;
    }

    public readonly record struct Year(int Value) : IComparable<Year>
    {
        public Year(int value) : this(value)
        {
            if (Value < 0)
                throw new ArgumentOutOfRangeException(nameof(Value), "Year cannot be negative.");
        }
        public int CompareTo(Year other) => Value.CompareTo(other.Value);
    }

    public readonly record struct Month(int Value) : IComparable<Month>
    {
        public Month(int value) : this(value)
        {
            if (Value < 1 || Value > 12)
                throw new ArgumentOutOfRangeException(nameof(Value), "Month must be between 1 and 12.");
        }
        public int CompareTo(Month other) => Value.CompareTo(other.Value);
    }

    public readonly record struct Day(int Value) : IComparable<Day>
    {
        public Day(int value) : this(value)
        {
            if (Value < 1 || Value > 31)
                throw new ArgumentOutOfRangeException(nameof(Value), "Day must be between 1 and 31.");
        }
        public int CompareTo(Day other) => Value.CompareTo(other.Value);
    }

    public readonly record struct Hour(int Value) : IComparable<Hour>
    {
        public Hour(int value) : this(value)
        {
            if (Value < 0 || Value > 23)
                throw new ArgumentOutOfRangeException(nameof(Value), "Hour must be between 0 and 23.");
        }
        public int CompareTo(Hour other) => Value.CompareTo(other.Value);
    }

    public readonly record struct Minute(int Value) : IComparable<Minute>
    {
        public Minute(int value) : this(value)
        {
            if (Value < 0 || Value > 59)
                throw new ArgumentOutOfRangeException(nameof(Value), "Minute must be between 0 and 59.");
        }
        public int CompareTo(Minute other) => Value.CompareTo(other.Value);
    }

    public readonly record struct Second(int Value) : IComparable<Second>
    {
        public Second(int value) : this(value)
        {
            if (Value < 0 || Value > 59)
                throw new ArgumentOutOfRangeException(nameof(Value), "Second must be between 0 and 59.");
        }
        public int CompareTo(Second other) => Value.CompareTo(other.Value);
    }

    public readonly record struct CCDateOnly : IComparable<CCDateOnly>
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
    }

    public readonly record struct CCTimeOnly : IComparable<CCTimeOnly>
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
    }
}
