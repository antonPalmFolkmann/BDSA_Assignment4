using System;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;

public interface IKanbanContext : IDisposable
    {
        DbSet<Task> Tasks { get; }
        DbSet<Tag> Tags { get; }
        DbSet<User> Users { get; }
        int SaveChanges();
    }