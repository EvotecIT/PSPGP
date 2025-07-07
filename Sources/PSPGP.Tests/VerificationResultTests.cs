using FluentAssertions;
using System;
using Xunit;

namespace PSPGP.Tests;

/// <summary>
/// Unit tests for VerificationResult model
/// </summary>
public class VerificationResultTests {
    /// <summary>
    /// Verifies default property values after
    /// constructing <see cref="VerificationResult"/>.
    /// </summary>
    [Fact]
    public void VerificationResult_DefaultConstructor_ShouldInitializeProperties() {
        // Act
        var result = new VerificationResult();

        // Assert
        result.FilePath.Should().BeNull("FilePath should be null by default");
        result.Status.Should().BeFalse("Status should be false by default");
        result.Error.Should().BeNull("Error should be null by default");
        result.Signer.Should().BeNull("Signer should be null by default");
    }

    /// <summary>
    /// Ensures property setters correctly store values
    /// provided to the <see cref="VerificationResult"/> instance.
    /// </summary>
    [Fact]
    public void VerificationResult_WithValues_ShouldSetProperties() {
        // Arrange
        var filePath = @"C:\test\file.pgp";
        var status = true;
        var error = "Test error message";
        var signer = "signer.asc";

        // Act
        var result = new VerificationResult {
            FilePath = filePath,
            Status = status,
            Error = error,
            Signer = signer
        };

        // Assert
        result.FilePath.Should().Be(filePath, "FilePath should be set correctly");
        result.Status.Should().Be(status, "Status should be set correctly");
        result.Error.Should().Be(error, "Error should be set correctly");
        result.Signer.Should().Be(signer, "Signer should be set correctly");
    }

    /// <summary>
    /// Checks that a result with <c>Status</c> set to
    /// <c>true</c> is considered successful.
    /// </summary>
    [Fact]
    public void VerificationResult_StatusTrue_ShouldIndicateSuccess() {
        // Arrange & Act
        var result = new VerificationResult {
            FilePath = "test.pgp",
            Status = true,
            Error = null
        };

        // Assert
        result.Status.Should().BeTrue("Status should indicate successful verification");
        result.Error.Should().BeNull("Error should be null for successful verification");
    }

    /// <summary>
    /// Checks that a result with <c>Status</c> set to
    /// <c>false</c> exposes the associated error.
    /// </summary>
    [Fact]
    public void VerificationResult_StatusFalse_ShouldIndicateFailure() {
        // Arrange & Act
        var result = new VerificationResult {
            FilePath = "test.pgp",
            Status = false,
            Error = "Verification failed"
        };

        // Assert
        result.Status.Should().BeFalse("Status should indicate failed verification");
        result.Error.Should().NotBeNull("Error should contain failure information");
    }
}