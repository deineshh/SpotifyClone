using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Identity;

internal sealed class IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
    : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityTables(builder);

        builder.HasDefaultSchema("identity");
    }

    private static void ConfigureIdentityTables(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(u => u.Email)
                .HasMaxLength(320);

            entity.Property(u => u.UserName)
                .HasMaxLength(256);
        });

        builder.Entity<IdentityRole<Guid>>(entity
            => entity.ToTable("Roles"));

        builder.Entity<IdentityUserRole<Guid>>(entity
            => entity.ToTable("UserRoles"));

        builder.Entity<IdentityUserClaim<Guid>>(entity
            => entity.ToTable("UserClaims"));

        builder.Entity<IdentityUserLogin<Guid>>(entity
            => entity.ToTable("UserLogins"));

        builder.Entity<IdentityRoleClaim<Guid>>(entity
            => entity.ToTable("RoleClaims"));

        builder.Entity<IdentityUserToken<Guid>>(entity
            => entity.ToTable("UserTokens"));
    }
}
