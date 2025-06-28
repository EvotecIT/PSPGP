using Org.BouncyCastle.Bcpg;
using System;

namespace PSPGP;

/// <summary>
/// Information about a PGP key file.
/// </summary>
public class PGPKeyInfo {
    /// <summary>Path to the inspected key file.</summary>
    public string FilePath { get; set; }

    /// <summary>User identifiers embedded in the key.</summary>
    public string[] UserIds { get; set; }

    /// <summary>Algorithm used to create the key.</summary>
    public PublicKeyAlgorithmTag Algorithm { get; set; }

    /// <summary>Expiration date of the key if specified.</summary>
    public DateTime? Expiration { get; set; }
}

