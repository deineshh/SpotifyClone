
namespace SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

public sealed class EmailSendFailedApplicationException : ApplicationExceptionBase
{
    public EmailSendFailedApplicationException(string message, Exception innerException)
            : base(message, innerException)
    {
    }
}
