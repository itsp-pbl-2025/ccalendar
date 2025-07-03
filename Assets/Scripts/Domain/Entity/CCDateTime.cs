using System;

namespace Domain.Entity
{
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

    public readonly struct Year : IComparable<Year>, IEquatable<Year>
    {
        public int Value { get; }

        public Year(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Year cannot be negative.");
            Value = value;
        }
        public int CompareTo(Year other) => Value.CompareTo(other.Value);

        public bool Equals(Year other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Year other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }


    public readonly struct Month : IComparable<Month>, IEquatable<Month>
    {
        public int Value { get; }

        public Month(int value)
        {
            if (value < 1 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "Month must be between 1 and 12.");
            Value = value;
        }
        public int CompareTo(Month other) => Value.CompareTo(other.Value);

        public bool Equals(Month other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Month other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public readonly struct Day : IComparable<Day>, IEquatable<Day>
    {
        public int Value { get; }

        public Day(int value)
        {
            if (value < 1 || value > 31)
                throw new ArgumentOutOfRangeException(nameof(value), "Day must be between 1 and 31.");
            Value = value;
        }
        public int CompareTo(Day other) => Value.CompareTo(other.Value);

        public bool Equals(Day other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Day other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public readonly struct Hour : IComparable<Hour>, IEquatable<Hour>
    {
        public int Value { get; }

        public Hour(int value)
        {
            if (value < 0 || value > 23)
                throw new ArgumentOutOfRangeException(nameof(value), "Hour must be between 0 and 23.");
            Value = value;
        }
        public int CompareTo(Hour other) => Value.CompareTo(other.Value);

        public bool Equals(Hour other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Hour other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public readonly struct Minute : IComparable<Minute>, IEquatable<Minute>
    {
        public int Value { get; }

        public Minute(int value)
        {
            if (value < 0 || value > 59)
                throw new ArgumentOutOfRangeException(nameof(value), "Minute must be between 0 and 59.");
            Value = value;
        }
        public int CompareTo(Minute other) => Value.CompareTo(other.Value);

        public bool Equals(Minute other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Minute other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public readonly struct Second : IComparable<Second>, IEquatable<Second>
    {
        public int Value { get; }

        public Second(int value)
        {
            if (value < 0 || value > 59)
                throw new ArgumentOutOfRangeException(nameof(value), "Second must be between 0 and 59.");
            Value = value;
        }
        public int CompareTo(Second other) => Value.CompareTo(other.Value);

        public bool Equals(Second other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Second other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
    
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
        
        public static CCTimeOnly Now => CCDateTime.Today.ToTimeOnly();

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
}