using Org.BouncyCastle.Bcpg;
using System;

namespace PSPGP;

/// <summary>
/// Information about a PGP key file.
/// </summary>
public class PGPKeyInfo {
    /// <summary>Path to the inspected key file.</summary>
    public string FilePath { get; set; }

    /// <summary>OpenPGP key identifier formatted as a 16-digit hexadecimal value.</summary>
    public string KeyId { get; set; }

    /// <summary>Full OpenPGP key fingerprint formatted as hexadecimal.</summary>
    public string Fingerprint { get; set; }

    /// <summary>User identifiers embedded in the key.</summary>
    public string[] UserIds { get; set; }

    /// <summary>Algorithm used to create the key.</summary>
    public PublicKeyAlgorithmTag Algorithm { get; set; }

    /// <summary>Key strength in bits.</summary>
    public int BitStrength { get; set; }

    /// <summary>Date and time when the key was created.</summary>
    public DateTime CreationTime { get; set; }

    /// <summary>Expiration date of the key if specified.</summary>
    public DateTime? Expiration { get; set; }

    /// <summary>Indicates whether this is the master key in its key ring.</summary>
    public bool IsMasterKey { get; set; }

    /// <summary>Indicates whether the key is suitable for encryption.</summary>
    public bool IsEncryptionKey { get; set; }

    /// <summary>Indicates whether the key has been revoked.</summary>
    public bool IsRevoked { get; set; }
}
