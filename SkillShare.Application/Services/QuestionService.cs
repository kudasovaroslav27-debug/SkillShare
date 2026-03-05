using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.Question;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

/// <summary>
/// Сервис для работы с вопросами
/// </summary>
public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuestionService(
        IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<DataResult<QuestionDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var question = await _unitOfWork.Questions.GetAll()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectToType<QuestionDto>()
            .FirstOrDefaultAsync(ct);

        if (question == null)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        return DataResult<QuestionDto>.Success(question);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<QuestionDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var lessonExists = await _unitOfWork.Lessons.ExistsAsync(x => x.Id == lessonId, ct);
        if (!lessonExists)
        {
            return CollectionResult<QuestionDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var questions = await _unitOfWork.Questions.GetAll()
            .AsNoTracking()
            .Where(x => x.LessonId == lessonId)
            .ProjectToType<QuestionDto>()
            .ToListAsync(ct);

        if (!questions.Any())
        {
            return CollectionResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        return CollectionResult<QuestionDto>.Success(questions);
    }

    /// <inheritdoc/>
    public async Task<DataResult<QuestionDto>> CreateAsync(CreateQuestionDto dto, CancellationToken ct = default)
    {
        var lessonExists = await _unitOfWork.Lessons.ExistsAsync(x => x.Id == dto.LessonId, ct);
        if (!lessonExists)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var newQuestion = new Question
        {
            Description = dto.Description,
            LessonId = dto.LessonId,
            CorrectAnswer = dto.CorrectAnswer
        };

        await _unitOfWork.Questions.CreateAsync(newQuestion);
        await _unitOfWork.Questions.SaveChangesAsync();

        return DataResult<QuestionDto>.Success(_mapper.Map<QuestionDto>(newQuestion));
    }

    /// <inheritdoc/>
    public async Task<DataResult<QuestionDto>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var question = await _unitOfWork.Questions.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (question == null)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        _unitOfWork.Questions.Remove(question);
        await _unitOfWork.Questions.SaveChangesAsync();

        return DataResult<QuestionDto>.Success(_mapper.Map<QuestionDto>(question));
    }
}

