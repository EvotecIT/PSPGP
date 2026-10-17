using PgpCore;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PSPGP;

/// <summary>Bounded HTTPS key-server requests with authenticated certificate parsing.</summary>
public static class KeyServerHelper {
    private const int MaximumKeyBytes = 8 * 1024 * 1024;
    private static readonly HttpClient Client = new(new HttpClientHandler { AllowAutoRedirect = false }) {
        Timeout = TimeSpan.FromSeconds(60)
    };

    /// <summary>Downloads validated, armored public certificates over HTTPS.</summary>
    public static Task<string> DownloadKeyAsync(Uri serverUri, string search) =>
        DownloadKeyAsync(serverUri, search, null, CancellationToken.None);

    /// <summary>Downloads public certificates, optionally matching an independently trusted fingerprint.</summary>
    public static async Task<string> DownloadKeyAsync(Uri serverUri, string search, string expectedFingerprint, CancellationToken cancellationToken) {
        ValidateServer(serverUri);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        cancellationToken = timeout.Token;
        if (string.IsNullOrWhiteSpace(search)) throw new ArgumentException("A key search is required.", nameof(search));
        using var request = new HttpRequestMessage(HttpMethod.Get,
            serverUri.AbsoluteUri.TrimEnd('/') + "/pks/lookup?op=get&search=" + Uri.EscapeDataString(search));
        using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        if (response.Content.Headers.ContentLength > MaximumKeyBytes) throw new InvalidDataException("Key-server response exceeds 8 MiB.");
        using var body = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var data = new MemoryStream();
        byte[] buffer = new byte[16384];
        int read;
        while ((read = await body.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0) {
            if (data.Length + read > MaximumKeyBytes) throw new InvalidDataException("Key-server response exceeds 8 MiB.");
            data.Write(buffer, 0, read);
        }
        cancellationToken.ThrowIfCancellationRequested();
        data.Position = 0;
        using var normalized = new MemoryStream();
        PGP.ExportPublicKeys(data, normalized, expectedFingerprint);
        return Encoding.ASCII.GetString(normalized.ToArray());
    }

    /// <summary>Uploads an armored public certificate over HTTPS.</summary>
    public static Task UploadKeyAsync(Uri serverUri, string armoredKey) =>
        UploadKeyAsync(serverUri, armoredKey, CancellationToken.None);

    /// <summary>Uploads validated public certificates with cancellation.</summary>
    public static async Task UploadKeyAsync(Uri serverUri, string armoredKey, CancellationToken cancellationToken) {
        ValidateServer(serverUri);
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(armoredKey));
        using var normalized = new MemoryStream();
        PGP.ExportPublicKeys(input, normalized);
        using var content = new StringContent("keytext=" + Uri.EscapeDataString(Encoding.ASCII.GetString(normalized.ToArray())),
            Encoding.UTF8, "application/x-www-form-urlencoded");
        using var request = new HttpRequestMessage(HttpMethod.Post, serverUri.AbsoluteUri.TrimEnd('/') + "/pks/add") { Content = content };
        // Only the status is useful. Do not buffer or wait for an untrusted response body.
        using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    internal static void ValidateServer(Uri serverUri) {
        if (serverUri == null || !serverUri.IsAbsoluteUri || serverUri.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(serverUri.UserInfo) || !string.IsNullOrEmpty(serverUri.Query) || !string.IsNullOrEmpty(serverUri.Fragment))
            throw new ArgumentException("Key servers require an absolute HTTPS URL without credentials, query, or fragment.", nameof(serverUri));
    }
}
