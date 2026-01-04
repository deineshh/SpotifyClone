using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests;

public sealed class DependencyTests
{
    private const string DomainLayerName = "SpotifyClone.Shared.BuildingBlocks.Domain";
    private const string ApplicationLayerName = "SpotifyClone.Shared.BuildingBlocks.Application";
    private const string InfrastructureLayerName = "SpotifyClone.Shared.BuildingBlocks.Infrastructure";

    // This test is commented out FOR NOW because the architecture assembly will have dependencies on all layers
    //      only if it will reference them directly. Will be uncommented if all layers will be referenced directly.

    //[Fact]
    //public void ArchitectureTests_Should_HaveDependenciesOnAllLayers()
    //{
    //    // Arrange
    //    var archAssembly = Assembly.GetExecutingAssembly();
    //    string[] expectedDependencies = new[]
    //    {
    //        DomainLayerName,
    //        ApplicationLayerName,
    //        InfrastructureLayerName
    //    };

    //    // Act
    //    TestResult result = Types.InAssembly(archAssembly)
    //        .Should()
    //        .HaveDependencyOnAll(expectedDependencies)
    //        .GetResult();

    //    // Assert
    //    result.IsSuccessful.Should().BeTrue();
    //}

    [Fact]
    public void DomainLayer_Should_Not_HaveDependenciesOnOtherLayers()
    {
        // Arrange
        var domainAssembly = Assembly.Load(DomainLayerName);
        string[] forbiddenDependencies = new[]
        {
            ApplicationLayerName,
            InfrastructureLayerName
        };

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOnAll(forbiddenDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    // This test is commented out FOR NOW because the Application may not have direct dependencies on the Domain

    //[Fact]
    //public void ApplicationLayer_Should_HaveDependencyOnDomainLayer()
    //{
    //    // Arrange
    //    var applicationAssembly = Assembly.Load(ApplicationLayerName);
    //    string[] expectedDependencies = new[]
    //    {
    //        DomainLayerName
    //    };

    //    // Act
    //    TestResult result = Types.InAssembly(applicationAssembly)
    //        .Should()
    //        .HaveDependencyOnAll(expectedDependencies)
    //        .GetResult();

    //    // Assert
    //    result.IsSuccessful.Should().BeTrue();
    //}

    [Fact]
    public void ApplicationLayer_Should_NotHaveDependencyOnInfrastructureLayer()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationLayerName);
        string[] forbiddenDependencies = new[]
        {
            InfrastructureLayerName
        };

        // Act
        TestResult result = Types.InAssembly(applicationAssembly)
            .Should()
            .NotHaveDependencyOnAll(forbiddenDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_Should_HaveDependencyOnDomainLayer()
    {
        // Arrange
        var infrastructureAssembly = Assembly.Load(InfrastructureLayerName);
        string[] expectedDependencies = new[]
        {
            DomainLayerName
        };

        // Act
        TestResult result = Types.InAssembly(infrastructureAssembly)
            .Should()
            .HaveDependencyOnAll(expectedDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_Should_HaveDependencyOnApplicationLayer()
    {
        // Arrange
        var infrastructureAssembly = Assembly.Load(InfrastructureLayerName);
        string[] expectedDependencies = new[]
        {
            ApplicationLayerName
        };

        // Act
        TestResult result = Types.InAssembly(infrastructureAssembly)
            .Should()
            .HaveDependencyOnAll(expectedDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_Should_Not_HaveDependencyOnDomainLayer()
    {
        // Arrange
        var infrastructureAssembly = Assembly.Load(InfrastructureLayerName);
        string[] forbiddenDependencies = new[]
        {
            DomainLayerName
        };

        // Act
        TestResult result = Types.InAssembly(infrastructureAssembly)
            .Should()
            .NotHaveDependencyOnAll(forbiddenDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
