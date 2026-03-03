using FluentValidation;
using SkillShare.Domain.Dto.StudentAnswer;

namespace SkillShare.Application.Validations.FluentValidations.StudentAnswer;

public class UpdateStudentAnswerDtoValidator : AbstractValidator<UpdateStudentAnswerDto>
{
    public UpdateStudentAnswerDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100)
            .Must(BeValidScore);
    }

    private bool BeValidScore(float score)
    {
        return Math.Abs(score * 10 - Math.Round(score * 10)) < 0.001f;
    }
}