using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

/// <summary>
/// <para>Returns information about a PGP key such as algorithm, expiration and user IDs.</para>
/// <example>
/// <code>
/// Get-PGPKeyInfo -FilePath $PSScriptRoot\Keys\PublicPGP.asc
/// </code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "PGPKeyInfo")]
[OutputType(typeof(PGPKeyInfo))]
public class CmdletGetPGPKeyInfo : PSCmdlet {
    /// <summary>Paths to key files to inspect.</summary>
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    public string[] FilePath { get; set; }

    protected override void ProcessRecord() {
        foreach (var path in FilePath) {
            try {
                string resolved = PathResolver.Resolve(this, path);
                if (!File.Exists(resolved)) {
                    ErrorActionHelper.WriteErrorOrWarning(
                        this,
                        new FileNotFoundException($"Key file doesn't exist {resolved}"),
                        "KeyFileNotFound",
                        ErrorCategory.InvalidArgument,
                        resolved,
                        $"Key file doesn't exist {resolved}");
                    continue;
                }

                using FileStream keyStream = File.OpenRead(resolved);
                using Stream decoderStream = PgpUtilities.GetDecoderStream(keyStream);
                PgpObjectFactory factory = new(decoderStream);
                PgpPublicKey publicKey = null;

                object pgpObject = factory.NextPgpObject();
                switch (pgpObject) {
                    case PgpPublicKeyRing publicRing:
                        publicKey = publicRing.GetPublicKey();
                        break;
                    case PgpSecretKeyRing secretRing:
                        publicKey = secretRing.GetSecretKey().PublicKey;
                        break;
                    case PgpPublicKey key:
                        publicKey = key;
                        break;
                }

                if (publicKey is null) {
                    keyStream.Position = 0;
                    using Stream decoderStream2 = PgpUtilities.GetDecoderStream(keyStream);
                    try {
                        var bundle = new PgpPublicKeyRingBundle(decoderStream2);
                        foreach (PgpPublicKeyRing ring in bundle.GetKeyRings()) {
                            publicKey = ring.GetPublicKey();
                            break;
                        }
                    } catch {
                        keyStream.Position = 0;
                        using Stream decoderStream3 = PgpUtilities.GetDecoderStream(keyStream);
                        var bundle = new PgpSecretKeyRingBundle(decoderStream3);
                        foreach (PgpSecretKeyRing ring in bundle.GetKeyRings()) {
                            publicKey = ring.GetSecretKey().PublicKey;
                            break;
                        }
                    }
                }

                if (publicKey != null) {
                    var userIds = new List<string>();
                    foreach (string id in publicKey.GetUserIds()) {
                        userIds.Add(id);
                    }
                    DateTime? expiration = publicKey.GetValidSeconds() == 0
                        ? null
                        : publicKey.CreationTime.AddSeconds(publicKey.GetValidSeconds());
                    var info = new PGPKeyInfo {
                        FilePath = resolved,
                        UserIds = userIds.ToArray(),
                        Algorithm = publicKey.Algorithm,
                        Expiration = expiration
                    };
                    WriteObject(info);
                } else {
                    WriteError(new ErrorRecord(new InvalidDataException($"Cannot read key from {resolved}"), "InvalidKey", ErrorCategory.InvalidData, resolved));
                }
            } catch (Exception ex) {
                WriteError(new ErrorRecord(ex, "GetPGPKeyInfoFailed", ErrorCategory.NotSpecified, path));
            }
        }
    }
}
