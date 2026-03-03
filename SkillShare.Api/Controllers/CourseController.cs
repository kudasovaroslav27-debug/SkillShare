using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;


/// <summary>
/// [Authorize]
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("User/{AuthorId}")]
    public async Task<ActionResult<CollectionResult<CourseDto>>> GetByIdAuthorCourse(long AuthorId)
    {
        var response = await _courseService.GetByAuthorIdAsync(AuthorId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        var user = User;
        var response = await _courseService.GetByIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpDelete]
    public async Task<ActionResult<DataResult<CourseDto>>> DeleteCourseById(int id)
    {
        var response = await _courseService.DeleteAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Создание курса
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <remarks>
    /// Some info
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<DataResult<CourseDto>>> CreateCourse(long userId, CreateCourseDto dto)
    {
        var response = await _courseService.CreateAsync(userId, dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut]
    public async Task<ActionResult<DataResult<CourseDto>>> UpdateCourse(UpdateCourseDto dto)
    {
        var response = await _courseService.UpdateAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
