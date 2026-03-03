namespace SkillShare.Domain.Dto.Question;

public record CreateQuestionDto(int LessonId, string Description, string CorrectAnswer);