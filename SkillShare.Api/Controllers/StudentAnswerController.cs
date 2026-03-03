using Microsoft.AspNetCore.Mvc;
using SkillShare.Domain.Dto.StudentAnswer;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/StudentAnswers")]
public class StudentAnswerController : ControllerBase
{
    private readonly IStudentAnswerService _studentAnswerService;

    public StudentAnswerController(IStudentAnswerService studentAnswerService)
    {
        _studentAnswerService = studentAnswerService;
    }

    /// <summary>
    /// Удаляет ответ студента
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<DataResult<StudentAnswerDto>>> Delete(long id)
    {
        var response = await _studentAnswerService.DeleteAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получает ответы конкретного студента
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<CollectionResult<StudentAnswerDto>>> GetByUserId(long userId)
    {
        var response = await _studentAnswerService.GetByUserIdAsync(userId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получает ответы по идентификатору урока
    /// </summary>
    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<CollectionResult<StudentAnswerDto>>> GetByLessonId(int lessonId)
    {
        var response = await _studentAnswerService.GetByLessonIdAsync(lessonId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получает ответы по идентификатору курса
    /// </summary>
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<CollectionResult<StudentAnswerDto>>> GetByCourseId(int courseId)
    {
        var response = await _studentAnswerService.GetByCourseIdAsync(courseId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}

