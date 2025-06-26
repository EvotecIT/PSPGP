using FluentAssertions;
using PgpCore;
using System;
using System.IO;
using Xunit;

namespace PSPGP.Tests;

/// <summary>
/// Unit tests for PGPConfigurator helper class
/// </summary>
public class PGPConfiguratorTests {
    [Fact]
    public void Configure_WithAllNullParameters_ShouldNotThrow() {
        // Arrange
        var pgp = new PGP();

        // Act & Assert
        var exception = Record.Exception(() =>
            PGPConfigurator.Configure(pgp, null, null, null, null, null, null));

        exception.Should().BeNull("Configuration with null parameters should not throw");
    }

    [Fact]
    public void Configure_WithValidParameters_ShouldNotThrow() {
        // Arrange
        var pgp = new PGP();

        // Act & Assert
        var exception = Record.Exception(() =>
            PGPConfigurator.Configure(
                pgp,
                Org.BouncyCastle.Bcpg.HashAlgorithmTag.Sha256,
                Org.BouncyCastle.Bcpg.CompressionAlgorithmTag.Zip,
                PgpCore.Enums.PGPFileType.Binary,
                1, // Standard signature type
                Org.BouncyCastle.Bcpg.PublicKeyAlgorithmTag.RsaGeneral,
                Org.BouncyCastle.Bcpg.SymmetricKeyAlgorithmTag.Aes256));

        exception.Should().BeNull("Configuration with valid parameters should not throw");
    }

    [Fact]
    public void Configure_WithNullPGP_ShouldThrow() {
        // Act & Assert
        var action = () => PGPConfigurator.Configure(null,
            Org.BouncyCastle.Bcpg.HashAlgorithmTag.Sha256, null, null, null, null, null);

        action.Should().Throw<NullReferenceException>()
            .WithMessage("Object reference not set to an instance of an object.");
    }
}