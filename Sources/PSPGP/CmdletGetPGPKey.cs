using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

[Cmdlet(VerbsCommon.Get, "PGPKey")]
public class CmdletGetPGPKey : PSCmdlet {
    [Parameter(Mandatory = true)]
    public string KeyServer { get; set; }

    [Parameter(Mandatory = true)]
    public string Search { get; set; }

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
