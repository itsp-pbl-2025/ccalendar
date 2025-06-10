using System;
using System.Collections.Generic;

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
    }
}