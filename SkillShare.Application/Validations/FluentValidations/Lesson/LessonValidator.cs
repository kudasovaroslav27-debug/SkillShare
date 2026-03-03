using FluentValidation;
using SkillShare.Domain.Dto.Lesson;

namespace SkillShare.Application.Validations.FluentValidations.Lesson;

public class LessonDtoValidator : AbstractValidator<LessonDto>
{
    public LessonDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CourseId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(10000);

        RuleFor(x => x.Number)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);
    }
}