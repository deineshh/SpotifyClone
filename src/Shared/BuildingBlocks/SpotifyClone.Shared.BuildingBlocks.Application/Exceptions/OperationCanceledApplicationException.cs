namespace SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

public sealed class OperationCanceledApplicationException : ApplicationExceptionBase
{
    public OperationCanceledApplicationException()
        : base("The operation was canceled.")
    {
    }
}
