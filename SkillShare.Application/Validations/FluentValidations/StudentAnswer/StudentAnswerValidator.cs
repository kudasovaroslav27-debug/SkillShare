using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.StudentAnswer;

namespace SkillShare.Application.Validations.FluentValidations.StudentAnswer;

public class StudentAnswerDtoValidator : AbstractValidator<StudentAnswerDto>
{
    public StudentAnswerDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100);
    }
}