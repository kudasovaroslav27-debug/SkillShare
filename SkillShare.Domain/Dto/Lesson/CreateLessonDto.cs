using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.Lesson;

public record CreateLessonDto(int CourseId, string Name, string Content, int Number);

