namespace Domain.Entity
{
    public class CCDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        private readonly Year year { get; } = new Year(year);
        private readonly Month month { get; } = new Month(month);
        private readonly Day day { get; } = new Day(day);
        private readonly Hour hour { get; } = new Hour(hour);
        private readonly Minute minute { get; } = new Minute(minute);
        private readonly Second second { get; } = new Second(second);
    }

    private class Year(int year): Year
    {
        private readonly int year { get; } = year;
        if (year < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year cannot be negative.");
        }
    }
    
    private class Month(int month): Month
    {
        private readonly int month { get; } = month;
        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }
    }
    
    private class Day(int day): Day
    {
        private readonly int day { get; } = day;
        if (day < 1 || day > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(day), "Day must be between 1 and 31.");
        }
    }
    
    private class Hour(int hour): Hour
    {
        private readonly int hour { get; } = hour;
        if (hour < 0 || hour > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hour), "Hour must be between 0 and 23.");
        }
    }
    
    private class Minute(int minute): Minute
    {
        private readonly int minute { get; } = minute;
        if (minute < 0 || minute > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(minute), "Minute must be between 0 and 59.");
        }
    }
    
    private class Second(int second): Second
    {
        private readonly int second { get; } = second;
        if (second < 0 || second > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(second), "Second must be between 0 and 59.");
        }
    }
}