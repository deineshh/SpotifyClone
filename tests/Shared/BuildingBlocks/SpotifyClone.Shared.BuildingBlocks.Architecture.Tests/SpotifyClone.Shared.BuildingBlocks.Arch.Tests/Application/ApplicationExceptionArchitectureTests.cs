using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Application;

public sealed class ApplicationExceptionArchitectureTests
{
    [Fact]
    public void ApplicationExceptionBase_Should_BeAbstractClass()
    {
        // Arrange
        Type applicationExceptionType = typeof(ApplicationExceptionBase);

        // Act
        bool isAbstractClass = applicationExceptionType.IsAbstract && applicationExceptionType.IsClass;

        // Assert
        isAbstractClass.Should().BeTrue();
    }

    [Fact]
    public void ApplicationExceptionBase_Should_InheritFrom_Exception()
    {
        // Arrange
        Type applicationExceptionType = typeof(ApplicationExceptionBase);
        Type exceptionType = typeof(Exception);

        // Act
        bool inheritsFromException = exceptionType.IsAssignableFrom(applicationExceptionType);

        // Assert
        inheritsFromException.Should().BeTrue();
    }

    [Fact]
    public void ApplicationExceptions_Should_BeSealed()
    {
        // Arrange
        Assembly domainAssembly = typeof(ApplicationExceptionBase).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(ApplicationExceptionBase))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationExceptions_Should_HaveApplicationExceptionPostfix()
    {
        // Arrange
        Assembly domainAssembly = typeof(ApplicationExceptionBase).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(ApplicationExceptionBase))
            .Should()
            .HaveNameEndingWith("ApplicationException")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
