using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.UserRole;

public record RemoveUserRoleDto(string Login, int RoleId);

