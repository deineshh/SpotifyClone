using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Accounts.Application;
using SpotifyClone.Accounts.Infrastructure.Persistence.Accounts;
using SpotifyClone.Accounts.Infrastructure.Persistence.Identity;

namespace SpotifyClone.Accounts.Infrastructure.DependencyInjection;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(AccountsApplicationAssemblyReference.Assembly);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            AccountsApplicationAssemblyReference.Assembly));

        services.AddDbContext<IdentityAppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("MainDb")));

        services.AddDbContext<AccountsAppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("MainDb")));

        return services;
    }
}
