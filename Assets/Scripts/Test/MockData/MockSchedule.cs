using System.Collections.Generic;
using Infrastructure.Data.Schema;

namespace Test.MockData
{
    public static class MockSchedule
    {
        public static List<DSchedule> GetMockSchedules()
        {
            return new List<DSchedule>
            {
                new DSchedule
                {
                    Id = 1,
                    Type = Domain.Enum.ScheduleType.Date,
                    Title = "Date Schedule"
                },
                new DSchedule
                {
                    Id = 2,
                    Type = Domain.Enum.ScheduleType.Duration,
                    Title = "Duration Schedule"
                }
            };
        }
    }
}