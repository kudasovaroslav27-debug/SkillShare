using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;

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
            .WithMany(c => c.Grades) // Добавь ICollection<UserCourseGrade> Grades в класс Course
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // 2. Связь с юзером
        builder.HasOne(x => x.User)
            .WithMany(u => u.Grades) // Добавь ICollection<UserCourseGrade> Grades в класс User
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. ИСПРАВЛЕННЫЙ СИДИНГ ДАННЫХ
        builder.HasData(new UserCourseGrade
        {
            Id = 1,
            UserId = 1,   // Должен существовать в таблице Users
            CourseId = 1, // Должен существовать в таблице Courses
            Grade = 4.5f
        });
    }
}

