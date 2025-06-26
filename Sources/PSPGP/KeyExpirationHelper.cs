using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

internal static class KeyExpirationHelper
{
    internal static DateTime? GetExpiration(string filePath)
    {
        using FileStream keyStream = File.OpenRead(filePath);
        using Stream decoderStream = PgpUtilities.GetDecoderStream(keyStream);
        PgpObjectFactory factory = new(decoderStream);
        PgpPublicKey? publicKey = null;

        object pgpObject = factory.NextPgpObject();
        switch (pgpObject)
        {
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

        if (publicKey != null)
        {
            return publicKey.GetValidSeconds() == 0
                ? null
                : publicKey.CreationTime.AddSeconds(publicKey.GetValidSeconds());
        }

        return null;
    }

    internal static void WarnIfExpired(PSCmdlet cmdlet, string filePath, DateTime? expiration)
    {
        if (expiration.HasValue)
        {
            DateTime now = DateTime.UtcNow;
            if (expiration.Value <= now)
            {
                cmdlet.WriteWarning($"PGP key '{filePath}' expired on {expiration.Value:u}.");
            }
            else if (expiration.Value - now <= TimeSpan.FromDays(30))
            {
                cmdlet.WriteWarning($"PGP key '{filePath}' will expire on {expiration.Value:u}.");
            }
        }
    }
}
