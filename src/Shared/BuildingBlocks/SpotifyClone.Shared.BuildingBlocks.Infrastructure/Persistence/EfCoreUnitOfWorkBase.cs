using Microsoft.EntityFrameworkCore;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;

namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence;

public abstract class EfCoreUnitOfWorkBase<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }

    protected EfCoreUnitOfWorkBase(TDbContext dbContext)
        => DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await DbContext.SaveChangesAsync(cancellationToken);
}
