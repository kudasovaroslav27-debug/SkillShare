using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.StudentAnswer;

public record StudentAnswerDto(long Id, long StudentId, long QuestionId, long? TeacherId, float Score);