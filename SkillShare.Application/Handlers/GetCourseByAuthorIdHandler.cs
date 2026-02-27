using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Queries;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.Application.Handlers;

public class GetCourseByAuthorIdHandler(IBaseRepository<Course> courseRepository)
    : IRequestHandler<GetCourseByAuthorIdQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetCourseByAuthorIdQuery request, CancellationToken ct)
    {
        return await courseRepository.GetAll()
            .Where(x => x.AuthorId == request.AuthorId)
            .ProjectToType<CourseDto>()
            .ToArrayAsync(ct);
    }
}

