namespace SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
