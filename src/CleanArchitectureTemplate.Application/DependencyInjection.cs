using CleanArchitectureTemplate.Application.Common.Behaviors;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ICacheVersionStore, CacheVersionStore>();
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        return services;
    }
}
