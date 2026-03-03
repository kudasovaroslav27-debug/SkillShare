using SkillShare.Domain.Dto.UserCourseGrade;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

public interface IUserCourseGradeService
{
    /// <summary>
    /// Получение оценки по ID
    /// </summary>
    Task<DataResult<UserCourseGradeDto>> GetByIdAsync(long id, CancellationToken ct = default);

    /// <summary>
    /// Получение оценки по UserId
    /// </summary>
    Task<CollectionResult<UserCourseGradeDto>> GetByUserIdAsync(long userId, CancellationToken ct = default);

    /// <summary>
    /// Удаление оценки пользователя
    /// </summary>
    Task<DataResult<UserCourseGradeDto>> DeleteAsync(long id, CancellationToken ct = default);
}
