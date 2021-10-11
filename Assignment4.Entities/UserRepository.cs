using System.Collections.Generic;
using Assignment4.Core;

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
            throw new System.NotImplementedException();
        }

        public UserDTO Read(int userId)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<UserDTO> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public Response Update(UserUpdateDTO user)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
