using FluentAssertions;
using SpotifyClone.Shared.BuildingBlocks.Application.Email;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Messaging;

namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.Tests.Messaging;

public sealed class SmtpEmailSenderTests
{
    [Fact]
    public void SendAsync_Should_NotThrowForValidMessage()
    {
        // Arrange
        var sender = new SmtpEmailSender(new SmtpOptions("Host", 1234, false));
        var message = new EmailMessage(["test@example.com"], "Test");

        // Act
        Func<object> act = () => sender.SendAsync(message);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task SendAsync_ShouldHonorCancellation()
    {
        // Arrange
        var sender = new SmtpEmailSender(new SmtpOptions("Host", 1234, false));
        var message = new EmailMessage(["test@example.com"], "Test");

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        Func<Task> act = () => sender.SendAsync(message, cancellationToken: cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
