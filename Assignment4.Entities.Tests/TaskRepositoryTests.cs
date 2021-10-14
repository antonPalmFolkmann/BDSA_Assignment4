using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assignment4.Core;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests
    {
        private readonly KanbanContext _context;
        private readonly TaskRepository _repo;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();


            var Tag1 = new Tag { Id = 1, Name = "Tag1", Tasks = new List<Task>() };
            var Tag2 = new Tag { Id = 2, Name = "Tag2", Tasks = new List<Task>() };
            var Tag3 = new Tag { Id = 3, Name = "Tag3", Tasks = new List<Task>() };


            var task1 = new Task { Id = 1, Title = "Task1", Description = "Task1 Description", State = State.New, Tags = new[] { Tag1, Tag2 } };

            var user1 = new User { Id = 1, Name = "Name1", Email = "title1@mail.com", Tasks = null };
            var user2 = new User { Id = 2, Name = "Name2", Email = "title2@mail.com", Tasks = null };
            var user3 = new User { Id = 3, Name = "Name3", Email = "title3@mail.com", Tasks = null };
            var user4 = new User { Id = 4, Name = "Name4", Email = "title4@mail.com", Tasks = null };

            task1.AssignedTo = user1;

            context.Tasks.AddRange(
                task1,
                new Task { Id = 2, Title = "Task2", AssignedTo = user2, Description = "Task2 Description", State = State.New, Tags = new[] { Tag1, Tag2 } },
                new Task { Id = 3, Title = "Task3", AssignedTo = user3, Description = "Task3 Description", State = State.Removed, Tags = new[] { Tag1, Tag2, Tag3 } },
                new Task { Id = 4, Title = "Task4", AssignedTo = user4, Description = "Task4 Description", State = State.Active, Tags = new[] { Tag1, Tag2 } }
            );

            context.SaveChanges();

            _context = context;
            _repo = new TaskRepository(_context);
        }

        [Fact]
        public void Create_given_task_return_response_and_id()
        {
            //Arrrange
            var task = new TaskCreateDTO { Title = "Task5" };

            //Act
            var created = _repo.Create(task);

            //Assert
            Assert.Equal(5, created.TaskId);
            Assert.Equal(Response.Created, created.Response);
        }

        [Fact]
        public void Create_given_duplicate_task_return_conflict_response_and_id()
        {
            //Arrrange
            var task = new TaskCreateDTO { Title = "Task3" };

            //Act
            var created = _repo.Create(task);

            //Assert
            Assert.Equal(-1, created.TaskId);
            Assert.Equal(Response.Conflict, created.Response);
        }

        [Fact]
        public void ReadAll_returns_all_tasks()
        {
            //Arrange
            var tags = new HashSet<string>();

            var task1 = new TaskDTO(1, "Task1", "Name1", tags, State.New);
            var task2 = new TaskDTO(2, "Task2", "Name2", tags, State.New);
            var task3 = new TaskDTO(3, "Task3", "Name3", tags, State.Removed);
            var task4 = new TaskDTO(4, "Task4", "Name4", tags, State.Active);

            //Act
            var tasks = _repo.ReadAll();

            //Assert
            Assert.Collection(tasks,
                t => Assert.Equal(task1.Title, t.Title),
                t => Assert.Equal(task2.Title, t.Title),
                t => Assert.Equal(task3.Title, t.Title),
                t => Assert.Equal(task4.Title, t.Title)
            );
        }

        [Fact]
        public void ReadAllRemoved_returns_all_removed_tasks()
        {
            //Arrange
            var tags = new HashSet<string>();
            var task3 = new TaskDTO(3, "Task3", "Name3", tags, State.Removed);

            //Act
            var tasks = _repo.ReadAllRemoved();

            //Assert
            Assert.Collection(tasks,
                t => Assert.Equal(task3.Title, t.Title)
            );
        }

        [Fact]
        public void ReadAllByTag_given_tag_returns_task()
        {
            //Arrange
            var tags = new HashSet<string>();
            var task3 = new TaskDTO(3, "Task3", "Name3", tags, State.Removed);

            //Act
            var tasks = _repo.ReadAllByTag("Tag3");

            //Assert
            Assert.Collection(tasks,
                t => Assert.Equal(task3.Title, t.Title)
            );
        }

        [Fact]
        public void ReadAllByUser_given_user_returns_task()
        {
            //Arrange
            var tags = new HashSet<string>();
            var task2 = new TaskDTO(2, "Task2", "Name2", tags, State.Removed);

            //Act
            var tasks = _repo.ReadAllByUser(2);

            //Assert
            Assert.Collection(tasks,
                t => Assert.Equal(task2.Title, t.Title)
            );
        }

        [Fact]
        public void ReadAllByState_given_state_returns_related_tasks()
        {
            //Arrange
            var tags = new HashSet<string>();
            var task4 = new TaskDTO(4, "Task4", "Name4", tags, State.Active);

            //Act
            var tasks = _repo.ReadAllByState(State.Active);

            //Then
            Assert.Collection(tasks,
                t => Assert.Equal(task4.Title, t.Title)
            );
        }

        [Fact]
        public void Read_given_non_existing_id_returns_null()
        {
            //Arrange & Act
            var taskDTO = _repo.Read(66);

            //Assert
            Assert.Null(taskDTO);
        }

        [Fact]
        public void Read_given_existing_id_returns_Task()
        {
            //Arrange & Act
            var task = _repo.Read(2);

            //Assert
            Assert.Equal(2, task.Id);
            Assert.Equal("Task2", task.Title);
            Assert.Equal("Name2", task.AssignedToName);
            Assert.Equal("Task2 Description", task.Description);
            Assert.Equal(State.New, task.State);
        }

        [Fact]
        public void Update_given_non_existing_id_returns_NotFound()
        {
            //Arrange
            var taskDTO = new TaskUpdateDTO { Title = "Title6" };

            //Act
            var updated = _repo.Update(taskDTO);

            //Assert
            Assert.Equal(Response.NotFound, updated);
        }

        [Fact]
        public void Update_updates_existing_task()
        {
            //Arrange
            var task = new TaskUpdateDTO
            {
                Id = 1,
                Title = "Title1"
            };

            //Act
            var updated = _repo.Update(task);

            //Assert
            Assert.Equal(Response.Updated, updated);
        }

        [Fact]
        public void Delete_given_non_existing_id_returns_NotFound()
        {
            //Arrange & Act
            var deleted = _repo.Delete(42);

            //Assert
            Assert.Equal(Response.NotFound, deleted);
        }

        [Fact]
        public void Delete_given_existing_id_with_wrong_state_returns_Conflict()
        {
            //Arrange & Act
            var deleted = _repo.Delete(3);

            //Assert
            Assert.Equal(Response.Conflict, deleted);
            Assert.NotNull(_context.Tasks.Find(3));
        }

        [Fact]
        public void Delete_given_existing_id_with_state_new_deletes()
        {
            //Arrange & Act
            var deleted = _repo.Delete(2);

            //Assert
            Assert.Equal(Response.Deleted, deleted);
            Assert.Null(_context.Tasks.Find(2));
        }
    }
}
