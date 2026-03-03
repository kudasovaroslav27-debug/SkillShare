using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

public interface ICourseService
{
    /// <summary>
    /// Получение всех курсов
    /// </summary>
    Task<DataResult<CourseDto>> GetByIdAsync(int courseId, CancellationToken ct = default);

    /// <summary>
    /// Получение курса по Id учителя
    /// </summary>
    Task<CollectionResult<CourseDto>> GetByAuthorIdAsync(long AuthorId, CancellationToken ct = default);

    /// <summary>
    /// Создание курса с базовыми параметрами
    /// </summary>
    Task<DataResult<CourseDto>> CreateAsync(long userId, CreateCourseDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление курса по Id 
    /// </summary>
    Task<DataResult<CourseDto>> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Обновление курса
    /// </summary>
    Task<DataResult<CourseDto>> UpdateAsync(UpdateCourseDto dto, CancellationToken ct = default);
}
