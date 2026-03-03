using Microsoft.AspNetCore.Mvc;
using SkillShare.Application.Services;
using SkillShare.Domain.Dto.Token;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/Tokens")]
public class TokenController : Controller
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<ActionResult<DataResult<TokenDto>>> RefreshToken(TokenDto dto)
    {
        var response = await _tokenService.RefreshToken(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}
