using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface ITaskRepository
    {
        public CCTask Insert(CCTask ccTask);
        public bool Update(CCTask ccTask);
        public bool InsertUpdate(CCTask ccTask);
        public bool Remove(CCTask ccTask);
        public ICollection<CCTask> GetAll();
    }
}
