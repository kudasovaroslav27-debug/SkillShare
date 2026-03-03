namespace SkillShare.Domain.Dto.CourseDto;

public record CourseDto(string Title, string Description, decimal Price, long AuthorId, int Id);
