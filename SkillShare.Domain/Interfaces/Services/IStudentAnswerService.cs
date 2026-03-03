using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillShare.Domain.Dto.StudentAnswer;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

public interface IStudentAnswerService
{
    /// <summary>
    /// Удаление ответов студентов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<DataResult<StudentAnswerDto>> DeleteAsync(long id, CancellationToken ct = default);

    /// <summary>
    /// Получает все ответы конкретного студента
    /// </summary>
    Task<CollectionResult<StudentAnswerDto>> GetByUserIdAsync(long userId, CancellationToken ct = default);

    /// <summary>
    /// Получает все ответы на вопросы конкретного урока
    /// </summary>
    Task<CollectionResult<StudentAnswerDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);

    /// <summary>
    /// Получает все ответы на вопросы конкретного курса
    /// </summary>
    Task<CollectionResult<StudentAnswerDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default);
}
