namespace SkillShare.Domain.Interfaces;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdateAt { get; set; }


}
