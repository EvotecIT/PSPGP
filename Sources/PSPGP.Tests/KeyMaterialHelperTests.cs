using FluentAssertions;
using PgpCore;
using System.IO;
using System.Text;
using Xunit;

namespace PSPGP.Tests;

public class KeyMaterialHelperTests {
    [Fact]
    public void OpenRead_WithUtf8BomArmoredKeys_ShouldSupportEncryptAndDecrypt() {
        using var helper = new TestFileHelper();

        string publicKeyPath = helper.GetTempFilePath("public.asc");
        string privateKeyPath = helper.GetTempFilePath("private.asc");
        string publicKeyWithBomPath = helper.GetTempFilePath("public-bom.asc");
        string privateKeyWithBomPath = helper.GetTempFilePath("private-bom.asc");
        const string password = "P@ssw0rd!";
        const string originalText = "Message encrypted with BOM-normalized keys";

        var pgp = new PGP();
        pgp.GenerateKey(new FileInfo(publicKeyPath), new FileInfo(privateKeyPath), "test@example.com", password);

        File.WriteAllText(publicKeyWithBomPath, File.ReadAllText(publicKeyPath), new UTF8Encoding(true));
        File.WriteAllText(privateKeyWithBomPath, File.ReadAllText(privateKeyPath), new UTF8Encoding(true));

        using Stream publicKeyStream = KeyMaterialHelper.OpenRead(publicKeyWithBomPath);
        var encryptionKeys = new EncryptionKeys(publicKeyStream);
        var encryptPgp = new PGP(encryptionKeys);
        string encrypted = encryptPgp.EncryptArmoredString(originalText);

        using Stream privateKeyStream = KeyMaterialHelper.OpenRead(privateKeyWithBomPath);
        var decryptionKeys = new EncryptionKeys(privateKeyStream, password);
        var decryptPgp = new PGP(decryptionKeys);
        string decrypted = decryptPgp.DecryptArmoredString(encrypted);

        decrypted.Should().Be(originalText);
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
