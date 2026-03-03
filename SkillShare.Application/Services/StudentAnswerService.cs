using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.StudentAnswer;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class StudentAnswerService : IStudentAnswerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentAnswerService(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(x => x.Id == userId, ct);
        if (!userExists)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var answers = await _unitOfWork.StudentAnswers.GetAll()
            .AsNoTracking()
            .Where(x => x.StudentId == userId)
            .ProjectToType<StudentAnswerDto>()
            .ToListAsync(ct);

        if (!answers.Any())
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        return CollectionResult<StudentAnswerDto>.Success(answers);
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByLessonIdAsync(int lessonId, CancellationToken ct = default)
    {
        var lessonExists = await _unitOfWork.Lessons.ExistsAsync(x => x.Id == lessonId, ct);
        if (!lessonExists)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var answers = await _unitOfWork.StudentAnswers.GetAll()
            .AsNoTracking()
            .Where(x => x.Question.LessonId == lessonId)
            .ProjectToType<StudentAnswerDto>()
            .ToListAsync(ct);
        if (!answers.Any())
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        return CollectionResult<StudentAnswerDto>.Success(answers);
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var courseExists = await _unitOfWork.Courses.ExistsAsync(x => x.Id == courseId, ct);
        if (!courseExists)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        var answers = await _unitOfWork.StudentAnswers.GetAll()
            .AsNoTracking()
            .Where(x => x.Question.Lesson.CourseId == courseId)
            .ProjectToType<StudentAnswerDto>()
            .ToListAsync(ct);

        if (!answers.Any())
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        return CollectionResult<StudentAnswerDto>.Success(answers);
    }

    public async Task<DataResult<StudentAnswerDto>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var answer = await _unitOfWork.StudentAnswers.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (answer == null)
        {
            return DataResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        _unitOfWork.StudentAnswers.Remove(answer);
        await _unitOfWork.StudentAnswers.SaveChangesAsync();

        return DataResult<StudentAnswerDto>.Success(_mapper.Map<StudentAnswerDto>(answer));
    }
}

