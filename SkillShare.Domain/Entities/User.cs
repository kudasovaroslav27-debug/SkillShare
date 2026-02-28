using SkillShare.Domain.Interfaces;

namespace SkillShare.Domain.Entities;

public class User : IEntityId<long>, IAuditable
{
    public long Id { get; set; }

    public string Login { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public int Age { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public virtual List<Role> Roles { get; set; }

    public virtual UserToken UserToken { get; set; }

    public virtual UserRole UserRole { get; set; }

    public virtual ICollection<UserCourseGrade> Grades { get; set; }

    public virtual ICollection<Course> Courses { get; set; }
}