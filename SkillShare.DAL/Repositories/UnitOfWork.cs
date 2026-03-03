using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;


    public UnitOfWork(
        ApplicationDbContext context,
        IBaseRepository<User> users,
        IBaseRepository<Role> roles,
        IBaseRepository<UserRole> userRoles,
        IBaseRepository<Course> courses,
        IBaseRepository<Lesson> lessons,
        IBaseRepository<Question> questions,
        IBaseRepository<StudentAnswer> studentAnswers,
        IBaseRepository<UserCourseGrade> userCourseGrades,
        IBaseRepository<UserToken> userTokens)
    {
        _context = context;
        Users = users;
        Roles = roles;
        UserRoles = userRoles;
        Courses = courses;
        Lessons = lessons;
        Questions = questions;
        StudentAnswers = studentAnswers;
        UserCourseGrades = userCourseGrades;
        UserTokens = userTokens;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
       return await _context.SaveChangesAsync();
    }

    public IBaseRepository<User> Users { get; set; }
    public IBaseRepository<Role> Roles { get; set; }
    public IBaseRepository<UserRole> UserRoles { get; set; }
    public IBaseRepository<Course> Courses { get; set; }
    public IBaseRepository<Lesson> Lessons { get; set; }
    public IBaseRepository<Question> Questions { get; set; }
    public IBaseRepository<StudentAnswer> StudentAnswers { get; set; }
    public IBaseRepository<UserCourseGrade> UserCourseGrades { get; set; }
    public IBaseRepository<UserToken> UserTokens { get; set; }
}
