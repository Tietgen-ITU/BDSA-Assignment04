using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _dbContext;

        public TaskRepository(KanbanContext context)
        {
            _dbContext = context;
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            return _dbContext.Tasks.Select(x => new TaskDTO(
                x.Id,
                x.Title,
                x.AssignedTo != null ? x.AssignedTo.Name : null,
                x.Tags.Select(t => t.Name).ToList(),
                x.State
            )).ToList();
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            User user = _dbContext.Users.SingleOrDefault(u => u.Id == task.AssignedToId.GetValueOrDefault());

            var newTask = new Task
            {
                Title = task.Title,
                AssignedTo = user,
                Description = task.Description,
                State = State.New,
                Tags = _dbContext.Tags.Where(x => task.Tags.Contains(x.Name)).ToList()
            };

            _dbContext.Tasks.Add(newTask);
            _dbContext.SaveChanges();

            return (Response.Created, newTask.Id);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public TaskDetailsDTO Read(int taskId)
        {
            Task task = _dbContext.Tasks.SingleOrDefault(t => t.Id == taskId);

            if (task == null)
                return null;

            return new TaskDetailsDTO(task.Id, 
                                        task.Title, 
                                        task.Description,
                                        DateTime.MinValue,
                                        task.AssignedTo?.Name,
                                        task.Tags?.Select(x => x.Name).ToList(),
                                        task.State,
                                        DateTime.MinValue);
            
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            return _dbContext.Tasks.Where(x => x.State == state)
                                    .Select(x => ConvertToTaskDTO(x))
                                    .ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            return _dbContext.Tasks.Where(x => x.Tags.Any(t => t.Name == tag))
                                    .Select(x => ConvertToTaskDTO(x))
                                    .ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            return _dbContext.Tasks.Where(x => x.AssignedTo.Id == userId)
                                    .Select(x => ConvertToTaskDTO(x))
                                    .ToList();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            return _dbContext.Tasks.Where(x => x.State == State.Removed)
                                    .Select(x => ConvertToTaskDTO(x))
                                    .ToList();
        }

        public Response Update(TaskUpdateDTO task)
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

            return Response.Updated;
        }

        public Response Delete(int taskId)
        {
            Task task = _dbContext.Tasks.Single(t => t.Id == taskId);
            _dbContext.Tasks.Remove(task);

            _dbContext.SaveChanges();
            return Response.Deleted;
        }

        private TaskDTO ConvertToTaskDTO(Task task)
        {
            return new TaskDTO(task.Id, 
                                task.Title,
                                task.AssignedTo?.Name,
                                task.Tags?.Select(x => x.Name).ToList(),
                                task.State);
        }
    }
}
