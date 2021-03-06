﻿using Microsoft.EntityFrameworkCore;
using MyUniversity.UserManager.Repository.Entities.University;
using MyUniversity.UserManager.Repository.Entities.User;
using MyUniversity.UserManager.Repository.Helpers;

namespace MyUniversity.UserManager.Repository.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UniversityEntity>()
                .HasData(
                new UniversityEntity
                {
                    TenantId = "11116258-9207-4bdc-b101-fb560cc8cb20",
                    Name = "Ivan Franko National University of Lviv",
                    Address = "University Street, 1, Lviv, Lviv region, 79000",
                    EmailAddress = "Ivan.Franko.University@lnu.edu.ua",
                    PhoneNumber = "+380 (50) 123 4567"
                });


            modelBuilder
                .Entity<RoleEntity>()
                .HasData(
                    new RoleEntity { Id = 1, Role = "SuperAdmin" },
                    new RoleEntity { Id = 2, Role = "Service" },
                    new RoleEntity { Id = 3, Role = "UniversityAdmin" },
                    new RoleEntity { Id = 4, Role = "Teacher" },
                    new RoleEntity { Id = 5, Role = "Student" }
                );

            PasswordHashHelper.CreatePasswordHash("Admin", out var hash, out var salt);
            modelBuilder
                .Entity<UserEntity>()
                .HasData(
                    new UserEntity
                    {
                        Id = 1,
                        FirstName = "Super",
                        LastName = "Admin",
                        EmailAddress = "Super.Admin@gmail.com",
                        PhoneNumber = "380 50 123 4567",
                        TenantId = null,
                        IsSoftDeleted = false,
                        PasswordHash = hash,
                        PasswordSalt = salt
                    },
                    new UserEntity
                    {
                        Id = 2,
                        FirstName = "UserManager",
                        LastName = "Service",
                        EmailAddress = "UserManager.Service@gmail.com",
                        PhoneNumber = "000",
                        TenantId = null,
                        IsSoftDeleted = false,
                        PasswordHash = hash,
                        PasswordSalt = salt
                    }
                );

            modelBuilder
                .Entity<UserRoleEntity>()
                .HasData(
                    new UserRoleEntity { RoleId = 1, UserId = 1 },
                    new UserRoleEntity { RoleId = 3, UserId = 1 },
                    new UserRoleEntity { RoleId = 4, UserId = 1 },
                    new UserRoleEntity { RoleId = 2, UserId = 2 }
                );
        }
    }
}
