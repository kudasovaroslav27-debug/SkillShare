using MediatR;
using SkillShare.Application.Commands;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.Application.Handlers;

public class CreateCourseHandler(IBaseRepository<Course> courseRepository)
    : IRequestHandler<CreateCourseCommand, Course> 
{
    public async Task<Course> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        var course = new Course()
        {
            Title = request.Title,
            Description = request.Description,
            ParentId = (int?)request.ParentId,
            Price = request.Price,
            AuthorId = request.UserId
        };

        await courseRepository.CreateAsync(course);
        await courseRepository.SaveChangesAsync();

        return course; 
    }
}
