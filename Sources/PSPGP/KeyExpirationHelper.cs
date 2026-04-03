using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

/// <summary>
/// Provides helpers for reading PGP key expiration
/// information and reporting expiration warnings.
/// </summary>
internal static class KeyExpirationHelper {
    /// <summary>
    /// Reads the expiration date from the specified PGP key file.
    /// </summary>
    /// <param name="filePath">Path to the PGP key.</param>
    /// <returns>The expiration date if available.</returns>
    internal static DateTime? GetExpiration(string filePath) {
        using Stream keyStream = KeyMaterialHelper.OpenRead(filePath);
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

        if (publicKey != null) {
            return publicKey.GetValidSeconds() == 0
                ? null
                : publicKey.CreationTime.AddSeconds(publicKey.GetValidSeconds());
        }

        return null;
    }

    /// <summary>
    /// Emits a warning from the given cmdlet if the key is expired or
    /// expiring soon.
    /// </summary>
    /// <param name="cmdlet">Cmdlet writing the warning.</param>
    /// <param name="filePath">Path to the PGP key.</param>
    /// <param name="expiration">Expiration date of the key.</param>
    internal static void WarnIfExpired(PSCmdlet cmdlet, string filePath, DateTime? expiration) {
        if (expiration.HasValue) {
            DateTime now = DateTime.UtcNow;
            if (expiration.Value <= now) {
                cmdlet.WriteWarning($"PGP key '{filePath}' expired on {expiration.Value:u}.");
            } else if (expiration.Value - now <= TimeSpan.FromDays(30)) {
                cmdlet.WriteWarning($"PGP key '{filePath}' will expire on {expiration.Value:u}.");
            }
        }
    }
}
