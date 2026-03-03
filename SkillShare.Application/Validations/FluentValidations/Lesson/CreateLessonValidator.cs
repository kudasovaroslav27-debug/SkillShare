using FluentValidation;
using SkillShare.Domain.Dto.Lesson;

namespace SkillShare.Application.Validations.FluentValidations.Lesson;

public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
{
    public CreateLessonDtoValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Number);
    }
}