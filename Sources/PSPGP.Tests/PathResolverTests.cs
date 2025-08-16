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
    /// <summary>
    /// Validates that passing a null cmdlet results
    /// in an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Resolve_WithNullCmdlet_ShouldThrowArgumentNull() {
        // Arrange
        var testPath = @"test.txt";

        // Act
        var action = () => PathResolver.Resolve(null, testPath);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("cmdlet");
    }

    /// <summary>
    /// Ensures the Resolve method exists with the
    /// expected signature and return type.
    /// </summary>
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

    /// <summary>
    /// Confirms the PathResolver type is implemented as
    /// a public static class.
    /// </summary>
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