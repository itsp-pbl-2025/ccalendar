using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Entity;

namespace Domain.Api
{
    [Serializable]
    public record HolidayList
    {
        public List<HolidayRaw> holidays;
    }

    [Serializable]
    public record HolidayRaw
    {
        public string date;
        public string name;
        
        private static readonly Regex HolidayRawDateRegex = new(@"^(\d+)-(\d\d)-(\d\d)$");

        public bool TryParseDate(out CCDateOnly dateOnly)
        {
            dateOnly = new CCDateOnly(0, 1, 1);
            
            var str = Convert.ToString(date);
            var match = HolidayRawDateRegex.Match(str);
            if (!match.Success) return false;
            
            var mg = match.Groups;
            dateOnly = new CCDateOnly(int.Parse(mg[1].Value), int.Parse(mg[2].Value), int.Parse(mg[3].Value));
            return true;
        }
    }
}