using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly SqlConnection _connection;
        private readonly KanbanContext _context;

        public TaskRepository(SqlConnection connection, KanbanContext context)
        {
            _connection = connection;
            _context = context;
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            throw new System.NotImplementedException();
        }

        public Response Delete(int taskId)
        {
            throw new System.NotImplementedException();
        }

        public TaskDetailsDTO Read(int taskId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            var cmdText = @"SELECT * FROM Tasks";
            using var command = new SqlCommand(cmdText, _connection);

            OpenConnection();

            using var reader = command.ExecuteReader();

            var list = new List<TaskDTO>();
            var task = new Task();

            while (reader.Read())
            {
                list.Add(new TaskDTO
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    AssignedToName = reader.GetString("AssignedToName"),
                    Tags = _context
                            .Entry(task)
                            .Collection(e => e.Tags)
                            .Query()
                            .OrderBy(t => t.Name)
                            .Select(t => t.Name)
                            .ToList()
                            .AsReadOnly(),
                    State = (Core.State)Enum.Parse(typeof(State), "test")
                });
            }

            CloseConnection();

            ReadOnlyCollection<TaskDTO> readOnlyTasks = new ReadOnlyCollection<TaskDTO>(list);
            
            return readOnlyTasks;
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            throw new System.NotImplementedException();
        }

        public Response Update(TaskUpdateDTO task)
        {
            throw new System.NotImplementedException();
        }

        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
