#nullable enable
using AppCore.Interfaces;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.DAO;
using Infrastructure.Data.Schema;
using LiteDB;

namespace Infrastructure.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly ILiteCollection<DHistoryContainer> _col;
        
        public HistoryRepository(LiteDatabase db) => _col = db.GetCollection<DHistoryContainer>("history");
        
        public bool Update(HistoryContainer history)
        {
            var dHistory = history.FromDomain();
            return _col.Update(dHistory);
        }

        public bool InsertUpdate(HistoryContainer history)
        {
            var dHistory = history.FromDomain();
            return _col.Upsert(dHistory);
        }

        public bool Remove(HistoryType type)
        {
            return _col.Delete((int)type);
        }

        public HistoryContainer? Get(HistoryType type)
        {
            var dHistory = _col.FindById((int)type);
            return dHistory?.ToDomain() ?? null;
        }
    }
}