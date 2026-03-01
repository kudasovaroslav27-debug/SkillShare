using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.CourseDto;

public record CreateCourseDto(string Title, string Description, decimal Price, int ?ParentId);

