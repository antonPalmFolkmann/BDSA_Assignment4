using System;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Assignment4.Core;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests
    {
        [Fact]
        public void Create_given_Tag_returns_response_and_Id()
        {
        //Given
        using var connection = new SqliteConnection("filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        using var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        
        //When
        var repo = new TagRepository(context);
        var tag = new TagCreateDTO{
            Name = "CreateTag"
        };
        var created = repo.Create(tag);
        var response = Response.Created;
        
        //Then
        var tagDTO = new TagDTO(1, "CreateTag");
        Assert.Equal(1, created.TagId);
        Assert.Equal(response, created.Response);
        }

        [Fact]
        public void newTesttoBeImplemented()
        {
        //Given
        
        //When
        
        //Then
        }
    }
}
