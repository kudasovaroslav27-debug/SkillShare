using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillShare.Domain.Enum;

public enum ErrorCodes
{
    UserNotFound = 1001,
    UserAlreadyExists = 1002,
    InvalidClientRequest = 1003,
    UserRolesNotFound = 1004,
    UserAlreadyExistsRole = 1005,
    GradeNotFound = 1006,

    CourseNotFound = 2001,
    CourseAlreadyExists = 2002,

    PasswordNotEqualsPasswordConfirm = 3001,
    PasswordIsWrong = 3002,

    InvalidRefreshToken = 4001,
    TokenMismatch = 4002,
    RefreshTokenExpired = 4003,

    RoleAlreadyExists = 5001,
    RoleNotFound = 5002,

    ValidError = 6001,

    QuestionNotFound = 7001,

    LessonNotFound = 8001,

    AnswerNotFound = 9001

}
