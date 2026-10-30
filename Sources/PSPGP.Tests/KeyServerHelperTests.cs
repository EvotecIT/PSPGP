using PgpCore;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace PSPGP.Tests;

public sealed class KeyServerHelperTests {
    [Theory]
    [InlineData("http://keys.example.test")]
    [InlineData("https://user:password@keys.example.test")]
    [InlineData("https://keys.example.test?query=value")]
    [InlineData("https://keys.example.test#fragment")]
    public async Task Requests_InvalidServer_RejectBeforeDispatch(string address) {
        using var handler = new Handler(_ => throw new InvalidOperationException("No HTTP dispatch expected."));
        using var client = new HttpClient(handler);
        await Assert.ThrowsAsync<ArgumentException>(() => KeyServerHelper.DownloadKeyAsync(new Uri(address), "test", null, default, client));
        await Assert.ThrowsAsync<ArgumentException>(() => KeyServerHelper.UploadKeyAsync(new Uri(address), "unused", default, client));
        Assert.Equal(0, handler.Requests);
    }

    [Fact]
    public async Task Download_PublicCertificate_NormalizesAndChecksTrustedFingerprint() {
        using var files = new TestFileHelper();
        string publicPath = files.GetTempFilePath("public.asc");
        string privatePath = files.GetTempFilePath("private.asc");
        using var pgp = new PGP();
        pgp.GenerateKey(new FileInfo(publicPath), new FileInfo(privatePath), "fixture@example.test", "synthetic-passphrase", strength: 2048);
        string certificate = File.ReadAllText(publicPath);
        using var key = new MemoryStream(Encoding.ASCII.GetBytes(certificate));
        string fingerprint = Assert.Single(PGP.InspectKeys(key), item => item.IsMasterKey).Fingerprint;
        using var handler = new Handler(request => {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/base/pks/lookup?op=get&search=user%2Btag%40example.test", request.RequestUri.PathAndQuery);
            Assert.Null(request.Headers.Authorization);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(certificate) };
        });
        using var client = new HttpClient(handler);
        string result = await KeyServerHelper.DownloadKeyAsync(new Uri("https://keys.example.test/base"), "user+tag@example.test", fingerprint, default, client);
        Assert.Contains("BEGIN PGP PUBLIC KEY BLOCK", result);
        using var normalized = new MemoryStream(Encoding.ASCII.GetBytes(result));
        Assert.Equal(fingerprint, Assert.Single(PGP.InspectKeys(normalized), item => item.IsMasterKey).Fingerprint);
        await Assert.ThrowsAsync<InvalidKeyMaterialException>(() => KeyServerHelper.DownloadKeyAsync(new Uri("https://keys.example.test/base"), "user+tag@example.test", new string('0', fingerprint.Length), default, client));
        Assert.Equal(2, handler.Requests);

        using var uploadHandler = new Handler(request => {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/base/pks/add", request.RequestUri.AbsolutePath);
            Assert.Equal("application/x-www-form-urlencoded", request.Content.Headers.ContentType.MediaType);
            string body = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.StartsWith("keytext=", body);
            Assert.Contains("BEGIN PGP PUBLIC KEY BLOCK", Uri.UnescapeDataString(body));
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new UnreadableContent() };
        });
        using var uploadClient = new HttpClient(uploadHandler);
        await KeyServerHelper.UploadKeyAsync(new Uri("https://keys.example.test/base"), certificate, default, uploadClient);
        Assert.Equal(1, uploadHandler.Requests);
    }

    [Fact]
    public async Task Download_OversizedOrFailedResponse_RejectsBeforeCertificateParsing() {
        using var handler = new Handler(_ => {
            var content = new ByteArrayContent(Array.Empty<byte>());
            content.Headers.ContentLength = 8 * 1024 * 1024 + 1;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        });
        using var client = new HttpClient(handler);
        await Assert.ThrowsAsync<InvalidDataException>(() => KeyServerHelper.DownloadKeyAsync(new Uri("https://keys.example.test"), "test", null, default, client));
        using var failedHandler = new Handler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden));
        using var failedClient = new HttpClient(failedHandler);
        await Assert.ThrowsAsync<HttpRequestException>(() => KeyServerHelper.DownloadKeyAsync(new Uri("https://keys.example.test"), "test", null, default, failedClient));
        Assert.Equal(1, handler.Requests);
        Assert.Equal(1, failedHandler.Requests);
    }

    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler {
        internal int Requests { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            Requests++;
            return Task.FromResult(respond(request));
        }
    }

    private sealed class UnreadableContent : HttpContent {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            throw new InvalidOperationException("The upload response body must not be read.");
        protected override bool TryComputeLength(out long length) { length = 0; return false; }
    }
}
