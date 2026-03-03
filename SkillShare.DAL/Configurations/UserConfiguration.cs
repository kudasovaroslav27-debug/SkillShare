using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.Name).IsRequired().HasMaxLength(30);
        builder.Property(u => u.LastName).HasMaxLength(30);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Password).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasMany(u => u.Roles).WithMany(r => r.Users).UsingEntity<UserRole>(
            join => join
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade),
            join => join
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade),
            join =>
            {
                join.HasKey(ur => new { ur.UserId, ur.RoleId });
                join.ToTable("UserRoles");
            });

        builder.HasOne(u => u.UserToken)
            .WithOne(t => t.User)
            .HasForeignKey<UserToken>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Courses)
        .WithOne(c => c.Author)
        .HasForeignKey(c => c.AuthorId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
           new User
           {
               Id = 1,
               Login = "admin",
               Email = "admin@skillshare.com",
               Name = "Admin User",
               Password = "admin123",
           },
           new User
           {
               Id = 2,
               Login = "instructor",
               Email = "instructor@skillshare.com",
               Name = "John Doe",
               Password ="instructor123"
           },
           new User
           {
               Id = 3,
               Login = "bob123",
               Email = "bob@example.com",
               Name = "Bob Smith",
               Password = "bob123"  
           }
       );
    }
}

