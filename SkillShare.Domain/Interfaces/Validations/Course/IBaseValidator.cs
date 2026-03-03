using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Validations;

public interface IBaseValidator<in T> where T : class
{
    BaseResult ValidateOnNull(T model);
}
