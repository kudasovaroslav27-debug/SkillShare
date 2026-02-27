using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SkillShare.Domain.Dto.CourseDto;

namespace SkillShare.Application.Queries;

public class GetCourseQuery(long courseId) : IRequest<CourseDto?>
{
    public long CourseId { get; set; } = courseId;
}

