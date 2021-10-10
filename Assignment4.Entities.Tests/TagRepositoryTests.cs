using System;
using System.Collections.Generic;
using System.Linq;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests
    {
        private readonly TagRepository _repository;

        public TagRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:"); //dotnet add package Microsoft.EntityFrameworkCore.Sqlite
            connection.Open(); //Need to explicitly open connection when working with SQLite
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            var taskAdd = new Task { Id = 1, Title = "Add stuff", Description = "The task of adding stuff", State = State.New };
            var taskRemove = new Task { Id = 2, Title = "Remove stuff", Description = "The task of removing stuff", State = State.Active };
            var taskChange = new Task { Id = 3, Title = "Change stuff", Description = "The task of changing stuff", State = State.Resolved };

            var tag1 = new Tag { Id = 1, Name = "a", Tasks = new List<Task> { taskAdd, taskRemove } };
            var tag2 = new Tag { Id = 2, Name = "b", Tasks = new List<Task> { taskChange } };
            var tag3 = new Tag { Id = 3, Name = "c" };

            context.Tags.AddRange(
               tag1,
               tag2,
               tag3
            );

            context.SaveChanges();

            _repository = new TagRepository(context);
        }

        [Fact]
        public void Create_given_new_tag_returns_Created()
        {
            TagCreateDTO tag = new TagCreateDTO
            {
                Name = "d"
            };
            Response expectedResponse = Response.Created;
            int expectedId = 4;

            (Response actualResponse, int actualId) = _repository.Create(tag);


            Assert.Equal(expectedId, actualId);
            Assert.Equal(expectedResponse, actualResponse);
        }



        [Fact]
        public void Create_given_existing_tag_returns_Conflict()
        {
            TagCreateDTO tag = new TagCreateDTO
            {
                Name = "a"
            };
            Response expectedResponse = Response.Conflict;
            int expectedId = 1;

            (Response actualResponse, int actualId) = _repository.Create(tag);

            Assert.Equal(expectedId, actualId);
            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact]
        public void ReadAll_returns_list_with_3_elements()
        {
            int expectedCount = 3;

            int actualCount = _repository.ReadAll().Count;

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void Read_given_existing_id_returns_tag()
        {
            TagDTO expected = new TagDTO(1, "a");

            TagDTO actual = _repository.Read(1);

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Read_given_nonexisting_id_returns_null()
        {
            TagDTO expected = null;

            TagDTO actual = _repository.Read(17);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Update_given_existing_tag_updates_tag()
        {
            Response expectedResponse = Response.Updated;
            TagUpdateDTO tag = new TagUpdateDTO
            {
                Id = 1,
                Name = "d"
            };


            Response actualRepsonse = _repository.Update(tag);
            var updatedTag = _repository.Read(1);

            Assert.Equal(tag.Id, updatedTag.Id);
            Assert.Equal(tag.Name, updatedTag.Name);
            Assert.Equal(expectedResponse, actualRepsonse);
        }

        [Fact]
        public void Update_given_nonexisting_tag_returns_NotFound()
        {
            Response expectedResponse = Response.NotFound;
            TagUpdateDTO tag = new TagUpdateDTO
            {
                Id = 4,
                Name = "d"
            };

            Response actualRepsonse = _repository.Update(tag);
            Assert.Equal(expectedResponse, actualRepsonse);
        }

        [Fact]
        public void Delete_given_unused_tag_removes_tag()
        {
            Response expectedResponse = Response.Deleted;

            Response actualResponse = _repository.Delete(3);
            bool doesStillExist = _repository.ReadAll().Any(x => x.Id == 3);

            Assert.False(doesStillExist);
            Assert.Equal(expectedResponse, actualResponse);

        }

        [Fact]
        public void Delete_given_tag_in_use_returns_conflict()
        {
            Response expectedResponse = Response.Conflict;

            Response actualResponse = _repository.Delete(1);
            bool doesStillExist = _repository.ReadAll().Any(x => x.Id == 1);

            Assert.True(doesStillExist);
            Assert.Equal(expectedResponse, actualResponse);

        }


        //Response Delete(int tagId, bool force = false);

    }
}