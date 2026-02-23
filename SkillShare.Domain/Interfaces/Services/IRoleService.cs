using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.UserRole;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

/// <summary>
/// Сервис предназначенный для управлением ролями пользователей
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление роли
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> DeleteRoleAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Обновление роли
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Добавление роли пользователю
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление роли пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> DeleteRoleForUserAsync(RemoveUserRoleDto dto, CancellationToken ct = default);

    /// <summary>
    /// Обновление роли у пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<DataResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto, CancellationToken ct = default);
}
