using SkillShare.Domain.Interfaces;

namespace SkillShare.Domain.Entities;

public class Lesson : IEntityId<int>, IAuditable 
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string Name { get; set; }

    public string Content { get; set; }

    public int Number { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public Course Course { get; set; }

    public List<Question> Questions { get; set; }

}
