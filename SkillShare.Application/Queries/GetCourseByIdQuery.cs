using MediatR;
using SkillShare.Domain.Dto.CourseDto;

namespace SkillShare.Application.Queries;

public class GetCourseQuery(int courseId) : IRequest<CourseDto?>
{
    public int CourseId { get; set; } = courseId;
}

