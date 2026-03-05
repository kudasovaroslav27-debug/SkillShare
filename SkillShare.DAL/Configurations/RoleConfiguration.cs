using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;


/// <summary>
/// Настройки для сущности 
/// <see cref="Role" />
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).ValueGeneratedOnAdd();
        builder.Property(r => r.Name).IsRequired().HasMaxLength(20);

        builder.HasIndex(r => r.Name).IsUnique();

        builder
           .HasMany(r => r.Users)
           .WithMany(u => u.Roles)
           .UsingEntity<UserRole>(
               join => join
                   .HasOne<User>()
                   .WithMany()
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade),
               join => join
                   .HasOne<Role>()
                   .WithMany()
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Cascade),
               join =>
                {
                    join.HasKey(ur => new { ur.UserId, ur.RoleId });
                    join.ToTable("UserRoles");
                }
             );

        builder.HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Instructor" },
            new Role { Id = 3, Name = "Student" }
        );
    }
}
