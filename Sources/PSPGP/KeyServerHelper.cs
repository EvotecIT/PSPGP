using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PSPGP;

/// <summary>
/// Provides helper methods for interacting with PGP key servers.
/// </summary>
public static class KeyServerHelper {
    /// <summary>
    /// Downloads a public key from the specified key server.
    /// </summary>
    /// <param name="serverUri">Server from which to download the key.</param>
    /// <param name="search">Search string identifying the key.</param>
    /// <returns>The armored key text.</returns>
    public static async Task<string> DownloadKeyAsync(Uri serverUri, string search) {
        using HttpClient client = new();
        string url = $"{serverUri.AbsoluteUri.TrimEnd('/')}/pks/lookup?op=get&search={Uri.EscapeDataString(search)}";
        return await client.GetStringAsync(url).ConfigureAwait(false);
    }

    /// <summary>
    /// Uploads a public key to the specified key server.
    /// </summary>
    /// <param name="serverUri">Destination key server.</param>
    /// <param name="armoredKey">Armored public key text.</param>
    /// <returns>A task representing the asynchronous upload operation.</returns>
    public static async Task UploadKeyAsync(Uri serverUri, string armoredKey) {
        using HttpClient client = new();
        var content = new StringContent($"keytext={Uri.EscapeDataString(armoredKey)}", Encoding.UTF8, "application/x-www-form-urlencoded");
        using HttpResponseMessage response = await client.PostAsync($"{serverUri.AbsoluteUri.TrimEnd('/')}/pks/add", content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}

