using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.Lesson;
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
    private readonly IBaseRepository<Lesson> _lessonRepository; 
    private readonly IBaseRepository<Course> _courseRepository; 
    private readonly IMapper _mapper;

    public StudentAnswerService(
        IBaseRepository<StudentAnswer> answerRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<Lesson> lessonRepository,
        IBaseRepository<Course> courseRepository,
        IMapper mapper)
    {
        _answerRepository = answerRepository;
        _userRepository = userRepository;
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<CollectionResult<StudentAnswerDto>> GetByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsAsync(x => x.Id == userId, ct);
        if (!userExists)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var answers = await _answerRepository.GetAll()
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
        var lessonExists = await _lessonRepository.ExistsAsync(x => x.Id == lessonId, ct);
        if (!lessonExists)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        var answers = await _answerRepository.GetAll()
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
        var courseExists = await _courseRepository.ExistsAsync(x => x.Id == courseId, ct);
        if (!courseExists)
        {
            return CollectionResult<StudentAnswerDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        var answers = await _answerRepository.GetAll()
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
        var answer = await _answerRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (answer == null)
        {
            return DataResult<StudentAnswerDto>.Failure((int)ErrorCodes.AnswerNotFound, ErrorMessage.AnswerNotFound);
        }

        _answerRepository.Remove(answer);
        await _answerRepository.SaveChangesAsync();

        return DataResult<StudentAnswerDto>.Success(_mapper.Map<StudentAnswerDto>(answer));
    }
}

