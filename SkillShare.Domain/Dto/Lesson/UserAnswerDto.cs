using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.Lesson;

public record UserAnswerDto(int QuestionId, string UserAnswer);