using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).ValueGeneratedOnAdd();
        builder.Property(l => l.Name).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Content).IsRequired();

        builder.HasIndex(l => l.CourseId);

        builder.HasOne(l => l.Course)
               .WithMany(c => c.Lessons)
               .HasForeignKey(l => l.CourseId)
               .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(c => c.Questions)
               .WithOne(q => q.Lesson)
               .HasForeignKey(q => q.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Lesson
            {
                Id = 1,
                CourseId = 2,
                Name = "Введение в платформу .NET",
                Content = "Текст урока про CLR, JIT и сборку мусора.",
                Number = 1
            },
            new Lesson
            {
                Id = 2,
                CourseId = 2,
                Name = "Типы данных и переменные",
                Content = "Текст урока про значимые и ссылочные типы.",
                Number = 2,
            },
            new Lesson
            {
                Id = 3,
                CourseId = 3,
                Name = "Основы HTTP",
                Content = "Разбираем методы GET, POST, PUT, DELETE.",
                Number = 1,
            },
            new Lesson
            {
                Id = 4,
                CourseId = 1,
                Name = "Общее введение в программирование",
                Content = "Этот урок подходит для всех курсов программирования.",
                Number = 1
            }
        );
    }
}
