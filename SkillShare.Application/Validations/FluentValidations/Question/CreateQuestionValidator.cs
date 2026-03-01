using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SkillShare.Domain.Dto.Question;

namespace SkillShare.Application.Validations.FluentValidations.Question;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(1000);

        RuleFor(x => x.LessonId)
            .GreaterThan(0);
    }
}