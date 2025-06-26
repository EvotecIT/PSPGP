using FluentAssertions;
using PSPGP.Models;
using System;
using Xunit;

namespace PSPGP.Tests.Models;

/// <summary>
/// Unit tests for VerificationResult model
/// </summary>
public class VerificationResultTests {
    [Fact]
    public void VerificationResult_DefaultConstructor_ShouldInitializeProperties() {
        // Act
        var result = new VerificationResult();

        // Assert
        result.FilePath.Should().BeNull("FilePath should be null by default");
        result.Status.Should().BeFalse("Status should be false by default");
        result.Error.Should().BeNull("Error should be null by default");
    }

    [Fact]
    public void VerificationResult_WithValues_ShouldSetProperties() {
        // Arrange
        var filePath = @"C:\test\file.pgp";
        var status = true;
        var error = "Test error message";

        // Act
        var result = new VerificationResult {
            FilePath = filePath,
            Status = status,
            Error = error
        };

        // Assert
        result.FilePath.Should().Be(filePath, "FilePath should be set correctly");
        result.Status.Should().Be(status, "Status should be set correctly");
        result.Error.Should().Be(error, "Error should be set correctly");
    }

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