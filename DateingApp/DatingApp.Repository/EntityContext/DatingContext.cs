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

        public DbSet<Photo> Photo { get; set; }

        public DbSet<Like> Like { get; set; }

        public DbSet<Message> Message { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);


            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
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
