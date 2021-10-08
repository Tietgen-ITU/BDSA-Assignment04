using System;
using System.Linq;
using System.Collections.ObjectModel;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests

{
    public class TaskRepositoryTests
    {
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:"); //dotnet add package Microsoft.EntityFrameworkCore.Sqlite
            connection.Open(); //Need to explicitly open connection when working with SQLite
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            var tag1 = new Tag { Id = 1, Name = "a" };
            var tag2 = new Tag { Id = 2, Name = "b" };
            var tag3 = new Tag { Id = 3, Name = "c" };

            var Arne = new User { Id = 1, Name = "Arne", Email = "snabela@snabela.com" };
            var Anton = new User { Id = 2, Name = "Antonius", Email = "snabela@crushader.net" };

            var taskAdd = new Task { Id = 1, Title = "Add stuff", Description = "The task of adding stuff", State = State.New, AssignedTo = Arne };
            var taskRemove = new Task { Id = 2, Title = "Remove stuff", Description = "The task of removing stuff", State = State.Active, AssignedTo = Arne };
            var taskChange = new Task { Id = 3, Title = "Change stuff", Description = "The task of changing stuff", State = State.Resolved, AssignedTo = Anton };

            context.Tasks.AddRange(
               taskAdd,
               taskRemove,
               taskChange
            );

            context.SaveChanges();

            _repository = new TaskRepository(context);
        }

        [Fact]
        public void TestAll()
        {
            int expectedCount = 3;

            //When
            int resultCount = _repository.All().Count;

            //Then
            Assert.Equal(expectedCount,resultCount);
        }

        [Fact]
        public void Delete_task_by_id()
        {
            Task task = new Task
            {
                Id = 3,
            };

            //When
            _repository.Delete(3);
            bool doesStillExist = _repository.All().Any(x => x.Id == 3);

            //Then
            Assert.False(doesStillExist);
        }

        [Fact]
        public void Find_task_by_id_returns_TaskDTO()
        {
            TaskDTO task = new TaskDTO
            {
                Id = 2,

            };

            var expected = new TaskDetailsDTO { Id = 2, Title = "Remove stuff", Description = "The task of removing stuff", State = State.Active, AssignedToEmail = "snabela@snabela.com", AssignedToName = "Arne", AssignedToId = 1 };
            TaskDetailsDTO result = _repository.FindById(2);

            Assert.Equal(expected, result);







        }





        [Fact]
        public void Create_returns_id_of_generated_task()
        {
            TaskDTO task = new TaskDTO
            {
                Title = "Eat something",
                Description = "The task of eating something",
                State = State.New,
                Tags = new Collection<string>{},
            };

            int actual = _repository.Create(task);
            

            Assert.Equal(4, actual);
        }

        [Fact]
        public void Update_given_existing_task_updates_task()
        {
            TaskDTO task = new TaskDTO
            {
                Id = 1,
                Title = "Add updated stuff",
                Description = "The task of adding updated stuff",
                State = State.Active,
                Tags = new Collection<string>()
            };

            _repository.Update(task);

            var updatedTask = _repository.FindById(1);
            Assert.Equal(1, updatedTask.Id);
            Assert.Equal("Add updated stuff", updatedTask.Title);
            Assert.Equal("The task of adding updated stuff", updatedTask.Description);
            Assert.Equal(State.Active, updatedTask.State);
        }








    }
}
