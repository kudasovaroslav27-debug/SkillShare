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

    public async Task<DataResult<float>> PassLessonAsync(PassLessonDto dto, CancellationToken ct = default)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId, ct);
        if (!userExists)
        {
            return DataResult<float>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var lesson = await _unitOfWork.Lessons.GetAll()
            .Where(l => l.Id == dto.LessonId)
            .Select(l => new { l.Id, l.CourseId })
            .FirstOrDefaultAsync(ct);

        if (lesson == null)
        {
            return DataResult<float>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var questionIds = dto.UserAnswers.Select(x => (int)x.QuestionId).ToList();
        var questionsData = await _unitOfWork.Questions.GetAll()
            .Where(q => q.LessonId == dto.LessonId && questionIds.Contains((int)q.Id))
            .ToDictionaryAsync(
                k => (long)k.Id,
                v => new { v.CorrectAnswer, v.Score }, 
                ct);

        float lessonGrade = 0;
        var answersToSave = new List<StudentAnswer>();

        foreach (var userAnswer in dto.UserAnswers)
        {
            if (!questionsData.TryGetValue(userAnswer.QuestionId, out var questionInfo)) continue;

            bool isCorrect = string.Equals(
                userAnswer.UserAnswer?.Trim(),
                questionInfo.CorrectAnswer?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            float pointsEarned = isCorrect ? questionInfo.Score : 0;
            lessonGrade += pointsEarned;

            answersToSave.Add(new StudentAnswer
            {
                StudentId = dto.UserId,
                QuestionId = userAnswer.QuestionId,
                Score = pointsEarned
            });
        }

        var userCourseGrade = await _unitOfWork.UserCourseGrades.GetAll()
            .FirstOrDefaultAsync(x => x.CourseId == lesson.CourseId && x.UserId == dto.UserId, ct);

        if (userCourseGrade == null)
        {
            userCourseGrade = new UserCourseGrade
            {
                UserId = dto.UserId,
                CourseId = lesson.CourseId,
                Grade = lessonGrade
            };
            await _unitOfWork.UserCourseGrades.CreateAsync(userCourseGrade);
        }
        else
        {
            userCourseGrade.Grade += lessonGrade;
            _unitOfWork.UserCourseGrades.Update(userCourseGrade);
        }

        foreach (var answer in answersToSave)
        {
            await _unitOfWork.StudentAnswers.CreateAsync(answer);
        }

        await _unitOfWork.StudentAnswers.SaveChangesAsync();

        return DataResult<float>.Success(lessonGrade);
    }


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
}
