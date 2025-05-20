namespace Domain.Entity
{
    public class CCDateTime
    {
        private readonly Year year;
        private readonly Month month;
        private readonly Day day;
        private readonly Hour hour;
        private readonly Minute minute;
        private readonly Second second;

        public CCDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            this.year = new Year(year);
            this.month = new Month(month);
            this.day = new Day(day);
            this.hour = new Hour(hour);
            this.minute = new Minute(minute);
            this.second = new Second(second);
        }
    }

    // 各ラッパークラス
    internal class Year
    {
        public int Value { get; }

        public Year(int year)
        {
            if (year < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Year cannot be negative.");
            }
            Value = year;
        }
    }

    internal class Month
    {
        public int Value { get; }

        public Month(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
            }
            Value = month;
        }
    }

    internal class Day
    {
        public int Value { get; }

        public Day(int day)
        {
            if (day < 1 || day > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "Day must be between 1 and 31.");
            }
            Value = day;
        }
    }

    internal class Hour
    {
        public int Value { get; }

        public Hour(int hour)
        {
            if (hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), "Hour must be between 0 and 23.");
            }
            Value = hour;
        }
    }

    internal class Minute
    {
        public int Value { get; }

        public Minute(int minute)
        {
            if (minute < 0 || minute > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), "Minute must be between 0 and 59.");
            }
            Value = minute;
        }
    }

    internal class Second
    {
        public int Value { get; }

        public Second(int second)
        {
            if (second < 0 || second > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(second), "Second must be between 0 and 59.");
            }
            Value = second;
        }
    }
}
