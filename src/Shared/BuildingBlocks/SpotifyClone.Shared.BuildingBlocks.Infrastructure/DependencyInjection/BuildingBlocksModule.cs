using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;

namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.DependencyInjection;

public static class BuildingBlocksModule
{
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(cfg =>
        {
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingPipelineBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionalPipelineBehavior<,>));
        });

        return services;
    }
}
