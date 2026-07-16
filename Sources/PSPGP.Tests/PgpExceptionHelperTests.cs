using FluentAssertions;
using PgpCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Xunit;

namespace PSPGP.Tests;

public class PgpExceptionHelperTests {
    public static IEnumerable<object[]> TypedErrorCategories() {
        yield return new object[] { new IncorrectPassphraseException("wrong passphrase"), ErrorCategory.AuthenticationError };
        yield return new object[] { new MessageIntegrityException("integrity check failed"), ErrorCategory.SecurityError };
        yield return new object[] { new InvalidKeyMaterialException("invalid key"), ErrorCategory.InvalidData };
        yield return new object[] { new NotEncryptedDataException("not encrypted"), ErrorCategory.InvalidData };
        yield return new object[] { new NoDecryptionKeyException("missing key"), ErrorCategory.ObjectNotFound };
        yield return new object[] {
            new InvalidDataException("wrapper", new IncorrectPassphraseException("wrong passphrase")),
            ErrorCategory.AuthenticationError
        };
    }

    [Theory]
    [MemberData(nameof(TypedErrorCategories))]
    public void GetErrorCategory_WithPgpCoreTypedException_ReturnsActionableCategory(
        Exception exception,
        ErrorCategory expectedCategory) {
        PgpExceptionHelper.GetErrorCategory(exception).Should().Be(expectedCategory);
    }

    [Fact]
    public void Normalize_WithTypedInvalidKeyMaterial_AddsKeyPathGuidance() {
        var exception = new InvalidKeyMaterialException("invalid key material");

        Exception normalized = PgpExceptionHelper.Normalize(exception, "invalid.asc");

        normalized.Should().BeOfType<InvalidDataException>();
        normalized.Message.Should().Contain("invalid.asc");
        normalized.InnerException.Should().BeSameAs(exception);
    }

    [Fact]
    public void Normalize_WithPacketType20Error_ShouldReturnAeadGuidance() {
        var exception = new IOException("unknown packet type encountered: 20");

        Exception normalized = PgpExceptionHelper.Normalize(exception);

        normalized.Should().BeOfType<NotSupportedException>();
        normalized.Message.Should().Contain("AEAD");
        normalized.InnerException.Should().BeSameAs(exception);
    }

    [Fact]
    public void Normalize_WithWrappedPacketType20Error_ShouldReturnAeadGuidance() {
        var innerException = new IOException("unknown packet type encountered: 20");
        var exception = new InvalidDataException("encrypted data was not readable", innerException);

        Exception normalized = PgpExceptionHelper.Normalize(exception);

        normalized.Should().BeOfType<NotSupportedException>();
        normalized.Message.Should().Contain("AEAD");
        normalized.InnerException.Should().BeSameAs(exception);
    }
}
