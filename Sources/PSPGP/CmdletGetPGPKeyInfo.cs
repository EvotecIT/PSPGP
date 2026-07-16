using Org.BouncyCastle.Bcpg;
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

    /// <summary>
    /// Processes each provided key file and emits
    /// <see cref="PGPKeyInfo"/> objects describing the contents.
    /// </summary>
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

                using Stream keyStream = KeyMaterialHelper.OpenRead(resolved);
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
                        KeyId = $"0x{unchecked((ulong)publicKey.KeyId):X16}",
                        Fingerprint = BitConverter.ToString(publicKey.GetFingerprint()).Replace("-", string.Empty),
                        UserIds = userIds.ToArray(),
                        Algorithm = publicKey.Algorithm,
                        BitStrength = publicKey.BitStrength,
                        CreationTime = publicKey.CreationTime,
                        Expiration = expiration,
                        IsMasterKey = publicKey.IsMasterKey,
                        IsEncryptionKey = publicKey.IsEncryptionKey,
                        IsRevoked = publicKey.IsRevoked()
                    };
                    WriteObject(info);
                } else {
                    WriteError(new ErrorRecord(new InvalidDataException($"Cannot read key from {resolved}"), "InvalidKey", ErrorCategory.InvalidData, resolved));
                }
            } catch (Exception ex) {
                WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "GetPGPKeyInfoFailed", path, path));
            }
        }
    }
}
