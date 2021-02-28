using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyUniversity.UserManager.Repository.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUniversity.UserManager.Repository.DbConfigurations
{
    class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Role).IsUnique();
            builder.Property(e => e.Role)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
