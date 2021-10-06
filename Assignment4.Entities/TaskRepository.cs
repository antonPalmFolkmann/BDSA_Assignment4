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

        public TaskRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public IReadOnlyCollection<TaskDTO> All()
        {
            var cmdText = @"SELECT * FROM Tasks";
            using var command = new SqlCommand(cmdText, _connection);

            OpenConnection();

            using var reader = command.ExecuteReader();
            using var context = new KanbanContext();

            var list = new List<TaskDTO>();
            var task = new Task();

            while (reader.Read())
            {
                list.Add(new TaskDTO
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    AssignedToId = reader.GetInt32("AssignedToId"),
                    Tags = context
                            .Entry(task)
                            .Collection(e => e.Tags)
                            .Query()
                            .OrderBy(t => t.Name)
                            .Select(t => t.Name)
                            .ToList(),
                    State = (Core.State)Enum.Parse(typeof(State), "test")
                });
            }

            CloseConnection();

            ReadOnlyCollection<TaskDTO> readOnlyTasks = new ReadOnlyCollection<TaskDTO>(list);
            
            return readOnlyTasks;
        }

        public int Create(TaskDTO task)
        {
            var cmdText = @"INSERT Task (Id, Title, AssignedToId, Description, State, Tags)
                            VALUES (@Id, @Title, @AssignedToId, @Description, @State, @Tags);
                            SELECT LASTVAL()";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@AssignedToId", task.AssignedToId);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@State", task.State);
            command.Parameters.AddWithValue("@Tags", task.Tags);

            OpenConnection();

            var id = command.ExecuteScalar();

            CloseConnection();

            return (int)id;
        }

        public void Delete(int taskId)
        {
            var cmdText = @"DELETE Tasks WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", taskId);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }

        public TaskDetailsDTO FindById(int id)
        {
            var cmdText = @"SELECT Tasks WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", id);

            OpenConnection();

            using var reader = command.ExecuteReader();

            var description = reader.Read()
                ? new TaskDetailsDTO
                {
                    Description = reader.GetString("Description")
                }
                : null;

            CloseConnection();

            return description;
        }

        public void Update(TaskDTO task)
        {
            var cmdText = @"UPDATE Tasks SET
                            Id = @Id,
                            Title = @Title,
                            Description = @Description,
                            AssignedToId = @AssignedToId,
                            Tags = @Tags,
                            State = @State
                            WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@AssignedToId", task.AssignedToId);
            command.Parameters.AddWithValue("@Tags", task.Tags);
            command.Parameters.AddWithValue("@State", task.State);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
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
