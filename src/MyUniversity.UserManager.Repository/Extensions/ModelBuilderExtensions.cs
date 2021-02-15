using Microsoft.EntityFrameworkCore;
using MyUniversity.UserManager.Repository.Entities.University;

namespace MyUniversity.UserManager.Repository.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UniversityEntity>().HasData(
                new UniversityEntity
                {
                    TenantId = "100ba3c7-2e71-43e1-9c20-8b5f51bb930e",
                    Name = "Ivan Franko National University of Lviv",
                    Address = "University Street, 1, Lviv, Lviv region, 79000",
                    EmailAddress = "IvanFrankoUniversity@gmail.com",
                    PhoneNumber = "+380 (50) 123 4567"
                });

        }
    }
}
