using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.StudentAnswer;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class StudentAnswerService : IStudentAnswerService
{
    private readonly IBaseRepository<StudentAnswer> _answerRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Question> _questionRepository;
    private readonly IValidator<CreateStudentAnswerDto> _createValidator;
    private readonly IValidator<UpdateStudentAnswerDto> _updateValidator;
    private readonly IMapper _mapper;

    public StudentAnswerService(
        IBaseRepository<StudentAnswer> answerRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<Question> questionRepository,
        IMapper mapper,
        IValidator<CreateStudentAnswerDto> createValidator,
        IValidator<UpdateStudentAnswerDto> updateValidator)
    {
        _answerRepository = answerRepository;
        _userRepository = userRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<DataResult<StudentAnswerDto>> CreateAsync(CreateStudentAnswerDto dto, CancellationToken ct = default)
    {
        var student = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.StudentId, ct);
        var question = await _questionRepository.GetAll().FirstOrDefaultAsync(x => x.Id == (int)dto.QuestionId, ct);

        if (student == null)
        {
            return DataResult<StudentAnswerDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        if (question == null)
        {
            return DataResult<StudentAnswerDto>.Failure((int)ErrorCodes.QuestionNotFound, ErrorMessage.QuestionNotFound);
        }

        var answer = new StudentAnswer
        {
            StudentId = dto.StudentId,
            QuestionId = dto.QuestionId,
            Score = 0, 
        };

        await _answerRepository.CreateAsync(answer);
        await _answerRepository.SaveChangesAsync();

        return DataResult<StudentAnswerDto>.Success(_mapper.Map<StudentAnswerDto>(answer));
    }

    public async Task<DataResult<StudentAnswerDto>> UpdateAsync(UpdateStudentAnswerDto dto, CancellationToken ct = default)
    {
        var answer = await _answerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

        if (answer == null)
        {
            return DataResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        answer.TeacherId = dto.TeacherId;
        answer.Score = dto.Score;

        _answerRepository.Update(answer);
        await _answerRepository.SaveChangesAsync();

        return DataResult<StudentAnswerDto>.Success(_mapper.Map<StudentAnswerDto>(answer));
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var answers = await _answerRepository.GetAll()
            .Where(x => x.StudentId == userId)
            .ToListAsync(ct);

        if (answers == null)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        return CollectionResult<StudentAnswerDto>.Success(_mapper.Map<IEnumerable<StudentAnswerDto>>(answers).ToList());
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var answers = await _answerRepository.GetAll()
            .Where(x => x.Question.LessonId == lessonId)
            .ToListAsync(ct);

        if (answers == null)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        return CollectionResult<StudentAnswerDto>.Success(_mapper.Map<IEnumerable<StudentAnswerDto>>(answers).ToList());
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var answers = await _answerRepository.GetAll()
            .Where(x => x.Question.Lesson.CourseId == courseId)
            .ToListAsync(ct);

        if (answers == null)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        return CollectionResult<StudentAnswerDto>.Success(_mapper.Map<IEnumerable<StudentAnswerDto>>(answers).ToList());
    }

    public async Task<DataResult<StudentAnswerDto>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var answer = await _answerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id, ct);

        if (answer == null)
        {
            return DataResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        _answerRepository.Remove(answer);
        await _answerRepository.SaveChangesAsync();

        return DataResult<StudentAnswerDto>.Success(_mapper.Map<StudentAnswerDto>(answer));
    }
}
