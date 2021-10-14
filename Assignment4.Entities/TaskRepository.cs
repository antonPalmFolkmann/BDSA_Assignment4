using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IKanbanContext _context;

        public TaskRepository(IKanbanContext context)
        {
            _context = context;
        }

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            var duplicateTask = from c in _context.Tasks
                                where c.Title == task.Title
                                select new TaskDTO(
                                    c.Id,
                                    c.Title,
                                    c.AssignedTo.Name,
                                    (IReadOnlyCollection<string>)c.Tags,
                                    c.State
                                );

            if (duplicateTask.Count() != 0)
            {
                return (Response.Conflict, -1);
            }

            var entity = new Task { Title = task.Title };

            _context.Tasks.Add(entity);

            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int taskId)
        {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null)
            {
                return Response.NotFound;
            }

            if (entity.State != State.New)
            {
                return Response.Conflict;
            }

            _context.Tasks.Remove(entity);
            _context.SaveChanges();

            return Response.Deleted;
        }

        public TaskDetailsDTO Read(int taskId)
        {
            var tasks = from t in _context.Tasks
                        where t.Id == taskId
                        select new TaskDetailsDTO(
                            t.Id,
                            t.Title,
                            t.Description,
                            t.Created,
                            t.AssignedTo.Name,
                            t.Tags.Select(c => c.Name).ToHashSet(),
                            t.State,
                            t.StateUpdated
                        );

            return tasks.FirstOrDefault();
        }

        public IReadOnlyCollection<TaskDTO> ReadAll() =>
            _context.Tasks
                .Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, t.Tags.Select(t => t.Name).ToHashSet(), t.State))
                .ToList().AsReadOnly();

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) =>
            _context.Tasks
                .Where(t => t.State == state)
                .Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, t.Tags.Select(t => t.Name).ToHashSet(), t.State))
                .ToList().AsReadOnly();

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag) 
        {
            var tasks = new List<TaskDTO>();

            foreach (var task in _context.Tasks)
            {
                foreach (var t in task.Tags)
                {
                    if (t.Name == tag)
                    {
                        tasks.Add(new TaskDTO ( task.Id, task.Title, task.AssignedTo.Name, task.Tags.Select(t => t.Name).ToHashSet(), task.State ));
                    }
                }
            }

            return tasks;
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId) =>
            _context.Tasks
                .Where(t => t.AssignedTo.Id == userId)
                .Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, t.Tags.Select(t => t.Name).ToHashSet(), t.State))
                .ToList().AsReadOnly();

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved() =>
            _context.Tasks
                .Where(t => t.State == State.Removed)
                .Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, t.Tags.Select(t => t.Name).ToHashSet(), t.State))
                .ToList().AsReadOnly();

        public Response Update(TaskUpdateDTO task)
        {
            var entity = _context.Tasks.Find(task.Id);

            if (entity == null)
            {
                return Response.NotFound;
            }

            entity.Title = task.Title;

            _context.SaveChanges();

            return Response.Updated;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
