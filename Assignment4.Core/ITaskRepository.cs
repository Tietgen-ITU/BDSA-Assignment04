using System;
using System.Collections.Generic;

namespace Assignment4.Core
{
    public interface ITaskRepository : IDisposable
    {
        IReadOnlyCollection<TaskDTO> All();
        
        int Create(TaskDTO task);

        void Delete(int taskId);

        TaskDetailsDTO FindById(int id);

        void Update(TaskDTO task);
    }
}
