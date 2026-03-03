using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.Lesson;
using SkillShare.Domain.Dto.Question;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class LessonService : ILessonService
{
    private readonly IBaseRepository<Lesson> _lessonRepository;
    private readonly IBaseRepository<Course> _courseRepository;
    private readonly IMapper _mapper;

    public LessonService(
        IBaseRepository<Lesson> lessonRepository,
        IBaseRepository<Course> courseRepository,
        IMapper mapper)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<DataResult<LessonDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var lessonDto = await _lessonRepository.GetAll()
            .Where(x => x.Id == id)
            .ProjectToType<LessonDto>() 
            .FirstOrDefaultAsync(ct); 

        if (lessonDto == null)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        return DataResult<LessonDto>.Success(lessonDto);
    }


    public async Task<CollectionResult<LessonDto>> GetByCourseIdAsync(int courseId, CancellationToken ct = default)
    {
        var courseExists = await _courseRepository.ExistsAsync(x => x.Id == courseId, ct);
        if (!courseExists)
        {
            return CollectionResult<LessonDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        var lessons = await _lessonRepository.GetAll()
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
        var courseExists = await _courseRepository.ExistsAsync(x => x.Id == dto.CourseId, ct);
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

        await _lessonRepository.CreateAsync(newLesson);
        await _lessonRepository.SaveChangesAsync();

        return DataResult<LessonDto>.Success(_mapper.Map<LessonDto>(newLesson));
    }



    public async Task<DataResult<LessonDto>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (lesson == null)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        _lessonRepository.Remove(lesson);
        await _lessonRepository.SaveChangesAsync();

        return DataResult<LessonDto>.Success(_mapper.Map<LessonDto>(lesson));
    }

    public async Task<DataResult<LessonDto>> UpdateAsync(UpdateLessonDto dto, CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

        if (lesson == null)
        {
            return DataResult<LessonDto>.Failure((int)ErrorCodes.LessonNotFound, ErrorMessage.LessonNotFound);
        }

        lesson.Name = dto.Name;
        lesson.Content = dto.Content;
        lesson.Number = dto.Number;

        await _lessonRepository.SaveChangesAsync();

        return DataResult<LessonDto>.Success(_mapper.Map<LessonDto>(lesson));
    }
}
