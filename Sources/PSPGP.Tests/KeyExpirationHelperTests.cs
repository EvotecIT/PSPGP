using PgpCore;
using Xunit;

namespace PSPGP.Tests;

public sealed class KeyExpirationHelperTests {
    [Fact]
    public void GetExpiration_CertificateWithoutExpiration_ReturnsNull() {
        using var files = new TestFileHelper();
        string publicPath = files.GetTempFilePath("public.asc");
        string privatePath = files.GetTempFilePath("private.asc");
        using var pgp = new PGP();
        pgp.GenerateKey(new FileInfo(publicPath), new FileInfo(privatePath), "fixture@example.test", "synthetic-passphrase", strength: 2048);

        Assert.Null(KeyExpirationHelper.GetExpiration(publicPath));
    }
}
