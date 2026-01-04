namespace SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

public sealed class ConcurrencyConflictApplicationException : ApplicationExceptionBase
{
    public ConcurrencyConflictApplicationException()
        : base("A concurrency conflict occured.")
    {
    }
}
