using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PSPGP;

public static class KeyServerHelper {
    public static async Task<string> DownloadKeyAsync(Uri serverUri, string search) {
        using HttpClient client = new();
        string url = $"{serverUri.AbsoluteUri.TrimEnd('/')}/pks/lookup?op=get&search={Uri.EscapeDataString(search)}";
        return await client.GetStringAsync(url).ConfigureAwait(false);
    }

    public static async Task UploadKeyAsync(Uri serverUri, string armoredKey) {
        using HttpClient client = new();
        var content = new StringContent($"keytext={Uri.EscapeDataString(armoredKey)}", Encoding.UTF8, "application/x-www-form-urlencoded");
        using HttpResponseMessage response = await client.PostAsync($"{serverUri.AbsoluteUri.TrimEnd('/')}/pks/add", content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}
