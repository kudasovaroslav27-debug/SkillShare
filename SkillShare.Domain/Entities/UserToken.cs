using SkillShare.Domain.Interfaces;

namespace SkillShare.Domain.Entities;

public class UserToken : IEntityId<long>
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpireTime { get; set; }

    public User User { get; set; }
}
