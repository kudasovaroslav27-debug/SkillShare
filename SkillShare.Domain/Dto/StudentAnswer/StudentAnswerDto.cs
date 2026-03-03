namespace SkillShare.Domain.Dto.StudentAnswer;

public record StudentAnswerDto(long Id, long StudentId, long QuestionId, long? TeacherId, float Score);