using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SkillShare.Domain.Dto.CourseDto;

namespace SkillShare.Application.Queries;

public class GetCourseByAuthorIdQuery(long authorId) : IRequest<IEnumerable<CourseDto>>
{
    public long AuthorId { get; set; } = authorId;
}; 
