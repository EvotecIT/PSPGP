using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

/// <summary>
/// Downloads a public key from a key server.
/// </summary>
[Cmdlet(VerbsCommon.Get, "PGPKey")]
public class CmdletGetPGPKey : PSCmdlet {
    /// <summary>URL of the key server.</summary>
    [Parameter(Mandatory = true)]
    public string KeyServer { get; set; }

    /// <summary>Search string identifying the key.</summary>
    [Parameter(Mandatory = true)]
    public string Search { get; set; }

    /// <summary>File path where the downloaded key is stored.</summary>
    [Parameter]
    public string OutFilePath { get; set; }

    protected override void ProcessRecord() {
        try {
            Uri serverUri = new(KeyServer);
            string keyData = KeyServerHelper.DownloadKeyAsync(serverUri, Search).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(OutFilePath)) {
                string resolved = PathResolver.Resolve(this, OutFilePath);
                File.WriteAllText(resolved, keyData);
            } else {
                WriteObject(keyData);
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "GetPGPKeyFailed", ErrorCategory.NotSpecified, null));
        }
    }
}

