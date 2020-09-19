using DatingApp.Model.Entity;
using DatingApp.Repository.EntityContext;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Repository.Repository
{
    public class Seed
    {
        private DatingContext _contex { get; }
        public Seed(DatingContext contex)
        {
            _contex = contex;
        }


        public void SeedData()
        {
            var json = System.IO.File.ReadAllText(@"E:/Angular/DateingApp/DateingApp/DatingApp.Repository/Data/seed.json");

            List<User> userDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);

            foreach (var userData in userDatas)
            {
                CreatPassordHash("password", out byte[] PasswordHash, out byte[] passwordSalt);

                userData.PasswordHash = PasswordHash;
                userData.PasswordSalt = passwordSalt;

                _contex.User.Add(userData);
                _contex.SaveChanges();

            }
            
        }

        private void CreatPassordHash(string password, out byte[] passwordHach, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHach = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
