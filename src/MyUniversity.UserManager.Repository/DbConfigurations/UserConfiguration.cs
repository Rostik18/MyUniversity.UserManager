using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyUniversity.UserManager.Repository.Entities.User;

namespace MyUniversity.UserManager.Repository.DbConfigurations
{
    class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(e => e.EmailAddress).IsUnique();
            builder.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.IsSoftDeleted)
                .HasDefaultValue(false);

            builder
                .HasOne(e => e.University)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.PasswordHash)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(e => e.PasswordSalt)
                .HasMaxLength(128)
                .IsRequired();
        }
    }
}
