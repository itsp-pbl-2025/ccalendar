using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface ITaskRepository
    {
        public Task Insert(Task task);
        public bool Update(Task task);
        public bool InsertUpdate(Task task);
        public bool Remove(Task task);
        public ICollection<Task> GetAll();
    }
}
