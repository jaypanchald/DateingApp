﻿using DatingApp.Model.Entity;
using System.Threading.Tasks;

namespace DatingApp.Repository.Repository
{
    public interface IAuthRepository
    {
        Task<User> Login(string username, string password);
        Task<User> Register(User user, string password);
        Task<bool> UserExits(string userName);
    }
}
