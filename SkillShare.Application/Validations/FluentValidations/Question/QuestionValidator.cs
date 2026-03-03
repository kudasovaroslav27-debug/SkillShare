using FluentValidation;
using SkillShare.Domain.Dto;

namespace SkillShare.Application.Validations.FluentValidations.Question;

public class QuestionDtoValidator : AbstractValidator<QuestionDto>
{
    public QuestionDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(1000);

        RuleFor(x => x.LessonId)
            .GreaterThan(0);
    }
}