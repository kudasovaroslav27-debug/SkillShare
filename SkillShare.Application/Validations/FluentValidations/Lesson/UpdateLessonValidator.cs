using FluentValidation;
using SkillShare.Domain.Dto.Lesson;

namespace SkillShare.Application.Validations.FluentValidations.Lesson;

public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
{
    public UpdateLessonDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Number).GreaterThan(0);
    }
}
