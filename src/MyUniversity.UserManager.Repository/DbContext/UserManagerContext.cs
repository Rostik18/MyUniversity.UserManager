using Microsoft.EntityFrameworkCore;
using MyUniversity.UserManager.Repository.DbConfigurations;
using MyUniversity.UserManager.Repository.Entities.University;
using MyUniversity.UserManager.Repository.Entities.User;
using MyUniversity.UserManager.Repository.Extensions;

namespace MyUniversity.UserManager.Repository.DbContext
{
    public class UserManagerContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public UserManagerContext(DbContextOptions<UserManagerContext> options) : base(options)
        {
        }

        public DbSet<UniversityEntity> Universities { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UniversityConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.Seed();
        }
    }
}
