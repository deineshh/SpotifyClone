using FluentValidation;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;
using SpotifyClone.Streaming.Application;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Jobs;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Infrastructure.Media;
using SpotifyClone.Streaming.Infrastructure.Persistence;
using SpotifyClone.Streaming.Infrastructure.Persistence.Database;
using SpotifyClone.Streaming.Infrastructure.Persistence.Repositories;
using SpotifyClone.Streaming.Infrastructure.Storage;

namespace SpotifyClone.Streaming.Infrastructure.DependencyInjection;

public static class StreamingModule
{
    public static IServiceCollection AddStreamingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            StreamingApplicationAssemblyReference.Assembly));

        services.AddValidatorsFromAssembly(StreamingApplicationAssemblyReference.Assembly);

        services.AddDbContext<StreamingAppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("MainDb"),
                b => b.MigrationsAssembly(typeof(StreamingAppDbContext).Assembly.FullName)));

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());
        services.AddHangfireServer();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IStreamingUnitOfWork>());
        services.AddScoped<IStreamingUnitOfWork, StreamingEfCoreUnitOfWork>();
        services.AddScoped<IAudioAssetRepository, AudioAssetEfCoreRepository>();
        services.AddScoped<IMediaService, FfmpegMediaService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();

        services.AddTransient<AudioConversionJob>();

        return services;
    }
}
