using Microsoft.AspNetCore.Mvc;
using SkillShare.Domain.Dto.UserCourseGrade;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/UserCourseGrades")]
public class UserCourseGradeController : ControllerBase
{
    private readonly IUserCourseGradeService _gradeService;
    public UserCourseGradeController(IUserCourseGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    /// <summary>
    /// Получить оценку по Id
    /// </summary>
    [HttpGet("grade/{id}")]
    public async Task<ActionResult<DataResult<UserCourseGradeDto>>> GetById(long id)
    {
        var response = await _gradeService.GetByIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Получить оценку по Id
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<DataResult<UserCourseGradeDto>>> GetByUserId(long userId)
    {
        var response = await _gradeService.GetByUserIdAsync(userId);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Удалить оценку пользователя
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<DataResult<UserCourseGradeDto>>> Delete(long id)
    {
        var response = await _gradeService.DeleteAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}


