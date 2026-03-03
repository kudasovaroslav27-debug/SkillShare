using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.Domain.Interfaces.Databases;

public interface IUnitOfWork : IStateSaveChanges
{
    Task<IDbContextTransaction> BeginTransactionAsync();

    IBaseRepository<User> Users { get; set; }

    IBaseRepository<Role> Roles { get; set; }

    IBaseRepository<UserRole> UserRoles { get; set; }

    IBaseRepository<Course> Courses { get; set; }

    IBaseRepository<Lesson> Lessons { get; set; }

    IBaseRepository<Question> Questions { get; set; }

    IBaseRepository<StudentAnswer> StudentAnswers { get; set; }

    IBaseRepository<UserCourseGrade> UserCourseGrades { get; set; }

    IBaseRepository<UserToken> UserTokens { get; set; }
}
