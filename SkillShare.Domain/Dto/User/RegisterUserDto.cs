namespace SkillShare.Domain.Dto.User;

public record RegisterUserDto(string Login, string Password, string PasswordConfirm,string Name, string LastName, string Email, int Age);

