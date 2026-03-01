using System.Configuration;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillShare.Application.Services;
using SkillShare.Application.Validations;
using SkillShare.Application.Validations.FluentValidations.Course;
using SkillShare.Application.Validations.FluentValidations.Lesson;
using SkillShare.Application.Validations.FluentValidations.Question;
using SkillShare.Application.Validations.FluentValidations.Role;
using SkillShare.Application.Validations.FluentValidations.StudentAnswer;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Dto.Lesson;
using SkillShare.Domain.Dto.Question;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.StudentAnswer;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Interfaces.Validations;
using SkillShare.Domain.Settings;

namespace SkillShare.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        InitMapsterMapping(services);

        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        var options = configuration.GetSection(nameof(RedisSettings));
        var redisUrl = options["Url"];
        var instanceName = options["InstanceName"];

        services.AddStackExchangeRedisCache(redisCacheOptions => {
            redisCacheOptions.Configuration = redisUrl;
            redisCacheOptions.InstanceName = instanceName;
        });


        initServices(services);
    }

    public static void initServices(this IServiceCollection services)
    {
        services.AddScoped<ICourseValidator, CourseValidator>();
        services.AddScoped<IValidator<CreateCourseDto>, CreateCourseValidator>();
        services.AddScoped<IValidator<UpdateCourseDto>, UpdateCourseValidator>();

        services.AddScoped<IValidator, RoleValidator>();
        services.AddScoped<IValidator<UpdateRoleDto>, UpdateRoleValidator>();
        services.AddScoped<IValidator<CreateRoleDto>, CreateRoleValidator>();

        services.AddScoped<IValidator<CreateQuestionDto>, CreateQuestionDtoValidator>();
        services.AddScoped<IValidator<QuestionDto>, QuestionDtoValidator>();

        services.AddScoped<IValidator<LessonDto>, LessonDtoValidator>();
        services.AddScoped<IValidator<CreateLessonDto>, CreateLessonDtoValidator>();
        services.AddScoped<IValidator<UpdateLessonDto>, UpdateLessonDtoValidator>();

        services.AddScoped<IValidator<StudentAnswerDto>, StudentAnswerDtoValidator>();
        services.AddScoped<IValidator<CreateStudentAnswerDto>, CreateStudentAnswerDtoValidator>();
        services.AddScoped<IValidator<UpdateStudentAnswerDto>, UpdateStudentAnswerDtoValidator>();

        services.AddScoped<IUserCourseGradeService, UserCourseGradeService>();
        services.AddScoped<IStudentAnswerService, StudentAnswerService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
    }

    private static void InitMapsterMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
