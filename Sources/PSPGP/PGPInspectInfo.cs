using System.Collections.Generic;
using Org.BouncyCastle.Bcpg;

namespace PSPGP;

/// <summary>
/// Represents the metadata extracted from a PGP message.
/// </summary>
public class PGPInspectInfo {
    /// <summary>Path of the inspected input when applicable.</summary>
    public string SourcePath { get; set; }

    /// <summary>Indicates whether the message is ASCII armored.</summary>
    public bool IsArmored { get; set; }

    /// <summary>Headers found in the armored message.</summary>
    public Dictionary<string, string> MessageHeaders { get; set; }

    /// <summary>Version header value when present.</summary>
    public string Version { get; set; }

    /// <summary>Comment header value when present.</summary>
    public string Comment { get; set; }

    /// <summary>Indicates whether the message is compressed.</summary>
    public bool IsCompressed { get; set; }

    /// <summary>Indicates whether the message is encrypted.</summary>
    public bool IsEncrypted { get; set; }

    /// <summary>Recipient key IDs found in encrypted content.</summary>
    public string[] RecipientKeyIds { get; set; }

    /// <summary>Indicates whether the message is integrity protected.</summary>
    public bool IsIntegrityProtected { get; set; }

    /// <summary>Indicates whether the message is signed.</summary>
    public bool IsSigned { get; set; }

    /// <summary>Symmetric algorithm reported by the message.</summary>
    public SymmetricKeyAlgorithmTag SymmetricKeyAlgorithm { get; set; }

    /// <summary>Embedded file name reported by the message.</summary>
    public string FileName { get; set; }

    /// <summary>Embedded modification time reported by the message.</summary>
    public System.DateTime ModificationDateTime { get; set; }
}
