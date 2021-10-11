using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assignment4.Core;

namespace Assignment4.Entities.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();
            
            var tag1 = new Tag {Id = 1, Name = "Tag 1", Tasks = new List<Task>()};
            var tags = new List<Tag>();
            tags.Add(tag1);

            var task1 = new Task { Id = 1, Title = "Task 1", AssignedTo = null, Description = "Task 1 description", State = Core.State.Active, Tags = tags};
            tag1.Tasks.Add(task1);
            var tasks = new List<Task>();
            tasks.Add(task1);

            var user1 = new User { Id = 1, Name = "Name 1", Email = "title1@mail.com", Tasks = tasks};
            task1.AssignedTo = user1;

            context.Add(user1);

            context.SaveChanges();

            _context = context;
            _repository = new UserRepository(_context);
        }

        [Fact]
        public void Create_user_return_reponse_and_id() {
            // Arrange
            var user2 = new UserCreateDTO { Name = "Name 2", Email = "title2@mail.com" };

            // Act
            var created = _repository.Create(user2);

            // Assert
            Assert.Equal(2, created.UserId);
            Assert.Equal(Response.Created, created.Response);
        }

        [Fact]
        public void Delete_given_non_existing_id_returns_NotFound()
        {
            // Arrange
            var repository = new UserRepository(_context);

            // Act
            var deleted = repository.Delete(25);

            // Assert
            Assert.Equal(Response.NotFound, deleted);
        }

        [Fact]
        public void Delete_given_existing_id_deletes()
        {
            // Arrange
            var repository = new UserRepository(_context);

            // Act
            var deleted = repository.Delete(1);

            // Assert
            Assert.Equal(Response.Deleted, deleted);
            Assert.Null(_context.Users.Find(3));
        }

        [Fact]
        public void Update_given_non_existing_id_returns_NotFound()
        {
            var repository = new UserRepository(_context);

            var user = new UserUpdateDTO
            {
                Id = 14
            };

            var updated = repository.Update(user);

            Assert.Equal(Response.NotFound, updated);
        }

        [Fact]
        public void Update_updates_existing_user() 
        {
            var repository = new UserRepository(_context);

            var user = new UserUpdateDTO
            {
                Id = 1,
                Name = "Name 1 updated",
                Email = "title1@mail.com"
            };

            var updated = repository.Update(user);

            Assert.Equal(Response.Updated, updated);
        }


        public void Dispose()
        {
            _context.Dispose();
        }   
    }
}
