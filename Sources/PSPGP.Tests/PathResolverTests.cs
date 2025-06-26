using FluentAssertions;
using System;
using System.IO;
using System.Management.Automation;
using Xunit;


namespace PSPGP.Tests;

/// <summary>
/// Unit tests for PathResolver helper class
/// Note: PathResolver requires a real PSCmdlet with SessionState for full functionality.
/// These tests validate the method signature and basic behavior expectations.
/// Full path resolution testing should be done in PowerShell integration tests.
/// </summary>
public class PathResolverTests {
    [Fact]
    public void Resolve_WithNullCmdlet_ShouldThrowNullReference() {
        // Arrange
        var testPath = @"test.txt";

        // Act & Assert
        var action = () => PathResolver.Resolve(null, testPath);
        action.Should().Throw<NullReferenceException>()
            .WithMessage("Object reference not set to an instance of an object.");
    }

    [Fact]
    public void PathResolver_ClassExists_ShouldHaveCorrectSignature() {
        // Arrange & Act
        var method = typeof(PathResolver).GetMethod("Resolve");

        // Assert
        method.Should().NotBeNull();
        method.IsStatic.Should().BeTrue();
        method.ReturnType.Should().Be(typeof(string));

        var parameters = method.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(PSCmdlet));
        parameters[1].ParameterType.Should().Be(typeof(string));
    }

    [Fact]
    public void PathResolver_IsPublicStaticClass() {
        // Arrange & Act
        var type = typeof(PathResolver);

        // Assert
        type.IsClass.Should().BeTrue();
        type.IsPublic.Should().BeTrue();
        type.IsAbstract.Should().BeTrue(); // Static classes are abstract
        type.IsSealed.Should().BeTrue();   // Static classes are sealed
    }
}