using FluentValidation;
using SkillShare.Domain.Dto.CourseDto;

namespace SkillShare.Application.Validations.FluentValidations.Course;

public class UpdateCourseValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Id).NotEmpty();
    }
}
