using System;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment4.Core;
using System.Collections.Generic;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests
    {
        private readonly KanbanContext _context;
        private readonly TagRepository _repository;
        public TagRepositoryTests(){
            var connection = new SqliteConnection("filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            var tag1 = new Tag {Id = 1, Name = "Tag1", Tasks = new List<Task>()};
            var tag2 = new Tag {Id = 2, Name = "Tag2", Tasks = new List<Task>()};
            var tag3 = new Tag {Id = 3, Name = "Tag3", Tasks = new List<Task>()};
            var tags = new List<Tag>();

            var task1 = new Task { Id = 1, Title = "Task 1", AssignedTo = null, Description = "Task 1 description", State = Core.State.Active, Tags = tags};
            tag2.Tasks.Add(task1);
            var tasks = new List<Task>();
            tasks.Add(task1);

            var user1 = new User { Id = 1, Name = "Name 1", Email = "title1@mail.com", Tasks = tasks};
            task1.AssignedTo = user1;

            var user2 = new User { Id = 2, Name = "Name 2", Email = "title2@mail.com", Tasks = new List<Task>()};

            context.Add(tag1);
            context.Add(tag2);
            context.Add(tag3);

            context.SaveChanges();

            _context = context;
            _repository = new TagRepository(_context);
        }

        [Fact]
        public void Create_given_Tag_returns_response_and_Id()
        {
        //Given
        var tag = new TagCreateDTO{
            Name = "CreateTag"
        };
        
        //When
        var created = _repository.Create(tag);
        
        //Then
        Assert.Equal(4, created.TagId);
        Assert.Equal(Response.Created, created.Response);
        }

        [Fact]
        public void Create_given_Tag_already_existing_should_return_Conflict()
        {
        //Given
        var tag = new TagCreateDTO{
            Name = "Tag1"
        };
        
        //When
        var created = _repository.Create(tag);

        //Then
        Assert.Equal(Response.Conflict, created.Response);
        }

        [Fact]
        public void Update_given_not_existing_id_returns_notfound()
        {
        //Given
        var repo = new TagRepository(_context);
        
        //When
        var tag = new TagUpdateDTO{
            Id = 99
        };

        var updated = repo.Update(tag);
        
        //Then
        Assert.Equal(Response.NotFound, updated);
        }

        [Fact]
        public void Update_given_existing_id_returns_Updated()
        {
        //Given
        var repo = new TagRepository(_context);
        
        //When
        var tag = new TagUpdateDTO{
            Id = 3,
            Name = "tag3new"
        };

        var updated = repo.Update(tag);
        
        //Then
        Assert.Equal(Response.Updated, updated);
        }

        [Fact]
        public void Delete_given_existing_tagid_returns_deleted()
        {
        //Given
        var repo = new TagRepository(_context);
        
        //When
        var deleted = repo.Delete(1);
        
        //Then
        Assert.Equal(Response.Deleted, deleted);
        Assert.Null(_context.Tags.Find(1));
        }

        [Fact]
        public void Delete_tag_already_in_use_without_force_returns_conflict()
        {
        //Given
        var repo = new TagRepository(_context);
        
        //When
        var deleted = repo.Delete(2);
        
        //Then
        Assert.Equal(Response.Conflict, deleted);
        }

        [Fact]
        public void Delete_tag_already_in_use_with_force_returns_deleted()
        {
        //Given
        var repo = new TagRepository(_context);

        //When
        var deleted = repo.Delete(2,true);

        //Then
        Assert.Equal(Response.Deleted, deleted);
        }

        [Fact]
        public void Delete_given_not_existing_id_returns_notfound()
        {
        //Given
        var repo = new TagRepository(_context);
        
        //When
        var deleted = repo.Delete(99);
        
        //Then
        Assert.Equal(Response.NotFound, deleted);
        }

        [Fact]
        public void Read_given_existing_id_returns_tag()
        {
        //Given
        var tag = _repository.Read(3);
        
        //Then
        Assert.Equal("Tag3", tag.Name);
        Assert.Equal(3, tag.Id);
        }

        [Fact]
        public void Read_given_not_existing_Id_returns_Null()
        {
        //Given
        var tag = _repository.Read(99);
        
        //Then
        Assert.Null(tag);
        }

        [Fact]
        public void ReadAll_returns_all_tags()
        {
        //Given
        var allTags = _repository.ReadAll();

        //When
        
        //Then
        Assert.Collection(allTags,
                t => Assert.Equal(new TagDTO(1, "Tag1"), t),
                t => Assert.Equal(new TagDTO(2, "Tag2"), t),
                t => Assert.Equal(new TagDTO(3, "Tag3"), t)
            );
        }

        public void Dispose(){
            _context.Dispose();
        }
    }
}
