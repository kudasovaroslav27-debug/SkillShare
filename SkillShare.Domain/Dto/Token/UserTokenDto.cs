using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.Token
{
    public record UserTokenDto(long UserId, string RefreshToken);
}
