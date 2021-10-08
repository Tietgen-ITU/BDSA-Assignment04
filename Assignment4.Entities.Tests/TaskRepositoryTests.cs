using System;
using System.Linq;
using System.Collections.Generic;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;

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
        public void ReadAll_ReturnsCountOf3()
        {
            int expectedCount = 3;

            //When
            int resultCount = _repository.ReadAll().Count;

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
            bool doesStillExist = _repository.ReadAll().Any(x => x.Id == 3);

            //Then
            Assert.False(doesStillExist);
        }

        [Fact]
        public void Find_task_by_id_returns_TaskDTO()
        {

            var expected = new TaskDetailsDTO(2, 
                                            "Remove stuff", 
                                            "The task of removing stuff", 
                                            DateTime.Now, 
                                            "Arne", 
                                            new List<string>(), 
                                            State.Active, 
                                            DateTime.Now);

            TaskDetailsDTO result = _repository.Read(2);

            Assert.Equal(expected, result);
        }





        [Fact]
        public void Create_returns_id_of_generated_task()
        {
            var task = new TaskCreateDTO
            {
                Title = "Eat something",
                Description = "Assignment 4 holdet er cool",
                AssignedToId = 1,
                Tags = new List<string>{},
            };

            var actual = _repository.Create(task);
            

            Assert.Equal(4, actual.TaskId);
            Assert.Equal(Response.Created, actual.Response);
        }

        [Fact]
        public void Update_given_existing_task_updates_task()
        {
            var task = new TaskUpdateDTO
            {
                Id = 1,
                Title = "Add updated stuff",
                AssignedToId = 2,
                Description = "The task of adding updated stuff",
                State = State.Active,
                Tags = new List<string>()
            };

            _repository.Update(task);

            var updatedTask = _repository.Read(1);

            Assert.Equal(1, updatedTask.Id);
            Assert.Equal("Add updated stuff", updatedTask.Title);
            Assert.Equal("The task of adding updated stuff", updatedTask.Description);
            Assert.Equal(State.Active, updatedTask.State);
        }








    }
}
