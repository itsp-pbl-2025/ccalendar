using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class DHistoryDao
    {
        public static DHistoryContainer FromDomain(this HistoryContainer history)
        {
            return new DHistoryContainer()
            {
                Id = (int)history.Type,
                Data = history.Data,
                UpdatedAt = history.UpdatedAt
            };
        }

        public static HistoryContainer ToDomain(this DHistoryContainer history)
        {
            return new HistoryContainer((HistoryType)history.Id, history.Data, history.UpdatedAt);
        }
    }
}