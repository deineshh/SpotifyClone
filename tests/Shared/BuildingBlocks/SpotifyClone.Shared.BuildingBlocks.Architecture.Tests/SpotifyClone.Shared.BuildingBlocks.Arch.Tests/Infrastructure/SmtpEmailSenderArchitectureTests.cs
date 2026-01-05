using System.Reflection;
using SpotifyClone.Shared.BuildingBlocks.Application.Email;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Messaging;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Infrastructure;

public sealed class SmtpEmailSenderArchitectureTests
{
    [Fact]
    public void SmtpEmailSender_Should_ImplementIEmailSender()
    {
        // Arrange
        Type smtpEmailSenderType = typeof(SmtpEmailSender);
        Type iEmailSenderType = typeof(IEmailSender);

        // Act & Assert
        Assert.True(iEmailSenderType.IsAssignableFrom(smtpEmailSenderType));
    }

    [Fact]
    public void SmtpEmailSender_Should_HaveConstructorWithSmtpOptionsParameter()
    {
        // Arrange
        Type smtpEmailSenderType = typeof(SmtpEmailSender);
        Type smtpOptionsType = typeof(SmtpOptions);

        // Act
        ConstructorInfo? constructor = smtpEmailSenderType.GetConstructor(new[] { smtpOptionsType });

        // Assert
        Assert.NotNull(constructor);
    }

    [Fact]
    public void SmtpEmailSender_Should_BeSealed()
    {
        // Arrange & Act
        Type smtpEmailSenderType = typeof(SmtpEmailSender);

        // Act
        bool isSealed = smtpEmailSenderType.IsSealed;

        // Assert
        Assert.True(isSealed);
    }
}
