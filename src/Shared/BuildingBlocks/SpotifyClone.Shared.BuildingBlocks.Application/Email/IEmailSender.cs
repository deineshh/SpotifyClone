namespace SpotifyClone.Shared.BuildingBlocks.Application.Email;

public interface IEmailSender
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="to">Primary recipients (To)</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlBody">HTML body (optional if plainTextBody provided)</param>
    /// <param name="plainTextBody">Plain text fallback (optional if htmlBody provided)</param>
    /// <param name="from">Optional sender override (name and email)</param>
    /// <param name="cc">Carbon copy recipients</param>
    /// <param name="bcc">Blind carbon copy recipients</param>
    /// <param name="replyTo">Reply-to address</param>
    /// <param name="attachments">File attachments</param>
    /// <param name="priority">Email priority (normal, high, low)</param>
    /// <returns>Result indicating success/failure (optional, but useful)</returns>
    Task SendAsync(
        EmailMessage message,
        IEnumerable<EmailAttachment>? attachments = null,
        EmailPriority priority = EmailPriority.Normal);
}
