namespace SkillShare.Domain.Dto.UserCourseGrade;

public record UserCourseGradeDto(long Id, long UserId, int CourseId, float Grade);