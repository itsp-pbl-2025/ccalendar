#nullable enable
using Domain.Entity;
using Domain.Enum;

namespace AppCore.Interfaces
{
    public interface IHistoryRepository
    {
        /// <summary>
        /// すでにTypeの同じ記録が存在すれば、更新する。
        /// </summary>
        /// <param name="history">更新内容</param>
        /// <returns>記録があり、内容の変更もあるならtrue</returns>
        public bool Update(HistoryContainer history);
        
        /// <summary>
        /// 記録を挿入するか、更新する。
        /// </summary>
        /// <param name="history">更新内容</param>
        /// <returns>挿入がされたならtrue</returns>
        public bool InsertUpdate(HistoryContainer history);
        
        /// <summary>
        /// Typeが一致する記録があれば、削除する。
        /// </summary>
        /// <param name="type">削除するHistoryType</param>
        /// <returns>削除されたかどうか</returns>
        public bool Remove(HistoryType type);
        
        /// <summary>
        /// Typeが一致する記録があれば、返す。
        /// </summary>
        /// <param name="type">獲得するHistoryType</param>
        /// <returns>記録なければnull</returns>
        public HistoryContainer? Get(HistoryType type);
    }
}