namespace SkillShare.Domain.Dto.Lesson;

public record CreateLessonDto(int CourseId, string Name, string Content, int Number);

