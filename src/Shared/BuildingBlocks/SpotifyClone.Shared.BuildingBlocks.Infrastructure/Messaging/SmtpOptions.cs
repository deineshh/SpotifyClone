namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.Messaging;

public sealed record SmtpOptions(
    string Host,
    int Port,
    bool EnableSsl,
    string? UserName = null,
    string? Password = null,
    string? DefaultFromEmail = null,
    string? DefaultFromName = null)
{
    public const string SectionName = "Smtp";
}
