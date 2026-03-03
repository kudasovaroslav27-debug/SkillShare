using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillShare.Application.Services;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Dto.Question;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.UserRole;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/questions")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    /// <summary>
    /// Создание вопроса
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DataResult<QuestionDto>>> Create(CreateQuestionDto dto)
    {
        var response = await _questionService.CreateAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Удаление вопроса
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<DataResult<QuestionDto>>> Delete(int id)
    {
        var response = await _questionService.DeleteAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получение вопроса по Id
    /// </summary>
    [HttpGet("api/v1/Questions")]
    public async Task<ActionResult<QuestionDto>> GetQuestionById(int id)
    {
        var response = await _questionService.GetByIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получение вопроса по LessonId
    /// </summary>
    [HttpGet("by-lesson/{lessonId}")]
    public async Task<ActionResult<QuestionDto>> GetQuestionByLesson(int lessonId)
    {
        var response = await _questionService.GetByLessonIdAsync(lessonId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}