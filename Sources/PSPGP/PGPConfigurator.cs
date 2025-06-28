using Org.BouncyCastle.Bcpg;
using PgpCore;

namespace PSPGP;

/// <summary>
/// Applies optional configuration values to a <see cref="PgpCore.PGP"/> instance.
/// </summary>
public static class PGPConfigurator {
    /// <summary>
    /// Sets optional configuration parameters on a <see cref="PgpCore.PGP"/> instance.
    /// Only values that are provided are applied.
    /// </summary>
    /// <param name="pgp">Instance to configure.</param>
    /// <param name="hashAlgorithm">Hash algorithm for operations.</param>
    /// <param name="compressionAlgorithm">Compression algorithm for operations.</param>
    /// <param name="fileType">File type for encryption/decryption.</param>
    /// <param name="signatureType">Signature type when signing.</param>
    /// <param name="publicKeyAlgorithm">Public key algorithm for key creation.</param>
    /// <param name="symmetricKeyAlgorithm">Symmetric key algorithm for encryption.</param>
    public static void Configure(
        PGP pgp,
        HashAlgorithmTag? hashAlgorithm,
        CompressionAlgorithmTag? compressionAlgorithm,
        PgpCore.Enums.PGPFileType? fileType,
        int? signatureType,
        PublicKeyAlgorithmTag? publicKeyAlgorithm,
        SymmetricKeyAlgorithmTag? symmetricKeyAlgorithm) {
        if (hashAlgorithm.HasValue) pgp.HashAlgorithmTag = hashAlgorithm.Value;
        if (compressionAlgorithm.HasValue) pgp.CompressionAlgorithm = compressionAlgorithm.Value;
        if (fileType.HasValue) pgp.FileType = fileType.Value;
        if (signatureType.HasValue) pgp.PgpSignatureType = signatureType.Value;
        if (publicKeyAlgorithm.HasValue) pgp.PublicKeyAlgorithm = publicKeyAlgorithm.Value;
        if (symmetricKeyAlgorithm.HasValue) pgp.SymmetricKeyAlgorithm = symmetricKeyAlgorithm.Value;
    }
}

