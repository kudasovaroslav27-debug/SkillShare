using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;


/// <summary>
/// Настройки для сущности 
/// <see cref="StudentAnswer" />
/// </summary>
public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
{
    public void Configure(EntityTypeBuilder<StudentAnswer> builder)
    {
        builder.ToTable("StudentAnswers");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.Id).ValueGeneratedOnAdd();
        builder.Property(sa => sa.StudentId).IsRequired();
        builder.Property(sa => sa.QuestionId).IsRequired();
        builder.Property(sa => sa.TeacherId).IsRequired(false);
        builder.Property(sa => sa.Score)
               .IsRequired()
               .HasDefaultValue(0.0f);

        builder.HasIndex(sa => sa.StudentId);
        builder.HasIndex(sa => sa.QuestionId);
        builder.HasIndex(sa => sa.TeacherId);

        builder.HasOne(sa => sa.Student)
               .WithMany() 
               .HasForeignKey(sa => sa.StudentId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(sa => sa.Teacher)
               .WithMany() 
               .HasForeignKey(sa => sa.TeacherId)
               .OnDelete(DeleteBehavior.NoAction); 

        builder.HasOne(sa => sa.Question)
               .WithMany() 
               .HasForeignKey(sa => sa.QuestionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new StudentAnswer
        {
            Id = 1,
            StudentId = 3,
            QuestionId = 1,
            TeacherId = 2,
            Score = 5.0f,
        });
    }
}
