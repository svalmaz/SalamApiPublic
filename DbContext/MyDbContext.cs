using ApiForKwork.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ApiForKwork.SqlDbContext
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<AdvertisementImage> AdvertisementImages { get; set; }
        public DbSet<ProfileImage> ProfileImages { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserMessageList> UserMessagesList { get; set; }

    }
}