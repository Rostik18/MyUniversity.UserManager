﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyUniversity.UserManager.Repository.DbContext;

namespace MyUniversity.UserManager.Repository.Migrations
{
    [DbContext(typeof(UserManagerContext))]
    [Migration("20210215190003_UniversityTable")]
    partial class UniversityTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "6.0.0-preview.1.21102.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MyUniversity.UserManager.Repository.Entities.University.UniversityEntity", b =>
                {
                    b.Property<string>("TenantId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("TenantId");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Universities");

                    b.HasData(
                        new
                        {
                            TenantId = "100ba3c7-2e71-43e1-9c20-8b5f51bb930e",
                            Address = "University Street, 1, Lviv, Lviv region, 79000",
                            EmailAddress = "IvanFrankoUniversity@gmail.com",
                            Name = "Ivan Franko National University of Lviv",
                            PhoneNumber = "+380 (50) 123 4567"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
