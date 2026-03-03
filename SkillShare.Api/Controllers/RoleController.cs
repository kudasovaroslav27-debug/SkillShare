using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.UserRole;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Api.Controllers;


[ApiController]
[Authorize(Roles ="Admin")]
[Route("api/v{version:apiVersion}/Roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <remarks>
    /// Some info
    /// </remarks>
    [HttpPost("api/v1/Roles")]
    public async Task<ActionResult<DataResult<Role>>> Create(CreateRoleDto dto)
    {
        var response = await _roleService.CreateRoleAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Добавление роли для пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("assign")]
    public async Task<ActionResult<DataResult<Role>>> AddRoleForUser(UserRoleDto dto)
    {
        var response = await _roleService.AddRoleForUserAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <remarks>
    /// Some info
    /// </remarks>
    [HttpPut("UpdateRole")]
    public async Task<ActionResult<DataResult<Role>>> Update(UpdateRoleDto dto)
    {
        var response = await _roleService.UpdateRoleAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Удаление роли
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <remarks>
    /// Some info
    /// </remarks>
    [HttpDelete]
    public async Task<ActionResult<DataResult<Role>>> Delete(int id)
    {
        var response = await _roleService.DeleteRoleAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Удаление роли пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpDelete("remove/{userId}/{roleId}")]
    public async Task<ActionResult<DataResult<Role>>> DeleteRoleForUser(RemoveUserRoleDto dto)
    {
        var response = await _roleService.DeleteRoleForUserAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// Обновление роли пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("UpdateUserRole")]
    public async Task<ActionResult<DataResult<Role>>> UpdateRoleForUser(UpdateUserRoleDto dto)
    {
        var response = await _roleService.UpdateRoleForUserAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}