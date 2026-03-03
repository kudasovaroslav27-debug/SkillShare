namespace SkillShare.Domain.Dto.CourseDto;

public record CreateCourseDto(string Title, string Description, decimal Price, int ?ParentId);

