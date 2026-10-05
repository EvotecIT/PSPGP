using PgpCore;
using System.Linq;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

/// <summary>
/// <para>Returns information about a PGP key such as algorithm, expiration and user IDs.</para>
/// </summary>
/// <example>
/// <code>
/// Get-PGPKeyInfo -FilePath $PSScriptRoot\Keys\PublicPGP1.asc
/// </code>
/// </example>
[Cmdlet(VerbsCommon.Get, "PGPKeyInfo")]
[OutputType(typeof(PGPKeyInfo))]
public class CmdletGetPGPKeyInfo : PSCmdlet {
    /// <summary>Paths to key files to inspect.</summary>
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    public string[] FilePath { get; set; }

    /// <summary>Emits subkeys as separate records in addition to primary certificates.</summary>
    [Parameter]
    public SwitchParameter IncludeSubkeys { get; set; }

    /// <summary>
    /// Processes each provided key file and emits
    /// <see cref="PGPKeyInfo"/> objects describing the contents.
    /// </summary>
    protected override void ProcessRecord() {
        foreach (var path in FilePath) {
            try {
                string resolved = PathResolver.Resolve(this, path);
                if (!File.Exists(resolved)) {
                    CmdletError.Write(
                        this,
                        new FileNotFoundException($"Key file doesn't exist {resolved}"),
                        "KeyFileNotFound",
                        ErrorCategory.InvalidArgument,
                        resolved);
                    continue;
                }

                using Stream keyStream = KeyMaterialHelper.OpenRead(resolved);
                PGPKeyInfo[] keys = PGP.InspectKeys(keyStream).Select(key => new PGPKeyInfo {
                    FilePath = resolved, KeyId = key.KeyId, Fingerprint = key.Fingerprint,
                    PrimaryFingerprint = key.PrimaryFingerprint, UserIds = key.UserIds,
                    Algorithm = key.Algorithm, BitStrength = key.BitStrength, CreationTime = key.CreationTime,
                    Expiration = key.Expiration, IsMasterKey = key.IsMasterKey, IsEncryptionKey = key.IsEncryptionKey,
                    IsRevoked = key.IsRevoked, CanSign = key.CanSign, CanEncrypt = key.CanEncrypt,
                    IsUsableForSigning = key.IsUsableForSigning, IsUsableForEncryption = key.IsUsableForEncryption
                }).ToArray();
                foreach (var primary in keys.Where(key => key.IsMasterKey)) {
                    primary.Subkeys = keys.Where(key => !key.IsMasterKey && key.PrimaryFingerprint == primary.Fingerprint).ToArray();
                }
                WriteObject(IncludeSubkeys.IsPresent ? keys : keys.Where(key => key.IsMasterKey).ToArray(), true);
            } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
                WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "GetPGPKeyInfoFailed", path, path));
            }
        }
    }
}
