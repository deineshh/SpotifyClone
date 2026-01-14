using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClone.Accounts.Application;
using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Repositories;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Accounts.Infrastructure.Auth;
using SpotifyClone.Accounts.Infrastructure.Persistence;
using SpotifyClone.Accounts.Infrastructure.Persistence.Accounts.Database;
using SpotifyClone.Accounts.Infrastructure.Persistence.Accounts.Repositories;
using SpotifyClone.Accounts.Infrastructure.Persistence.Auth;
using SpotifyClone.Accounts.Infrastructure.Persistence.Auth.Repositories;
using SpotifyClone.Accounts.Infrastructure.Persistence.Identity;
using SpotifyClone.Accounts.Infrastructure.Persistence.Identity.Database;
using SpotifyClone.Accounts.Infrastructure.Services;
using SpotifyClone.Shared.BuildingBlocks.Application;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Mappers;

namespace SpotifyClone.Accounts.Infrastructure.DependencyInjection;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(AccountsApplicationAssemblyReference.Assembly);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            BuildingBlocksApplicationAssemblyReference.Assembly,
            AccountsApplicationAssemblyReference.Assembly));

        services.AddDbContext<AccountsAppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("MainDb"),
                b => b.MigrationsAssembly(typeof(AccountsAppDbContext).Assembly.FullName)));

        services.AddDbContext<IdentityAppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("MainDb"),
                b => b.MigrationsAssembly(typeof(IdentityAppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IAccountsUnitOfWork>());
        services.AddScoped<IAccountsUnitOfWork, AccountsEfCoreUnitOfWork>();

        services.AddScoped<IUserProfileRepository, UserProfileEfCoreRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenEfCoreRepository>();

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddSingleton<IDomainExceptionMapper, AccountsDomainExceptionMapper>();
        services.AddSingleton<ITokenHasher, Sha256TokenHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<ICurrentUser, CurrentUser>();

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options
            => options.User.RequireUniqueEmail = true)
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
