using PgpCore;
using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSPGP;

/// <summary>Downloads validated public certificates from an HTTPS key server.</summary>
[Cmdlet(VerbsCommon.Get, "PGPKey", SupportsShouldProcess = true)]
public class CmdletGetPGPKey : AsyncPSCmdlet {
    /// <summary>HTTPS URL of the key server.</summary>
    [Parameter(Mandatory = true)]
    public string KeyServer { get; set; }

    /// <summary>Search string identifying the key; a user ID or short key ID does not establish trust.</summary>
    [Parameter(Mandatory = true)]
    public string Search { get; set; }

    /// <summary>Optional complete primary fingerprint obtained through an independently trusted channel.</summary>
    [Parameter]
    public string ExpectedFingerprint { get; set; }

    /// <summary>Optional destination file, replaced only after certificate validation.</summary>
    [Parameter]
    public string OutFilePath { get; set; }

    /// <summary>Returns the successfully written key file or key-pair files.</summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>Downloads the certificate and optionally saves it after validation.</summary>
    protected override async Task ProcessRecordAsync() {
        try {
            Uri server = new(KeyServer);
            KeyServerHelper.ValidateServer(server);
            string output = string.IsNullOrEmpty(OutFilePath) ? null : PathResolver.Resolve(this, OutFilePath);
            if (!ShouldProcess(output ?? KeyServer, "Download public PGP certificate")) return;
            string data = await KeyServerHelper.DownloadKeyAsync(server, Search, ExpectedFingerprint, CancelToken).ConfigureAwait(false);
            CancelToken.ThrowIfCancellationRequested();
            if (output == null) WriteObject(data);
            else {
                using var input = new MemoryStream(Encoding.ASCII.GetBytes(data));
                PGP.ExportPublicKeys(input, new FileInfo(output), ExpectedFingerprint);
                if (PassThru.IsPresent) WriteObject(new FileInfo(output));
            }
        } catch (OperationCanceledException) when (CancelToken.IsCancellationRequested) { throw; }
        catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
            WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "GetPGPKeyFailed"));
        }
    }
}
