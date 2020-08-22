using DatingApp.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository.EntityContext
{
    public class DatingContext : DbContext
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=DatingApp;Trusted_Connection=True;MultipleActiveResultSets=true";

        public DatingContext() : base() { }
        public DatingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
