namespace SkillShare.Domain.Dto.Lesson;

public record LessonDto(int Id, int CourseId, string Name, string Content, int Number);
