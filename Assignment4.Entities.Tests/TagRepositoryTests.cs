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
            var tags = new List<Tag>();
            context.Add(tag1);
            context.Add(tag2);

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
        Assert.Equal(3, created.TagId);
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
        public void Delete_given_existing_tagid_returns_deleted()
        {
        //Given
        var repo = new TagRepository(_context);
        
        //When
        var deleted = repo.Delete(1);
        
        //Then
        Assert.Equal(Response.Deleted, deleted);
        }
    }
}
