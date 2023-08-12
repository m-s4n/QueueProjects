using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ExcelCreator.Web.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace ExcelCreator.Web.Database
{
    public class AppIdentityDbContext:IdentityDbContext
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options): base(options) { }

        public DbSet<UserFile> UserFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            IdentityUser user = new()
            {
                UserName = "musti1",
                Email = "musti1@gmail.com",
            };
            PasswordHasher<IdentityUser> passHasher = new PasswordHasher<IdentityUser>();
            string passwordHash = passHasher.HashPassword(user: user, password: "musti1");
            user.PasswordHash = passwordHash;
            builder.Entity<IdentityUser>().HasData(user);
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

    }
}
