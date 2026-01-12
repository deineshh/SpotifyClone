using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Shared.BuildingBlocks.Application;
using SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;

namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.DependencyInjection;

public static class BuildingBlocksModule
{
    public static IServiceCollection AddBuildingBlocks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                BuildingBlocksApplicationAssemblyReference.Assembly);

            cfg.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionalPipelineBehavior<,>));
        });

        return services;
    }
}
