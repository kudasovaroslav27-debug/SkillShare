using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Entities;

public class UserCourseGrade
{
    public long Id { get; set; }

    public int CourseId { get; set; }

    public float Grade { get; set; }

    public long UserId { get; set; }

    public User User { get; set; }

    public Course Course { get; set; }
}

