using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;
using SkillShare.Domain.Settings;

namespace SkillShare.Application.Services;

public class TokenService : ITokenService
{
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IBaseRepository<User> _userRepository;
    private readonly string _jwtKey;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository, TimeProvider timeProvider, IBaseRepository<UserToken> userTokenRepository)
    {
        _timeProvider = timeProvider;
        _userRepository = userRepository;
        _jwtKey = options.Value.JwtKey;
        _issuer = options.Value.Issuer;
        _audience = options.Value.Audience;
        _userTokenRepository = userTokenRepository;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityToken =
            new JwtSecurityToken(_issuer, _issuer, claims, null, DateTime.UtcNow.AddMinutes(10), credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }

    public string GenerateRefreshToken()
    {
        var randomNumbers = new byte[32];

        using var randomNumbersGenerate = RandomNumberGenerator.Create();
        randomNumbersGenerate.GetBytes(randomNumbers);

        return Convert.ToBase64String(randomNumbers);
    }

    public CollectionResult<Claim> GetClaimsFromUser(User user)
    {
        if (user == null)
        {
            return CollectionResult<Claim>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }
        if (user.Roles == null || user.Roles.Count == 0)
        {
            return CollectionResult<Claim>.Failure((int)ErrorCodes.UserRolesNotFound, ErrorMessage.UserRolesNotFound);
        }
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x.Name)));

        return CollectionResult<Claim>.Success(claims);
    }


    public async Task<DataResult<TokenDto>> RefreshToken(TokenDto dto, CancellationToken ct)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.UserToken)
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.UserToken.RefreshToken == dto.RefreshToken, ct);

        if (user == null)
        {
            return DataResult<TokenDto>.Failure((int)ErrorCodes.InvalidRefreshToken, ErrorMessage.InvalidRefreshToken);
        }

        var claimsPrincipalResult = GetPrincipalFromExpiredToken(dto.AccessToken);
        if (!claimsPrincipalResult.IsSuccess)
        {
            return DataResult<TokenDto>.Failure(claimsPrincipalResult.Error);
        }

        var loginFromToken = claimsPrincipalResult.Data.Identity?.Name;
        if (loginFromToken != user.Login)
        {
            return DataResult<TokenDto>.Failure((int)ErrorCodes.TokenMismatch, ErrorMessage.TokenMismatch);
        }

        if (user.UserToken.RefreshTokenExpireTime <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            return DataResult<TokenDto>.Failure((int)ErrorCodes.RefreshTokenExpired, ErrorMessage.RefreshTokenExpired);
        }

        var getNewClaimsResult = GetClaimsFromUser(user);
        if (!getNewClaimsResult.IsSuccess)
        {
            return DataResult<TokenDto>.Failure(getNewClaimsResult.Error);
        }

        var newAccessToken = GenerateAccessToken(getNewClaimsResult.Data);
        var newRefreshToken = GenerateRefreshToken();

        user.UserToken.RefreshToken = newRefreshToken;
        user.UserToken.RefreshTokenExpireTime = _timeProvider.GetUtcNow().UtcDateTime.AddDays(7);

        _userTokenRepository.Update(user.UserToken);
        await _userTokenRepository.SaveChangesAsync();

        var tokenDto = new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };

        return DataResult<TokenDto>.Success(tokenDto);
    }

    public async Task<DataResult<User>> GetValidUserForRefreshAsync(
    TokenDto dto,
    CancellationToken ct)
    {
        var claimsPrincipalResult = GetPrincipalFromExpiredToken(dto.AccessToken);
        if (!claimsPrincipalResult.IsSuccess)
        {
            return DataResult<User>.Failure(claimsPrincipalResult.Error);
        }

        var login = claimsPrincipalResult.Data.Identity?.Name;
        if (string.IsNullOrEmpty(login))
        {
            return DataResult<User>.Failure((int)ErrorCodes.TokenMismatch, ErrorMessage.TokenMismatch);
        }

        var user = await _userRepository.GetAll()
            .Include(x => x.UserToken)
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == login, ct);

        if (user?.UserToken == null)
        {
            return DataResult<User>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        if (user.UserToken.RefreshToken != dto.RefreshToken)
        {
            return DataResult<User>.Failure((int)ErrorCodes.InvalidRefreshToken, ErrorMessage.InvalidRefreshToken);
        }

        if (user.UserToken.RefreshTokenExpireTime <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            return DataResult<User>.Failure((int)ErrorCodes.RefreshTokenExpired, ErrorMessage.RefreshTokenExpired);
        }

        return DataResult<User>.Success(user);
    }

    /// <summary>
    /// Получение ClaimsPrincipal из истекшего токена
    /// </summary>
    /// <param name="accessToken">Access token</param>
    /// <returns>ClaimsPrincipal</returns>
    /// <exception cref="SecurityTokenException">Когда токен невалидный</exception>
    public DataResult<ClaimsPrincipal> GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
            ValidateLifetime = false,
            ValidAudience = _audience,
            ValidIssuer = _issuer
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(
            accessToken,
            tokenValidationParameters,
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            return DataResult<ClaimsPrincipal>.Failure((int)ErrorCodes.InvalidClientRequest, ErrorMessage.InvalidClientRequest);
        }

        return DataResult<ClaimsPrincipal>.Success(claimsPrincipal);
    }
}
