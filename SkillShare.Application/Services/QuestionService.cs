using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Dto.Lesson;
using SkillShare.Domain.Dto.Question;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IBaseRepository<Question> _questionRepository;
    private readonly IBaseRepository<Lesson> _lessonRepository;
    private readonly IMapper _mapper;

    public QuestionService(
        IBaseRepository<Question> questionRepository,
        IBaseRepository<Lesson> lessonRepository,
        IMapper mapper)
    {
        _questionRepository = questionRepository;
        _lessonRepository = lessonRepository;
        _mapper = mapper;
    }

    public async Task<DataResult<QuestionDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetAll()
            .Where(x => x.Id == id)
            .ProjectToType<QuestionDto>()
            .FirstOrDefaultAsync(ct);

        if (question == null)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        return DataResult<QuestionDto>.Success(question);
    }

    public async Task<CollectionResult<QuestionDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var lessonExists = await _lessonRepository.ExistsAsync(x => x.Id == lessonId, ct);
        if (!lessonExists)
        {
            return CollectionResult<QuestionDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var questions = await _questionRepository.GetAll()
            .Where(x => x.LessonId == lessonId)
            .ProjectToType<QuestionDto>()
            .ToListAsync(ct);

        if (!questions.Any())
        {
            return CollectionResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        return CollectionResult<QuestionDto>.Success(questions);
    }

    public async Task<DataResult<QuestionDto>> CreateAsync(CreateQuestionDto dto, CancellationToken ct = default)
    {
        var lessonExists = await _lessonRepository.ExistsAsync(x => x.Id == dto.LessonId, ct);
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

        await _questionRepository.CreateAsync(newQuestion);
        await _questionRepository.SaveChangesAsync();

        return DataResult<QuestionDto>.Success(_mapper.Map<QuestionDto>(newQuestion));
    }

    public async Task<DataResult<QuestionDto>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (question == null)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        _questionRepository.Remove(question);
        await _questionRepository.SaveChangesAsync();

        return DataResult<QuestionDto>.Success(_mapper.Map<QuestionDto>(question));
    }
}

