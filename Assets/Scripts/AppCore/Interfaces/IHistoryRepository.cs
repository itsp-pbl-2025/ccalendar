#nullable enable
using Domain.Entity;
using Domain.Enum;

namespace AppCore.Interfaces
{
    public interface IHistoryRepository
    {
        public bool Update(HistoryContainer history);
        public bool InsertUpdate(HistoryContainer schedule);
        public bool Remove(HistoryType type);
        public HistoryContainer? Get(HistoryType type);
    }
}