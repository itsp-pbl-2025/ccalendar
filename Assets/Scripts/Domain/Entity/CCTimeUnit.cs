using System;

namespace Domain.Entity
{
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
            if (value is < 1 or > 12)
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
            if (value is < 1 or > 31)
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
            if (value is < 0 or > 23)
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
            if (value is < 0 or > 59)
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
            if (value is < 0 or > 59)
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
}