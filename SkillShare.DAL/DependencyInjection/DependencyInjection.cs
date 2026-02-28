using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Crypto;
using SkillShare.DAL.Interceptors;
using SkillShare.DAL.Repositories;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Repositories;

namespace SkillShare.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("PostgresSQL");

        services.AddSingleton<DateInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options => 
        { 
            options.UseNpgsql(connectionString);
        });  
        services.InitRepositories();

    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
        services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
        services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();
        services.AddScoped<IBaseRepository<StudentAnswer>, BaseRepository<StudentAnswer>>();
        services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
        services.AddScoped<IBaseRepository<Question>, BaseRepository<Question>>();
        services.AddScoped<IBaseRepository<Lesson>, BaseRepository<Lesson>>();
        services.AddScoped<IBaseRepository<Course>, BaseRepository<Course>>();
        services.AddScoped<IBaseRepository<Answer>, BaseRepository<Answer>>();
        services.AddScoped<IBaseRepository<UserCourseGrade>, BaseRepository<UserCourseGrade>>();
    }
}
