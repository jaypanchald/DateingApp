using DatingApp.Model.Entity;
using DatingApp.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.IO.Pipelines;
using DatingApp.Repository.PagedList;
using DatingApp.Model.Helper;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace DatingApp.Repository.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<PagedList<User>> GetFilterUser(UserParams param);
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

        public async Task<PagedList<User>> GetFilterUser(UserParams param)
        {
            IQueryable<User> users = _contex.User.Include(i => i.Photos)
                .Where(w => w.Id != param.UserId &&
                w.Gender == param.Gender).OrderByDescending(o => o.LastActive)
                .AsQueryable();

            if (param.MinAge != 18 || param.MaxAge != 99)
            {
                var minDob = DateTime.UtcNow.AddYears(-param.MaxAge - 1);
                var maxDob = DateTime.UtcNow.AddYears(-param.MinAge);

                users = users.Where(w => w.DateOfBirth >= minDob && w.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(param.OrderBy))
            {
                switch (param.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(o => o.Created);
                        break;
                    default:
                        users = users.OrderByDescending(o => o.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, 
                param.PageNumber,
                param.PageSize);
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
