using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.Question;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

public interface IQuestionService
{
    /// <summary>
    /// Получение вопроса по Id
    /// </summary>
    Task<DataResult<QuestionDto>> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Получение всех вопросов конкретного урока
    /// </summary>
    Task<CollectionResult<QuestionDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default);

    /// <summary>
    /// Создание вопроса к уроку
    /// </summary>
    Task<DataResult<QuestionDto>> CreateAsync(CreateQuestionDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление вопроса
    /// </summary>
    Task<DataResult<QuestionDto>> DeleteAsync(int id, CancellationToken ct = default);
}


