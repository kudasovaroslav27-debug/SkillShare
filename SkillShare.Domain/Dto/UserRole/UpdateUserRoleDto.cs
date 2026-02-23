using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Dto.UserRole;

public record UpdateUserRoleDto(string Login, int FromRoleId, int ToRoleId);

