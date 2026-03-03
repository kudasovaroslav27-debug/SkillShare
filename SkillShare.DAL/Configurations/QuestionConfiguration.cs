using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id).ValueGeneratedOnAdd();
        builder.Property(q => q.Description).IsRequired().HasMaxLength(4000); 
        builder.Property(q => q.Difficult).HasConversion<int>().IsRequired();

        builder.HasMany(q => q.StudentAnswers)
               .WithOne(a => a.Question)
               .HasForeignKey(a => a.QuestionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.Lesson)
           .WithMany(l => l.Questions) 
           .HasForeignKey(q => q.LessonId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Question
            {
                Id = 1,
                Description = "Что такое инкапсуляция?",
                LessonId = 1, 
                CreatedAt = DateTime.UtcNow
            },
            new Question
            {
                Id = 2,
                Description = "В чем разница между классом и структурой?",
                LessonId = 1,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
