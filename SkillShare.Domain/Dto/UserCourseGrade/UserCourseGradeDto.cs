using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.UserCourseGrade;

public record UserCourseGradeDto(long Id, long UserId, int CourseId, float Grade);