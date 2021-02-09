using Dating.Model.Entity;
using DatingApp.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DatingApp.Repository.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatingContext _contex;

        public AuthRepository(DatingContext contex)
        {
            _contex = contex;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _contex.User
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(f => f.UserName == username);
            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            CreatPassordHash(password, out byte[] passwordHach, out byte[] passwordSalt);

            user.PasswordHash = passwordHach;
            user.PasswordSalt = passwordSalt;

            await _contex.AddAsync(user);
            await _contex.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExits(string userName)
        {
            return await _contex.User.AnyAsync(a => a.UserName == userName);
        }
        private void CreatPassordHash(string password, out byte[] passwordHach, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHach = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        private bool VerifyPasswordHash(string password, byte[] passwordHach, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHach[i])
                        return false;
                }
            }
            return true;
        }
    }
}
