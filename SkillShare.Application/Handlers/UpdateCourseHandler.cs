using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Commands;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.Application.Handlers;



public class UpdateCourseHandler(IBaseRepository<Course> courseRepository)
    : IRequestHandler<UpdateCourseCommand, Course>
{
    public async Task<Course> Handle(UpdateCourseCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetAll()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct); 

        course.Title = request.Title;
        course.Description = request.Description;
        course.Price = request.Price;

        var updatedCourse = courseRepository.Update(course); 
        await courseRepository.SaveChangesAsync();

        return updatedCourse; 
    }
}

