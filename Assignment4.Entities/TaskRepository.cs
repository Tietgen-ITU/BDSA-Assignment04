using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _dbContext;

        public TaskRepository(KanbanContext context)
        {
            _dbContext = context;
        }

        public IReadOnlyCollection<TaskDTO> All()
        {
            return _dbContext.Tasks.Select(x => new TaskDTO
            {
                Id = x.Id,
                Title = x.Title,
                AssignedToId = x.AssignedTo != null ? x.AssignedTo.Id : null,
                Description = x.Description,
                State = x.State,
                Tags = x.Tags.Select(t => t.Name).ToList()
            }).ToList();
        }

        public int Create(TaskDTO task)
        {
            User user = _dbContext.Users.SingleOrDefault(u => u.Id == task.AssignedToId.GetValueOrDefault());

            _dbContext.Tasks.Add(new Task
            {
                Title = task.Title,
                AssignedTo = user,
                Description = task.Description,
                State = task.State,
                Tags = _dbContext.Tags.Where(x => task.Tags.Contains(x.Name)).ToList()
            });

            return _dbContext.SaveChanges();
        }

        public void Delete(int taskId)
        {
            Task task = _dbContext.Tasks.Single(t => t.Id == taskId);
            _dbContext.Tasks.Remove(task);

            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public TaskDetailsDTO FindById(int id)
        {
            Task task = _dbContext.Tasks.SingleOrDefault(t => t.Id == id);

            if (task == null)
                return null;

            return new TaskDetailsDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToId = task.AssignedTo.Id,
                AssignedToName = task.AssignedTo.Name,
                AssignedToEmail = task.AssignedTo.Email,
                Tags = task.Tags.Select(tag => tag.Name),
                State = task.State
            };
        }

        public void Update(TaskDTO task)
        {
            Task dbTask = _dbContext.Tasks.Single(x => x.Id == task.Id);
            User dbUser = _dbContext.Users.SingleOrDefault(x => x.Id == task.AssignedToId.GetValueOrDefault());

            // Update values...
            dbTask.Id = task.Id;
            dbTask.Title = task.Title;
            dbTask.Description = task.Description;
            dbTask.AssignedTo = dbUser;
            dbTask.State = task.State;
            dbTask.Tags = _dbContext.Tags.Where(x => task.Tags.Contains(x.Name)).ToList();

            _dbContext.SaveChanges();
        }
    }
}
