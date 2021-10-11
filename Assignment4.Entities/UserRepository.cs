using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;

namespace Assignment4.Entities
{
    public class UserRepository : IUserRepository
    {
        private readonly IKanbanContext _context;

        public UserRepository(IKanbanContext context) {
            _context = context;
        }

        public (Response Response, int UserId) Create(UserCreateDTO user)
        {
            var entity = new User { Name = user.Name, Email = user.Email };

            _context.Users.Add(entity);

            _context.SaveChanges();

            return (Response.Created, entity.Id);
        }

        public Response Delete(int userId, bool force = false)
        {
            var entity = _context.Users.Find(userId);

            if (entity == null) {
                return Response.NotFound;
            }

            _context.Users.Remove(entity);
            _context.SaveChanges();

            return Response.Deleted;
        }

        public UserDTO Read(int userId)
        {
            var users = from u in _context.Users
                        where u.Id == userId
                        select new UserDTO(
                            u.Id, 
                            u.Name,
                            u.Email
                        );
            
            return users.FirstOrDefault();
        }

        public IReadOnlyCollection<UserDTO> ReadAll() =>
            _context.Users
                    .Select(c => new UserDTO(c.Id, c.Name, c.Email))
                    .ToList().AsReadOnly();

        public Response Update(UserUpdateDTO user)
        {
            var entity = _context.Users.Find(user.Id);

            if (entity == null) {
                return Response.NotFound;
            }

            entity.Name = user.Name;
            entity.Email = user.Email;

            _context.SaveChanges();

            return Response.Updated;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
