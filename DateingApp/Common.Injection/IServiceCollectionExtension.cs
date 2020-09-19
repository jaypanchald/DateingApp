using DatingApp.Repository.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Injection
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDatingLibrary(this IServiceCollection services)
        {
            services.AddTransient<IAuthRepository, AuthRepository>();
            //services.AddTransient<IRepository, Repository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}