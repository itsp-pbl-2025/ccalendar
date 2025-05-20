using System;

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

        public CCDateTime(DateTime datetime)
        {
            this.year = new Year(datetime.Year);
            this.month = new Month(datetime.Month);
            this.day = new Day(datetime.Day);
            this.hour = new Hour(datetime.Hour);
            this.minute = new Minute(datetime.Minute);
            this.second = new Second(datetime.Second);
        }

        public static bool operator ==(CCDateTime a, CCDateTime b)
        {
            return a.Year == b.Year && a.Month == b.Month && a.Day == b.Day &&
                   a.Hour == b.Hour && a.Minute == b.Minute && a.Second == b.Second;
        }
        
        
        public static bool operator !=(CCDateTime a, CCDateTime b)
        {
            if (a == b) return false;
            return true;
        }
        
        public static bool operator <(CCDateTime a, CCDateTime b)
        {
            if (a.Year != b.Year) return a.Year < b.Year;
            if (a.Month != b.Month) return a.Month < b.Month;
            if (a.Day != b.Day) return a.Day < b.Day;
            if (a.Hour != b.Hour) return a.Hour < b.Hour;
            if (a.Minute != b.Minute) return a.Minute < b.Minute;
            return a.Second < b.Second;
        }
        
        public static bool operator >(CCDateTime a, CCDateTime b)
        {
            if (a == b) return false;
            if (a < b) return false;
            return true;
        }
        
        public static bool operator >=(CCDateTime a, CCDateTime b)
        {
            if (a > b && a == b) return true;
            return false;
        }

        public static bool operator <=(CCDateTime a, CCDateTime b)
        {
            if (a < b && a == b) return true;
            return false;
        }

        public int Year => year.Value;
        public int Month => month.Value;
        public int Day => day.Value;
        public int Hour => hour.Value;
        public int Minute => minute.Value;
        public int Second => second.Value;
    }
    
    class Year
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

    class Month
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

    class Day
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

    class Hour
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

    class Minute
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

    class Second
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
