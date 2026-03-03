using Microsoft.Extensions.DependencyInjection;
using SkillShare.Producer.Interfaces;

namespace SkillShare.Producer.DependencyInjection;

public static class DependencyInjection
{
    public static void AddProducer(this IServiceCollection services)
    {
        services.AddScoped<IMessageProducer, Producer>();
    }
}