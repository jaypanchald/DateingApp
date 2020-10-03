using DatingApp.Model.Entity;
using DatingApp.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.IO.Pipelines;

namespace DatingApp.Repository.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(int id);
        Task<bool> updateLastActive(int id);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly DatingContext _contex; 

        public UserRepository(DatingContext contex) : base(contex)
        {
            _contex = contex;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _contex.User.Include(i => i.Photos).ToListAsync();
        }

        public async Task<User> GetUser(int id)
        {
            return await _contex.User.Include(i => i.Photos).FirstOrDefaultAsync(f=>f.Id == id);
        }

        public async Task<bool> updateLastActive(int id)
        {
            var user = await _contex.User.FirstOrDefaultAsync(f => f.Id == id);
            if (user != null)
            {
                user.LastActive = DateTime.UtcNow;
                if(await Update(user))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
