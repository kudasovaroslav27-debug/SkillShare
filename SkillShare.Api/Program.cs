using Prometheus;
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

        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
        builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.UseHttpClientMetrics();

        builder.Services.AddDataAccessLayer(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddProducer();
        builder.Services.AddConsumer();

        builder.Services.AddAuthenticationAndAuthorization(builder);
        builder.Services.AddSwagger();

        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
        
        builder.Services.AddApplication(builder.Configuration);

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

        app.UseMetricServer();
        app.UseHttpMetrics();

        app.MapGet("/random-number", () =>
        {
            var number = Random.Shared.Next(0, 10);
            return Results.Ok(number);
        });

        app.MapMetrics();
        app.MapControllers();

        app.Run();
    }
}
