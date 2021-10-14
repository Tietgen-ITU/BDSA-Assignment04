using System;
using System.Linq;
using System.Collections.Generic;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Assignment4.Entities.Tests
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _repository;

        public UserRepositoryTests()
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
            var Rasmus = new User {Id = 3, Name  = "Rasmus", Email ="onfisk@itu.dk" };

            var taskAdd = new Task { Id = 1, Title = "Add stuff", Description = "The task of adding stuff", Tags = new List<Tag>(), State = State.New, AssignedTo = Arne, Created = DateTime.UtcNow, StateUpdated = DateTime.UtcNow };
            var taskRemove = new Task { Id = 2, Title = "Remove stuff", Description = "The task of removing stuff", Tags = new List<Tag>(), State = State.Active, AssignedTo = Arne, Created = DateTime.UtcNow, StateUpdated = DateTime.UtcNow };
            var taskChange = new Task { Id = 3, Title = "Change stuff", Description = "The task of changing stuff", Tags = new List<Tag>(), State = State.Resolved, AssignedTo = Anton, Created = DateTime.UtcNow, StateUpdated = DateTime.UtcNow };

            context.Tasks.AddRange(
               taskAdd,
               taskRemove,
               taskChange
            );

            context.Users.AddRange(
                Rasmus
            );

            context.SaveChanges();

            _repository = new UserRepository(context);
        }

    


        [Fact]
        public void TestReadAllReturns3()
        {
            int expectedCount = 3;

            //When
            int resultCount = _repository.ReadAll().Count;

            //Then
            Assert.Equal(expectedCount, resultCount);

        }

        [Fact]
        public void TestRead(){

            int expectedId = 1;

            UserDTO d = _repository.Read(1);
            
            Assert.Equal(d.Id,expectedId);
          
        }

        [Fact]
        public void Update_given_existing_task_updates_user()
        {
            var user = new UserUpdateDTO
            {
                Id = 1,
                Name = "Anton",
                Email = "antbr@itu.dk"
            };

            _repository.Update(user);

            var updatedUser = _repository.Read(1);

            Assert.Equal(1, updatedUser.Id);
            Assert.Equal("Anton", updatedUser.Name);
            Assert.Equal("antbr@itu.dk", updatedUser.Email);
        }

        [Fact]
        public void Delete_user_by_id()
        {
            //When
            Response result = _repository.Delete(3);
            bool doesStillExist = _repository.ReadAll().Any(x => x.Id == 3);
            //Then
            Assert.False(doesStillExist);
            Assert.Equal(Response.Deleted, result);
        }

        
        [Fact]
        public void Create_returns_id_of_generated_user()
        {
            var user = new UserCreateDTO
            {
                Name = "Rasmus",
                Email = "ondfisk@itu.dk",
            };

            var actual = _repository.Create(user);
            Assert.Equal(Response.Created, actual.Response);
        }
    }
}
