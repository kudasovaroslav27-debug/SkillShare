using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Common;
using SkillShare.Domain.Dto.Token;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Result;

namespace SkillShare.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    DataResult<ClaimsPrincipal> GetPrincipalFromExpiredToken(string accessToken);

    Task<DataResult<TokenDto>> RefreshToken(TokenDto Dto, CancellationToken ct = default);

    CollectionResult<Claim> GetClaimsFromUser(User user);
}
