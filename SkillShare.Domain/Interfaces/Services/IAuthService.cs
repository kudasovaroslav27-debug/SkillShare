using SkillShare.Domain.Dto.Token;
using SkillShare.Domain.Dto.User;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

/// <summary>
/// Сервис предназначенный для регистрации/авторизации 
/// </summary>
/// <param name="dto"></param>
/// <returns></returns>
public interface IAuthService
{
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<UserDto>> Register(RegisterUserDto dto, CancellationToken ct = default);

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<DataResult<TokenDto>> Login(LoginUserDto dto, CancellationToken ct = default);
}
