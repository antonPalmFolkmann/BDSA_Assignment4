using System;
using System.Collections.Generic;

namespace Assignment4.Core
{
    public interface IUserRepository : IDisposable
    {
        (Response Response, int UserId) Create(UserCreateDTO user);
        IReadOnlyCollection<UserDTO> ReadAll();
        UserDTO Read(int userId);
        Response Update(UserUpdateDTO user);
        Response Delete(int userId, bool force = false);
    }
}