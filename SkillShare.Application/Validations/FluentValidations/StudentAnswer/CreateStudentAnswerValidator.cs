using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SkillShare.Domain.Dto.StudentAnswer;

namespace SkillShare.Application.Validations.FluentValidations.StudentAnswer;

public class CreateStudentAnswerDtoValidator : AbstractValidator<CreateStudentAnswerDto>
{
    public CreateStudentAnswerDtoValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .GreaterThan(0);
    }
}