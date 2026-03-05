using System.Linq;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.Lesson;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

/// <summary>
/// Сервис для работы с уроками
/// </summary>
public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LessonService(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<DataResult<LessonDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var lessonDto = await _unitOfWork.Lessons.GetAll()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectToType<LessonDto>() 
            .FirstOrDefaultAsync(ct); 

        if (lessonDto == null)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        return DataResult<LessonDto>.Success(lessonDto);
    }

    /// <inheritdoc/>
    public async Task<DataResult<float>> PassLessonAsync(PassLessonDto dto, CancellationToken ct = default)
    {
        var (validationResult, courseId) = await ValidateUserAndLesson(dto.UserId, dto.LessonId, ct);
        if (!validationResult.IsSuccess)
        {
            return DataResult<float>.Failure(validationResult.Error);
        }

        var (answersToSave, lessonGrade) = await ProcessAnswers(dto, ct);

        await UpdateCourseGrade(dto.UserId, courseId, lessonGrade, ct);

        await _unitOfWork.StudentAnswers.CreateRangeAsync(answersToSave, ct);

        await _unitOfWork.SaveChangesAsync();

        return DataResult<float>.Success(lessonGrade);
    }

    /// <inheritdoc/>
    public async Task<CollectionResult<LessonDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var courseExists = await _unitOfWork.Courses.ExistsAsync(x => x.Id == courseId, ct);
        if (!courseExists)
        {
            return CollectionResult<LessonDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        var lessons = await _unitOfWork.Lessons.GetAll()
            .AsNoTracking()
            .Where(x => x.CourseId == courseId)
            .ProjectToType<LessonDto>() 
            .ToListAsync(ct);

        if (!lessons.Any())
        {
            return CollectionResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        return CollectionResult<LessonDto>.Success(lessons);
    }

    /// <inheritdoc/>
    public async Task<DataResult<LessonDto>> CreateAsync(CreateLessonDto dto, CancellationToken ct = default)
    {
        var courseExists = await _unitOfWork.Courses.ExistsAsync(x => x.Id == dto.CourseId, ct);
        if (!courseExists)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        var newLesson = new Lesson
        {
            Name = dto.Name,
            Content = dto.Content,
            Number = dto.Number,
            CourseId = dto.CourseId
        };

        await _unitOfWork.Lessons.CreateAsync(newLesson);
        await _unitOfWork.Lessons.SaveChangesAsync();

        return DataResult<LessonDto>.Success(_mapper.Map<LessonDto>(newLesson));
    }

    /// <inheritdoc/>
    public async Task<DataResult<LessonDto>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var lesson = await _unitOfWork.Lessons.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (lesson == null)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        _unitOfWork.Lessons.Remove(lesson);
        await _unitOfWork.Lessons.SaveChangesAsync();

        return DataResult<LessonDto>.Success(_mapper.Map<LessonDto>(lesson));
    }

    /// <inheritdoc/>
    public async Task<DataResult<LessonDto>> UpdateAsync(UpdateLessonDto dto, CancellationToken ct = default)
    {
        var lesson = await _unitOfWork.Lessons.GetAll()
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

        if (lesson == null)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        lesson.Name = dto.Name;
        lesson.Content = dto.Content;
        lesson.Number = dto.Number;

        await _unitOfWork.Lessons.SaveChangesAsync();

        return DataResult<LessonDto>.Success(_mapper.Map<LessonDto>(lesson));
    }

    /// <inheritdoc/>
    private async Task<(BaseResult Result, int CourseId)> ValidateUserAndLesson(long userId, int lessonId, CancellationToken ct)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == userId, ct);
        if (!userExists)
        {
            return (BaseResult.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound), 0);
        }

        var lessonData = await _unitOfWork.Lessons.GetAll()
            .Where(l => l.Id == lessonId)
            .Select(l => new { l.CourseId })
            .FirstOrDefaultAsync(ct);

        if (lessonData == null)
        {
            return (BaseResult.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound), 0);
        }

        return (BaseResult.Success(), lessonData.CourseId);
    }

    /// <inheritdoc/>
    private async Task<(List<StudentAnswer> Answers, float Grade)> ProcessAnswers(PassLessonDto dto, CancellationToken ct)
    {
        var questionIds = dto.UserAnswers.Select(x => x.QuestionId).ToList();

        var questionsData = await _unitOfWork.Questions.GetAll()
            .Where(q => q.LessonId == dto.LessonId && questionIds.Contains(q.Id))
            .ToDictionaryAsync(k => k.Id, v => new { v.CorrectAnswer, v.Score }, ct);

        float totalGrade = 0;
        var answersToSave = new List<StudentAnswer>();

        foreach (var userAnswer in dto.UserAnswers)
        {
            if (!questionsData.TryGetValue(userAnswer.QuestionId, out var questionInfo)) continue;

            bool isCorrect = string.Equals(
                userAnswer.UserAnswer?.Trim(),
                questionInfo.CorrectAnswer?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            float points = isCorrect ? questionInfo.Score : 0;
            totalGrade += points;

            answersToSave.Add(new StudentAnswer
            {
                StudentId = dto.UserId,
                QuestionId = userAnswer.QuestionId,
                Score = points
            });
        }

        return (answersToSave, totalGrade);
    }

    /// <inheritdoc/>
    private async Task UpdateCourseGrade(long userId, int courseId, float lessonGrade, CancellationToken ct)
    {
        var userCourseGrade = await _unitOfWork.UserCourseGrades.GetAll()
            .FirstOrDefaultAsync(x => x.CourseId == courseId && x.UserId == userId, ct);

        if (userCourseGrade == null)
        {
            userCourseGrade = new UserCourseGrade
            {
                UserId = userId,
                CourseId = courseId,
                Grade = lessonGrade
            };
            await _unitOfWork.UserCourseGrades.CreateAsync(userCourseGrade);
        }
        else
        {
            userCourseGrade.Grade += lessonGrade;
            _unitOfWork.UserCourseGrades.Update(userCourseGrade);
        }
    }
}
