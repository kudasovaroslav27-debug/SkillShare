using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;


/// <summary>
/// Настройки для сущности 
/// <see cref="UserCourseGrade" />
/// </summary>
public class UserCourseGradeConfiguration : IEntityTypeConfiguration<UserCourseGrade>
{
    public void Configure(EntityTypeBuilder<UserCourseGrade> builder)
    {
        builder.ToTable("UserCourseGrades");

        builder.HasKey(x => x.Id);
        builder.Property(ug => ug.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Grade)
            .IsRequired();

        builder.HasOne(x => x.Course)
            .WithMany(c => c.Grades) 
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.User)
            .WithMany(u => u.Grades) 
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new UserCourseGrade { Id = 1, UserId = 1, CourseId = 2, Grade = 4.8f },
            new UserCourseGrade { Id = 2, UserId = 3, CourseId = 2, Grade = 5.0f }
        );
    }
}

