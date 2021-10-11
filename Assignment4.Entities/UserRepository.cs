using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System;

namespace Assignment4.Entities
{
public class UserRepository : IUserRepository{

      private readonly KanbanContext _dbContext;

        public UserRepository(KanbanContext context)
        {
            _dbContext = context;
        }


            public Create User (UserCreateDTO user){
            var newUser = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return newUser;
        }
}