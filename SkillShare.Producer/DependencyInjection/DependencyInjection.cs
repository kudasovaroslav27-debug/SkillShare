using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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