using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillShare.Domain.Dto.Lesson;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

public interface ILessonService
{
    /// <summary>
    /// Получение урока по Id
    /// </summary>
    Task<DataResult<LessonDto>> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Получение списка уроков для конкретного курса
    /// </summary>
    Task<CollectionResult<LessonDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);

    /// <summary>
    /// Создание урока
    /// </summary>
    Task<DataResult<LessonDto>> CreateAsync(CreateLessonDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление урока
    /// </summary>
    Task<DataResult<LessonDto>> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Обновление данных урока
    /// </summary>
    Task<DataResult<LessonDto>> UpdateAsync(UpdateLessonDto dto, CancellationToken ct = default);

    /// <summary>
    /// Обрабатывает прохождение урока студентом, проверяет ответы и выставляет баллы.
    /// </summary>
    Task<DataResult<float>> PassLessonAsync(PassLessonDto dto, CancellationToken ct = default);
}
