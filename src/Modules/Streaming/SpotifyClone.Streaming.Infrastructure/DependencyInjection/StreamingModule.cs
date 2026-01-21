using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Streaming.Application;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Infrastructure.Media;
using SpotifyClone.Streaming.Infrastructure.Storage;

namespace SpotifyClone.Streaming.Infrastructure.DependencyInjection;

public static class StreamingModule
{
    public static IServiceCollection AddStreamingModule(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            StreamingApplicationAssemblyReference.Assembly));

        services.AddValidatorsFromAssembly(StreamingApplicationAssemblyReference.Assembly);

        services.AddScoped<IMediaService, FfmpegMediaService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();

        return services;
    }
}
