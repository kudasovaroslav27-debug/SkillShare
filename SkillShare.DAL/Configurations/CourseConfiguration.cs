using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Title).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).IsRequired();
        builder.Property(c => c.Price).HasDefaultValue(0.00m).IsRequired();

        builder.HasOne(c => c.Author)
               .WithMany(u => u.Courses)
               .HasForeignKey(c => c.AuthorId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Lessons)
               .WithOne(l => l.Course)
               .HasForeignKey(l => l.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.ParentId);

        builder.HasData(
           new Course { Id = 1, Title = "Программирование", AuthorId = 2, Description = "Корень", Price = 0 },
           new Course { Id = 2, Title = "C# Developer", ParentId = 1, AuthorId = 2, Description = "Ветка C#", Price = 100 },
           new Course { Id = 3, Title = "Web API", ParentId = 2, AuthorId = 2, Description = "Подкурс Web", Price = 50 },
           new Course { Id = 4, Title = "Java Developer", ParentId = 1, AuthorId = 2, Description = "Ветка Java", Price = 100 }
       );
    }
}

