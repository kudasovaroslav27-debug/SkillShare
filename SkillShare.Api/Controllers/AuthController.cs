using Microsoft.AspNetCore.Mvc;
using SkillShare.Application.Services;
using SkillShare.Domain.Dto.Token;
using SkillShare.Domain.Dto.User;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;


namespace SkillShare.Api.Controllers;

[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрацияя пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("register")]   
    public async Task<ActionResult<DataResult<UserDto>>> Register(RegisterUserDto dto)
    {
        var response = await _authService.Register(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<DataResult<TokenDto>>> Login(LoginUserDto dto)
    {
        var response = await _authService.Login(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
