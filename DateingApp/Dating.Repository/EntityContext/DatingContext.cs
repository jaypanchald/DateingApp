using Dating.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dating.Repository.EntityContext
{
    public class DatingContext : IdentityDbContext<User, AppRole, int,
            IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
            IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=DatingApp;Trusted_Connection=True;MultipleActiveResultSets=true";

        public DatingContext() : base() { }
        public DatingContext(DbContextOptions options) : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photo { get; set; }

        public DbSet<Like> Like { get; set; }

        public DbSet<Message> Message { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


            builder.Entity<User>()
              .HasMany(u => u.UserRoles)
              .WithOne(u => u.User)
              .HasForeignKey(u => u.UserId)
              .IsRequired();

            builder.Entity<AppRole>()
             .HasMany(u => u.UserRoles)
             .WithOne(u => u.Role)
             .HasForeignKey(u => u.RoleId)
             .IsRequired();

            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
               .HasOne(u => u.Liker)
               .WithMany(u => u.Likees )
               .HasForeignKey(u => u.LikerId)
               .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
               .HasOne(u => u.Sender)
               .WithMany(u => u.MessagesSend)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
               .HasOne(u => u.Recipient)
               .WithMany(u => u.MessagesReceived)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
