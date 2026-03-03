using Microsoft.AspNetCore.Mvc;
using SkillShare.Domain.Dto.Lesson;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/lessons")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Получить урок по Id
    /// </summary>

    [HttpGet("{id}")]
    public async Task<ActionResult<DataResult<LessonDto>>> GetById(int id)
    {
        var response = await _lessonService.GetByIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получить все уроки конкретного курса
    /// </summary>
    [HttpGet("courses/{courseId}")]
    public async Task<ActionResult<CollectionResult<LessonDto>>> GetByCourseId(int courseId)
    {
        var response = await _lessonService.GetByCourseIdAsync(courseId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Создать новый урок
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DataResult<LessonDto>>> Create(CreateLessonDto dto)
    {
        var response = await _lessonService.CreateAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("pass")]
    public async Task<ActionResult<DataResult<float>>> PassLesson(PassLessonDto dto)
    {
        var response = await _lessonService.PassLessonAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Обновить данные урока
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<DataResult<LessonDto>>> Update(UpdateLessonDto dto)
    {
        var response = await _lessonService.UpdateAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Удалить урок
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<DataResult<LessonDto>>> Delete(int id)
    {
        var response = await _lessonService.DeleteAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
