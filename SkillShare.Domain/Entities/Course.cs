using SkillShare.Domain.Interfaces;

namespace SkillShare.Domain.Entities;

public class Course : IEntityId<int>, IAuditable
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int? ParentId { get; set; }

    public long AuthorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public virtual Course? Parent { get; set; }

    public virtual ICollection<Course> Children { get; set; } 

    public virtual User Author { get; set; }

    public virtual List<Lesson> Lessons { get; set; }

    public virtual List<UserCourseGrade> Grades { get; set; }
}