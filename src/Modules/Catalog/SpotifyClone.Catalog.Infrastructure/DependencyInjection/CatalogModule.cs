using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Catalog.Application;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

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

        return services;
    }
}
