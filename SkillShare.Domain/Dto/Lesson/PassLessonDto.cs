namespace SkillShare.Domain.Dto.Lesson;

public record PassLessonDto(long UserId, int LessonId, UserAnswerDto[] UserAnswers);
