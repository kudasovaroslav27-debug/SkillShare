using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
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
    private readonly IValidator<CreateQuestionDto> _createQuestionValidator;
    private readonly IValidator<QuestionDto> _questionValidator;
    private readonly IMapper _mapper;

    public QuestionService(
        IBaseRepository<Question> questionRepository,
        IBaseRepository<Lesson> lessonRepository,
        IMapper mapper,
        IValidator<CreateQuestionDto> createValidator,
        IValidator<QuestionDto> updateValidator)
    {
        _questionRepository = questionRepository;
        _lessonRepository = lessonRepository;
        _mapper = mapper;
        _createQuestionValidator = createValidator;
        _questionValidator = updateValidator;
    }

    public async Task<DataResult<QuestionDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (question == null)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        return DataResult<QuestionDto>.Success(_mapper.Map<QuestionDto>(question));
    }

    public async Task<CollectionResult<QuestionDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var questions = await _questionRepository.GetAll()
            .Where(x => x.LessonId == lessonId)
            .ToListAsync(ct);

        if (questions == null || questions.Count == 0)
        {
            return CollectionResult<QuestionDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        return CollectionResult<QuestionDto>.Success(_mapper.Map<IEnumerable<QuestionDto>>(questions).ToList());
    }

    public async Task<DataResult<QuestionDto>> CreateAsync(CreateQuestionDto dto, CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == dto.LessonId, ct);

        if (lesson == null)
        {
            return DataResult<QuestionDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var newQuestion = new Question
        {
            Description = dto.Description,
            LessonId = dto.LessonId
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
