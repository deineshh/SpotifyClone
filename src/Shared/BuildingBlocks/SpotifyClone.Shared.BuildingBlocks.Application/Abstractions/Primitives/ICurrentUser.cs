namespace SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;

public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
