using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Catalog.Application;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Genres;
using SpotifyClone.Catalog.Domain.Aggregates.Moods;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Catalog.Infrastructure.Persistence;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;
using SpotifyClone.Catalog.Infrastructure.Persistence.Repositories;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;

namespace SpotifyClone.Catalog.Infrastructure.DependencyInjection;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            CatalogApplicationAssemblyReference.Assembly));

        services.AddValidatorsFromAssembly(CatalogApplicationAssemblyReference.Assembly);

        services.AddDbContext<CatalogAppDbContext>(options => options.UseNpgsql(
            configuration.GetConnectionString("MainDb"),
            b => b.MigrationsAssembly(typeof(CatalogAppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ICatalogUnitOfWork>());
        services.AddScoped<ICatalogUnitOfWork, CatalogEfCoreUnitOfWork>();
        services.AddScoped<ITrackRepository, TrackEfCoreRepository>();
        services.AddScoped<IAlbumRepository, AlbumEfCoreRepository>();
        services.AddScoped<IArtistRepository, ArtistEfCoreRepository>();
        services.AddScoped<IGenreRepository, GenreEfCoreRepository>();
        services.AddScoped<IMoodRepository, MoodEfCoreRepository>();

        return services;
    }
}
