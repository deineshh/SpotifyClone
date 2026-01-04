namespace SpotifyClone.Shared.BuildingBlocks.Application.Email;

public sealed record EmailMessage(
    IEnumerable<string> to,
    string subject,
    string? htmlBody = null,
    string? plainTextBody = null,
    (string Name, string Email)? from = null,
    IEnumerable<string>? cc = null,
    IEnumerable<string>? bcc = null,
    string? replyTo = null);
