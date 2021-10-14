using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System;

namespace Assignment4.Entities
{
    public class UserRepository : IUserRepository
    {

        private readonly KanbanContext _dbContext;

        public UserRepository(KanbanContext context)
        {
            _dbContext = context;
        }
        public (Response Response, int UserId) Create(UserCreateDTO user)
        {
            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return (Response.Created, newUser.Id);
        }

        public IReadOnlyCollection<UserDTO> ReadAll()
        {
            return _dbContext.Users.Select(x => new UserDTO(x.Id, x.Name, x.Email)).ToList();

        }

        public UserDTO Read(int userId)
        {
            User u = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            UserDTO o = new UserDTO(u.Id, u.Name, u.Email);
            return o;
        }

        public Response Update(UserUpdateDTO user)
        {
            User u = _dbContext.Users.FirstOrDefault(x => x.Id == user.Id);
            u.Email = user.Email;
            u.Id = user.Id;
            u.Name = user.Name;
            _dbContext.SaveChanges();
            return (Response.Updated);
        }

        public Response Delete(int userId, bool force = false)
        {
            User u = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

            if ((u.Tasks != null && u.Tasks.Any()) && !force)
            {
                return Response.Conflict;
            }
            else
            {
                _dbContext.Users.Remove(u);
                _dbContext.SaveChanges();
                return Response.Deleted;
            }
            
        }
    }
}