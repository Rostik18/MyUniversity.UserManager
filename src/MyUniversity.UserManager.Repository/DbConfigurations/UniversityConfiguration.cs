using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyUniversity.UserManager.Repository.Entities.University;

namespace MyUniversity.UserManager.Repository.DbConfigurations
{
    public class UniversityConfiguration : IEntityTypeConfiguration<UniversityEntity>
    {
        public void Configure(EntityTypeBuilder<UniversityEntity> builder)
        {
            builder.ToTable("Universities");

            builder.HasKey(e => e.TenantId);

            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(e => e.Address).IsUnique();
            builder.Property(e => e.Address)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(e => e.EmailAddress).IsUnique();
            builder.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(e => e.PhoneNumber).IsUnique();
            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
