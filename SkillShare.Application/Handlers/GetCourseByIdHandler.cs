using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Queries;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.Application.Handlers;

public class GetCourseByIdHandler(IBaseRepository<Course> courseRepository) : IRequestHandler<GetCourseQuery, CourseDto?>
{
    public async Task<CourseDto?> Handle(GetCourseQuery request, CancellationToken ct)
    {
        return await courseRepository.GetAll()
             .Where(x => x.Id == request.CourseId)
             .ProjectToType<CourseDto>()
             .FirstOrDefaultAsync(ct);
    }
}

