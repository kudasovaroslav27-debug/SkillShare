using MockQueryable.Moq;
using Moq;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.Test.Configuration;

public static class MockRepositoriesGetter
{
    public static Mock<IBaseRepository<Course>> GetMockCourseRepository()
    {
        var mock = new Mock<IBaseRepository<Course>>();

        var courses  = GetCourses().BuildMockDbSet();
        mock.Setup (u  => u.GetAll()).Returns(valueFunction: () => courses.Object);
        return mock;
    }

    public static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var mock = new Mock<IBaseRepository<User>>();

        var users  = GetUsers().BuildMockDbSet();
        mock.Setup( u  => u.GetAll()).Returns(valueFunction: () => users.Object);
        return mock;
    }

    public static IQueryable<Course> GetCourses()
    {
        return new List<Course>() {
        new Course() {
            Id = 1,
            Title = "#1",
            Description = "Информация пока нет",
            CreatedAt = DateTime.Now.AddDays(-2),
            UpdateAt = DateTime.Now.AddDays(-2)
        },
        new Course() {
            Id = 2,
            Title = "#2",
            Description = "Информация пока нет",
            CreatedAt = DateTime.Now.AddDays(-2),
            UpdateAt = DateTime.Now.AddDays(-2)
        }
    }.AsQueryable(); 
}

    public static IQueryable<User> GetUsers()
    {
        return new List<User>() {
        new User() {
            Id = 1,
            Login = "ITSkillShare",
            Password = "dasdKAJSkdqwej4ej#",
            CreatedAt = DateTime.Now.AddDays(-2),
            UpdateAt = DateTime.Now.AddDays(-2)
        },
        new User() {
            Id = 2,
            Login = "Basne",
            Password = "daskdak@K#K@",
            CreatedAt = DateTime.Now.AddDays(-2),
            UpdateAt = DateTime.Now.AddDays(-2)
        }
    }.AsQueryable();

    }
}


