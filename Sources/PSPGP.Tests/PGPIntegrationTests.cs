using FluentAssertions;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;
using System;
using System.IO;
using Xunit;

namespace PSPGP.Tests;

/// <summary>
/// Integration tests for PGP operations using the PgpCore library directly
/// </summary>
public class PGPCoreIntegrationTests : IDisposable {
    private readonly string _tempDirectory;

    public PGPCoreIntegrationTests() {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "PSPGP_Integration_Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    /// <summary>
    /// Validates that keys can be created and used to
    /// encrypt and decrypt string content using PgpCore
    /// directly.
    /// </summary>
    [Fact]
    public void PGPCore_CreateKeysEncryptDecrypt_ShouldWork() {
        // Arrange
        var publicKeyPath = Path.Combine(_tempDirectory, "public.asc");
        var privateKeyPath = Path.Combine(_tempDirectory, "private.asc");
        var originalText = "This is an integration test message";
        var username = "integration@test.com";
        var password = "IntegrationTest123!";

        // Act & Assert - Create keys
        var pgp = new PGP();
        var createKeysAction = () => pgp.GenerateKey(
            new FileInfo(publicKeyPath),
            new FileInfo(privateKeyPath),
            username,
            password);

        createKeysAction.Should().NotThrow("Key creation should succeed");
        File.Exists(publicKeyPath).Should().BeTrue("Public key should exist");
        File.Exists(privateKeyPath).Should().BeTrue("Private key should exist");

        PgpPublicKey generatedKey = ReadMasterPublicKey(publicKeyPath);
        generatedKey.BitStrength.Should().Be(3072, "PgpCore v8 generates stronger keys by default");
        generatedKey.IsEncryptionKey.Should().BeTrue();

        // Configure PGP with the generated keys for encryption/decryption
        var encryptionKeys = new EncryptionKeys(new FileInfo(publicKeyPath));
        var encryptPgp = new PGP(encryptionKeys);

        // Act & Assert - Encrypt string
        var encryptAction = () => encryptPgp.EncryptArmoredString(originalText);
        var encryptedString = encryptAction.Should().NotThrow("Encryption should succeed").Subject;

        encryptedString.Should().Contain("-----BEGIN PGP MESSAGE-----");

        // Configure PGP for decryption
        var decryptionKeys = new EncryptionKeys(new FileInfo(privateKeyPath), password);
        var decryptPgp = new PGP(decryptionKeys);

        // Act & Assert - Decrypt string
        var decryptAction = () => decryptPgp.DecryptArmoredString(encryptedString);
        var decryptedText = decryptAction.Should().NotThrow("Decryption should succeed").Subject;

        decryptedText.Should().Be(originalText, "Decrypted text should match original");
    }

    /// <summary>
    /// Exercises file encryption and decryption workflows
    /// with generated keys.
    /// </summary>
    [Fact]
    public void PGPCore_FileEncryptionDecryption_ShouldWork() {
        // Arrange
        var publicKeyPath = Path.Combine(_tempDirectory, "file_public.asc");
        var privateKeyPath = Path.Combine(_tempDirectory, "file_private.asc");
        var sourceFile = Path.Combine(_tempDirectory, "source.txt");
        var encryptedFile = Path.Combine(_tempDirectory, "encrypted.pgp");
        var decryptedFile = Path.Combine(_tempDirectory, "decrypted.txt");

        var originalContent = "This is test file content for integration testing";
        var password = "FileTest123!";

        File.WriteAllText(sourceFile, originalContent);

        // Create keys
        var pgp = new PGP();
        pgp.GenerateKey(
            new FileInfo(publicKeyPath),
            new FileInfo(privateKeyPath),
            "filetest@test.com",
            password);

        // Configure PGP for encryption
        var encryptionKeys = new EncryptionKeys(new FileInfo(publicKeyPath));
        var encryptPgp = new PGP(encryptionKeys);

        // Act - Encrypt file
        var encryptAction = () => encryptPgp.EncryptFile(new FileInfo(sourceFile), new FileInfo(encryptedFile));
        encryptAction.Should().NotThrow("File encryption should succeed");
        File.Exists(encryptedFile).Should().BeTrue("Encrypted file should exist");

        // Configure PGP for decryption
        var decryptionKeys = new EncryptionKeys(new FileInfo(privateKeyPath), password);
        var decryptPgp = new PGP(decryptionKeys);

        // Act - Decrypt file
        var decryptAction = () => decryptPgp.DecryptFile(new FileInfo(encryptedFile), new FileInfo(decryptedFile));
        decryptAction.Should().NotThrow("File decryption should succeed");
        File.Exists(decryptedFile).Should().BeTrue("Decrypted file should exist");

        // Assert
        var decryptedContent = File.ReadAllText(decryptedFile);
        decryptedContent.Should().Be(originalContent, "Decrypted content should match original");
    }

    /// <summary>
    /// Ensures key generation succeeds when passing
    /// custom configuration options.
    /// </summary>
    [Fact]
    public void PGPCore_KeyGeneration_WithCustomParameters_ShouldWork() {
        // Arrange
        var publicKeyPath = Path.Combine(_tempDirectory, "custom_public.asc");
        var privateKeyPath = Path.Combine(_tempDirectory, "custom_private.asc");

        // Act
        var pgp = new PGP();
        var createAction = () => pgp.GenerateKey(
            new FileInfo(publicKeyPath),
            new FileInfo(privateKeyPath),
            "custom@test.com",
            "CustomTest123!",
            1024,  // Key strength
            25,    // Certainty
            true); // Emit version

        // Assert
        createAction.Should().NotThrow("Custom key generation should succeed");
        File.Exists(publicKeyPath).Should().BeTrue("Public key should be created");
        File.Exists(privateKeyPath).Should().BeTrue("Private key should be created");

        new FileInfo(publicKeyPath).Length.Should().BeGreaterThan(0, "Public key should not be empty");
        new FileInfo(privateKeyPath).Length.Should().BeGreaterThan(0, "Private key should not be empty");
    }

    /// <summary>
    /// Confirms decryption fails when using an incorrect
    /// password for the private key.
    /// </summary>
    [Fact]
    public void PGPCore_InvalidPassword_ShouldThrow() {
        // Arrange
        var publicKeyPath = Path.Combine(_tempDirectory, "invalid_public.asc");
        var privateKeyPath = Path.Combine(_tempDirectory, "invalid_private.asc");
        var correctPassword = "CorrectPassword123!";
        var wrongPassword = "WrongPassword123!";

        // Create keys with correct password
        var pgp = new PGP();
        pgp.GenerateKey(
            new FileInfo(publicKeyPath),
            new FileInfo(privateKeyPath),
            "invalid@test.com",
            correctPassword);

        // Encrypt a message using public key
        var originalText = "Test message for wrong password";
        var encryptionKeys = new EncryptionKeys(new FileInfo(publicKeyPath));
        var encryptPgp = new PGP(encryptionKeys);
        var encryptedString = encryptPgp.EncryptArmoredString(originalText);

        // Act & Assert - Try to decrypt with wrong password
        var decryptionKeysWrong = new EncryptionKeys(new FileInfo(privateKeyPath), wrongPassword);
        var decryptPgpWrong = new PGP(decryptionKeysWrong);

        var decryptAction = () => decryptPgpWrong.DecryptArmoredString(encryptedString);

        // Should throw when using wrong password
        decryptAction.Should().Throw<Exception>("Should throw when using wrong password");
    }

    public void Dispose() {
        try {
            if (Directory.Exists(_tempDirectory)) {
                Directory.Delete(_tempDirectory, true);
            }
        } catch {
            // Ignore cleanup errors
        }
    }

    private static PgpPublicKey ReadMasterPublicKey(string path) {
        using Stream stream = File.OpenRead(path);
        var bundle = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(stream));
        foreach (PgpPublicKeyRing ring in bundle.GetKeyRings()) {
            foreach (PgpPublicKey key in ring.GetPublicKeys()) {
                if (key.IsMasterKey) {
                    return key;
                }
            }
        }

        throw new InvalidDataException($"No master public key was found in '{path}'.");
    }
}
