using Serilog;
using SkillShare.Api.Middlewares;
using SkillShare.Application.DependencyInjection;
using SkillShare.Consumer.DependencyInjection;
using SkillShare.DAL.DependencyInjection;
using SkillShare.Domain.Settings;
using SkillShare.Producer.DependencyInjection;
namespace SkillShare.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.WriteLine("=== Environment Variables ===");
        Console.WriteLine($"RabbitMQSettings__HostName: {Environment.GetEnvironmentVariable("RabbitMQSettings__HostName")}");
        Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
        var rabbitHost = builder.Configuration["RabbitMQSettings:HostName"];
        Console.WriteLine($"Configured RabbitMQ Host: {rabbitHost}");

        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));

        builder.Services.AddDataAccessLayer(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddProducer();
        builder.Services.AddConsumer();

        builder.Services.AddAuthenticationAndAuthorization(builder);
        builder.Services.AddSwagger();

        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
        
        builder.Services.AddApplication();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillShare Swagger v 1.0");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
